using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;


namespace OptimizacionPortafolio
{
    class Program
    {
        private static double[][] DatosHistoricos = new double[][] {
                new double[] { 0.014, 0.035, 0.030, 0.018, 0.023, 0.027, 0.035, 0.036, -0.009, -0.013, 0.026, 0.042 },
                new double[] { 0.032, 0.055, -0.036, 0.052, 0.047, 0.034, 0.063, 0.048, 0.025, 0.040, 0.036, -0.017 },
                new double[] { 0.054, 0.056, 0.048, -0.007, 0.053, 0.036, 0.017, 0.047, 0.019, 0.017, 0.040, 0.032 },
                new double[] { 0.038, 0.062, -0.037, 0.050, 0.065, -0.043, 0.062, 0.034, 0.035, 0.056, 0.057, 0.025 },
                new double[] { 0.049, 0.067, -0.039, 0.051, 0.049, 0.037, 0.055, 0.025, 0.052, 0.020, 0.045, 0.040  }
        };

        private static string[] Companias = new String[] { "Google", "Apple", "Microsoft", "IBM", "Facebook" };

        private static String[] Meses = new String[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        double[] media;
        double[,] covarianza;
        static void Main(string[] args)
        {
            try
            {
                //markowitz 
                DataTable tbDatos = Program.DatosTabulados();
                DataTable tbExpectativas=Program.expectativas(0.02, 0.0025, 8);
                
                Program p = new Program();
                p.ConstruirCovarianza(tbDatos);
                p.ModeloRiesgo(tbExpectativas, 8);

                imprimirTable(tbDatos);
                Console.WriteLine();
                Console.WriteLine();
                imprimirTable(tbExpectativas);

                Console.ReadLine();
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }

        private static void imprimirTable(DataTable t)
        {
            foreach (DataColumn c in t.Columns)
                Console.Write(c.ColumnName + "|");
            Console.WriteLine("");
            foreach(DataRow f in t.Rows)
            { 
                for(int i=0;i< f.ItemArray.Length;i++)
                    Console.Write(f.ItemArray[i] + "|");
                Console.WriteLine("");
            }
                
        }

        private static DataTable DatosTabulados()
        {
            DataTable t = new DataTable();
            t.Columns.Add(new DataColumn("Mes", typeof(string)));

            foreach (string nombre in Companias)
                t.Columns.Add(new DataColumn(nombre, typeof(double)));

            for (int i = 0; i < Meses.Length; i++)
            {
                DataRow row = t.NewRow();
                row["Mes"] = Meses[i];
                for (int j = 0; j < Companias.Length; j++)
                    row[Companias[j]] = DatosHistoricos[j][i];
                
                t.Rows.Add(row);
            }
            return t;
        }

        public void ConstruirCovarianza(DataTable data)
        {

            int m = Companias.Length;
            int n = Meses.Length;

            media = new double[m];
            covarianza = new double[m, m];
            DataRow row = data.NewRow();
            row["Mes"] = "(Promedio)";
            for (int invest = m; 0 <= --invest; )
            {
                double sum = 0;
                for (int t = n; 0 <= --t; )
                    sum += (double)data.Rows[t][Companias[invest]];
                media[invest] = sum / n;
                row[Companias[invest]] = media[invest];
            }

            for (int invest = m; 0 <= --invest; )
            {
                for (int jnvest = m; 0 <= --jnvest; )
                {
                    double crossCor = 0;
                    for (int t = n; 0 <= --t; )
                        crossCor += (double)data.Rows[t][Companias[invest]] * (double)data.Rows[t][Companias[jnvest]];

                    covarianza[invest, jnvest] = crossCor / n - media[invest] * media[jnvest];
                }
            }
            data.Rows.Add(row);
        }

        public bool ModeloRiesgo(DataTable plan, int iterations)
        {

            int m = Companias.Length;

            for (int reqIx = 0; reqIx < iterations; reqIx++)
            {
                InteriorPointSolver solver = new InteriorPointSolver();

                int[] asignaciones = new int[m];

                for (int i = 0; i < m; i++)
                {
                    solver.AddVariable(Companias[i], out asignaciones[i]);
                    solver.SetBounds(asignaciones[i], 0, 1);
                }
                                
                int rentabilidad;
                solver.AddRow("rentabilidad", out rentabilidad);

                // La rentabilidad debe superar el minimo pedido

                solver.SetBounds(rentabilidad, (double)plan.Rows[reqIx]["minimum"], double.PositiveInfinity);

                int unity;
                solver.AddRow("Invertir la suma a", out unity);
                solver.SetBounds(unity, 1, 1);

                // El rendimiento esperado es una combinacion lineal ponderada de las inversiones
                // unity = suma de inversiones

                for (int invest = m; 0 <= --invest; )
                {
                    solver.SetCoefficient(rentabilidad, asignaciones[invest], media[invest]);
                    solver.SetCoefficient(unity, asignaciones[invest], 1);
                }

                // The variance of the result is a quadratic combination of the covariants and allocations.

                int varianza;
                solver.AddRow("varianza", out varianza);
                for (int invest = m; 0 <= --invest; )
                {
                    for (int jnvest = m; 0 <= --jnvest; )
                    {
                        solver.SetCoefficient(varianza, covarianza[invest, jnvest], asignaciones[invest], asignaciones[jnvest]);
                    }
                }

                // the goal is to minimize the variance, given the linear lower bound on asked return.

                solver.AddGoal(varianza, 0, true);

                InteriorPointSolverParams lpParams = new InteriorPointSolverParams();

                solver.Solve(lpParams);
                if (solver.Result != LinearResult.Optimal)
                    return false;

                for (int invest = m; 0 <= --invest; )
                {
                    plan.Rows[reqIx][Companias[invest]] = (double)solver.GetValue(asignaciones[invest]);
                }
                plan.Rows[reqIx]["actual"] = (double)solver.GetValue(rentabilidad);
                plan.Rows[reqIx]["Std.Dev."] = Math.Sqrt((double)solver.Statistics.Primal);
            }
            return true;
        }

        internal static DataTable expectativas(double minimum, double increment, double iterations)
        {
            DataTable table = new DataTable();

            table.Columns.Add(new DataColumn("minimum", typeof(double)));
            table.Columns.Add(new DataColumn("actual", typeof(double)));
            table.Columns.Add(new DataColumn("Std.Dev.", typeof(double)));

            foreach (string name in Companias)
                table.Columns.Add(new DataColumn(name, typeof(double)));
            for (int rowIndex = 0; rowIndex < iterations; rowIndex++){
                DataRow row = table.NewRow();
                row["minimum"] = minimum + rowIndex * increment;
                row["actual"] = 0;
                row["Std.Dev."] = 0;
                for (int col = 0; col < Companias.Length; col++)
                {
                    row[Companias[col]] = 0;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }

}

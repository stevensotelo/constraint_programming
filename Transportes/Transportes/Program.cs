using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportes
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Creación de espacio de trabajo
                SolverContext solver = new SolverContext();
                Model model = solver.CreateModel();

                // Matriz de costo de envios
                int castigo=int.MaxValue;
                int[,] costos=new int[,]{{20,19,14,21,16},{15,20,13,19,16},{18,15,18,20,castigo}};

                // Creación de variables de toma de decisiones
                Set setX = new Set(Domain.Integer, "x");
                Decision x = new Decision(Domain.Integer, "xDecision", setX);                
                model.AddDecision(x);
                
                // Parametros
                Set disponibilidad = new Set(Domain.Integer,"disponibilidad");
                Parameter pFabrica = new Parameter(Domain.Integer, "pDisponibilidad", disponibilidad);
                disponibilidad.SetBinding(new List<int>() { 40, 60, 70 });
                Set requerimiento = new Set(Domain.Integer, "requerimientos");
                Parameter pRequerimiento = new Parameter(Domain.Integer, "pRequerimientos", requerimiento);
                requerimiento.SetBinding(new List<int>() { 30, 40, 50, 40, 60 });
                
                // Restriciones
                model.AddConstraints("disponibilidad_fabrica" + Model.ForEach(disponibilidad, d => Model.Sum(Model.ForEach(requerimiento, s => x[s,d])) == disponibilidad[d]);

                // Objetivo
                //Model.ForEach(x, i => x[i] * costos[int.Parse(x[i].Name[1]), int.Parse(x[i].Name[2])]);
                model.AddGoal("minimo",GoalKind.Minimize,Model.Sum(x));

                Solution solucion = solver.Solve(new SimplexDirective());
                Report reporte = solucion.GetReport();
                Console.Write(reporte);

                Console.ReadLine();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        public static List<Decision> crearVariables(string titulo)
        {
            List<Decision> vars = new List<Decision>();
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 5; j++)
                    vars.Add(new Decision(Domain.Integer, titulo + i.ToString() + j.ToString()));
            }
            return vars;
        }
    }
}

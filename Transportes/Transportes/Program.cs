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
                int castigo = int.MaxValue;
                                                
                // Parametros
                Set disponibilidad = new Set(Domain.Any, "disponibilidad");
                Parameter pDisponibilidad = new Parameter(Domain.Integer, "pDisponibilidad", disponibilidad);
                Set requerimiento = new Set(Domain.Any, "requerimientos");
                Parameter pRequerimiento = new Parameter(Domain.Integer, "pRequerimientos", requerimiento);
                Set costos = new Set(Domain.Any, "costos");
                Parameter pCostos = new Parameter(Domain.Integer, "pCostos", disponibilidad, requerimiento);
                                
                // Creación de variables de toma de decisiones
                Set setX = new Set(Domain.Any, "x");
                Decision x = new Decision(Domain.Integer, "xDecision", disponibilidad,requerimiento);
                model.AddDecision(x);

                // Binding Parameters
                disponibilidad.SetBinding(new List<int>() { 40, 60, 70 });
                requerimiento.SetBinding(new List<int>() { 30, 40, 50, 40, 60 });
                costos.SetBinding((new int[,] { { 20, 19, 14, 21, 16 }, { 15, 20, 13, 19, 16 }, { 18, 15, 18, 20, castigo } }).Cast<int>().ToArray());

                // Agregar parametros al modelo
                model.AddParameter(pDisponibilidad);
                model.AddParameter(pRequerimiento);
                model.AddParameter(pCostos);                

                // Restriciones
                model.AddConstraints("rDisponibilidad", Model.ForEach(disponibilidad, d => Model.Sum(Model.ForEach(requerimiento, r => x[r, d])) <= pDisponibilidad[d]));
                model.AddConstraints("rRequerimientos", Model.ForEach(requerimiento, r => Model.Sum(Model.ForEach(disponibilidad, d => x[r, d])) <= pRequerimiento[r]));

                // Objetivo
                model.AddGoal("minimo", GoalKind.Minimize,Model.Sum(Model.ForEach(disponibilidad, d => Model.Sum(Model.ForEach(requerimiento, r => x[r, d]*pCostos[r,d])))));

                Solution solucion = solver.Solve();
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

using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemasSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SolverContext context = new SolverContext();
                Model model = context.CreateModel();
                // Creación de variables
                Decision x1 = new Decision(Domain.Integer, "x1");
                model.AddDecision(x1);
                Decision x2 = new Decision(Domain.Integer, "x2");
                model.AddDecision(x2);
                Decision x3 = new Decision(Domain.Integer, "x3");
                model.AddDecision(x3);
                // Creación de limites
                model.AddConstraint("limitX1", 50 <= x1 <= 300);
                model.AddConstraint("limitX2", 100 <= x2 <= 200);
                model.AddConstraint("limitX3", 20 <= x3 <= 1000);
                // Creación de restricciones
                model.AddConstraint("restriccion1", 200 <= (x1 + x2 + x3) <= 280);
                model.AddConstraint("restriccion2", 100 <= (x1 + (3 * x3)) <= 2000);
                model.AddConstraint("restriccion3", 50 <= ((2 + x1) + (4 * x3)) <= 1000);
                // Función objetivo
                model.AddGoal("maximo", GoalKind.Maximize, -(4 * x1) - (2 * x2) + x3);
                // Solucion
                Solution solucion = context.Solve(new SimplexDirective());
                Report reporte = solucion.GetReport();
                // Imprimir
                Console.WriteLine(reporte);
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}

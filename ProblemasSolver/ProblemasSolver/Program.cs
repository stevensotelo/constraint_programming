using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;
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
                Console.WriteLine("Seleccione el método por el que desea resolver el problema:\n1 Programación por restricciones\n2 Programación Lineal");
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
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
                        break;
                    case 2:
                        //Solucionador específico
                        ConstraintSystem csp = ConstraintSystem.CreateSolver();
                        // Creacíón de variables
                        CspTerm sx1 = csp.CreateVariable(csp.CreateIntegerInterval(50, 300), "x1");
                        CspTerm sx2 = csp.CreateVariable(csp.CreateIntegerInterval(100, 200), "x2");
                        CspTerm sx3 = csp.CreateVariable(csp.CreateIntegerInterval(20, 1000), "x3");
                        // Creación de restricciones
                        csp.AddConstraints(200 <= (sx1 + sx2 + sx3) <= 280,
                                            100 <= sx1 + (3 * sx2) <= 2000,
                                            50 <= (2 * sx1) + (4 * sx3) <= 1000);
                        
                        // Solución
                        ConstraintSolverSolution cspSolucion = csp.Solve();
                        int numero = 1;
                        while (cspSolucion.HasFoundSolution)
                        {
                            object rx1, rx2, rx3;
                            if (!cspSolucion.TryGetValue(sx1, out rx1) || !cspSolucion.TryGetValue(sx2, out rx2) || !cspSolucion.TryGetValue(sx3, out rx3))
                                throw new InvalidProgramException("No se encontro solución");
                            Console.WriteLine(String.Format("Solución {0} :\nx1={1}\nx2={2}\nx3={3}", numero, rx1, rx2,rx3));
                            numero += 1;
                            cspSolucion.GetNext();
                        }
                        /*
                        //Solucionador específico
                        SimplexSolver sSolver = new SimplexSolver();
                        //Creación de variables
                        int sx1, sx2, sx3;
                        sSolver.AddVariable("x1", out sx1);
                        sSolver.SetBounds(sx1, 50, 300);
                        sSolver.AddVariable("x2", out sx2);
                        sSolver.SetBounds(sx2, 100, 200);
                        sSolver.AddVariable("x3", out sx3);
                        sSolver.SetBounds(sx3,20,1000);
                        //Creación de restricciones
                        int r1, r2, r3, goal;
                        sSolver.AddRow("restriccion1", out r1);
                        sSolver.SetCoefficient(r1, sx1, 1);
                        sSolver.SetCoefficient(r1, sx2, 1);
                        sSolver.SetCoefficient(r1, sx3, 1);
                        sSolver.SetBounds(r1, 200, 280);
                        sSolver.AddRow("restriccion2", out r2);
                        sSolver.SetCoefficient(r2, sx1, 1);
                        sSolver.SetCoefficient(r2, sx2, 3);
                        sSolver.SetBounds(r2, 100, 2000);
                        sSolver.AddRow("restriccion3", out r3);
                        sSolver.SetCoefficient(r3, sx1, 2);
                        sSolver.SetCoefficient(r3, sx3, 4);
                        sSolver.SetBounds(r3, 50, 1000);
                        //Función objetivo
                        sSolver.AddRow("objetivo", out goal);
                        sSolver.SetCoefficient(goal, sx1, -4);
                        sSolver.SetCoefficient(goal, sx2, -2);
                        sSolver.SetCoefficient(goal, sx3, 1);
                        sSolver.SetBounds(goal, Rational.NegativeInfinity,Rational.PositiveInfinity);
                        sSolver.AddGoal(goal, 1, false);
                        //Solución
                        sSolver.Solve(new SimplexSolverParams());
                        sSolver.GetReport();
                        */
                        break;
                }

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

using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodegas
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /*CREACIÓN DEL PROBLEMA*/
                Console.WriteLine("CREACIÓN DEL PROBLEMA");

                //Crea una instancia de SolverContext bajo el patron de diseño Singleton
                SolverContext contexto = SolverContext.GetContext();

                //Esta variable almacena todo el modelo mátematico del problema
                Model model = contexto.CreateModel();

                /*VARIABLES DEL PROBLEMA*/
                Console.WriteLine("VARIABLES DEL PROBLEMA");

                //Se define el dominio de las variables del problema y su nombre
                Decision x1a = new Decision(Domain.IntegerNonnegative, "x1a");
                Decision x1b = new Decision(Domain.IntegerNonnegative, "x1b");
                Decision x1c = new Decision(Domain.IntegerNonnegative, "x1c");
                Decision x2a = new Decision(Domain.IntegerNonnegative, "x2a");
                Decision x2b = new Decision(Domain.IntegerNonnegative, "x2b");
                Decision x2c = new Decision(Domain.IntegerNonnegative, "x2c");
                Decision x3a = new Decision(Domain.IntegerNonnegative, "x3a");
                Decision x3b = new Decision(Domain.IntegerNonnegative, "x3b");
                Decision x3c = new Decision(Domain.IntegerNonnegative, "x3c");
                
                //Las variables creadas son adicionadas al modelo del problema
                model.AddDecisions(new Decision[] { x1a, x1b, x1c, x2a, x2b, x2c, x3a, x3b, x3c });

                /*RESTRICCIONES DEL PROBLEMA*/
                Console.WriteLine("RESTRICCIONES DEL PROBLEMA");

                //Restricciones de capacidad en toneladas de las bodegas
                //X1A + X1B + X1C ≤ 2000
                model.AddConstraint("rCapacidadToneladasX1", x1a + x1b + x1c <= 100);
                //X2A + X2B + X2C ≤ 1500
                model.AddConstraint("rCapacidadToneladasX2", x2a + x2b + x2c <= 110);
                //X3A + X3B + X3C ≤ 3000
                model.AddConstraint("rCapacidadToneladasX3", x3a + x3b + x3c <= 120);

                //Restricciones de capacidad en volumen de las bodegas
                //60X1A + 50X1B + 25X1C ≤ 100000
                model.AddConstraint("rCapacidadVolumenX1", 60 * x1a + 50 * x1b + 25 * x1c <= 1600);
                //60X2A + 50X2B + 25X2C ≤ 300000
                model.AddConstraint("rCapacidadVolumenX2", 60 * x2a + 50 * x2b + 25 * x2c <= 1700);
                //60X3A + 50X3B + 25X3C ≤ 135000
                model.AddConstraint("rCapacidadVolumenX3", 60 * x3a + 50 * x3b + 25 * x3c <= 5000);

                //Restricciones de oferta en toneladas de cada tipo de carga
                //X1A + X2A + X3A ≤ 6000
                model.AddConstraint("rToneladasTipoCarga1", x1a + x2a + x3a <= 50);
                //X1B + X2B + X3B ≤ 4000
                model.AddConstraint("rToneladasTipoCarga2", x1b + x2b + x3b <= 120);
                //X1C + X2C + X3C ≤ 2000
                model.AddConstraint("rToneladasTipoCarga3", x1c + x2c + x3c <= 100);

                //Restricciones de porcentaje igual
                //3X1A + 3X1B + 3X1C - 4X2A - 4X2B - 4X2C = 0
                model.AddConstraint("Porcentaje1", 11 * (x1a + x1b + x1c) - 10 * (x2a + x2b + x2c) == 0);
                //3X1A + 3X1B + 3X1C - 2X3A - 2X3B - 2X3C = 0
                model.AddConstraint("Porcentaje2", 6 * (x1a + x1b + x1c) - 5 * (x3a + x3b + x3c) == 0);

                /*OPTIMIZACIÓN DEL PROBLEMA*/

                model.AddGoal("Maximo", GoalKind.Maximize, 6 * (x1a + x1b + x1c) + 8 * (x2a + x2b + x2c) + 5 * (x3a + x3b + x3c));

                /*SOLUCIÓN*/
                Console.WriteLine("SOLUCIÓN");

                var solucion = contexto.Solve();

                /*PRESENTACIÓN DE VALORES*/
                Console.WriteLine("PRESENTACIÓN DE VALORES");

                Console.WriteLine("La solución hallada tiene una consideración : " + solucion.Quality.ToString());
                foreach(var i in solucion.Goals)
                    Console.WriteLine("MÁXIMO= " + i.ToDouble());

                Console.WriteLine("VARIABLES");
                foreach (var i in model.Decisions)
                    Console.WriteLine(i.Name + "=" + i.ToDouble());

                Console.WriteLine("RESTRICCIONES");
                foreach (var i in model.Constraints)
                    Console.WriteLine(i.Name + ":\t\t" + i.Expression);

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
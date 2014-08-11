/*
* To change this license header, choose License Headers in Project Properties.
* To change this template file, choose Tools | Templates
* and open the template in the editor.
*/

import ExamplesJaCoP.Example;
import JaCoP.constraints.SumWeight;
import JaCoP.constraints.XplusYplusQeqZ;
import JaCoP.core.IntVar;
import JaCoP.core.Store;
import JaCoP.core.Var;
import JaCoP.search.*;
import java.util.ArrayList;
import javax.swing.JOptionPane;

/**
 *
 * @author Steven
 */
public class Bodega extends Example
{
    
    public void model()
    {
        
        store = new Store();
        vars = new ArrayList<Var>();
        
        //Variables del problema
        IntVar x1a = new IntVar(store, "X1A", 0, 100);
        IntVar x1b = new IntVar(store, "X1B", 0, 100);
        IntVar x1c = new IntVar(store, "X1C", 0, 100);
        IntVar x2a = new IntVar(store, "X2A", 0, 110);
        IntVar x2b = new IntVar(store, "X2B", 0, 110);
        IntVar x2c = new IntVar(store, "X2C", 0, 110);
        IntVar x3a = new IntVar(store, "X3A", 0, 120);
        IntVar x3b = new IntVar(store, "X3B", 0, 120);
        IntVar x3c = new IntVar(store, "X3C", 0, 120);
        
        /*Restricciones de capacidad en toneladas de las bodegas*/
        //X1A + X1B + X1C ≤ 2000
        IntVar sum1 = new IntVar(store, "rCapacidadToneladasX1", 0, 100);
        store.impose(new XplusYplusQeqZ(x1a, x1b, x1c, sum1));
        //X2A + X2B + X2C ≤ 1500
        IntVar sum2 = new IntVar(store, "rCapacidadToneladasX2", 0, 110);
        store.impose(new XplusYplusQeqZ(x2a, x2b, x2c, sum2));
        //X3A + X3B + X3C ≤ 3000
        IntVar sum3 = new IntVar(store, "rCapacidadToneladasX3", 0, 120);
        store.impose(new XplusYplusQeqZ(x3a, x3b, x3c, sum3));
        
        /*Restricciones de capacidad en volumen de las bodegas*/
        int[] w = {60, 50, 25};
        //60X1A + 50X1B + 25X1C ≤ 100000
        IntVar sum4 = new IntVar(store, "rCapacidadVolumenX1", 0, 1600);
        store.impose(new SumWeight(new IntVar[]{x1a, x1b, x1c}, w, sum4));
        //60X2A + 50X2B + 25X2C ≤ 300000
        IntVar sum5 = new IntVar(store, "rCapacidadVolumenX2", 0, 1700);
        store.impose(new SumWeight(new IntVar[]{x2a, x2b, x2c}, w, sum5));
        //60X3A + 50X3B + 25X3C ≤ 135000
        IntVar sum6 = new IntVar(store, "rCapacidadVolumenX3", 0, 5000);
        store.impose(new SumWeight(new IntVar[]{x3a, x3b, x3c}, w, sum6));
        
        /*Restricciones de oferta en toneladas de cada tipo de carga*/
        //X1A + X2A + X3A ≤ 6000
        IntVar sum7 = new IntVar(store, "rToneladasTipoCarga1", 0, 50);
        store.impose(new XplusYplusQeqZ(x1a, x2a, x3a, sum7));
        //X1B + X2B + X3B ≤ 4000
        IntVar sum8 = new IntVar(store, "rToneladasTipoCarga2", 0, 120);
        store.impose(new XplusYplusQeqZ(x1b, x2b, x3b, sum8));
        //X1C + X2C + X3C ≤ 2000
        IntVar sum9 = new IntVar(store, "rToneladasTipoCarga3", 0, 100);
        store.impose(new XplusYplusQeqZ(x1c, x2c, x3c, sum9));
        
        /*Restricciones de porcentaje igual*/
        //3X1A + 3X1B + 3X1C - 4X2A - 4X2B - 4X2C = 0
        IntVar sum10 = new IntVar(store, "Porcentaje1", 0, 0);
        store.impose(new SumWeight(new IntVar[]{x1a, x1b, x1c, x2a, x2b, x2c}, new int[]  {11, 11, 11, -10, -10, -10}, sum10));
        //3X1A + 3X1B + 3X1C - 2X3A - 2X3B - 2X3C = 0
        IntVar sum11 = new IntVar(store, "Porcentaje2", 0, 0);
        store.impose(new SumWeight(new IntVar[]{x1a, x1b, x1c, x3a, x3b, x3c}, new int[]{6, 6, 6, -5, -5, -5}, sum11));
        
        /*Optimización*/
        cost = new IntVar(store, "Optimo", Integer.MIN_VALUE, -1);
        store.impose(new SumWeight(new IntVar[]{x1a, x2a, x3a, x1b, x2b, x3b, x1c, x2c, x3c}, new int[]{-6, -6, -6, -8, -8, -8, -5, -5, -5,}, cost));
        
        /*Registro de variables*/
        vars.add(x1a);
        vars.add(x1b);
        vars.add(x1c);
        vars.add(x2a);
        vars.add(x2b);
        vars.add(x2c);
        vars.add(x3a);
        vars.add(x3b);
        vars.add(x3c);
        vars.add(cost);
    }
    
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args)
    {
        Bodega example = new Bodega();
        try
        {
            example.model();
            int ejecucion=Integer.parseInt(JOptionPane.showInputDialog("Seleccione una opción:\n\n1.Resultados óptimos\n2.Todas\n3.Salir\n"));            
            while(ejecucion==1 || ejecucion==2)
            {
                switch (ejecucion)
                {
                    case 1:
                        if (example.searchAllOptimal())
                            System.out.println("Solucion encontrada");
                        else
                            System.out.println("No se hallo solucion");
                        break;
                    case 2:
                        if (example.searchAllAtOnce())
                            System.out.println("Solucion encontrada");
                        else
                            System.out.println("No se hallo solucion");
                        break;
                    case 3:
                        System.exit(0);
                        break;
                }
                ejecucion=Integer.parseInt(JOptionPane.showInputDialog("Seleccione una opción:\n\n1.Resultados óptimos\n2.Todas\n3.Salir\n"));
            }
        }
        catch(Exception ex)
        {
            System.out.println(ex);
        }
    }
    
    public boolean search()
    {
        long T1, T2;
        T1 = System.currentTimeMillis();
        SelectChoicePoint select = new SimpleSelect(vars.toArray(new Var[1]), null,new IndomainMin());
        search = new DepthFirstSearch();
        boolean result = search.labeling(store, select);
        if (result)
            store.print();
        T2 = System.currentTimeMillis();
        System.out.println("\n\t*** Tiempo de ejecución = " + (T2 - T1) + " ms");
        return result;
    }
    
    public boolean searchAllAtOnce()
    {
        long T1, T2;
        T1 = System.currentTimeMillis();
        SelectChoicePoint select = new SimpleSelect(vars.toArray(new Var[1]),new MostConstrainedStatic(), new IndomainMin());
        search = new DepthFirstSearch();
        search.setSolutionListener(new PrintOutListener<IntVar>());
        search.getSolutionListener().searchAll(true);
        search.getSolutionListener().recordSolutions(false);
        boolean result = search.labeling(store, select);
        T2 = System.currentTimeMillis();
        
        if (result) {
            System.out.println("Número de soluciones " + search.getSolutionListener().solutionsNo());
            search.printAllSolutions();
        }
        else
            System.out.println("No se encontraron soluciones");
        System.out.println("\n\t*** Tiempo de ejecución = " + (T2 - T1) + " ms");
        return result;
    }
    
    public boolean searchAllOptimal()
    {
        long T1, T2;
        T1 = System.currentTimeMillis();
        SelectChoicePoint select = new SimpleSelect(vars.toArray(new Var[1]), null,new IndomainMin());
        search = new DepthFirstSearch();
        search.setSolutionListener(new PrintOutListener<IntVar>());
        search.getSolutionListener().searchAll(true);
        search.getSolutionListener().recordSolutions(false);
        boolean result = search.labeling(store, select, cost);
        T2 = System.currentTimeMillis();
        System.out.println("\n\t*** Tiempo de ejecución = " + (T2 - T1) + " ms");
        return result;
    }
}

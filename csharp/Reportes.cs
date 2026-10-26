/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Reportes.cs  -  Matriz proyecto x estado, montos y resumen por cliente
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

partial class Program
{
    // -----------------------------------------------------------------------------
    // REPORTES
    // -----------------------------------------------------------------------------
    // MATRIZ (tema 3): filas = proyectos, columnas = estados


// MATRIZ (tema 3): filas = proyectos, columnas = estados
    static int[,] ConstruirMatrizProyectoEstado()
    {
        int[,] matriz = new int[PROYECTOS.Length, ESTADOS.Length];   // inicia en 0

        for (int k = 0; k < codigos.Count; k++)
        {
            int fila = PROYECTOS.Length - 1;           // por defecto: OTRO
            for (int i = 0; i < PROYECTOS.Length; i++)
            {
                if (PROYECTOS[i] == proyectos[k])
                    fila = i;
            }
            for (int j = 0; j < ESTADOS.Length; j++)
            {
                if (ESTADOS[j] == estados[k])
                    matriz[fila, j] = matriz[fila, j] + 1;
            }
        }
        return matriz;
    }

    static void MostrarMatriz(int[,] matriz)
    {
        string encabezado = "PROYECTO".PadRight(12);
        foreach (string estado in ESTADOS)
            encabezado = encabezado + estado.PadLeft(13);
        encabezado = encabezado + "TOTAL".PadLeft(8);
        Console.WriteLine(encabezado);
        Console.WriteLine(Linea('-', encabezado.Length));

        int[] totalesColumna = new int[ESTADOS.Length];
        for (int i = 0; i < PROYECTOS.Length; i++)
        {
            int sumaFila = 0;
            string linea = PROYECTOS[i].PadRight(12);
            for (int j = 0; j < ESTADOS.Length; j++)
            {
                linea = linea + matriz[i, j].ToString().PadLeft(13);
                sumaFila = sumaFila + matriz[i, j];
                totalesColumna[j] = totalesColumna[j] + matriz[i, j];
            }
            Console.WriteLine(linea + sumaFila.ToString().PadLeft(8));
        }

        Console.WriteLine(Linea('-', encabezado.Length));
        string lineaTotal = "TOTAL".PadRight(12);
        foreach (int total in totalesColumna)
            lineaTotal = lineaTotal + total.ToString().PadLeft(13);
        Console.WriteLine(lineaTotal + codigos.Count.ToString().PadLeft(8));
    }

    // Devuelve (total, promedio, mayor, menor) recorriendo el arreglo montos
    static (double, double, double, double) CalcularMontos()
    {
        if (montos.Count == 0)
            return (0, 0, 0, 0);
        double total = 0;
        double mayor = montos[0];
        double menor = montos[0];
        foreach (double monto in montos)
        {
            total = total + monto;
            if (monto > mayor)
                mayor = monto;
            if (monto < menor)
                menor = monto;
        }
        return (total, total / montos.Count, mayor, menor);
    }

    static int ContarPendientes()
    {
        int pendientes = 0;
        foreach (string estado in estados)
        {
            if (estado != "ATENDIDO")
                pendientes = pendientes + 1;
        }
        return pendientes;
    }

    // Agrupa por DNI en CUATRO ARREGLOS PARALELOS nuevos: la posicion i de
    // los cuatro corresponde al mismo cliente.
    static void ResumenPorCliente(List<string> dnisCliente, List<string> nombresCliente,
                                  List<int> cantidades, List<double> totales)
    {
        for (int k = 0; k < codigos.Count; k++)
        {
            int posicion = -1;
            for (int i = 0; i < dnisCliente.Count; i++)
            {
                if (dnisCliente[i] == dnis[k])
                {
                    posicion = i;
                    break;
                }
            }
            if (posicion == -1)
            {
                dnisCliente.Add(dnis[k]);
                nombresCliente.Add(nombres[k]);
                cantidades.Add(1);
                totales.Add(montos[k]);
            }
            else
            {
                cantidades[posicion] = cantidades[posicion] + 1;
                totales[posicion] = totales[posicion] + montos[k];
            }
        }
    }

    // Insercion de mayor a menor total, moviendo los 4 arreglos JUNTOS: si solo
    // se moviera 'totales', cada monto quedaria asignado al cliente equivocado.
    static void OrdenarClientesPorTotal(List<string> dnisCliente, List<string> nombresCliente,
                                        List<int> cantidades, List<double> totales)
    {
        for (int i = 1; i < totales.Count; i++)
        {
            string dniAux = dnisCliente[i];
            string nombreAux = nombresCliente[i];
            int cantidadAux = cantidades[i];
            double totalAux = totales[i];
            int j = i - 1;
            while (j >= 0 && totales[j] < totalAux)
            {
                dnisCliente[j + 1] = dnisCliente[j];
                nombresCliente[j + 1] = nombresCliente[j];
                cantidades[j + 1] = cantidades[j];
                totales[j + 1] = totales[j];
                j = j - 1;
            }
            dnisCliente[j + 1] = dniAux;
            nombresCliente[j + 1] = nombreAux;
            cantidades[j + 1] = cantidadAux;
            totales[j + 1] = totalAux;
        }
    }
}

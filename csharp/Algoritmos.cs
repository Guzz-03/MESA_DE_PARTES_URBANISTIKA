/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Algoritmos.cs  -  Busqueda y ordenamiento (tema 3)
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

partial class Program
{
    // =========================================================================
    // BUSQUEDA (TEMA 3)
    // =========================================================================

    // ALTERNATIVA A. Busca el texto en codigo, DNI, nombre, lote y proyecto.
    static (List<int>, int) BusquedaLineal(string textoBuscado)
    {
        List<int> encontrados = new List<int>();
        int comparaciones = 0;
        for (int i = 0; i < codigos.Count; i++)
        {
            // Los 5 datos de la posicion i, tomados de 5 arreglos paralelos
            string[] campos = { codigos[i], dnis[i], nombres[i], lotes[i], proyectos[i] };
            foreach (string campo in campos)
            {
                comparaciones = comparaciones + 1;
                if (ContieneTexto(campo, textoBuscado))
                {
                    encontrados.Add(i);
                    break;
                }
            }
        }
        return (encontrados, comparaciones);
    }

    // ALTERNATIVA B. 'orden' son las posiciones ordenadas por codigo.
    static (int, int) BusquedaBinaria(List<int> orden, string codigo)
    {
        codigo = Normalizar(codigo);
        int comparaciones = 0;
        int inicio = 0;
        int fin = orden.Count - 1;
        while (inicio <= fin)
        {
            int medio = (inicio + fin) / 2;
            comparaciones = comparaciones + 1;
            string actual = codigos[orden[medio]];
            int resultado = string.CompareOrdinal(actual, codigo);
            if (resultado == 0)
                return (orden[medio], comparaciones);
            else if (resultado < 0)
                inicio = medio + 1;
            else
                fin = medio - 1;
        }
        return (-1, comparaciones);
    }

    // -----------------------------------------------------------------------------
    // ORDENAMIENTO
    // -----------------------------------------------------------------------------
// Con arreglos paralelos, mover un expediente obligaria a mover sus 11 datos.
    // En su lugar se ordena un ARREGLO DE POSICIONES y los originales no se tocan.

    // Compara las posiciones a y b segun el criterio: <0, 0 o >0
    static int Comparar(int a, int b, string criterio)
    {
        if (criterio == "MONTO")
            return montos[a].CompareTo(montos[b]);                 // numero: 900 < 1000
        if (criterio == "FECHA")
            return string.CompareOrdinal(FechaOrdenable(fechas[a]), FechaOrdenable(fechas[b]));
        if (criterio == "CLIENTE")
            return string.CompareOrdinal(Normalizar(nombres[a]), Normalizar(nombres[b]));
        if (criterio == "PROYECTO")
            return string.CompareOrdinal(proyectos[a], proyectos[b]);
        return string.CompareOrdinal(codigos[a], codigos[b]);
    }

    // BURBUJA. Devuelve (posiciones ordenadas, comparaciones, intercambios)
    static (List<int>, int, int) OrdenarBurbuja(string criterio, bool ascendente)
    {
        List<int> orden = TodasLasPosiciones();
        int comparaciones = 0;
        int intercambios = 0;
        int total = orden.Count;

        for (int i = 0; i < total - 1; i++)
        {
            bool huboIntercambio = false;
            for (int j = 0; j < total - 1 - i; j++)
            {
                comparaciones = comparaciones + 1;
                int resultado = Comparar(orden[j], orden[j + 1], criterio);
                bool desordenado;
                if (ascendente)
                    desordenado = resultado > 0;
                else
                    desordenado = resultado < 0;
                if (desordenado)
                {
                    int aux = orden[j];
                    orden[j] = orden[j + 1];
                    orden[j + 1] = aux;
                    intercambios = intercambios + 1;
                    huboIntercambio = true;
                }
            }
            if (!huboIntercambio)
                break;   // ya esta ordenado
        }
        return (orden, comparaciones, intercambios);
    }
    // INSERCION. Devuelve (posiciones ordenadas, comparaciones, desplazamientos)
    static (List<int>, int, int) OrdenarInsercion(string criterio, bool ascendente)
    {
        List<int> orden = TodasLasPosiciones();
        int comparaciones = 0;
        int desplazamientos = 0;

        for (int i = 1; i < orden.Count; i++)
        {
            int actual = orden[i];
            int j = i - 1;
            while (j >= 0)
            {
                comparaciones = comparaciones + 1;
                int resultado = Comparar(orden[j], actual, criterio);
                bool debeCorrerse;
                if (ascendente)
                    debeCorrerse = resultado > 0;
                else
                    debeCorrerse = resultado < 0;
                if (!debeCorrerse)
                    break;
                orden[j + 1] = orden[j];
                desplazamientos = desplazamientos + 1;
                j = j - 1;
            }
            orden[j + 1] = actual;
        }
        return (orden, comparaciones, desplazamientos);
    }
}

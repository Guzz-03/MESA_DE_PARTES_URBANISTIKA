/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Cadenas.cs  -  Manejo de cadenas de caracteres (tema 4)
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

partial class Program
{
    // =========================================================================
    // CADENAS DE CARACTERES (TEMA 4)
    // =========================================================================

    static bool SoloDigitos(string cadena)
    {
        if (cadena.Length == 0)
            return false;
        foreach (char caracter in cadena)
        {
            if (caracter < '0' || caracter > '9')
                return false;
        }
        return true;
    }

    static string QuitarTildes(string texto)
    {
        string resultado = "";
        foreach (char caracter in texto)
        {
            int posicion = CON_TILDE.IndexOf(caracter);
            if (posicion == -1)
                resultado = resultado + caracter;
            else
                resultado = resultado + SIN_TILDE[posicion];
        }
        return resultado;
    }

    // MAYUSCULAS, sin tildes y sin espacios sobrantes: "José " -> "JOSE"
    static string Normalizar(string texto)
    {
        return QuitarTildes(texto.Trim().ToUpper());
    }

    // Primera letra de cada palabra en mayuscula: "mARIA rojas" -> "Maria Rojas"
    static string FormatoNombre(string texto)
    {
        string[] palabras = texto.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        string resultado = "";
        foreach (string palabra in palabras)
        {
            if (resultado.Length > 0)
                resultado = resultado + " ";
            resultado = resultado + palabra.Substring(0, 1).ToUpper() + palabra.Substring(1).ToLower();
        }
        return resultado;
    }

    // Quita espacios y separadores de miles: "13,000.50" -> "13000.50"
    static string LimpiarMonto(string monto)
    {
        return monto.Trim().Replace(",", "");
    }

    // 13000 -> "S/ 13,000.00"
    static string FormatoSoles(double monto)
    {
        return "S/ " + monto.ToString("N2", CULTURA);
    }

    // Busca 'buscado' dentro de 'texto' caracter por caracter (algoritmo de subcadena) en lugar de usar Contains, para evidenciar el manejo de cadenas.
    static bool ContieneTexto(string texto, string buscado)
    {
        texto = Normalizar(texto);
        buscado = Normalizar(buscado);
        if (buscado.Length == 0 || buscado.Length > texto.Length)
            return false;

        for (int inicio = 0; inicio <= texto.Length - buscado.Length; inicio++)
        {
            bool coincide = true;
            for (int desplazamiento = 0; desplazamiento < buscado.Length; desplazamiento++)
            {
                if (texto[inicio + desplazamiento] != buscado[desplazamiento])
                {
                    coincide = false;
                    break;
                }
            }
            if (coincide)
                return true;
        }
        return false;
    }

    // --- Descomponer cadenas: separar un texto en sus partes ----------------

    // "EXP-2026-0007" -> ["EXP", "2026", "0007"]
    static string[] DescomponerCodigo(string codigo)
    {
        return codigo.Split('-');
    }

    // "18/08/2026" -> ["18", "08", "2026"]
    static string[] DescomponerFecha(string fecha)
    {
        return fecha.Split('/');
    }

    // "MZ A - LT 12" -> ["A", "12"]; si no tiene ese formato se devuelve tal cual
    static string[] DescomponerLote(string lote)
    {
        string[] partes = lote.Split('-');
        if (partes.Length != 2)
            return new string[] { lote, "" };
        string manzana = partes[0].Trim();
        string numero = partes[1].Trim();
        if (manzana.StartsWith("MZ"))
            manzana = manzana.Substring(2).Trim();
        if (numero.StartsWith("LT"))
            numero = numero.Substring(2).Trim();
        return new string[] { manzana, numero };
    }

    // "dd/mm/aaaa" -> "aaaammdd" para que el orden de texto sea cronologico
    static string FechaOrdenable(string fecha)
    {
        string[] partes = DescomponerFecha(fecha);
        return partes[2] + partes[1] + partes[0];
    }
}

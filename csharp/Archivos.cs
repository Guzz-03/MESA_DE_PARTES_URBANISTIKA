/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Archivos.cs  -  Persistencia (temas 5 a 7)
   - ARCHIVO DE TEXTO : expedientes.txt, una linea por expediente, 11 campos con '|'
   - ARCHIVO BINARIO  : expedientes.dat, registros de LONGITUD FIJA (182 bytes)
 Los dos formatos son IDENTICOS a los de la version en Python: un archivo
 creado por un programa lo puede leer el otro.
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

partial class Program
{
    const string ARCHIVO_TEXTO = "expedientes.txt";
    const string ARCHIVO_BINARIO = "expedientes.dat";
    const char SEPARADOR = '|';
    const int CAMPOS = 11;

    // Ancho en bytes de cada campo de texto del registro binario (el monto son 8 bytes)
    static readonly int[] ANCHOS = { 13, 10, 8, 40, 9, 12, 20, 30, 0, 12, 20 };
    const int TAMANIO_REGISTRO = 182;
    static readonly UTF8Encoding UTF8_SIN_BOM = new UTF8Encoding(false);

    // Vacia los 11 arreglos paralelos
    static void LimpiarDatos()
    {
        codigos.Clear(); fechas.Clear(); dnis.Clear(); nombres.Clear();
        telefonos.Clear(); proyectos.Clear(); lotes.Clear(); tiposDocumento.Clear();
        montos.Clear(); estados.Clear(); responsables.Clear();
    }

    // Los 10 campos de texto del expediente i, en el orden del archivo (sin el monto)
    static string[] CamposTexto(int i)
    {
        return new string[] { codigos[i], fechas[i], dnis[i], nombres[i], telefonos[i],
                              proyectos[i], lotes[i], tiposDocumento[i], "", estados[i], responsables[i] };
    }

    // -------------------------------------------------------------------------
    // ARCHIVO DE TEXTO
    // -------------------------------------------------------------------------

    static string LineaDeExpediente(int i)
    {
        string[] partes = CamposTexto(i);
        partes[8] = montos[i].ToString("0.00", CULTURA);
        for (int k = 0; k < CAMPOS; k++)
            partes[k] = partes[k].Replace(SEPARADOR, ' ');
        return string.Join(SEPARADOR.ToString(), partes);
    }

    // Descompone y revisa una linea. Devuelve los campos o null (con el motivo).
    static string[] ValidarLinea(string linea, out string motivo)
    {
        string[] partes = linea.Split(SEPARADOR);
        if (partes.Length != CAMPOS)
        {
            motivo = "tiene " + partes.Length + " campos y deben ser " + CAMPOS;
            return null;
        }
        for (int k = 0; k < CAMPOS; k++)
            partes[k] = partes[k].Trim();
        double monto;
        if (!ValidarFecha(partes[1]).Item1) { motivo = "fecha invalida"; return null; }
        if (!ValidarDni(partes[2]).Item1) { motivo = "DNI invalido"; return null; }
        if (!double.TryParse(partes[8], NumberStyles.Float, CULTURA, out monto))
        {
            motivo = "monto invalido";
            return null;
        }
        motivo = "OK";
        return partes;
    }

    static void AgregarDesdeCampos(string[] p)
    {
        AgregarExpediente(p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7],
                          double.Parse(p[8], CULTURA), p[9], p[10]);
    }

    // Escribe primero un .tmp y luego reemplaza: si falla a medias no se pierde el anterior
    static int GuardarTexto(string ruta = ARCHIVO_TEXTO)
    {
        string temporal = ruta + ".tmp";
        using (StreamWriter archivo = new StreamWriter(temporal, false, UTF8_SIN_BOM))
        {
            for (int i = 0; i < codigos.Count; i++)
                archivo.Write(LineaDeExpediente(i) + "\n");
        }
        if (File.Exists(ruta))
            File.Delete(ruta);
        File.Move(temporal, ruta);
        return codigos.Count;
    }

    // Lee el archivo de texto. Las lineas danadas se descartan y se avisan.
    static int CargarTexto(List<string> avisos, string ruta = ARCHIVO_TEXTO)
    {
        LimpiarDatos();
        if (!File.Exists(ruta))
            return 0;
        int numero = 0;
        foreach (string linea in File.ReadLines(ruta, Encoding.UTF8))
        {
            numero++;
            if (linea.Trim().Length == 0)
                continue;
            string motivo;
            string[] campos = ValidarLinea(linea, out motivo);
            if (campos == null)
                avisos.Add("Linea " + numero + ": " + motivo);
            else
                AgregarDesdeCampos(campos);
        }
        return codigos.Count;
    }

    // ACCESO SECUENCIAL: para llegar a la linea n hay que pasar por las anteriores
    static string[] LeerLineaTexto(int n, string ruta = ARCHIVO_TEXTO)
    {
        int numero = 0;
        foreach (string linea in File.ReadLines(ruta, Encoding.UTF8))
        {
            if (numero == n)
            {
                string motivo;
                return ValidarLinea(linea, out motivo);
            }
            numero++;
        }
        return null;
    }

    // -------------------------------------------------------------------------
    // ARCHIVO BINARIO DE REGISTROS DE LONGITUD FIJA
    // -------------------------------------------------------------------------

    // Texto -> exactamente 'ancho' bytes UTF-8 (recorta sin partir una tilde y rellena con ceros)
    static byte[] ABytes(string texto, int ancho)
    {
        byte[] datos = Encoding.UTF8.GetBytes(texto);
        while (datos.Length > ancho)
        {
            texto = texto.Substring(0, texto.Length - 1);
            datos = Encoding.UTF8.GetBytes(texto);
        }
        byte[] campo = new byte[ancho];
        Array.Copy(datos, campo, datos.Length);
        return campo;
    }

    static string ATexto(byte[] campo)
    {
        int largo = campo.Length;
        while (largo > 0 && campo[largo - 1] == 0)
            largo--;
        return Encoding.UTF8.GetString(campo, 0, largo);
    }

    static void EscribirRegistro(BinaryWriter escritor, int i)
    {
        string[] campos = CamposTexto(i);
        for (int k = 0; k < CAMPOS; k++)
        {
            if (k == 8)
                escritor.Write(montos[i]);          // double de 8 bytes
            else
                escritor.Write(ABytes(campos[k], ANCHOS[k]));
        }
    }

    static string[] LeerRegistro(BinaryReader lector)
    {
        string[] campos = new string[CAMPOS];
        for (int k = 0; k < CAMPOS; k++)
        {
            if (k == 8)
                campos[k] = Math.Round(lector.ReadDouble(), 2).ToString("R", CULTURA);
            else
                campos[k] = ATexto(lector.ReadBytes(ANCHOS[k]));
        }
        return campos;
    }

    static int GuardarBinario(string ruta = ARCHIVO_BINARIO)
    {
        using (BinaryWriter escritor = new BinaryWriter(File.Create(ruta)))
        {
            for (int i = 0; i < codigos.Count; i++)
                EscribirRegistro(escritor, i);
        }
        return codigos.Count;
    }

    static int CargarBinario(string ruta = ARCHIVO_BINARIO)
    {
        LimpiarDatos();
        if (!File.Exists(ruta))
            return 0;
        int total = ContarRegistrosBinario(ruta);
        using (BinaryReader lector = new BinaryReader(File.OpenRead(ruta)))
        {
            for (int n = 0; n < total; n++)
                AgregarDesdeCampos(LeerRegistro(lector));
        }
        return codigos.Count;
    }

    static int ContarRegistrosBinario(string ruta = ARCHIVO_BINARIO)
    {
        if (!File.Exists(ruta))
            return 0;
        return (int)(new FileInfo(ruta).Length / TAMANIO_REGISTRO);
    }

    // ACCESO DIRECTO: salta al byte n * 182 y lee solo ese registro
    static string[] LeerRegistroBinario(int n, string ruta = ARCHIVO_BINARIO)
    {
        if (n < 0 || n >= ContarRegistrosBinario(ruta))
            return null;
        using (BinaryReader lector = new BinaryReader(File.OpenRead(ruta)))
        {
            lector.BaseStream.Seek((long)n * TAMANIO_REGISTRO, SeekOrigin.Begin);
            return LeerRegistro(lector);
        }
    }

    // -------------------------------------------------------------------------
    // COMPARACION DE LAS DOS ALTERNATIVAS
    // -------------------------------------------------------------------------

    // Devuelve: bytesTxt, bytesBin, guardarTxt, guardarBin, cargarTxt, cargarBin, accesoTxt, accesoBin (ms)
    static double[] CompararFormatos(string rutaTexto, string rutaBinario, int repeticiones)
    {
        double[] r = new double[8];
        Stopwatch reloj = Stopwatch.StartNew();
        GuardarTexto(rutaTexto); r[2] = reloj.Elapsed.TotalMilliseconds;
        reloj.Restart(); GuardarBinario(rutaBinario); r[3] = reloj.Elapsed.TotalMilliseconds;
        r[0] = new FileInfo(rutaTexto).Length;
        r[1] = new FileInfo(rutaBinario).Length;
        reloj.Restart(); CargarTexto(new List<string>(), rutaTexto); r[4] = reloj.Elapsed.TotalMilliseconds;
        reloj.Restart(); CargarBinario(rutaBinario); r[5] = reloj.Elapsed.TotalMilliseconds;

        int ultimo = Math.Max(codigos.Count - 1, 0);
        reloj.Restart();
        for (int k = 0; k < repeticiones; k++) LeerLineaTexto(ultimo, rutaTexto);
        r[6] = reloj.Elapsed.TotalMilliseconds / repeticiones;
        reloj.Restart();
        for (int k = 0; k < repeticiones; k++) LeerRegistroBinario(ultimo, rutaBinario);
        r[7] = reloj.Elapsed.TotalMilliseconds / repeticiones;
        return r;
    }
}

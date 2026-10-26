/*
===============================================================================
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
 Version del TEMA 7 en C#: archivos parciales, archivo de texto y respaldo binario

 Curso  : Fundamentos de Programacion - CIIN1205P
 Docente: Humberto Pedro Jimenez Jara
 Grupo 1: Alvarado Valiente Valeria / Flores Chavez Anahi /
          Guzman Polo Carol Jacqueline / Hernandez Vasquez Andy Rafael /
          Valderrama Rosado Aldahir Obved
 UPN - Trujillo, 2026

 Compilar y ejecutar : dotnet run
 Pruebas automaticas : dotnet run -- --pruebas
===============================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

partial class Program
{
    // -------------------------------------------------------------------------
    // OPCIONES DE ARCHIVOS (TEMAS 5 A 7)
    // -------------------------------------------------------------------------

    static void OpcionRespaldoBinario()
    {
        if (!HayExpedientes())
            return;
        int total = GuardarBinario();
        Console.WriteLine("\n [OK] Respaldo creado: " + ARCHIVO_BINARIO + " (" + total +
                          " registros de " + TAMANIO_REGISTRO + " bytes).");
    }

    static void OpcionRestaurarBinario()
    {
        if (ContarRegistrosBinario() == 0)
        {
            Console.WriteLine("\n [!] No existe un respaldo binario con registros.");
            return;
        }
        if (Leer("\n Se reemplazaran los " + codigos.Count +
                 " expedientes actuales. Continuar? (S/N): ").ToUpper() != "S")
            return;
        int total = CargarBinario();
        GuardarTexto();
        Console.WriteLine("\n [OK] Se restauraron " + total + " expedientes desde el respaldo.");
    }

    static void OpcionVerRegistroBinario()
    {
        int total = ContarRegistrosBinario();
        if (total == 0)
        {
            Console.WriteLine("\n [!] Primero cree el respaldo (opcion 11).");
            return;
        }
        string numero = Leer("\n Numero de registro (1 a " + total + "): ");
        if (!SoloDigitos(numero) || numero.Length > 6 || int.Parse(numero) < 1 || int.Parse(numero) > total)
        {
            Console.WriteLine(" [!] Numero fuera de rango.");
            return;
        }
        int n = int.Parse(numero) - 1;
        string[] c = LeerRegistroBinario(n);
        Console.WriteLine("\n Byte de inicio: " + (n * TAMANIO_REGISTRO));
        Console.WriteLine(" " + c[0] + " | " + c[3] + " | " + c[5] + " | " +
                          FormatoSoles(double.Parse(c[8], CULTURA)) + " | " + c[9]);
    }

    static void GenerarExpedientesPrueba(int cantidad)
    {
        string[] nombresBase = { "Maria Rojas", "Carlos Vega", "Ana Chavez", "Jorge Miranda",
                                 "Rosa Guzman", "Pedro Alvarado", "Lucia Flores" };
        Random azar = new Random(2026);
        for (int k = 0; k < cantidad; k++)
        {
            AgregarExpediente("EXP-2026-" + (k + 1).ToString().PadLeft(5, '0'),
                azar.Next(1, 29).ToString().PadLeft(2, '0') + "/" + azar.Next(1, 13).ToString().PadLeft(2, '0') + "/2026",
                azar.Next(10000000, 99999999).ToString(),
                nombresBase[azar.Next(nombresBase.Length)] + " " + k,
                "9" + azar.Next(10000000, 99999999),
                PROYECTOS[azar.Next(PROYECTOS.Length)],
                "MZ " + "ABCDE"[azar.Next(5)] + " - LT " + azar.Next(1, 41).ToString().PadLeft(2, '0'),
                TIPOS_DOCUMENTO[azar.Next(TIPOS_DOCUMENTO.Length)],
                Math.Round(500 + azar.NextDouble() * 39500, 2),
                ESTADOS[azar.Next(ESTADOS.Length)],
                "MESA DE PARTES");
        }
    }

    // Compara texto vs binario con datos ficticios; los reales se recuperan al final
    static void OpcionCompararArchivos()
    {
        string carpetaTemporal = Path.Combine(Path.GetTempPath(), "mesa_partes_cmp");
        Directory.CreateDirectory(carpetaTemporal);
        string copiaTxt = Path.Combine(carpetaTemporal, "copia.txt");
        GuardarTexto(copiaTxt);

        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine(" COMPARACION: ARCHIVO DE TEXTO vs ARCHIVO BINARIO");
        Console.WriteLine(Linea('=', 62));
        Console.WriteLine("REGISTROS".PadLeft(10) + "FORMATO".PadLeft(9) + "BYTES".PadLeft(10) +
                          "GUARDAR ms".PadLeft(12) + "CARGAR ms".PadLeft(11) + "ULTIMO ms".PadLeft(11));
        foreach (int cantidad in new int[] { 100, 1000, 10000 })
        {
            LimpiarDatos();
            GenerarExpedientesPrueba(cantidad);
            double[] r = CompararFormatos(Path.Combine(carpetaTemporal, "t.txt"),
                                          Path.Combine(carpetaTemporal, "b.dat"), 50);
            Console.WriteLine(cantidad.ToString().PadLeft(10) + "TEXTO".PadLeft(9) + r[0].ToString().PadLeft(10) +
                r[2].ToString("0.00", CULTURA).PadLeft(12) + r[4].ToString("0.00", CULTURA).PadLeft(11) +
                r[6].ToString("0.000", CULTURA).PadLeft(11));
            Console.WriteLine("".PadLeft(10) + "BINARIO".PadLeft(9) + r[1].ToString().PadLeft(10) +
                r[3].ToString("0.00", CULTURA).PadLeft(12) + r[5].ToString("0.00", CULTURA).PadLeft(11) +
                r[7].ToString("0.000", CULTURA).PadLeft(11));
        }
        CargarTexto(new List<string>(), copiaTxt);
        Console.WriteLine(Linea('-', 62));
        Console.WriteLine(" ULTIMO = tiempo para leer el ultimo registro del archivo.");
        Console.WriteLine(" El texto se lee y corrige con el Bloc de notas; el binario");
        Console.WriteLine(" llega a cualquier registro de un salto (Seek) sin leer los demas.");
        Console.WriteLine(Linea('=', 62));
    }

    // -------------------------------------------------------------------------
    // MENU PRINCIPAL
    // -------------------------------------------------------------------------

    static void MostrarMenu()
    {
        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine("   MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO");
        Console.WriteLine(Linea('=', 62));
        Console.WriteLine("   1) Registrar un expediente nuevo");
        Console.WriteLine("   2) Listar todos los expedientes");
        Console.WriteLine("   3) Buscar expediente (nombre, DNI, lote o codigo)");
        Console.WriteLine("   4) Buscar por codigo exacto  [busqueda binaria]");
        Console.WriteLine("   5) Ordenar expedientes");
        Console.WriteLine("   6) Cambiar el estado de un expediente");
        Console.WriteLine("   7) Reporte por proyecto y estado  [matriz]");
        Console.WriteLine("   8) Comparar los dos algoritmos de busqueda");
        Console.WriteLine("   9) Resumen por cliente  [arreglos paralelos]");
        Console.WriteLine("  10) Cargar datos de prueba");
        Console.WriteLine("  ---- Archivos ----------------------------------------");
        Console.WriteLine("  11) Crear respaldo binario  (" + ARCHIVO_BINARIO + ")");
        Console.WriteLine("  12) Restaurar desde el respaldo binario");
        Console.WriteLine("  13) Ver un registro del respaldo  [acceso directo]");
        Console.WriteLine("  14) Comparar archivo de texto vs binario");
        Console.WriteLine("   0) Guardar y salir");
        Console.WriteLine(Linea('=', 62));
    }

    static void Main(string[] argumentos)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        if (argumentos.Length > 0 && argumentos[0] == "--pruebas")
        {
            Environment.Exit(EjecutarPruebas());
        }

        List<string> avisos = new List<string>();
        int cargados = CargarTexto(avisos);
        Console.WriteLine("\n Se cargaron " + cargados + " expedientes desde " + ARCHIVO_TEXTO + ".");
        foreach (string aviso in avisos)
            Console.WriteLine("   [!] Linea descartada -> " + aviso);

        while (true)
        {
            MostrarMenu();
            string opcion = Leer(" Elija una opcion: ");
            bool modifica = false;
            switch (opcion)
            {
                case "1": OpcionRegistrar(); modifica = true; break;
                case "2": OpcionListar(); break;
                case "3": OpcionBuscar(); break;
                case "4": OpcionBuscarBinaria(); break;
                case "5": OpcionOrdenar(); break;
                case "6": OpcionCambiarEstado(); modifica = true; break;
                case "7": OpcionReporte(); break;
                case "8": OpcionComparar(); break;
                case "9": OpcionResumenClientes(); break;
                case "10": OpcionDatosPrueba(); modifica = true; break;
                case "11": OpcionRespaldoBinario(); break;
                case "12": OpcionRestaurarBinario(); break;
                case "13": OpcionVerRegistroBinario(); break;
                case "14": OpcionCompararArchivos(); break;
                case "0":
                    GuardarTexto();
                    Console.WriteLine("\n [OK] " + codigos.Count + " expedientes guardados en " +
                                      ARCHIVO_TEXTO + ". Sistema cerrado.\n");
                    return;
                default:
                    Console.WriteLine("\n [!] Opcion no valida. Elija un numero del menu.");
                    break;
            }
            if (modifica)
                GuardarTexto();
        }
    }
}

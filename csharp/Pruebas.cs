/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Pruebas.cs  -  Pruebas automaticas. Ejecutar:  dotnet run -- --pruebas
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.IO;

partial class Program
{
    static int pruebasOk = 0;
    static int pruebasFallidas = 0;

    static void Verificar(string nombre, bool condicion)
    {
        if (condicion) { pruebasOk++; Console.WriteLine(" [OK]    " + nombre); }
        else { pruebasFallidas++; Console.WriteLine(" [FALLA] " + nombre); }
    }

    static void CargarEjemplos()
    {
        LimpiarDatos();
        AgregarExpediente("EXP-2026-0003", "20/08/2026", "09876543", "Ana Lucía Chávez", "998877665", "VERDE MAR", "MZ B - LT 21", "MINUTA DE COMPRA VENTA", 24000.0, "EN REVISION", "AREA LEGAL");
        AgregarExpediente("EXP-2026-0001", "18/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "CONTRATO DE SEPARACION", 13000.0, "ATENDIDO", "MESA DE PARTES");
        AgregarExpediente("EXP-2026-0002", "19/07/2026", "72451890", "Carlos Vega Torres", "912345678", "MORELIA", "MZ C - LT 04", "VOUCHER DE PAGO", 2500.5, "RECIBIDO", "MESA DE PARTES");
        AgregarExpediente("EXP-2026-0004", "21/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "VOUCHER DE PAGO", 1200.0, "ATENDIDO", "CAJA");
    }

    static string Foto()
    {
        List<string> lineas = new List<string>();
        for (int i = 0; i < codigos.Count; i++)
            lineas.Add(LineaDeExpediente(i));
        return string.Join("\n", lineas);
    }

    static int EjecutarPruebas()
    {
        Console.WriteLine("\n PRUEBAS AUTOMATICAS - MESA DE PARTES (C#)\n");
        string carpeta = Path.Combine(Path.GetTempPath(), "mesa_partes_pruebas");
        Directory.CreateDirectory(carpeta);
        string txt = Path.Combine(carpeta, "exp.txt");
        string dat = Path.Combine(carpeta, "exp.dat");

        // Cadenas
        Verificar("Normalizar quita tildes y espacios", Normalizar("  José Ñuñez ") == "JOSE NUNEZ");
        Verificar("FormatoNombre", FormatoNombre("mARIA   rojas") == "Maria Rojas");
        Verificar("ContieneTexto ignora tildes", ContieneTexto("Ana Lucía Chávez", "chavez"));
        Verificar("DescomponerLote", DescomponerLote("MZ A - LT 12")[0] == "A" && DescomponerLote("MZ A - LT 12")[1] == "12");
        Verificar("FechaOrdenable", FechaOrdenable("18/08/2026") == "20260818");

        // Validaciones
        Verificar("DNI valido / invalido", ValidarDni("45781236").Item1 && !ValidarDni("4578123A").Item1);
        Verificar("Celular debe empezar con 9", ValidarTelefono("987654321").Item1 && !ValidarTelefono("887654321").Item1);
        Verificar("Nombre necesita apellido", ValidarNombre("Ana Chávez").Item1 && !ValidarNombre("Andy").Item1);
        Verificar("Monto con miles y dos puntos", ValidarMonto("13,000.50").Item1 && !ValidarMonto("1.2.3").Item1);
        Verificar("Fecha con bisiesto", ValidarFecha("29/02/2028").Item1 && !ValidarFecha("29/02/2026").Item1);

        // Algoritmos
        CargarEjemplos();
        var (encontrados, comp) = BusquedaLineal("rojas");
        Verificar("Busqueda lineal parcial", encontrados.Count == 2 && encontrados[0] == 1 && encontrados[1] == 3);
        var (orden, c1, m1) = OrdenarBurbuja("CODIGO", true);
        Verificar("Busqueda binaria encuentra", BusquedaBinaria(orden, "exp-2026-0002").Item1 == 2);
        Verificar("Busqueda binaria no encuentra", BusquedaBinaria(orden, "EXP-2026-9999").Item1 == -1);
        bool coinciden = true;
        foreach (string criterio in new string[] { "CODIGO", "FECHA", "CLIENTE", "MONTO", "PROYECTO" })
        {
            var a = OrdenarBurbuja(criterio, false).Item1;
            var b = OrdenarInsercion(criterio, false).Item1;
            for (int k = 0; k < a.Count; k++)
                if (Comparar(a[k], b[k], criterio) != 0) coinciden = false;
        }
        Verificar("Burbuja e insercion dan el mismo orden", coinciden);
        var porFecha = OrdenarInsercion("FECHA", true).Item1;
        Verificar("Orden por fecha es cronologico", codigos[porFecha[0]] == "EXP-2026-0002");

        // Reportes
        int[,] matriz = ConstruirMatrizProyectoEstado();
        Verificar("Matriz HUANCHACO/ATENDIDO = 2", matriz[0, 3] == 2);
        var (total, promedio, mayor, menor) = CalcularMontos();
        Verificar("Montos total/mayor/menor", total == 40700.5 && mayor == 24000.0 && menor == 1200.0);

        // Archivo de texto
        string antes = Foto();
        GuardarTexto(txt);
        string primera = File.ReadAllLines(txt)[0];
        Verificar("Formato de linea del archivo de texto",
            primera == "EXP-2026-0003|20/08/2026|09876543|Ana Lucía Chávez|998877665|VERDE MAR|MZ B - LT 21|MINUTA DE COMPRA VENTA|24000.00|EN REVISION|AREA LEGAL");
        List<string> avisos = new List<string>();
        Verificar("Texto: guardar y cargar devuelve lo mismo", CargarTexto(avisos, txt) == 4 && Foto() == antes && avisos.Count == 0);
        File.AppendAllText(txt, "linea|rota\n\nEXP-2026-0009|31/02/2026|12345678|X Y|900000000|NURA|L|T|10|RECIBIDO|R\n");
        avisos.Clear();
        Verificar("Texto: lineas danadas se descartan", CargarTexto(avisos, txt) == 4 && avisos.Count == 2);
        Verificar("Texto: archivo inexistente = 0", CargarTexto(new List<string>(), Path.Combine(carpeta, "no.txt")) == 0);

        // Archivo binario
        CargarEjemplos();
        GuardarBinario(dat);
        Verificar("Binario: 4 registros de 182 bytes", new FileInfo(dat).Length == 4 * TAMANIO_REGISTRO);
        Verificar("Binario: guardar y cargar devuelve lo mismo", CargarBinario(dat) == 4 && Foto() == antes);
        string[] reg = LeerRegistroBinario(2, dat);
        Verificar("Binario: acceso directo al registro 3", reg[0] == "EXP-2026-0002" && reg[8] == "2500.5");
        Verificar("Binario: fuera de rango = null", LeerRegistroBinario(4, dat) == null);
        nombres[0] = new string('Á', 30);
        GuardarBinario(dat);
        Verificar("Binario: recorta sin romper tildes", LeerRegistroBinario(0, dat)[3] == new string('Á', 20));

        Console.WriteLine("\n Resultado: " + pruebasOk + " OK, " + pruebasFallidas + " fallidas.");
        return pruebasFallidas == 0 ? 0 : 1;
    }
}

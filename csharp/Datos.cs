/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Datos.cs  -  Arreglos paralelos, catalogos y operaciones sobre los arreglos (tema 4)
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

partial class Program
{
    // =========================================================================
    // ARREGLOS PARALELOS DE EXPEDIENTES (TEMA 4)
    // // =========================================================================
    // // La posicion i de TODOS los arreglos corresponde al mismo expediente.
    // Regla de oro: al agregar un expediente hay que agregar en LOS 11 arreglos.
    static List<string> codigos = new List<string>();
    static List<string> fechas = new List<string>();
    static List<string> dnis = new List<string>();
    static List<string> nombres = new List<string>();
    static List<string> telefonos = new List<string>();
    static List<string> proyectos = new List<string>();
    static List<string> lotes = new List<string>();
    static List<string> tiposDocumento = new List<string>();
    static List<double> montos = new List<double>();
    static List<string> estados = new List<string>();
    static List<string> responsables = new List<string>();

    // Catalogos del negocio inmobiliario (Urbanistika)
    static readonly string[] ESTADOS = { "RECIBIDO", "EN REVISION", "OBSERVADO", "ATENDIDO" };
    static readonly string[] PROYECTOS = { "HUANCHACO", "MORELIA", "VERDE MAR", "CALIZA", "NURA", "OTRO" };
    static readonly string[] TIPOS_DOCUMENTO = { "CONTRATO DE SEPARACION", "MINUTA DE COMPRA VENTA",
                                                    "VOUCHER DE PAGO", "COPIA DE DNI", "SOLICITUD DE DESISTIMIENTO" };

    // Tablas para manipular cadenas
    const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÜÑ ";
    const string CON_TILDE = "ÁÉÍÓÚÜÑáéíóúüñ";
    const string SIN_TILDE = "AEIOUUNAEIOUUN";

    static readonly CultureInfo CULTURA = CultureInfo.InvariantCulture;
    // -----------------------------------------------------------------------------
    // OPERACIONES SOBRE LOS ARREGLOS PARALELOS
    // -----------------------------------------------------------------------------

    // Inserta un expediente: un dato en la MISMA posicion de cada arreglo
    static void AgregarExpediente(string codigo, string fecha, string dni, string nombre,
                                  string telefono, string proyecto, string lote, string tipo,
                                  double monto, string estado, string responsable)
    {
        codigos.Add(codigo);
        fechas.Add(fecha);
        dnis.Add(dni);
        nombres.Add(nombre);
        telefonos.Add(telefono);
        proyectos.Add(proyecto);
        lotes.Add(lote);
        tiposDocumento.Add(tipo);
        montos.Add(monto);
        estados.Add(estado);
        responsables.Add(responsable);
    }

    // Siguiente codigo correlativo del anio: EXP-2026-0001.
    // Se DESCOMPONE cada codigo existente para quedarse con el numero mas alto.
    static string GenerarCodigo()
    {
        string anio = DateTime.Now.Year.ToString();
        int mayor = 0;
        foreach (string codigo in codigos)
        {
            string[] partes = DescomponerCodigo(codigo);
            if (partes.Length == 3 && partes[1] == anio && SoloDigitos(partes[2]))
            {
                if (int.Parse(partes[2]) > mayor)
                    mayor = int.Parse(partes[2]);
            }
        }
        return "EXP-" + anio + "-" + (mayor + 1).ToString().PadLeft(4, '0');
    }

    static int PosicionPorCodigo(string codigo)
    {
        codigo = Normalizar(codigo);
        for (int i = 0; i < codigos.Count; i++)
        {
            if (codigos[i] == codigo)
                return i;
        }
        return -1;
    }

    // Posicion de un expediente ABIERTO con el mismo DNI, lote y tipo, o -1
    static int PosicionDuplicado(string dni, string lote, string tipo)
    {
        for (int i = 0; i < codigos.Count; i++)
        {
            if (dnis[i] == dni && Normalizar(lotes[i]) == Normalizar(lote)
                && tiposDocumento[i] == tipo && estados[i] != "ATENDIDO")
                return i;
        }
        return -1;
    }

    static void MostrarTabla(List<int> posiciones)
    {
        if (posiciones.Count == 0)
        {
            Console.WriteLine("\n (Sin expedientes para mostrar)");
            return;
        }

        string encabezado = "CODIGO".PadRight(15) + "FECHA".PadRight(12) + "DNI".PadRight(10) +
                            "CLIENTE".PadRight(24) + "PROYECTO".PadRight(11) +
                            "MONTO".PadLeft(14) + "  ESTADO";
        Console.WriteLine("\n" + encabezado);
        Console.WriteLine(Linea('-', encabezado.Length));
        foreach (int i in posiciones)
        {
            string nombre = nombres[i];
            if (nombre.Length > 22)
                nombre = nombre.Substring(0, 21) + ".";
            Console.WriteLine(codigos[i].PadRight(15) + fechas[i].PadRight(12) + dnis[i].PadRight(10) +
                              nombre.PadRight(24) + proyectos[i].PadRight(11) +
                              FormatoSoles(montos[i]).PadLeft(14) + "  " + estados[i]);
        }
        Console.WriteLine(Linea('-', encabezado.Length));
        Console.WriteLine(" Total de expedientes mostrados: " + posiciones.Count);
    }

    // Muestra un expediente leyendo la posicion i de los 11 arreglos,
    // con el codigo, la fecha y el lote DESCOMPUESTOS en partes.
    static void MostrarFicha(int i)
    {
        string[] partesCodigo = DescomponerCodigo(codigos[i]);
        string[] partesFecha = DescomponerFecha(fechas[i]);
        string[] partesLote = DescomponerLote(lotes[i]);

        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine(" FICHA DEL EXPEDIENTE " + codigos[i] + "   (posicion " + i + ")");
        Console.WriteLine(Linea('=', 62));
        Console.WriteLine(" CODIGO      : " + codigos[i] + "  -> anio " + partesCodigo[1] +
                          ", correlativo " + partesCodigo[2]);
        Console.WriteLine(" FECHA       : " + fechas[i] + "  -> dia " + partesFecha[0] +
                          ", mes " + partesFecha[1] + ", anio " + partesFecha[2]);
        Console.WriteLine(" DNI         : " + dnis[i]);
        Console.WriteLine(" NOMBRE      : " + nombres[i]);
        Console.WriteLine(" TELEFONO    : " + telefonos[i]);
        Console.WriteLine(" PROYECTO    : " + proyectos[i]);
        if (partesLote[1].Length > 0)
            Console.WriteLine(" LOTE        : " + lotes[i] + "  -> manzana " + partesLote[0] +
                              ", lote " + partesLote[1]);
        else
            Console.WriteLine(" LOTE        : " + lotes[i]);
        Console.WriteLine(" TIPO DOC    : " + tiposDocumento[i]);
        Console.WriteLine(" MONTO       : " + FormatoSoles(montos[i]));
        Console.WriteLine(" ESTADO      : " + estados[i]);
        Console.WriteLine(" RESPONSABLE : " + responsables[i]);
        Console.WriteLine(Linea('=', 62));
    }

    // Devuelve [0, 1, 2, ..., n-1]: el orden en que se registraron
    static List<int> TodasLasPosiciones()
    {
        List<int> posiciones = new List<int>();
        for (int i = 0; i < codigos.Count; i++)
            posiciones.Add(i);
        return posiciones;
    }
}

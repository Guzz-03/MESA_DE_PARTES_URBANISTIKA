/*
 MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO  (C#)
 Archivo: Interfaz.cs  -  Entrada por teclado y opciones del menu
 Fundamentos de Programacion - CIIN1205P - UPN Trujillo 2026 - Grupo 1
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

partial class Program
{
    // -----------------------------------------------------------------------------
    // ENTRADA DE DATOS
    // -----------------------------------------------------------------------------


    // Lee una linea; si se cierra la entrada (Ctrl+Z / fin de archivo) termina.
    static string Leer(string mensaje)
    {
        Console.Write(mensaje);
        string linea = Console.ReadLine();
        if (linea == null)
        {
            Console.WriteLine("\n Sistema cerrado.");
            Environment.Exit(0);
        }
        return linea.Trim();
    }

    // Pide un dato y NO avanza hasta que sea valido. X = cancelar (devuelve null).
    // Recibe como parametro el metodo que valida el dato (reutilizacion).
    static string PedirDato(string mensaje, Func<string, (bool, string)> validador)
    {
        while (true)
        {
            string valor = Leer(mensaje);
            if (valor.ToUpper() == "X")
                return null;
            var (esValido, aviso) = validador(valor);
            if (esValido)
                return valor.Replace("|", " ");
            Console.WriteLine("   [!] " + aviso + "  (escriba X para cancelar)");
        }
    }

    // Muestra un catalogo numerado y devuelve la opcion elegida (null = cancelar)
    static string ElegirOpcion(string titulo, string[] opciones)
    {
        while (true)
        {
            Console.WriteLine("\n   " + titulo);
            for (int i = 0; i < opciones.Length; i++)
                Console.WriteLine("     " + (i + 1) + ") " + opciones[i]);
            string elegida = Leer("   Numero (X = cancelar): ");
            if (elegida.ToUpper() == "X")
                return null;
            if (SoloDigitos(elegida) && elegida.Length <= 3)
            {
                int numero = int.Parse(elegida);
                if (numero >= 1 && numero <= opciones.Length)
                    return opciones[numero - 1];
            }
            Console.WriteLine("   [!] Elija un numero entre 1 y " + opciones.Length + ".");
        }
    }

    static bool HayExpedientes()
    {
        if (codigos.Count == 0)
        {
            Console.WriteLine("\n [!] Todavia no hay expedientes registrados.");
            return false;
        }
        return true;
    }

    static string Linea(char caracter, int veces)
    {
        return new string(caracter, veces);
    }

    // =========================================================================
    // OPCIONES DEL MENU
    // =========================================================================

    static void OpcionRegistrar()
    {
        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine(" REGISTRO DE NUEVO EXPEDIENTE   (escriba X para cancelar)");
        Console.WriteLine(Linea('=', 62));

        string dni = PedirDato(" DNI del cliente (8 digitos): ", ValidarDni);
        if (dni == null)
            return;
        string nombre = PedirDato(" Nombres y apellidos: ", ValidarNombre);
        if (nombre == null)
            return;
        nombre = FormatoNombre(nombre);
        string telefono = PedirDato(" Celular (9 digitos): ", ValidarTelefono);
        if (telefono == null)
            return;
        string proyecto = ElegirOpcion("Proyecto:", PROYECTOS);
        if (proyecto == null)
            return;

        string lote = Leer(" Lote (ejemplo: MZ A - LT 12): ").ToUpper().Replace("|", " ");
        if (lote == "X")
            return;
        if (lote.Length == 0)
            lote = "SIN ASIGNAR";

        string tipo = ElegirOpcion("Tipo de documento presentado:", TIPOS_DOCUMENTO);
        if (tipo == null)
            return;

        int repetido = PosicionDuplicado(dni, lote, tipo);
        if (repetido != -1)
        {
            Console.WriteLine("\n [!] ATENCION: ya existe un expediente abierto con esos datos:");
            Console.WriteLine("     " + codigos[repetido] + " | " + nombres[repetido] + " | " + estados[repetido]);
            if (Leer(" Desea registrarlo igual? (S/N): ").ToUpper() != "S")
            {
                Console.WriteLine(" Registro cancelado.");
                return;
            }
        }

        string textoMonto = PedirDato(" Monto de la operacion en soles: ", ValidarMonto);
        if (textoMonto == null)
            return;
        double monto = Math.Round(double.Parse(LimpiarMonto(textoMonto), CULTURA), 2);

        string hoy = DateTime.Now.ToString("dd/MM/yyyy", CULTURA);
        string fecha;
        while (true)
        {
            fecha = Leer(" Fecha dd/mm/aaaa [Enter = " + hoy + "]: ");
            if (fecha.Length == 0)
                fecha = hoy;
            if (fecha.ToUpper() == "X")
                return;
            var (esValida, aviso) = ValidarFecha(fecha);
            if (esValida)
                break;
            Console.WriteLine("   [!] " + aviso);
        }

        string responsable = Leer(" Responsable que recibe [Enter = MESA DE PARTES]: ").ToUpper();
        if (responsable.Length == 0)
            responsable = "MESA DE PARTES";

        string codigo = GenerarCodigo();
        AgregarExpediente(codigo, fecha, dni, nombre, telefono, proyecto, lote,
                          tipo, monto, "RECIBIDO", responsable);
        Console.WriteLine("\n [OK] Expediente registrado con el codigo: " + codigo);
    }

    static void OpcionListar()
    {
        if (!HayExpedientes())
            return;
        MostrarTabla(TodasLasPosiciones());
        var (total, promedio, mayor, menor) = CalcularMontos();
        Console.WriteLine(" Monto total    : " + FormatoSoles(total));
        Console.WriteLine(" Monto promedio : " + FormatoSoles(promedio));
        Console.WriteLine(" Pendientes     : " + ContarPendientes() + " de " + codigos.Count);
    }

    static void OpcionBuscar()
    {
        string texto = Leer("\n Escriba nombre, DNI, lote, proyecto o codigo: ");
        if (texto.Length == 0)
        {
            Console.WriteLine(" [!] No escribio nada que buscar.");
            return;
        }
        var (encontrados, comparaciones) = BusquedaLineal(texto);
        if (encontrados.Count == 0)
            Console.WriteLine("\n No se encontro ningun expediente con '" + texto + "'.");
        else
        {
            MostrarTabla(encontrados);
            if (encontrados.Count == 1)
                MostrarFicha(encontrados[0]);
        }
        Console.WriteLine(" [Metrica] Busqueda lineal: " + comparaciones + " comparaciones.");
    }

    static void OpcionBuscarBinaria()
    {
        if (!HayExpedientes())
            return;
        string codigo = Leer("\n Codigo exacto (ejemplo EXP-2026-0001): ");
        if (codigo.Length == 0)
        {
            Console.WriteLine(" [!] No escribio ningun codigo.");
            return;
        }
        var (orden, compOrden, _) = OrdenarBurbuja("CODIGO", true);
        var (posicion, compBusq) = BusquedaBinaria(orden, codigo);
        if (posicion == -1)
            Console.WriteLine("\n No existe el expediente " + codigo.ToUpper() + ".");
        else
            MostrarFicha(posicion);
        Console.WriteLine(" [Metrica] Ordenamiento previo: " + compOrden + " comparaciones.");
        Console.WriteLine(" [Metrica] Busqueda binaria   : " + compBusq + " comparaciones.");
    }

    static void OpcionOrdenar()
    {
        if (!HayExpedientes())
            return;
        Console.WriteLine("\n Ordenar por:");
        Console.WriteLine("   1) Codigo    2) Fecha    3) Cliente    4) Monto    5) Proyecto");
        string[] criterios = { "CODIGO", "FECHA", "CLIENTE", "MONTO", "PROYECTO" };
        string elegido = Leer(" Numero: ");
        if (elegido.Length != 1 || elegido[0] < '1' || elegido[0] > '5')
        {
            Console.WriteLine(" [!] Opcion no valida.");
            return;
        }
        string criterio = criterios[int.Parse(elegido) - 1];
        bool ascendente = Leer(" Ascendente (A) o descendente (D)? [A]: ").ToUpper() != "D";
        Console.WriteLine("\n Algoritmo:  1) Burbuja    2) Insercion");

        List<int> orden;
        string detalle;
        if (Leer(" Numero [1]: ") == "2")
        {
            var (ordenIns, comparaciones, movimientos) = OrdenarInsercion(criterio, ascendente);
            orden = ordenIns;
            detalle = "INSERCION: " + comparaciones + " comparaciones y " + movimientos + " desplazamientos.";
        }
        else
        {
            var (ordenBur, comparaciones, movimientos) = OrdenarBurbuja(criterio, ascendente);
            orden = ordenBur;
            detalle = "BURBUJA: " + comparaciones + " comparaciones y " + movimientos + " intercambios.";
        }
        MostrarTabla(orden);
        Console.WriteLine(" [Metrica] " + detalle);
    }

    // Se cambia el dato en la posicion i del arreglo 'estados'
    static void OpcionCambiarEstado()
    {
        if (!HayExpedientes())
            return;
        string codigo = Leer("\n Codigo del expediente: ");
        int i = PosicionPorCodigo(codigo);
        if (i == -1)
        {
            Console.WriteLine("\n [!] No existe el expediente " + codigo.ToUpper() + ".");
            return;
        }
        Console.WriteLine(" Estado actual: " + estados[i]);
        string nuevo = ElegirOpcion("Nuevo estado:", ESTADOS);
        if (nuevo == null)
            return;
        if (nuevo == estados[i])
        {
            Console.WriteLine("\n [!] El expediente ya esta en estado " + nuevo + ".");
            return;
        }
        estados[i] = nuevo;
        Console.WriteLine("\n [OK] El expediente " + codigos[i] + " ahora esta en estado: " + nuevo);
    }

    static void OpcionReporte()
    {
        if (!HayExpedientes())
            return;
        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine(" REPORTE DE EXPEDIENTES POR PROYECTO Y ESTADO");
        Console.WriteLine(Linea('=', 62));
        MostrarMatriz(ConstruirMatrizProyectoEstado());
        var (total, promedio, mayor, menor) = CalcularMontos();
        Console.WriteLine("\n Monto total registrado : " + FormatoSoles(total));
        Console.WriteLine(" Monto promedio         : " + FormatoSoles(promedio));
        Console.WriteLine(" Operacion mas alta     : " + FormatoSoles(mayor));
        Console.WriteLine(" Operacion mas baja     : " + FormatoSoles(menor));
        Console.WriteLine(" Expedientes pendientes : " + ContarPendientes());
    }

    // Mide las dos alternativas de busqueda sobre el mismo dato
    static void OpcionComparar()
    {
        if (!HayExpedientes())
            return;
        string codigo = Leer("\n Codigo a buscar con ambos algoritmos: ");
        if (codigo.Length == 0)
        {
            Console.WriteLine(" [!] No escribio ningun codigo.");
            return;
        }
        var (encontrados, compLineal) = BusquedaLineal(codigo);
        var (orden, compOrden, _) = OrdenarBurbuja("CODIGO", true);
        var (posicion, compBinaria) = BusquedaBinaria(orden, codigo);

        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine(" COMPARACION DE ALTERNATIVAS ALGORITMICAS");
        Console.WriteLine(Linea('=', 62));
        Console.WriteLine(" Expedientes en los arreglos : " + codigos.Count);
        Console.WriteLine();
        Console.WriteLine(" ALTERNATIVA A - Busqueda lineal");
        Console.WriteLine("   Comparaciones          : " + compLineal);
        Console.WriteLine("   Encontro el expediente : " + (encontrados.Count > 0 ? "SI" : "NO"));
        Console.WriteLine("   Requiere orden previo  : NO");
        Console.WriteLine("   Admite busqueda parcial: SI");
        Console.WriteLine();
        Console.WriteLine(" ALTERNATIVA B - Busqueda binaria");
        Console.WriteLine("   Comparaciones          : " + compBinaria);
        Console.WriteLine("   Encontro el expediente : " + (posicion != -1 ? "SI" : "NO"));
        Console.WriteLine("   Costo de ordenar antes : " + compOrden + " comparaciones");
        Console.WriteLine("   Admite busqueda parcial: NO (solo coincidencia exacta)");
        Console.WriteLine();
        Console.WriteLine(" CONCLUSION: la binaria gana en la busqueda por codigo cuando el");
        Console.WriteLine(" arreglo ya esta ordenado. Si hay que ordenar en cada consulta, la");
        Console.WriteLine(" lineal conviene con pocos registros.");
        Console.WriteLine(Linea('=', 62));
    }

    static void OpcionResumenClientes()
    {
        if (!HayExpedientes())
            return;
        List<string> dnisCliente = new List<string>();
        List<string> nombresCliente = new List<string>();
        List<int> cantidades = new List<int>();
        List<double> totales = new List<double>();
        ResumenPorCliente(dnisCliente, nombresCliente, cantidades, totales);
        OrdenarClientesPorTotal(dnisCliente, nombresCliente, cantidades, totales);

        Console.WriteLine("\n" + Linea('=', 62));
        Console.WriteLine(" RESUMEN POR CLIENTE (de mayor a menor monto pagado)");
        Console.WriteLine(Linea('=', 62));
        string encabezado = "DNI".PadRight(10) + "CLIENTE".PadRight(24) +
                            "EXPEDIENTES".PadLeft(12) + "TOTAL PAGADO".PadLeft(16);
        Console.WriteLine(encabezado);
        Console.WriteLine(Linea('-', encabezado.Length));
        for (int i = 0; i < dnisCliente.Count; i++)
        {
            Console.WriteLine(dnisCliente[i].PadRight(10) + nombresCliente[i].PadRight(24) +
                              cantidades[i].ToString().PadLeft(12) + FormatoSoles(totales[i]).PadLeft(16));
        }
        Console.WriteLine(Linea('-', encabezado.Length));
        Console.WriteLine(" Clientes distintos: " + dnisCliente.Count);
    }

    // Carga expedientes ficticios para demostrar el sistema
    static void OpcionDatosPrueba()
    {
        if (codigos.Count > 0)
        {
            if (Leer("\n Ya hay " + codigos.Count +
                     " expedientes. Agregar los de prueba igual? (S/N): ").ToUpper() != "S")
                return;
        }
        AgregarExpediente(GenerarCodigo(), "18/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "CONTRATO DE SEPARACION", 13000.0, "ATENDIDO", "MESA DE PARTES");
        AgregarExpediente(GenerarCodigo(), "19/08/2026", "72451890", "Carlos Vega Torres", "912345678", "MORELIA", "MZ C - LT 04", "VOUCHER DE PAGO", 2500.0, "RECIBIDO", "MESA DE PARTES");
        AgregarExpediente(GenerarCodigo(), "20/08/2026", "09876543", "Ana Lucia Chavez", "998877665", "VERDE MAR", "MZ B - LT 21", "MINUTA DE COMPRA VENTA", 24000.0, "EN REVISION", "AREA LEGAL");
        AgregarExpediente(GenerarCodigo(), "21/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "VOUCHER DE PAGO", 1200.0, "ATENDIDO", "CAJA");
        AgregarExpediente(GenerarCodigo(), "22/08/2026", "60123456", "Jorge Miranda Silva", "955443322", "CALIZA", "MZ D - LT 08", "COPIA DE DNI", 500.0, "OBSERVADO", "MESA DE PARTES");
        AgregarExpediente(GenerarCodigo(), "25/08/2026", "31245678", "Rosa Guzman Polo", "976543210", "NURA", "MZ E - LT 15", "CONTRATO DE SEPARACION", 18500.0, "RECIBIDO", "MESA DE PARTES");
        AgregarExpediente(GenerarCodigo(), "26/08/2026", "80654321", "Pedro Alvarado Ruiz", "934567890", "MORELIA", "MZ C - LT 09", "MINUTA DE COMPRA VENTA", 31000.0, "EN REVISION", "AREA LEGAL");
        AgregarExpediente(GenerarCodigo(), "27/08/2026", "27418596", "Lucia Flores Chavez", "921098765", "HUANCHACO", "MZ A - LT 30", "SOLICITUD DE DESISTIMIENTO", 13000.0, "OBSERVADO", "GERENCIA");
        Console.WriteLine("\n [OK] Se agregaron 8 expedientes de prueba.");
    }
}

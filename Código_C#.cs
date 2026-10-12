/*
===============================================================================
MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO
Version del TEMA 4 en C#: cadenas de caracteres y arreglos paralelos

Curso  : Fundamentos de Programacion - CIIN1205P
Docente: Humberto Pedro Jimenez Jara
Grupo 1: Alvarado Valiente Valeria / Flores Chavez Anahi /
        Guzman Polo Carol Jacqueline / Hernandez Vasquez Andy Rafael /
        Valderrama Rosado Aldahir Obved
UPN - Trujillo, 2026
===============================================================================
*/
class Program
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

    // --- Validaciones: devuelven una tupla (esValido, mensaje) --------------

    static (bool, string) ValidarDni(string dni)
    {
        if (dni.Length != 8)
            return (false, "El DNI debe tener exactamente 8 digitos.");
        if (!SoloDigitos(dni))
            return (false, "El DNI solo puede contener numeros.");
        return (true, "OK");
    }

    static (bool, string) ValidarTelefono(string telefono)
    {
        if (telefono.Length != 9)
            return (false, "El celular debe tener exactamente 9 digitos.");
        if (!SoloDigitos(telefono))
            return (false, "El celular solo puede contener numeros.");
        if (telefono[0] != '9')
            return (false, "El celular debe empezar con 9.");
        return (true, "OK");
    }

    static (bool, string) ValidarNombre(string nombre)
    {
        if (nombre.Length < 5)
            return (false, "El nombre debe tener al menos 5 caracteres.");
        foreach (char caracter in Normalizar(nombre))
        {
            if (LETRAS.IndexOf(caracter) == -1)
                return (false, "El nombre solo puede contener letras y espacios.");
        }
        if (nombre.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length < 2)
            return (false, "Ingrese nombre y apellido.");
        return (true, "OK");
    }

    static (bool, string) ValidarMonto(string monto)
    {
        monto = LimpiarMonto(monto);
        if (monto.Length == 0)
            return (false, "El monto no puede estar vacio.");
        int puntos = 0;
        int digitos = 0;
        foreach (char caracter in monto)
        {
            if (caracter == '.')
                puntos = puntos + 1;
            else if (caracter < '0' || caracter > '9')
                return (false, "El monto solo admite numeros y un punto decimal.");
            else
                digitos = digitos + 1;
        }
        if (puntos > 1)
            return (false, "El monto tiene mas de un punto decimal.");
        if (digitos == 0)
            return (false, "El monto debe tener al menos un numero.");
        if (double.Parse(monto, CULTURA) <= 0)
            return (false, "El monto debe ser mayor que cero.");
        return (true, "OK");
    }

    // Valida dd/mm/aaaa descomponiendo la cadena, incluido el anio bisiesto
    static (bool, string) ValidarFecha(string fecha)
    {
        if (fecha.Length != 10 || fecha[2] != '/' || fecha[5] != '/')
            return (false, "La fecha debe tener el formato dd/mm/aaaa.");

        string[] partes = DescomponerFecha(fecha);
        if (!SoloDigitos(partes[0]) || !SoloDigitos(partes[1]) || !SoloDigitos(partes[2]))
            return (false, "La fecha solo puede contener numeros y '/'.");

        int dia = int.Parse(partes[0]);
        int mes = int.Parse(partes[1]);
        int anio = int.Parse(partes[2]);

        if (mes < 1 || mes > 12)
            return (false, "El mes debe estar entre 01 y 12.");
        if (anio < 2020 || anio > 2030)
            return (false, "El anio debe estar entre 2020 y 2030.");

        int[] diasPorMes = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        int tope = diasPorMes[mes - 1];
        bool bisiesto = (anio % 4 == 0 && anio % 100 != 0) || (anio % 400 == 0);
        if (mes == 2 && bisiesto)
            tope = 29;
        if (dia < 1 || dia > tope)
            return (false, "El dia no existe para ese mes (maximo " + tope + ").");
        return (true, "OK");
    }

    // -----------------------------------------------------------------------------
    // ENTRADA DE DATOS
    // -----------------------------------------------------------------------------
    // Lee una linea; si se cierra la entrada (Ctrl+Z / fin de archivo) termina.


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


    // -----------------------------------------------------------------------------
    // REPORTES
    // -----------------------------------------------------------------------------

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
        Console.WriteLine("   0) Salir");
        Console.WriteLine(Linea('=', 62));
    }

    // El menu se repite hasta que el usuario elige 0
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            MostrarMenu();
            string opcion = Leer(" Elija una opcion: ");
            switch (opcion)
            {
                case "1": OpcionRegistrar(); break;
                case "2": OpcionListar(); break;
                case "3": OpcionBuscar(); break;
                case "4": OpcionBuscarBinaria(); break;
                case "5": OpcionOrdenar(); break;
                case "6": OpcionCambiarEstado(); break;
                case "7": OpcionReporte(); break;
                case "8": OpcionComparar(); break;
                case "9": OpcionResumenClientes(); break;
                case "10": OpcionDatosPrueba(); break;
                case "0":
                    Console.WriteLine("\n Sistema cerrado. Los datos se guardan en archivo a partir del tema 5.\n");
                    return;
                default:
                    Console.WriteLine("\n [!] Opcion no valida. Elija un numero del menu.");
                    break;
            }
        }
    }
}
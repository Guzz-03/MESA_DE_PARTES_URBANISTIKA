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

    // -----------------------------------------------------------------------------
    // VALIDACIONES Y CADENAS
    // -----------------------------------------------------------------------------

    static bool SoloDigitos(string texto)
    {
        if (texto.Length == 0) return false;
        bool ok = true;
        for (int i = 0; i < texto.Length; i++)
        {
            char c = texto[i];
            if (c < '0' || c > '9')
            {
                ok = false;
            }
        }
        return ok;
    }

    static string LimpiarMonto(string monto)
    {
        string limpio = "";
        for (int i = 0; i < monto.Length; i++)
        {
            char c = monto[i];
            if (c != ',' && c != ' ')
            {
                limpio += c;
            }
        }
        return limpio;
    }

    static string LimpiarSeparador(string texto)
    {
        string limpio = "";
        for (int i = 0; i < texto.Length; i++)
        {
            char c = texto[i];
            if (c == '|')
            {
                limpio += " ";
            }
        else
            {
                limpio += c;
            }
        }
        return limpio;
    }

    static int ContarPalabras(string texto)
    {
        int total = 0;
        bool dentro = false;
        for (int i = 0; i < texto.Length; i++)
        {
            char c = texto[i];
            if (c == ' ')
            {
                dentro = false;
            }
            else
            {
                if (!dentro)
                {
                    total++;
                }
                dentro = true;
            }
        }
        return total;
    }

    static bool ValidarDni(string dni, ref string mensaje)
    {
        if (dni.Length != 8)
        {
            mensaje = "El DNI debe tener exactamente 8 digitos.";
            return false;
        }
        if (!SoloDigitos(dni))
        {
            mensaje = "El DNI solo puede contener numeros.";
            return false;
        }
        return true;
    }

    static bool ValidarTelefono(string tel, ref string mensaje)
    {
        if (tel.Length != 9)
        {
            mensaje = "El celular debe tener exactamente 9 digitos.";
            return false;
        }
        if (!SoloDigitos(tel))
        {
            mensaje = "El celular solo puede contener numeros.";
            return false;
        }
        if (tel[0] != '9')
        {
            mensaje = "El celular debe empezar con 9.";
            return false;
        }
        return true;
    }

    static bool ValidarNombre(string nombre, ref string mensaje)
    {
        if (nombre.Length < 5)
        {
            mensaje = "El nombre debe tener al menos 5 caracteres.";
            return false;
        }
        string revisado = nombre.ToUpper();
        bool ok = true;
        for (int i = 0; i < revisado.Length; i++)
        {
            char c = revisado[i];
            if ((c < 'A' || c > 'Z') && c != ' ')
            {
                ok = false;
            }
        }
        if (!ok)
        {
            mensaje = "El nombre solo puede contener letras y espacios.";
            return false;
        }
        if (ContarPalabras(nombre) < 2)
        {
            mensaje = "Ingrese nombre y apellido.";
            return false;
        }
        return true;
    }

    static bool ValidarMonto(string texto, ref string mensaje)
    {
        string monto = LimpiarMonto(texto);
        int puntos = 0;
        int digitos = 0;
        if (monto.Length == 0)
        {
            mensaje = "El monto no puede estar vacio.";
            return false;
        }
        bool ok = true;
        for (int i = 0; i < monto.Length; i++)
        {
            char c = monto[i];
            if (c == '.')
            {
                puntos++;
            }
            else
            {
                if (c < '0' || c > '9')
                {
                    ok = false;
                }
                else
                {
                    digitos++;
                }
            }
        }
        if (!ok)
        {
            mensaje = "El monto solo admite numeros y un punto decimal.";
            return false;
        }
        if (puntos > 1)
        {
            mensaje = "El monto tiene mas de un punto decimal.";
            return false;
        }
        if (digitos == 0)
        {
            mensaje = "El monto debe tener al menos un numero.";
            return false;
        }
        if (double.Parse(monto, System.Globalization.CultureInfo.InvariantCulture) <= 0)
        {
            mensaje = "El monto debe ser mayor que cero.";
            return false;
        }
            return true;
        }

    static bool ValidarFecha(string fecha, ref string mensaje)
    {
        if (fecha.Length != 10)
        {
            mensaje = "La fecha debe tener el formato dd/mm/aaaa.";
            return false;
        }
        if (fecha[2] != '/' || fecha[5] != '/')
        {
            mensaje = "La fecha debe usar / como separador: dd/mm/aaaa.";
            return false;
        }
        string pDia = fecha.Substring(0, 2);
        string pMes = fecha.Substring(3, 2);
        string pAnio = fecha.Substring(6, 4);
        if (!SoloDigitos(pDia) || !SoloDigitos(pMes) || !SoloDigitos(pAnio))
        {
            mensaje = "La fecha solo puede contener numeros y /.";
            return false;
        }
        int dia = int.Parse(pDia);
        int mes = int.Parse(pMes);
        int anio = int.Parse(pAnio);
        if (mes < 1 || mes > 12)
        {
            mensaje = "El mes debe estar entre 01 y 12.";
            return false;
        }
        if (anio < 2020 || anio > 2030)
        {
            mensaje = "El anio debe estar entre 2020 y 2030.";
            return false;
        }

        int tope = 31;
        switch (mes)
        {
            case 2:
                tope = 28;
                break;
            case 4: case 6: case 9: case 11:
                tope = 30;
                break;
            default:
                tope = 31;
                break;
        }
        bool bisiesto = (anio % 4 == 0 && anio % 100 != 0) || (anio % 400 == 0);
        if (mes == 2 && bisiesto)
        {
            tope = 29;
        }
        if (dia < 1 || dia > tope)
        {
            mensaje = "El dia no existe para ese mes (maximo " + tope + ").";
            return false;
        }
        return true;
    }

    static string RellenarCeros(double numero, int largo)
    {
        string texto = ((long)numero).ToString();
        while (texto.Length < largo)
        {
            texto = "0" + texto;
        }
        return texto;
    }

    static string FechaDeHoy()
    {
        DateTime now = DateTime.Now;
        string t = now.ToString("yyyyMMdd");
        return t.Substring(6, 2) + "/" + t.Substring(4, 2) + "/" + t.Substring(0, 4);
    }

    static long FechaActualInt()
    {
        return long.Parse(DateTime.Now.ToString("yyyyMMdd"));
    }

    static bool ContieneTexto(string texto, string buscado)
    {
        texto = texto.ToUpper();
        buscado = buscado.ToUpper();
        bool ok = false;
        if (buscado.Length > 0 && buscado.Length <= texto.Length)
        {
            int inicio = 1;
            while (!ok && inicio <= texto.Length - buscado.Length + 1)
            {
                bool coincide = true;
                int k = 1;
                while (coincide && k <= buscado.Length)
                {
                    if (texto[inicio + k - 2] != buscado[k - 1])
                    {
                        coincide = false;
                    }
                    k++;
                }
                if (coincide)
                {
                    ok = true;
                }
                inicio++;
            }
        }
        return ok;
    }

    // -----------------------------------------------------------------------------
    // ENTRADA DE DATOS
    // -----------------------------------------------------------------------------

    static int ElegirOpcion(string titulo, string[] opciones, int total)
    {
        int elegida = 99;
        do
        {
            Console.WriteLine("   " + titulo);
            for (int i = 1; i <= total; i++)
            {
                Console.WriteLine("     " + i + ") " + opciones[i]);
            }
            Console.Write("   Numero (X = cancelar): ");
            string texto = Console.ReadLine();
            if (texto.ToUpper() == "X")
            {
                elegida = 0;
            }
            else
            {
                if (SoloDigitos(texto))
                {
                    int val = int.Parse(texto);
                    if (val >= 1 && val <= total)
                    {
                        elegida = val;
                    }
                    else
                    {
                        Console.WriteLine("   [!] La opcion debe estar entre 1 y " + total + ".");
                    }
                    }
                else
                {
                    Console.WriteLine("   [!] Ingrese solo el numero de la opcion.");
                }
            }
        } while (elegida == 99);
        return elegida;
    }

    static bool HayExpedientes(int n)
    {
        bool hay = n > 0;
        if (!hay)
        {
            Console.WriteLine();
            Console.WriteLine(" [!] Todavia no hay expedientes registrados.");
        }
        return hay;
    }

    // -----------------------------------------------------------------------------
    // DESCOMPONER CADENAS
    // -----------------------------------------------------------------------------

    static string ExtraerParte(string texto, char separador, int numeroParte)
    {
        string parte = "";
        int actual = 1;
        for (int i = 0; i < texto.Length; i++)
        {
            char c = texto[i];
            if (c == separador)
            {
                actual++;
            }
            else
            {
                if (actual == numeroParte)
                {
                    parte += c;
                }
            }
        }
        return parte;
    }

    static string Recortar(string texto)
    {
        int ini = 1;
        int fn = texto.Length;
        while (ini <= fn && texto[ini - 1] == ' ')
        {
            ini++;
        }
        while (fn >= ini && texto[fn - 1] == ' ')
        {
            fn--;
        }
        string limpio = "";
        if (ini <= fn)
        {
            limpio = texto.Substring(ini - 1, fn - ini + 1);
        }
        return limpio;
    }

    static string QuitarPrefijo(string texto, string prefijo)
    {
        string resto = texto;
        if (texto.Length >= prefijo.Length)
        {
            if (texto.Substring(0, prefijo.Length) == prefijo)
            {
                resto = Recortar(texto.Substring(prefijo.Length));
            }
        }
        return resto;
    }

    static string FechaOrdenable(string fecha)
    {
        return ExtraerParte(fecha, '/', 3) + ExtraerParte(fecha, '/', 2) + ExtraerParte(fecha, '/', 1);
    }

    // -----------------------------------------------------------------------------
    // OPERACIONES SOBRE LOS ARREGLOS PARALELOS
    // -----------------------------------------------------------------------------

    static void AgregarExpediente(ref int n, string[] codigos, string[] fechas, string[] dnis, string[] nombres, string[] telefonos, string[] proyectos, string[] lotes, string[] tipos, double[] montos, string[] estados, string[] responsables, string cod, string fec, string dni, string nom, string tel, string pro, string lot, string tip, double mon, string est, string res)
    {
        n++;
        codigos[n] = cod;
        fechas[n] = fec;
        dnis[n] = dni;
        nombres[n] = nom;
        telefonos[n] = tel;
        proyectos[n] = pro;
        lotes[n] = lot;
        tipos[n] = tip;
        montos[n] = mon;
        estados[n] = est;
        responsables[n] = res;
    }

    static string GenerarCodigo(string[] codigos, int n)
    {
        string anio = ((int)(FechaActualInt() / 10000)).ToString();
        int mayor = 0;
        for (int i = 1; i <= n; i++)
        {
            if (ExtraerParte(codigos[i], '-', 2) == anio && SoloDigitos(ExtraerParte(codigos[i], '-', 3)))
            {
                int val = int.Parse(ExtraerParte(codigos[i], '-', 3));
                if (val > mayor)
                {
                    mayor = val;
                }
            }
        }
        return "EXP-" + anio + "-" + RellenarCeros(mayor + 1, 4);
    }

    static int PosicionPorCodigo(string[] codigos, int n, string codigo)
    {
        int posicion = 0;
        for (int i = 1; i <= n; i++)
        {
            if (posicion == 0 && codigos[i] == codigo.ToUpper())
            {
                posicion = i;
            }
        }
        return posicion;
    }

    static int PosicionDuplicado(string[] dnis, string[] lotes, string[] tipos, string[] estados, int n, string dni, string lote, string tipo)
    {
        int posicion = 0;
        for (int i = 1; i <= n; i++)
        {
            if (posicion == 0 && dnis[i] == dni && lotes[i] == lote && tipos[i] == tipo && estados[i] != "ATENDIDO")
            {
                posicion = i;
            }
        }
        return posicion;
    }

    static void MostrarFicha(int i, string[] codigos, string[] fechas, string[] dnis, string[] nombres, string[] telefonos, string[] proyectos, string[] lotes, string[] tipos, double[] montos, string[] estados, string[] responsables)
    {
        Console.WriteLine();
        Console.WriteLine(" FICHA DEL EXPEDIENTE " + codigos[i] + "   (posicion " + i + ")");
        Console.WriteLine(" -----------------------------------------------------------");
        Console.WriteLine(" CODIGO      : " + codigos[i] + "  -> anio " + ExtraerParte(codigos[i], '-', 2) + ", correlativo " + ExtraerParte(codigos[i], '-', 3));
        Console.WriteLine(" FECHA       : " + fechas[i] + "  -> dia " + ExtraerParte(fechas[i], '/', 1) + ", mes " + ExtraerParte(fechas[i], '/', 2) + ", anio " + ExtraerParte(fechas[i], '/', 3));
        Console.WriteLine(" DNI         : " + dnis[i]);
        Console.WriteLine(" NOMBRE      : " + nombres[i]);
        Console.WriteLine(" TELEFONO    : " + telefonos[i]);
        Console.WriteLine(" PROYECTO    : " + proyectos[i]);
        string manzana = QuitarPrefijo(Recortar(ExtraerParte(lotes[i], '-', 1)), "MZ");
        string numeroLote = QuitarPrefijo(Recortar(ExtraerParte(lotes[i], '-', 2)), "LT");
        if (numeroLote.Length > 0)
        {
            Console.WriteLine(" LOTE        : " + lotes[i] + "  -> manzana " + manzana + ", lote " + numeroLote);
        }
        else
        {
            Console.WriteLine(" LOTE        : " + lotes[i]);
        }
        Console.WriteLine(" TIPO DOC    : " + tipos[i]);
        Console.WriteLine(" MONTO       : S/ " + montos[i]);
        Console.WriteLine(" ESTADO      : " + estados[i]);
        Console.WriteLine(" RESPONSABLE : " + responsables[i]);
    }

    static void MostrarTabla(int[] posiciones, int total, string[] codigos, string[] fechas, string[] dnis, string[] nombres, string[] proyectos, double[] montos, string[] estados)
    {
        if (total == 0)
        {
            Console.WriteLine(" (Sin expedientes para mostrar)");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine(" CODIGO        | FECHA      | DNI      | CLIENTE | PROYECTO | MONTO | ESTADO");
            Console.WriteLine(" ---------------------------------------------------------------------------");
            for (int k = 1; k <= total; k++)
            {
                int i = posiciones[k];
                Console.WriteLine(" " + codigos[i] + " | " + fechas[i] + " | " + dnis[i] + " | " + nombres[i] + " | " + proyectos[i] + " | S/ " + montos[i] + " | " + estados[i]);
            }
            Console.WriteLine(" Total de expedientes mostrados: " + total);
        }
    }

    static void TodasLasPosiciones(int[] orden, int n)
    {
        for (int i = 1; i <= n; i++)
        {
            orden[i] = i;
        }
    }

    // -----------------------------------------------------------------------------
    // ORDENAMIENTO
    // -----------------------------------------------------------------------------

    static void ArmarClaves(string[] claves, int n, int criterio, string[] codigos, string[] fechas, string[] nombres, string[] proyectos, double[] montos)
    {
        for (int i = 1; i <= n; i++)
        {
            switch (criterio)
            {
                case 2:
                    claves[i] = FechaOrdenable(fechas[i]);
                    break;
                case 3:
                    claves[i] = nombres[i].ToUpper();
                    break;
                case 4:
                    claves[i] = RellenarCeros((long)(montos[i] * 100), 12);
                    break;
                case 5:
                    claves[i] = proyectos[i];
                    break;
                default:
                    claves[i] = codigos[i];
                    break;
            }
        }
    }

    static void OrdenarBurbuja(int[] orden, string[] claves, int n, bool ascendente, ref int comparaciones, ref int intercambios)
    {
        TodasLasPosiciones(orden, n);
        comparaciones = 0;
        intercambios = 0;
        int i = 1;
        bool huboIntercambio = true;
        while (i <= n - 1 && huboIntercambio)
        {
            huboIntercambio = false;
            for (int j = 1; j <= n - i; j++)
            {
                comparaciones++;
                bool desordenado;
                if (ascendente)
                {
                    desordenado = string.Compare(claves[orden[j]], claves[orden[j + 1]]) > 0;
                }
                else
                {
                    desordenado = string.Compare(claves[orden[j]], claves[orden[j + 1]]) < 0;
                }
                if (desordenado)
                {
                    int temporal = orden[j];
                    orden[j] = orden[j + 1];
                    orden[j + 1] = temporal;
                    intercambios++;
                    huboIntercambio = true;
                }
            }
            i++;
        }
    }

    static void OrdenarInsercion(int[] orden, string[] claves, int n, bool ascendente, ref int comparaciones, ref int desplazamientos)
    {
        TodasLasPosiciones(orden, n);
        comparaciones = 0;
        desplazamientos = 0;
        for (int i = 2; i <= n; i++)
        {
            int actual = orden[i];
            int j = i - 1;
            bool debeCorrerse = true;
            while (j >= 1 && debeCorrerse)
            {
                comparaciones++;
                if (ascendente)
                {
                    debeCorrerse = string.Compare(claves[orden[j]], claves[actual]) > 0;
                }
                else
                {
                    debeCorrerse = string.Compare(claves[orden[j]], claves[actual]) < 0;
                }
                if (debeCorrerse)
                {
                    orden[j + 1] = orden[j];
                    desplazamientos++;
                    j--;
                }
            }
            orden[j + 1] = actual;
        }
    }

    // -----------------------------------------------------------------------------
    // BUSQUEDA
    // -----------------------------------------------------------------------------

    static void BusquedaLineal(string[] codigos, string[] dnis, string[] nombres, string[] lotes, string[] proyectos, int n, string buscado, int[] encontrados, ref int total, ref int comparaciones)
    {
        total = 0;
        comparaciones = 0;
        for (int i = 1; i <= n; i++)
        {
            bool coincide = false;
            int k = 1;
            while (!coincide && k <= 5)
            {
                comparaciones++;
                string campo = "";
                switch (k)
                {
                    case 1: campo = codigos[i]; break;
                    case 2: campo = dnis[i]; break;
                    case 3: campo = nombres[i]; break;
                    case 4: campo = lotes[i]; break;
                    default: campo = proyectos[i]; break;
                }
                coincide = ContieneTexto(campo, buscado);
                k++;
            }
            if (coincide)
            {
                total++;
                encontrados[total] = i;
            }
        }
    }

    static int BusquedaBinaria(string[] codigos, int[] orden, int n, string codigo, ref int comparaciones)
    {
        codigo = codigo.ToUpper();
        int posicion = 0;
        comparaciones = 0;
        int ini = 1;
        int finRango = n;
        while (ini <= finRango && posicion == 0)
        {
            int medio = (ini + finRango) / 2;
            comparaciones++;
            if (codigos[orden[medio]] == codigo)
            {
                posicion = orden[medio];
            }
            else
            {
                if (string.Compare(codigos[orden[medio]], codigo) < 0)
                {
                    ini = medio + 1;
                }
                else
                {
                    finRango = medio - 1;
                }
            }
        }
        return posicion;
    }

    // -----------------------------------------------------------------------------
    // REPORTES
    // -----------------------------------------------------------------------------

    static void ReporteProyectoEstado(string[] proyectos, string[] estados, int n, string[] catProyectos, string[] catEstados)
    {
        int[,] matriz = new int[7, 5];
        for (int k = 1; k <= n; k++)
        {
            int fila = 6;
            for (int i = 1; i <= 6; i++)
            {
                if (catProyectos[i] == proyectos[k])
                {
                    fila = i;
                }
            }
            for (int j = 1; j <= 4; j++)
            {
                if (catEstados[j] == estados[k])
                {
                    matriz[fila, j]++;
                }
            }
        }
        Console.WriteLine();
        Console.WriteLine(" REPORTE DE EXPEDIENTES POR PROYECTO Y ESTADO");
        Console.WriteLine(" PROYECTO | RECIBIDO | EN REVISION | OBSERVADO | ATENDIDO | TOTAL");
        for (int i = 1; i <= 6; i++)
        {
            int sumaFila = 0;
            Console.Write(" " + catProyectos[i]);
            for (int j = 1; j <= 4; j++)
            {
                Console.Write(" | " + matriz[i, j]);
                sumaFila += matriz[i, j];
            }
            Console.WriteLine(" | " + sumaFila);
        }
        Console.WriteLine(" TOTAL GENERAL: " + n);
    }

    static void ReporteMontos(double[] montos, string[] estados, int n)
    {
        double total = 0;
        double mayor = montos[1];
        double menor = montos[1];
        int pendientes = 0;
        for (int k = 1; k <= n; k++)
        {
            total += montos[k];
            if (montos[k] > mayor) mayor = montos[k];
            if (montos[k] < menor) menor = montos[k];
            if (estados[k] != "ATENDIDO") pendientes++;
        }
        Console.WriteLine();
        Console.WriteLine(" Monto total registrado : S/ " + total);
        Console.WriteLine(" Monto promedio         : S/ " + (Math.Round((total / n) * 100.0) / 100.0));
        Console.WriteLine(" Operacion mas alta     : S/ " + mayor);
        Console.WriteLine(" Operacion mas baja     : S/ " + menor);
        Console.WriteLine(" Expedientes pendientes : " + pendientes);
    }

    static void ResumenPorCliente(string[] dnis, string[] nombres, double[] montos, int n)
    {
        string[] dnisCliente = new string[201];
        string[] nombresCliente = new string[201];
        int[] cantidades = new int[201];
        double[] totales = new double[201];
        int clientes = 0;

        for (int k = 1; k <= n; k++)
        {
            int posicion = 0;
            for (int i = 1; i <= clientes; i++)
            {
                if (posicion == 0 && dnisCliente[i] == dnis[k])
                {
                    posicion = i;
                }
            }
            if (posicion == 0)
            {
                clientes++;
                dnisCliente[clientes] = dnis[k];
                nombresCliente[clientes] = nombres[k];
                cantidades[clientes] = 1;
                totales[clientes] = montos[k];
            }
            else
            {
                cantidades[posicion]++;
                totales[posicion] += montos[k];
            }
        }

        for (int i = 2; i <= clientes; i++)
        {
            string dniAux = dnisCliente[i];
            string nombreAux = nombresCliente[i];
            int cantidadAux = cantidades[i];
            double totalAux = totales[i];
            int j = i - 1;
            bool debeCorrerse = true;
            while (j >= 1 && debeCorrerse)
            {
                if (totales[j] < totalAux)
                {
                    dnisCliente[j + 1] = dnisCliente[j];
                    nombresCliente[j + 1] = nombresCliente[j];
                    cantidades[j + 1] = cantidades[j];
                    totales[j + 1] = totales[j];
                    j--;
                }
                else
                {
                    debeCorrerse = false;
                }
            }
            dnisCliente[j + 1] = dniAux;
            nombresCliente[j + 1] = nombreAux;
            cantidades[j + 1] = cantidadAux;
            totales[j + 1] = totalAux;
        }

        Console.WriteLine();
        Console.WriteLine(" RESUMEN POR CLIENTE (de mayor a menor monto pagado)");
        Console.WriteLine(" DNI      | CLIENTE | EXPEDIENTES | TOTAL PAGADO");
        for (int i = 1; i <= clientes; i++)
        {
            Console.WriteLine(" " + dnisCliente[i] + " | " + nombresCliente[i] + " | " + cantidades[i] + " | S/ " + totales[i]);
        }
        Console.WriteLine(" Clientes distintos: " + clientes);
    }

    // =============================================================================
    // ALGORITMO PRINCIPAL
    // =============================================================================

    static void Main(string[] args)
    {
        string[] codigos = new string[201];
        string[] fechas = new string[201];
        string[] dnis = new string[201];
        string[] nombres = new string[201];
        string[] telefonos = new string[201];
        string[] proyectos = new string[201];
        string[] lotes = new string[201];
        string[] tipos = new string[201];
        string[] estados = new string[201];
        string[] responsables = new string[201];
        double[] montos = new double[201];

        string[] catProyectos = new string[7];
        string[] catEstados = new string[5];
        string[] catTipos = new string[6];

        int[] orden = new int[201];
        int[] encontrados = new int[201];
        string[] claves = new string[201];

        catProyectos[1] = "HUANCHACO";
        catProyectos[2] = "MORELIA";
        catProyectos[3] = "VERDE MAR";
        catProyectos[4] = "CALIZA";
        catProyectos[5] = "NURA";
        catProyectos[6] = "OTRO";

        catEstados[1] = "RECIBIDO";
        catEstados[2] = "EN REVISION";
        catEstados[3] = "OBSERVADO";
        catEstados[4] = "ATENDIDO";

        catTipos[1] = "CONTRATO DE SEPARACION";
        catTipos[2] = "MINUTA DE COMPRA VENTA";
        catTipos[3] = "VOUCHER DE PAGO";
        catTipos[4] = "COPIA DE DNI";
        catTipos[5] = "SOLICITUD DE DESISTIMIENTO";

        int n = 0;
        bool salir = false;

        do
        {
            Console.WriteLine();
            Console.WriteLine(" ==========================================================");
            Console.WriteLine("   MESA DE PARTES DIGITAL - URBANISTIKA GRUPO INMOBILIARIO");
            Console.WriteLine(" ==========================================================");
            Console.WriteLine("    1) Registrar un expediente nuevo");
            Console.WriteLine("    2) Listar todos los expedientes");
            Console.WriteLine("    3) Buscar expediente (nombre, DNI, lote o codigo)");
            Console.WriteLine("    4) Buscar por codigo exacto  [busqueda binaria]");
            Console.WriteLine("    5) Ordenar expedientes");
            Console.WriteLine("    6) Cambiar el estado de un expediente");
            Console.WriteLine("    7) Reporte por proyecto y estado  [matriz]");
            Console.WriteLine("    8) Comparar los dos algoritmos de busqueda");
            Console.WriteLine("    9) Resumen por cliente  [arreglos paralelos]");
            Console.WriteLine("   10) Cargar datos de prueba");
            Console.WriteLine("    0) Salir");
            Console.Write(" Elija una opcion: ");
            string texto = Console.ReadLine();

            int opcionMenu = 99;
            if (SoloDigitos(texto))
            {
                opcionMenu = int.Parse(texto);
            }

            switch (opcionMenu)
            {
                case 1:
                    bool cancelado = false;
                    if (n >= 200)
                    {
                        Console.WriteLine(" [!] Se alcanzo el maximo de 200 expedientes.");
                        cancelado = true;
                    }
                    string dni = "", nombre = "", tel = "", lote = "", monto = "", fecha = "", responsable = "";
                    string mensaje = "";
                    bool valido = false;

                    if (!cancelado)
                    {
                        Console.WriteLine(" REGISTRO DE NUEVO EXPEDIENTE   (escriba X para cancelar)");
                        do
                        {
                            Console.Write(" DNI del cliente (8 digitos): ");
                            dni = Console.ReadLine();
                            cancelado = dni.ToUpper() == "X";
                            valido = cancelado;
                            if (!cancelado)
                            {
                                valido = ValidarDni(dni, ref mensaje);
                                if (!valido) Console.WriteLine("   [!] " + mensaje);
                            }
                        } while (!valido);
                    }

                    if (!cancelado)
                    {
                        do
                        {
                            Console.Write(" Nombres y apellidos: ");
                            nombre = Console.ReadLine();
                            cancelado = nombre.ToUpper() == "X";
                            valido = cancelado;
                            if (!cancelado)
                            {
                                valido = ValidarNombre(nombre, ref mensaje);
                                if (!valido) Console.WriteLine("   [!] " + mensaje);
                            }
                        } while (!valido);
                    }

                    if (!cancelado)
                    {
                        do
                        {
                            Console.Write(" Celular (9 digitos): ");
                            tel = Console.ReadLine();
                            cancelado = tel.ToUpper() == "X";
                            valido = cancelado;
                            if (!cancelado)
                            {
                                valido = ValidarTelefono(tel, ref mensaje);
                                if (!valido) Console.WriteLine("   [!] " + mensaje);
                            }
                        } while (!valido);
                    }

                    int p = 0;
                    if (!cancelado)
                    {
                        p = ElegirOpcion("Proyecto:", catProyectos, 6);
                        cancelado = (p == 0);
                    }

                    int t = 0;
                    if (!cancelado)
                    {
                        Console.Write(" Lote (ejemplo: MZ A - LT 12): ");
                        lote = Console.ReadLine();
                        lote = LimpiarSeparador(lote.ToUpper());
                        if (lote.Length == 0) lote = "SIN ASIGNAR";
                        t = ElegirOpcion("Tipo de documento presentado:", catTipos, 5);
                        cancelado = (t == 0);
                    }

                    if (!cancelado)
                    {
                        int posDup = PosicionDuplicado(dnis, lotes, tipos, estados, n, dni, lote, catTipos[t]);
                        if (posDup > 0)
                        {
                            Console.WriteLine(" [!] ATENCION: ya existe un expediente abierto con esos datos:");
                            Console.WriteLine("     " + codigos[posDup] + " | " + nombres[posDup] + " | " + estados[posDup]);
                            Console.Write(" Desea registrarlo igual? (S/N): ");
                            string resp = Console.ReadLine();
                            cancelado = resp.ToUpper() != "S";
                        }
                    }

                    if (!cancelado)
                    {
                        do
                        {
                            Console.Write(" Monto de la operacion en soles: ");
                            monto = Console.ReadLine();
                            cancelado = monto.ToUpper() == "X";
                            valido = cancelado;
                            if (!cancelado)
                            {
                                valido = ValidarMonto(monto, ref mensaje);
                                if (!valido) Console.WriteLine("   [!] " + mensaje);
                            }
                        } while (!valido);
                    }

                    if (!cancelado)
                    {
                        do
                        {
                            Console.Write(" Fecha dd/mm/aaaa [Enter = hoy]: ");
                            fecha = Console.ReadLine();
                            if (fecha.Length == 0) fecha = FechaDeHoy();
                            cancelado = fecha.ToUpper() == "X";
                            valido = cancelado;
                            if (!cancelado)
                            {
                                valido = ValidarFecha(fecha, ref mensaje);
                                if (!valido) Console.WriteLine("   [!] " + mensaje);
                            }
                        } while (!valido);
                    }

                    if (cancelado)
                    {
                        Console.WriteLine(" Registro cancelado.");
                    }
                    else
                    {
                        Console.Write(" Responsable que recibe [Enter = MESA DE PARTES]: ");
                        responsable = Console.ReadLine();
                        responsable = LimpiarSeparador(responsable.ToUpper());
                        if (responsable.Length == 0) responsable = "MESA DE PARTES";
                        string codigo = GenerarCodigo(codigos, n);
                        double monVal = double.Parse(LimpiarMonto(monto), System.Globalization.CultureInfo.InvariantCulture);
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, codigo, fecha, dni, nombre, tel, catProyectos[p], lote, catTipos[t], monVal, "RECIBIDO", responsable);
                        Console.WriteLine(" [OK] Expediente registrado con el codigo: " + codigo);
                    }
                    break;

                case 2:
                    if (HayExpedientes(n))
                    {
                        TodasLasPosiciones(orden, n);
                        MostrarTabla(orden, n, codigos, fechas, dnis, nombres, proyectos, montos, estados);
                    }
                    break;

                case 3:
                    Console.Write(" Escriba nombre, DNI, lote, proyecto o codigo: ");
                    texto = Console.ReadLine();
                    if (texto.Length == 0)
                    {
                        Console.WriteLine(" [!] No escribio nada que buscar.");
                    }
                    else
                    {
                        int totalBusqueda = 0, compLineal = 0;
                        BusquedaLineal(codigos, dnis, nombres, lotes, proyectos, n, texto, encontrados, ref totalBusqueda, ref compLineal);
                        if (totalBusqueda == 0)
                        {
                            Console.WriteLine(" No se encontro ningun expediente con: " + texto);
                        }
                        else
                        {
                            MostrarTabla(encontrados, totalBusqueda, codigos, fechas, dnis, nombres, proyectos, montos, estados);
                            if (totalBusqueda == 1)
                            {
                                MostrarFicha(encontrados[1], codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables);
                            }
                        }
                        Console.WriteLine(" [Metrica] Busqueda lineal: " + compLineal + " comparaciones.");
                    }
                    break;

                case 4:
                    if (HayExpedientes(n))
                    {
                        Console.Write(" Codigo exacto (ejemplo EXP-2026-0001): ");
                        string codBin = Console.ReadLine();
                        ArmarClaves(claves, n, 1, codigos, fechas, nombres, proyectos, montos);
                        int compOrden = 0, movs = 0;
                        OrdenarBurbuja(orden, claves, n, true, ref compOrden, ref movs);
                        int compBin = 0;
                        int posFound = BusquedaBinaria(codigos, orden, n, codBin, ref compBin);
                        if (posFound == 0)
                        {
                            Console.WriteLine(" No existe el expediente " + codBin.ToUpper() + ".");
                        }
                        else
                        {
                            MostrarFicha(posFound, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables);
                        }
                        Console.WriteLine(" [Metrica] Ordenamiento previo: " + compOrden + " comparaciones.");
                        Console.WriteLine(" [Metrica] Busqueda binaria   : " + compBin + " comparaciones.");
                    }
                    break;

                case 5:
                    if (HayExpedientes(n))
                    {
                        Console.WriteLine(" Ordenar por: 1) Codigo  2) Fecha  3) Cliente  4) Monto  5) Proyecto");
                        Console.Write(" Numero: ");
                        texto = Console.ReadLine();
                        int criterio = 0;
                        if (SoloDigitos(texto)) criterio = int.Parse(texto);
                        if (criterio < 1 || criterio > 5)
                        {
                            Console.WriteLine(" [!] Opcion no valida.");
                        }
                        else
                        {
                            Console.Write(" Ascendente (A) o descendente (D)? [A]: ");
                            texto = Console.ReadLine();
                            bool asc = texto.ToUpper() != "D";
                            Console.Write(" Algoritmo: 1) Burbuja  2) Insercion [1]: ");
                            texto = Console.ReadLine();
                            ArmarClaves(claves, n, criterio, codigos, fechas, nombres, proyectos, montos);
                            int comp = 0, mov = 0;
                            if (texto == "2")
                            {
                                OrdenarInsercion(orden, claves, n, asc, ref comp, ref mov);
                                MostrarTabla(orden, n, codigos, fechas, dnis, nombres, proyectos, montos, estados);
                                Console.WriteLine(" [Metrica] INSERCION: " + comp + " comparaciones y " + mov + " desplazamientos.");
                            }
                            else
                            {
                                OrdenarBurbuja(orden, claves, n, asc, ref comp, ref mov);
                                MostrarTabla(orden, n, codigos, fechas, dnis, nombres, proyectos, montos, estados);
                                Console.WriteLine(" [Metrica] BURBUJA: " + comp + " comparaciones y " + mov + " intercambios.");
                            }
                        }
                    }
                    break;

                case 6:
                    if (HayExpedientes(n))
                    {
                        Console.Write(" Codigo del expediente: ");
                        string codEst = Console.ReadLine();
                        int posEst = PosicionPorCodigo(codigos, n, codEst);
                        if (posEst == 0)
                        {
                            Console.WriteLine(" [!] No existe el expediente " + codEst.ToUpper() + ".");
                        }
                        else
                        {
                            Console.WriteLine(" Estado actual: " + estados[posEst]);
                            int idxEst = ElegirOpcion("Nuevo estado:", catEstados, 4);
                            if (idxEst > 0)
                            {
                                if (catEstados[idxEst] == estados[posEst])
                                {
                                    Console.WriteLine(" [!] El expediente ya esta en estado " + catEstados[idxEst] + ".");
                                }
                                else
                                {
                                    estados[posEst] = catEstados[idxEst];
                                    Console.WriteLine(" [OK] El expediente " + codigos[posEst] + " ahora esta en estado: " + catEstados[idxEst]);
                                }
                            }
                        }
                    }
                    break;

                case 7:
                    if (HayExpedientes(n))
                    {
                        ReporteProyectoEstado(proyectos, estados, n, catProyectos, catEstados);
                        ReporteMontos(montos, estados, n);
                    }
                    break;

                case 8:
                    if (HayExpedientes(n))
                    {
                        Console.Write(" Codigo a buscar con ambos algoritmos: ");
                        string codComp = Console.ReadLine();
                        int totL = 0, compL = 0;
                        BusquedaLineal(codigos, dnis, nombres, lotes, proyectos, n, codComp, encontrados, ref totL, ref compL);
                        ArmarClaves(claves, n, 1, codigos, fechas, nombres, proyectos, montos);
                        int compOrd = 0, movOrd = 0;
                        OrdenarBurbuja(orden, claves, n, true, ref compOrd, ref movOrd);
                        int compB = 0;
                        BusquedaBinaria(codigos, orden, n, codComp, ref compB);
                        Console.WriteLine();
                        Console.WriteLine(" COMPARACION DE ALTERNATIVAS ALGORITMICAS");
                        Console.WriteLine(" Expedientes en los arreglos : " + n);
                        Console.WriteLine(" A - Busqueda lineal : " + compL + " comparaciones (no requiere orden, admite busqueda parcial)");
                        Console.WriteLine(" B - Busqueda binaria: " + compB + " comparaciones (+ " + compOrd + " para ordenar antes, solo codigo exacto)");
                        Console.WriteLine(" CONCLUSION: binaria para el codigo exacto, lineal para nombre, DNI y lote.");
                    }
                    break;

                case 9:
                    if (HayExpedientes(n))
                    {
                        ResumenPorCliente(dnis, nombres, montos, n);
                    }
                    break;

                case 10:
                    if (n + 8 > 200)
                    {
                        Console.WriteLine(" [!] No hay espacio para los datos de prueba.");
                    }
                    else
                    {
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "18/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "CONTRATO DE SEPARACION", 13000, "ATENDIDO", "MESA DE PARTES");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "19/08/2026", "72451890", "Carlos Vega Torres", "912345678", "MORELIA", "MZ C - LT 04", "VOUCHER DE PAGO", 2500, "RECIBIDO", "MESA DE PARTES");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "20/08/2026", "09876543", "Ana Lucia Chavez", "998877665", "VERDE MAR", "MZ B - LT 21", "MINUTA DE COMPRA VENTA", 24000, "EN REVISION", "AREA LEGAL");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "21/08/2026", "45781236", "Maria Rojas Delgado", "987654321", "HUANCHACO", "MZ A - LT 12", "VOUCHER DE PAGO", 1200, "ATENDIDO", "CAJA");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "22/08/2026", "60123456", "Jorge Miranda Silva", "955443322", "CALIZA", "MZ D - LT 08", "COPIA DE DNI", 500, "OBSERVADO", "MESA DE PARTES");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "25/08/2026", "31245678", "Rosa Guzman Polo", "976543210", "NURA", "MZ E - LT 15", "CONTRATO DE SEPARACION", 18500, "RECIBIDO", "MESA DE PARTES");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "26/08/2026", "80654321", "Pedro Alvarado Ruiz", "934567890", "MORELIA", "MZ C - LT 09", "MINUTA DE COMPRA VENTA", 31000, "EN REVISION", "AREA LEGAL");
                        AgregarExpediente(ref n, codigos, fechas, dnis, nombres, telefonos, proyectos, lotes, tipos, montos, estados, responsables, GenerarCodigo(codigos, n), "27/08/2026", "27418596", "Lucia Flores Chavez", "921098765", "HUANCHACO", "MZ A - LT 30", "SOLICITUD DE DESISTIMIENTO", 13000, "OBSERVADO", "GERENCIA");
                        Console.WriteLine(" [OK] Se agregaron 8 expedientes de prueba.");
                    }
                    break;

                case 0:
                    Console.WriteLine(" Sistema cerrado. Hasta luego.");
                    salir = true;
                    break;

                default:
                    Console.WriteLine(" [!] Opcion no valida. Elija un numero del menu.");
                    break;
            }
        } while (!salir);
    }
}
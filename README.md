# Mesa de Partes Digital – Urbanistika (temas 1 a 7)

Tres versiones del MISMO algoritmo: **PSeInt** (`pseint/`), **Python** (`python/`) y **C#** (`csharp/`).

Fundamentos de Programación – CIIN1205P – UPN Trujillo 2026 – Grupo 1

## Qué cambió respecto al parcial (tema 4)
1. **Código repartido en módulos** (Python) y en **archivos parciales** (C#), cada uno con una sola responsabilidad.
2. **Archivo de texto** `expedientes.txt`: se carga al iniciar y se guarda solo después de registrar, cambiar estado, cargar datos de prueba y al salir.
3. **Respaldo binario** `expedientes.dat`: registros de longitud fija (182 bytes) con **acceso directo** al registro *n*.
4. **Comparación texto vs binario** (opción 14): tamaño, tiempo de guardar, de cargar y de leer el último registro con 100, 1 000 y 10 000 expedientes.
5. **Pruebas automáticas** en los dos lenguajes.
6. Los dos programas usan **exactamente el mismo formato**: un archivo creado en Python se abre en C# y al revés (comprobado).

## Formato del archivo de texto
Una línea por expediente, 11 campos separados por `|`, en el orden de los arreglos paralelos:
```
codigo|fecha|dni|nombre|telefono|proyecto|lote|tipo|monto|estado|responsable
EXP-2026-0001|18/08/2026|45781236|Maria Rojas Delgado|987654321|HUANCHACO|MZ A - LT 12|CONTRATO DE SEPARACION|13000.00|ATENDIDO|MESA DE PARTES
```
Las líneas dañadas (campos de menos, fecha, DNI o monto inválidos) se descartan y se avisa el número de línea.
> Si el PSeInt usa otro separador u otro orden de campos, solo hay que cambiar `SEPARADOR` y el orden en `archivos.py` / `Archivos.cs`.

## Registro binario (182 bytes)
| Campo | Bytes | Campo | Bytes |
|---|---|---|---|
| código | 13 | lote | 20 |
| fecha | 10 | tipo de documento | 30 |
| DNI | 8 | monto (double) | 8 |
| nombre | 40 | estado | 12 |
| teléfono | 9 | responsable | 20 |
| proyecto | 12 | **Total** | **182** |

El registro *n* empieza en el byte `n × 182`. Los textos se rellenan con ceros y se recortan sin partir una letra con tilde.

## PSeInt
```
Abrir pseint/MesaDePartes.psc con PSeInt
Configurar > Opciones del lenguaje > perfil "UPN" (también corre con "Estricto")
```
- Mismo menú, mismos catálogos y mismo formato de línea `|` que Python y C#.
- PSeInt no escribe archivos en disco: el `expedientes.txt` se **simula** con un arreglo de líneas (explicado al inicio del .psc).
- `pseint/diagramas/`: 9 diagramas de flujo de los subprocesos principales.

## Python
```
cd python
py main.py                                 # programa
py -m unittest discover -s pruebas -v      # 27 pruebas
```
| Módulo | Contenido |
|---|---|
| `cadenas.py` | normalizar, tildes, subcadena, descomponer código/fecha/lote |
| `validaciones.py` | DNI, celular, nombre, monto, fecha (bisiesto) |
| `datos.py` | los 11 arreglos paralelos, catálogos, alta y búsqueda de posiciones |
| `algoritmos.py` | búsqueda lineal/binaria, burbuja/inserción con métricas |
| `reportes.py` | matriz proyecto×estado, montos, resumen por cliente |
| `archivos.py` | texto, binario, acceso directo y comparación |
| `interfaz.py` | teclado, tablas y opciones del menú |
| `main.py` | menú principal, carga y guardado |

## C#
```
cd csharp
dotnet run                     # programa
dotnet run -- --pruebas        # 26 pruebas
```
`Program.cs` (menú y opciones de archivos), `Datos.cs`, `Cadenas.cs`, `Validaciones.cs`, `Algoritmos.cs`, `Reportes.cs`, `Interfaz.cs`, `Archivos.cs`, `Pruebas.cs`. Todos son partes de la misma clase `partial class Program`.

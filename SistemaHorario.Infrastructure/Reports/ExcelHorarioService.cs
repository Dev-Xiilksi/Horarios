using ClosedXML.Excel;
using SistemaHorario.Application.DTOs.Auxiliares;
using SistemaHorario.Application.Interfaces;

namespace SistemaHorario.Infrastructure.Reports
{
    public class ExcelHorarioService : IExcelHorarioService
    {
        //encabezados de las columnas del resumen (propiedad de HorarioAuxDTO -> titulo)
        private static readonly Dictionary<string, string> EncabezadosResumen = new()
        {
            {"Codigo", "Código"},
            {"Educador", "Educador"},
            {"Ciclo","Ciclo"},
            {"Area","Area"},
            {"Ia","IMPARTE ASIGNATURA"},
            {"Aaa","ACTIVIDADES DE AUTOCUIDADO Y APRECIO PERSONAL"},
            {"Alm","ALMUERZO / REFACCIÓN"},
            {"Cuida","CUIDA ENTRADA, RECREO O SALIDA"},
            {"Fmagis","FRANJA MAGIS"},
            {"Gaa","GESTIÓN DE ACTIVIDADES DE ACOMPAÑAMIENTO"},
            {"Gao","GESTIÓN DE ACTIVIDADES DE ORIENTACIÓN"},
            {"Ora","ORACIÓN"},
            {"Rarea","REUNIÓN COORDINADOR O ÁREA"},
            {"Rcomuni","REUNIÓN COMUNIDAD APRENDIZAJE"},
            {"Rorient","REUNIÓN CON ORIENTADOR/A"},
            {"Rpersonal","REUNIÓN PERSONAL COORDINADOR"},
            {"Rtp","REVISA TRABAJOS Y PLANIFICA - TRABAJO PERSONAL" },
            {"Anp","ACTIVIDAD NO PROGRAMADA" },
            {"Total","TOTAL" }
        };

        public byte[] GenerarExcelResumen(IEnumerable<HorarioAuxDTO> horarios)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");

            ws.Cell(2, 1).InsertData(horarios);

            int columnIndex = 1;
            foreach (var property in typeof(HorarioAuxDTO).GetProperties())
            {
                if (EncabezadosResumen.TryGetValue(property.Name, out var encabezado))
                    ws.Cell(1, columnIndex).Value = encabezado;
                columnIndex++;
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerarExcel(HorarioExcelDto data)
        {
            using var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Horario");

            int currentRow = 1;

            ws.Cell(currentRow, 1).Value = "Código:";
            ws.Cell(currentRow, 2).Value = data.Empleado.NumEmpleado;
            currentRow++;

            ws.Cell(currentRow, 1).Value = "Educador:";
            ws.Cell(currentRow, 2).Value = data.Empleado.Educador;
            currentRow++;

            ws.Cell(currentRow, 1).Value = "Grado:";
            ws.Cell(currentRow, 2).Value =
                string.Join(Environment.NewLine,
                    data.Grados.Select(x =>
                        $"{x.Colegio} {x.Nivel} {x.Grado} {x.Seccion}"));

            currentRow += 4;

            string[] headers =
            {
            "Franja",
            "Hora inicio",
            "Hora fin",
            "Tiempo",
            "Lunes",
            "Martes",
            "Miércoles",
            "Jueves",
            "Viernes",
            "Sábados"
        };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(currentRow, i + 1).Value = headers[i];
            }

            currentRow++;

            foreach (var f in data.Franjas)
            {
                ws.Cell(currentRow, 1).Value = f.Franja;
                ws.Cell(currentRow, 2).Value = f.HoraInicio?.ToString();
                ws.Cell(currentRow, 3).Value = f.HoraFin?.ToString();
                ws.Cell(currentRow, 4).Value = f.Tiempo?.ToString();
                ws.Cell(currentRow, 5).Value = f.Lunes;
                ws.Cell(currentRow, 6).Value = f.Martes;
                ws.Cell(currentRow, 7).Value = f.Miercoles;
                ws.Cell(currentRow, 8).Value = f.Jueves;
                ws.Cell(currentRow, 9).Value = f.Viernes;
                ws.Cell(currentRow, 10).Value = f.Sabados;

                currentRow++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();

            wb.SaveAs(ms);

            return ms.ToArray();
        }

        public byte[] GenerarExcelConsolidados(IEnumerable<HorarioConsolidadoDto> horario)
        {
            using var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Consolidado");

            ws.Cell(2, 1).InsertData(horario);

            var headers = new Dictionary<string, string>()
            {
                {"Codigo", "Código"},
                {"Educador", "Educador"},
                {"Ciclo","Ciclo"},
                {"Area","Area"},
                //{"Colegio","Colegio"},
                //{"Nivel","Nivel"},
                //{"Grado","Grado"},
                //{"Seccion","Sección"},

                //{"Ctfa","CTFA"},
                //{"Estudiantes","# Estud."},
                //{"Observacion","Notas"},
                //{"Adm","ADM"},
                //{"Ecc","ECC"},
                //{"Euc","EUC"},
                //{"Camm","CAMM"},
                //{"Ef","EF"},
                //{"Sse","SSE"},
                //{"Rpmf","RPMF"},
                //{"Techo","TECHO"},

                {"IA","Imparte Asignatura"},
                {"AAA","Actividades de Autocuidado y Aprecio Personal"},
                {"ALM","Almuerzo / Refacción"},
                {"CUIDA","Cuid Entrada, Recreo o Salida"},
                {"FMAGIS","Franja Magis"},
                {"GAA","Gestión de Actividades de Acompañamiento"},
                {"GAO","Gestión de Actividades de Orientación"},
                {"ORA","Oración"},
                {"RAREA","Reunión Coordinador o Área"},
                {"RCOMUNI","Reunión Comunidad Aprendizaje"},
                {"RORIENT","Reunión con Orientador/a"},
                {"RPERSONAL","Reunión Personal Coordinador"},
                {"RTP","Revisa Trabajos y Planifica Trabajo Personal" },
                {"ANP","Activida no Programada" },
                {"TOTAL","Total" }
            };


            if (headers != null && headers.Any())
            {
                int columnIndex = 1;
                foreach (var property in typeof(HorarioConsolidadoDto).GetProperties())
                {
                    if (headers.ContainsKey(property.Name))
                    {
                        ws.Cell(1, columnIndex).Value = headers[property.Name];
                    }
                    columnIndex++;
                }
            }
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }
    }
}

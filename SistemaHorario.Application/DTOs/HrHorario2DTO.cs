using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SistemaHorario.Application.DTOs
{
    public class HrHorario2DTO
    {
        public int IdHorario2 { get; set; }
        public int? IdHorario1 { get; set; }
        public short? Dia { get; set; }
        public int? IdHrg2 { get; set; }
        public int? IdMps { get; set; }
        public short? IdHra { get; set; }

        [JsonIgnore]
        public TimeSpan HoraInicio { get; set; }

        [JsonIgnore]
        public TimeSpan HoraFinal { get; set; }


        //propiedades ricas en comportamiento

        public string Horai //es para mostrar el horario
        {
            //get => HoraInicio.ToString(@"hh\:mm");
            //set => HoraInicio = TimeSpan.Parse(value);

            get => HoraInicio.ToString(@"hh\:mm\:ss");
            set
            {
                if (!string.IsNullOrEmpty(value))
                    HoraInicio = TimeSpan.Parse(value);
                else
                    HoraInicio = TimeSpan.Zero;
            }
        }

        public string Horaf //es para mostrar el horario
        {
            //get => HoraFinal.ToString(@"hh\:mm");
            //set => HoraFinal = TimeSpan.Parse(value);
            get => HoraFinal.ToString(@"hh\:mm\:ss");
            set
            {
                if (!string.IsNullOrEmpty(value))
                    HoraFinal = TimeSpan.Parse(value);
                else
                    HoraFinal = TimeSpan.Zero;
            }
        }
        public string? Tiempo { get; set; }
        public int NumeroFila { get; set; }

        //aux
        public int? LunesActividadId { get; set; }

        public int? MartesActividadId { get; set; }

        public int? MiercolesActividadId { get; set; }

        public int? JuevesActividadId { get; set; }

        public int? ViernesActividadId { get; set; }

        public int? SabadoActividadId { get; set; }

        [JsonIgnore]
        public string EsLectivo { get; set; } = string.Empty;

        //Activities Name
        [JsonIgnore]
        public string LunesActividadEsLectivo { get; set; } = string.Empty;
        [JsonIgnore]
        public string MartesActividadEsLectivo { get; set; } = string.Empty;
        [JsonIgnore]
        public string MiercolesActividadEsLectivo { get; set; } = string.Empty;
        [JsonIgnore]
        public string JuevesActividadEsLectivo { get; set; } = string.Empty;
        [JsonIgnore]
        public string ViernesActividadEsLectivo { get; set; } = string.Empty;
        [JsonIgnore]
        public string SabadoActividadEsLectivo { get; set; } = string.Empty;




        [JsonIgnore]
        public string? HoraIni { get; set; }

        [JsonIgnore]
        public string? HoraFin { get; set; }

        [JsonIgnore]
        public string HoraIniLunes { get; set; } = "";
        [JsonIgnore]
        public string HoraFinLunes { get; set; } = "";
        [JsonIgnore]
        public string HoraIniMartes { get; set; } = "";
        [JsonIgnore]
        public string HoraFinMartes { get; set; } = "";
        [JsonIgnore]
        public string HoraIniMiercoles { get; set; } = "";
        [JsonIgnore]
        public string HoraFinMiercoles { get; set; } = "";
        [JsonIgnore]
        public string HoraIniJueves { get; set; } = "";
        [JsonIgnore]
        public string HoraFinJueves { get; set; } = "";
        [JsonIgnore]
        public string HoraIniViernes { get; set; } = "";
        [JsonIgnore]
        public string HoraFinViernes { get; set; } = "";
        [JsonIgnore]
        public string HoraIniSabados { get; set; } = "";
        [JsonIgnore]
        public string HoraFinSabados { get; set; } = "";

        //temps
        [JsonIgnore]
        public TimeSpan HoraIniLunesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraFinLunesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraIniMartesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraFinMartesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraIniMiercolesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraFinMiercolesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraIniJuevesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraFinJuevesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraIniViernesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraFinViernesT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraIniSabadosT { get; set; }
        [JsonIgnore]
        public TimeSpan HoraFinSabadosT { get; set; } 

    }
}

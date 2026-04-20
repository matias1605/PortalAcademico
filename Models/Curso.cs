using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Los créditos deben ser mayor a 0.")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; }

        [Range(1, 500, ErrorMessage = "El cupo debe ser mayor a 0.")]
        [Display(Name = "Cupo Máximo")]
        public int CupoMaximo { get; set; }

        [Display(Name = "Horario Inicio")]
        public TimeOnly HorarioInicio { get; set; }

        [Display(Name = "Horario Fin")]
        public TimeOnly HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
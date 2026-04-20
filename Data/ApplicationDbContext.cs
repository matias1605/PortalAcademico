using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Curso>(e =>
            {
                e.HasIndex(c => c.Codigo).IsUnique();
                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Curso_Creditos", "\"Creditos\" > 0"));
                e.ToTable(t => t.HasCheckConstraint(
                    "CK_Curso_Horario", "\"HorarioInicio\" < \"HorarioFin\""));
            });

            builder.Entity<Matricula>(e =>
            {
                e.HasIndex(m => new { m.CursoId, m.UsuarioId }).IsUnique();
                e.HasOne(m => m.Curso)
                 .WithMany(c => c.Matriculas)
                 .HasForeignKey(m => m.CursoId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Curso>().HasData(
                new Curso { Id=1, Codigo="MAT101", Nombre="Matemáticas I",
                    Creditos=4, CupoMaximo=30,
                    HorarioInicio=new TimeOnly(8,0),
                    HorarioFin=new TimeOnly(10,0), Activo=true },
                new Curso { Id=2, Codigo="PRG201", Nombre="Programación I",
                    Creditos=3, CupoMaximo=25,
                    HorarioInicio=new TimeOnly(10,0),
                    HorarioFin=new TimeOnly(12,0), Activo=true },
                new Curso { Id=3, Codigo="FIS101", Nombre="Física I",
                    Creditos=4, CupoMaximo=20,
                    HorarioInicio=new TimeOnly(14,0),
                    HorarioFin=new TimeOnly(16,0), Activo=true }
            );
        }
    }
}



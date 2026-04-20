using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "cursos_activos";

        public CoordinadorController(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        // GET /Coordinador
        public async Task<IActionResult> Index()
        {
            var cursos = await _db.Cursos
                .Include(c => c.Matriculas)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            return View(cursos);
        }

        // GET /Coordinador/CrearCurso
        public IActionResult CrearCurso() => View(new Curso());

        // POST /Coordinador/CrearCurso
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCurso(Curso curso)
        {
            // Validación server-side: HorarioFin > HorarioInicio
            if (curso.HorarioInicio >= curso.HorarioFin)
                ModelState.AddModelError("HorarioFin",
                    "El horario fin debe ser posterior al inicio.");

            // Validación: código único
            if (await _db.Cursos.AnyAsync(c => c.Codigo == curso.Codigo))
                ModelState.AddModelError("Codigo",
                    "Ya existe un curso con ese código.");

            if (!ModelState.IsValid) return View(curso);

            _db.Cursos.Add(curso);
            await _db.SaveChangesAsync();

            // Invalidar cache Redis
            await _cache.RemoveAsync(CacheKey);

            TempData["Exito"] = $"Curso {curso.Nombre} creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Coordinador/EditarCurso/5
        public async Task<IActionResult> EditarCurso(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST /Coordinador/EditarCurso/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCurso(int id, Curso curso)
        {
            if (id != curso.Id) return BadRequest();

            if (curso.HorarioInicio >= curso.HorarioFin)
                ModelState.AddModelError("HorarioFin",
                    "El horario fin debe ser posterior al inicio.");

            if (await _db.Cursos.AnyAsync(c => c.Codigo == curso.Codigo && c.Id != id))
                ModelState.AddModelError("Codigo",
                    "Ya existe un curso con ese código.");

            if (!ModelState.IsValid) return View(curso);

            _db.Update(curso);
            await _db.SaveChangesAsync();

            // Invalidar cache Redis
            await _cache.RemoveAsync(CacheKey);

            TempData["Exito"] = "Curso actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Coordinador/DesactivarCurso/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarCurso(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            curso.Activo = !curso.Activo;
            await _db.SaveChangesAsync();

            // Invalidar cache Redis
            await _cache.RemoveAsync(CacheKey);

            TempData["Exito"] = curso.Activo
                ? $"Curso {curso.Nombre} activado."
                : $"Curso {curso.Nombre} desactivado.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Coordinador/Matriculas/5
        public async Task<IActionResult> Matriculas(int id)
        {
            var curso = await _db.Cursos
                .Include(c => c.Matriculas)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (curso == null) return NotFound();
            ViewBag.Curso = curso;
            return View(curso.Matriculas.ToList());
        }

        // POST /Coordinador/CambiarEstado
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int matriculaId,
            EstadoMatricula estado, int cursoId)
        {
            var matricula = await _db.Matriculas.FindAsync(matriculaId);
            if (matricula == null) return NotFound();
            matricula.Estado = estado;
            await _db.SaveChangesAsync();
            TempData["Exito"] = $"Matrícula actualizada a {estado}.";
            return RedirectToAction(nameof(Matriculas), new { id = cursoId });
        }
    }
}
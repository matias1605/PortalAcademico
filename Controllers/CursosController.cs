using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;
using System.Text.Json;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "cursos_activos";

        public CursosController(ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            IDistributedCache cache)
        {
            _db = db;
            _userManager = userManager;
            _cache = cache;
        }

        // GET /Cursos
        public async Task<IActionResult> Index(string? nombre, int? creditosMin,
    int? creditosMax, string? horario)
{
    // Validaciones server-side
    if (creditosMin.HasValue && creditosMin.Value < 0)
        ModelState.AddModelError("creditosMin", "Los créditos no pueden ser negativos.");

    if (creditosMax.HasValue && creditosMax.Value < 0)
        ModelState.AddModelError("creditosMax", "Los créditos no pueden ser negativos.");

    if (!ModelState.IsValid)
    {
        ViewBag.ErrorFiltro = "Los valores de créditos no pueden ser negativos.";
        ViewBag.Nombre      = nombre;
        ViewBag.CreditosMin = creditosMin;
        ViewBag.CreditosMax = creditosMax;
        ViewBag.Horario     = horario;
        return View(new List<Curso>());
    }

    List<Curso> cursos;
    bool sinFiltros = string.IsNullOrEmpty(nombre)
                   && creditosMin == null
                   && creditosMax == null
                   && string.IsNullOrEmpty(horario);

    if (sinFiltros)
    {
        var cached = await _cache.GetStringAsync(CacheKey);
        if (cached != null)
            cursos = JsonSerializer.Deserialize<List<Curso>>(cached) ?? new();
        else
        {
            cursos = await _db.Cursos.Where(c => c.Activo).ToListAsync();
            await _cache.SetStringAsync(CacheKey,
                JsonSerializer.Serialize(cursos),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                });
        }
    }
    else
    {
        var q = _db.Cursos.Where(c => c.Activo).AsQueryable();
        if (!string.IsNullOrEmpty(nombre))
            q = q.Where(c => c.Nombre.Contains(nombre));
        if (creditosMin.HasValue)
            q = q.Where(c => c.Creditos >= creditosMin.Value);
        if (creditosMax.HasValue)
            q = q.Where(c => c.Creditos <= creditosMax.Value);
        if (!string.IsNullOrEmpty(horario) && TimeOnly.TryParse(horario, out var h))
            q = q.Where(c => c.HorarioInicio >= h);
        cursos = await q.ToListAsync();
    }

    ViewBag.Nombre      = nombre;
    ViewBag.CreditosMin = creditosMin;
    ViewBag.CreditosMax = creditosMax;
    ViewBag.Horario     = horario;
    return View(cursos);
}

        // GET /Cursos/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _db.Cursos
                .Include(c => c.Matriculas)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (curso == null) return NotFound();

            // Guardar último curso visitado en Session
            HttpContext.Session.SetString("UltimoCursoId",     curso.Id.ToString());
            HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

            if (User.Identity?.IsAuthenticated == true)
            {
                var uid = _userManager.GetUserId(User);
                ViewBag.YaMatriculado = await _db.Matriculas
                    .AnyAsync(m => m.CursoId == id && m.UsuarioId == uid
                               && m.Estado != EstadoMatricula.Cancelada);
            }
            ViewBag.MatriculasActivas = curso.Matriculas
                .Count(m => m.Estado != EstadoMatricula.Cancelada);
            return View(curso);
        }
    }
}
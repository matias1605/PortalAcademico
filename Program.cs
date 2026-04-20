using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQLite
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=portal_academico.db";
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connStr));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services
    .AddDefaultIdentity<IdentityUser>(o => {
        o.SignIn.RequireConfirmedAccount = false;
        o.Password.RequiredLength = 6;
        o.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ── PREGUNTA 4: Sesiones y Redis ──────────────────────────
// Cache Redis: cachea listado de cursos activos por 60s
// Sesión Redis: guarda último curso visitado para el navbar
var redisConn = builder.Configuration["Redis:ConnectionString"]
    ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(o => {
    o.Configuration = redisConn;
    o.InstanceName  = "PortalAcademico:";
});
builder.Services.AddSession(o => {
    o.IdleTimeout        = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly    = true;
    o.Cookie.IsEssential = true;
});
// ──────────────────────────────────────────────────────────

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseMigrationsEndPoint();
else {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cursos}/{action=Index}/{id?}");
app.MapRazorPages();

// Migraciones y seed automático
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await SeedData.InicializarAsync(scope.ServiceProvider);
}

app.Run();
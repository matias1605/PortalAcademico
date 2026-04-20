# Portal Académico – Gestión de Cursos y Matrículas

ASP.NET Core MVC (.NET 8) + Identity + EF Core (SQLite) + Redis

---

## Pasos para correr localmente

### 1. Requisitos previos

- .NET 8 SDK
- VS Code + extensión C#
- Redis local o cuenta en Redis Labs → https://app.redislabs.com

### 2. Clonar el repositorio

```bash
git clone https://github.com/matias1605/PortalAcademico.git
cd PortalAcademico
```

### 3. Restaurar dependencias

```bash
dotnet restore
```

### 4. Aplicar migraciones

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Ejecutar

```bash
dotnet run
```

Abrir: http://localhost:5000

### 6. Usuario coordinador inicial

- Email: coordinador@usmp.pe
- Password: Coord@1234

---

## Variables de entorno en Render

| Variable                               | Valor                             |
| -------------------------------------- | --------------------------------- |
| `ASPNETCORE_ENVIRONMENT`               | `Production`                      |
| `ASPNETCORE_URLS`                      | `http://0.0.0.0:${PORT}`          |
| `ConnectionStrings__DefaultConnection` | `Data Source=portal_academico.db` |
| `Redis__ConnectionString`              | `host:port,password=TU_PASSWORD`  |

---

## URL en Render

https://portal-academico.onrender.com

---

## Ramas

| Rama                        | Pregunta                       |
| --------------------------- | ------------------------------ |
| `feature/bootstrap-dominio` | P1: Modelos + EF Core + seed   |
| `feature/catalogo-cursos`   | P2: Catálogo + filtros         |
| `feature/matriculas`        | P3: Inscripción + validaciones |
| `feature/sesion-redis`      | P4: Redis session + cache      |
| `feature/panel-coordinador` | P5: CRUD + rol Coordinador     |
| `deploy/render`             | P6: Deploy Render.com          |

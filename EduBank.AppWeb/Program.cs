
using EduBank.BLL.Services;
using EduBank.DAL.DataContext;
using EduBank.DAL.Repository;
using EduBank.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EdubanckssrContext>(optiones =>
{
    optiones.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(Options =>
 {
     Options.LoginPath = "/Acceso/Login";
     Options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});

builder.Services.AddScoped<IGenericRepository<Categoria>, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.AddScoped<IMovimientoRepository, MovimientoRepository>();
builder.Services.AddScoped<IMovimientoService, MovimientoService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<IGenericRepository<Categoria>, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// Agregar estas líneas en el registro de servicios
// En Program.cs agregar:
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<ICuentaService, CuentaService>();


// Servicios
builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();

// Repositorios
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
builder.Services.AddScoped<IGenericRepository<Transferencia>, GenericRepository<Transferencia>>();

// Registrar servicios de Pagos Habituales
builder.Services.AddScoped<IPagoHabitualService, PagoHabitualService>();
builder.Services.AddScoped<IPagoHabitualRepository, PagoHabitualRepository>();

//cHATBOT


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//    //pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Presentacion}/{action=Index}/{id?}");

app.Run();

using EduBank.AppWeb.Helpers;
using EduBank.DAL.DataContext;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

using System.Security.Claims;

namespace EduBank.AppWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly EdubanckssrContext _db;

        public AccesoController(EdubanckssrContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Contrasena != model.ConfirmarContrasena)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View(model);
            }

            // Validar si el correo ya existe
            bool existe = await _db.Usuarios.AnyAsync(u => u.CorreoElectronico == model.CorreoElectronico);
            if (existe)
            {
                ViewData["Mensaje"] = "El correo electrónico ya está registrado. Intenta con otro.";
                return View(model);
            }

            // Hashear la contraseña antes de guardar
            string contrasenaHash = PasswordHasher.HashPassword(model.Contrasena);

            Usuario usuario = new Usuario()
            {
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                CorreoElectronico = model.CorreoElectronico,
                Contrasena = contrasenaHash, // ← Guardar el hash, no la contraseña en texto plano
                FechaRegistro = DateTime.Now
            };

            await _db.Usuarios.AddAsync(usuario);
            await _db.SaveChangesAsync();

            if (usuario.UsuarioId != 0)
            {
                TempData["RegistroExitoso"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
                return RedirectToAction("Login", "Acceso");
            }

            ViewData["Mensaje"] = "No se pudo crear el usuario. Intenta nuevamente.";
            return View(model);
        }




        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Buscar usuario por correo
            Usuario? usuario_encontrado = await _db.Usuarios
                .Where(u => u.CorreoElectronico == modelo.CorreoElectronico)
                .FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "Credenciales inválidas";
                return View();
            }

            // Verificar contraseña usando el hash
            bool contrasenaValida = PasswordHasher.VerifyPassword(modelo.Contrasena, usuario_encontrado.Contrasena);

            if (!contrasenaValida)
            {
                ViewData["Mensaje"] = "Credenciales inválidas";
                return View();
            }

            // Crear claims para la autenticación
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim("UsuarioId", usuario_encontrado.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, usuario_encontrado.CorreoElectronico)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                //IsPersistent = modelo.Recordarme // ← Si agregas esta opción en el ViewModel
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }


    }
}
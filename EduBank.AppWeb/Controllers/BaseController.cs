using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        protected int ObtenerUsuarioId()
        {
            // Intentar obtener de diferentes claims posibles
            var userIdClaim = User.FindFirst("UsuarioId") ??
                            User.FindFirst(ClaimTypes.NameIdentifier) ??
                            User.FindFirst("sub");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Log para debugging
            Console.WriteLine($"No se pudo obtener UsuarioId. Claims disponibles:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            throw new UnauthorizedAccessException("Usuario no autenticado o ID de usuario no disponible");
        }

        protected string ObtenerNombreUsuario()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ??
                   User.FindFirst("name")?.Value ??
                   "Usuario";
        }

        protected string ObtenerEmailUsuario()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value ??
                   User.FindFirst("email")?.Value ??
                   string.Empty;
        }

        protected bool EstaAutenticado()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }
    }
}
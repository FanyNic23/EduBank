using EduBank.AppWeb.Models;
using EduBank.DAL;
using EduBank.DAL.DataContext;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {

        private readonly ILogger<HomeController> _logger;
        private readonly HomeRepository homeRepository;
        private readonly EdubanckssrContext context;


        public HomeController(ILogger<HomeController> logger, EdubanckssrContext context)
        {
            _logger = logger;
            homeRepository = new HomeRepository(context);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObtenerUsuarioId();  // ← Obtener ID del usuario logueado

            var ingresosM = await homeRepository.ResumenMensualIngresos(usuarioId);
            var gastosM = await homeRepository.ResumenMensualGastos(usuarioId);
            var GastosIngresos = await homeRepository.GastosIngresos(usuarioId);
            var recordatorios = await homeRepository.Recordatorio(usuarioId);
            var graficos = new VMGraficos
            {
                ResumenGastos = (await homeRepository.ResumenGastos(usuarioId)).ResumenGastos,
                ResumenIngresos = (await homeRepository.ResumenIngresos(usuarioId)).ResumenIngresos
            };

            var home = new VMHome
            {
                GastoM = gastosM,
                IngresoM = ingresosM,
                GastosIngresos = GastosIngresos,
                Recor = recordatorios,
                Graficos = graficos
            };

            return View(home);
        }
        public async Task<IActionResult> ResumenIngresos()
        {
            return Json(new { mensaje = "hola" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }



    }
}
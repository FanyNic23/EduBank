using EduBank.BLL.Services;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class TransferenciaController : BaseController
    {
        private readonly ITransferenciaService _transferenciaService;
        private readonly ICuentaService _cuentaService;

        public TransferenciaController(ITransferenciaService transferenciaService, ICuentaService cuentaService)
        {
            _transferenciaService = transferenciaService;
            _cuentaService = cuentaService;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var usuarioId = ObtenerUsuarioId();
        //    var cuentas = await _cuentaService.ObtenerPorUsuario(usuarioId);
        //    var model = new VMTransferencia
        //    {
        //        CuentasUsuario = cuentas.ToList(),
        //        FechaTransferencia = DateTime.Now
        //    };
        //    return View(model);
        //}

        // VISTA PARA VER HISTORIAL
        public async Task<IActionResult> Historial()
        {
            var usuarioId = ObtenerUsuarioId();
            var transferencias = await _transferenciaService.ObtenerTransferenciasPorUsuario(usuarioId);
            return View(transferencias);
        }

        [HttpPost]
        public async Task<IActionResult> RealizarTransferencia([FromBody] VMTransferencia model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos" });

                var transferencia = new Transferencia
                {
                    CuentaOrigenId = model.CuentaOrigenId,
                    CuentaDestinoId = model.CuentaDestinoId,
                    Monto = model.Monto,
                    FechaTransferencia = model.FechaTransferencia,
                    Comentario = model.Comentario
                };

                var resultado = await _transferenciaService.RealizarTransferencia(transferencia);

                return Json(new
                {
                    success = resultado,
                    message = resultado ? "Transferencia realizada exitosamente" : "Error al realizar la transferencia"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var usuarioId = ObtenerUsuarioId();
            var transferencias = await _transferenciaService.ObtenerTransferenciasPorUsuario(usuarioId);
            return View(transferencias);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTransferenciasJson()
        {
            var usuarioId = ObtenerUsuarioId();
            var transferencias = await _transferenciaService.ObtenerTransferenciasPorUsuario(usuarioId);
            return Json(transferencias);
        }

        [HttpPost]
        public async Task<IActionResult> RevertirTransferencia(int id)
        {
            try
            {
                var resultado = await _transferenciaService.RevertirTransferencia(id);
                return Json(new
                {
                    success = resultado,
                    message = resultado ? "Transferencia revertida exitosamente" : "Error al revertir la transferencia"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

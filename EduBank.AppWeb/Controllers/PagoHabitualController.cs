// EduBank.AppWeb/Controllers/PagoHabitualController.cs
using EduBank.BLL.Services;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class PagoHabitualController : BaseController
    {
        private readonly IPagoHabitualService _pagoService;
        private readonly ICuentaService _cuentaService;
        private readonly ICategoriaService _categoriaService;

        public PagoHabitualController(
            IPagoHabitualService pagoService,
            ICuentaService cuentaService,
            ICategoriaService categoriaService)
        {
            _pagoService = pagoService;
            _cuentaService = cuentaService;
            _categoriaService = categoriaService;
        }

        [HttpGet]
        [Route("PagoHabitual")]
        [Route("PagoHabitual/Index")]
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObtenerUsuarioId();
            var pagos = await _pagoService.ObtenerPorUsuario(usuarioId);
            var cuentas = await _cuentaService.ObtenerPorUsuario(usuarioId);
            var categorias = await _categoriaService.ObtenerPorUsuario(usuarioId);

            ViewBag.Cuentas = cuentas.Where(c => c.Activo).ToList();
            ViewBag.Categorias = categorias.Where(c => c.Activo && c.Tipo == "G").ToList();

            return View(pagos);
        }

        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] VMPagoHabitual model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos" });

                var pago = new PagosHabituales
                {
                    UsuarioId = ObtenerUsuarioId(),
                    Nombre = model.Nombre,
                    Frecuencia = model.Frecuencia,
                    UnidadFrecuencia = model.UnidadFrecuencia,
                    FechaInicio = model.FechaInicio,
                    Hora = model.Hora,
                    FechaFin = model.FechaFin,
                    CuentaId = model.CuentaId,
                    CategoriaId = model.CategoriaId,
                    Monto = model.Monto,
                    Comentario = model.Comentario,
                    EsActivo = model.EsActivo,
                    AgregarAutomaticamente = model.AgregarAutomaticamente
                };

                var resultado = await _pagoService.Insertar(pago);

                return Json(new
                {
                    success = resultado,
                    message = resultado ? "Pago habitual registrado exitosamente" : "Error al registrar el pago habitual"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar([FromBody] VMPagoHabitual model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos" });

                // Validar que el pago pertenece al usuario
                var pertenece = await _pagoService.PerteneceAUsuario(model.PagoHabitualId, ObtenerUsuarioId());
                if (!pertenece)
                    return Json(new { success = false, message = "No tiene permisos para editar este pago" });

                var pago = new PagosHabituales
                {
                    PagoHabitualId = model.PagoHabitualId,
                    UsuarioId = ObtenerUsuarioId(),
                    Nombre = model.Nombre,
                    Frecuencia = model.Frecuencia,
                    UnidadFrecuencia = model.UnidadFrecuencia,
                    FechaInicio = model.FechaInicio,
                    Hora = model.Hora,
                    FechaFin = model.FechaFin,
                    CuentaId = model.CuentaId,
                    CategoriaId = model.CategoriaId,
                    Monto = model.Monto,
                    Comentario = model.Comentario,
                    EsActivo = model.EsActivo,
                    AgregarAutomaticamente = model.AgregarAutomaticamente
                };

                var resultado = await _pagoService.Actualizar(pago);

                return Json(new
                {
                    success = resultado,
                    message = resultado ? "Pago habitual actualizado exitosamente" : "Error al actualizar el pago habitual"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Validar que el pago pertenece al usuario
                var pertenece = await _pagoService.PerteneceAUsuario(id, ObtenerUsuarioId());
                if (!pertenece)
                    return Json(new { success = false, message = "No tiene permisos para eliminar este pago" });

                var resultado = await _pagoService.Eliminar(id);

                return Json(new
                {
                    success = resultado,
                    message = resultado ? "Pago habitual eliminado exitosamente" : "Error al eliminar el pago habitual"
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
            var pagos = await _pagoService.ObtenerPorUsuario(usuarioId);
            return View(pagos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerJson(int id)
        {
            try
            {
                Console.WriteLine($"🔍 ObtenerJson - ID solicitado: {id}");

                var usuarioId = ObtenerUsuarioId();
                Console.WriteLine($"🔍 ObtenerJson - UsuarioId: {usuarioId}");

                // Obtener todos los pagos del usuario (que ya incluyen relaciones)
                var pagosUsuario = await _pagoService.ObtenerPorUsuario(usuarioId);
                Console.WriteLine($"🔍 ObtenerJson - Total pagos del usuario: {pagosUsuario.Count()}");

                var pago = pagosUsuario.FirstOrDefault(p => p.PagoHabitualId == id);

                if (pago == null)
                {
                    Console.WriteLine($"❌ ObtenerJson - Pago no encontrado para ID: {id}");
                    return Json(new { success = false, message = "Pago habitual no encontrado" });
                }

                Console.WriteLine($"✅ ObtenerJson - Pago encontrado: {pago.Nombre}");

                // ✅ CREAR EL VIEW MODEL CORRECTAMENTE
                var vm = new VMPagoHabitual
                {
                    PagoHabitualId = pago.PagoHabitualId,
                    Nombre = pago.Nombre ?? string.Empty,
                    Frecuencia = pago.Frecuencia,
                    UnidadFrecuencia = pago.UnidadFrecuencia ?? "D",
                    FechaInicio = pago.FechaInicio,
                    Hora = pago.Hora,
                    FechaFin = pago.FechaFin,
                    CuentaId = pago.CuentaId,
                    CategoriaId = pago.CategoriaId,
                    Monto = pago.Monto,
                    Comentario = pago.Comentario ?? string.Empty,
                    EsActivo = pago.EsActivo,
                    AgregarAutomaticamente = pago.AgregarAutomaticamente
                };

                Console.WriteLine($"📦 ObtenerJson - ViewModel creado:");
                Console.WriteLine($"   - PagoHabitualId: {vm.PagoHabitualId}");
                Console.WriteLine($"   - Nombre: {vm.Nombre}");
                Console.WriteLine($"   - Monto: {vm.Monto}");

                return Json(new { success = true, data = vm });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ObtenerJson - Error: {ex.Message}");
                Console.WriteLine($"💥 ObtenerJson - StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoRequest request)
        {
            try
            {
                // Validar que el pago pertenece al usuario
                var pertenece = await _pagoService.PerteneceAUsuario(request.PagoHabitualId, ObtenerUsuarioId());
                if (!pertenece)
                    return Json(new { success = false, message = "No tiene permisos para modificar este pago" });

                var resultado = await _pagoService.CambiarEstado(request.PagoHabitualId, request.Activo);

                return Json(new
                {
                    success = resultado,
                    message = resultado ?
                        (request.Activo ? "Pago activado exitosamente" : "Pago desactivado exitosamente") :
                        "Error al cambiar el estado del pago"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProximosPagos()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var pagos = await _pagoService.ObtenerProximosPagos(usuarioId, 5);
                var total = await _pagoService.ObtenerTotalPagosProximos(usuarioId);

                return Json(new
                {
                    success = true,
                    data = pagos,
                    total = total
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EjecutarPagoManual(int id)
        {
            try
            {
                // Validar que el pago pertenece al usuario
                var pertenece = await _pagoService.PerteneceAUsuario(id, ObtenerUsuarioId());
                if (!pertenece)
                    return Json(new { success = false, message = "No tiene permisos para ejecutar este pago" });

                var resultado = await _pagoService.EjecutarPagoAutomatico(id);

                return Json(new
                {
                    success = resultado,
                    message = resultado ? "Pago ejecutado exitosamente" : "Error al ejecutar el pago"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class CambiarEstadoRequest : Controllers.CambiarEstadoRequest
        {
            public int PagoHabitualId { get; set; }
            public bool Activo { get; set; }
        }
    }

    // Clase auxiliar para el request de cambiar estado
    public class CambiarEstadoRequest
    {
        public int PagoHabitualId { get; set; }
        public bool Activo { get; set; }
    }
}
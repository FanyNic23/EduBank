using EduBank.BLL.Services;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class CuentaController : BaseController
    {
        private readonly ICuentaService _cuentaService;
        private readonly ILogger<CuentaController> _logger;

        public CuentaController(ICuentaService cuentaService, ILogger<CuentaController> logger)
        {
            _cuentaService = cuentaService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] VMCuenta modelo)
        {
            try
            {
                if (modelo == null)
                    return BadRequest(new { valor = false, mensaje = "Datos inválidos." });

                var usuarioId = ObtenerUsuarioId();

                var entidad = new Cuenta
                {
                    UsuarioId = usuarioId,
                    Nombre = modelo.Nombre,
                    Tipo = modelo.Tipo,
                    Moneda = modelo.Moneda,
                    Saldo = modelo.Saldo,
                    Activo = modelo.Activo
                };

                bool resultado = await _cuentaService.Insertar(entidad);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ? "Cuenta insertada correctamente." : "No se pudo completar la operación."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error interno del servidor.",
                    detalle = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                _logger.LogInformation("Solicitando lista de cuentas para usuario: {UsuarioId}", usuarioId);

                var lista = await _cuentaService.ObtenerPorUsuario(usuarioId);

                // CORREGIR: Usar camelCase para las propiedades
                var vm = lista.Select(c => new
                {
                    cuentaId = c.CuentaId,    // camelCase
                    nombre = c.Nombre,        // camelCase
                    tipo = c.Tipo,            // camelCase
                    moneda = c.Moneda,        // camelCase
                    saldo = c.Saldo,          // camelCase
                    activo = c.Activo         // camelCase
                }).ToList();

                _logger.LogInformation("Retornando {Count} cuentas para usuario {UsuarioId}", vm.Count, usuarioId);

                return Ok(vm);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentando listar cuentas");
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de cuentas para usuario: {UsuarioId}", ObtenerUsuarioId());
                return StatusCode(500, new { valor = false, mensaje = "Error interno del servidor" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] VMCuenta modelo)
        {
            try
            {
                if (modelo == null)
                    return BadRequest(new { valor = false, mensaje = "No se recibieron datos válidos." });

                // Validaciones básicas
                if (modelo.CuentaId == 0)
                    return BadRequest(new { valor = false, mensaje = "ID de cuenta inválido para actualización." });

                if (string.IsNullOrWhiteSpace(modelo.Nombre))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Nombre' es obligatorio." });

                if (string.IsNullOrWhiteSpace(modelo.Tipo))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Tipo' es obligatorio." });

                if (string.IsNullOrWhiteSpace(modelo.Moneda))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Moneda' es obligatorio." });

                // Obtener usuario autenticado
                var usuarioId = ObtenerUsuarioId();

                // Verificar que la cuenta existe y pertenece al usuario
                var cuentaExistente = await _cuentaService.Obtener(modelo.CuentaId);
                if (cuentaExistente == null || cuentaExistente.UsuarioId != usuarioId)
                    return NotFound(new { valor = false, mensaje = "Cuenta no encontrada o no pertenece al usuario actual." });

                // Actualizar la entidad existente
                cuentaExistente.Nombre = modelo.Nombre.Trim();
                cuentaExistente.Tipo = modelo.Tipo.Trim();
                cuentaExistente.Moneda = modelo.Moneda.Trim();
                cuentaExistente.Saldo = modelo.Saldo;
                // No actualizamos Activo aquí, usa CambiarEstado para eso

                var resultado = await _cuentaService.Actualizar(cuentaExistente);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ? "Cuenta actualizada correctamente." : "No se pudo actualizar la cuenta."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado o sesión expirada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error interno del servidor al actualizar la cuenta.",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerJson(int id)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var cuenta = await _cuentaService.Obtener(id);

                if (cuenta == null || cuenta.UsuarioId != usuarioId)
                    return NotFound(new { valor = false, mensaje = "Cuenta no encontrada" });

                var vm = new VMCuenta
                {
                    CuentaId = cuenta.CuentaId,
                    Nombre = cuenta.Nombre,
                    Tipo = cuenta.Tipo,
                    Moneda = cuenta.Moneda,
                    Saldo = cuenta.Saldo,
                    Activo = cuenta.Activo
                };
                return Ok(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var cuenta = await _cuentaService.Obtener(id);
                if (cuenta == null || cuenta.UsuarioId != usuarioId)
                    return NotFound(new { valor = false, mensaje = "Cuenta no encontrada o no autorizada" });

                // Eliminación física
                var ok = await _cuentaService.Eliminar(id);
                return Ok(new
                {
                    valor = ok,
                    mensaje = ok ? "Cuenta eliminada permanentemente" : "Error al eliminar la cuenta"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
            catch (DbUpdateException ex)
            {
                // Manejar error de integridad referencial (si hay movimientos o transferencias asociadas)
                return BadRequest(new
                {
                    valor = false,
                    mensaje = "No se puede eliminar la cuenta porque tiene movimientos o transferencias asociadas"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoRequest model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { valor = false, mensaje = "Datos inválidos" });

                var usuarioId = ObtenerUsuarioId();
                var cuenta = await _cuentaService.Obtener(model.CuentaId);
                if (cuenta == null || cuenta.UsuarioId != usuarioId)
                    return NotFound(new { valor = false, mensaje = "Cuenta no encontrada o no autorizada" });

                // Actualizar estado
                cuenta.Activo = model.Activo;
                var resultado = await _cuentaService.Actualizar(cuenta);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ?
                        (model.Activo ? "Cuenta activada correctamente" : "Cuenta desactivada correctamente") :
                        "Error al cambiar el estado de la cuenta"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { valor = false, mensaje = "Error interno del servidor" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerActivas()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var lista = await _cuentaService.ObtenerPorUsuario(usuarioId);
                var activas = lista.Where(c => c.Activo).ToList();

                var vm = activas.Select(c => new
                {
                    cuentaId = c.CuentaId,
                    nombre = c.Nombre,
                    tipo = c.Tipo,
                    moneda = c.Moneda,
                    saldo = c.Saldo,
                    activo = c.Activo
                }).ToList();

                return Ok(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerInactivas()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var lista = await _cuentaService.ObtenerPorUsuario(usuarioId);

                var inactivas = lista
                    .Where(c => !c.Activo)
                    .Select(c => new
                    {
                        cuentaId = c.CuentaId,
                        nombre = c.Nombre,
                        tipo = c.Tipo,
                        moneda = c.Moneda,
                        saldo = c.Saldo,
                        activo = c.Activo
                    })
                    .ToList();

                return Ok(inactivas);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerSaldoTotal()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var saldoTotal = await _cuentaService.ObtenerSaldoTotal(usuarioId);

                return Ok(new
                {
                    valor = true,
                    saldoTotal = saldoTotal
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

        public class CambiarEstadoRequest
        {
            public int CuentaId { get; set; }
            public bool Activo { get; set; }
        }
    }
}
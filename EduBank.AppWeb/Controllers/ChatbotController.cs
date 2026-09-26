using EduBank.AppWeb.Models; // Ajusta según tus namespaces
using EduBank.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class ChatbotController : BaseController
    {
        private readonly ICuentaService _cuentaService;
        private readonly IMovimientoService _movimientoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IPagoHabitualService _pagoHabitualService;

        public ChatbotController(
            ICuentaService cuentaService,
            IMovimientoService movimientoService,
            ICategoriaService categoriaService,
            IPagoHabitualService pagoHabitualService)
        {
            _cuentaService = cuentaService;
            _movimientoService = movimientoService;
            _categoriaService = categoriaService;
            _pagoHabitualService = pagoHabitualService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDatosUsuario()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();

                var cuentas = await _cuentaService.ObtenerPorUsuario(usuarioId);
                var movimientos = await _movimientoService.ObtenerPorUsuario(usuarioId);
                var categorias = await _categoriaService.ObtenerPorUsuario(usuarioId);
                var pagosHabituales = await _pagoHabitualService.ObtenerPorUsuario(usuarioId);
                var saldoTotal = await _cuentaService.ObtenerSaldoTotal(usuarioId);

                var datosUsuario = new
                {
                    Cuentas = cuentas.Select(c => new {
                        c.CuentaId,
                        c.Nombre,
                        c.Saldo,
                        c.Moneda,
                        c.Activo
                    }),
                    Movimientos = movimientos.Take(50).Select(m => new {
                        m.MovimientoId,
                        m.Tipo,
                        m.Monto,
                        m.FechaOperacion,
                        m.Comentario,
                        CategoriaNombre = m.Categoria?.Nombre,
                        CuentaNombre = m.Cuenta?.Nombre
                    }),
                    Categorias = categorias.Select(c => new {
                        c.CategoriaId,
                        c.Nombre,
                        c.Tipo,
                        c.Activo
                    }),
                    PagosHabituales = pagosHabituales.Select(p => new {
                        p.PagoHabitualId,
                        p.Nombre,
                        p.Monto,
                        p.Frecuencia,
                        p.UnidadFrecuencia,
                        p.EsActivo
                    }),
                    SaldoTotal = saldoTotal,
                    TotalIngresos = await _movimientoService.ObtenerTotalPorTipo('I', usuarioId),
                    TotalGastos = await _movimientoService.ObtenerTotalPorTipo('G', usuarioId)
                };

                return Ok(datosUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error al obtener datos del usuario" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMovimientosPorPeriodo(string periodo = "mes")
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var movimientos = await _movimientoService.ObtenerPorPeriodo(usuarioId, periodo, DateTime.Now);

                var datos = movimientos.Select(m => new {
                    m.MovimientoId,
                    m.Tipo,
                    m.Monto,
                    m.FechaOperacion,
                    m.Comentario,
                    Categoria = m.Categoria?.Nombre,
                    Cuenta = m.Cuenta?.Nombre
                });

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error al obtener movimientos" });
            }
        }

    }
}
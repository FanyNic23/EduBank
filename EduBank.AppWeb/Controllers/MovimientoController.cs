using EduBank.BLL.Services;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class MovimientoController : BaseController
    {
        private readonly IMovimientoService _movService;
        private readonly ICategoriaService _catService;
        private readonly ICuentaService _cuentaService;

        public MovimientoController(IMovimientoService movService, ICategoriaService catService, ICuentaService cuentaService)
        {
            _movService = movService;
            _catService = catService;
            _cuentaService = cuentaService;
        }

        [HttpGet]
        [Route("Movimiento")]
        [Route("Movimiento/Index")]
        // Vista principal del dashboard
        public IActionResult Index()
        {
            if (!EstaAutenticado())
                return RedirectToAction("Login", "Acceso");

            return View();
        }

        // Insertar o actualizar un movimiento (POST JSON)
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] VMMovimiento vm)
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                if (vm == null)
                    return BadRequest(new { valor = false, mensaje = "Datos inválidos" });

                if (vm.Monto <= 0)
                    return BadRequest(new { valor = false, mensaje = "Monto debe ser mayor que 0" });

                if (string.IsNullOrWhiteSpace(vm.Tipo) || !(vm.Tipo == "I" || vm.Tipo == "G"))
                    return BadRequest(new { valor = false, mensaje = "Tipo inválido (I=Ingreso, G=Gasto)" });

                int usuarioId = ObtenerUsuarioId();

                // Validar que la categoría pertenezca al usuario
                var categoria = await _catService.ObtenerPorIdYUsuario(vm.CategoriaId, usuarioId);
                if (categoria == null)
                    return BadRequest(new { valor = false, mensaje = "Categoría no encontrada o no pertenece al usuario" });

                // Validar que la cuenta pertenezca al usuario
                var cuenta = await _cuentaService.ObtenerPorIdYUsuario(vm.CuentaId, usuarioId);
                if (cuenta == null)
                    return BadRequest(new { valor = false, mensaje = "Cuenta no encontrada o no pertenece al usuario" });

                // **NUEVA VALIDACIÓN: Verificar saldo suficiente para gastos**
                if (vm.Tipo == "G" && cuenta.Saldo < vm.Monto)
                    return BadRequest(new
                    {
                        valor = false,
                        mensaje = "Saldo insuficiente en la cuenta",
                        saldoActual = cuenta.Saldo,
                        montoRequerido = vm.Monto
                    });

                // Parsear fecha en formato yyyy-MM-dd
                DateTime fechaOperacion;
                if (!DateTime.TryParseExact(vm.FechaOperacion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaOperacion))
                {
                    fechaOperacion = DateTime.Now.Date;
                }

                var mov = new Movimiento
                {
                    MovimientoId = vm.MovimientoId,
                    CuentaId = vm.CuentaId,
                    CategoriaId = vm.CategoriaId,
                    Tipo = vm.Tipo,
                    FechaOperacion = fechaOperacion,
                    Monto = vm.Monto,
                    Comentario = vm.Comentario,
                    EsAutomatico = false,
                    CreadoEn = DateTime.Now,
                    ActualizadoEn = DateTime.Now,
                    // **IMPORTANTE: Asignar la relación de Cuenta para el filtro por usuario**
                    Cuenta = cuenta
                };

                bool resultado;
                if (vm.MovimientoId == 0)
                {
                    resultado = await _movService.Insertar(mov);
                }
                else
                {
                    resultado = await _movService.Actualizar(mov);
                }

                // **ACTUALIZAR: Obtener el saldo actualizado de la cuenta**
                var cuentaActualizada = await _cuentaService.ObtenerPorIdYUsuario(vm.CuentaId, usuarioId);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ? "Operación realizada correctamente" : "Error al realizar la operación",
                    // **NUEVO: Incluir información del saldo actualizado**
                    saldoActualizado = cuentaActualizada?.Saldo ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        // Lista todos los movimientos del usuario (JSON)
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                int usuarioId = ObtenerUsuarioId();
                var lista = await _movService.ObtenerPorUsuario(usuarioId);

                var vm = lista.Select(m => new VMMovimiento
                {
                    MovimientoId = m.MovimientoId,
                    CuentaId = m.CuentaId,
                    CategoriaId = m.CategoriaId,
                    Tipo = m.Tipo,
                    FechaOperacion = m.FechaOperacion.ToString("yyyy-MM-dd"),
                    Monto = m.Monto,
                    Comentario = m.Comentario,
                    CategoriaNombre = m.Categoria?.Nombre,
                    CategoriaIcono = m.Categoria?.Icono
                }).ToList();

                return Ok(new { valor = true, movimientos = vm });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al obtener movimientos",
                    detalle = ex.Message
                });
            }
        }

        // Obtener un movimiento específico del usuario (JSON)
        [HttpGet]
        public async Task<IActionResult> ObtenerJson(long id)
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                int usuarioId = ObtenerUsuarioId();
                var movimiento = await _movService.Obtener(id, usuarioId);

                if (movimiento == null)
                    return NotFound(new { valor = false, mensaje = "Movimiento no encontrado" });

                var vm = new VMMovimiento
                {
                    MovimientoId = movimiento.MovimientoId,
                    CuentaId = movimiento.CuentaId,
                    CategoriaId = movimiento.CategoriaId,
                    Tipo = movimiento.Tipo,
                    FechaOperacion = movimiento.FechaOperacion.ToString("yyyy-MM-dd"),
                    Monto = movimiento.Monto,
                    Comentario = movimiento.Comentario,
                    CategoriaNombre = movimiento.Categoria?.Nombre,
                    CategoriaIcono = movimiento.Categoria?.Icono
                };

                return Ok(new { valor = true, movimiento = vm });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al obtener movimiento",
                    detalle = ex.Message
                });
            }
        }

        // Eliminar movimiento del usuario
        [HttpDelete]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                int usuarioId = ObtenerUsuarioId();

                // **NUEVO: Obtener el movimiento antes de eliminarlo para saber la cuenta**
                var movimiento = await _movService.Obtener(id, usuarioId);
                if (movimiento == null)
                    return NotFound(new { valor = false, mensaje = "Movimiento no encontrado" });

                bool resultado = await _movService.Eliminar(id, usuarioId);

                // **ACTUALIZAR: Obtener el saldo actualizado de la cuenta**
                var cuentaActualizada = await _cuentaService.ObtenerPorIdYUsuario(movimiento.CuentaId, usuarioId);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ? "Movimiento eliminado correctamente" : "No se pudo eliminar el movimiento",
                    // **NUEVO: Incluir información del saldo actualizado**
                    saldoActualizado = cuentaActualizada?.Saldo ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al eliminar movimiento",
                    detalle = ex.Message
                });
            }
        }

        // Estadísticas para dashboard
        [HttpGet]
        public async Task<IActionResult> Estadisticas()
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                int usuarioId = ObtenerUsuarioId();

                var totalIngresos = await _movService.ObtenerTotalPorTipo('I', usuarioId);
                var totalGastos = await _movService.ObtenerTotalPorTipo('G', usuarioId);
                var saldo = totalIngresos - totalGastos;

                // **NUEVO: Obtener saldo total de todas las cuentas**
                var saldoTotalCuentas = await _cuentaService.ObtenerSaldoTotal(usuarioId);

                var totalesPorCategoria = await _movService.ObtenerTotalesPorCategoria(usuarioId, null);
                var movimientosRecientes = (await _movService.ObtenerRecientes(usuarioId, 10))
                    .Select(m => new
                    {
                        m.MovimientoId,
                        m.CategoriaId,
                        CategoriaNombre = m.Categoria?.Nombre,
                        CategoriaIcono = m.Categoria?.Icono,
                        m.Tipo,
                        FechaOperacion = m.FechaOperacion.ToString("yyyy-MM-dd"),
                        m.Monto,
                        m.Comentario,
                        CuentaNombre = m.Cuenta?.Nombre // **NUEVO: Incluir nombre de cuenta**
                    });

                return Ok(new
                {
                    valor = true,
                    TotalIngresos = totalIngresos,
                    TotalGastos = totalGastos,
                    SaldoMovimientos = saldo,
                    SaldoTotalCuentas = saldoTotalCuentas, // **NUEVO: Saldo real de cuentas**
                    Diferencia = saldoTotalCuentas - saldo, // **NUEVO: Para detectar inconsistencias**
                    TotalesPorCategoria = totalesPorCategoria.Select(t => new {
                        t.CategoriaId,
                        t.CategoriaNombre,
                        t.Total
                    }),
                    Recientes = movimientosRecientes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al obtener estadísticas",
                    detalle = ex.Message
                });
            }
        }

        // Totales por categoría filtrando por tipo
        [HttpGet]
        public async Task<IActionResult> TotalesPorCategoria(char tipo)
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                if (tipo != 'I' && tipo != 'G')
                    return BadRequest(new { valor = false, mensaje = "Tipo inválido (solo I o G)" });

                int usuarioId = ObtenerUsuarioId();
                var datos = await _movService.ObtenerTotalesPorCategoria(usuarioId, tipo);

                return Ok(new
                {
                    valor = true,
                    datos = datos.Select(d => new {
                        d.CategoriaId,
                        d.CategoriaNombre,
                        d.Total
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al obtener totales por categoría",
                    detalle = ex.Message
                });
            }
        }

        // Obtener movimientos por período
        [HttpGet]
        public async Task<IActionResult> ObtenerPorPeriodo(string periodo, string fecha)
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                // Validar parámetros
                if (string.IsNullOrEmpty(periodo) || string.IsNullOrEmpty(fecha))
                    return BadRequest(new { valor = false, mensaje = "Parámetros incompletos" });

                // Parsear fecha
                if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaReferencia))
                    return BadRequest(new { valor = false, mensaje = "Formato de fecha inválido. Use yyyy-MM-dd" });

                // Validar período
                var periodosValidos = new[] { "dia", "semana", "mes", "año" };
                if (!periodosValidos.Contains(periodo.ToLower()))
                    return BadRequest(new { valor = false, mensaje = "Período no válido. Use: dia, semana, mes, año" });

                int usuarioId = ObtenerUsuarioId();

                // Obtener movimientos
                var movimientos = await _movService.ObtenerPorPeriodo(usuarioId, periodo, fechaReferencia);

                // Obtener rango del período
                var rango = await _movService.ObtenerRangoPeriodo(periodo, fechaReferencia);

                // Calcular totales
                var totalIngresos = movimientos.Where(m => m.Tipo == "I").Sum(m => m.Monto);
                var totalGastos = movimientos.Where(m => m.Tipo == "G").Sum(m => m.Monto);
                var saldo = totalIngresos - totalGastos;

                // Convertir a ViewModel
                var vm = movimientos.Select(m => new VMMovimiento
                {
                    MovimientoId = m.MovimientoId,
                    CuentaId = m.CuentaId,
                    CategoriaId = m.CategoriaId,
                    Tipo = m.Tipo,
                    FechaOperacion = m.FechaOperacion.ToString("yyyy-MM-dd"),
                    Monto = m.Monto,
                    Comentario = m.Comentario,
                    CategoriaNombre = m.Categoria?.Nombre,
                    CategoriaIcono = m.Categoria?.Icono
                }).ToList();

                return Ok(new
                {
                    valor = true,
                    movimientos = vm,
                    totalIngresos,
                    totalGastos,
                    saldo,
                    rango = new
                    {
                        inicio = rango.Inicio.ToString("yyyy-MM-dd"),
                        fin = rango.Fin.ToString("yyyy-MM-dd")
                    },
                    periodoActual = periodo,
                    fechaReferencia = fechaReferencia.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al obtener movimientos por período",
                    detalle = ex.Message
                });
            }
        }

        // Estadísticas por período
        [HttpGet]
        public async Task<IActionResult> EstadisticasPorPeriodo(string periodo, string fecha)
        {
            try
            {
                if (!EstaAutenticado())
                    return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });

                // Validar parámetros
                if (string.IsNullOrEmpty(periodo) || string.IsNullOrEmpty(fecha))
                    return BadRequest(new { valor = false, mensaje = "Parámetros incompletos" });

                // Parsear fecha
                if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaReferencia))
                    return BadRequest(new { valor = false, mensaje = "Formato de fecha inválido. Use yyyy-MM-dd" });

                // Validar período
                var periodosValidos = new[] { "dia", "semana", "mes", "año" };
                if (!periodosValidos.Contains(periodo.ToLower()))
                    return BadRequest(new { valor = false, mensaje = "Período no válido. Use: dia, semana, mes, año" });

                int usuarioId = ObtenerUsuarioId();

                // Obtener movimientos
                var movimientos = await _movService.ObtenerPorPeriodo(usuarioId, periodo, fechaReferencia);

                // Calcular totales
                var totalIngresos = movimientos.Where(m => m.Tipo == "I").Sum(m => m.Monto);
                var totalGastos = movimientos.Where(m => m.Tipo == "G").Sum(m => m.Monto);
                var saldo = totalIngresos - totalGastos;

                // Calcular totales por categoría
                var ingresosPorCategoria = movimientos
                    .Where(m => m.Tipo == "I")
                    .GroupBy(m => new { m.CategoriaId, m.Categoria.Nombre })
                    .Select(g => new {
                        g.Key.CategoriaId,
                        g.Key.Nombre,
                        Total = g.Sum(m => m.Monto)
                    })
                    .OrderByDescending(x => x.Total);

                var gastosPorCategoria = movimientos
                    .Where(m => m.Tipo == "G")
                    .GroupBy(m => new { m.CategoriaId, m.Categoria.Nombre })
                    .Select(g => new {
                        g.Key.CategoriaId,
                        g.Key.Nombre,
                        Total = g.Sum(m => m.Monto)
                    })
                    .OrderByDescending(x => x.Total);

                // Obtener rango del período
                var rango = await _movService.ObtenerRangoPeriodo(periodo, fechaReferencia);

                return Ok(new
                {
                    valor = true,
                    totalIngresos,
                    totalGastos,
                    saldo,
                    ingresosPorCategoria,
                    gastosPorCategoria,
                    rango = new
                    {
                        inicio = rango.Inicio.ToString("yyyy-MM-dd"),
                        fin = rango.Fin.ToString("yyyy-MM-dd")
                    },
                    periodoActual = periodo,
                    fechaReferencia = fechaReferencia.ToString("yyyy-MM-dd")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error al obtener estadísticas por período",
                    detalle = ex.Message
                });
            }
        }
    }
}
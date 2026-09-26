using System;
using System.Linq;
using System.Threading.Tasks;
using EduBank.BLL.Services;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduBank.AppWeb.Controllers
{
    [Authorize]
    public class CategoriaController : BaseController // HEREDAR DE BASECONTROLLER
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] VMCategoria modelo)
        {
            try
            {
                // Validación del modelo recibido
                if (modelo == null)
                    return BadRequest(new { valor = false, mensaje = "No se recibieron datos válidos." });

                if (string.IsNullOrWhiteSpace(modelo.Nombre))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Nombre' es obligatorio." });

                if (string.IsNullOrWhiteSpace(modelo.Tipo))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Tipo' es obligatorio (I o G)." });

                // Obtener usuario autenticado (puede lanzar UnauthorizedAccessException)
                var usuarioId = ObtenerUsuarioId();

                // Construcción de la entidad
                var entidad = new Categoria
                {
                    Nombre = modelo.Nombre.Trim(),
                    Descripcion = modelo.Descripcion?.Trim(),
                    Tipo = modelo.Tipo.Trim(),
                    Icono = modelo.Icono,
                    Color = modelo.Color,
                    Activo = true,
                    UsuarioId = usuarioId
                };

                bool resultado;

                // Lógica para insertar o actualizar
                if (modelo.CategoriaId == 0)
                {
                    resultado = await _categoriaService.Insertar(entidad);
                }
                else
                {
                    var categoriaExistente = await _categoriaService.ObtenerPorIdYUsuario(modelo.CategoriaId, usuarioId);
                    if (categoriaExistente == null)
                        return NotFound(new { valor = false, mensaje = "Categoría no encontrada o no pertenece al usuario actual." });

                    entidad.CategoriaId = modelo.CategoriaId;
                    resultado = await _categoriaService.Actualizar(entidad);
                }

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ? "Operación realizada correctamente." : "No se pudo completar la operación."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado o sesión expirada." });
            }
            catch (ArgumentException ex)
            {
                // Para errores de parámetros inválidos
                return BadRequest(new { valor = false, mensaje = $"Error de argumento: {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                // Por ejemplo, errores de lógica en el servicio
                return StatusCode(500, new { valor = false, mensaje = $"Error de operación: {ex.Message}" });
            }
            catch (Exception ex)
            {
                // Error general: útil para depuración
                return StatusCode(500, new
                {
                    valor = false,
                    mensaje = "Error interno del servidor.",
                    detalle = ex.Message // ⚠️ puedes quitar 'detalle' en producción
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                // ✅ Este método ahora devuelve TODAS las categorías (activas e inactivas)
                var lista = await _categoriaService.ObtenerPorUsuario(usuarioId);

                var vm = lista.Select(c => new VMCategoria
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Tipo = c.Tipo,
                    Icono = c.Icono,
                    Color = c.Color,
                    Activo = c.Activo, // ← Esto ahora mostrará el estado real
                    UsuarioId = c.UsuarioId
                }).ToList();

                return Ok(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] VMCategoria modelo)
        {
            try
            {
                if (modelo == null)
                    return BadRequest(new { valor = false, mensaje = "No se recibieron datos válidos." });

                // Validaciones básicas
                if (modelo.CategoriaId == 0)
                    return BadRequest(new { valor = false, mensaje = "ID de categoría inválido para actualización." });

                if (string.IsNullOrWhiteSpace(modelo.Nombre))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Nombre' es obligatorio." });

                if (string.IsNullOrWhiteSpace(modelo.Tipo))
                    return BadRequest(new { valor = false, mensaje = "El campo 'Tipo' es obligatorio (I o G)." });

                // Obtener usuario autenticado
                var usuarioId = ObtenerUsuarioId();

                // Verificar que la categoría existe y pertenece al usuario
                var categoriaExistente = await _categoriaService.ObtenerPorIdYUsuario(modelo.CategoriaId, usuarioId);
                if (categoriaExistente == null)
                    return NotFound(new { valor = false, mensaje = "Categoría no encontrada o no pertenece al usuario actual." });

                // ✅ CORRECCIÓN: Actualizar la entidad existente en lugar de crear una nueva
                categoriaExistente.Nombre = modelo.Nombre.Trim();
                categoriaExistente.Descripcion = modelo.Descripcion?.Trim();
                categoriaExistente.Tipo = modelo.Tipo.Trim();
                categoriaExistente.Icono = modelo.Icono;
                categoriaExistente.Color = modelo.Color;
                // No actualizamos Activo aquí, usa CambiarEstado para eso

                var resultado = await _categoriaService.Actualizar(categoriaExistente);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ? "Categoría actualizada correctamente." : "No se pudo actualizar la categoría."
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
                    mensaje = "Error interno del servidor al actualizar la categoría.",
                    detalle = ex.Message // Para debugging
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerJson(int id)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var c = await _categoriaService.ObtenerPorIdYUsuario(id, usuarioId);
                if (c == null)
                    return NotFound(new { valor = false, mensaje = "Categoría no encontrada" });

                var vm = new VMCategoria
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Tipo = c.Tipo,
                    Icono = c.Icono,
                    Color = c.Color,
                    Activo = c.Activo,
                    UsuarioId = c.UsuarioId
                };
                return Ok(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

        [HttpDelete]
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var categoria = await _categoriaService.ObtenerPorIdYUsuario(id, usuarioId);
                if (categoria == null)
                    return NotFound(new { valor = false, mensaje = "Categoría no encontrada o no autorizada" });

                // AHORA: Hard Delete (eliminación física)
                var ok = await _categoriaService.Eliminar(id);
                return Ok(new
                {
                    valor = ok,
                    mensaje = ok ? "Categoría eliminada permanentemente" : "Error al eliminar la categoría"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
            catch (DbUpdateException ex)
            {
                // Manejar error de integridad referencial (si hay movimientos asociados)
                return BadRequest(new
                {
                    valor = false,
                    mensaje = "No se puede eliminar la categoría porque tiene movimientos asociados"
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
                var categoria = await _categoriaService.ObtenerPorIdYUsuario(model.CategoriaId, usuarioId);
                if (categoria == null)
                    return NotFound(new { valor = false, mensaje = "Categoría no encontrada o no autorizada" });

                // AHORA: Usa el método específico para cambiar estado
                var resultado = await _categoriaService.CambiarEstado(model.CategoriaId, model.Activo);

                return Ok(new
                {
                    valor = resultado,
                    mensaje = resultado ?
                        (model.Activo ? "Categoría activada correctamente" : "Categoría desactivada correctamente") :
                        "Error al cambiar el estado de la categoría"
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

        public class CambiarEstadoRequest
        {
            public int CategoriaId { get; set; }
            public bool Activo { get; set; }
        }

        // NUEVO: Endpoint para obtener categorías activas
        [HttpGet]
        public async Task<IActionResult> ObtenerActivas()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var lista = await _categoriaService.ObtenerPorUsuario(usuarioId);
                var activas = lista.Where(c => c.Activo).ToList();

                var vm = activas.Select(c => new VMCategoria
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Tipo = c.Tipo,
                    Icono = c.Icono,
                    Color = c.Color,
                    Activo = c.Activo
                }).ToList();

                return Ok(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { valor = false, mensaje = "Usuario no autenticado" });
            }
        }

    }
}
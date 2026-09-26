using EduBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduBank.BLL.Services
{
    public interface ITransferenciaService
    {
        Task<bool> RealizarTransferencia(Transferencia transferencia);
        Task<bool> RevertirTransferencia(int transferenciaId);
        Task<Transferencia?> ObtenerTransferencia(int id);
        Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorUsuario(int usuarioId);
        Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorCuenta(int cuentaId);
        Task<IEnumerable<Transferencia>> ObtenerTransferenciasPorPeriodo(int usuarioId, DateTime desde, DateTime hasta);
        Task<decimal> ObtenerTotalTransferenciasPorPeriodo(int usuarioId, DateTime desde, DateTime hasta);
    }
}

using System.Threading.Tasks;
using EduBank.Models.ViewModel;

namespace EduBank.DAL;

public interface IHomeRepository
{
    Task<VMHomeRMensual> ResumenMensualIngresos(int usuarioId);
    Task<VMHomeRMensual> ResumenMensualGastos(int usuarioId);
    Task<VMHPorcentajeGI> GastosIngresos(int usuarioId);
    Task<VMHomeRecordatorios> Recordatorio(int usuarioId);
    Task<VMGraficos> ResumenGastos(int usuarioId);
    Task<VMGraficos> ResumenIngresos(int usuarioId);
}
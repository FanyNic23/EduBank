using EduBank.DAL.DataContext;
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EduBank.DAL;

public class HomeRepository : IHomeRepository
{
    private readonly EdubanckssrContext _dbContext;

    public HomeRepository(EdubanckssrContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VMHPorcentajeGI> GastosIngresos(int usuarioId)
    {
        // Filtrar movimientos por usuario a través de la cuenta
        var movimientosQuery = _dbContext.Movimientos
            .Include(m => m.Cuenta)
            .Where(m => m.Cuenta.UsuarioId == usuarioId);

        var totalMovimientos = await movimientosQuery.CountAsync();

        if (totalMovimientos == 0)
        {
            return new VMHPorcentajeGI
            {
                Gastos = 0,
                Ingresos = 0
            };
        }

        int totalIngresos = await movimientosQuery.CountAsync(i => i.Tipo == "I");
        int totalGastos = await movimientosQuery.CountAsync(t => t.Tipo == "G");

        decimal PorcentajeIngreso = (decimal)totalIngresos / totalMovimientos * 100;
        decimal PorcentajeGasto = (decimal)totalGastos / totalMovimientos * 100;

        return new VMHPorcentajeGI { Ingresos = PorcentajeIngreso, Gastos = PorcentajeGasto };
    }

    public async Task<VMHomeRMensual> ResumenMensualIngresos(int usuarioId)
    {
        var hoy = DateTime.UtcNow;
        var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioMesAnterior = inicioMesActual.AddMonths(-1);
        var finMesAnterior = inicioMesActual.AddDays(-1);

        // Filtrar por usuario
        var movimientosQuery = _dbContext.Movimientos
            .Include(m => m.Cuenta)
            .Where(m => m.Cuenta.UsuarioId == usuarioId);

        decimal IngresosA = await movimientosQuery
            .Where(i => i.Tipo == "I" && i.CreadoEn >= inicioMesActual && i.CreadoEn <= hoy)
            .SumAsync(i => i.Monto);

        decimal IngresoP = await movimientosQuery
            .Where(I => I.Tipo == "I" && I.CreadoEn >= inicioMesAnterior && I.CreadoEn <= finMesAnterior)
            .SumAsync(i => i.Monto);

        decimal IngresoPorcentaje = IngresoP > 0 ? ((IngresosA - IngresoP) / IngresoP) * 100 : 0;

        return new VMHomeRMensual
        {
            Monto = IngresosA,
            Porcentaje = IngresoPorcentaje
        };
    }

    public async Task<VMHomeRMensual> ResumenMensualGastos(int usuarioId)
    {
        var hoy = DateTime.UtcNow;
        var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioMesAnterior = inicioMesActual.AddMonths(-1);
        var finMesAnterior = inicioMesActual.AddDays(-1);

        // Filtrar por usuario
        var movimientosQuery = _dbContext.Movimientos
            .Include(m => m.Cuenta)
            .Where(m => m.Cuenta.UsuarioId == usuarioId);

        decimal GastosA = await movimientosQuery
            .Where(i => i.Tipo == "G" && i.CreadoEn >= inicioMesActual && i.CreadoEn <= hoy)
            .SumAsync(i => i.Monto);

        decimal GastosP = await movimientosQuery
            .Where(I => I.Tipo == "G" && I.CreadoEn >= inicioMesAnterior && I.CreadoEn <= finMesAnterior)
            .SumAsync(i => i.Monto);

        decimal GastosPorcentaje = GastosP > 0 ? ((GastosA - GastosP) / GastosP) * 100 : 0;

        return new VMHomeRMensual
        {
            Monto = GastosA,
            Porcentaje = GastosPorcentaje
        };
    }

    public async Task<VMHomeRecordatorios> Recordatorio(int usuarioId)
    {
        // Filtrar recordatorios por usuario
        int RPendientes = await _dbContext.RecordatoriosGenerales
            .CountAsync(r => r.UsuarioId == usuarioId && r.EsActivo == true);

        var hoy = DateTime.UtcNow;
        var fechaReciente = hoy.AddDays(-3);

        int RecoRecientes = await _dbContext.RecordatoriosGenerales
            .CountAsync(r => r.UsuarioId == usuarioId &&
                           r.FechaInicio >= fechaReciente &&
                           r.FechaInicio <= hoy &&
                           r.EsActivo == true);

        var records = await _dbContext.RecordatoriosGenerales
            .Where(r => r.UsuarioId == usuarioId && r.EsActivo == true)
            .ToListAsync();

        return new VMHomeRecordatorios
        {
            RecordatoriosPendientes = RPendientes,
            RecordatoriosRecientes = RecoRecientes,
            recordatorio = records
        };
    }

    public async Task<VMGraficos> ResumenGastos(int usuarioId)
    {
        var movimientos = await _dbContext.Movimientos
            .Include(m => m.Cuenta)
            .Where(m => m.Cuenta.UsuarioId == usuarioId && m.Tipo == "G")
            .ToListAsync();

        var resumen = movimientos
            .GroupBy(m => new { Mes = m.CreadoEn.ToString("MMM") })
            .Select(g => new ResumenGastos
            {
                Mes = g.Key.Mes,
                Total = g.Sum(m => m.Monto)
            })
            .ToList();

        return new VMGraficos { ResumenGastos = resumen };
    }

    public async Task<VMGraficos> ResumenIngresos(int usuarioId)
    {
        var movimientos = await _dbContext.Movimientos
            .Include(m => m.Cuenta)
            .Include(m => m.Categoria)
            .Where(m => m.Cuenta.UsuarioId == usuarioId && m.Tipo == "I")
            .ToListAsync();

        var resumen = movimientos
            .GroupBy(m => m.Categoria.Nombre)
            .Select(g => new ResumenIngresos
            {
                Categoria = g.Key,
                Total = g.Sum(m => m.Monto)
            })
            .ToList();

        return new VMGraficos { ResumenIngresos = resumen };
    }
}
namespace EduBank.Models.ViewModel;

public class VMHome
{
    public VMHomeRMensual IngresoM { get; set; }
    public VMHomeRMensual GastoM { get; set; }
    public VMHPorcentajeGI GastosIngresos { get; set; }
    public VMHomeRecordatorios Recor { get; set; }
    public VMGraficos Graficos { get; set; }


}
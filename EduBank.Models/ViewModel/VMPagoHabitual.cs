public class VMPagoHabitual
{
    public int PagoHabitualId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Frecuencia { get; set; }
    public string UnidadFrecuencia { get; set; } = "D";
    public DateTime FechaInicio { get; set; }
    public TimeOnly? Hora { get; set; }
    public DateTime? FechaFin { get; set; }
    public int CuentaId { get; set; }
    public int CategoriaId { get; set; }
    public decimal Monto { get; set; }
    public string? Comentario { get; set; }
    public bool EsActivo { get; set; }
    public bool AgregarAutomaticamente { get; set; }
}
using EduBank.Models;
using EduBank.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EduBank.DAL.DataContext;

public partial class EdubanckssrContext : DbContext
{
    public EdubanckssrContext()
    {
    }

    public EdubanckssrContext(DbContextOptions<EdubanckssrContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<PagosHabituales> PagosHabituales { get; set; }

    public virtual DbSet<RecordatoriosGenerale> RecordatoriosGenerales { get; set; }

    public virtual DbSet<Transferencia> Transferencias { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1E5A78E2589");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Icono).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Usuario).WithMany(p => p.Categoria)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_Categorias_Usuarios");
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.CuentaId).HasName("PK__Cuentas__40072E81B15FAC96");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Moneda)
                .HasMaxLength(10)
                .HasDefaultValue("PEN");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Tipo).HasMaxLength(50);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_Cuentas_Usuarios");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.MovimientoId).HasName("PK__Movimien__BF923C2CB735E795");

            entity.Property(e => e.ActualizadoEn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Categoria).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimientos_Categorias");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.CuentaId)
                .HasConstraintName("FK_Movimientos_Cuentas");
        });

        modelBuilder.Entity<PagosHabituales>(entity =>
        {
            entity.HasKey(e => e.PagoHabitualId).HasName("PK__PagosHab__03A10EC8089FA647");

            entity.Property(e => e.EsActivo).HasDefaultValue(true);
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.UnidadFrecuencia)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Categoria).WithMany(p => p.PagosHabituales)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PagosHabituales_Categorias");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.PagosHabituales)
                .HasForeignKey(d => d.CuentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PagosHabituales_Cuentas");

            entity.HasOne(d => d.Usuario).WithMany(p => p.PagosHabituales)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_PagosHabituales_Usuarios");
        });

        modelBuilder.Entity<RecordatoriosGenerale>(entity =>
        {
            entity.HasKey(e => e.RecordatorioId).HasName("PK__Recordat__9E994668D9C4A2EF");

            entity.Property(e => e.EsActivo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.UnidadFrecuencia)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Usuario).WithMany(p => p.RecordatoriosGenerales)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK_RecordatoriosGenerales_Usuarios");
        });

        modelBuilder.Entity<Transferencia>(entity =>
        {
            entity.HasKey(e => e.TransferenciaId).HasName("PK__Transfer__E5B4F5D235DB1FFF");

            entity.Property(e => e.FechaTransferencia).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CuentaDestino).WithMany(p => p.TransferenciaCuentaDestinos)
                .HasForeignKey(d => d.CuentaDestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencias_CuentaDestino");

            entity.HasOne(d => d.CuentaOrigen).WithMany(p => p.TransferenciaCuentaOrigens)
                .HasForeignKey(d => d.CuentaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencias_CuentaOrigen");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B89601EDC4");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Usuarios__531402F32B5BF4F0").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Contrasena).HasMaxLength(255);
            entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

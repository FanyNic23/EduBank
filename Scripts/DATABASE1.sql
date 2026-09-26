USE edubanckssr



-- =============================================
-- ELIMINAR TABLAS EXISTENTES (en orden correcto por dependencias)
-- =============================================
IF OBJECT_ID('Transferencias', 'U') IS NOT NULL DROP TABLE Transferencias;
IF OBJECT_ID('PagosHabituales', 'U') IS NOT NULL DROP TABLE PagosHabituales;
IF OBJECT_ID('RecordatoriosGenerales', 'U') IS NOT NULL DROP TABLE RecordatoriosGenerales;
IF OBJECT_ID('Movimientos', 'U') IS NOT NULL DROP TABLE Movimientos;
IF OBJECT_ID('Categorias', 'U') IS NOT NULL DROP TABLE Categorias;
IF OBJECT_ID('Cuentas', 'U') IS NOT NULL DROP TABLE Cuentas;
IF OBJECT_ID('Usuarios', 'U') IS NOT NULL DROP TABLE Usuarios;
GO


select * from Usuarios

-- =============================================
-- TABLA: Usuarios (CORREGIDA)
-- =============================================
CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    CorreoElectronico NVARCHAR(100) NOT NULL UNIQUE,
    Contrasena NVARCHAR(255) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- TABLA: Cuentas (CORREGIDA)
-- =============================================
CREATE TABLE Cuentas (
    CuentaId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL CHECK (Tipo IN ('Efectivo', 'Bancaria', 'Tarjeta', 'Inversión')),
    Saldo DECIMAL(18,2) NOT NULL DEFAULT 0,
    Moneda NVARCHAR(10) NOT NULL DEFAULT 'PEN',
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Cuentas_Usuarios FOREIGN KEY (UsuarioId) 
        REFERENCES Usuarios(UsuarioId) ON DELETE CASCADE
);
GO

-- =============================================
-- TABLA: Categorias (¡CORRECCIÓN CRÍTICA!)
-- =============================================
CREATE TABLE Categorias (
    CategoriaId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,  -- Cambiado de NULL a NOT NULL (según diagrama)
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Tipo CHAR(1) NOT NULL CHECK (Tipo IN ('I', 'G')),  -- ¡CORREGIDO! 'I','G' no 'Ingreso','Gasto'
    Icono NVARCHAR(50) NULL,
    Color NVARCHAR(20) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Categorias_Usuarios FOREIGN KEY (UsuarioId) 
        REFERENCES Usuarios(UsuarioId) ON DELETE CASCADE
);
GO

-- =============================================
-- TABLA: Movimientos (CORREGIDA - relación con Cuentas)
-- =============================================
CREATE TABLE Movimientos (
    MovimientoId BIGINT IDENTITY(1,1) PRIMARY KEY,
    CuentaId INT NOT NULL,      -- ¡AGREGADO! Relación con Cuentas
    CategoriaId INT NOT NULL,
    Tipo CHAR(1) NOT NULL CHECK (Tipo IN ('I', 'G')),
    FechaOperacion DATETIME2 NOT NULL,
    Monto DECIMAL(18,2) NOT NULL CHECK (Monto > 0),
    Comentario NVARCHAR(MAX) NULL,
    EsAutomatico BIT NOT NULL DEFAULT 0,
    CreadoEn DATETIME2 NOT NULL DEFAULT GETDATE(),
    ActualizadoEn DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Movimientos_Cuentas FOREIGN KEY (CuentaId) 
        REFERENCES Cuentas(CuentaId) ON DELETE CASCADE,
    CONSTRAINT FK_Movimientos_Categorias FOREIGN KEY (CategoriaId) 
        REFERENCES Categorias(CategoriaId)
);
GO

-- =============================================
-- TABLA: Transferencias (CORREGIDA)
-- =============================================
CREATE TABLE Transferencias (
    TransferenciaId INT IDENTITY(1,1) PRIMARY KEY,
    CuentaOrigenId INT NOT NULL,
    CuentaDestinoId INT NOT NULL,
    Monto DECIMAL(18,2) NOT NULL CHECK (Monto > 0),
    FechaTransferencia DATETIME2 NOT NULL DEFAULT GETDATE(),
    Comentario NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Transferencias_CuentaOrigen FOREIGN KEY (CuentaOrigenId) 
        REFERENCES Cuentas(CuentaId),
    CONSTRAINT FK_Transferencias_CuentaDestino FOREIGN KEY (CuentaDestinoId) 
        REFERENCES Cuentas(CuentaId),
    CONSTRAINT CHK_Transferencias_CuentasDiferentes 
        CHECK (CuentaOrigenId != CuentaDestinoId)
);
GO

-- =============================================
-- TABLA: PagosHabituales (CORREGIDA)
-- =============================================
CREATE TABLE PagosHabituales (
    PagoHabitualId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Frecuencia INT NOT NULL CHECK (Frecuencia > 0),
    UnidadFrecuencia CHAR(1) NOT NULL CHECK (UnidadFrecuencia IN ('D', 'S', 'M')),
    FechaInicio DATETIME2 NOT NULL,
    Hora TIME NULL,
    FechaFin DATETIME2 NULL,
    CuentaId INT NOT NULL,
    CategoriaId INT NOT NULL,
    Monto DECIMAL(18,2) NOT NULL CHECK (Monto > 0),
    Comentario NVARCHAR(MAX) NULL,
    EsActivo BIT NOT NULL DEFAULT 1,
    AgregarAutomaticamente BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_PagosHabituales_Usuarios FOREIGN KEY (UsuarioId) 
        REFERENCES Usuarios(UsuarioId) ON DELETE CASCADE,
    CONSTRAINT FK_PagosHabituales_Cuentas FOREIGN KEY (CuentaId) 
        REFERENCES Cuentas(CuentaId),
    CONSTRAINT FK_PagosHabituales_Categorias FOREIGN KEY (CategoriaId) 
        REFERENCES Categorias(CategoriaId)
);
GO

-- =============================================
-- TABLA: RecordatoriosGenerales (CORREGIDA)
-- =============================================
CREATE TABLE RecordatoriosGenerales (
    RecordatorioId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Frecuencia INT NOT NULL CHECK (Frecuencia > 0),
    UnidadFrecuencia CHAR(1) NOT NULL CHECK (UnidadFrecuencia IN ('D', 'S', 'M')),
    FechaInicio DATETIME2 NOT NULL,
    Hora TIME NULL,
    FechaFin DATETIME2 NULL,
    Comentario NVARCHAR(MAX) NULL,
    EsActivo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_RecordatoriosGenerales_Usuarios FOREIGN KEY (UsuarioId) 
        REFERENCES Usuarios(UsuarioId) ON DELETE CASCADE
);
GO




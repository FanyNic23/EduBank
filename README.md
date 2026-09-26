# EduBank SSR

Aplicacion web ASP.NET Core para gestionar cuentas, movimientos, transferencias y pagos habituales.

## Requisitos

- .NET 8 SDK
- SQL Server o SQL Server Express

## Configuracion local

1. Crear la base de datos ejecutando `Scripts/DATABASE1.sql`.
2. Configurar la cadena de conexion sin subir credenciales al repositorio.

En PowerShell, por ejemplo:

```powershell
$env:ConnectionStrings__cadenaSQL = "Server=YOUR_SERVER;Database=edubanckssr;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
dotnet run --project .\EduBank.AppWeb
```

Tambien se puede copiar `EduBank.AppWeb/appsettings.example.json` a una configuracion local ignorada por Git y completar sus valores.

## Ejecutar

```powershell
dotnet restore
dotnet build .\EduBank_SSR_NET.sln
dotnet run --project .\EduBank.AppWeb
```

No se deben subir contrasenas, tokens, certificados, bases de datos locales ni archivos generados en `bin/`, `obj/` o `.vs/`.

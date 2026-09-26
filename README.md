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

## Despliegue en Azure con Docker

El proyecto incluye un `Dockerfile`, infraestructura Bicep en `infra/main.bicep` y un workflow de GitHub Actions.

### Probar el contenedor localmente

```powershell
docker build -t edubank:local .
docker run --rm -p 8080:8080 `
	-e ConnectionStrings__cadenaSQL="Server=YOUR_SERVER;Database=edubanckssr;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False" `
	edubank:local
```

Abrir `http://localhost:8080`.

### Crear la infraestructura

Instalar Azure CLI y Bicep, iniciar sesion con `az login` y crear un grupo de recursos en una region permitida por la suscripcion. El nombre debe ser globalmente unico porque se usa para ACR, SQL Server y App Service.

```powershell
az group create --name rg-edubank --location chilecentral
az deployment group create `
	--resource-group rg-edubank `
	--template-file infra/main.bicep `
	--parameters namePrefix=edubankfany `
		sqlAdministratorLogin=edubankadmin `
		sqlAdministratorPassword="USE_UNA_CONTRASENA_SEGURA"
```

La contrasena debe proporcionarse mediante un mecanismo seguro en entornos reales, no pegarse en el historial de la terminal. El script SQL se ejecuta despues contra la base de datos creada. Para produccion se recomienda reemplazar el firewall publico por Private Endpoint y usar Azure Key Vault para el secreto SQL.

### GitHub Actions

El workflow `.github/workflows/deploy-azure.yml` requiere estos secrets del repositorio:

- `AZURE_CREDENTIALS`: credencial JSON de un service principal con permisos sobre el grupo de recursos.
- `ACR_LOGIN_SERVER`, `ACR_USERNAME` y `ACR_PASSWORD`: credenciales del registro ACR.
- `AZURE_WEBAPP_NAME`: nombre del App Service.

La cadena `ConnectionStrings__cadenaSQL` se configura en App Service, nunca en GitHub ni en el `Dockerfile`.
Para un entorno productivo se recomienda sustituir las credenciales administrativas de ACR por OIDC y permisos `AcrPush` asignados al service principal.

## Estado del despliegue Azure

El proyecto fue desplegado manualmente y validado correctamente en Azure:

- Aplicacion: `Azure App Service for Containers`.
- Contenedor: imagen Docker .NET 8 publicada en Azure Container Registry.
- Base de datos: `Azure SQL Database` con el esquema de `Scripts/DATABASE1.sql`.
- Region: `chilecentral`.
- Grupo de recursos: `rg-edubank-chile`.
- URL: https://edubankfany26-app.azurewebsites.net
- Validacion: App Service en estado `Running`, respuesta HTTP 200 y registro de usuario probado.

El despliegue manual es el estado funcional de referencia. La automatizacion mediante GitHub Actions esta preparada en `.github/workflows/deploy-azure.yml`, pero requiere configurar sus secrets en GitHub. No se deben publicar contrasenas, tokens ni credenciales de Azure en el repositorio.

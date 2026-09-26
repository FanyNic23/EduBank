FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EduBank_SSR_NET.sln .
COPY EduBank.AppWeb/EduBank.AppWeb.csproj EduBank.AppWeb/
COPY EduBank.BLL/EduBank.BLL.csproj EduBank.BLL/
COPY EduBank.DAL/EduBank.DAL.csproj EduBank.DAL/
COPY EduBank.Models/EduBank.Models.csproj EduBank.Models/
RUN dotnet restore EduBank.AppWeb/EduBank.AppWeb.csproj

COPY . .
RUN dotnet publish EduBank.AppWeb/EduBank.AppWeb.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EduBank.AppWeb.dll"]

# ETAPA DE CONSTRUCCIÓN (BUILD)
# Usamos el SDK completo para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar primero los csproj y restaurar dependencias (Optimización de caché de Docker)

COPY ["Xana.API/Xana.API.csproj", "Xana.API/"]
COPY ["Xana.Application/Xana.Application.csproj", "Xana.Application/"]
COPY ["Xana.Domain/Xana.Domain.csproj", "Xana.Domain/"]
COPY ["Xana.Infrastructure/Xana.Infrastructure.csproj", "Xana.Infrastructure/"]

RUN dotnet restore "Xana.API/Xana.API.csproj"

# 2. Copiar el resto del código
COPY . .

# 3. Compilar y Publicar en modo Release
WORKDIR "/src/Xana.API"
RUN dotnet publish "Xana.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA FINAL (RUN)
# Usamos la imagen "Chiseled" (jammy-chiseled) para producción
# Es ultra ligera y segura (no tiene shell, ni apt, nada)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS final
WORKDIR /app

# Cloud Run escucha en el puerto 8080 por defecto
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Xana.API.dll"]
# Usa una imagen base de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el archivo del proyecto y el resto del código fuente
COPY ["ShoppingCartApi/ShoppingCartApi.csproj", "ShoppingCartApi/"]
COPY ["ShoppingCartApiSolution.sln", "."]
COPY . .

# Restaura las dependencias
WORKDIR /src/ShoppingCartApi
RUN dotnet restore "ShoppingCartApi.csproj"

# Construye la aplicación
RUN dotnet build "ShoppingCartApi.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "ShoppingCartApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Usa una imagen base de .NET ASP.NET para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Copia los archivos publicados
COPY --from=publish /app/publish .

# Define el punto de entrada
ENTRYPOINT ["dotnet", "ShoppingCartApi.dll"]

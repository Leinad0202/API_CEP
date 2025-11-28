# Usar a imagem oficial do .NET 8 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copiar csproj e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar tudo e publicar
COPY . ./
RUN dotnet publish -c Release -o /out

# Imagem runtime para rodar a API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Definir porta que o container vai expor
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

# Comando para rodar a API
ENTRYPOINT ["dotnet", "API_CEP.dll"]

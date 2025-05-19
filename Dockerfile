# Imagem base do ambiente de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8090

# Imagem para build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo da solução
COPY Contato.Cadastrar.Worker.sln .

# Copia os arquivos dos projetos
COPY Contato.Cadastrar.Worker.Service/Contato.Cadastrar.Worker.Service.csproj Contato.Cadastrar.Worker.Service/
COPY Contato.Cadastrar.Worker.Application/Contato.Cadastrar.Worker.Application.csproj Contato.Cadastrar.Worker.Application/
COPY Contato.Cadastrar.Worker.Domain/Contato.Cadastrar.Worker.Domain.csproj Contato.Cadastrar.Worker.Domain/
COPY Contato.Cadastrar.Worker.Infra/Contato.Cadastrar.Worker.Infra.csproj Contato.Cadastrar.Worker.Infra/

# Restaura os pacotes NuGet
RUN dotnet restore Contato.Cadastrar.Worker.sln

# Copia o restante dos arquivos
COPY . .

# Compila o projeto
WORKDIR /src/Contato.Cadastrar.Worker.Service
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publica a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final com o runtime e a aplicação publicada
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Contato.Cadastrar.Worker.Service.dll"]

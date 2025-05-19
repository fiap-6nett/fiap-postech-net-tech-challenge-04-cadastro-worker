# Base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8090
EXPOSE 8092

# Build image to compile the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file
COPY Contato.Cadastrar.Worker.sln .

# Copy the necessary project files
COPY Contato.Cadastrar.Worker.Service/Contato.Cadastrar.Worker.Service.csproj Contato.Cadastrar.Worker.Service/
COPY Contato.Cadastrar.Worker.Application/Contato.Cadastrar.Worker.Application.csproj Contato.Cadastrar.Worker.Application/
COPY Contato.Cadastrar.Worker.Domain/Contato.Cadastrar.Worker.Domain.csproj Contato.Cadastrar.Worker.Domain/
COPY Contato.Cadastrar.Worker.Infra/Contato.Cadastrar.Worker.Infra.csproj Contato.Cadastrar.Worker.Infra/

# Restore the NuGet packages
RUN dotnet restore Contato.Cadastrar.Worker.sln

# Copy the rest of the files
COPY . .

# Build the project
WORKDIR /src/Contato.Cadastrar.Worker.Service
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish the application for deployment
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image that includes the runtime and the compiled application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entrypoint for the Worker application (make sure this is correct)
ENTRYPOINT ["dotnet", "Contato.Cadastrar.Worker.Service.dll"]

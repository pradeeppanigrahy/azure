FROM microsoft/aspnetcore:2.0-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-sac2016 AS build
WORKDIR /src
COPY DockerExperiment/DockerExperiment.csproj DockerExperiment/
RUN dotnet restore DockerExperiment/DockerExperiment.csproj
COPY . .
WORKDIR /src/DockerExperiment
RUN dotnet build DockerExperiment.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish DockerExperiment.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY D:/Self/Azure/docker.txt C:/app/wwwroot
ENTRYPOINT ["dotnet", "DockerExperiment.dll"]

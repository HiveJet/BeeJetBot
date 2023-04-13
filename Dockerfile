#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# copy everything and restore
COPY ["BeeJet.Bot/BeeJet.Bot.csproj", "BeeJet.Bot/"]
RUN dotnet restore "BeeJet.Bot/BeeJet.Bot.csproj"

COPY ["BeeJet.Web/BeeJet.Web.csproj", "BeeJet.Web/"]
RUN dotnet restore "BeeJet.Web/BeeJet.Web.csproj"

COPY . .

# main build
WORKDIR "/src/BeeJet.Web"
RUN dotnet build "BeeJet.Web.csproj" -c Release -o /app/build

# publish the build
FROM build AS publish
RUN dotnet publish "BeeJet.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeeJet.Web.dll"]
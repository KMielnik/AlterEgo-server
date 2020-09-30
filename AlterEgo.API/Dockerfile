#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["AlterEgo.API/AlterEgo.API.csproj", "AlterEgo.API/"]
COPY ["AlterEgo.Infrastucture/AlterEgo.Infrastructure.csproj", "AlterEgo.Infrastucture/"]
COPY ["AlterEgo.Core/AlterEgo.Core.csproj", "AlterEgo.Core/"]
RUN dotnet restore "AlterEgo.API/AlterEgo.API.csproj"
COPY . .
WORKDIR "/src/AlterEgo.API"
RUN dotnet build "AlterEgo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlterEgo.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN DEBIAN_FRONTEND=noninteractive apt-get -qq update \
 && DEBIAN_FRONTEND=noninteractive apt-get -qqy install ffmpeg libgdiplus curl
RUN curl -fsSL https://get.docker.com -o get-docker.sh \
 && sh get-docker.sh

ENV ASPNETCORE_ENVIRONMENT="Production"
ENTRYPOINT ["dotnet", "AlterEgo.API.dll"]
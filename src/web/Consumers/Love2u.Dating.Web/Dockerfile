#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/web/Consumers/Love2u.Dating.Web/Love2u.Dating.Web.csproj", "src/web/Consumers/Love2u.Dating.Web/"]
RUN dotnet restore "src/web/Consumers/Love2u.Dating.Web/Love2u.Dating.Web.csproj"
COPY . .
WORKDIR "/src/src/web/Consumers/Love2u.Dating.Web"
RUN dotnet build "Love2u.Dating.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Love2u.Dating.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Love2u.Dating.Web.dll"]
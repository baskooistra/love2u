FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Profile/Love2u.Profiles.API/Love2u.Profiles.API.csproj", "src/Services/Profile/Love2u.Profiles.API/"]
RUN dotnet restore "src/Services/Profile/Love2u.Profiles.API/Love2u.Profiles.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Profile/Love2u.Profiles.API"
RUN dotnet build "Love2u.Profiles.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Love2u.Profiles.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Love2u.Profiles.API.dll"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Services/Love2u.IdentityProvider/Love2u.IdentityProvider.csproj", "src/Services/Love2u.IdentityProvider/"]
RUN dotnet restore "src/Services/Love2u.IdentityProvider/Love2u.IdentityProvider.csproj"
COPY . .
WORKDIR "/src/src/Services/Love2u.IdentityProvider"
RUN dotnet build "Love2u.IdentityProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Love2u.IdentityProvider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Love2u.IdentityProvider.dll"]

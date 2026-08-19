FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

COPY TechForClimate.csproj ./
RUN dotnet restore TechForClimate.csproj

COPY . ./
RUN dotnet publish TechForClimate.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TechForClimate.dll"]

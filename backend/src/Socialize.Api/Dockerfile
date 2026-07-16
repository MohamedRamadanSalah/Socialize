FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Socialize.sln ./
COPY src/Socialize.Domain/Socialize.Domain.csproj src/Socialize.Domain/
COPY src/Socialize.Application/Socialize.Application.csproj src/Socialize.Application/
COPY src/Socialize.Infrastructure/Socialize.Infrastructure.csproj src/Socialize.Infrastructure/
COPY src/Socialize.Api/Socialize.Api.csproj src/Socialize.Api/
COPY tests/Socialize.Application.Tests/Socialize.Application.Tests.csproj tests/Socialize.Application.Tests/
RUN dotnet restore src/Socialize.Api/Socialize.Api.csproj

COPY . .
RUN dotnet publish src/Socialize.Api/Socialize.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Socialize.Api.dll"]

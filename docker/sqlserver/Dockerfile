FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/ioBox.SqlServer.Demo/ioBox.SqlServer.Demo.csproj", "src/ioBox.SqlServer.Demo/"]
COPY ["src/ioBox/ioBox.csproj", "src/ioBox/"]
COPY ["src/ioBox/ioBox.csproj", "src/ioBox/"]
RUN dotnet restore "./src/ioBox.SqlServer.Demo/ioBox.SqlServer.Demo.csproj"
COPY . .
WORKDIR "/src/src/ioBox.SqlServer.Demo"
RUN dotnet build "./ioBox.SqlServer.Demo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ioBox.SqlServer.Demo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ioBox.SqlServer.Demo.dll"]

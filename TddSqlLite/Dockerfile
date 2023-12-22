FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TddSqlLite/TddSqlLite.csproj", "TddSqlLite/"]
RUN dotnet restore "TddSqlLite/TddSqlLite.csproj"
COPY . .
WORKDIR "/src/TddSqlLite"
RUN dotnet build "TddSqlLite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TddSqlLite.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TddSqlLite.dll"]

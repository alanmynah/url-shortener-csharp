FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./url-shortener-csharp.csproj", "url-shortener-csharp/"]
RUN dotnet restore "./url-shortener-csharp.csproj"
COPY . .
WORKDIR "/src/url-shortener-csharp"
RUN dotnet build "url-shortener-csharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "url-shortener-csharp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "url-shortener-csharp.dll"]
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CreatorHub.Web/CreatorHub.Web.csproj", "CreatorHub.Web/"]
COPY ["CreatorHub.Infrastructure/CreatorHub.Infrastructure.csproj", "CreatorHub.Infrastructure/"]
COPY ["CreatorHub.Application/CreatorHub.Application.csproj", "CreatorHub.Application/"]
COPY ["CreatorHub.Domain/CreatorHub.Domain.csproj", "CreatorHub.Domain/"]
RUN dotnet restore "CreatorHub.Web/CreatorHub.Web.csproj"

COPY . .
RUN dotnet publish "CreatorHub.Web/CreatorHub.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "CreatorHub.Web.dll"]
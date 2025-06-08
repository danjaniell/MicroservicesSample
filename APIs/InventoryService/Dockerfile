FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["APIs/InventoryService/InventoryService.csproj", "APIs/InventoryService/"]
RUN dotnet restore "APIs/InventoryService/InventoryService.csproj"

COPY . .
WORKDIR "/src/APIs/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
EXPOSE 5105
ENV ASPNETCORE_URLS=http://+:5105

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]

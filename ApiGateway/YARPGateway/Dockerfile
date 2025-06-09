FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ApiGateway/YARPGateway/YARPGateway.csproj", "ApiGateway/YARPGateway/"]
RUN dotnet restore "ApiGateway/YARPGateway/YARPGateway.csproj"

COPY . .
WORKDIR "/src/ApiGateway/YARPGateway"
RUN dotnet build "YARPGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YARPGateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
EXPOSE 5166
ENV ASPNETCORE_URLS=http://+:5166

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YARPGateway.dll"]

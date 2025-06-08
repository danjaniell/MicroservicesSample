FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ApiGateway/OcelotApiGateway/OcelotApiGateway.csproj", "ApiGateway/OcelotApiGateway/"]
RUN dotnet restore "ApiGateway/OcelotApiGateway/OcelotApiGateway.csproj"

COPY . .
WORKDIR "/src/ApiGateway/OcelotApiGateway"
RUN dotnet build "OcelotApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OcelotApiGateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
EXPOSE 5165
ENV ASPNETCORE_URLS=http://+:5165

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OcelotApiGateway.dll"]

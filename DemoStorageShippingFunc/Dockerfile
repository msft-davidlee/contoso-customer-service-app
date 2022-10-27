#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#See https://mcr.microsoft.com/v2/azure-functions/dotnet/tags/list
#See https://hub.docker.com/_/microsoft-azure-functions-dotnet
FROM mcr.microsoft.com/azure-functions/dotnet:4.14.0-dotnet6-appservice AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DemoStorageShippingFunc/DemoStorageShippingFunc.csproj", "DemoStorageShippingFunc/"]
COPY ["DemoCore/DemoCore.csproj", "DemoCore/"]
COPY ["DemoPartnerCore/DemoPartnerCore.csproj", "DemoPartnerCore/"]
RUN dotnet restore "DemoStorageShippingFunc/DemoStorageShippingFunc.csproj"
COPY . .
WORKDIR "/src/DemoStorageShippingFunc"
RUN dotnet build "DemoStorageShippingFunc.csproj" -c Release -o /app/build

FROM build AS publish
ARG version
RUN dotnet publish "DemoStorageShippingFunc.csproj" /p:Version=$version -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#See https://mcr.microsoft.com/v2/azure-functions/dotnet/tags/list
#See https://hub.docker.com/_/microsoft-azure-functions-dotnet
FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet6-appservice AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DemoServiceBusShippingFunc/DemoServiceBusShippingFunc.csproj", "DemoServiceBusShippingFunc/"]
COPY ["DemoPartnerCore/DemoPartnerCore.csproj", "DemoPartnerCore/"]
RUN dotnet restore "DemoServiceBusShippingFunc/DemoServiceBusShippingFunc.csproj"
COPY . .
WORKDIR "/src/DemoServiceBusShippingFunc"
RUN dotnet build "DemoServiceBusShippingFunc.csproj" -c Release -o /app/build

FROM build AS publish
ARG version
RUN dotnet publish "DemoServiceBusShippingFunc.csproj" /p:Version=$version -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
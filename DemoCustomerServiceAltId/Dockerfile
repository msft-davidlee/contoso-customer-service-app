#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DemoCustomerServiceAltId/DemoCustomerServiceAltId.csproj", "DemoCustomerServiceAltId/"]
RUN dotnet restore "DemoCustomerServiceAltId/DemoCustomerServiceAltId.csproj"
COPY . .
WORKDIR "/src/DemoCustomerServiceAltId"
RUN dotnet build "DemoCustomerServiceAltId.csproj" -c Release -o /app/build

FROM build AS publish
ARG version
RUN dotnet publish "DemoCustomerServiceAltId.csproj" /p:Version=$version -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoCustomerServiceAltId.dll"]
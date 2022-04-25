#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DemoPartnerAPI/DemoPartnerAPI.csproj", "DemoPartnerAPI/"]
RUN dotnet restore "DemoPartnerAPI/DemoPartnerAPI.csproj"
COPY . .
WORKDIR "/src/DemoPartnerAPI"
RUN dotnet build "DemoPartnerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoPartnerAPI.csproj" /p:Version=${version} -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoPartnerAPI.dll"]
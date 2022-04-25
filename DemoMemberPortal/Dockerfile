#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG APPVERSION
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DemoMemberPortal/DemoMemberPortal.csproj", "DemoMemberPortal/"]
RUN dotnet restore "DemoMemberPortal/DemoMemberPortal.csproj"
COPY . .
WORKDIR "/src/DemoMemberPortal"
RUN dotnet build "DemoMemberPortal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoMemberPortal.csproj" /p:Version=$APPVERSION -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoMemberPortal.dll"]
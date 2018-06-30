FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY SRMQInAction.csproj SRMQInAction/
RUN dotnet restore SRMQInAction/SRMQInAction.csproj
COPY . .
WORKDIR /src
RUN dotnet build SRMQInAction.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SRMQInAction.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ["dotnet", "SRMQInAction.dll"]

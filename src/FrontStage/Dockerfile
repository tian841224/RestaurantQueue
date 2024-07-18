FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# �Ĥ@���q�G�ϥ� SDK �M���i��c��
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# �ƻs .csproj �����٭�̿ඵ
COPY ["CommonLibrary/CommonLibrary.csproj", "CommonLibrary/"]
COPY ["FrontStage/FrontStage.csproj", "FrontStage/"]
RUN dotnet restore "FrontStage/FrontStage.csproj"
COPY . ./

WORKDIR "/src/FrontStage"
RUN dotnet build "FrontStage.csproj" -c Release -o /app/build

# �o��
FROM build AS publish
RUN dotnet publish "FrontStage.csproj" -c Release -o /app/publish /p:UseAppHost=false

# �̲׶��q
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontStage.dll"]
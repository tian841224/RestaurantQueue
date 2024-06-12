# 使用 .NET SDK 作為基礎映像進行建構
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製 .csproj 並還原依賴項
COPY *.csproj ./
RUN dotnet restore

# 複製剩餘的項目檔案並建構
COPY . ./
RUN dotnet publish -c Release -o out

# 使用 ASP.NET Core 運行時映像作為基礎映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# 複製建構產物到運行時環境
COPY --from=build /app/out .

# 設置環境變數為開發模式
ENV ASPNETCORE_ENVIRONMENT Development

# 開放應用程序運行所需的端口
EXPOSE 443
EXPOSE 80

# 設置啟動命令
ENTRYPOINT ["dotnet", "RestaurantQueue.dll"]

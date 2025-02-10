# Sử dụng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép file .csproj đúng thư mục
COPY DOCA.API/*.csproj ./DOCA.API/
WORKDIR /app/DOCA.API

# Khôi phục dependencies
RUN dotnet restore

# Quay lại thư mục gốc và sao chép toàn bộ source code
WORKDIR /app
COPY . ./

# Build ứng dụng
RUN dotnet publish -c Release -o /out

# Sử dụng .NET Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 7273
ENTRYPOINT ["dotnet", "DOCA.API.dll"]

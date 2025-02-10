# Sử dụng hình ảnh .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép file dự án và khôi phục các gói NuGet
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ source code vào container và tiến hành build
COPY . ./
RUN dotnet publish -c Release -o /out

# Sử dụng hình ảnh .NET Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Sao chép từ bước build sang container runtime
COPY --from=build /out ./

# Cấu hình cổng và lệnh chạy ứng dụng
EXPOSE 80
ENTRYPOINT ["dotnet", "YourProjectName.dll"]

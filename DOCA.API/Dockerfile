#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
#WORKDIR /app
#
## Copy toàn bộ solution vào container
#COPY . .
#
## Cài đặt dependency
#RUN dotnet restore
#
## Mở cổng 8080
#EXPOSE 8080
#
## Chạy ứng dụng với chế độ watch
#CMD ["dotnet", "watch", "run", "--urls=http://0.0.0.0:8080"]


# Sử dụng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy toàn bộ source code vào container
COPY . .  
RUN dotnet restore

# Build và publish ứng dụng
RUN dotnet publish DOCA.API/DOCA.API.csproj -c Release -o /out

# Sử dụng .NET Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "DOCA.API.dll"]

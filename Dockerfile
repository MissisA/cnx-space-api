# 1. ใช้ SDK ของ .NET 10.0 ในการ Build โค้ด
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /App

# ก๊อปปี้ไฟล์โปรเจกต์และ Restore dependencies
COPY . ./
RUN dotnet restore

# Build และ Publish แอปพลิเคชัน
RUN dotnet publish -c Release -o out

# 2. ใช้ Runtime 10.0 เพื่อสั่งรันโปรเจกต์บนคลาวด์ให้ประหยัดเนื้อที่
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /App
COPY --from=build-env /App/out .

# คำสั่งสั่งรันระบบหลังบ้าน
ENTRYPOINT ["dotnet", "CnxSpaceApi.dll"]
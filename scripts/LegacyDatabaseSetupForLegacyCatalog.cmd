@echo off
setlocal enabledelayedexpansion

REM Setup Database Script for CritterCommerce Legacy Catalog
REM Windows Command Prompt version

echo 🚀 Setting up Legacy Catalog Database...

REM Check if .NET CLI exists
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET CLI is required but not installed. Please install .NET 9 SDK.
    exit /b 1
)

REM Check if Docker Compose exists
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose is required but not installed.
    exit /b 1
)

REM Start SQL Server container
echo 🐳 Starting SQL Server container...
docker-compose --profile sqlserver up -d
if %errorlevel% neq 0 (
    echo ❌ Failed to start SQL Server container
    exit /b 1
)

echo ⏳ Waiting for SQL Server to be ready...
timeout /t 10 /nobreak >nul

REM Create migration
echo 📝 Creating database migration...
dotnet ef migrations add InitialCreate --project src/Legacy/Legacy.Catalog.Application --startup-project src/Legacy/Legacy.Catalog.Api
if %errorlevel% neq 0 (
    echo ❌ Failed to create migration
    exit /b 1
)

REM Apply migration
echo 🗄️ Applying database migration...
dotnet ef database update --project src/Legacy/Legacy.Catalog.Application --startup-project src/Legacy/Legacy.Catalog.Api
if %errorlevel% neq 0 (
    echo ❌ Failed to apply migration
    exit /b 1
)

echo ✅ Database setup completed successfully!
echo 📊 You can now run the Legacy Catalog API with: dotnet run --project src/Legacy/Legacy.Catalog.Api

endlocal

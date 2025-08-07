#!/usr/bin/env pwsh

# Setup Database Script for CritterCommerce Legacy Catalog
# Works on Windows, macOS, and Linux with PowerShell Core

param(
    [switch]$SkipDocker,
    [switch]$OnlyMigration,
    [string]$MigrationName = "InitialCreate"
)

Write-Host "🚀 Setting up Legacy Catalog Database..." -ForegroundColor Green

# Function to check if command exists
function Test-CommandExists {
    param($Command)
    $null = Get-Command $Command -ErrorAction SilentlyContinue
    return $?
}

# Check prerequisites
if (-not (Test-CommandExists "dotnet")) {
    Write-Error "❌ .NET CLI is required but not installed. Please install .NET 9 SDK."
    exit 1
}

if (-not $SkipDocker -and -not (Test-CommandExists "docker-compose")) {
    Write-Error "❌ Docker Compose is required but not installed. Use -SkipDocker if you have SQL Server running elsewhere."
    exit 1
}

# Start SQL Server container unless skipped
if (-not $SkipDocker -and -not $OnlyMigration) {
    Write-Host "🐳 Starting SQL Server container..." -ForegroundColor Yellow
    docker-compose --profile sqlserver up -d
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Failed to start SQL Server container"
        exit 1
    }
    
    Write-Host "⏳ Waiting for SQL Server to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
}

# Create and apply migration
Write-Host "📝 Creating database migration..." -ForegroundColor Yellow
$migrationCmd = "dotnet ef migrations add $MigrationName --project src/Legacy/Legacy.Catalog.Application --startup-project src/Legacy/Legacy.Catalog.Api"
Invoke-Expression $migrationCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to create migration"
    exit 1
}

Write-Host "🗄️  Applying database migration..." -ForegroundColor Yellow
$updateCmd = "dotnet ef database update --project src/Legacy/Legacy.Catalog.Application --startup-project src/Legacy/Legacy.Catalog.Api"
Invoke-Expression $updateCmd

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to apply migration"
    exit 1
}

Write-Host "✅ Database setup completed successfully!" -ForegroundColor Green
Write-Host "📊 You can now run the Legacy Catalog API with: dotnet run --project src/Legacy/Legacy.Catalog.Api" -ForegroundColor Cyan

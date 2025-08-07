#!/bin/bash

# Setup Database Script for CritterCommerce Legacy Catalog
# Works on macOS and Linux

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Default values
SKIP_DOCKER=false
ONLY_MIGRATION=false
MIGRATION_NAME="InitialCreate"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-docker)
            SKIP_DOCKER=true
            shift
            ;;
        --only-migration)
            ONLY_MIGRATION=true
            shift
            ;;
        --migration-name)
            MIGRATION_NAME="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  --skip-docker        Skip starting Docker containers"
            echo "  --only-migration     Only run migration commands"
            echo "  --migration-name     Name for the migration (default: InitialCreate)"
            echo "  -h, --help          Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option $1"
            exit 1
            ;;
    esac
done

echo -e "${GREEN}🚀 Setting up Legacy Catalog Database...${NC}"

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
if ! command_exists dotnet; then
    echo -e "${RED}❌ .NET CLI is required but not installed. Please install .NET 9 SDK.${NC}"
    exit 1
fi

if [ "$SKIP_DOCKER" = false ] && ! command_exists docker-compose; then
    echo -e "${RED}❌ Docker Compose is required but not installed. Use --skip-docker if you have SQL Server running elsewhere.${NC}"
    exit 1
fi

# Start SQL Server container unless skipped
if [ "$SKIP_DOCKER" = false ] && [ "$ONLY_MIGRATION" = false ]; then
    echo -e "${YELLOW}🐳 Starting SQL Server container...${NC}"
    docker-compose --profile sqlserver up -d

    echo -e "${YELLOW}⏳ Waiting for SQL Server to be ready...${NC}"
    sleep 10
fi

# Create and apply migration
echo -e "${YELLOW}📝 Creating database migration...${NC}"
dotnet ef migrations add "$MIGRATION_NAME" \
    --project ../src/Legacy/Legacy.Catalog.Application \
    --startup-project ../src/Legacy/Legacy.Catalog.Api

echo -e "${YELLOW}🗄️  Applying database migration...${NC}"
dotnet ef database update \
    --project ../src/Legacy/Legacy.Catalog.Application \
    --startup-project ../src/Legacy/Legacy.Catalog.Api

echo -e "${GREEN}✅ Database setup completed successfully!${NC}"
echo -e "${CYAN}📊 You can now run the Legacy Catalog API with: dotnet run --project src/Legacy/Legacy.Catalog.Api${NC}"

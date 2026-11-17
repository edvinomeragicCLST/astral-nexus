#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
docker compose up -d
dotnet restore src/AstralNexus.Web/AstralNexus.Web.csproj
# Configure .NET user secrets or export ConnectionStrings__DefaultConnection first.
# Compose's .env is not automatically loaded into this dotnet process.
dotnet run --project src/AstralNexus.Web/AstralNexus.Web.csproj

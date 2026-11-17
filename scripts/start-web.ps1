[CmdletBinding()]
param(
    [string] $ListenUrl = 'http://localhost:5092'
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../src/AstralNexus.Web/AstralNexus.Web.csproj'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'Install the .NET 10 SDK and reopen your terminal first. See README.md.'
}

# The SQL container must already be running. Prompting avoids putting the real
# password in a command line, source file or PowerShell command history.
$securePassword = Read-Host 'Current local SQL Server sa password (same as .env)' -AsSecureString
$passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
$previousConnection = $env:ConnectionStrings__DefaultConnection
try {
    $connection = New-Object System.Data.Common.DbConnectionStringBuilder
    $connection['Server'] = 'localhost,14333'
    $connection['Database'] = 'AstralNexusDb'
    $connection['User ID'] = 'sa'
    $connection['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $connection['TrustServerCertificate'] = 'True'
    $connection['Encrypt'] = 'False'
    $connection['MultipleActiveResultSets'] = 'True'
    $env:ConnectionStrings__DefaultConnection = $connection.ConnectionString

    & dotnet run --project $project --urls $ListenUrl
    if ($LASTEXITCODE -ne 0) { throw 'Web startup failed. Review the dotnet output above.' }
}
finally {
    $env:ConnectionStrings__DefaultConnection = $previousConnection
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)
    if ($null -ne $connection) { $connection.Clear() }
    $securePassword.Dispose()
}

param(
    [string]$a,
    [string]$lineCommand
)

# Carregar variáveis do .env, se existir
if (Test-Path ".env") {
    Get-Content ".env" | ForEach-Object {
        if ($_ -and $_ -notmatch '^[\s#]') {
            if ($_ -match '^(?<name>[^=]+)=(?<value>.*)$') {
                $name = $matches['name'].Trim()
                $value = $matches['value'].Trim(' ', '"', "'")
                Set-Item -Path "Env:$name" -Value $value
            }
        }
    }
}

$CONTAINER = $env:CONTAINER
$USER = $env:USER
$PASSWORD = $env:SA_PASSWORD
$DATABASE = $env:DATABASE

if (-not $CONTAINER) { $CONTAINER = "gestorvendas_sqlserver" }
if (-not $USER) { $USER = "sa" }
if (-not $PASSWORD) {
    Write-Host "Erro: a variável SA_PASSWORD não está definida no ambiente ou no .env." -ForegroundColor Red
    exit 1
}
if (-not $DATABASE) {
    Write-Host "Erro: a variável DATABASE não está definida no ambiente ou no .env." -ForegroundColor Red
    exit 1
}

$sqlcmd = "/opt/mssql-tools18/bin/sqlcmd"

# Executar arquivo SQL
if ($a) {

    Write-Host "Executando arquivo: $a"

    docker exec -it $CONTAINER `
        $sqlcmd `
        -S localhost `
        -U $USER `
        -P $PASSWORD `
        -C `
        -d $DATABASE `
        -i "/database/$a"
}

# Executar linha SQL
elseif ($lineCommand) {

    Write-Host "Executando comando SQL..."

    docker exec -it $CONTAINER `
        $sqlcmd `
        -S localhost `
        -U $USER `
        -P $PASSWORD `
        -C `
        -d $DATABASE `
        -Q $lineCommand
}

else {
    Write-Host "Uso:"
    Write-Host ".\db.ps1 -a migrations/001_initial_migration.sql"
    Write-Host ".\db.ps1 -lineCommand `"SELECT * FROM Users`""
}
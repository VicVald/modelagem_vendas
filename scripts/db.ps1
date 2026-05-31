param(
    [string]$a,
    [string]$lineCommand
)

$CONTAINER = "gestorvendas_sqlserver"
$USER = "sa"
$PASSWORD = "SenhaForte123@@"
$DATABASE = "MeuBanco"

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
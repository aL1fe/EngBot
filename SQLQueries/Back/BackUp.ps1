$serverName = "127.0.0.1,1455"
$databaseName = "Mnemonics"
$userName = "sa"
$password = "DZVW2cBqzfYAaHwAPhEg"
$sqlFile = ".\BackUp.sql"

Invoke-Expression "sqlcmd -S $serverName -d $databaseName -U $userName -P $password -i $sqlFile"

Write-Host "Press Enter to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")

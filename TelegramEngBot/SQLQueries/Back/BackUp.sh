/opt/mssql-tools/bin/sqlcmd -S "127.0.0.1,1455" -d "Mnemonics" -U sa -P "DZVW2cBqzfYAaHwAPhEg" -v CurrentDate="'$(date +%Y-%m-%d_%H-%M)'" -Q 'BACKUP DATABASE [Mnemonics] TO DISK = "/var/opt/mssql/data/Backup/$(CurrentDate)_Mnemonics.bak";'
# serverName="127.0.0.1,1455"
# databaseName="Mnemonics"
# userName="sa"
# password="DZVW2cBqzfYAaHwAPhEg"
# /opt/mssql-tools/bin/sqlcmd -S $serverName -d $databaseName -U $userName -P $password -v CurrentDate="'$(date +%Y-%m-%d_%H-%M)'" -Q 'BACKUP DATABASE [Mnemonics] TO DISK = "/var/opt/mssql/data/Backup/$(CurrentDate)_Mnemonics.bak";'
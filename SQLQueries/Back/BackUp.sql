DECLARE @BackupPath VARCHAR(500)
DECLARE @CurrentDate date;

SET @CurrentDate = CONVERT(date, GETDATE());
SET @BackupPath = '/var/opt/mssql/data/Backup/' + CONVERT(varchar(10), @CurrentDate, 120) + '_Mnemonics.bak'

BACKUP DATABASE [Mnemonics] TO DISK = @BackupPath

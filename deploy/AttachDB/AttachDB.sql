USE master;
GO

-- Создание логического имени и физического пути для базы данных
DECLARE @DBLogicalName NVARCHAR(128) = 'Mnemonics';
DECLARE @DBPhysicalName NVARCHAR(260) = '/var/opt/mssql/data/Mnemonics.mdf';
DECLARE @DBLogPhysicalName NVARCHAR(260) = '/var/opt/mssql/data/Mnemonics_log.ldf';

-- Определение имени файла базы данных и лог-файла
DECLARE @DBFileName NVARCHAR(260) = @DBPhysicalName;
DECLARE @DBLogFileName NVARCHAR(260) = @DBLogPhysicalName;

-- Определение имени базы данных
DECLARE @DBName NVARCHAR(128) = 'Mnemonics';

-- Сформировать динамический SQL
DECLARE @SQL NVARCHAR(MAX) = N'
CREATE DATABASE ' + QUOTENAME(@DBName) + '
ON (FILENAME = N''' + @DBFileName + '''),
   (FILENAME = N''' + @DBLogFileName + ''')
FOR ATTACH;
';

-- Выполнить динамический SQL
EXEC sp_executesql @SQL;
GO

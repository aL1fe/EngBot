INSERT INTO OPENROWSET('SQLNCLI', 'Server=127.0.0.1,1444;UID=sa;PWD=DZVW2cBqzfYAaHwAPhEg',[Mnemonics].[dbo].[__EFMigrationsHistory])
SELECT 
	*
FROM [Mnemonics].[dbo].[__EFMigrationsHistory]

INSERT INTO OPENROWSET('SQLNCLI', 'Server=127.0.0.1,1444;UID=sa;PWD=DZVW2cBqzfYAaHwAPhEg',[Mnemonics].[dbo].[CommonVocabulary])
SELECT 
	*
FROM [Mnemonics].[dbo].[CommonVocabulary]

INSERT INTO OPENROWSET('SQLNCLI', 'Server=127.0.0.1,1444;UID=sa;PWD=DZVW2cBqzfYAaHwAPhEg',[Mnemonics].[dbo].[UserList])
SELECT 
	*
FROM [Mnemonics].[dbo].[UserList]

INSERT INTO OPENROWSET('SQLNCLI', 'Server=127.0.0.1,1444;UID=sa;PWD=DZVW2cBqzfYAaHwAPhEg',[Mnemonics].[dbo].[UserSettings])
SELECT 
	*
FROM [Mnemonics].[dbo].[UserSettings]

INSERT INTO OPENROWSET('SQLNCLI', 'Server=127.0.0.1,1444;UID=sa;PWD=DZVW2cBqzfYAaHwAPhEg',[Mnemonics].[dbo].[UserVocabularyItem])
SELECT 
	*
FROM [Mnemonics].[dbo].[UserVocabularyItem]


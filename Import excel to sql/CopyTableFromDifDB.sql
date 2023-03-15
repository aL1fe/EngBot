insert into [Mnemonics].[dbo].[Dictionary](Id, EngWord, RusWord, UrlLink)
select 
	NEWID()
	,[Excel].[dbo].[Dict].[Eng]
	,[Excel].[dbo].[Dict].[Rus]
	,'non'
from [Excel].[dbo].[Dict]




use Mnemonics;
--begin tran
--	update Dictionary
--	set EngWord = TRIM(EngWord)
--	where EngWord like ' %' or EngWord like '% '
--rollback tran

SELECT 
	*
FROM Dictionary
where EngWord like '% '
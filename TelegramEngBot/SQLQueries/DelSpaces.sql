use Mnemonics;
--begin tran
--	update CommonVocabulary
--	set EngWord = TRIM(EngWord)
--	where EngWord like ' %' or EngWord like '% '
--rollback tran

SELECT 
	*
FROM CommonVocabulary
where EngWord like ' %' or EngWord like '% '
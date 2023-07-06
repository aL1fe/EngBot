USE Mnemonics;

BEGIN TRAN

	--update CommonVocabulary
	--SET EngWord = 'slipped one''s mind'--, RusWord = N'вылетело из головы'
	--WHERE EngWord like '%sb%';

	--update CommonVocabulary
	--SET EngWord = REPLACE(EngWord, 'sb', 'somebody')
	--WHERE EngWord like '% sb%';
	
	update CommonVocabulary
	SET RusWord = REPLACE(RusWord, N'опережать от графика', N'опережать график')
	WHERE RusWord = N'опережать от графика';

	SELECT
		*
	FROM CommonVocabulary
	--WHERE EngWord like '% somebody%';
	WHERE RusWord = N'опережать график';

ROLLBACK TRAN
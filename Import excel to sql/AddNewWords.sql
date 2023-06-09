USE Mnemonics;

BEGIN TRAN

	insert into CommonVocabulary(Id, EngWord, RusWord, UrlLink) values
	(NEWID(), 'pay off', N'окупиться', 'non')
	,(NEWID(), 'brush up', N'освежить (информацию)', 'non')
	,(NEWID(), 'come up with', N'придумать', 'non')
	,(NEWID(), 'eat out', N'есть вне дома', 'non')
	,(NEWID(), 'chip in', N'скинуться', 'non')
	,(NEWID(), 'hang in', N'держись!', 'non')
	,(NEWID(), 'redundant', N'избыточный, излижний', 'non')
	,(NEWID(), 'complication', N'осложнение', 'non')
	,(NEWID(), 'to subside', N'стихать, спадать', 'non')
	,(NEWID(), 'equanimity', N'невозмутимость', 'non')
	,(NEWID(), 'discrepancy', N'несоответствие', 'non')
	,(NEWID(), 'to adheres to sth', N'придерживаться чего-л.', 'non')
	,(NEWID(), 'shortcomings', N'недостатки', 'non')
	,(NEWID(), 'flaws', N'недостатки', 'non')
	,(NEWID(), 'What tipped you off?', N'Что подсказало вам?', 'non')
	,(NEWID(), 'betrayal', N'предательство', 'non')
	,(NEWID(), 'contagious', N'заразный', 'non')
	,(NEWID(), 'strain of flu', N'штамм гриппа', 'non')
	,(NEWID(), 'proceed to', N'перейти к', 'non')
	,(NEWID(), 'sour', N'кислый', 'non')
	,(NEWID(), 'sour cream', N'сметана', 'non')

ROLLBACK TRAN
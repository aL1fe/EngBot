USE Mnemonics;
SELECT
	UL.Id
	,UL.TelegramFirstName
	,UL.TelegramLastName
	,UL.TelegramUserName
	,UL.TelegramUserId
	,US.IsPronunciationOn
	,US.IsSmileOn
	,UL.LastArticleId
	,UL.LastActivity
	,UL.IsSynced
FROM UserList UL
join UserSettings US on US.Id = UL.UserSettingsId
order by UL.LastActivity desc
DELETE FROM UserVocabularyItem 
WHERE AppUserId IN (SELECT Id FROM UserList)

DELETE FROM UserList
DELETE FROM UserSettings
DELETE FROM UserVocabularyItem
DELETE FROM CommonVocabulary
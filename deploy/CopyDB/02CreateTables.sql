USE [Mnemonics]
GO

/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 6/28/2023 4:50:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CommonVocabulary](
	[Id] [uniqueidentifier] NOT NULL,
	[EngWord] [nvarchar](max) NULL,
	[RusWord] [nvarchar](max) NULL,
	[UrlLink] [nvarchar](max) NULL,
 CONSTRAINT [PK_CommonVocabulary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[UserList](
	[Id] [uniqueidentifier] NOT NULL,
	[TelegramUserId] [bigint] NOT NULL,
	[TelegramUserName] [nvarchar](max) NULL,
	[TelegramFirstName] [nvarchar](max) NULL,
	[TelegramLastName] [nvarchar](max) NULL,
	[UserSettingsId] [uniqueidentifier] NULL,
	[LastArticleId] [uniqueidentifier] NULL,
	[IsSynced] [bit] NOT NULL,
	[LastActivity] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserList] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsSynced]
GO

ALTER TABLE [dbo].[UserList] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [LastActivity]
GO

ALTER TABLE [dbo].[UserList]  WITH CHECK ADD  CONSTRAINT [FK_UserList_CommonVocabulary_LastArticleId] FOREIGN KEY([LastArticleId])
REFERENCES [dbo].[CommonVocabulary] ([Id])
GO

ALTER TABLE [dbo].[UserList] CHECK CONSTRAINT [FK_UserList_CommonVocabulary_LastArticleId]
GO

ALTER TABLE [dbo].[UserList]  WITH CHECK ADD  CONSTRAINT [FK_UserList_UserSettings_UserSettingsId] FOREIGN KEY([UserSettingsId])
REFERENCES [dbo].[UserSettings] ([Id])
GO

ALTER TABLE [dbo].[UserList] CHECK CONSTRAINT [FK_UserList_UserSettings_UserSettingsId]
GO

CREATE TABLE [dbo].[UserSettings](
	[Id] [uniqueidentifier] NOT NULL,
	[IsPronunciationOn] [bit] NOT NULL,
	[IsSmileOn] [bit] NOT NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[UserVocabularyItem](
	[Id] [uniqueidentifier] NOT NULL,
	[ArticleId] [uniqueidentifier] NULL,
	[Weight] [int] NOT NULL,
	[AppUserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_UserVocabularyItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserVocabularyItem]  WITH CHECK ADD  CONSTRAINT [FK_UserVocabularyItem_CommonVocabulary_ArticleId] FOREIGN KEY([ArticleId])
REFERENCES [dbo].[CommonVocabulary] ([Id])
GO

ALTER TABLE [dbo].[UserVocabularyItem] CHECK CONSTRAINT [FK_UserVocabularyItem_CommonVocabulary_ArticleId]
GO

ALTER TABLE [dbo].[UserVocabularyItem]  WITH CHECK ADD  CONSTRAINT [FK_UserVocabularyItem_UserList_AppUserId] FOREIGN KEY([AppUserId])
REFERENCES [dbo].[UserList] ([Id])
GO

ALTER TABLE [dbo].[UserVocabularyItem] CHECK CONSTRAINT [FK_UserVocabularyItem_UserList_AppUserId]
GO



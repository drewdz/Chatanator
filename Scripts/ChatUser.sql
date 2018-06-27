﻿CREATE TABLE ChatUser
(
	ChatUserId			BIGINT		IDENTITY(1, 1)			NOT NULL		CONSTRAINT pkChatUser PRIMARY KEY WITH FILLFACTOR = 80,
	FirstName			VARCHAR(100)						NOT NULL,
	LastName			VARCHAR(50)							NOT NULL,
	EmailAddress		VARCHAR(250)						NOT NULL		CONSTRAINT ukChatUserEmail UNIQUE
)
GO
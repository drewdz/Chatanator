﻿CREATE TABLE Connection
(
	ConnectionId		BIGINT		IDENTITY(1, 1)			NOT NULL		CONSTRAINT pkConnection PRIMARY KEY WITH FILLFACTOR = 80,
	ChatUserId			BIGINT								NOT NULL		CONSTRAINT fkConnectionChatUser FOREIGN KEY REFERENCES ChatUser(ChatUserId),
	OtherUserId			BIGINT								NOT NULL		CONSTRAINT fkConnectionChatUserOther FOREIGN KEY REFERENCES ChatUser(ChatUserId),
	CreateDate			DATETIME	DEFAULT(GETDATE())		NOT NULL,
	Active				BIT			DEFAULT(1)				NOT NULL
)
GO
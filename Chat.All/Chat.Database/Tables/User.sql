﻿CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FirstName] NVARCHAR(200) NOT NULL, 
    [LastName] NVARCHAR(200) NOT NULL, 
    [Email] NVARCHAR(200) NOT NULL UNIQUE, 
    [PasswordHash] NVARCHAR(200) NOT NULL, 
    [PasswordSalt] NVARCHAR(200) NOT NULL
)
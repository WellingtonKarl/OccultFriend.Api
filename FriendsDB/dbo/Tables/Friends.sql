﻿CREATE TABLE [dbo].[Friends] (
    [Id]          INT     Primary key      IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Password]    VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (MAX) NOT NULL,
    [Email]       VARCHAR (50)  NULL,
    [Data]        SMALLDATETIME NOT NULL,
    [ImagePath]   VARCHAR (2000) NULL,
    [IsChildreen] BIT NOT NULL
);


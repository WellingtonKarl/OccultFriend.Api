CREATE TABLE [dbo].[Friends] (
    [Id]          INT     Primary key      IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Password]    VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (MAX) NOT NULL,
    [Email]       VARCHAR (50)  NULL,
    [IsChildreen] BIT NOT NULL,
    [PathImage]   VARCHAR(2000) NULL
);


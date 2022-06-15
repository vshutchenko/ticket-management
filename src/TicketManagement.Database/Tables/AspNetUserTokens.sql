CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId]        NVARCHAR (450) NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [Name]          NVARCHAR (128) NOT NULL,
    [Value]         NVARCHAR (MAX) NULL
);
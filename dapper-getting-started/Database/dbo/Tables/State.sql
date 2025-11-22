CREATE TABLE [dbo].[State] (
    [ID]            INT           NOT NULL,
    [Name]          VARCHAR (50)  NOT NULL,
    [Abbreviation]  VARCHAR(2)    NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([ID] ASC)
);

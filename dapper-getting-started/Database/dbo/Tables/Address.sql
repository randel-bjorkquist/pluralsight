CREATE TABLE [dbo].[Address] (
    [ID]            INT          IDENTITY (1, 1) NOT NULL,
    [ContactID]     INT          NOT NULL,
    [AddressType]   VARCHAR (10) NOT NULL,
    [StreetAddress] VARCHAR (50) NOT NULL,
    [City]          VARCHAR (50) NOT NULL,
    [StateID]       INT          NOT NULL,
    [PostalCode]    VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Addresses_Contact] FOREIGN KEY ([ContactID]) REFERENCES [dbo].[Contact] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Addresses_State] FOREIGN KEY ([StateID]) REFERENCES [dbo].[State] ([ID])
);

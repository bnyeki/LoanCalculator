CREATE TABLE [dbo].[LoanSearchParameter] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [LoanAmmount]     FLOAT (53)     NOT NULL,
    [Term]            INT            NOT NULL,
    [Interest]        FLOAT (53)     NOT NULL,
    [InterestPeriod]  INT            NOT NULL,
    [CalculationTime] DATETIME       NOT NULL,
    [UserId]          NVARCHAR (128) NULL,
    CONSTRAINT [PK_Loans] PRIMARY KEY CLUSTERED ([Id] ASC)
);


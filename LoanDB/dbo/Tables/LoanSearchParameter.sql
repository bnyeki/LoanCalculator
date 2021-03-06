
CREATE TABLE [dbo].[LoanSearchParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LoanAmmount] [float] NOT NULL,
	[Term] [int] NOT NULL,
	[InterestPeriodId] [int] NOT NULL,
	[CalculationTime] [datetime] NOT NULL,
	[UserId] [nvarchar](128) NULL,
	[InterestFirstPeriod] [float] NOT NULL,
	[InterestSecondPeriod] [float] NULL,
	[InterestThirdPeriod] [float] NULL,
	[TermFirstPeriod] [int] NOT NULL,
	[TermSecondPeriod] [int] NULL,
	[TermThirdPeriod] [int] NULL,
 CONSTRAINT [PK_Loans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LoanSearchParameter]  WITH CHECK ADD  CONSTRAINT [FK_LoanSearchParameter_Period] FOREIGN KEY([InterestPeriodId])
REFERENCES [dbo].[Period] ([Id])
GO
ALTER TABLE [dbo].[LoanSearchParameter] CHECK CONSTRAINT [FK_LoanSearchParameter_Period]
GO

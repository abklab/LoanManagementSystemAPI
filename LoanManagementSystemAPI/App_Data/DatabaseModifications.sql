--Create Transactions Log Table
CREATE TABLE [dbo].[MOMO_RESPONSE_LOG](
	[EntryID] [int] IDENTITY(1,1) NOT NULL,
	[TYPE] [varchar](50) NULL,
	[TXNID] [varchar](50) NULL,
	[AMOUNT] [decimal](18, 0) NULL,
	[MSISDN] [varchar](50) NULL,
	[MNO] [varchar](50) NULL,
	[RESULT] [nchar](10) NOT NULL,
	[ERRORCODE] [varchar](20) NULL,
	[ERRORDESCRIPTION] [varchar](50) NULL,
	[FLAG] [nchar](10) NULL,
	[CONTENT] [varchar](100) NULL,
	[LastUpdated] [datetime] NULL,
 CONSTRAINT [PK_MOMO_RESPONSE_LOG] PRIMARY KEY CLUSTERED 
(
	[EntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--Procedure to log Transactions
create proc [dbo].[usp_SaveTransactionLog]
			@TYPE [varchar](50),
			@TXNID [varchar](50),	
			@MSISDN [varchar](50),
			@MNO [varchar](10)='MNO',
			@RESULT [varchar](50),
			@ERRORCODE [varchar](20),
			@ERRORDESCRIPTION [varchar](100),
			@FLAG[Nchar](10),
			@CONTENT [varchar](100),
			@AMOUNT decimal =0
		   as
	INSERT INTO
			 [dbo].[MOMO_RESPONSE_LOG]          
             ([TYPE], TXNID, MSISDN, MNO, RESULT, ERRORCODE, FLAG, ERRORDESCRIPTION,[CONTENT], AMOUNT,LastUpdated)
	VALUES   (@TYPE, @TXNID,@MSISDN,@MNO,@RESULT,@ERRORCODE,@FLAG,@ERRORDESCRIPTION,@CONTENT, @AMOUNT,GETDATE())


	
	
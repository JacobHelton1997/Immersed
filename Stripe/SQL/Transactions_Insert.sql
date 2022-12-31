-- =============================================
-- Author: Jacob Helton
-- Create date: 12/22/2022
-- Description: Inserts a transaction
-- Code Reviewer: Liliana Meriweather

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Transactions_Insert] @PaymentTypeId int
								   ,@ExternalTransactionId nvarchar(255)
								   ,@ExternalUserId nvarchar(255)
								   ,@AmountCharged decimal(18,2)
								   ,@CreatedBy int
								   ,@InvoiceId nvarchar(50)
								   ,@Id int OUTPUT
as

/* ----- TEST CODE -----

	Declare @PaymentTypeId int = 1
		   ,@ExternalTransactionId nvarchar(255) = 'testing123'
		   ,@ExternalUserId nvarchar(255) = 'testing123'
		   ,@AmountCharged decimal(18,2) = 12.5
		   ,@InvoiceId nvarchar(50) = 'testing123'
		   ,@CreatedBy int = 1
		   ,@Id int = 0

	Execute dbo.Transactions_Insert @PaymentTypeId
								   ,@ExternalTransactionId
								   ,@ExternalUserId
								   ,@AmountCharged
								   ,@CreatedBy
								   ,@InvoiceId
								   ,@Id OUTPUT

	SELECT *
	FROM dbo.Transactions

*/

BEGIN

	INSERT INTO dbo.Transactions
					([PaymentTypeId]
					,[ExternalTransactionId]
					,[ExternalUserId]
					,[AmountCharged]
					,[CreatedBy]
					,[InvoiceId])

	VALUES			(@PaymentTypeId
					,@ExternalTransactionId
					,@ExternalUserId
					,@AmountCharged
					,@CreatedBy
					,@InvoiceId)

	SET @Id = SCOPE_IDENTITY();

END

-- =============================================
-- Author: Jacob Helton
-- Create date: 12/20/2022
-- Description: Inserts a subscription into dbo.Subscriptions and into the bridge table StripeSubscriptionProduct
-- Code Reviewer: Darryl Yeargin

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================
ALTER proc [dbo].[Subscriptions_Insert] 
							@SubscriptionId nvarchar(50)
						   ,@CustomerId nvarchar(50)
						   ,@DateEnded datetime2(7)
						   ,@isActive bit
						   ,@CreatedBy int
						   ,@InvoiceId nvarchar(50)
						   ,@ProductId nvarchar(50)
						   ,@Id int OUTPUT

as

/*-----TEST CODE-----

	Declare @SubscriptionId nvarchar(50) = 'testingId123456141'
		   ,@CustomerId nvarchar(50) = 'testingCustId'
		   ,@DateEnded datetime2(7) = '2023-01-21 05:00:00'
		   ,@isActive bit = 1
		   ,@CreatedBy int = 1
		   ,@InvoiceId nvarchar(50) = 'TestingInvoiceId123'
		   ,@ProductId nvarchar(50) = 'prod_N0rUWg2Tmm3mWt'
		   ,@Id int = 0

	Execute dbo.Subscriptions_Insert  @SubscriptionId
								     ,@CustomerId
									 ,@DateEnded
									 ,@isActive
								     ,@CreatedBy
								     ,@InvoiceId
									 ,@ProductId
								     ,@Id OUTPUT
	SELECT *
	FROM dbo.Subscriptions

	SELECT * 
	FROM dbo.StripeSubscriptionProduct

*/

BEGIN

	Declare @ProductIdInt int = (Select sp.Id
					FROM dbo.StripeProducts as sp
					Where @ProductId = sp.ProductId)


	INSERT INTO [dbo].[Subscriptions]
			([SubscriptionId]
			,[CustomerId]
			,[DateEnded]
			,[isActive]
			,[CreatedBy]
			,[InvoiceId])
     VALUES
			(@SubscriptionId 
			,@CustomerId
			,@DateEnded
			,@isActive
			,@CreatedBy 
			,@InvoiceId) 

	SET @Id = SCOPE_IDENTITY()

	INSERT INTO [dbo].[StripeSubscriptionProduct]
				([ProductId]
				,[SubscriptionId])
	VALUES
			(@ProductIdInt
			,@Id)

END

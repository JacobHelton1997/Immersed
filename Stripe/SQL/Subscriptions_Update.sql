-- =============================================
-- Author: Jacob Helton
-- Create date: 12/26/2022
-- Description: Updates a subscription in dbo.Subscriptions and dbo.StripeProductSubscription
-- Code Reviewer: Darryl Yeargin

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Subscriptions_Update] @InvoiceId nvarchar(50)
									   ,@SubscriptionId nvarchar(50)
									   ,@ProductId nvarchar(50)

as

BEGIN

	Declare @DateNow datetime2 = GETUTCDATE();

	UPDATE dbo.Subscriptions
	SET	[InvoiceId] = @InvoiceId
	   ,[DateModified] = @DateNow
	WHERE [SubscriptionId] = @SubscriptionId

	UPDATE dbo.StripeSubscriptionProduct
	SET [ProductId] = (SELECT Id
					   FROM dbo.StripeProducts
					   Where [ProductId] = @ProductId)
	WHERE [SubscriptionId] = (Select Id
							  FROM dbo.Subscriptions
							  WHERE [SubscriptionId] = @SubscriptionId)

END

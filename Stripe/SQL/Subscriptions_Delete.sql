-- =============================================
-- Author: Jacob Helton
-- Create date: 12/26/2022
-- Description: Deletes a subscription by the user ID that created it and the subscriptionId.
-- Code Reviewer: Darryl Yeargin

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Subscriptions_Delete] @SubscriptionId nvarchar(50)
								                       ,@UserId int

as

BEGIN

	Declare @SubscriptionIdInt int = (Select s.Id
					FROM dbo.StripeSubscriptionProduct as ssp inner join dbo.Subscriptions as s
						on ssp.SubscriptionId = s.Id
					Where @SubscriptionId = s.SubscriptionId)

	DELETE FROM dbo.StripeSubscriptionProduct
	WHERE SubscriptionId = @SubscriptionIdInt

	DELETE FROM dbo.Subscriptions
	WHERE SubscriptionId = @SubscriptionId and CreatedBy = @UserId

END

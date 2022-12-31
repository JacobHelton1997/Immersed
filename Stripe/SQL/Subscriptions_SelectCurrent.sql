-- =============================================
-- Author: Jacob Helton
-- Create date: 12/26/2022
-- Description: Selects a subscription by the user ID that created it.
-- Code Reviewer: Darryl Yeargin

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Subscriptions_SelectCurrent] @CreatedById int

as

/* ----- TEST CODE -----

	Declare @CreatedById int = 198

	Execute dbo.Subscriptions_SelectCurrent @CreatedById

*/

BEGIN

	SELECT s.[Id]
          ,s.[SubscriptionId]
          ,s.[CustomerId]
          ,s.[DateEnded]
          ,s.[isActive]
          ,s.[CreatedBy]
		  ,s.[InvoiceId]
		  ,sp.[PriceId]
	FROM [dbo].[Subscriptions] s inner join [dbo].[StripeSubscriptionProduct] ssp
		on s.Id = ssp.SubscriptionId
		inner join [dbo].[StripeProducts] sp
			on ssp.ProductId = sp.Id
	WHERE @CreatedById = [CreatedBy] and [isActive] = 1

END

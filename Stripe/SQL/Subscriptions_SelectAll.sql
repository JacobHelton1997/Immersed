-- =============================================
-- Author: Jacob Helton
-- Create date: 12/20/2022
-- Description: Paginated select for all Subscriptions
-- Code Reviewer: Samantha Emerson

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Subscriptions_SelectAll] @PageIndex int
									   ,@PageSize int

as

/* ----- TEST CODE -----

	Declare @PageIndex int = 0
		   ,@PageSize int = 10

	Execute dbo.Subscriptions_SelectAll @PageIndex
									   ,@PageSize

*/

BEGIN

	Declare @offset int = @PageIndex * @PageSize

	SELECT s.Id
	      ,s.SubscriptionId
          ,s.CustomerId
		  ,s.DateEnded
		  ,s.isActive
		  ,u.Id as CreatedBy
		  ,s.DateCreated
		  ,s.DateModified
		  ,s.InvoiceId
		  ,TotalCount = Count(1) OVER()
    FROM [dbo].[Subscriptions] as s inner join dbo.Users as u
		on s.CreatedBy = u.Id
								  
	Order by s.Id
	OFFSET @offSet Rows
	Fetch Next @PageSize Rows ONLY

END

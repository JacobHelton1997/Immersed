-- =============================================
-- Author: Jacob Helton
-- Create date: 12/27/2022
-- Description: Selects a paginated list of transactions by user id that created it
-- Code Reviewer: Liliana Meriweather

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Transactions_SelectByCreatedBy]  @PageIndex int
											  ,@PageSize int
											  ,@UserId int

as

/* ----- TEST CODE -----

	Execute dbo.Transactions_SelectByCreatedBy 0, 3, 5

*/

BEGIN

	DECLARE @Offset int = @PageIndex * @PageSize

	SELECT t.[Id]
		  ,t.[PaymentTypeId]
		  ,t.[ExternalTransactionId]
		  ,t.[ExternalUserId]
		  ,t.[AmountCharged]
		  ,t.[CreatedBy]
		  ,t.[DateCreated]
		  ,t.[InvoiceId]
		  ,TotalCount = COUNT(1) OVER()
	FROM dbo.Transactions t inner join dbo.Users u
		on u.Id = t.CreatedBy
	WHERE @UserId = u.[Id]

	ORDER BY t.Id

	OFFSET @offset ROWS
	FETCH NEXT @PageSize ROWS ONLY

END

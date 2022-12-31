-- =============================================
-- Author: Jacob Helton
-- Create date: 12/27/2022
-- Description: Selects a transaction by id
-- Code Reviewer: Liliana Meriweather

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[Transactions_SelectById] @Id int

as

/* ----- TEST CODE -----

	Execute dbo.Transactions_SelectById 15

*/

BEGIN

	SELECT t.[Id]
		  ,t.[PaymentTypeId]
		  ,t.[ExternalTransactionId]
		  ,t.[ExternalUserId]
		  ,t.[AmountCharged]
		  ,t.[CreatedBy]
		  ,t.[DateCreated]
		  ,t.[InvoiceId]
	FROM dbo.Transactions t inner join dbo.Users u
		on u.Id = t.CreatedBy
	WHERE @Id = t.[Id]

END

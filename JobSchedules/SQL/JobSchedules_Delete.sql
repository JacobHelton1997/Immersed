-- =============================================
-- Author: <Jacob Helton>
-- Create date: <11/23/2022>
-- Description:	<Soft Deletion of a JobSchedule by their Id. Will set IsActive to 0, IsDeleted to 1, and update the DateModified>
-- Code Reviewer: Joe Medina

-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================

ALTER proc [dbo].[JobSchedules_Delete]
						@Id int
					   ,@ModifiedBy int

as

/*--------- TEST CODE -----------------

	Declare @Id int = 12
		   ,@ModifiedBy int = 125
	
	SELECT *
	FROM [dbo].[JobSchedules]
	WHERE Id = @Id

	Execute [dbo].[JobSchedules_Delete] @Id
									   ,@ModifiedBy

	SELECT *
	FROM [dbo].[JobSchedules]
	WHERE Id = @Id

*/

BEGIN

	DECLARE @dateNow datetime2(7) = getutcdate();

	UPDATE [dbo].[JobSchedules]

	SET [DateModified] = @dateNow
	   ,[IsActive] = 0
	   ,[IsDeleted] = 1
	   ,[ModifiedBy] = @ModifiedBy
	WHERE Id = @Id

END

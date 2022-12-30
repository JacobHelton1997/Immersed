-- =============================================
-- Author: <Jacob Helton>
-- Create date: <11/23/2022>
-- Description:	<Inserts into the JobSchedules table a new JobSchedule>
-- Code Reviewer: Joe Medina

-- MODIFIED BY: <Jacob Helton>
-- MODIFIED DATE: <12/2/2022>
-- Code Reviewer:
-- Note: <Updating proc for Quartz JobController>
-- =============================================

ALTER proc [dbo].[JobSchedules_Insert]
			   @Id int OUTPUT
			  ,@ChronJobTypeId int
			  ,@UtcHourToRun int
			  ,@IntervalTypeId int
			  ,@DaysOfWeekId int
			  ,@EntityTypeId int
			  ,@RecipientId int
			  ,@Recipient nvarchar(255)
			  ,@CreatedBy int
			  ,@ModifiedBy int
			  
AS

/*--------- TEST CODE --------------

	Declare @Id int = 0
		   ,@ChronJobTypeId int = 2
		   ,@UtcHourToRun int = 22
		   ,@IntervalTypeId int = 1
		   ,@DaysOfWeekId int = 5
		   ,@EntityTypeId int = 1
		   ,@RecipientId int = 28
		   ,@Recipient nvarchar(255) = 'jacobTest@dispostable.com'
		   ,@CreatedBy int = 125
		   ,@ModifiedBy int = 125

	Execute [dbo].[JobSchedules_Insert]
									@Id OUTPUT
								   ,@ChronJobTypeId
								   ,@UtcHourToRun
								   ,@IntervalTypeId
								   ,@DaysOfWeekId
								   ,@EntityTypeId
								   ,@RecipientId
								   ,@Recipient
								   ,@CreatedBy
								   ,@ModifiedBy

	SELECT *
	FROM [dbo].[JobSchedules]
	WHERE Id = @Id

*/

BEGIN
	
	INSERT INTO [dbo].[JobSchedules]
						([ChronJobTypeId]
						,[UtcHourToRun]
						,[IntervalTypeId]
						,[DaysOfWeekId]
						,[EntityTypeId]
						,[RecipientId]
						,[Recipient]
						,[CreatedBy]
						,[ModifiedBy])
	VALUES 
			(@ChronJobTypeId
			  ,@UtcHourToRun
			  ,@IntervalTypeId
			  ,@DaysOfWeekId
			  ,@EntityTypeId
			  ,@RecipientId
			  ,@Recipient
			  ,@CreatedBy
			  ,@ModifiedBy)
	
	SET @Id = SCOPE_IDENTITY()

END


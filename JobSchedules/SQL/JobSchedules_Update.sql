-- =============================================
-- Author: <Jacob Helton>
-- Create date: <11/23/2022>
-- Description:	<Updates a JobSchedule by its Id>
-- Code Reviewer: Joe Medina

-- MODIFIED BY: <Jacob Helton>
-- MODIFIED DATE: <12/2/2022>
-- Code Reviewer: Joe Medina
-- Note:  <Updating proc for Quartz JobController>
-- =============================================

ALTER proc [dbo].[JobSchedules_Update]
			   @Id int
			  ,@ChronJobTypeId int
			  ,@IsRecurring bit
			  ,@UtcHourToRun int
			  ,@IntervalTypeId int
			  ,@DaysOfWeekId int
			  ,@EntityTypeId int
			  ,@RecipientId int
			  ,@Recipient nvarchar(255)
			  ,@IsActive bit
			  ,@ModifiedBy int

AS

/*--------- TEST CODE --------------

	Declare @Id int = 12
		   ,@ChronJobTypeId int = 1
		   ,@IsRecurring bit = 1
		   ,@UtcHourToRun int = 12
		   ,@IntervalTypeId int = 1
		   ,@DaysOfWeekId int = 7
		   ,@EntityTypeId int = 1
		   ,@RecipientId int = 28
		   ,@Recipient nvarchar(255) = 'testimmersed@dispostable.com'
		   ,@IsActive bit = 1
		   ,@ModifiedBy int = 125

	SELECT *
	FROM [dbo].[JobSchedules]
	Where Id = @Id

	Execute [dbo].[JobSchedules_Update] @Id
									   ,@ChronJobTypeId
									   ,@IsRecurring
									   ,@UtcHourToRun
									   ,@IntervalTypeId
									   ,@DaysOfWeekId
									   ,@EntityTypeId
									   ,@RecipientId
									   ,@Recipient
									   ,@IsActive
									   ,@ModifiedBy

	SELECT *
	FROM [dbo].[JobSchedules]
	Where Id = @Id

*/

BEGIN

	Declare @dateNow datetime2(7) = getutcdate();

	UPDATE [dbo].[JobSchedules]

	SET
		[ChronJobTypeId] = @ChronJobTypeId
	   ,[IsRecurring] = @IsRecurring
	   ,[UtcHourToRun] = @UtcHourToRun
	   ,[IntervalTypeId] = @IntervalTypeId
	   ,[DaysOfWeekId] = @DaysOfWeekId
	   ,[EntityTypeId] = @EntityTypeId
	   ,[RecipientId] = @RecipientId
	   ,[Recipient] = @Recipient
	   ,[IsActive] = @IsActive
	   ,[DateModified] = @dateNow
	   ,[ModifiedBy] = @ModifiedBy

	WHERE (
			Id = @Id
			)

END

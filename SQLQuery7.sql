USE [AttendanceSystem]
GO

SELECT [Id]
      ,[ActionId]
      ,[ActionName]
      ,[ActionDate]
      ,[IsFirst]
  FROM [dbo].[ActionsLogs]
GO

SELECT [Id]
      ,[Date]
      ,[FK_StatusId]
      ,[TotalHour]
      ,[StartAt]
      ,[EndAt]
      ,[IsActive]
  FROM [dbo].[WorkDays]
GO

-----------------------------------------

--DELETE FROM [dbo].[ActionsLogs]
--GO

--DELETE FROM [dbo].[WorkDays]
--GO


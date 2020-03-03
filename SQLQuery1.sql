USE [AttendanceSystem]
GO

/****** Object:  Table [dbo].[ActionsLogs]    Script Date: 2/27/2020 8:25:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActionsLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FK_ActionId] [int] NOT NULL,
	[ActionDate] [datetime] NOT NULL,
	[IsFirst] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ActionsLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


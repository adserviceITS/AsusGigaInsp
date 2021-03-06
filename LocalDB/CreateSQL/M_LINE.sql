USE [C:\PROJECTS\ASUSGIGAINSP\LOCALDB\ASUSGIGAINSP.MDF]
GO

/****** Object:  Table [dbo].[M_LINE]    Script Date: 2020/06/23 9:49:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[M_LINE](
	[LINE_ID] [nvarchar](50) NOT NULL,
	[LINE_NAME] [nvarchar](max) NULL,
	[INSERT_DATE] [datetime] NULL,
	[INSERT_ID] [nvarchar](10) NULL,
	[UPDATE_DATE] [datetime] NULL,
	[UPDATE_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[LINE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


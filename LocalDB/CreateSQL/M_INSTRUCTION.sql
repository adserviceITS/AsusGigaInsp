USE [C:\PROJECTS\ASUSGIGAINSP\LOCALDB\ASUSGIGAINSP.MDF]
GO

/****** Object:  Table [dbo].[M_INSTRUCTION]    Script Date: 2020/06/23 9:48:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[M_INSTRUCTION](
	[INSTRUCTION_ID] [nvarchar](3) NOT NULL,
	[INSTRUCTION] [nvarchar](50) NULL,
	[POS_ID] [int] NULL,
	[DEL_FLG] [nchar](1) NULL,
	[INSERT_DATE] [datetime] NULL,
	[INSERT_ID] [nvarchar](10) NULL,
	[UPDATE_DATE] [datetime] NULL,
	[UPDATE_ID] [nvarchar](10) NULL,
 CONSTRAINT [PK_M_INSTRUCTION] PRIMARY KEY CLUSTERED 
(
	[INSTRUCTION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[M_INSTRUCTION] ADD  CONSTRAINT [DF_M_INSTRUCTION_POS_ID]  DEFAULT ((0)) FOR [POS_ID]
GO

ALTER TABLE [dbo].[M_INSTRUCTION] ADD  CONSTRAINT [DF_M_INSTRUCTION_DEL_FLG]  DEFAULT ('0') FOR [DEL_FLG]
GO

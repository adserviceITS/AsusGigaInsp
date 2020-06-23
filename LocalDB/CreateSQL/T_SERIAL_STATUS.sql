USE [C:\PROJECTS\ASUSGIGAINSP\LOCALDB\ASUSGIGAINSP.MDF]
GO

/****** Object:  Table [dbo].[T_SERIAL_STATUS]    Script Date: 2020/06/23 9:52:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_SERIAL_STATUS](
	[ID] [bigint] NOT NULL,
	[SERIAL_NUMBER] [nvarchar](15) NULL,
	[ASUS_PART_NO] [nvarchar](max) NULL,
	[DESCRIPTION] [nvarchar](max) NULL,
	[SPECIFICATION] [nvarchar](max) NULL,
	[PALLET] [nvarchar](max) NULL,
	[CARTON] [nvarchar](max) NULL,
	[MAC_1] [nvarchar](12) NULL,
	[MAC_2] [nvarchar](12) NULL,
	[MODEL_NAME] [nvarchar](max) NULL,
	[CUSTOMER_MODEL_NAME] [nvarchar](max) NULL,
	[NW] [nvarchar](max) NULL,
	[GW] [nvarchar](max) NULL,
	[EAN_CODE] [nvarchar](max) NULL,
	[UPC_CODE] [nvarchar](max) NULL,
	[BIOS] [nvarchar](max) NULL,
	[IMEI] [nvarchar](max) NULL,
	[IMEI2] [nvarchar](max) NULL,
	[LOCK_CODE] [nvarchar](max) NULL,
	[SHIPPING_DATE] [nvarchar](max) NULL,
	[SO] [nvarchar](max) NULL,
	[SO_LINE] [nvarchar](max) NULL,
	[OSVER] [nvarchar](max) NULL,
	[MODEMVER] [nvarchar](max) NULL,
	[FWVER] [nvarchar](max) NULL,
	[GPSVER] [nvarchar](max) NULL,
	[MAPVER] [nvarchar](max) NULL,
	[MB_MAC] [nvarchar](max) NULL,
	[BCAS_CARD] [nvarchar](max) NULL,
	[H3GBARCODE] [nvarchar](max) NULL,
	[COM_SN1] [nvarchar](max) NULL,
	[COM_SN2] [nvarchar](max) NULL,
	[COM_SN3] [nvarchar](max) NULL,
	[COM_MAC1] [nvarchar](max) NULL,
	[COM_MAC2] [nvarchar](max) NULL,
	[COM_MAC3] [nvarchar](max) NULL,
	[STATUS] [nvarchar](4) NULL,
	[STATUS_UPDATE_DATE] [nvarchar](max) NULL,
	[SO_NO] [nvarchar](max) NULL,
	[WORKDAY] [nvarchar](max) NULL,
	[INSTRUCTION] [nvarchar](max) NULL,
	[REMARK] [nvarchar](max) NULL,
	[INSERT_DATE] [datetime] NULL,
	[INSERT_ID] [nvarchar](10) NULL,
	[UPDATE_DATE] [datetime] NULL,
	[UPDATE_ID] [nvarchar](10) NULL,
 CONSTRAINT [PK_T_SERIAL_STATUS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


USE [master]
GO
/****** Object:  Database [CableManagement]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE DATABASE [CableManagement]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CableManagement', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLDEV\MSSQL\DATA\CableManagement.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CableManagement_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLDEV\MSSQL\DATA\CableManagement_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [CableManagement] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CableManagement].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CableManagement] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CableManagement] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CableManagement] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CableManagement] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CableManagement] SET ARITHABORT OFF 
GO
ALTER DATABASE [CableManagement] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CableManagement] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CableManagement] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CableManagement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CableManagement] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CableManagement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CableManagement] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CableManagement] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CableManagement] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CableManagement] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CableManagement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CableManagement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CableManagement] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CableManagement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CableManagement] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CableManagement] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [CableManagement] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CableManagement] SET RECOVERY FULL 
GO
ALTER DATABASE [CableManagement] SET  MULTI_USER 
GO
ALTER DATABASE [CableManagement] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CableManagement] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CableManagement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CableManagement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CableManagement] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CableManagement] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'CableManagement', N'ON'
GO
ALTER DATABASE [CableManagement] SET QUERY_STORE = ON
GO
ALTER DATABASE [CableManagement] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [CableManagement]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cable]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cable](
	[CableId] [uniqueidentifier] NOT NULL,
	[WarehouseId] [int] NULL,
	[SupplierId] [int] NULL,
	[StartPoint] [int] NOT NULL,
	[EndPoint] [int] NOT NULL,
	[Length] [int] NOT NULL,
	[YearOfManufacture] [int] NULL,
	[Code] [nvarchar](50) NULL,
	[Status] [nvarchar](30) NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
	[CableParentId] [uniqueidentifier] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsExportedToUse] [bit] NOT NULL,
	[CableCategoryId] [int] NOT NULL,
	[IsInRequest] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Cable] PRIMARY KEY CLUSTERED 
(
	[CableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CableCategory]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CableCategory](
	[CableCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CableCategoryName] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_CableCategory] PRIMARY KEY CLUSTERED 
(
	[CableCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Issue]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Issue](
	[IssueId] [uniqueidentifier] NOT NULL,
	[IssueName] [nvarchar](50) NULL,
	[IssueCode] [varchar](50) NULL,
	[Description] [nvarchar](255) NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[CableRoutingName] [nvarchar](100) NULL,
	[Group] [nvarchar](50) NULL,
 CONSTRAINT [PK_Issue] PRIMARY KEY CLUSTERED 
(
	[IssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Node]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Node](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Longitude] [real] NOT NULL,
	[Latitude] [real] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Address] [nvarchar](max) NULL,
	[NodeCode] [nvarchar](max) NULL,
	[NodeNumberSign] [nvarchar](max) NULL,
	[Note] [nvarchar](max) NULL,
	[NumberOrder] [int] NOT NULL,
	[RouteId] [uniqueidentifier] NULL,
	[Status] [nvarchar](max) NULL,
	[MaterialCategory] [nvarchar](max) NULL,
 CONSTRAINT [PK_Node] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NodeCable]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NodeCable](
	[Id] [uniqueidentifier] NOT NULL,
	[CableId] [uniqueidentifier] NOT NULL,
	[NodeId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[OrderIndex] [int] NOT NULL,
 CONSTRAINT [PK_NodeCable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NodeMaterial]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NodeMaterial](
	[Id] [uniqueidentifier] NOT NULL,
	[OtherMaterialsId] [int] NOT NULL,
	[NodeId] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_NodeMaterial] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NodeMaterialCategory]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NodeMaterialCategory](
	[Id] [uniqueidentifier] NOT NULL,
	[OtherMaterialCategoryId] [int] NOT NULL,
	[NodeId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_NodeMaterialCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OtherMaterials]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OtherMaterials](
	[OtherMaterialsId] [int] IDENTITY(1,1) NOT NULL,
	[Unit] [nvarchar](15) NULL,
	[Quantity] [int] NULL,
	[Code] [nvarchar](50) NULL,
	[SupplierId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[WarehouseId] [int] NULL,
	[MaxQuantity] [int] NOT NULL,
	[MinQuantity] [int] NOT NULL,
	[Status] [nvarchar](15) NULL,
	[OtherMaterialsCategoryId] [int] NOT NULL,
 CONSTRAINT [PK__OtherMat__14E82BF4B0F17E3B] PRIMARY KEY CLUSTERED 
(
	[OtherMaterialsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OtherMaterialsCategory]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OtherMaterialsCategory](
	[OtherMaterialsCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[OtherMaterialsCategoryName] [nvarchar](100) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_OtherMaterialsCategory] PRIMARY KEY CLUSTERED 
(
	[OtherMaterialsCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Request]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Request](
	[RequestId] [uniqueidentifier] NOT NULL,
	[RequestName] [nvarchar](50) NULL,
	[Content] [nvarchar](255) NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
	[ApproverId] [uniqueidentifier] NULL,
	[IssueId] [uniqueidentifier] NULL,
	[Status] [nvarchar](20) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RequestCategoryId] [int] NOT NULL,
	[DeliverWarehouseId] [int] NULL,
 CONSTRAINT [PK_Request] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestCable]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestCable](
	[RequestId] [uniqueidentifier] NOT NULL,
	[CableId] [uniqueidentifier] NOT NULL,
	[StartPoint] [int] NOT NULL,
	[EndPoint] [int] NOT NULL,
	[Length] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RecoveryDestWarehouseId] [int] NULL,
	[Status] [nvarchar](15) NULL,
	[ImportedWarehouseId] [int] NULL,
 CONSTRAINT [PK__RequestC__9AC47BE7FD0DAC77] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC,
	[CableId] ASC,
	[StartPoint] ASC,
	[EndPoint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestCategories]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestCategories](
	[RequestCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[RequestCategoryName] [nvarchar](50) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_RequestCategories] PRIMARY KEY CLUSTERED 
(
	[RequestCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestOtherMaterials]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestOtherMaterials](
	[RequestId] [uniqueidentifier] NOT NULL,
	[OtherMaterialsId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[RecoveryDestWarehouseId] [int] NULL,
	[Status] [nvarchar](15) NULL,
	[ImportedWarehouseId] [int] NULL,
 CONSTRAINT [PK__RequestO__62E6D3C5FBF475BA] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC,
	[OtherMaterialsId] ASC,
	[Quantity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[Rolename] [varchar](20) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Route]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Route](
	[RouteId] [uniqueidentifier] NOT NULL,
	[RouteName] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
(
	[RouteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[SupplierId] [int] IDENTITY(1,1) NOT NULL,
	[SupplierName] [nvarchar](50) NOT NULL,
	[Country] [varchar](50) NULL,
	[CreatorId] [uniqueidentifier] NULL,
	[SupplierDescription] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionCable]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionCable](
	[TransactionId] [uniqueidentifier] NOT NULL,
	[CableId] [uniqueidentifier] NOT NULL,
	[StartPoint] [int] NOT NULL,
	[EndPoint] [int] NOT NULL,
	[Length] [int] NOT NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK__Transact__FC2F10F68675DA26] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC,
	[CableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionHistory]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionHistory](
	[TransactionId] [uniqueidentifier] NOT NULL,
	[TransactionCategoryName] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[WarehouseId] [int] NULL,
	[RequestId] [uniqueidentifier] NOT NULL,
	[FromWarehouseId] [int] NULL,
	[IssueId] [uniqueidentifier] NULL,
	[ToWarehouseId] [int] NULL,
 CONSTRAINT [PK__Transact__55433A6B23692FC5] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionOtherMaterials]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionOtherMaterials](
	[TransactionId] [uniqueidentifier] NOT NULL,
	[OtherMaterialsId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK__Transact__040DB8D40D72C562] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC,
	[OtherMaterialsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [uniqueidentifier] NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Firstname] [nvarchar](50) NOT NULL,
	[Lastname] [nvarchar](50) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Phone] [varchar](25) NULL,
	[RoleId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Warehouses]    Script Date: 2023-11-27 10:58:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warehouses](
	[WarehouseId] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseName] [nvarchar](max) NULL,
	[WarehouseKeeperid] [uniqueidentifier] NULL,
	[CreatorId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdateAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[WarehouseAddress] [nvarchar](max) NULL,
 CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED 
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230614112755_initdatabase', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230614142939_update-db-supplier', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230621152615_addwarehouseaddress', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230624142812_nvarchar for material unit', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230627103942_set-relation-database', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230630052806_fix approverId in request', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230630164546_add-node', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230703091217_add status for Issue', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230704175348_update transaction, warehouse, materials', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230705105036_update transaction and user', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230706092954_update transaction', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230707045202_update request cable', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230707101938_update-cable', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230711155515_add-order-node-cable', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230711165726_add-quantity', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230721143743_fix-db-cable-materials', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230721145138_add field status for material', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230721152216_fix issue', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230721161517_fix cables', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722061558_add table request category', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722062739_add FK for rq rqCate', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722091240_fix db material - field code', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722153400_update request table', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722174649_update cable material', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722190903_change field name in request', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722191626_update reqMat', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230722192248_update rqCab + rqMat', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230726035434_fix-issue-Id-in-Request', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230726152232_fix-issue-add-group-cableRouting', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230726161748_add-fromto-warehouseId for transaction', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230727143436_add-field-issueCode-in-transaction', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230727172946_add-field code for rqMat and TranMat', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230727180857_add status for transactionMat and rqMat', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230727182005_delete field code in rqMat and tranMat', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230728041416_add-field status for rqCab', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230728154541_restore cable and material cateogory', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230801070331_change property in table transaction', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230801072400_update FK for warehosue and request', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230804083943_add field IsInImportRequest in Cable table', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230805172456_add-route', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230810233916_nodeMaterialCategory', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230811002117_add-materialstr', N'6.0.1')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230815092936_add field description and rename isInImportRequest to IsInRequest in table Cable', N'6.0.1')
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'5457f352-925a-4f1c-bdfd-0fab00cf9662', 2, 1, 0, 1000, 1000, 2023, N'20000209', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425217' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349974' AS DateTime2), 0, 0, 7, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'c4de20ad-283b-430b-ae4b-1527c21aabad', 2, 1, 2500, 4000, 1500, 2020, N'20000215', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', N'78c85134-43a5-4069-8282-a29c170077a9', CAST(N'2023-11-23T02:24:14.8318189' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349984' AS DateTime2), 0, 0, 4, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'afe9450c-ea8f-4262-adf4-3e1d3acac844', 2, 1, 0, 500, 500, 2020, N'20000215', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', N'78c85134-43a5-4069-8282-a29c170077a9', CAST(N'2023-11-23T02:24:14.8318151' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349982' AS DateTime2), 0, 0, 4, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'5cb80ef1-9408-469b-b629-63c2bc7007e9', 1, 2, 0, 3000, 3000, 2020, N'20000205', N'Cũ', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425173' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349970' AS DateTime2), 0, 0, 4, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'e96c02f2-0f73-493e-84f6-65c1852632f1', 2, 1, 0, 2000, 2000, 2022, N'20000214', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425232' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349979' AS DateTime2), 0, 0, 3, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'037278ec-dd5a-403f-a827-8cdeaf3d1cbd', 1, 1, 0, 1000, 1000, 2022, N'20000207', N'mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425201' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349972' AS DateTime2), 0, 0, 6, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'78c85134-43a5-4069-8282-a29c170077a9', 2, 1, 0, 4000, 4000, 2020, N'20000215', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425235' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349980' AS DateTime2), 1, 0, 4, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'1db8da1b-b72c-4933-8cbd-b6db8dc147a3', NULL, 1, 0, 500, 500, 2022, N'20000203', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2424874' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349968' AS DateTime2), 0, 1, 2, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'876123c5-2061-49b8-97ec-c7f88c179745', 3, 1, 0, 3000, 3000, 2021, N'20000212', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425227' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349978' AS DateTime2), 0, 0, 2, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'63bd8c46-ca3c-4202-90d9-cc9d7ddc36e2', 1, 1, 0, 1000, 1000, 2019, N'20000202', N'Cũ', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2424474' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349966' AS DateTime2), 0, 0, 1, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'f7ad6558-e9c1-4dbd-920b-cd00f5408250', 1, 1, 0, 2000, 2000, 2022, N'20000204', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425168' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349969' AS DateTime2), 0, 0, 3, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'7f02da97-e4c8-417c-ac2b-da09fb73081b', NULL, 1, 500, 2500, 2000, 2020, N'20000215', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', N'78c85134-43a5-4069-8282-a29c170077a9', CAST(N'2023-11-23T02:24:14.8318185' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349983' AS DateTime2), 0, 1, 4, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'a0163b0d-5f48-4a95-a12c-df0645efcdad', 1, 2, 0, 1000, 1000, 2023, N'20000208', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425214' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349974' AS DateTime2), 0, 0, 7, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'096c1a69-3356-4dd4-9c08-e014c379e8c6', 1, 2, 0, 2500, 2500, 2022, N'20000206', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425192' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349971' AS DateTime2), 0, 0, 5, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'a0dc4f22-1c6e-491d-a482-e5ba5b444bdc', 3, 1, 0, 4000, 4000, 2020, N'20000211', N'Cũ', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425224' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349977' AS DateTime2), 0, 0, 2, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'36599039-0162-47ca-9885-e8761e0b023f', 3, 1, 0, 3000, 3000, 2022, N'20000213', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425230' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349979' AS DateTime2), 0, 0, 9, 0, NULL)
GO
INSERT [dbo].[Cable] ([CableId], [WarehouseId], [SupplierId], [StartPoint], [EndPoint], [Length], [YearOfManufacture], [Code], [Status], [CreatorId], [CableParentId], [CreatedAt], [UpdateAt], [IsDeleted], [IsExportedToUse], [CableCategoryId], [IsInRequest], [Description]) VALUES (N'e20b059f-fc6f-400f-b38c-fc6903194cb9', 2, 1, 0, 5000, 5000, 2023, N'20000210', N'Mới', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:14.2425220' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349975' AS DateTime2), 0, 0, 8, 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[CableCategory] ON 
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (1, N'Cáp quang ADSS 24FO KV100', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349955' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (2, N'Cáp quang ADSS 24FO KV300', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349957' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (3, N'Cáp quang ADSS 36FO KV100 (30FO G.652D + 6FO G.655)', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349958' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (4, N'Cáp quang ADSS 48FO KV400', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349959' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (5, N'Cáp quang ADSS 24FO KV400', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349960' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (6, N'Cáp quang ADSS 36FO KV500', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349961' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (7, N'Cáp quang ADSS 24FO G.652D KV100 (Chống sóc cắn) - LS', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349961' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (8, N'demo1', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349962' AS DateTime2), 0)
GO
INSERT [dbo].[CableCategory] ([CableCategoryId], [CableCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (9, N'demo2', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349963' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[CableCategory] OFF
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'00998dfe-7e5e-49cd-a6f7-2521aa2b49d2', N'Sự cố 2', N'SC20230506-06', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-04-04T00:00:00.000' AS DateTime), CAST(N'2023-03-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350029' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'3297c5a6-29c6-4df8-8956-457fba06082a', N'Sự cố 2', N'SC20230506-05', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-03-12T00:00:00.000' AS DateTime), CAST(N'2023-03-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350028' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'db65bbf8-ac45-40ac-8ff9-4d33b1f2beac', N'Sự cố 2', N'SC20230506-09', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-02-01T00:00:00.000' AS DateTime), CAST(N'2023-02-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350033' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'74ff851f-5c36-4f17-9a90-84bd7df50378', N'Sự cố 2', N'SC20230506-02', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-02-01T00:00:00.000' AS DateTime), CAST(N'2023-05-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350024' AS DateTime2), 0, N'Đang xử lý', N'Tuyến cáp B', N'Nhóm B')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', N'Sự cố 1', N'SC20230505-01', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-01-01T00:00:00.000' AS DateTime), CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350023' AS DateTime2), 0, N'Đang xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'd9936889-74d4-4ba3-8ab1-a93e152565e0', N'Sự cố 3', N'SC20230605-03', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-03-01T00:00:00.000' AS DateTime), CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350026' AS DateTime2), 0, N'Đang xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'7df65f03-2bfb-47f5-9acd-b30129cb22fd', N'Sự cố 2', N'SC20230506-07', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-05-01T00:00:00.000' AS DateTime), CAST(N'2023-04-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350031' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'23045931-a14b-4933-9c02-b84b86a60cf2', N'Sự cố 2', N'SC20230506-08', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-03-01T00:00:00.000' AS DateTime), CAST(N'2023-01-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350032' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'88cd8666-b628-46c3-af50-cf40f01615c2', N'Sự cố 2', N'SC20230506-11', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2022-11-01T00:00:00.000' AS DateTime), CAST(N'2023-06-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350035' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp C', N'Nhóm C')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'f1019ddc-b4cf-4c1b-8735-de4d0bba38b1', N'Sự cố 2', N'SC20230506-10', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2022-12-01T00:00:00.000' AS DateTime), CAST(N'2023-03-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350034' AS DateTime2), 0, N'Đã xử lý', N'Tuyến cáp C', N'Nhóm C')
GO
INSERT [dbo].[Issue] ([IssueId], [IssueName], [IssueCode], [Description], [CreatorId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [Status], [CableRoutingName], [Group]) VALUES (N'5f80289a-24e5-434b-b7b3-ffa086962315', N'Sự cố 2', N'SC20230506-04', N'Mô tả ...', N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', CAST(N'2023-03-06T00:00:00.000' AS DateTime), CAST(N'2023-02-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-24T03:46:12.0565964' AS DateTime2), 0, N'Đang xử lý', N'Tuyến cáp A', N'Nhóm A')
GO
INSERT [dbo].[Node] ([Id], [Name], [Description], [Longitude], [Latitude], [CreatedAt], [UpdateAt], [IsDeleted], [Address], [NodeCode], [NodeNumberSign], [Note], [NumberOrder], [RouteId], [Status], [MaterialCategory]) VALUES (N'207d7ff6-ec3c-482d-273b-08dbebbef560', NULL, NULL, 105.910339, 20.997488, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-27T10:47:01.2034239' AS DateTime2), 0, NULL, N'H1234', N'12', NULL, 1, N'8de12288-0bac-40b3-345e-08dbebbedcfd', N'Mới', NULL)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'85f41a31-314e-48bf-0441-08dbebbf41b0', 1, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 2, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:59:01.6038934' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'e08a6dfa-586d-4cd5-0442-08dbebbf41b0', 2, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 3, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T08:01:35.3052401' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'9d5bdb3e-8c57-4cad-f763-08dbedc87d5e', 2, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 2, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-25T22:20:03.5366135' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'02fd09d8-aeba-4daf-f764-08dbedc87d5e', 3, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 1, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-25T22:20:03.5555968' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'ff7b53ad-c9ab-4e32-3635-08dbef363d90', 2, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 3, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-27T10:47:51.7254650' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'5631c984-1ee5-40c5-3636-08dbef363d90', 3, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 1, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-27T10:50:10.7924244' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'f7dcc58d-cbc6-4504-3637-08dbef363d90', 6, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 2, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-27T17:59:17.2071948' AS DateTime2), 0)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'be7d058f-caf6-45cf-3638-08dbef363d90', 7, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 3, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-27T17:58:23.3414522' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'e6e6e7ef-fa35-4f21-87bb-19564808c6a6', 8, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 9, CAST(N'2023-11-27T17:58:23.2152782' AS DateTime2), CAST(N'2023-11-27T17:59:17.2170276' AS DateTime2), 0)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'23d350cb-3b3c-46c1-ad7a-2344c15ff3ee', 3, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 1, CAST(N'2023-11-23T08:01:11.6718945' AS DateTime2), CAST(N'2023-11-23T08:01:35.2327711' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'329c1647-c9c8-469c-8763-96ed22a88949', 6, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 1, CAST(N'2023-11-25T22:20:03.3706106' AS DateTime2), CAST(N'2023-11-27T10:47:24.3473271' AS DateTime2), 1)
GO
INSERT [dbo].[NodeMaterialCategory] ([Id], [OtherMaterialCategoryId], [NodeId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'fe4f71df-1804-4470-91cb-d074a4aa54a9', 4, N'207d7ff6-ec3c-482d-273b-08dbebbef560', 1, CAST(N'2023-11-23T08:01:35.2138217' AS DateTime2), CAST(N'2023-11-25T15:09:15.6341946' AS DateTime2), 1)
GO
SET IDENTITY_INSERT [dbo].[OtherMaterials] ON 
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (1, N'Bộ', 100, N'20000202', 1, CAST(N'2023-11-23T02:24:16.1098231' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350006' AS DateTime2), 0, 1, 0, 0, N'Mới', 1)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (2, N'Bộ', 150, N'20000203', 1, CAST(N'2023-11-23T02:24:16.1099781' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350007' AS DateTime2), 0, 1, 0, 0, N'Mới', 2)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (3, N'Bộ', 600, N'20000204', 1, CAST(N'2023-11-23T02:24:16.1099786' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350009' AS DateTime2), 0, 1, 0, 0, N'Mới', 3)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (4, N'Bộ', 700, N'20000205', 1, CAST(N'2023-11-23T02:24:16.1099788' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350009' AS DateTime2), 0, 1, 0, 0, N'Mới', 4)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (5, N'Bộ', 200, N'20000206', 1, CAST(N'2023-11-23T02:24:16.1099790' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350011' AS DateTime2), 0, 1, 0, 0, N'Mới', 5)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (6, N'Bộ', 201, N'20000207', 1, CAST(N'2023-11-23T02:24:16.1099795' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350012' AS DateTime2), 0, 1, 0, 0, N'Mới', 6)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (7, N'Bộ', 160, N'20000208', 1, CAST(N'2023-11-23T02:24:16.1099797' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350014' AS DateTime2), 0, 1, 0, 0, N'Mới', 6)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (8, N'Bộ', 300, N'20000209', 1, CAST(N'2023-11-23T02:24:16.1099798' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350015' AS DateTime2), 0, 1, 0, 0, N'Mới', 7)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (9, N'Bộ', 345, N'20000210', 1, CAST(N'2023-11-23T02:24:16.1099800' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350016' AS DateTime2), 0, 1, 0, 0, N'Mới', 8)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (10, N'Bộ', 790, N'20000211', 1, CAST(N'2023-11-23T02:24:16.1099803' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350017' AS DateTime2), 0, 1, 0, 0, N'Mới', 9)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (11, N'Bộ', 979, N'20000212', 2, CAST(N'2023-11-23T02:24:16.1099805' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350017' AS DateTime2), 0, 1, 0, 0, N'Mới', 10)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (12, N'Bộ', 100, N'20000213', 1, CAST(N'2023-11-23T02:24:16.1099806' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350018' AS DateTime2), 0, 2, 0, 0, N'Mới', 1)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (13, N'Bộ', 150, N'20000214', 1, CAST(N'2023-11-23T02:24:16.1099808' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350020' AS DateTime2), 0, 2, 0, 0, N'Mới', 2)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (14, N'Bộ', 600, N'200002015', 1, CAST(N'2023-11-23T02:24:16.1099809' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350020' AS DateTime2), 0, 2, 0, 0, N'Mới', 3)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (15, N'Bộ', 700, N'20000216', 1, CAST(N'2023-11-23T02:24:16.1099811' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350021' AS DateTime2), 0, 2, 0, 0, N'Mới', 4)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (16, N'Bộ', 979, N'20000217', 2, CAST(N'2023-11-23T02:24:16.1099823' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350006' AS DateTime2), 0, 2, 0, 0, N'Mới', 10)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (17, N'Bộ', 100, N'20000218', 1, CAST(N'2023-11-23T02:24:16.1099826' AS DateTime2), CAST(N'2023-11-24T03:46:12.0565966' AS DateTime2), 0, 3, 0, 0, N'Mới', 1)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (18, N'Bộ', 100, N'20000202', 1, CAST(N'2023-11-23T02:24:16.1099829' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349999' AS DateTime2), 0, 2, 0, 0, N'Mới', 1)
GO
INSERT [dbo].[OtherMaterials] ([OtherMaterialsId], [Unit], [Quantity], [Code], [SupplierId], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [MaxQuantity], [MinQuantity], [Status], [OtherMaterialsCategoryId]) VALUES (19, N'Bộ', 20, N'20000202', 1, CAST(N'2023-11-23T02:24:16.1099830' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349998' AS DateTime2), 0, 2, 0, 0, N'Cũ', 1)
GO
SET IDENTITY_INSERT [dbo].[OtherMaterials] OFF
GO
SET IDENTITY_INSERT [dbo].[OtherMaterialsCategory] ON 
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (1, N'Bộ treo cáp ADSS 24FO KV100', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-24T03:46:12.0565968' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (2, N'Bộ treo cáp ADSS 24FO KV200', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082096' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (3, N'Bộ treo cáp ADSS 24FO KV300', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082099' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (4, N'Bộ treo cáp ADSS 24FO KV400', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082107' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (5, N'Bộ treo cáp ADSS 24FO KV500', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082110' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (6, N'Bộ treo cáp ADSS 48FO KV100', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082113' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (7, N'Măng Xông 24FO NWC', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082116' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (8, N'Măng Xông 48FO NWC', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082120' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (9, N'Hộp ODF 24 Core SC/APC', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082124' AS DateTime2), 0)
GO
INSERT [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId], [OtherMaterialsCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (10, N'Chống rung 1350 (L1 1300mm)', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082127' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[OtherMaterialsCategory] OFF
GO
INSERT [dbo].[Request] ([RequestId], [RequestName], [Content], [CreatorId], [ApproverId], [IssueId], [Status], [CreatedAt], [UpdateAt], [IsDeleted], [RequestCategoryId], [DeliverWarehouseId]) VALUES (N'2336391c-8789-4b94-af6e-0532f6034cd5', N'Yêu cầu 2 ', NULL, N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', N'Approved', CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350048' AS DateTime2), 0, 1, NULL)
GO
INSERT [dbo].[Request] ([RequestId], [RequestName], [Content], [CreatorId], [ApproverId], [IssueId], [Status], [CreatedAt], [UpdateAt], [IsDeleted], [RequestCategoryId], [DeliverWarehouseId]) VALUES (N'3a264187-e458-436b-82a2-0c869d075e99', N'Yêu cầu 3 ', NULL, N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', N'Rejected', CAST(N'2023-06-10T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-24T04:20:51.0702889' AS DateTime2), 0, 1, NULL)
GO
INSERT [dbo].[Request] ([RequestId], [RequestName], [Content], [CreatorId], [ApproverId], [IssueId], [Status], [CreatedAt], [UpdateAt], [IsDeleted], [RequestCategoryId], [DeliverWarehouseId]) VALUES (N'19f10d44-52ee-4920-a836-7880a016445b', N'Yêu cầu 1 ', NULL, N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', N'Approved', CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350047' AS DateTime2), 0, 1, NULL)
GO
INSERT [dbo].[Request] ([RequestId], [RequestName], [Content], [CreatorId], [ApproverId], [IssueId], [Status], [CreatedAt], [UpdateAt], [IsDeleted], [RequestCategoryId], [DeliverWarehouseId]) VALUES (N'6cdc4dbd-f422-4a94-a76a-ab55837c7299', N'Yêu cầu 5 ', NULL, N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', NULL, N'Approved', CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350051' AS DateTime2), 0, 3, NULL)
GO
INSERT [dbo].[Request] ([RequestId], [RequestName], [Content], [CreatorId], [ApproverId], [IssueId], [Status], [CreatedAt], [UpdateAt], [IsDeleted], [RequestCategoryId], [DeliverWarehouseId]) VALUES (N'898f8cad-70cd-4e05-bb54-b3fa7bd01ac3', N'Yêu cầu 4 ', NULL, N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', NULL, N'Approved', CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350050' AS DateTime2), 0, 4, 2)
GO
INSERT [dbo].[Request] ([RequestId], [RequestName], [Content], [CreatorId], [ApproverId], [IssueId], [Status], [CreatedAt], [UpdateAt], [IsDeleted], [RequestCategoryId], [DeliverWarehouseId]) VALUES (N'1a2ee70d-9eab-4344-a3a7-dcc5732f245d', N'Test xuat kho 1', NULL, N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', N'5f80289a-24e5-434b-b7b3-ffa086962315', N'Rejected', CAST(N'2023-11-24T10:46:11.8742712' AS DateTime2), CAST(N'2023-11-24T03:47:21.7972716' AS DateTime2), 0, 1, NULL)
GO
INSERT [dbo].[RequestCable] ([RequestId], [CableId], [StartPoint], [EndPoint], [Length], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'2336391c-8789-4b94-af6e-0532f6034cd5', N'1db8da1b-b72c-4933-8cbd-b6db8dc147a3', 0, 500, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350054' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestCable] ([RequestId], [CableId], [StartPoint], [EndPoint], [Length], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'3a264187-e458-436b-82a2-0c869d075e99', N'c4de20ad-283b-430b-ae4b-1527c21aabad', 2500, 4000, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-24T04:20:51.0702929' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestCable] ([RequestId], [CableId], [StartPoint], [EndPoint], [Length], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'19f10d44-52ee-4920-a836-7880a016445b', N'7f02da97-e4c8-417c-ac2b-da09fb73081b', 500, 2500, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350053' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestCable] ([RequestId], [CableId], [StartPoint], [EndPoint], [Length], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'6cdc4dbd-f422-4a94-a76a-ab55837c7299', N'afe9450c-ea8f-4262-adf4-3e1d3acac844', 0, 500, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350057' AS DateTime2), 0, 2, N'Mới', NULL)
GO
INSERT [dbo].[RequestCable] ([RequestId], [CableId], [StartPoint], [EndPoint], [Length], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'898f8cad-70cd-4e05-bb54-b3fa7bd01ac3', N'78c85134-43a5-4069-8282-a29c170077a9', 0, 4000, 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350056' AS DateTime2), 0, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[RequestCategories] ON 
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (1, N'Xuất Kho', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350038' AS DateTime2), 0)
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (2, N'Nhập Kho', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350040' AS DateTime2), 0)
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (3, N'Thu Hồi', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350041' AS DateTime2), 0)
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (4, N'Chuyển Kho', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350042' AS DateTime2), 0)
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (5, N'Hủy', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350043' AS DateTime2), 0)
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (6, N'Hủy Ngoài Kho', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350044' AS DateTime2), 0)
GO
INSERT [dbo].[RequestCategories] ([RequestCategoryId], [RequestCategoryName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (7, N'Khác', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350044' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[RequestCategories] OFF
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'2336391c-8789-4b94-af6e-0532f6034cd5', 4, 20, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350062' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'3a264187-e458-436b-82a2-0c869d075e99', 4, 10, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350064' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'19f10d44-52ee-4920-a836-7880a016445b', 1, 10, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350060' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'19f10d44-52ee-4920-a836-7880a016445b', 2, 15, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350061' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'6cdc4dbd-f422-4a94-a76a-ab55837c7299', 1, 20, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350065' AS DateTime2), 0, 2, N'Cũ', NULL)
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'898f8cad-70cd-4e05-bb54-b3fa7bd01ac3', 1, 10, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350064' AS DateTime2), 0, NULL, NULL, NULL)
GO
INSERT [dbo].[RequestOtherMaterials] ([RequestId], [OtherMaterialsId], [Quantity], [CreatedAt], [UpdateAt], [IsDeleted], [RecoveryDestWarehouseId], [Status], [ImportedWarehouseId]) VALUES (N'1a2ee70d-9eab-4344-a3a7-dcc5732f245d', 17, 10, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-24T03:46:12.0565960' AS DateTime2), 0, 3, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Role] ON 
GO
INSERT [dbo].[Role] ([RoleId], [Rolename], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (1, N'Admin', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350088' AS DateTime2), 0)
GO
INSERT [dbo].[Role] ([RoleId], [Rolename], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (2, N'Leader', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350089' AS DateTime2), 0)
GO
INSERT [dbo].[Role] ([RoleId], [Rolename], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (3, N'Staff', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350090' AS DateTime2), 0)
GO
INSERT [dbo].[Role] ([RoleId], [Rolename], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (4, N'WareHouse Keeper', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350091' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
INSERT [dbo].[Route] ([RouteId], [RouteName], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'8de12288-0bac-40b3-345e-08dbebbedcfd', N'test', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-23T00:55:59.7082082' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[Supplier] ON 
GO
INSERT [dbo].[Supplier] ([SupplierId], [SupplierName], [Country], [CreatorId], [SupplierDescription], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (1, N'Nhà cung cấp A', N'Vietnam', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:13.4261841' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349950' AS DateTime2), 0)
GO
INSERT [dbo].[Supplier] ([SupplierId], [SupplierName], [Country], [CreatorId], [SupplierDescription], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (2, N'Nhà cung cấp B', N'Trung Quoc', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:13.4262289' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349951' AS DateTime2), 0)
GO
INSERT [dbo].[Supplier] ([SupplierId], [SupplierName], [Country], [CreatorId], [SupplierDescription], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (3, N'Nhà cung cấp C', N'Hoa Ky', N'65102f42-abc3-49f3-87c3-b53b12db324d', NULL, CAST(N'2023-11-23T02:24:13.4262292' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349952' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[Supplier] OFF
GO
INSERT [dbo].[TransactionCable] ([TransactionId], [CableId], [StartPoint], [EndPoint], [Length], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'f58d3293-bf48-4cbb-a25f-37f0a8dffef8', N'78c85134-43a5-4069-8282-a29c170077a9', 0, 4000, 4000, NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350082' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionCable] ([TransactionId], [CableId], [StartPoint], [EndPoint], [Length], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'02fd6464-5b0f-4ad9-90a7-c4e06b578d95', N'7f02da97-e4c8-417c-ac2b-da09fb73081b', 500, 2500, 2000, NULL, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350078' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionCable] ([TransactionId], [CableId], [StartPoint], [EndPoint], [Length], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'e01cef00-fe5f-483e-b86b-d09ed1d3e109', N'1db8da1b-b72c-4933-8cbd-b6db8dc147a3', 500, 2500, 2000, NULL, CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350079' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionCable] ([TransactionId], [CableId], [StartPoint], [EndPoint], [Length], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'7d182342-5a0d-4ef6-ad67-d1d3f1cca8eb', N'78c85134-43a5-4069-8282-a29c170077a9', 0, 4000, 4000, NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350080' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionCable] ([TransactionId], [CableId], [StartPoint], [EndPoint], [Length], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'81320ecf-f512-43c8-8ab7-f1464089d46c', N'afe9450c-ea8f-4262-adf4-3e1d3acac844', 0, 500, 500, NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350083' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionHistory] ([TransactionId], [TransactionCategoryName], [Description], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [RequestId], [FromWarehouseId], [IssueId], [ToWarehouseId]) VALUES (N'f58d3293-bf48-4cbb-a25f-37f0a8dffef8', N'Nhập Kho', N'', NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350074' AS DateTime2), 0, 2, N'898f8cad-70cd-4e05-bb54-b3fa7bd01ac3', 1, NULL, NULL)
GO
INSERT [dbo].[TransactionHistory] ([TransactionId], [TransactionCategoryName], [Description], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [RequestId], [FromWarehouseId], [IssueId], [ToWarehouseId]) VALUES (N'02fd6464-5b0f-4ad9-90a7-c4e06b578d95', N'Xuất Kho', N'', NULL, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350069' AS DateTime2), 0, 2, N'19f10d44-52ee-4920-a836-7880a016445b', NULL, N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', NULL)
GO
INSERT [dbo].[TransactionHistory] ([TransactionId], [TransactionCategoryName], [Description], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [RequestId], [FromWarehouseId], [IssueId], [ToWarehouseId]) VALUES (N'e01cef00-fe5f-483e-b86b-d09ed1d3e109', N'Xuất Kho', N'', NULL, CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350070' AS DateTime2), 0, 1, N'2336391c-8789-4b94-af6e-0532f6034cd5', NULL, N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', NULL)
GO
INSERT [dbo].[TransactionHistory] ([TransactionId], [TransactionCategoryName], [Description], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [RequestId], [FromWarehouseId], [IssueId], [ToWarehouseId]) VALUES (N'7d182342-5a0d-4ef6-ad67-d1d3f1cca8eb', N'Xuất Kho', N'', NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350074' AS DateTime2), 0, 1, N'898f8cad-70cd-4e05-bb54-b3fa7bd01ac3', NULL, NULL, 2)
GO
INSERT [dbo].[TransactionHistory] ([TransactionId], [TransactionCategoryName], [Description], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [RequestId], [FromWarehouseId], [IssueId], [ToWarehouseId]) VALUES (N'6ce32f72-8861-4caa-9efe-e183853acff2', N'Xuất Kho', N'', NULL, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350067' AS DateTime2), 0, 1, N'19f10d44-52ee-4920-a836-7880a016445b', NULL, N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', NULL)
GO
INSERT [dbo].[TransactionHistory] ([TransactionId], [TransactionCategoryName], [Description], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseId], [RequestId], [FromWarehouseId], [IssueId], [ToWarehouseId]) VALUES (N'81320ecf-f512-43c8-8ab7-f1464089d46c', N'Nhập Kho', N'', NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350075' AS DateTime2), 0, 2, N'6cdc4dbd-f422-4a94-a76a-ab55837c7299', NULL, N'45bab2ff-419b-4e66-ad40-918b8dd25ab8', NULL)
GO
INSERT [dbo].[TransactionOtherMaterials] ([TransactionId], [OtherMaterialsId], [Quantity], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'f58d3293-bf48-4cbb-a25f-37f0a8dffef8', 18, 10, NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349891' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionOtherMaterials] ([TransactionId], [OtherMaterialsId], [Quantity], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'e01cef00-fe5f-483e-b86b-d09ed1d3e109', 4, 20, NULL, CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349892' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionOtherMaterials] ([TransactionId], [OtherMaterialsId], [Quantity], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'7d182342-5a0d-4ef6-ad67-d1d3f1cca8eb', 1, 10, NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349892' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionOtherMaterials] ([TransactionId], [OtherMaterialsId], [Quantity], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'6ce32f72-8861-4caa-9efe-e183853acff2', 1, 10, NULL, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349894' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionOtherMaterials] ([TransactionId], [OtherMaterialsId], [Quantity], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'6ce32f72-8861-4caa-9efe-e183853acff2', 2, 15, NULL, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349893' AS DateTime2), 0)
GO
INSERT [dbo].[TransactionOtherMaterials] ([TransactionId], [OtherMaterialsId], [Quantity], [Note], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'81320ecf-f512-43c8-8ab7-f1464089d46c', 19, 20, NULL, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349889' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'c638ee6f-f6e6-44e1-9cd9-022a09336270', N'thukho11', N'11', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho11@gmail.com', N'0475245738', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4350084' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'920bd51b-99c7-49ad-a42a-0cc50a07e4dd', N'Binhhd', N'Đức Bình', N'Hoàng', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'binhhd@gmail.com', N'0987654321', 2, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349933' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'fc2197ef-a56c-4dd4-9690-1ded2b18c696', N'thukho3', N'3', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho3@gmail.com', N'0564729465', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349929' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'fc9ca860-8940-4e30-b3b2-2a976ecd4a0a', N'thukho7', N'7', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho7@gmail.com', N'0475245738', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349901' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'2098b045-60ed-4557-b021-3db8bf146817', N'thukho5', N'5', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho5@gmail.com', N'0347339472', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349926' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'd1efc3f4-f402-41f3-9e41-4f425d73a0fc', N'thukho8', N'8', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho8@gmail.com', N'0475245738', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349900' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'26dbdf61-71e0-4157-9a5b-7597393cf8da', N'thukho2', N'2', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho2@gmail.com', N'0435643627', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349930' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'e487a775-3212-470a-bc1f-75d50a06b260', N'thukho10', N'10', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho10@gmail.com', N'0475245738', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349897' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'5fea0d30-35ef-4a5b-8bc0-815208a13e70', N'quyhp', N'Phú Quý', N'Hà', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'quyhp@gmail.com', N'0111444867', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349934' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'b4bf8a3c-2656-4ebc-afce-9b6dabd740b1', N'thukho4', N'4', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho4@gmail.com', N'0846593946', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349927' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'65102f42-abc3-49f3-87c3-b53b12db324d', N'hieupm', N'Minh Hiếu', N'Phạm', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'minhhieupham@gmail.com', N'0123456789', 1, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349932' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'7d6e8344-cfdb-4ee2-93ec-bec746bd3524', N'hienvt', N'Trí Hiền', N'Vũ', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'hienvt@gmail.com', N'0564738297', 3, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349935' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'4bfe97d6-0d44-4325-b459-c49f23516962', N'thukho6', N'6', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho6@gmail.com', N'0975739346', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349902' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'74ed82b7-c976-4ee2-bf1e-cdf04b83f197', N'thukho1', N'1', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho1@gmail.com', N'0562749375', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349931' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'6aea0199-ed6d-4935-8d41-d00eda54a408', N'ducnm1234', N'minh', N'duc', N'819e84f7eb8e81c9f1923eede0d202ef3103dced9e53f6e06c2608552ec69a78', N'vegitobasicinstict1810@gmail.com', N'0902281810', 3, CAST(N'2023-11-25T11:18:56.227' AS DateTime), CAST(N'2023-11-25T11:18:56.2274070' AS DateTime2), CAST(N'2023-11-25T11:25:45.8665721' AS DateTime2), 0)
GO
INSERT [dbo].[User] ([UserId], [Username], [Firstname], [Lastname], [Password], [Email], [Phone], [RoleId], [CreatedDate], [CreatedAt], [UpdateAt], [IsDeleted]) VALUES (N'289b0881-1c4c-4729-be5e-f42b973eaf17', N'thukho9', N'9', N'Thủ Kho', N'931145d4ddd1811be545e4ac88a81f1fdbfaf0779c437efba16b884595274d11', N'thukho9@gmail.com', N'0475245738', 4, CAST(N'2023-11-23T02:24:12.707' AS DateTime), CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349899' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[Warehouses] ON 
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (1, N'ĐCV Hà Nội', N'74ed82b7-c976-4ee2-bf1e-cdf04b83f197', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9955568' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9955147' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349937' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (2, N'ĐCV Nam Định', N'26dbdf61-71e0-4157-9a5b-7597393cf8da', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958070' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958069' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349938' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (3, N'ĐCV Thanh Hóa', N'fc2197ef-a56c-4dd4-9690-1ded2b18c696', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958087' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958087' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349940' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (4, N'ĐCV Cầu Giát', N'b4bf8a3c-2656-4ebc-afce-9b6dabd740b1', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958112' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958111' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349940' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (5, N'ĐCV Vinh', N'2098b045-60ed-4557-b021-3db8bf146817', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958118' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958117' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349942' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (6, N'ĐCV Hà Tĩnh', N'4bfe97d6-0d44-4325-b459-c49f23516962', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958127' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958126' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349942' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (7, N'ĐCV Kỳ Anh', N'fc9ca860-8940-4e30-b3b2-2a976ecd4a0a', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958130' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958129' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349943' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (8, N'ĐCV Đồng Hới', N'd1efc3f4-f402-41f3-9e41-4f425d73a0fc', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958134' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958134' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349944' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (9, N'ĐCV Huế', N'289b0881-1c4c-4729-be5e-f42b973eaf17', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958138' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958137' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349945' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (10, N'ĐCV Phú Lộc', N'e487a775-3212-470a-bc1f-75d50a06b260', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958142' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958142' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349947' AS DateTime2), 0, NULL)
GO
INSERT [dbo].[Warehouses] ([WarehouseId], [WarehouseName], [WarehouseKeeperid], [CreatorId], [CreatedDate], [UpdatedDate], [CreatedAt], [UpdateAt], [IsDeleted], [WarehouseAddress]) VALUES (11, N'FFC Uông Bí', N'c638ee6f-f6e6-44e1-9cd9-022a09336270', N'65102f42-abc3-49f3-87c3-b53b12db324d', CAST(N'2023-11-23T02:24:12.9958146' AS DateTime2), NULL, CAST(N'2023-11-23T02:24:12.9958145' AS DateTime2), CAST(N'2023-11-22T19:24:24.4349948' AS DateTime2), 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[Warehouses] OFF
GO
/****** Object:  Index [IX_Cable_CableCategoryId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Cable_CableCategoryId] ON [dbo].[Cable]
(
	[CableCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Cable_CreatorId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Cable_CreatorId] ON [dbo].[Cable]
(
	[CreatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Cable_SupplierId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Cable_SupplierId] ON [dbo].[Cable]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Cable_WarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Cable_WarehouseId] ON [dbo].[Cable]
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Issue_CreatorId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Issue_CreatorId] ON [dbo].[Issue]
(
	[CreatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Issue_IssueCode]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Issue_IssueCode] ON [dbo].[Issue]
(
	[IssueCode] ASC
)
WHERE ([IssueCode] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Node_RouteId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Node_RouteId] ON [dbo].[Node]
(
	[RouteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_NodeCable_CableId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_NodeCable_CableId] ON [dbo].[NodeCable]
(
	[CableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_NodeCable_NodeId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_NodeCable_NodeId] ON [dbo].[NodeCable]
(
	[NodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_NodeMaterial_NodeId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_NodeMaterial_NodeId] ON [dbo].[NodeMaterial]
(
	[NodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_NodeMaterial_OtherMaterialsId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_NodeMaterial_OtherMaterialsId] ON [dbo].[NodeMaterial]
(
	[OtherMaterialsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_NodeMaterialCategory_NodeId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_NodeMaterialCategory_NodeId] ON [dbo].[NodeMaterialCategory]
(
	[NodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_NodeMaterialCategory_OtherMaterialCategoryId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_NodeMaterialCategory_OtherMaterialCategoryId] ON [dbo].[NodeMaterialCategory]
(
	[OtherMaterialCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OtherMaterials_OtherMaterialsCategoryId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_OtherMaterials_OtherMaterialsCategoryId] ON [dbo].[OtherMaterials]
(
	[OtherMaterialsCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OtherMaterials_SupplierId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_OtherMaterials_SupplierId] ON [dbo].[OtherMaterials]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OtherMaterials_WarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_OtherMaterials_WarehouseId] ON [dbo].[OtherMaterials]
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Request_ApproverId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Request_ApproverId] ON [dbo].[Request]
(
	[ApproverId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Request_CreatorId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Request_CreatorId] ON [dbo].[Request]
(
	[CreatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Request_DeliverWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Request_DeliverWarehouseId] ON [dbo].[Request]
(
	[DeliverWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Request_IssueId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Request_IssueId] ON [dbo].[Request]
(
	[IssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Request_RequestCategoryId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Request_RequestCategoryId] ON [dbo].[Request]
(
	[RequestCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequestCable_CableId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequestCable_CableId] ON [dbo].[RequestCable]
(
	[CableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequestCable_ImportedWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequestCable_ImportedWarehouseId] ON [dbo].[RequestCable]
(
	[ImportedWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequestCable_RecoveryDestWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequestCable_RecoveryDestWarehouseId] ON [dbo].[RequestCable]
(
	[RecoveryDestWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequestOtherMaterials_ImportedWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequestOtherMaterials_ImportedWarehouseId] ON [dbo].[RequestOtherMaterials]
(
	[ImportedWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequestOtherMaterials_OtherMaterialsId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequestOtherMaterials_OtherMaterialsId] ON [dbo].[RequestOtherMaterials]
(
	[OtherMaterialsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RequestOtherMaterials_RecoveryDestWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_RequestOtherMaterials_RecoveryDestWarehouseId] ON [dbo].[RequestOtherMaterials]
(
	[RecoveryDestWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Supplier_CreatorId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Supplier_CreatorId] ON [dbo].[Supplier]
(
	[CreatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionCable_CableId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionCable_CableId] ON [dbo].[TransactionCable]
(
	[CableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionHistory_FromWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_FromWarehouseId] ON [dbo].[TransactionHistory]
(
	[FromWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionHistory_IssueId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_IssueId] ON [dbo].[TransactionHistory]
(
	[IssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionHistory_RequestId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_RequestId] ON [dbo].[TransactionHistory]
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionHistory_ToWarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_ToWarehouseId] ON [dbo].[TransactionHistory]
(
	[ToWarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionHistory_WarehouseId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionHistory_WarehouseId] ON [dbo].[TransactionHistory]
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TransactionOtherMaterials_OtherMaterialsId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_TransactionOtherMaterials_OtherMaterialsId] ON [dbo].[TransactionOtherMaterials]
(
	[OtherMaterialsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_User_RoleId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_User_RoleId] ON [dbo].[User]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Warehouses_CreatorId]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Warehouses_CreatorId] ON [dbo].[Warehouses]
(
	[CreatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Warehouses_WarehouseKeeperid]    Script Date: 2023-11-27 10:58:27 PM ******/
CREATE NONCLUSTERED INDEX [IX_Warehouses_WarehouseKeeperid] ON [dbo].[Warehouses]
(
	[WarehouseKeeperid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cable] ADD  DEFAULT (newid()) FOR [CableId]
GO
ALTER TABLE [dbo].[Cable] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsExportedToUse]
GO
ALTER TABLE [dbo].[Cable] ADD  DEFAULT ((0)) FOR [CableCategoryId]
GO
ALTER TABLE [dbo].[Cable] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsInRequest]
GO
ALTER TABLE [dbo].[CableCategory] ADD  DEFAULT (N'') FOR [CableCategoryName]
GO
ALTER TABLE [dbo].[Issue] ADD  DEFAULT (newid()) FOR [IssueId]
GO
ALTER TABLE [dbo].[Node] ADD  DEFAULT ((0)) FOR [NumberOrder]
GO
ALTER TABLE [dbo].[NodeCable] ADD  DEFAULT ((0)) FOR [OrderIndex]
GO
ALTER TABLE [dbo].[NodeMaterial] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[OtherMaterials] ADD  DEFAULT ((0)) FOR [MaxQuantity]
GO
ALTER TABLE [dbo].[OtherMaterials] ADD  DEFAULT ((0)) FOR [MinQuantity]
GO
ALTER TABLE [dbo].[OtherMaterials] ADD  DEFAULT ((0)) FOR [OtherMaterialsCategoryId]
GO
ALTER TABLE [dbo].[Request] ADD  DEFAULT (newid()) FOR [RequestId]
GO
ALTER TABLE [dbo].[Request] ADD  DEFAULT ((0)) FOR [RequestCategoryId]
GO
ALTER TABLE [dbo].[TransactionHistory] ADD  DEFAULT (newid()) FOR [TransactionId]
GO
ALTER TABLE [dbo].[TransactionHistory] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [RequestId]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (newid()) FOR [UserId]
GO
ALTER TABLE [dbo].[Cable]  WITH CHECK ADD  CONSTRAINT [FK__Cable__CreatorId__403A8C7D] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Cable] CHECK CONSTRAINT [FK__Cable__CreatorId__403A8C7D]
GO
ALTER TABLE [dbo].[Cable]  WITH CHECK ADD  CONSTRAINT [FK__Cable__SupplierI__3E52440B] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[Cable] CHECK CONSTRAINT [FK__Cable__SupplierI__3E52440B]
GO
ALTER TABLE [dbo].[Cable]  WITH CHECK ADD  CONSTRAINT [FK__Cable__Warehouse__3D5E1FD2] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[Cable] CHECK CONSTRAINT [FK__Cable__Warehouse__3D5E1FD2]
GO
ALTER TABLE [dbo].[Cable]  WITH CHECK ADD  CONSTRAINT [FK_Cable_CableCategory_CableCategoryId] FOREIGN KEY([CableCategoryId])
REFERENCES [dbo].[CableCategory] ([CableCategoryId])
GO
ALTER TABLE [dbo].[Cable] CHECK CONSTRAINT [FK_Cable_CableCategory_CableCategoryId]
GO
ALTER TABLE [dbo].[Issue]  WITH CHECK ADD  CONSTRAINT [FK__Issue__CreatorId__4F7CD00D] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Issue] CHECK CONSTRAINT [FK__Issue__CreatorId__4F7CD00D]
GO
ALTER TABLE [dbo].[Node]  WITH CHECK ADD  CONSTRAINT [FK_Node_Route_RouteId] FOREIGN KEY([RouteId])
REFERENCES [dbo].[Route] ([RouteId])
GO
ALTER TABLE [dbo].[Node] CHECK CONSTRAINT [FK_Node_Route_RouteId]
GO
ALTER TABLE [dbo].[NodeCable]  WITH CHECK ADD  CONSTRAINT [FK_NodeCable_Cable_CableId] FOREIGN KEY([CableId])
REFERENCES [dbo].[Cable] ([CableId])
GO
ALTER TABLE [dbo].[NodeCable] CHECK CONSTRAINT [FK_NodeCable_Cable_CableId]
GO
ALTER TABLE [dbo].[NodeCable]  WITH CHECK ADD  CONSTRAINT [FK_NodeCable_Node_NodeId] FOREIGN KEY([NodeId])
REFERENCES [dbo].[Node] ([Id])
GO
ALTER TABLE [dbo].[NodeCable] CHECK CONSTRAINT [FK_NodeCable_Node_NodeId]
GO
ALTER TABLE [dbo].[NodeMaterial]  WITH CHECK ADD  CONSTRAINT [FK_NodeMaterial_Node_NodeId] FOREIGN KEY([NodeId])
REFERENCES [dbo].[Node] ([Id])
GO
ALTER TABLE [dbo].[NodeMaterial] CHECK CONSTRAINT [FK_NodeMaterial_Node_NodeId]
GO
ALTER TABLE [dbo].[NodeMaterial]  WITH CHECK ADD  CONSTRAINT [FK_NodeMaterial_OtherMaterials_OtherMaterialsId] FOREIGN KEY([OtherMaterialsId])
REFERENCES [dbo].[OtherMaterials] ([OtherMaterialsId])
GO
ALTER TABLE [dbo].[NodeMaterial] CHECK CONSTRAINT [FK_NodeMaterial_OtherMaterials_OtherMaterialsId]
GO
ALTER TABLE [dbo].[NodeMaterialCategory]  WITH CHECK ADD  CONSTRAINT [FK_NodeMaterialCategory_Node_NodeId] FOREIGN KEY([NodeId])
REFERENCES [dbo].[Node] ([Id])
GO
ALTER TABLE [dbo].[NodeMaterialCategory] CHECK CONSTRAINT [FK_NodeMaterialCategory_Node_NodeId]
GO
ALTER TABLE [dbo].[NodeMaterialCategory]  WITH CHECK ADD  CONSTRAINT [FK_NodeMaterialCategory_OtherMaterialsCategory_OtherMaterialCategoryId] FOREIGN KEY([OtherMaterialCategoryId])
REFERENCES [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId])
GO
ALTER TABLE [dbo].[NodeMaterialCategory] CHECK CONSTRAINT [FK_NodeMaterialCategory_OtherMaterialsCategory_OtherMaterialCategoryId]
GO
ALTER TABLE [dbo].[OtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK__OtherMate__Suppl__33D4B598] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[OtherMaterials] CHECK CONSTRAINT [FK__OtherMate__Suppl__33D4B598]
GO
ALTER TABLE [dbo].[OtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK__OtherMate__Wareh__36B12233] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[OtherMaterials] CHECK CONSTRAINT [FK__OtherMate__Wareh__36B12233]
GO
ALTER TABLE [dbo].[OtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK_OtherMaterials_OtherMaterialsCategory_OtherMaterialsCategoryId] FOREIGN KEY([OtherMaterialsCategoryId])
REFERENCES [dbo].[OtherMaterialsCategory] ([OtherMaterialsCategoryId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OtherMaterials] CHECK CONSTRAINT [FK_OtherMaterials_OtherMaterialsCategory_OtherMaterialsCategoryId]
GO
ALTER TABLE [dbo].[Request]  WITH CHECK ADD  CONSTRAINT [FK__Request__Approve__5441852A] FOREIGN KEY([ApproverId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Request] CHECK CONSTRAINT [FK__Request__Approve__5441852A]
GO
ALTER TABLE [dbo].[Request]  WITH CHECK ADD  CONSTRAINT [FK__Request__Creator__534D60F1] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Request] CHECK CONSTRAINT [FK__Request__Creator__534D60F1]
GO
ALTER TABLE [dbo].[Request]  WITH CHECK ADD  CONSTRAINT [FK__Request__IssueId__5535A963] FOREIGN KEY([IssueId])
REFERENCES [dbo].[Issue] ([IssueId])
GO
ALTER TABLE [dbo].[Request] CHECK CONSTRAINT [FK__Request__IssueId__5535A963]
GO
ALTER TABLE [dbo].[Request]  WITH CHECK ADD  CONSTRAINT [FK_Request_RequestCategories_RequestCategoryId] FOREIGN KEY([RequestCategoryId])
REFERENCES [dbo].[RequestCategories] ([RequestCategoryId])
GO
ALTER TABLE [dbo].[Request] CHECK CONSTRAINT [FK_Request_RequestCategories_RequestCategoryId]
GO
ALTER TABLE [dbo].[Request]  WITH CHECK ADD  CONSTRAINT [FK_Request_Warehouses_DeliverWarehouseId] FOREIGN KEY([DeliverWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[Request] CHECK CONSTRAINT [FK_Request_Warehouses_DeliverWarehouseId]
GO
ALTER TABLE [dbo].[RequestCable]  WITH CHECK ADD  CONSTRAINT [FK__RequestCa__Cable__59063A47] FOREIGN KEY([CableId])
REFERENCES [dbo].[Cable] ([CableId])
GO
ALTER TABLE [dbo].[RequestCable] CHECK CONSTRAINT [FK__RequestCa__Cable__59063A47]
GO
ALTER TABLE [dbo].[RequestCable]  WITH CHECK ADD  CONSTRAINT [FK__RequestCa__Reque__5812160E] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Request] ([RequestId])
GO
ALTER TABLE [dbo].[RequestCable] CHECK CONSTRAINT [FK__RequestCa__Reque__5812160E]
GO
ALTER TABLE [dbo].[RequestCable]  WITH CHECK ADD  CONSTRAINT [FK_RequestCable_Warehouses_ImportedWarehouseId] FOREIGN KEY([ImportedWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[RequestCable] CHECK CONSTRAINT [FK_RequestCable_Warehouses_ImportedWarehouseId]
GO
ALTER TABLE [dbo].[RequestCable]  WITH CHECK ADD  CONSTRAINT [FK_RequestCable_Warehouses_RecoveryDestWarehouseId] FOREIGN KEY([RecoveryDestWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[RequestCable] CHECK CONSTRAINT [FK_RequestCable_Warehouses_RecoveryDestWarehouseId]
GO
ALTER TABLE [dbo].[RequestOtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK__RequestOt__Other__5CD6CB2B] FOREIGN KEY([OtherMaterialsId])
REFERENCES [dbo].[OtherMaterials] ([OtherMaterialsId])
GO
ALTER TABLE [dbo].[RequestOtherMaterials] CHECK CONSTRAINT [FK__RequestOt__Other__5CD6CB2B]
GO
ALTER TABLE [dbo].[RequestOtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK__RequestOt__Reque__5BE2A6F2] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Request] ([RequestId])
GO
ALTER TABLE [dbo].[RequestOtherMaterials] CHECK CONSTRAINT [FK__RequestOt__Reque__5BE2A6F2]
GO
ALTER TABLE [dbo].[RequestOtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK_RequestOtherMaterials_Warehouses_ImportedWarehouseId] FOREIGN KEY([ImportedWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[RequestOtherMaterials] CHECK CONSTRAINT [FK_RequestOtherMaterials_Warehouses_ImportedWarehouseId]
GO
ALTER TABLE [dbo].[RequestOtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK_RequestOtherMaterials_Warehouses_RecoveryDestWarehouseId] FOREIGN KEY([RecoveryDestWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[RequestOtherMaterials] CHECK CONSTRAINT [FK_RequestOtherMaterials_Warehouses_RecoveryDestWarehouseId]
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD  CONSTRAINT [FK__Supplier__Creato__300424B4] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Supplier] CHECK CONSTRAINT [FK__Supplier__Creato__300424B4]
GO
ALTER TABLE [dbo].[TransactionCable]  WITH CHECK ADD  CONSTRAINT [FK__Transacti__Cable__47DBAE45] FOREIGN KEY([CableId])
REFERENCES [dbo].[Cable] ([CableId])
GO
ALTER TABLE [dbo].[TransactionCable] CHECK CONSTRAINT [FK__Transacti__Cable__47DBAE45]
GO
ALTER TABLE [dbo].[TransactionCable]  WITH CHECK ADD  CONSTRAINT [FK__Transacti__Trans__46E78A0C] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[TransactionHistory] ([TransactionId])
GO
ALTER TABLE [dbo].[TransactionCable] CHECK CONSTRAINT [FK__Transacti__Trans__46E78A0C]
GO
ALTER TABLE [dbo].[TransactionHistory]  WITH CHECK ADD  CONSTRAINT [FK_TransactionHistory_Issue_IssueId] FOREIGN KEY([IssueId])
REFERENCES [dbo].[Issue] ([IssueId])
GO
ALTER TABLE [dbo].[TransactionHistory] CHECK CONSTRAINT [FK_TransactionHistory_Issue_IssueId]
GO
ALTER TABLE [dbo].[TransactionHistory]  WITH CHECK ADD  CONSTRAINT [FK_TransactionHistory_Request_RequestId] FOREIGN KEY([RequestId])
REFERENCES [dbo].[Request] ([RequestId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TransactionHistory] CHECK CONSTRAINT [FK_TransactionHistory_Request_RequestId]
GO
ALTER TABLE [dbo].[TransactionHistory]  WITH CHECK ADD  CONSTRAINT [FK_TransactionHistory_Warehouses_FromWarehouseId] FOREIGN KEY([FromWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[TransactionHistory] CHECK CONSTRAINT [FK_TransactionHistory_Warehouses_FromWarehouseId]
GO
ALTER TABLE [dbo].[TransactionHistory]  WITH CHECK ADD  CONSTRAINT [FK_TransactionHistory_Warehouses_ToWarehouseId] FOREIGN KEY([ToWarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[TransactionHistory] CHECK CONSTRAINT [FK_TransactionHistory_Warehouses_ToWarehouseId]
GO
ALTER TABLE [dbo].[TransactionHistory]  WITH CHECK ADD  CONSTRAINT [FK_TransactionHistory_Warehouses_WarehouseId] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouses] ([WarehouseId])
GO
ALTER TABLE [dbo].[TransactionHistory] CHECK CONSTRAINT [FK_TransactionHistory_Warehouses_WarehouseId]
GO
ALTER TABLE [dbo].[TransactionOtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK__Transacti__Other__4BAC3F29] FOREIGN KEY([OtherMaterialsId])
REFERENCES [dbo].[OtherMaterials] ([OtherMaterialsId])
GO
ALTER TABLE [dbo].[TransactionOtherMaterials] CHECK CONSTRAINT [FK__Transacti__Other__4BAC3F29]
GO
ALTER TABLE [dbo].[TransactionOtherMaterials]  WITH CHECK ADD  CONSTRAINT [FK__Transacti__Trans__4AB81AF0] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[TransactionHistory] ([TransactionId])
GO
ALTER TABLE [dbo].[TransactionOtherMaterials] CHECK CONSTRAINT [FK__Transacti__Trans__4AB81AF0]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK__User__RoleId__276EDEB3] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK__User__RoleId__276EDEB3]
GO
ALTER TABLE [dbo].[Warehouses]  WITH CHECK ADD  CONSTRAINT [FK_Warehouses_User_CreatorId] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[User] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Warehouses] CHECK CONSTRAINT [FK_Warehouses_User_CreatorId]
GO
ALTER TABLE [dbo].[Warehouses]  WITH CHECK ADD  CONSTRAINT [FK_Warehouses_User_WarehouseKeeperid] FOREIGN KEY([WarehouseKeeperid])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Warehouses] CHECK CONSTRAINT [FK_Warehouses_User_WarehouseKeeperid]
GO
USE [master]
GO
ALTER DATABASE [CableManagement] SET  READ_WRITE 
GO

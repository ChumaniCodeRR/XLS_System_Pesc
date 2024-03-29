USE [master]
GO
/****** Object:  Database [JSE_Test]    Script Date: 2024/01/30 19:46:21 ******/
CREATE DATABASE [JSE_Test]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'JSE_Test', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\JSE_Test.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'JSE_Test_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\JSE_Test_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [JSE_Test] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JSE_Test].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [JSE_Test] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [JSE_Test] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [JSE_Test] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [JSE_Test] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [JSE_Test] SET ARITHABORT OFF 
GO
ALTER DATABASE [JSE_Test] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [JSE_Test] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [JSE_Test] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [JSE_Test] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [JSE_Test] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [JSE_Test] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [JSE_Test] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [JSE_Test] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [JSE_Test] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [JSE_Test] SET  DISABLE_BROKER 
GO
ALTER DATABASE [JSE_Test] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [JSE_Test] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [JSE_Test] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [JSE_Test] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [JSE_Test] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [JSE_Test] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [JSE_Test] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [JSE_Test] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [JSE_Test] SET  MULTI_USER 
GO
ALTER DATABASE [JSE_Test] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [JSE_Test] SET DB_CHAINING OFF 
GO
ALTER DATABASE [JSE_Test] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [JSE_Test] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [JSE_Test] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [JSE_Test] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [JSE_Test] SET QUERY_STORE = OFF
GO
USE [JSE_Test]
GO
/****** Object:  Table [dbo].[DailyMTM]    Script Date: 2024/01/30 19:46:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyMTM](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileDate] [date] NULL,
	[Contract] [nvarchar](50) NULL,
	[ExpiryDate] [date] NULL,
	[Classification] [nvarchar](50) NULL,
	[Strike] [float] NULL,
	[CallPut] [nvarchar](50) NULL,
	[MTMYield] [float] NULL,
	[MarkPrice] [float] NULL,
	[SpotRate] [float] NULL,
	[PreviousMTM] [float] NULL,
	[PreviousPrice] [float] NULL,
	[PremiumOnOption] [float] NULL,
	[Volatility] [float] NULL,
	[Delta] [float] NULL,
	[DeltaValue] [float] NULL,
	[ContractsTraded] [float] NULL,
	[OpenInterest] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[SP_Total_Contracts_Traded_Report]    Script Date: 2024/01/30 19:46:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_Total_Contracts_Traded_Report]
@Start_Date date,
@End_Date date
AS
BEGIN
	
		
	    update c
		set c.ContractsTraded = p.ProductCount
		FROM DailyMTM c
		INNER JOIN
		(select FileDate, count(contract) AS ProductCount 
		from DailyMTM 
		GROUP BY FileDate) p
		on c.FileDate = p.FileDate
        
		---Declare the temporary table---
		CREATE TABLE #totalCalculation(
		Id int NOT NULL,
		totalCnt int NOT NULL)
 
		---Copy data into the temporary table---
		  INSERT INTO #totalCalculation
		  SELECT * FROM DailyMTM
		---Select data from the temporary table---
		  SELECT * FROM #totalCalculation

END
GO
USE [master]
GO
ALTER DATABASE [JSE_Test] SET  READ_WRITE 
GO

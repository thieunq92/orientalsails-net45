<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuerySql.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.QuerySql" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%  Module.CommonDao.OpenSession().CreateSQLQuery("SET ANSI_NULLS ON SET QUOTED_IDENTIFIER ON CREATE TABLE [dbo].[os_QuotationPrice]( [Id] [int] IDENTITY(1,1) NOT NULL, [TripId] [int] NULL, [CruiseId] [int] NULL, [RoomClassId] [int] NULL, [RoomTypeId] [int] NULL, [IsCharter] [bit] NULL, [NumberOfPassenger] [int] NULL, [Price] [float] NULL, [QuotationId] [int] NULL, CONSTRAINT [PK_os_QuotationPrice] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] ALTER TABLE [dbo].[os_QuotationPrice] WITH CHECK ADD CONSTRAINT [FK_os_QuotationPrice_os_Quotation] FOREIGN KEY([QuotationId]) REFERENCES [dbo].[os_Quotation] ([Id]) ON UPDATE CASCADE ON DELETE SET NULL ALTER TABLE [dbo].[os_QuotationPrice] CHECK CONSTRAINT [FK_os_QuotationPrice_os_Quotation]")
                .ExecuteUpdate();%>
        </div>
    </form>
</body>
</html>

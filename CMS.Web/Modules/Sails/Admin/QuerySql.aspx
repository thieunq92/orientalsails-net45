<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuerySql.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.QuerySql" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%  Module.CommonDao.OpenSession().CreateSQLQuery("BEGIN TRANSACTION SET QUOTED_IDENTIFIER ON SET ARITHABORT ON SET NUMERIC_ROUNDABORT OFF SET CONCAT_NULL_YIELDS_NULL ON SET ANSI_NULLS ON SET ANSI_PADDING ON SET ANSI_WARNINGS ON COMMIT BEGIN TRANSACTION ALTER TABLE dbo.os_Quotation SET (LOCK_ESCALATION = TABLE) COMMIT select Has_Perms_By_Name(N'dbo.os_Quotation', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.os_Quotation', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.os_Quotation', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION ALTER TABLE dbo.os_Contract SET (LOCK_ESCALATION = TABLE) COMMIT select Has_Perms_By_Name(N'dbo.os_Contract', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.os_Contract', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.os_Contract', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION ALTER TABLE dbo.os_AgencyContract ADD ContractId int NULL, QuotationId int NULL ALTER TABLE dbo.os_AgencyContract ADD CONSTRAINT FK_os_AgencyContract_os_Contract FOREIGN KEY ( ContractId ) REFERENCES dbo.os_Contract ( Id ) ON UPDATE CASCADE ON DELETE SET NULL ALTER TABLE dbo.os_AgencyContract ADD CONSTRAINT FK_os_AgencyContract_os_Quotation FOREIGN KEY ( QuotationId ) REFERENCES dbo.os_Quotation ( Id ) ON UPDATE CASCADE ON DELETE SET NULL ALTER TABLE dbo.os_AgencyContract SET (LOCK_ESCALATION = TABLE) COMMIT select Has_Perms_By_Name(N'dbo.os_AgencyContract', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.os_AgencyContract', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.os_AgencyContract', 'Object', 'CONTROL') as Contr_Per")
                .ExecuteUpdate();%>
        </div>
    </form>
</body>
</html>

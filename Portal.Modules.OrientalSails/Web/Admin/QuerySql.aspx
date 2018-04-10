<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuerySql.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.QuerySql" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%  Module.CommonDao.OpenSession().CreateSQLQuery(" ALTER TABLE os_Agency ADD TradingName nvarchar(MAX),Representative nvarchar(MAX),RepresentativePosition nvarchar(MAX)")
                .ExecuteUpdate();%>
        </div>
    </form>
</body>
</html>

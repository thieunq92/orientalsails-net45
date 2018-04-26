 <%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true"
    CodeBehind="AgencyEdit.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.AgencyEdit" %>

<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls.FileUpload"
    TagPrefix="svc" %>
<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls" TagPrefix="svc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <title>Agency</title>
</asp:Content>
<asp:Content ID="AdminContent" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="container agency-panel">
        <h2 class="page-header">Agency information</h2>
        <div class="row">
            <div class="col-xs-1"></div>
            <div class="col-xs-10">
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Name
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="textBoxName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Trading name
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtTradingName" runat="server" CssClass="form-control" placeholder="Trading name"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Representative
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtRepresentative" runat="server" CssClass="form-control" placeholder="Representative"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Representative Position
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtRepresentativePosition" runat="server" CssClass="form-control" placeholder="Representative position"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Contact
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtContact" runat="server" CssClass="form-control" placeholder="Contact"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Contact Address
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtContactAddress" runat="server" CssClass="form-control" placeholder="Contact address"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Contact Email
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtContactEmail" runat="server" CssClass="form-control" placeholder="Contact email"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Contact Position
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtContactPosition" runat="server" CssClass="form-control" placeholder="Contact position"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Phone
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Contact position"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Fax
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtFax" runat="server" CssClass="form-control" placeholder="Fax"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Location
                        </div>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlLocations" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="--Location--" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Address
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Address"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Email
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Website
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control" placeholder="Website"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Tax code
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtTaxCode" runat="server" CssClass="form-control" placeholder="Tax code"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Role
                        </div>
                        <div class="col-xs-3">
                            <asp:DropDownList runat="server" ID="ddlAgencyRoles" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="--Role--" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Accountant
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtAccountant" runat="server" CssClass="form-control" placeholder="Accountant"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Payment period
                        </div>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlPaymentPeriod" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Value="None" Text="--Payment period--"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                            Other information
                        </div>
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Other information"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                        </div>
                        <div class="col-xs-10">
                            <asp:Literal ID="litCreated" runat="server"></asp:Literal>
                            <asp:Literal ID="litModified" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-2">
                        </div>
                        <div class="col-xs-10">
                            <asp:Button runat="server" ID="buttonSave" CssClass="btn btn-primary"
                                Text="Save" OnClick="buttonSave_Click"></asp:Button>
                        </div>
                    </div>
                </div>

            </div>
            <div class="col-xs-1"></div>
        </div>

        <h2 class="page-header">Sales in charge</h2>
        <div class="row">
            <div class="col-xs-1"></div>
            <div class="col-xs-10">
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-3">
                            Sale in charge
                        </div>
                        <div class="col-xs-3">
                            <asp:DropDownList ID="ddlSales" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="--Sales in charge--" Value="-1"></asp:ListItem>
                            </asp:DropDownList>

                        </div>
                        <div class="col-xs-3">
                            <asp:TextBox ID="txtSaleStart" runat="server" CssClass="form-control" placeholder="Apply from"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-3">
                            Sales in charge history
                        </div>
                        <div class="col-xs-8">
                            <table style="width: 100%;">
                                <asp:Repeater ID="rptHistory" runat="server" OnItemDataBound="rptHistory_ItemDataBound">
                                    <ItemTemplate>
                                        <tr id="trLine" runat="server">
                                            <td>
                                                <asp:Literal ID="litSale" runat="server"></asp:Literal>
                                                apply from
                                                <asp:Literal ID="litSaleStart" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xs-1"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $(document).ready(function () {
            var listToHide = ['Captain', 'Sales', 'Anonymous user', 'Authenticated user', 'Editor', 'Administrator', 'Agent $71', 'Agent $68', 'Agent $69', 'Agent $70', 'Agent $72', 'Agent $73', 'Agent $74', 'Agent $75'];
            for (var i = 0; i < listToHide.length; i++) {
                $('#<%=ddlAgencyRoles.ClientID%> option').each(function (k, v) {
                    if ($(v).text() == listToHide[i]) {
                        $(v).css('display', 'none');
                    }
                });
            }
        })
    </script>
</asp:Content>

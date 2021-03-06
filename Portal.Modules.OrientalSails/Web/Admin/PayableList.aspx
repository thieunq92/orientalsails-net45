<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true"
    CodeBehind="PayableList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.PayableList"
    Title="Payable Report" %>

<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls" TagPrefix="svc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h3>Payable report
        </h3>
    </div>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-1">
                From
            </div>
            <div class="col-xs-2">
                <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" data-control="datetimepicker" placeholder="From (dd/mm/yyyy)"></asp:TextBox>
            </div>
            <div class="col-xs-1">
                To
            </div>
            <div class="col-xs-2">
                <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" data-control="datetimepicker" placeholder="To (dd/mm/yyyy)"></asp:TextBox>
            </div>
            <div class="col-xs-1">
                Supplier
            </div>
            <div class="col-xs-2">
                <asp:DropDownList ID="ddlSupplier" runat="server" CssClass="form-control">
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-12">
                <asp:Button ID="btnDisplay" runat="server" Text="Display" OnClick="btnDisplay_Click"
                    CssClass="btn btn-primary" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <div id="tabs">
                <ul class="nav nav-tabs">
                    <asp:Repeater ID="rptCostTypeTabHeader" runat="server">
                        <ItemTemplate>
                            <li><a href="#tabs-<%# DataBinder.Eval(Container.DataItem, "Id") %>" data-toggle="tab">
                                <%# DataBinder.Eval(Container.DataItem, "Name") %></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <div class="tab-content">
                    <asp:Repeater ID="rptPanel" runat="server" OnItemDataBound="rptPanel_ItemDataBound">
                        <ItemTemplate>
                            <div class="tab-pane fade" id="tabs-<%# DataBinder.Eval(Container.DataItem, "Id") %>">
                                <table class="table table-bordered table-hover table-common">
                                    <tr class="active">
                                        <th>Date
                                        </th>
                                        <th>Partner
                                        </th>
                                        <th>Service
                                        </th>
                                        <th>Total
                                        </th>
                                        <th>Paid
                                        </th>
                                        <th>Payable
                                        </th>
                                        <th></th>
                                    </tr>
                                    <asp:Repeater ID="rptExpenseServices" runat="server" OnItemDataBound="rptExpenseServices_ItemDataBound"
                                        OnItemCommand="rptExpenseServices_ItemCommand">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <asp:HyperLink ID="hplDate" runat="server"></asp:HyperLink>
                                                </td>
                                                <td>
                                                    <asp:HyperLink ID="hplPartner" runat="server"></asp:HyperLink>
                                                </td>
                                                <td>
                                                    <asp:HyperLink ID="hplService" runat="server"></asp:HyperLink>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="litTotal" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="litPaid" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="litPayable" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtPay" runat="server" CssClass="form-control" input-mask="{'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'rightAlign':false}" Text="0"></asp:TextBox>
                                                        <span class="input-group-btn">
                                                            <asp:Button ID="btnPay" runat="server" Text="Pay" CssClass="btn btn-primary" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                        </span>
                                                    </div>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <tr>
                                                <td colspan="3">GRAND TOTAL
                                                </td>
                                                <td>
                                                    <asp:Literal ID="litTotal" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="litPaid" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="litPayable" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnPay" runat="server" Text="Pay all" CssClass="btn btn-primary" OnClientClick="return confirm('Are your sure ?')" />
                                                </td>
                                            </tr>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btn btn-primary" OnClick="btnExport_Click" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $(document).ready(function () {
            $(".tab-pane").first().attr("class", "tab-pane fade in active");
            $("#tabs ul.nav li").first().attr("class", "active");
        })
    </script>
</asp:Content>

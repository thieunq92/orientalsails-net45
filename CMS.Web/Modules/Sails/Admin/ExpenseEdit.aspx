<%@ Page Language="C#" MasterPageFile="SailsMaster.Master" AutoEventWireup="true"
    Codebehind="ExpenseEdit.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.ExpenseEdit"
    Title="Untitled Page" %>

<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls" TagPrefix="svc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <fieldset>
        <legend>
            <img alt="Room" src="../Images/sails.gif" align="absMiddle" />
            Expenses </legend>
        <div class="search_panel">
            <table>
                <tr>
                    <td>
                        From</td>
                    <td>
                        <asp:TextBox ID="txtFrom" runat="server"></asp:TextBox>
                        <ajax:CalendarExtender ID="calendarFrom" runat="server" TargetControlID="txtFrom"
                            Format="dd/MM/yyyy">
                        </ajax:CalendarExtender>
                    </td>
                    <td>
                        To</td>
                    <td>
                        <asp:TextBox ID="txtTo" runat="server"></asp:TextBox>
                        <ajax:CalendarExtender ID="calendarTo" runat="server" TargetControlID="txtTo" Format="dd/MM/yyyy">
                        </ajax:CalendarExtender>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Button ID="btnDisplay" runat="server" Text="Display" OnClick="btnDisplay_Click"
            CssClass="button" />
        <div>
            <ul style="list-style: none; padding: 0px; padding-top: 5px; margin: 0px; margin-top: 10px;"
                class="tabbutton">
                <asp:Repeater ID="rptCruises" runat="server" OnItemDataBound="rptCruises_ItemDataBound">
                    <HeaderTemplate>
                        <li id="liMenu" runat="server">
                            <asp:HyperLink ID="hplCruises" runat="server" Text="All"></asp:HyperLink>
                        </li>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li id="liMenu" runat="server">
                            <asp:HyperLink ID="hplCruises" runat="server"></asp:HyperLink>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div class="basicinfo">
        </div>
        <%--<asp:Button ID="btnCalculate" runat="server" Text="Apply" OnClick="btnCalculate_Click" CssClass="button"/>--%>
        <div class="data_table">
            <div class="data_grid">
                <table>
                    <asp:Repeater ID="rptBookingList" runat="server" OnItemDataBound="rptBookingList_ItemDataBound"
                        OnItemCommand="rptBookingList_ItemCommand">
                        <HeaderTemplate>
                            <tr class="header">
                                <th>
                                    Date
                                </th>
                                <th>
                                    Number of pax
                                </th>
                                <th>
                                    (Tiền xe)
                                </th>
                                <th>
                                    Ticket
                                </th>
                                <th>
                                    Guide
                                </th>
                                <th>
                                    (Tiền ăn)
                                </th>
                                <th>
                                    Kayaking
                                </th>
                                <th>
                                    (Hỗ trợ dịch vụ)
                                </th>
                                <th>
                                    (Tiền tàu)
                                </th>
                                <th>
                                    (Thuê tàu Hải Phong)
                                </th>
                                <th>
                                    Total
                                </th>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="item">
                                <td>
                                    <asp:Literal ID="litDate" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hiddenAdult" runat="server" />
                                    <asp:HiddenField ID="hiddenChild" runat="server" />
                                    <asp:HiddenField ID="hiddenBaby" runat="server" />
                                </td>
                                <td>
                                    <asp:Literal ID="litPax" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:TextBox ID="txtTransfer" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtTicket" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtGuide" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtMeal" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtKayaing" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtServiceSup" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtCruise" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox ID="txtRent" runat="server" Width="100"></asp:TextBox></td>
                                <td>
                                    <asp:Literal ID="litTotal" runat="server"></asp:Literal></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr>
                                <td>
                                    TOTAL</td>
                                <td>
                                    <asp:Literal ID="litPax" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litTransfer" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litTicket" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litGuide" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litMeal" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litKayaing" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litServiceSup" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litCruise" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litRent" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="litTotal" runat="server"></asp:Literal></td>
                            </tr>
                        </FooterTemplate>
                    </asp:Repeater>
                </table>
            </div>
            <%--            <div class="pager">
            <svc:Pager ID="pagerOrders" runat="server" ControlToPage="rptBookingList" OnPageChanged="pagerOrders_PageChanged" PageSize="50" />
            </div>--%>
        </div>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="button" />
    </fieldset>
</asp:Content>

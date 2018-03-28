<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true"
    CodeBehind="AgencyView.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.AgencyView"
    Title="Agency View" %>

<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls" TagPrefix="svc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <svc:Popup ID="popupManager" runat="server">
    </svc:Popup>
    <h2 class="page-header">
        <asp:Literal runat="server" ID="litName1"></asp:Literal></h2>
    <div class="panel-agency">
        <div class="row">
            <div class="col-xs-1">
                <label>Name</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litName"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Tên giao dịch</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litTenTiengViet"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Người đại diện</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litGiamDoc"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Role</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litRole"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Address</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litAddress"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Phone</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litPhone"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Fax</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litFax"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Email</label>
            </div>
            <div class="col-xs-11">
                <asp:HyperLink runat="server" ID="hplEmail"></asp:HyperLink>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Sale in charge</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litSale"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Sale phone</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litSalePhone"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Tax code</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litTax"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Location</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litLocation"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Accountant</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litAccountant"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Payment</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litPayment"></asp:Literal>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-1">
                <label>Other info</label>
            </div>
            <div class="col-xs-11">
                <asp:Literal runat="server" ID="litNote"></asp:Literal>
            </div>
        </div>
        <div class="btn-toolbar">
            <asp:HyperLink runat="server" ID="hplEditAgency" CssClass="btn btn-primary">Edit agency</asp:HyperLink>
            <asp:HyperLink runat="server" ID="hplBookingList" CssClass="btn btn-primary">Booking by agency</asp:HyperLink>
            <div id="disableInform" style="display: none">
                You don't have permission to use this function. If you want to use this function please contact administrator
            </div>
            <asp:HyperLink runat="server" ID="hplReceivable"
                CssClass="btn btn-primary">Receivables (last 3 months)</asp:HyperLink>
            <a href="SeriesManager.aspx?NodeId=1&SectionId=15&ai=<%= Request.QueryString["AgencyId"] %>" class="btn btn-primary">Series Booking</a>
        </div>
    </div>
    <div class="panel-contacts">
        <asp:PlaceHolder runat="server" ID="plhContacts">
            <h4>CONTACTS</h4>
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th>Name
                    </th>
                    <th>Position
                    </th>
                    <th>Booker
                    </th>
                    <th>Phone
                    </th>
                    <th>Email
                    </th>
                    <th>Birthday
                    </th>
                    <th>Note
                    </th>
                    <th></th>
                    <th></th>
                    <th></th>
                </tr>
                <asp:Repeater runat="server" ID="rptContacts" OnItemDataBound="rptContacts_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="ltrName"></asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="litPosition"></asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="litBooker"></asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="litPhone"></asp:Literal>
                            </td>
                            <td>
                                <asp:HyperLink runat="server" ID="hplEmail"></asp:HyperLink>
                            </td>
                            <td>
                                <%# ((DateTime?)Eval("Birthday"))==null?"" : ((DateTime?)Eval("Birthday")).Value.ToString("dd/MM/yyyy")%>
                            </td>
                            <td>
                                <%# DataBinder.Eval(Container.DataItem, "Note") %>
                            </td>
                            <td>
                                <asp:HyperLink runat="server" ID="hplCreateMeeting"><i class="fa fa-users fa-lg text-success" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="Add meeting"></i></asp:HyperLink>
                            </td>
                            <td>
                                <asp:HyperLink runat="server" ID="hplName"><i class="fa fa-pencil-square-o fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="Edit"></i></asp:HyperLink>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="lbtDelete" OnClick="lbtDelete_Click" CommandArgument='<%#Eval("Id")%>'
                                    OnClientClick="return confirm('Are you sure?')"><i class="fa fa-close fa-lg text-danger" aria-hidden="true" title="" data-toggle="tooltip" data-placement="top" data-original-title="Delete"></i></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <div class="btn-toolbar">
                <asp:HyperLink runat="server" ID="hplAddContact" CssClass="btn btn-primary">Add contact</asp:HyperLink>
            </div>
        </asp:PlaceHolder>
        <asp:Label runat="server" ID="lblContacts" Text="You don't have permission to use this function. If you want to use this function please contact administrator"
            Visible="False" />
    </div>
    <div class="panel-recentactivities">
        <asp:PlaceHolder runat="server" ID="plhActivities">
            <h4>RECENT ACTIVITIES
            </h4>
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th>Date meeting
                    </th>
                    <th>Sale
                    </th>
                    <th>Meeting with
                    </th>
                    <th>Position
                    </th>
                    <th>Note
                    </th>
                    <th>Edit
                    </th>
                    <th>Delete
                    </th>
                </tr>
                <asp:Repeater runat="server" ID="rptActivities" OnItemDataBound="rptActivities_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="ltrDateMeeting" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ltrSale" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ltrName" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ltrPosition" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="ltrNote" />
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="lbtEditActivity"><i class="fa fa-pencil-square-o fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="Edit"></i></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="lbtDeleteActivity" OnClick="lbtDeleteActivity_Click"
                                    CommandArgument='<%#Eval("Id")%>' OnClientClick="return confirm('Are you sure?')"><i class="fa fa-close fa-lg text-danger" aria-hidden="true" title="" data-toggle="tooltip" data-placement="top" data-original-title="Delete"></i></asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </asp:PlaceHolder>
        <asp:Label runat="server" ID="lblActivities" Text="You don't have permission to use this function. If you want to use this function please contact administrator"
            Visible="False" />
    </div>
    <div class="panel-contracts">
        <h4>CONTRACTS</h4>
        <asp:PlaceHolder runat="server" ID="plhContracts">
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th>Created Date</th>
                    <th>Name
                    </th>
                    <th>Expired on
                    </th>
                    <th>Received</th>
                    <th>Download
                    </th>
                    <th>Edit
                    </th>
                </tr>
                <asp:Repeater runat="server" ID="rptContracts" OnItemDataBound="rptContracts_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="litCreatedDate" />
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="litName"></asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="litExpired"></asp:Literal>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="litReceived"></asp:Literal>
                            </td>
                            <td>
                                <asp:HyperLink runat="server" ID="hplDownload" ToolTip="Download"></asp:HyperLink>
                            </td>
                            <td>
                                <asp:HyperLink runat="server" ID="hplEdit"><i class="fa fa-pencil-square-o fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="Edit"></i></asp:HyperLink>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <div class="btn-toolbar">
                <button type="button" class="btn btn-primary" data-toggle="modal" data-target=".modal-issuecontract">Issue contract</button>
            </div>
        </asp:PlaceHolder>
        <asp:Label runat="server" ID="lblContracts" Text="You don't have permission to use this function. If you want to use this function please contact administrator"
            Visible="False" />
    </div>
    <div class="modal fade modal-issuecontract" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h3 class="modal-title" id="gridSystemModalLabel">Issue Contract</h3>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-1">
                                <label>Name</label>
                            </div>
                            <div class="col-xs-11">
                                <input type="text" class="form-control" placeholder="Name" />
                            </div>

                        </div>
                    </div>

                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-2">
                                <label>Valid from</label>
                            </div>
                            <div class="col-xs-4">
                                <input type="text" class="form-control" placeholder="Valid from (dd/mm/yyyy)" data-type="datetimepicker" />
                            </div>
                            <div class="col-xs-2">
                                <label>Valid to</label>
                            </div>
                            <div class="col-xs-4">
                                <input type="text" class="form-control" placeholder="Valid to (dd/mm/yyyy)" data-type="datetimepicker" />
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-3">
                                <label>Select quotation</label>
                            </div>
                            <div class="col-xs-9">
                                <select class="form-control">
                                    <option>Quotation lv1</option>
                                    <option>Quotation lv2</option>
                                    <option>Quotation lv3</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-1">
                                <label>Upload</label>
                            </div>
                            <div class="col-xs-11">
                                <input type="text" class="form-control" placeholder="File upload"></input>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-1">
                                <label>Preview</label>
                            </div>
                            <div class="col-xs-11">
                                <div class="btn-toolbar">
                                    <button type="button" class="btn btn-primary">Send email</button>
                                    <asp:Button runat ="server" ID ="btnExportContractPreviewWord" class="btn btn-primary" Text="Export to Word" OnClick="btnExportContractPreviewWord_Click"></asp:Button>
                                    <asp:Button runat ="server" ID ="btnExportContractPreviewPdf" class="btn btn-primary" Text="Export to Word" OnClick="btnExportContractPreviewPdf_Click"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary">Issue</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript">
        $("[data-type='datetimepicker']").datetimepicker({
            timepicker: false,
            format: 'd/m/Y',
            scrollInput: false,
            scrollMonth: false
        });
    </script>
</asp:Content>

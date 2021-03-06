using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.Core.Util;
using CMS.ServerControls;
using CMS.Web.Admin.Controls;
using CMS.Web.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class OrderReport : SailsAdminBase
    {
        #region -- PRIVATE MEMBERS --

        private readonly ILog _logger = LogManager.GetLogger(typeof(OrderReport));
        protected UserSelector bookedUser;

        #endregion

        #region -- PAGE EVENTS --

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                pagerOrders.AllowCustomPaging = true;
                //Response.AppendHeader("Refresh", Convert.ToString(60));

                const string centerPopup =
@"function PopupCenter(pageURL, title,h,w) {
var left = (screen.width/2)-(w/2);
var top = (screen.height/2)-(h/2);
var targetWin = window.open (pageURL, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width='+w+', height='+h+', top='+top+', left='+left);
targetWin.focus();
return targetWin;
}";
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "centerPopup", centerPopup, true);

                if (!IsPostBack)
                {
                    #region -- Load info --

                    BindTrips();
                    LoadData();

                    #endregion
                    GetDataSource();
                    rptBookingList.DataBind();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error when Page_load in BookingList", ex);
                ShowError(ex.Message);
            }
        }

        #endregion

        #region -- CONTROL EVENTS --

        protected void rptBookingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                RepeaterOrder.FILE_NAME = "OrderReport.aspx";
                RepeaterOrder.SetOrderLink(e, "Agency", Request.QueryString);
                RepeaterOrder.SetOrderLink(e, "Trip", Request.QueryString);
                RepeaterOrder.SetOrderLink(e, "StartDate", Request.QueryString);
            }

            if (e.Item.DataItem is Booking)
            {
                Booking booking = (Booking)e.Item.DataItem;

                bool isDisabled = false;

                var hplCode = e.Item.FindControl("hplCode") as HyperLink;
                if (hplCode != null)
                {
                    hplCode.Text = string.Format(BookingFormat, booking.Id);
                    hplCode.NavigateUrl = string.Format("BookingView.aspx?NodeId={0}&SectionId={1}&bi={2}",
                                                            Node.Id, Section.Id, booking.Id);
                }

                #region -- Date --

                Label labelDate = e.Item.FindControl("labelDate") as Label;
                if (labelDate != null)
                {
                    labelDate.Text = booking.StartDate.ToString("dd/MM/yyyy");
                }

                #endregion

                #region -- Trip --

                Label labelTrip = e.Item.FindControl("labelTrip") as Label;
                if (labelTrip != null)
                {
                    labelTrip.Text = booking.Trip.Name;
                    if (booking.Trip.NumberOfOptions > 1)
                    {
                        labelTrip.Text += string.Format("({0})", booking.TripOption);
                    }
                }

                #endregion

                #region -- Partner --

                Label labelPartner = e.Item.FindControl("labelPartner") as Label;
                if (labelPartner != null)
                {
                    if (booking.Agency != null)
                    {
                        labelPartner.Text = booking.Agency.Name;
                    }
                    else
                    {
                        labelPartner.Text = SailsModule.NOAGENCY;
                    }
                }

                if (booking.Agency != null && booking.Agency.Sale != null)
                    ValueBinder.BindLiteral(e.Item, "litSale", booking.Agency.Sale.AllName);

                #endregion

                #region -- Rooms --

                Label labelRoom = e.Item.FindControl("labelRoom") as Label;
                if (labelRoom != null)
                {
                    try
                    {
                        Dictionary<string, int> rooms = new Dictionary<string, int>();
                        Dictionary<string, int> roomAvaiable = new Dictionary<string, int>();
                        // Đối với mỗi booking đã đặt
                        foreach (BookingRoom room in booking.BookingRooms)
                        {
                            // Lấy về loại phòng
                            string key = room.RoomClass.Name + " " + room.RoomType.Name;
                            if (room.RoomType.IsShared)
                            {
                                key += " spaces";
                            }

                            if (rooms.ContainsKey(key))
                            {
                                // Nếu đã có trong từ điển thì cộng thêm
                                if (room.RoomType.IsShared)
                                {
                                    rooms[key] += 1;
                                }
                                else
                                {
                                    rooms[key] += room.VirtualAdult;
                                }
                            }
                            else
                            {
                                // Nếu chưa có trong từ điển thì thêm vào
                                if (room.RoomType.IsShared)
                                {
                                    rooms.Add(key, 1);
                                }
                                else
                                {
                                    rooms.Add(key, room.VirtualAdult);
                                }
                            }

                            // Nếu thông tin về loại phòng này chưa có trong từ điển
                            if (!roomAvaiable.ContainsKey(key))
                            {
                                // Hoàn toàn phụ thuộc vào số chỗ trống
                                roomAvaiable.Add(key, Module.RoomCount(room.RoomClass, room.RoomType, booking.Cruise, booking.StartDate, booking.Trip.NumberOfDay, booking));
                            }
                        }

                        //TODO: Recheck
                        // Kiểm tra với từng loại phòng, nếu có 1 loại ko đủ chỗ trống thì loại
                        foreach (KeyValuePair<string, int> entry in rooms)
                        {
                            labelRoom.Text += entry.Value + @" " + entry.Key + @"<br/>";

                            //if (Convert.ToInt32(entry.Value) > Convert.ToInt32(roomAvaiable[entry.Key]))
                            //{
                            //    isDisabled = true;
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }
                }

                #endregion

                var labelNumberOfPax = e.Item.FindControl("labelNumberOfPax") as Label;
                if (labelNumberOfPax != null)
                {
                    labelNumberOfPax.Text = string.Format(Resources.formatCustomerCount, booking.Adult, booking.Child,
                                                          booking.Baby);
                }

                var lbtApprove = e.Item.FindControl("lbtApprove") as LinkButton;
                if (lbtApprove != null)
                {
                    // Lấy lại giá trị đánh dấu số phòng có đủ hay không
                    if (!isDisabled)
                    {
                        Control chkNofity = e.Item.FindControl("chkNofity");
                        string popupurl = string.Format("SendEmail.aspx?NodeId={0}&SectionId={1}&BookId={2}&status=1",
                                                        Node.Id, Section.Id, booking.Id);
                        lbtApprove.OnClientClick = "if (document.getElementById('" + chkNofity.ClientID + "').checked == 1) PopupCenter('" + popupurl + "','Send email',800,600)";
                    }
                    else
                    {
                        lbtApprove.Enabled = false;
                        lbtApprove.Text = Resources.textStatusNotAvaiable;
                        lbtApprove.Attributes.Add("style", "color:" + SailsModule.IMPORTANT);
                    }
                }

                LinkButton lbtCancel = e.Item.FindControl("lbtCancel") as LinkButton;
                if (lbtCancel != null)
                {
                    Control chkNofity = e.Item.FindControl("chkNofity");
                    string popupurl = string.Format("SendEmail.aspx?NodeId={0}&SectionId={1}&BookId={2}&status=2", Node.Id, Section.Id, booking.Id);
                    lbtCancel.OnClientClick = "if (document.getElementById('" + chkNofity.ClientID + "').checked == 1) PopupCenter('" + popupurl + "','Send email',800,600)";
                }

                if (!string.IsNullOrEmpty(booking.Agency.Phone))
                    ValueBinder.BindLiteral(e.Item, "litPhone", "(" + booking.Agency.Phone + ")");
                if (booking.Deadline.HasValue)
                {
                    ValueBinder.BindLiteral(e.Item, "litPendingUntil", booking.Deadline.Value.ToString("HH:mm dd/MM/yyyy"));
                }
            }
        }

        protected void rptBookingList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "approve":
                    Booking approved = Module.BookingGetById(Convert.ToInt32(e.CommandArgument));
                    approved.Status = StatusType.Approved;
                    approved.ConfirmedBy = Page.User.Identity as User;
                    Module.Save(approved, UserIdentity);
                    GetDataSource();
                    rptBookingList.DataBind();
                    break;
                case "cancel":
                    Booking cancelled = Module.BookingGetById(Convert.ToInt32(e.CommandArgument));
                    cancelled.Status = StatusType.Cancelled;
                    cancelled.IsApproved = false;
                    cancelled.ConfirmedBy = Page.User.Identity as User;
                    Module.Save(cancelled, UserIdentity);
                    GetDataSource();
                    rptBookingList.DataBind();
                    break;
            }
        }

        protected void pagerOrders_PageChanged(object sender, PageChangedEventArgs e)
        {
            GetDataSource();
            rptBookingList.DataBind();
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            iTextSharpHelper.ExportToPdf(WriteReport, Response);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = BuildQueryString();
            PageRedirect(string.Format("OrderReport.aspx?NodeId={0}&SectionId={1}{2}", Node.Id, Section.Id, query));
        }

        #endregion

        #region -- METHODS --

        protected virtual void GetDataSource()
        {
            // Điều kiện bắt buộc: chưa xóa và đang pending
            //ICriterion criterion = Expression.Eq(Booking.DELETED, false);
            //criterion = Expression.And(criterion, Expression.Eq(Booking.STATUS, StatusType.Pending));
            var criterion = Expression.And(Expression.Eq("Status", StatusType.Pending),
                                           Expression.Ge("Deadline", DateTime.Now));
            criterion = Expression.And(criterion, Expression.Eq("Deleted", false));

            int count;

            // Điều kiện từ query string
            if (Request.QueryString["UserId"] != null)
            {
                User user = Module.UserGetById(Convert.ToInt32(Request.QueryString["UserId"]));
                criterion = Expression.And(criterion, Expression.Eq("CreatedBy", user));

                // và mình là sale in charge
                criterion = Expression.And(criterion, Expression.Eq("agency.Sale", UserIdentity));
            }
            else // nếu không có user thì lấy create by me OR sale in charge
            {
                if (Request.QueryString["mode"] != "all")
                    criterion = Expression.And(criterion,
                                               Expression.Or(Expression.Eq("CreatedBy", UserIdentity),
                                                             Expression.Eq("agency.Sale", UserIdentity)));

            }

            if (Request.QueryString["TripId"] != null)
            {
                SailsTrip trip = Module.TripGetById(Convert.ToInt32(Request.QueryString["TripId"]));
                criterion = Expression.And(criterion, Expression.Eq("Trip", trip));
            }

            if (Request.QueryString["Date"] != null)
            {
                DateTime date = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["date"]));
                criterion = Expression.And(criterion, Expression.Eq("StartDate", date.Date));
            }

            Order order = RepeaterOrder.GetOrderFromQueryString(Request.QueryString);
            if (order == null)
            {
                order = Order.Asc("Deadline");
            }

            rptBookingList.DataSource = Module.BookingGetByCriterion(criterion, order, out count, pagerOrders.PageSize,
                                                                     pagerOrders.CurrentPageIndex);
            pagerOrders.VirtualItemCount = count;
        }

        private void WriteReport(Document document)
        {
            document.Open();
            document.SetMargins(2, 2, 2, 2);

            // Tạo table và gán các thuộc tính mặc định
            Font defaultFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12);
            PdfPTable table = new PdfPTable(5);
            table.DefaultCell.Padding = 2;
            table.WidthPercentage = 100;
            table.SetWidths(new int[] { 15, 30, 15, 30, 10 });

            #region -- Header --
            PdfPCell dateCell = new PdfPCell(new Phrase("Date", defaultFont));
            dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(dateCell);

            PdfPCell tripCell = new PdfPCell(new Phrase("Trip", defaultFont));
            tripCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(tripCell);

            PdfPCell partnerCell = new PdfPCell(new Phrase("Partner", defaultFont));
            partnerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(partnerCell);

            PdfPCell roomCell = new PdfPCell(new Phrase("Room", defaultFont));
            roomCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(roomCell);

            PdfPCell cell = new PdfPCell(new Phrase("Pax", defaultFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
            #endregion

            foreach (RepeaterItem item in rptBookingList.Items)
            {
                #region -- Cell Date --

                Label labelDate = item.FindControl("labelDate") as Label;
                if (labelDate != null)
                {
                    table.AddCell(new Phrase(labelDate.Text, defaultFont));
                }
                else
                {
                    table.AddCell("");
                }

                #endregion

                #region -- Cell Trip --

                Label labelTrip = item.FindControl("labelTrip") as Label;
                if (labelTrip != null)
                {
                    table.AddCell(new Phrase(labelTrip.Text, defaultFont));
                }
                else
                {
                    table.AddCell("");
                }

                #endregion

                #region -- Cell Partner --

                Label labelPartner = item.FindControl("labelPartner") as Label;
                if (labelPartner != null)
                {
                    table.AddCell(new Phrase(labelPartner.Text, defaultFont));
                }
                else
                {
                    table.AddCell("");
                }

                #endregion

                #region -- Cell Room --

                Label labelRoom = item.FindControl("labelRoom") as Label;
                if (labelRoom != null)
                {
                    table.AddCell(new Phrase(labelRoom.Text.Replace("<br/>", "\r\n"), defaultFont));
                }
                else
                {
                    table.AddCell("");
                }

                #endregion

                #region -- Cell Pax --

                Label labelNumberOfPax = item.FindControl("labelNumberOfPax") as Label;
                if (labelNumberOfPax != null)
                {
                    table.AddCell(new Phrase(labelNumberOfPax.Text, defaultFont));
                }
                else
                {
                    table.AddCell("");
                }

                #endregion
            }

            document.Add(table);
        }

        private string BuildQueryString()
        {
            string query = string.Empty;
            if (bookedUser.SelectedUserId > 0)
            {
                query += "&UserId=" + bookedUser.SelectedUserId;
            }
            if (ddlTrips.SelectedIndex > 0)
            {
                query += "&TripId=" + ddlTrips.SelectedValue;
            }
            if (!string.IsNullOrEmpty(txtDate.Text))
            {
                DateTime date;
                if (DateTime.TryParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                                           out date))
                {
                    query += "&Date=" + date.ToOADate().ToString("####");
                }
            }
            return query;
        }

        private void BindTrips()
        {
            ddlTrips.DataSource = Module.TripGetAll(true);
            ddlTrips.DataValueField = "Id";
            ddlTrips.DataTextField = "Name";
            ddlTrips.DataBind();
            ddlTrips.Items.Insert(0, new ListItem("-- Trips --", "0"));
        }

        private void LoadData()
        {
            if (Request.QueryString["UserId"] != null)
            {
                User user = Module.UserGetById(Convert.ToInt32(Request.QueryString["UserId"]));
                bookedUser.SelectedUserId = user.Id;
                bookedUser.SelectedUser = user.UserName;
            }

            if (Request.QueryString["TripId"] != null)
            {
                ddlTrips.SelectedValue = Request.QueryString["TripId"];
            }

            if (Request.QueryString["Date"] != null)
            {
                DateTime date = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["date"]));
                txtDate.Text = date.ToString("dd/MM/yyyy");
            }
        }

        #endregion
    }
}
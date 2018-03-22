﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aspose.Words;
using Aspose.Words.Tables;
using CMS.Core.Domain;
using CMS.ServerControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Document = Aspose.Words.Document;
using Image = System.Drawing.Image;
using ListItem = System.Web.UI.WebControls.ListItem;
using Table = Aspose.Words.Tables.Table;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class ViewMeetings : SailsAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindBackDateFromToControl();
            BindBackDateToToControl();
            BindRptMeetings();
            BindSalesToControl();
            BindBackSalesToControl();
            BindBackPageSizeToControl();
            BindSortStatus();
            DisplaySalesControl();
            DisplaySalesColumn();
        }

        private void InsertTableActivityToDocument(DocumentBuilder builder, bool needInsertSalesHeader,
            string dateMeeting, string sales, string meetingWith, string position, string belongToAgency, string note)
        {
            if (IsUserAdministrator())
            {
                InsertHeader(builder, needInsertSalesHeader, sales);
            }
            builder.StartTable();
            InsertRow(builder, dateMeeting, meetingWith, position, belongToAgency, note);
            builder.EndTable();
        }

        public void InsertHeader(DocumentBuilder builder, bool needInsertSalesHeader, string sales)
        {
            var font = builder.Font;
            font.Bold = true;
            font.Size = 16;
            var paragraph = builder.ParagraphFormat;
            paragraph.Alignment = ParagraphAlignment.Center;

            if (needInsertSalesHeader)
            {
                builder.Writeln(string.Format("Sales {0} meetings from {1} to {2}", sales, txtFrom.Text, txtTo.Text));
                builder.Writeln("");
            }
        }

        public void InsertRow(DocumentBuilder builder, string dateMeeting, string meetingWith, string position,
            string belongToAgency, string note)
        {
            var font = builder.Font;
            var paragraph = builder.ParagraphFormat;
            font.Size = 12;
            paragraph = builder.ParagraphFormat;
            paragraph.Alignment = ParagraphAlignment.Left;
            builder.CellFormat.Width = 80;
            builder.InsertCell().CellFormat.HorizontalMerge = CellMerge.None;
            if (dateMeeting != null)
            {
                builder.Writeln(dateMeeting);
            }

            builder.InsertCell();
            if (meetingWith != null)
            {
                builder.Writeln(meetingWith);
            }

            builder.InsertCell();
            if (position != null)
            {
                builder.Writeln(position);
            }

            font.Size = 10;
            builder.InsertCell().CellFormat.FitText = true;

            if (belongToAgency != null)
            {
                builder.Writeln(belongToAgency);
            }
            builder.CellFormat.Width = 200;
            builder.EndRow();

            font.Bold = false;
            font.Size = 12;

            builder.InsertCell().CellFormat.HorizontalMerge = CellMerge.First;
            if (note != null)
            {
                builder.Writeln(note);
            }
            builder.InsertCell().CellFormat.HorizontalMerge = CellMerge.Previous;
            builder.Writeln("");

            builder.InsertCell().CellFormat.HorizontalMerge = CellMerge.Previous;
            builder.Writeln("");

            builder.InsertCell().CellFormat.HorizontalMerge = CellMerge.Previous;
            builder.Writeln("");
            builder.EndRow();
        }

        public bool IsUserAdministrator()
        {
            if (UserIdentity.HasPermission(AccessLevel.Administrator))
            {
                return true;
            }
            return false;
        }

        public DateTime? GetDateFrom()
        {
            try
            {
                return DateTime.ParseExact(Request.Form[txtFrom.UniqueID], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public DateTime? GetDateTo()
        {
            try
            {
                return DateTime.ParseExact(Request.Form[txtTo.UniqueID], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public DateTime? GetDateFromOnQueryString()
        {
            try
            {
                var dateFrom = Request.QueryString["datefrom"];
                if (!String.IsNullOrEmpty(dateFrom))
                    return DateTime.ParseExact(dateFrom, "ddMMyyyy", CultureInfo.InvariantCulture);
                return GetFirstDateOfMonth();
            }
            catch
            {
                return GetFirstDateOfMonth();
            }
        }

        public DateTime? GetDateToOnQueryString()
        {
            try
            {
                var dateTo = Request.QueryString["dateto"];
                if (!String.IsNullOrEmpty(dateTo))
                    return DateTime.ParseExact(dateTo, "ddMMyyyy", CultureInfo.InvariantCulture);
                return GetLastDateOfMonth();
            }
            catch
            {
                return GetLastDateOfMonth();
            }
        }

        public string GetSales()
        {
            return Request.Form[ddlSales.UniqueID];
        }

        public User GetSalesOnQueryString()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["sales"]))
                {
                    var salesId = Int32.Parse(Request.QueryString["sales"]);
                    if (salesId == -1)
                        return null;
                    return Module.GetObject<User>(salesId);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public int GetPageSize()
        {
            const int defaultPageSize = 20;
            try
            {
                if (!String.IsNullOrEmpty(txtPageSize.UniqueID))
                {
                    return Int32.Parse(Request.Form[txtPageSize.UniqueID]);
                }
                return defaultPageSize;
            }
            catch
            {
                return defaultPageSize;
            }
        }

        public int GetPageSizeOnQueryString()
        {
            const int defaultPageSize = 20;
            var pageSize = Request.QueryString["pagesize"];
            try
            {
                if (!String.IsNullOrEmpty(pageSize))
                {
                    return Int32.Parse(pageSize);
                }
                return defaultPageSize;
            }
            catch
            {
                return defaultPageSize;
            }
        }

        public int GetSortUpDateTimeOnQueryString()
        {
            const int nosort = -1;
            try
            {
                if (!(String.IsNullOrEmpty(Request.QueryString["sortut"])))
                {
                    return Int32.Parse(Request.QueryString["sortut"]);
                }
                return nosort;
            }
            catch
            {
                return nosort;
            }
        }

        public int GetSortDateMeetingOnQueryString()
        {
            const int nosort = -1;
            try
            {
                if (!(String.IsNullOrEmpty(Request.QueryString["sortdm"])))
                {
                    return Int32.Parse(Request.QueryString["sortdm"]);
                }
                return nosort;
            }
            catch
            {
                return nosort;
            }
        }

        public IList<User> GetSalesListInActivity(bool salesFilter)
        {
            int count;
            IList<Activity> activityList = GetActivities(0, 0, out count, salesFilter);
            IList<User> salesList = new List<User>();

            for (int i = 0; i < activityList.Count; i++)
            {
                var activity = activityList[i] as Activity;
                var sales = activity.User;
                if (salesList.Contains(sales))
                {
                    continue;
                }
                salesList.Add(sales);
            }

            return salesList;
        }

        public IList<Activity> GetActivities(int pageSize, int pageIndex, out int count, bool salesFilter)
        {
            var dateFrom = GetDateFromOnQueryString();
            var dateTo = GetDateToOnQueryString();
            var sortut = GetSortUpDateTimeOnQueryString();
            var sortdm = GetSortDateMeetingOnQueryString();
            var finalCriterion = Expression.Ge("Id", 0) as ICriterion;
            var sales = GetSalesOnQueryString();
            ICriterion salesCriterion;

            if (dateFrom != null)
            {
                var ts = new TimeSpan(0, 0, 0);
                ICriterion dateFromCriterion = Expression.Ge("DateMeeting", dateFrom.Value.Date + ts);
                finalCriterion = Expression.And(finalCriterion, dateFromCriterion);
            }
            if (dateTo != null)
            {
                var ts = new TimeSpan(23, 59, 59);
                ICriterion dateToCriterion = Expression.Le("DateMeeting", dateTo.Value.Date + ts);
                finalCriterion = Expression.And(finalCriterion, dateToCriterion);
            }


            //if (IsUserAdministrator())
            //{
            if (salesFilter == true)
            {
                if (sales != null)
                {
                    salesCriterion = Expression.Eq("User", sales);
                    finalCriterion = Expression.And(finalCriterion, salesCriterion);
                }
            }
            //}
            //else
            //{
            //    salesCriterion = Expression.Eq("User", UserIdentity);
            //    finalCriterion = Expression.And(finalCriterion, salesCriterion);
            //}

            count = Module.CountObjet<Activity>(finalCriterion);

            const int nosort = -1;
            const int descendingstatus = 0;
            const int ascendingstatus = 1;
            const bool descending = false;
            const bool ascending = true;

            if (sortdm != nosort)
            {
                if (sortdm == descendingstatus)
                {
                    return Module.GetObject<Activity>(finalCriterion, pageSize, pageIndex, new Order("DateMeeting", descending));
                }
                return Module.GetObject<Activity>(finalCriterion, pageSize, pageIndex, new Order("DateMeeting", ascending));
            }

            if (sortut != nosort)
            {
                if (sortut == descendingstatus)
                {
                    return Module.GetObject<Activity>(finalCriterion, pageSize, pageIndex, new Order("UpdateTime", descending));
                }
                return Module.GetObject<Activity>(finalCriterion, pageSize, pageIndex, new Order("UpdateTime", ascending));
            }

            return Module.GetObject<Activity>(finalCriterion, pageSize, pageIndex, new Order("DateMeeting", descending));
        }

        public void BindBackDateFromToControl()
        {
            var dateFrom = GetDateFromOnQueryString();
            if (dateFrom != null)
                txtFrom.Text = dateFrom.Value.ToString("dd/MM/yyyy");
            else
            {
                txtFrom.Text = GetFirstDateOfMonth().ToString("dd/MM/yyyy");
            }

        }

        public void BindBackDateToToControl()
        {
            var dateTo = GetDateToOnQueryString();
            if (dateTo != null)
                txtTo.Text = dateTo.Value.ToString("dd/MM/yyyy");
            else
            {
                txtTo.Text = GetLastDateOfMonth().ToString("dd/MM/yyyy");
            }
        }

        public void BindBackSalesToControl()
        {
            var sales = GetSalesOnQueryString();
            if (sales != null)
            {
                ddlSales.SelectedValue = sales.Id.ToString();
            }
        }

        public void BindBackPageSizeToControl()
        {
            var pageSize = GetPageSizeOnQueryString();
            txtPageSize.Text = pageSize.ToString();
        }

        public void BindSalesToControl()
        {
            ddlSales.Items.Clear();
            ddlSales.Items.Insert(0, new ListItem("-- Sales --", "-1"));
            var salesList = GetSalesListInActivity(false);
            for (int i = 0; i < salesList.Count; i++)
            {
                var id = salesList[i].Id.ToString();
                var name = salesList[i].FullName;
                ddlSales.Items.Add(new ListItem(name, id));
            }
        }

        public void BindRptMeetings()
        {
            pagerMeetings.AllowCustomPaging = true;
            int count;
            var pageSize = GetPageSizeOnQueryString();

            pagerMeetings.PageSize = pageSize;
            rptMeetings.DataSource = GetActivities(pagerMeetings.PageSize, pagerMeetings.CurrentPageIndex, out count, true);
            pagerMeetings.VirtualItemCount = count;
            rptMeetings.DataBind();
        }

        /// <summary>
        /// gắn ảnh chỉ ra trạng thái sắp xếp vào nhãn cần sắp xếp
        /// </summary>
        public void BindSortStatus()
        {
            var imgSortUtStatus = rptMeetings.Controls[0].Controls[0].FindControl("imgSortUtStatus") as System.Web.UI.WebControls.Image;
            var imgSortDmStatus = rptMeetings.Controls[0].Controls[0].FindControl("imgSortDmStatus") as System.Web.UI.WebControls.Image;
            var sortUtStatus = GetSortUpDateTimeOnQueryString();
            var sortDmStatus = GetSortDateMeetingOnQueryString();

            const int nosort = -1;
            if (sortDmStatus != nosort)
            {
                sortUtStatus = nosort;
            }

            ShowSortStatus(imgSortDmStatus, sortDmStatus);
            ShowSortStatus(imgSortUtStatus, sortUtStatus);
        }

        public void ShowSortStatus(System.Web.UI.WebControls.Image imageControl, int sortStatus)
        {
            const int nosort = -1;
            const int descendingstatus = 0;
            const int ascendingstatus = 1;
            if (sortStatus == nosort)
                imageControl.Visible = false;
            else
            {
                if (sortStatus == descendingstatus)
                {
                    imageControl.ImageUrl =
                        @"https://cdn3.iconfinder.com/data/icons/musthave/128/Stock%20Index%20Down.png";
                    imageControl.ToolTip = "Descending";
                    imageControl.Visible = true;
                }
                else
                {
                    imageControl.ImageUrl =
                        @"https://cdn3.iconfinder.com/data/icons/musthave/128/Stock%20Index%20Up.png";
                    imageControl.ToolTip = "Ascending";
                    imageControl.Visible = true;
                }
            }

        }

        public void Redirect(string control)
        {
            var dateFrom = GetDateFrom();
            var dateTo = GetDateTo();
            var pageSize = GetPageSize();
            var sortUpdateTime = GetSortUpDateTimeOnQueryString();
            var sortDateMeeting = GetSortDateMeetingOnQueryString();
            var sales = GetSales();
            var queryString = "";
            var nvc = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            nvc.Remove("datefrom");
            nvc.Remove("dateto");
            nvc.Remove("page");
            nvc.Remove("sales");
            nvc.Remove("pagesize");
            nvc.Remove("sortut");
            nvc.Remove("sortdm");
            var rawUrl = Request.Url.AbsolutePath + "?" + nvc.ToString();
            if (dateFrom != null)
                queryString += "&datefrom=" + dateFrom.Value.ToString("ddMMyyyy");

            if (dateTo != null)
                queryString += "&dateto=" + dateTo.Value.ToString("ddMMyyyy");

            if (!String.IsNullOrEmpty(sales))
                queryString += "&sales=" + sales;

            queryString += "&pagesize=" + pageSize;

            const int nosort = -1;
            const int descendingstatus = 0;
            const int ascendingstatus = 1;

            if (control == "lbtupdatetime")
            {
                if (sortUpdateTime == nosort)
                {
                    sortUpdateTime = descendingstatus;
                }
                else
                {
                    if (sortUpdateTime == descendingstatus)
                    {
                        sortUpdateTime = ascendingstatus;
                    }
                    else
                    {
                        sortUpdateTime = descendingstatus;
                    }
                }
                queryString += "&sortut=" + sortUpdateTime;
            }

            if (control == "lbtdatemeeting")
            {
                if (sortDateMeeting == nosort)
                {
                    sortDateMeeting = descendingstatus;
                }
                else
                {
                    if (sortDateMeeting == descendingstatus)
                    {
                        sortDateMeeting = ascendingstatus;
                    }
                    else
                    {
                        sortDateMeeting = descendingstatus;
                    }
                }
                queryString += "&sortdm=" + sortDateMeeting;
            }

            Response.Redirect(rawUrl + queryString);
        }

        public void Redirect()
        {
            Redirect(null);
        }

        public void DisplaySalesControl()
        {
            //if (!IsUserAdministrator())
            //{
            plhSales.Visible = true;
            //}
        }

        public void DisplaySalesColumn()
        {
            //if (!IsUserAdministrator())
            //{
            var thSales = rptMeetings.Controls[0].Controls[0].FindControl("thSales");
            thSales.Visible = true;
            for (int i = 0; i < rptMeetings.Items.Count; i++)
            {
                rptMeetings.Items[i].FindControl("tdSales").Visible = true;
            }
            //}
        }
        
        public DateTime GetFirstDateOfMonth()
        {
            var today = DateTime.Now;
            var todayMonth = today.Month;
            var todayYear = today.Year;
            const int firstDayOfMonth = 1;
            var firstDateOfMonth = new DateTime(todayYear, todayMonth, firstDayOfMonth);
            return firstDateOfMonth;
        }

        public DateTime GetLastDateOfMonth()
        {
            var today = DateTime.Now;
            var todayMonth = today.Month;
            var todayYear = today.Year;
            var lastDayOfMonth = DateTime.DaysInMonth(todayYear, todayMonth);
            var lastDateOfMonth = new DateTime(todayYear, todayMonth, lastDayOfMonth);
            return lastDateOfMonth;
        }

        protected void rptMeetings_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var activity = e.Item.DataItem as Activity;
            var name = e.Item.FindControl("ltrName") as Literal;
            var position = e.Item.FindControl("ltrPosition") as Literal;
            var dateMeeting = e.Item.FindControl("ltrDateMeeting") as Literal;
            var agency = e.Item.FindControl("ltrAgency") as Literal;
            var updateTime = e.Item.FindControl("ltrUpdateTime") as Literal;

            if (activity != null)
            {
                if (dateMeeting != null) dateMeeting.Text = activity.DateMeeting.ToString("dd/MM/yyyy");
                if (updateTime != null)
                    if (activity.UpdateTime != null) updateTime.Text = activity.UpdateTime.Value.ToString("dd/MM/yyyy");
                if (name != null) name.Text = Module.GetObject<AgencyContact>(activity.ObjectId).Name;
                if (position != null) position.Text = Module.GetObject<AgencyContact>(activity.ObjectId).Position;

                var note = e.Item.FindControl("ltrNote") as Literal;
                note.Text = activity.Note;

                var ltrSale = (Literal)e.Item.FindControl("ltrSale");
                ltrSale.Text = activity.User.FullName;

                if (agency != null) agency.Text = Module.GetObject<Agency>(Int32.Parse(activity.Params)).Name;

            }
        }

        protected void btnExportMeetings_OnClick(object sender, EventArgs e)
        {
            var document = new Document();
            var builder = new DocumentBuilder(document);
            int count;

            IList<Activity> activityList = GetActivities(0, 0, out count, true);
            var salesList = GetSalesListInActivity(true) as IList<User>;

            for (int i = 0; i < salesList.Count; i++)
            {
                var needInsertSalesHeader = true;
                for (int j = 0; j < activityList.Count; j++)
                {
                    var activity = activityList[j] as Activity;
                    var uniqueSales = salesList[i];
                    var salesInActivity = activity.User;
                    if (uniqueSales.Id != salesInActivity.Id)
                        continue;

                    var name = Module.GetObject<AgencyContact>(activity.ObjectId).Name;
                    var position = Module.GetObject<AgencyContact>(activity.ObjectId).Position;
                    var dateMeeting = activity.DateMeeting.ToString("dd/MM/yyyy");
                    var agency = Module.GetObject<Agency>(Int32.Parse(activity.Params)).Name;
                    var note = activity.Note;
                    var salesName = uniqueSales.FullName;
                    InsertTableActivityToDocument(builder, needInsertSalesHeader, dateMeeting, salesName, name, position, agency, note);
                    needInsertSalesHeader = false;
                }
            }
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/ms-word";
            Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Meetings.doc"));

            MemoryStream m = new MemoryStream();

            document.Save(m, SaveFormat.Doc);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();

        }

        protected void btnView_OnClick(object sender, EventArgs e)
        {
            Redirect();
        }

        protected void pagerMeetings_OnPageChanged(object sender, PageChangedEventArgs e)
        {
            BindRptMeetings();
        }

        protected void lbtUpdateTime_OnClick(object sender, EventArgs e)
        {
            var controlclick = "lbtupdatetime";
            Redirect(controlclick);
        }

        protected void lbtDateMeeting_OnClick(object sender, EventArgs e)
        {
            var controlclick = "lbtdatemeeting";
            Redirect(controlclick);
        }
    }
}
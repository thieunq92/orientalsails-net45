using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using CMS.Core.Util;
using GemBox.Spreadsheet;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class CustomerComment : SailsAdminBasePage
    {
        private Cruise _cruise;
        private DateTime _date;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["cruiseid"] != null && Request.QueryString["date"] != null)
            {
                _date = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["date"]));
                _cruise = Module.CruiseGetById(Convert.ToInt32(Request.QueryString["cruiseid"]));
            }
            if (!IsPostBack)
            {
                ddlCruises.DataSource = Module.CruiseGetAll();
                ddlCruises.DataTextField = "Name";
                ddlCruises.DataValueField = "Id";
                ddlCruises.DataBind();
                if (_cruise != null)
                {
                    ddlCruises.SelectedValue = _cruise.Id.ToString();
                    txtDate.Text = _date.ToString("dd/MM/yyyy");
                    GetDataSource();
                }
            }
        }

        protected void GetDataSource()
        {
            IList list = Module.BookingGetInDate(_cruise, _date);
            IList result = new ArrayList();
            foreach (Booking booking in list)
            {
                foreach (BookingRoom bkroom in booking.BookingRooms)
                {
                    result.Add(bkroom);
                }
            }

            rptRooms.DataSource = result;
            rptRooms.DataBind();
        }

        protected void rptRooms_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem  is BookingRoom)
            {
                BookingRoom bkroom = (BookingRoom) e.Item.DataItem;

                Literal litRoom = e.Item.FindControl("litRoom") as Literal;
                if (litRoom!=null)
                {
                    string agency = string.Empty;
                    if (bkroom.Book.Agency!=null)
                    {
                        agency = bkroom.Book.Agency.Name;
                        agency += " "+ bkroom.Book.AgencyCode;
                    }

                    string room = string.Empty;
                    if (bkroom.Room!=null)
                    {
                        room = "/" + bkroom.Room.Name;
                    }
                    else
                    {
                        room = "/" + bkroom.RoomClass.Name + " " + bkroom.RoomType.Name;
                    }

                    litRoom.Text = string.Format("{0} {1}", agency, room);
                }

                TextBox txtTourComment = (TextBox) e.Item.FindControl("txtTourComment");
                TextBox txtRoomComment = (TextBox)e.Item.FindControl("txtRoomComment");
                TextBox txtFoodComment = (TextBox)e.Item.FindControl("txtFoodComment");
                TextBox txtStaffComment = (TextBox)e.Item.FindControl("txtStaffComment");
                TextBox txtGuideComment = (TextBox)e.Item.FindControl("txtGuideComment");
                TextBox txtCustomerIdea = (TextBox)e.Item.FindControl("txtCustomerIdea");

                txtTourComment.Text = bkroom.TourComment;
                txtRoomComment.Text = bkroom.RoomComment;
                txtFoodComment.Text = bkroom.FoodComment;
                txtStaffComment.Text = bkroom.StaffComment;
                txtGuideComment.Text = bkroom.GuideComment;
                txtCustomerIdea.Text = bkroom.CustomerIdea;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptRooms.Items)
            {
                HiddenField hiddenId = (HiddenField)item.FindControl("hiddenId");
                TextBox txtTourComment = (TextBox)item.FindControl("txtTourComment");
                TextBox txtRoomComment = (TextBox)item.FindControl("txtRoomComment");
                TextBox txtFoodComment = (TextBox)item.FindControl("txtFoodComment");
                TextBox txtStaffComment = (TextBox)item.FindControl("txtStaffComment");
                TextBox txtGuideComment = (TextBox)item.FindControl("txtGuideComment");
                TextBox txtCustomerIdea = (TextBox)item.FindControl("txtCustomerIdea");

                BookingRoom bkroom = Module.BookingRoomGetById(Convert.ToInt32(hiddenId.Value));
                bkroom.TourComment = txtTourComment.Text;
                bkroom.RoomComment = txtRoomComment.Text;
                bkroom.FoodComment = txtFoodComment.Text;
                bkroom.StaffComment = txtStaffComment.Text;
                bkroom.GuideComment = txtGuideComment.Text;
                bkroom.CustomerIdea = txtCustomerIdea.Text;

                Module.SaveOrUpdate(bkroom);
            }
        }

        protected void btnComment_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDate.Text) && ddlCruises.SelectedIndex >= 0)
            {
                DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                PageRedirect(string.Format("CustomerComment.aspx?NodeId={0}&SectionId={1}&cruiseid={2}&date={3}",
                                           Node.Id, Section.Id, ddlCruises.SelectedValue, date.ToOADate()));
            }
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            IList list = Module.BookingGetInDate(null, _date);
            // Sắp xếp thứ tự theo tàu để có thể lọc ra sau đó

            int totalRows = 0;
            foreach (Booking booking in list)
            {
                totalRows += booking.BookingRooms.Count; // Tổng số dòng trắng
            }


            // Bắt đầu thao tác export
            ExcelFile excelFile = new ExcelFile();
            excelFile.LoadXls(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/Comments.xls"));
            // Mở sheet 0
            ExcelWorksheet sheet = excelFile.Worksheets[0];

            #region -- Xuất dữ liệu --

            const int firstCruise = 12; // Dòng đầu tiên là thông tin cruise
            const int firstrow = 13; // Dòng đầu tiên là thông tin thường
            int firstCRow = firstCruise; // Dòng đầu tiên của một tàu
            int cruiseIndex = 0; // Số thứ tự của tàu
            sheet.Cells[8, 0].Value = string.Format("Ngày {0} tháng {1} năm {2}", _date.Day, _date.Month, _date.Year);
            
            sheet.Rows[firstrow].InsertCopy(totalRows - 1, sheet.Rows[firstrow]);
            int curr = firstrow;
            Cruise cruise = null;
            foreach (Booking booking in list)
            {
                if (booking.Cruise!=cruise)
                {
                    int cRow = firstCruise;
                    if (cruise!=null) // Là null chứng tỏ mới chỉ là tàu đầu tiên
                    {
                        sheet.Rows[curr].InsertCopy(1, sheet.Rows[firstCruise]);
                        cRow = curr;
                        cruiseIndex += 1;
                    }
                    else
                    {
                        cruiseIndex = 1;
                    }
                    cruise = booking.Cruise;

                    sheet.Cells[cRow, 0].Value = Text.RomanNumeralize(cruiseIndex);
                    sheet.Cells[cRow, 1].Value = cruise.Name;

                    if (cRow == curr)
                    {
                        curr += 1;
                    }
                    firstCRow = cRow;
                }
                foreach (BookingRoom bkroom in booking.BookingRooms)
                {
                    sheet.Cells[curr, 0].Value = curr - firstCRow;
                    string agency = string.Empty;
                    if (bkroom.Book.Agency != null)
                    {
                        agency = bkroom.Book.Agency.Name;
                        agency += " " + bkroom.Book.AgencyCode;
                    }

                    string room;
                    if (bkroom.Room != null)
                    {
                        room = "/" + bkroom.Room.Name;
                    }
                    else
                    {
                        room = "/" + bkroom.RoomClass.Name + " " + bkroom.RoomType.Name;
                    }

                    sheet.Cells[curr, 1].Value = string.Format("{0} {1}", agency, room);
                    
                    if (bkroom.Book.Agency!=null)
                    {
                        sheet.Cells[curr, 2].Value = bkroom.Book.Agency.Email;
                    }

                    sheet.Cells[curr, 3].Value = bkroom.TourComment;
                    sheet.Cells[curr, 4].Value = bkroom.RoomComment;
                    sheet.Cells[curr, 5].Value = bkroom.FoodComment;
                    sheet.Cells[curr, 6].Value = bkroom.StaffComment;
                    sheet.Cells[curr, 7].Value = bkroom.GuideComment;
                    sheet.Cells[curr, 8].Value = bkroom.CustomerIdea;
                    curr += 1;
                }
            }

            #endregion

            #region -- Trả dữ liệu về cho người dùng --

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("Roomlist{0:dd_MMM}", _date));

            MemoryStream m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();

            #endregion
        }
    }
}

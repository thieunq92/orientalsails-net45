using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class StayDetail : SailsAdminBasePage
    {
        private Booking _booking;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["bookingid"] != null)
            {
                _booking = Module.BookingGetById(Convert.ToInt32(Request.QueryString["bookingid"]));
            }

            if (!IsPostBack)
            {
                #region -- Xử lý customer trước khi bind data --

                IList customers = GetCustomers();

                #endregion

                rptCustomers.DataSource = customers;
                rptCustomers.DataBind();
            }
        }

        private IList GetCustomers()
        {
            IList customers = new ArrayList();
            foreach (BookingRoom room in _booking.BookingRooms)
            {
                // Nếu book single chỉ add khách đầu tiên
                if (room.IsSingle)
                {
                    customers.Add(room.Customers[0]);
                }

                foreach (Customer customer in room.RealCustomers)
                {
                    // Nếu không phải single add tất cả khách người lớn
                    if (!room.IsSingle && customer.Type == CustomerType.Adult)
                    {
                        customers.Add(customer);
                    }
                    // và trẻ em
                    if (customer.Type == CustomerType.Children && room.HasChild)
                    {
                        customers.Add(customer);
                    }
                    if (customer.Type == CustomerType.Baby && room.HasBaby)
                    {
                        customers.Add(customer);
                    }
                }
            }
            return customers;
        }

        protected void rptCustomers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Customer)
            {
                Customer customer = (Customer)e.Item.DataItem;
                Literal litRoom = e.Item.FindControl("litRoom") as Literal;
                if (litRoom != null)
                {
                    if (customer.BookingRoom.Room != null)
                    {
                        litRoom.Text = customer.BookingRoom.Room.Name;
                    }
                }

                TextBox txtFullName = (TextBox)e.Item.FindControl("txtFullName");
                DropDownList ddlGender = (DropDownList)e.Item.FindControl("ddlGender");
                TextBox txtBirthDate = (TextBox)e.Item.FindControl("txtBirthDate");
                TextBox txtNationality = (TextBox)e.Item.FindControl("txtNationality");
                DropDownList ddlNationalities = (DropDownList)e.Item.FindControl("ddlNationalities");
                TextBox txtPassport = (TextBox)e.Item.FindControl("txtPassport");
                CheckBox chkVietKieu = (CheckBox)e.Item.FindControl("chkVietKieu");
                TextBox txtPurpose = (TextBox)e.Item.FindControl("txtPurpose");
                DropDownList ddlPurposes = (DropDownList)e.Item.FindControl("ddlPurposes");
                TextBox txtStayTerm = (TextBox)e.Item.FindControl("txtStayTerm");
                TextBox txtStayIn = (TextBox)e.Item.FindControl("txtStayIn");

                txtFullName.Text = customer.Fullname;
                if (customer.IsMale.HasValue)
                {
                    if (customer.IsMale.Value)
                    {
                        ddlGender.SelectedIndex = 1;
                    }
                    else
                    {
                        ddlGender.SelectedIndex = 2;
                    }
                }
                if (customer.Birthday.HasValue)
                {
                    txtBirthDate.Text = customer.Birthday.Value.ToString("dd/MM/yyyy");
                }
                txtNationality.Text = customer.Country;
                txtPassport.Text = customer.Passport;
                chkVietKieu.Checked = customer.IsVietKieu;
                txtPurpose.Text = customer.Purpose;
                txtStayTerm.Text = customer.StayTerm;
                txtStayIn.Text = customer.StayIn;
                ddlNationalities.DataSource = Module.NationalityGetAll();
                ddlNationalities.DataTextField = "Name";
                ddlNationalities.DataValueField = "Id";
                ddlNationalities.DataBind();
                ddlNationalities.Items.Insert(0, "Không rõ quốc tịch");

                if (customer.Nationality != null)
                {
                    ddlNationalities.SelectedValue = customer.Nationality.Id.ToString();
                }

                ddlPurposes.DataSource = Module.PurposeGetAll();
                ddlPurposes.DataTextField = "Name";
                ddlPurposes.DataValueField = "Id";
                ddlPurposes.DataBind();
                if (customer.StayPurpose != null)
                {
                    ddlPurposes.SelectedValue = customer.StayPurpose.Id.ToString();
                }
                else
                {
                    ddlPurposes.SelectedValue = "3";
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptCustomers.Items)
            {
                HiddenField hiddenId = (HiddenField)item.FindControl("hiddenId");
                TextBox txtFullName = (TextBox)item.FindControl("txtFullName");
                DropDownList ddlGender = (DropDownList)item.FindControl("ddlGender");
                TextBox txtBirthDate = (TextBox)item.FindControl("txtBirthDate");
                TextBox txtNationality = (TextBox)item.FindControl("txtNationality");
                DropDownList ddlNationalities = (DropDownList)item.FindControl("ddlNationalities");
                TextBox txtPassport = (TextBox)item.FindControl("txtPassport");
                CheckBox chkVietKieu = (CheckBox)item.FindControl("chkVietKieu");
                TextBox txtPurpose = (TextBox)item.FindControl("txtPurpose");
                DropDownList ddlPurposes = (DropDownList)item.FindControl("ddlPurposes");
                TextBox txtStayTerm = (TextBox)item.FindControl("txtStayTerm");
                TextBox txtStayIn = (TextBox)item.FindControl("txtStayIn");

                Customer customer = Module.CustomerGetById(Convert.ToInt32(hiddenId.Value));
                customer.Fullname = txtFullName.Text;
                if (ddlGender.SelectedIndex > 0)
                {
                    customer.IsMale = ddlGender.SelectedIndex == 1;
                }
                else
                {
                    customer.IsMale = null;
                }

                if (!string.IsNullOrEmpty(txtBirthDate.Text))
                {
                    customer.Birthday = DateTime.ParseExact(txtBirthDate.Text, "dd/MM/yyyy",
                                                            CultureInfo.InvariantCulture);
                }
                else
                {
                    customer.Birthday = null;
                }

                customer.Country = txtNationality.Text;
                customer.Passport = txtPassport.Text;
                customer.IsVietKieu = chkVietKieu.Checked;
                customer.Purpose = txtPurpose.Text;
                customer.StayTerm = txtStayTerm.Text;
                customer.StayIn = txtStayIn.Text;

                if (ddlNationalities.SelectedIndex > 0)
                {
                    customer.Nationality = Module.NationalityGetById(Convert.ToInt32(ddlNationalities.SelectedValue));
                }
                customer.StayPurpose = Module.PurposeGetById(Convert.ToInt32(ddlPurposes.SelectedValue));

                Module.SaveOrUpdate(customer);
            }
            ShowError("Dữ liệu lưu lại thành công!");
        }

        protected void btnBlank_Click(object sender, EventArgs e)
        {
            Response.Clear();
            string path = Server.MapPath("/Modules/Sails/Admin/ExportTemplates/ImportTemplate.xls");
            FileInfo template = new FileInfo(path);
            if (!template.Exists)
            {
                ShowError("Không tìm thấy tập tin mẫu!");
                return;
            }
            Response.AddHeader("Content-Disposition", "attachment; filename=" + template.Name);
            Response.AddHeader("Content-Length", template.Length.ToString());
            Response.ContentType = "application/vnd.ms-excel";
            Response.WriteFile(template.FullName);
            Response.End();
        }

        protected void btnIndex_Click(object sender, EventArgs e)
        {
            ExcelFile excelFile = new ExcelFile();
            string tplPath = Server.MapPath("/Modules/Sails/Admin/ExportTemplates/ImportTemplate.xls");
            excelFile.LoadXls(tplPath);
            ExcelWorksheet sheet = excelFile.Worksheets[0];
            // Dòng dữ liệu đầu tiên
            const int firstrow = 1;
            int crow = firstrow;
            IList customers = GetCustomers();
            sheet.Rows[crow].InsertCopy(customers.Count - 1, sheet.Rows[firstrow]);
            foreach (Customer customer in customers)
            {
                sheet.Cells[crow, 0].Value = crow - firstrow + 1;
                sheet.Cells[crow, 1].Value = customer.Fullname;
                
                if (customer.IsMale.HasValue)
                {
                    if (customer.IsMale.Value)
                    {
                        sheet.Cells[crow, 2].Value = "M | Nam";
                    }
                    else
                    {
                        sheet.Cells[crow, 2].Value = "F | Nữ";
                    }
                }

                if (customer.Birthday.HasValue)
                {
                    sheet.Cells[crow, 3].Value = customer.Birthday;
                }

                if (customer.Nationality!=null)
                {
                    sheet.Cells[crow, 4].Value = customer.Nationality.Code;
                }

                sheet.Cells[crow, 5].Value = customer.Passport;

                if (customer.IsVietKieu)
                {
                    sheet.Cells[crow, 6].Value = "VK | Việt Kiều";
                }
                else if (customer.Nationality!=null && customer.Nationality.Code.ToLower() == "vietnam")
                {
                    sheet.Cells[crow, 6].Value = "VN | Việt Nam";
                }
                else
                {
                    sheet.Cells[crow, 6].Value = "NN | Nước ngoài";
                }

                if (customer.StayPurpose!=null)
                {
                    sheet.Cells[crow, 7].Value = customer.StayPurpose.Code;
                }
                sheet.Cells[crow, 8].Value = customer.StayTerm;
                crow += 1;
            }

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("customer{0:dd_MMM}", DateTime.Now));

            MemoryStream m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (fileImport.HasFile)
            {
                ExcelFile excelFile = new ExcelFile();
                excelFile.LoadXls(fileImport.PostedFile.InputStream);
                ExcelWorksheet sheet = excelFile.Worksheets[0];
                // Dòng dữ liệu đầu tiên
                const int firstrow = 1;
                int crow = firstrow;
                IList customers = GetCustomers();                
                foreach (Customer customer in customers)
                {
                    if (sheet.Cells[crow, 1].Value != null)
                    {
                        customer.Fullname = sheet.Cells[crow, 1].Value.ToString();
                    }
                    if (sheet.Cells[crow, 2].Value != null)
                    {
                        string gender = sheet.Cells[crow, 2].Value.ToString();
                        if (gender[0]=='M')
                        {
                            customer.IsMale = true;
                        }
                        else
                        {
                            customer.IsMale = false;
                        }
                    }
                    if (sheet.Cells[crow, 3].Value!=null)
                    {
                        customer.Birthday = (DateTime) sheet.Cells[crow, 3].Value;
                    }

                    if (sheet.Cells[crow, 4].Value!=null)
                    {
                        customer.Nationality = Module.NationalityGetByCode(sheet.Cells[crow, 4].Value.ToString());
                    }
                    //passport, vk, md, han tam tru
                    if (sheet.Cells[crow, 5].Value!=null)
                    {
                        customer.Passport = sheet.Cells[crow, 5].Value.ToString();
                    }

                    if (sheet.Cells[crow, 6].Value != null)
                    {
                        string str = sheet.Cells[crow, 6].Value.ToString();
                        str = str.Substring(0, 2);
                        switch (str.ToLower())
                        {
                            case "vk":
                                customer.IsVietKieu = true;
                                break;
                            default:
                                customer.IsVietKieu = false;
                                break;
                        }
                    }

                    if (sheet.Cells[crow, 7].Value != null)
                    {
                        string str = sheet.Cells[crow, 7].Value.ToString();
                        customer.StayPurpose = Module.purposeGetByCode(str);
                    }

                    if (sheet.Cells[crow, 8].Value != null)
                    {
                        customer.StayTerm = sheet.Cells[crow, 8].Value.ToString();
                    }

                    Module.SaveOrUpdate(customer);
                    crow += 1;
                }
            }
            else
            {
                ShowError("Phải chọn tệp tin tải lên trước");
                return;
            }

            #region -- Xử lý customer trước khi bind data --

            IList datasource = GetCustomers();

            #endregion

            rptCustomers.DataSource = datasource;
            rptCustomers.DataBind();
        }
    }
}

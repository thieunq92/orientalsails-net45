using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.ServerControls;
using CMS.Web.Util;
using GemBox.Spreadsheet;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Utils;
using Portal.Modules.OrientalSails.BusinessLogic.Share;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class AgencyList : SailsAdminBase
    {
        private UserBLL userBLL;
        public UserBLL UserBLL
        {
            get
            {
                if (userBLL == null)
                    userBLL = new UserBLL();
                return userBLL;
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            pagerBookings.AllowCustomPaging = true;
            if (hplViewMeetings != null)
            {
                hplViewMeetings.NavigateUrl = string.Format("ViewMeetings.aspx?NodeId={0}&SectionId={1}", Node.Id,
                    Section.Id);
            }
            if (!IsPostBack)
            {
                ddlRoles.DataSource = Module.RoleGetAll();
                ddlRoles.DataTextField = "Name";
                ddlRoles.DataValueField = "Id";
                ddlRoles.DataBind();
                ddlRoles.Items.Insert(0, "All roles");

                #region -- Bind sales --

                if (Module.PermissionCheck(Permission.VIEW_ALL_AGENCY, UserIdentity))
                {
                    Role role = Module.RoleGetById(Convert.ToInt32(Module.ModuleSettings("Sale")));
                    ddlSales.DataSource = role.Users;
                    ddlSales.DataTextField = "Username";
                    ddlSales.DataValueField = "Id";
                    ddlSales.DataBind();
                }

                ddlSales.Items.Insert(1, new ListItem("Unbound sale", "0"));
                ddlSales.Items.Insert(0, "All sales");

                ddlLocations.DataSource = Module.GetObject<AgencyLocation>(null, 0, 0);
                ddlLocations.DataTextField = "Name";
                ddlLocations.DataValueField = "Id";
                ddlLocations.DataBind();

                ddlLocations.Items.Insert(0, "All locations");

                #endregion

                GetDataSource();
                LoadFromQueryString();
            }
        }

        protected void rptAgencies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is vAgency)
            {
                vAgency agency = (vAgency)e.Item.DataItem;
                HyperLink hplName = e.Item.FindControl("hplName") as HyperLink;
                if (hplName != null)
                {
                    hplName.Text = agency.Name;
                    hplName.NavigateUrl = string.Format("AgencyView.aspx{0}&AgencyId={1}", GetBaseQueryString(), agency.Id);
                }

                HtmlAnchor aName = e.Item.FindControl("aName") as HtmlAnchor;
                if (aName != null)
                {
                    aName.InnerHtml = agency.Name;
                    aName.Attributes.CssStyle.Add("cursor", "pointer");

                    string script = string.Format("Done('{0}','{1}')", agency.Name.Replace("'", @"\'"), agency.Id);
                    aName.Attributes.Add("onclick", script);
                }

                HyperLink hplEdit = e.Item.FindControl("hplEdit") as HyperLink;
                if (hplEdit != null)
                {
                    hplEdit.NavigateUrl = string.Format("AgencyEdit.aspx{0}&AgencyId={1}", GetBaseQueryString(), agency.Id);
                }

                Literal litRole = e.Item.FindControl("litRole") as Literal;
                if (litRole != null)
                {
                    var hyperLink = e.Item.FindControl("hplPriceSetting") as HyperLink;
                    if (hyperLink != null)
                    {
                        if (agency.Role != null)
                        {
                            litRole.Text = agency.Role.Name;
                            hyperLink.Visible = false;
                        }
                        else
                        {
                            litRole.Text = "Customize Role";
                            hyperLink.Visible = true;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("hplPriceSetting = null");
                    }
                }

                Literal litContract = e.Item.FindControl("litContract") as Literal;
                if (litContract != null)
                {
                    switch (agency.ContractStatus)
                    {
                        case 0:
                            litContract.Text = @"No contract";
                            break;
                        case 1:
                            litContract.Text = @"Expired";
                            break;
                        case 2:
                            litContract.Text = @"Contract in valid";
                            break;
                        case 3:
                            litContract.Text = @"Expire soon";
                            break;
                        case 4:
                            litContract.Text = @"Contract sent";
                            break;
                    }
                }

                var trItem = e.Item.FindControl("trItem") as HtmlTableRow;
                if (trItem != null)
                {
                    switch (agency.ContractStatus)
                    {
                        case 0: // No contract
                            trItem.Attributes.Add("class", "danger");
                            break;
                        case 1: // Contract expired
                            trItem.Attributes.Add("class", "active");
                            break;
                        case 2: // Có hợp đồng và chưa hết hạn trong vòng 30 ngày
                            trItem.Attributes.Add("class", "success");
                            break;
                        case 3: // Có hợp đồng nhưng sẽ hết hạn trong vòng 30 ngày
                            trItem.Attributes.Add("class", "warning");
                            break;
                        case 4:
                            trItem.Attributes.CssStyle.Add("background-color", "greenyellow");
                            break;
                    }
                }

                if (agency.ContractStatus != 0)
                {
                    if (string.IsNullOrEmpty(agency.Contract))
                    {
                        HyperLink hplContract = e.Item.FindControl("hplContract") as HyperLink;
                        if (hplContract != null)
                        {
                            hplContract.Visible = false;
                        }
                    }
                    else
                    {
                        HyperLink hplContract = e.Item.FindControl("hplContract") as HyperLink;
                        if (hplContract != null)
                        {
                            hplContract.Text = @"[View]";
                            hplContract.NavigateUrl = agency.Contract;
                        }
                    }
                }

                ValueBinder.BindLiteral(e.Item, "litPayment", agency.PaymentPeriod);

                var litIndex = e.Item.FindControl("litIndex") as Literal;
                if (litIndex != null)
                {
                    litIndex.Text = (e.Item.ItemIndex + pagerBookings.PageSize * pagerBookings.CurrentPageIndex + 1) + ".";
                }

                var litSale = e.Item.FindControl("litSale") as Literal;
                if (litSale != null)
                {
                    if (agency.Sale != null)
                    {
                        litSale.Text = agency.Sale.UserName;
                    }
                }

                var hplPriceSetting = e.Item.FindControl("hplPriceSetting") as HyperLink;
                if (hplPriceSetting != null)
                {
                    hplPriceSetting.NavigateUrl =
                        string.Format("PriceConfiguration.aspx?NodeId={0}&SectionId={1}&agentid={2}", Node.Id, Section.Id,
                                      agency.Id);
                }

                HtmlTableCell tdLastBooking = e.Item.FindControl("tdLastBooking") as HtmlTableCell;
                if (tdLastBooking != null)
                {
                    if (agency.LastBooking.HasValue)
                    {
                        tdLastBooking.InnerHtml = string.Format("{0} (<a href='{1}'>list</a>) ",
                                                                agency.LastBooking.Value.ToString("dd/MM/yyyy"),
                                                                string.Format(
                                                                    "BookingList.aspx?NodeId={0}&SectionId={1}&ai={2}",
                                                                    Node.Id, Section.Id, agency.Id));
                    }
                    else
                    {
                        tdLastBooking.InnerText = "Never";
                    }
                }
            }
        }

        protected void pagerBookings_PageChanged(object sender, PageChangedEventArgs e)
        {
            GetDataSource();
        }

        protected void GetDataSource()
        {
            int count;
            rptAgencies.DataSource = Module.vAgencyGetByQueryString(Request.QueryString, pagerBookings.PageSize, pagerBookings.CurrentPageIndex, out count, UserIdentity);
            pagerBookings.VirtualItemCount = count;
            rptAgencies.DataBind();
        }

        protected void rptAgencies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Agency agency;
            switch (e.CommandName.ToLower())
            {
                case "delete":
                    agency = Module.AgencyGetById(Convert.ToInt32(e.CommandArgument));
                    agency.Deleted = true;
                    Module.SaveOrUpdate(agency);
                    //Module.Delete(agency);
                    GetDataSource();
                    break;
            }
        }

        protected virtual void btnSearch_Click(object sender, EventArgs e)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                str += "&Name=" + txtName.Text;
            }

            if (ddlRoles.SelectedIndex > 0)
            {
                str += "&RoleId=" + ddlRoles.SelectedValue;
            }

            if (ddlSales.SelectedIndex > 0)
            {
                str += "&saleid=" + ddlSales.SelectedValue;
            }

            if (ddlContracts.SelectedIndex > 0)
            {
                str += "&contract=" + ddlContracts.SelectedValue;
            }

            if (ddlLocations.SelectedIndex > 0)
            {
                str += "&locationid=" + ddlLocations.SelectedValue;
            }
            PageRedirect(string.Format("AgencyList.aspx?NodeId={0}&SectionId={1}{2}", Node.Id, Section.Id, str));
        }

        protected void LoadFromQueryString()
        {
            if (Request.QueryString["Name"] != null)
            {
                txtName.Text = Request.QueryString["Name"];
            }

            if (Request.QueryString["RoleId"] != null)
            {
                ddlRoles.SelectedValue = Request.QueryString["RoleId"];
            }

            if (Request.QueryString["saleid"] != null)
            {
                ddlSales.SelectedValue = Request.QueryString["saleid"];
            }

            if (Request.QueryString["locationid"] != null)
            {
                ddlLocations.SelectedValue = Request.QueryString["locationid"];
            }

            if (Request.QueryString["contract"] != null)
            {
                ddlContracts.SelectedValue = Request.QueryString["contract"];
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (!Module.PermissionCheck(Permission.ACTION_EXPORTAGENCY, UserIdentity))
            {
                ShowError(Resources.textDeniedFunction);
                return;
            }
            int count;
            IList data = Module.vAgencyGetByQueryString(Request.QueryString, 0, 0, out count, UserIdentity);
            string tpl = Server.MapPath("/Modules/Sails/Admin/ExportTemplates/Danh_sach_dai_ly.xls");
            ExcelFile excelFile = new ExcelFile();
            excelFile.LoadXls(tpl);

            ExcelWorksheet sheet = excelFile.Worksheets[0];
            // Dòng dữ liệu đầu tiên
            const int firstrow = 7;
            int crow = firstrow;
            sheet.Rows[crow].InsertCopy(count - 1, sheet.Rows[firstrow]);

            foreach (vAgency agency in data)
            {
                sheet.Cells[crow, 0].Value = crow - firstrow + 1;
                sheet.Cells[crow, 1].Value = agency.Name;
                sheet.Cells[crow, 2].Value = agency.Phone;
                sheet.Cells[crow, 3].Value = agency.Fax;
                sheet.Cells[crow, 4].Value = agency.Address;
                sheet.Cells[crow, 5].Value = agency.Email;
                if (agency.Sale != null)
                {
                    sheet.Cells[crow, 6].Value = agency.Sale.UserName;
                }
                if (agency.LastBooking.HasValue)
                {
                    sheet.Cells[crow, 7].Value = agency.LastBooking.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    sheet.Cells[crow, 7].Value = "Never";
                }
                switch (agency.ContractStatus)
                {
                    case 0:
                        sheet.Cells[crow, 8].Value = "No";
                        break;
                    case 1:
                        sheet.Cells[crow, 8].Value = "Pending";
                        break;
                    case 2:
                        sheet.Cells[crow, 8].Value = "Yes";
                        break;
                }
                crow++;
            }
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=agencylist.xls");

            var m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();
        }

        protected void btnReboundSale_Click(object sender, EventArgs e)
        {
            var list = Module.AgencyResetSale();
            rptAgencies.DataSource = list;
            rptAgencies.DataBind();

            // tạo list sale
            var sales = new List<User>();
            foreach (var vAgency in list)
            {
                if (vAgency.Sale != null && !sales.Contains(vAgency.Sale))
                {
                    sales.Add(vAgency.Sale);
                }
            }

            // đối với từng sales
            foreach (User sale in sales)
            {
                if (string.IsNullOrEmpty(sale.Email)) continue;

                var agencies = new List<vAgency>();
                foreach (var vAgency in list)
                {
                    if (vAgency.Sale == sale)
                    {
                        agencies.Add(vAgency);
                    }
                }

                var str = new StringBuilder();
                str.AppendFormat("<p>This is a list of your unbounded agencies:</p>");
                str.AppendFormat("<ol>");
                foreach (var vAgency in agencies)
                {
                    str.AppendFormat("<li>{0}</li>", vAgency.Name);
                }
                str.AppendFormat("</ol>");

                //Module.SendMail(sale.Email, "Your agency has been unbounded", str.ToString(), "koh2203@gmail.com");
            }
        }

        protected void btnRecheck_Click(object sender, EventArgs e)
        {
            rptAgencies.DataSource = Module.AgencyResetSale(false);
            //rptAgencies.DataSource = Module.AgencyRecheck();
            rptAgencies.DataBind();
        }
    }
}

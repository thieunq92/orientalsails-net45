using System;
using System.Text;
using System.Web.Services.Discovery;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Web.Util;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Aspose.Words;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using Portal.Modules.OrientalSails.BusinessLogic;
using Aspose.Words.Rendering;
using Microsoft.Office.Interop.Word;
using System.Linq;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class AgencyView : SailsAdminBasePage
    {
        private bool _editPermission;
        private bool _viewBookingPermission;
        private bool _contactsPermission;
        private bool _recentActivitiesPermission;
        private bool _contractsPermission;
        private AgencyViewBLL agencyViewBLL;
        public AgencyViewBLL AgencyViewBLL
        {
            get
            {
                if (agencyViewBLL == null)
                    agencyViewBLL = new AgencyViewBLL();
                return agencyViewBLL;
            }
        }

        public Agency Agency
        {
            get
            {
                Agency agency = null;
                try
                {
                    if (Request.QueryString["AgencyId"] != null)
                        agency = AgencyViewBLL.AgencyGetById(Convert.ToInt32(Request.QueryString["AgencyId"]));
                }
                catch (Exception) { }
                return agency;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _editPermission = Module.PermissionCheck(Permission.ACTION_EDITAGENCY, UserIdentity);
                _viewBookingPermission = Module.PermissionCheck("VIEWBOOKINGBYAGENCY", UserIdentity);
                _contactsPermission = Module.PermissionCheck("CONTACTS", UserIdentity);
                _recentActivitiesPermission = Module.PermissionCheck("RECENTACTIVITIES", UserIdentity);
                _contractsPermission = Module.PermissionCheck("CONTRACTS", UserIdentity);

                if (Request.QueryString["agencyid"] != null)
                {
                    var agency = Module.AgencyGetById(Convert.ToInt32(Request.QueryString["agencyid"]));

                    if (agency.Sale == UserIdentity)
                    {
                        _editPermission = true;
                    }
                    litName1.Text = agency.Name;
                    litName.Text = agency.Name;
                    litTenTiengViet.Text = agency.TenTiengViet;
                    litGiamDoc.Text = agency.GiamDoc;
                    if (agency.Role != null)
                        litRole.Text = agency.Role.Name;
                    else
                    {
                        litRole.Text = "Customize Role";
                    }
                    if (agency.Sale != null)
                    {
                        litSale.Text = agency.Sale.AllName;
                        litSalePhone.Text = agency.Sale.Website;
                    }
                    else
                        litSale.Text = @"Unbound";
                    litTax.Text = agency.TaxCode;
                    if (agency.Location != null)
                        litLocation.Text = agency.Location.Name;
                    litAddress.Text = agency.Address;

                    if (!string.IsNullOrEmpty(agency.Email))
                    {
                        hplEmail.NavigateUrl = string.Format("mailto:{0}", agency.Email);
                        hplEmail.Text = agency.Email;
                    }
                    litPhone.Text = agency.Phone;
                    litFax.Text = agency.Fax;

                    litAccountant.Text = agency.Accountant;
                    litPayment.Text = agency.PaymentPeriod.ToString();

                    litNote.Text = agency.Description;

                    var agencyId = Agency.Id;
                    rptContracts.DataSource = AgencyViewBLL.AgencyContractGetAllByAgency(agencyId);
                    rptContracts.DataBind();

                    rptContacts.DataSource = Module.ContactGetByAgency(agency, !_editPermission); // nếu không có quyền edit thì ko có quyền view
                    rptContacts.DataBind();

                    hplEditAgency.Visible = _editPermission;
                    hplEditAgency.NavigateUrl = string.Format("AgencyEdit.aspx?NodeId={0}&SectionId={1}&agencyid={2}", Node.Id,
                                                        Section.Id, agency.Id);

                    hplAddContact.Visible = _editPermission;
                    hplAddContact.NavigateUrl = "javascript:";
                    string url = string.Format("AgencyContactEdit.aspx?NodeId={0}&SectionId={1}&agencyid={2}",
                                                    Node.Id, Section.Id, agency.Id);
                    hplAddContact.Attributes.Add("onclick", CMS.ServerControls.Popup.OpenPopupScript(url, "Contact", 300, 400));

                    url = string.Format("AgencyContractEdit.aspx?NodeId={0}&SectionId={1}&agencyid={2}",
                                                    Node.Id, Section.Id, agency.Id);

                    hplBookingList.NavigateUrl = string.Format(
                        "BookingList.aspx?NodeId={0}&SectionId={1}&ai={2}", Node.Id, Section.Id, agency.Id);
                    hplReceivable.NavigateUrl =
                        string.Format("PaymentReport.aspx?NodeId={0}&SectionId={1}&ai={2}&from={3}&to={4}",
                                      Node.Id, Section.Id, agency.Id, DateTime.Today.AddMonths(-3).ToOADate(), DateTime.Today.ToOADate());

                    rptActivities.DataSource = Module.GetObject<Activity>(Expression.And(Expression.Eq("ObjectType", "MEETING"), Expression.Eq("Params", Convert.ToString(agency.Id))), 0, 0,
                                                                          Order.Desc("DateMeeting"));
                    rptActivities.DataBind();
                }
            }
            RenderViewBookingByThisAgency();
            RenderContacts();
            RenderRecentActivities();
            RenderContracts();
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (agencyViewBLL != null)
            {
                agencyViewBLL.Dispose();
                agencyViewBLL = null;
            }
        }
        public void RenderViewBookingByThisAgency()
        {
            if (!_viewBookingPermission)
            {
                hplBookingList.CssClass = hplBookingList.CssClass + " " + "disable";
                hplBookingList.Attributes["href"] = "javascript:";
                var script = @"<script type = 'text/javascript'>";
                script = script +
                         @"$('#" + hplBookingList.ClientID + "').click(function(){$('#disableInform').dialog({resiable:false,modal:true,draggable:false})})";
                script = script + "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "disableInform", script);
            }
        }

        public void RenderContacts()
        {
            if (!_contactsPermission)
            {
                plhContacts.Visible = false;
                lblContacts.Visible = true;
            }
        }

        public void RenderRecentActivities()
        {
            if (!_recentActivitiesPermission)
            {
                plhActivities.Visible = false;
                lblActivities.Visible = true;
            }
        }

        public void RenderContracts()
        {
            if (!_contractsPermission)
            {
                plhContracts.Visible = false;
                lblContracts.Visible = true;
            }
        }

        protected void rptContracts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is AgencyContract)
            {
                var contract = (AgencyContract)e.Item.DataItem;

                ValueBinder.BindLiteral(e.Item, "litName", contract.ContractName);
                ValueBinder.BindLiteral(e.Item, "litExpired", contract.ExpiredDate.Value.ToString("dd/MM/yyyy"));
                if (contract.CreateDate != null)
                    ValueBinder.BindLiteral(e.Item, "litCreatedDate", contract.CreateDate.Value.ToString("dd/MM/yyyy"));

                if (contract.Received == true)
                {
                    ValueBinder.BindLiteral(e.Item, "litReceived", "Yes");
                }
                else
                {
                    ValueBinder.BindLiteral(e.Item, "litReceived", "No");
                }

                var hplDownload = (HyperLink)e.Item.FindControl("hplDownload");
                hplDownload.NavigateUrl = contract.FilePath;
                //hplDownload.NavigateUrl = contract.FileName;
                hplDownload.Text = contract.FileName;

                var linkEdit = (HyperLink)e.Item.FindControl("hplEdit");

                var url = string.Format("AgencyContractEdit.aspx?NodeId={0}&SectionId={1}&contractid={2}",
                                Node.Id, Section.Id, contract.Id);
                linkEdit.NavigateUrl = "javascript:";
                linkEdit.Attributes.Add("onclick", CMS.ServerControls.Popup.OpenPopupScript(url, "Contract", 300, 400));
                linkEdit.Visible = _editPermission;
            }
        }

        protected void rptContacts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is AgencyContact)
            {
                var contact = (AgencyContact)e.Item.DataItem;

                if (!contact.Enabled)
                {
                    e.Item.Visible = false;
                    return;
                }

                var ltrName = (Literal)e.Item.FindControl("ltrName");
                ltrName.Text = contact.Name;

                var hplName = (HyperLink)e.Item.FindControl("hplName");
                hplName.NavigateUrl = "javascript:";

                if (_editPermission)
                {
                    string url = string.Format("AgencyContactEdit.aspx?NodeId={0}&SectionId={1}&contactid={2}",
                                               Node.Id, Section.Id, contact.Id);
                    hplName.Attributes.Add("onclick", CMS.ServerControls.Popup.OpenPopupScript(url, "Contact", 300, 400));
                }

                var linkEmail = (HyperLink)e.Item.FindControl("hplEmail");
                linkEmail.Text = contact.Email;
                linkEmail.NavigateUrl = string.Format("mailto:{0}", contact.Email);

                ValueBinder.BindLiteral(e.Item, "litPosition", contact.Position);
                ValueBinder.BindLiteral(e.Item, "litPhone", contact.Phone);

                if (contact.IsBooker)
                {
                    ValueBinder.BindLiteral(e.Item, "litBooker", "Booker");
                }

                var lbtDelete = (LinkButton)e.Item.FindControl("lbtDelete");
                lbtDelete.Visible = _editPermission;
                lbtDelete.CommandArgument = contact.Id.ToString();

                {
                    var hplCreateMeeting = (HyperLink)e.Item.FindControl("hplCreateMeeting");
                    hplCreateMeeting.NavigateUrl = "javascript:";
                    string url = string.Format("EditMeeting.aspx?NodeId={0}&SectionId={1}&contact={2}",
                                               Node.Id, Section.Id, contact.Id);
                    hplCreateMeeting.Attributes.Add("onclick",
                                                 CMS.ServerControls.Popup.OpenPopupScript(url, "Contract", 300, 400));
                }
            }
        }

        protected void lbtDelete_Click(object sender, EventArgs e)
        {
            var btn = (IButtonControl)sender;
            var contact = Module.ContactGetById(Convert.ToInt32(btn.CommandArgument));

            contact.Enabled = false;
            Module.SaveOrUpdate(contact);

            PageRedirect(Request.RawUrl);
        }

        protected void rptActivities_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var activity = e.Item.DataItem as Activity;
            var name = e.Item.FindControl("ltrName") as Literal;
            var position = e.Item.FindControl("ltrPosition") as Literal;
            var dateMeeting = e.Item.FindControl("ltrDateMeeting") as Literal;
            if (activity != null)
            {
                if (dateMeeting != null) dateMeeting.Text = activity.DateMeeting.ToString("dd/MM/yyyy");
                if (name != null) name.Text = Module.GetObject<AgencyContact>(activity.ObjectId).Name;
                if (position != null) position.Text = Module.GetObject<AgencyContact>(activity.ObjectId).Position;

                var note = e.Item.FindControl("ltrNote") as Literal;
                var strBuilder = new StringBuilder();
                string[] noteWord = activity.Note.Split(new char[] { ' ' });
                bool isLessWords = false;
                for (int i = 0; i <= 50; i++)
                {
                    try
                    {
                        strBuilder.Append(noteWord[i] + " ");
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        isLessWords = true;
                        break;
                    }
                }
                if (!isLessWords) strBuilder.Append("...");
                if (note != null) note.Text = strBuilder.ToString();

                var lbtEditActivity = (LinkButton)e.Item.FindControl("lbtEditActivity");

                var ltrSale = (Literal)e.Item.FindControl("ltrSale");
                ltrSale.Text = activity.User.FullName;

                lbtEditActivity.PostBackUrl = "javascript:";
                string url = string.Format("EditMeeting.aspx?NodeId={0}&SectionId={1}&activity={2}",
                                           Node.Id, Section.Id, activity.Id);
                lbtEditActivity.Attributes.Add("onclick",
                                             CMS.ServerControls.Popup.OpenPopupScript(url, "Contract", 300, 400));
            }
        }

        protected void lbtDeleteActivity_Click(object sender, EventArgs e)
        {
            var btn = (IButtonControl)sender;
            Activity activity = Module.GetObject<Activity>(Convert.ToInt32(btn.CommandArgument));
            Module.Delete(activity);
            PageRedirect(Request.RawUrl);
        }

        protected void btnExportContractPreviewWord_Click(object sender, EventArgs e)
        {
            ExportContractToWord();
        }

        protected void btnExportContractPreviewPdf_Click(object sender, EventArgs e)
        {
            ExportContractToPdf();
        }

        protected void btnExportQuotationPreviewWord_Click(object sender, EventArgs e)
        {
            ExportQuotationToWord();
        }

        protected void btnExportQuotationPreviewPdf_Click(object sender, EventArgs e)
        {
            ExportQuotationToPdf();
        }

        public void ExportContractToWord()
        {
            var doc = GetGeneratedContract();
            var m = new MemoryStream();
            doc.Save(m, SaveFormat.Doc);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/msword";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("{0}.doc", "contract"));
            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            m.Close();
            Response.End();
        }

        public void ExportQuotationToWord()
        {
            var doc = GetGeneratedQuotation();
            var m = new MemoryStream();
            doc.Save(m, SaveFormat.Doc);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/msword";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("{0}.doc", "quotation"));
            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            m.Close();
            Response.End();
        }

        public void ExportContractToPdf()
        {
            var doc = GetGeneratedContract();
            var m = new MemoryStream();
            doc.Save(m, SaveFormat.Doc);
            byte[] wordContent = m.GetBuffer();
            var tmpFile = Path.GetTempFileName();
            var tmpFileStream = File.OpenWrite(tmpFile);
            tmpFileStream.Write(wordContent, 0, wordContent.Length);
            tmpFileStream.Close();

            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = appWord.Documents.Open(tmpFile);
            var tmpFileExport = Path.GetTempFileName();
            wordDocument.ExportAsFixedFormat(tmpFileExport, WdExportFormat.wdExportFormatPDF);
            var fileStream = new FileStream(tmpFileExport, FileMode.Open, FileAccess.ReadWrite);
            var mtest = new MemoryStream();
            fileStream.CopyTo(mtest);
            fileStream.Close();
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("{0}.pdf", "contract"));
            Response.OutputStream.Write(mtest.GetBuffer(), 0, mtest.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            mtest.Close();
            m.Close();
            Response.End();
        }

        public void ExportQuotationToPdf()
        {
            var doc = GetGeneratedQuotation();
            var m = new MemoryStream();
            doc.Save(m, SaveFormat.Doc);
            byte[] wordContent = m.GetBuffer();
            var tmpFile = Path.GetTempFileName();
            var tmpFileStream = File.OpenWrite(tmpFile);
            tmpFileStream.Write(wordContent, 0, wordContent.Length);
            tmpFileStream.Close();

            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = appWord.Documents.Open(tmpFile);
            var tmpFileExport = Path.GetTempFileName();
            wordDocument.ExportAsFixedFormat(tmpFileExport, WdExportFormat.wdExportFormatPDF);
            var fileStream = new FileStream(tmpFileExport, FileMode.Open, FileAccess.ReadWrite);
            var mtest = new MemoryStream();
            fileStream.CopyTo(mtest);
            fileStream.Close();
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("{0}.pdf", "contract"));
            Response.OutputStream.Write(mtest.GetBuffer(), 0, mtest.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            mtest.Close();
            m.Close();
            Response.End();
        }

        public Aspose.Words.Document GetGeneratedContract()
        {
            DateTime? validFromDate = null;

            try
            {
                validFromDate = DateTime.ParseExact(txtContractValidFromDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception) { }

            var validFromDay = validFromDate != null ? validFromDate.Value.Day.ToString("#00") : "";
            var validFromMonth = validFromDate != null ? (validFromDate.Value.Month + 2).ToString("#00") : "";
            var validFromYear = validFromDate != null ? validFromDate.Value.Year.ToString() : "";

            var doc = new Aspose.Words.Document(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/Contract.doc"));
            var agencyName = "";
            var agencyTenGiaoDich = "";
            var agencyNguoiDaiDien = "";
            var agencyAddress = "";
            var agencyPhone = "";
            var agencyFax = "";
            var agencyTaxCode = "";
            try
            {
                agencyName = !String.IsNullOrEmpty(Agency.Name) ? Agency.Name : "";
            }
            catch { }

            try
            {
                agencyTenGiaoDich = !String.IsNullOrEmpty(Agency.TenTiengViet) ? Agency.TenTiengViet : "";
            }
            catch { }

            try
            {
                agencyNguoiDaiDien = !String.IsNullOrEmpty(Agency.GiamDoc) ? Agency.GiamDoc : "";
            }
            catch { }

            try
            {
                agencyAddress = !String.IsNullOrEmpty(Agency.Address) ? Agency.Address : "";
            }
            catch { }

            try
            {
                agencyPhone = !String.IsNullOrEmpty(Agency.Phone) ? Agency.Phone : "";
            }
            catch { }

            try
            {
                agencyFax = !String.IsNullOrEmpty(Agency.Fax) ? Agency.Fax : "";
            }
            catch { }

            try
            {
                agencyTaxCode = !String.IsNullOrEmpty(Agency.TaxCode) ? Agency.TaxCode : "";
            }
            catch { }

            DateTime? validToDate = null;
            try
            {
                validToDate = DateTime.ParseExact(txtContractValidToDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }

            var textValidToDate = "";
            try
            {
                textValidToDate = validToDate.Value.ToString("dd/mm/yyyy");
            }
            catch { }

            doc.Range.Replace(new Regex("(\\[ValidFromDay\\])"), validFromDay);
            doc.Range.Replace(new Regex("(\\[ValidFromMonth\\])"), validFromMonth);
            doc.Range.Replace(new Regex("(\\[ValidFromYear\\])"), validFromYear);
            doc.Range.Replace(new Regex("(\\[AgencyName\\])"), agencyName);
            doc.Range.Replace(new Regex("(\\[TenGiaoDich\\])"), agencyTenGiaoDich);
            doc.Range.Replace(new Regex("(\\[NguoiDaiDien\\])"), agencyNguoiDaiDien);
            doc.Range.Replace(new Regex("(\\[AgencyAddress\\])"), agencyAddress);
            doc.Range.Replace(new Regex("(\\[AgencyPhone\\])"), agencyPhone);
            doc.Range.Replace(new Regex("(\\[AgencyFax\\])"), agencyFax);
            doc.Range.Replace(new Regex("(\\[AgencyTaxCode\\])"), agencyTaxCode);
            doc.Range.Replace(new Regex("(\\[ValidToDate\\])"), textValidToDate);
            return doc;
        }

        public Aspose.Words.Document GetGeneratedQuotation()
        {
            var selectedQuotationTemplate = -1;
            try
            {
                selectedQuotationTemplate = Int32.Parse(ddlQuotationTemplate.SelectedValue);
            }
            catch { }

            var templatePath = "";
            switch (selectedQuotationTemplate)
            {
                case 1:
                    templatePath = "/Modules/Sails/Admin/ExportTemplates/QuotationLv1.doc";
                    break;
                case 2:
                    templatePath = "/Modules/Sails/Admin/ExportTemplates/QuotationLv2.doc";
                    break;
                case 3:
                    templatePath = "/Modules/Sails/Admin/ExportTemplates/QuotationLv3.doc";
                    break;
            }
            var doc = new Aspose.Words.Document(Server.MapPath(templatePath));

            DateTime? validFromDate = null;

            try
            {
                validFromDate = DateTime.ParseExact(txtQuotationValidFromDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception) { }

            var textValidFromDate = "";
            try
            {
                textValidFromDate = validFromDate.Value.ToString("dd/mm/yyyy");
            }
            catch { }
            DateTime? validToDate = null;
            try
            {
                validToDate = DateTime.ParseExact(txtQuotationValidToDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }

            var textValidToDate = "";
            try
            {
                textValidToDate = validToDate.Value.ToString("dd/mm/yyyy");
            }
            catch { }

            doc.Range.Replace(new Regex("(\\[ValidFromDate\\])"), textValidFromDate);
            doc.Range.Replace(new Regex("(\\[ValidToDate\\])"), textValidToDate);
            return doc;
        }

        protected void btnIssueQuotation_Click(object sender, EventArgs e)
        {
            var agencyId = Agency.Id;
            var existedAgencyContract = AgencyViewBLL.AgencyContractGetAllByAgency(agencyId);
            var agencyContract = new AgencyContract();
            if (existedAgencyContract.Count > 0)
            {
                agencyContract = existedAgencyContract.FirstOrDefault();
            }
            agencyContract.Agency = Agency;

            var selectedQuotationTemplate = -1;
            try
            {
                selectedQuotationTemplate = Int32.Parse(ddlContractTemplate.SelectedValue);
            }
            catch { }
            agencyContract.ContractTemplate = selectedQuotationTemplate;

            var contractValidFromDate = new DateTime();
            try
            {
                contractValidFromDate = DateTime.ParseExact(txtQuotationValidFromDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }
            agencyContract.ContractValidFromDate = contractValidFromDate;

            var contractValidToDate = new DateTime();
            try
            {
                contractValidToDate = DateTime.ParseExact(txtQuotationValidToDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }
            agencyContract.ContractValidToDate = contractValidToDate;

            AgencyViewBLL.AgencyContractSaveOrUpdate(agencyContract);
        }

        protected void btnIssueContract_Click(object sender, EventArgs e)
        {
            var agencyId = Agency.Id;
            var existedAgencyContract = AgencyViewBLL.AgencyContractGetAllByAgency(agencyId);
            var agencyContract = new AgencyContract();
            if (existedAgencyContract.Count > 0)
            {
                agencyContract = existedAgencyContract.FirstOrDefault();
            }

            agencyContract.Agency = Agency;

            var selectedContractTemplate = -1;
            try
            {
                selectedContractTemplate = Int32.Parse(ddlContractTemplate.SelectedValue);
            }
            catch { }
            agencyContract.ContractTemplate = selectedContractTemplate;

            var contractValidFromDate = new DateTime();
            try
            {
                contractValidFromDate = DateTime.ParseExact(txtContractValidFromDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }
            agencyContract.ContractValidFromDate = contractValidFromDate;

            var contractValidToDate = new DateTime();
            try
            {
                contractValidToDate = DateTime.ParseExact(txtContractValidToDate.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }

            agencyContract.ContractValidToDate = contractValidToDate;
            AgencyViewBLL.AgencyContractSaveOrUpdate(agencyContract);
        }

    }
}
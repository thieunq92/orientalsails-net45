using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class BookingServices : SailsAdminBasePage
    {
        private Booking _booking;

        private Customer _currentCustomer;
        private IList _services;

        protected IList Services
        {
            get
            {
                if (_services == null)
                {
                    _services = Module.ExtraOptionGetCustomer();
                }
                return _services;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["bookingid"] != null)
            {
                _booking = Module.BookingGetById(Convert.ToInt32(Request.QueryString["bookingid"]));
            }
            if (!IsPostBack)
            {
                rptCustomers.DataSource = _booking.Customers;
                rptCustomers.DataBind();
            }
        }

        protected void rptCustomers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Customer)
            {
                _currentCustomer = (Customer)e.Item.DataItem;
            }
            Repeater rptServices = e.Item.FindControl("rptServices") as Repeater;
            if (rptServices != null)
            {
                rptServices.DataSource = Services;
                rptServices.DataBind();
            }
        }

        protected void rptServices_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is ExtraOption)
            {
                ExtraOption service = (ExtraOption)e.Item.DataItem;
                CheckBox chkService = (CheckBox)e.Item.FindControl("chkService");
                HiddenField hiddenId = (HiddenField)e.Item.FindControl("hiddenId");
                HiddenField hiddenService = (HiddenField)e.Item.FindControl("hiddenService");

                // Lấy dữ liệu về dịch vụ, ưu tiên theo customer, nếu không có thì sử dụng booking
                // Lấy theo customer
                CustomerService customerService = Module.CustomerServiceGetByCustomerAndService(_currentCustomer, service);
                if (customerService != null)
                {
                    chkService.Checked = !customerService.IsExcluded;
                    hiddenId.Value = customerService.Id.ToString();
                    return;
                }

                // Lấy theo booking
                //if (_booking.ExtraServices.Contains(service))
                //{
                //    hiddenId.Value = "0";
                //    chkService.Checked = true;
                //}
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem customerItem in rptCustomers.Items)
            {
                Repeater rptServices = (Repeater)customerItem.FindControl("rptServices");
                HiddenField hiddenCustomerId = (HiddenField)customerItem.FindControl("hiddenId");
                Customer customer = Module.CustomerGetById(Convert.ToInt32(hiddenCustomerId.Value));
                foreach (RepeaterItem serviceItem in rptServices.Items)
                {
                    // Cần phải biết mặc định là có hay không sử dụng dịch vụ
                    // Nếu mặc định khác với thực tế hoặc đã có dữ liệu trong CSDL thì lưu lại
                    CheckBox chkService = (CheckBox)serviceItem.FindControl("chkService");
                    HiddenField hiddenId = (HiddenField)serviceItem.FindControl("hiddenId");
                    HiddenField hiddenServiceId = (HiddenField)serviceItem.FindControl("hiddenServiceId");

                    if (string.IsNullOrEmpty(hiddenId.Value) && chkService.Checked)
                    {
                        CustomerService service = new CustomerService();
                        service.Service = Module.ExtraOptionGetById(Convert.ToInt32(hiddenServiceId.Value));
                        service.Customer = customer;
                        service.IsExcluded = false;
                        Module.SaveOrUpdate(service);
                        continue;
                    }

                    if (hiddenId.Value == "0" && !chkService.Checked)
                    {
                        CustomerService service = new CustomerService();
                        service.Service = Module.ExtraOptionGetById(Convert.ToInt32(hiddenServiceId.Value));
                        service.Customer = customer;
                        service.IsExcluded = true;
                        Module.SaveOrUpdate(service);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(hiddenId.Value) && hiddenId.Value != "0")
                    {
                        CustomerService service = Module.CustomerServiceGetById(Convert.ToInt32(hiddenId.Value));
                        if (chkService.Checked == service.IsExcluded)
                        {
                            service.IsExcluded = !chkService.Checked;
                            Module.SaveOrUpdate(service);
                        }
                    }
                }
            }
        }
    }
}

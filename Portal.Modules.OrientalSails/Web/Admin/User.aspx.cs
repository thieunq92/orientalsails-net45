using System;
using CMS.Core.Util;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.BusinessLogic.Share;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class UserPanel : SailsAdminBasePage
    {
        private UserBLL userBLL;
        public UserBLL UserBLL
        {
            get
            {
                if (userBLL == null)
                {
                    userBLL = new UserBLL();
                }
                return userBLL;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUserControls();
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (userBLL != null)
            {
                userBLL.Dispose();
                userBLL = null;
            }
        }

        private void BindUserControls()
        {
            ltrUserName.Text = UserBLL.UserGetCurrent().UserName;
            ltrFirstName.Text = UserBLL.UserGetCurrent().FirstName;
            ltrLastName.Text = UserBLL.UserGetCurrent().LastName;
            ltrEmail.Text = UserBLL.UserGetCurrent().Email;
            chkActive.Checked = UserIdentity.IsActive;
        }
    }
}
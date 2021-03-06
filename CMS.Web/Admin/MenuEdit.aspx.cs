using System;
using System.Collections;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.Web.Admin.UI;
using CMS.Web.UI;
using CMS.Web.Util;

namespace CMS.Web.Admin
{
    /// <summary>
    /// Summary description for MenuEdit.
    /// </summary>
    public class MenuEdit : AdminBasePage
    {
        private CustomMenu _activeMenu;
        protected Button btnAdd;
        protected Button btnBack;
        protected Button btnDelete;
        protected Button btnDown;
        protected Button btnRemove;

        protected Button btnSave;
        protected Button btnUp;
        protected DropDownList ddlPlaceholder;
        protected ListBox lbxAvailableNodes;
        protected ListBox lbxSelectedNodes;
        protected RequiredFieldValidator rfvName;
        protected TextBox txtName;

        private void Page_Load(object sender, EventArgs e)
        {
            base.Title = "Thực đơn tùy biến";
            if (Context.Request.QueryString["MenuId"] != null)
            {
                if (Int32.Parse(Context.Request.QueryString["MenuId"]) == -1)
                {
                    // Create a new CustomMenu instance
                    _activeMenu = new CustomMenu();
                    _activeMenu.RootNode = base.ActiveNode;
                    btnDelete.Visible = false;
                }
                else
                {
                    // Get Menu data
                    _activeMenu = (CustomMenu) base.CoreRepository.GetObjectById(typeof (CustomMenu),
                                                                                 Int32.Parse(
                                                                                     Context.Request.QueryString[
                                                                                         "MenuId"]));
                    btnDelete.Visible = true;
                    btnDelete.Attributes.Add("onclick", "return confirm('Bạn đã chắc chắn?');");
                }
            }
            if (! IsPostBack)
            {
                txtName.Text = _activeMenu.Name;
                BindPlaceholders();
                BindAvailableNodes();
                BindSelectedNodes();
            }
        }

        private void BindPlaceholders()
        {
            string templatePath = UrlHelper.GetApplicationPath() + ActiveNode.Template.Path;
            BaseTemplate template = (BaseTemplate) LoadControl(templatePath);
            ddlPlaceholder.DataSource = template.Containers;
            ddlPlaceholder.DataValueField = "Key";
            ddlPlaceholder.DataTextField = "Key";
            ddlPlaceholder.DataBind();
            if (_activeMenu.Id != -1)
            {
                ListItem li = ddlPlaceholder.Items.FindByValue(_activeMenu.Placeholder);
                if (li != null)
                {
                    li.Selected = true;
                }
            }
        }

        private void BindAvailableNodes()
        {
            IList rootNodes = ActiveNode.Site.RootNodes;
            AddAvailableNodes(rootNodes);
        }

        private void BindSelectedNodes()
        {
            foreach (Node node in _activeMenu.Nodes)
            {
                lbxSelectedNodes.Items.Add(new ListItem(node.Title, node.Id.ToString()));
                // also remove from available nodes
                ListItem item = lbxAvailableNodes.Items.FindByValue(node.Id.ToString());
                if (item != null)
                {
                    lbxAvailableNodes.Items.Remove(item);
                }
            }
        }

        private void AddAvailableNodes(IList nodes)
        {
            foreach (Node node in nodes)
            {
                int indentSpaces = node.Level*5;
                string itemIndentSpaces = String.Empty;
                for (int i = 0; i < indentSpaces; i++)
                {
                    itemIndentSpaces += "&nbsp;";
                }
                ListItem li = new ListItem(Context.Server.HtmlDecode(itemIndentSpaces) + node.Title, node.Id.ToString());
                lbxAvailableNodes.Items.Add(li);
                if (node.ChildNodes.Count > 0)
                {
                    AddAvailableNodes(node.ChildNodes);
                }
            }
        }

        private void AttachSelectedNodes()
        {
            _activeMenu.Nodes.Clear();
            foreach (ListItem item in lbxSelectedNodes.Items)
            {
                Node node = (Node) base.CoreRepository.GetObjectById(typeof (Node), Int32.Parse(item.Value));
                _activeMenu.Nodes.Add(node);
            }
        }

        private void SaveMenu()
        {
            base.CoreRepository.ClearQueryCache("Menus");

            if (_activeMenu.Id > 0)
            {
                base.CoreRepository.UpdateObject(_activeMenu);
            }
            else
            {
                base.CoreRepository.SaveObject(_activeMenu);
                Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", ActiveNode.Id));
            }
        }

        private void SynchronizeNodeListBoxes()
        {
            // First fetch a fresh list of available nodes because everything has to be
            // nice in place.
            lbxAvailableNodes.Items.Clear();
            BindAvailableNodes();
            // make sure the selected nodes are not in the available nodes list.
            int itemCount = lbxAvailableNodes.Items.Count;
            for (int i = itemCount - 1; i >= 0; i--)
            {
                ListItem item = lbxAvailableNodes.Items[i];
                if (lbxSelectedNodes.Items.FindByValue(item.Value) != null)
                {
                    lbxAvailableNodes.Items.RemoveAt(i);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Context.Response.Redirect("NodeEdit.aspx?NodeId=" + ActiveNode.Id.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                _activeMenu.Name = txtName.Text;
                _activeMenu.Placeholder = ddlPlaceholder.SelectedValue;
                try
                {
                    AttachSelectedNodes();
                    SaveMenu();
                    ShowMessage("Thực đơn đã được lưu");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_activeMenu != null)
            {
                base.CoreRepository.ClearQueryCache("Menus");

                try
                {
                    base.CoreRepository.DeleteObject(_activeMenu);
                    Context.Response.Redirect("NodeEdit.aspx?NodeId=" + ActiveNode.Id.ToString());
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ListItem item = lbxAvailableNodes.SelectedItem;
            if (item != null)
            {
                lbxSelectedNodes.Items.Add(item);
                item.Selected = false;
            }
            SynchronizeNodeListBoxes();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ListItem item = lbxSelectedNodes.SelectedItem;
            if (item != null)
            {
                lbxSelectedNodes.Items.Remove(lbxSelectedNodes.SelectedItem);
                item.Selected = false;
            }
            SynchronizeNodeListBoxes();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            ListItem item = lbxSelectedNodes.SelectedItem;
            if (item != null)
            {
                int origPosition = lbxSelectedNodes.SelectedIndex;
                if (origPosition > 0)
                {
                    lbxSelectedNodes.Items.Remove(item);
                    lbxSelectedNodes.Items.Insert(origPosition - 1, item);
                }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            ListItem item = lbxSelectedNodes.SelectedItem;
            if (item != null)
            {
                int origPosition = lbxSelectedNodes.SelectedIndex;
                if (origPosition < lbxSelectedNodes.Items.Count - 1)
                {
                    lbxSelectedNodes.Items.Remove(item);
                    lbxSelectedNodes.Items.Insert(origPosition + 1, item);
                }
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion
    }
}
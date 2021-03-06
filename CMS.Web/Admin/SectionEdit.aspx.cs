using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.Core.Communication;
using CMS.Core.Domain;
using CMS.Web.Admin.UI;
using CMS.Web.UI;
using CMS.Web.Util;

namespace CMS.Web.Admin
{
    /// <summary>
    /// Summary description for SectionEdit.
    /// </summary>
    public class SectionEdit : AdminBasePage
    {
        private Section _activeSection;
        private IList _availableModuleTypes;
        protected Button btnBack;
        protected Button btnSave;

        protected CheckBox chkShowTitle;
        protected CompareValidator cpvCache;
        protected DropDownList ddlModule;
        protected DropDownList ddlPlaceholder;
        protected HyperLink hplLookup;
        protected HyperLink hplNewConnection;
        protected Label lblModule;
        protected PlaceHolder plcCustomSettings;
        protected Panel pnlConnections;
        protected Panel pnlCustomSettings;
        protected RequiredFieldValidator rfvCache;
        protected RequiredFieldValidator rfvTitle;
        protected Repeater rptConnections;
        protected Repeater rptCustomSettings;
        protected Repeater rptRoles;
        protected TextBox txtCacheDuration;
        protected TextBox txtTitle;

        private void Page_Load(object sender, EventArgs e)
        {
            Title = "Sửa vùng phân hệ";

            if (_activeSection != null && ! IsPostBack)
            {
                BindSectionControls();
                BindModules();
                BindPlaceholders();
                BindCustomSettings();
                BindConnections();
                BindRoles();
            }
        }

        /// <summary>
        /// Loads aan existing Section from the database or creates a new one if the SectionId = -1
        /// </summary>
        private void LoadSection()
        {
            // NOTE: Called from OnInit!
            if (Context.Request.QueryString["SectionId"] != null)
            {
                if (Int32.Parse(Context.Request.QueryString["SectionId"]) == -1)
                {
                    // Create a new section instance
                    _activeSection = new Section();
                    _activeSection.Node = ActiveNode;
                    if (! IsPostBack)
                    {
                        _activeSection.CopyRolesFromNode();
                    }
                }
                else
                {
                    // Get section data
                    _activeSection = (Section) CoreRepository.GetObjectById(typeof (Section),
                                                                                 Int32.Parse(
                                                                                     Context.Request.QueryString[
                                                                                         "SectionId"]));
                }
            }
            // Preload available ModuleTypes because we might need them to display the CustomSettings
            // of the first ModuleType if none is selected (when adding a brand new Section).
            _availableModuleTypes = CoreRepository.GetAll(typeof (ModuleType), "Name");
            // Create the controls for the ModuleType-specific settings.
            CreateCustomSettings();
        }

        private void CreateCustomSettings()
        {
            // Find out the ModuleType. Existing Sections have ModuleType property but for new ones
            // we have to determine which ModuleType is selected.
            ModuleType mt = null;
            if (_activeSection.ModuleType != null)
            {
                mt = _activeSection.ModuleType;
            }
            else if (Context.Request.Form[ddlModule.UniqueID] != null)
            {
                // The user has selected a ModuleType. Fetch that one from the database and
                // create the settings.
                int moduleTypeId = Int32.Parse(Context.Request.Form[ddlModule.UniqueID]);
                mt = (ModuleType) CoreRepository.GetObjectById(typeof (ModuleType), moduleTypeId);
            }
            else
            {
                // Get the Settings of the first ModuleType in the list.
                if (_availableModuleTypes.Count > 0)
                {
                    mt = (ModuleType) _availableModuleTypes[0];
                }
            }

            if (mt != null)
            {
                ModuleBase module;
                if (_activeSection.Id > 0)
                {
                    module = _moduleLoader.GetModuleFromSection(_activeSection);
                }
                else
                {
                    module = _moduleLoader.GetModuleFromClassName(mt.ClassName);
                }
                foreach (ModuleSetting ms in mt.ModuleSettings)
                {
                    HtmlTableRow settingRow = new HtmlTableRow();
                    HtmlTableCell labelCell = new HtmlTableCell();
                    labelCell.InnerText = ms.FriendlyName;
                    HtmlTableCell controlCell = new HtmlTableCell();
                    controlCell.Controls.Add(SettingControlHelper.CreateSettingControl(ms.Name, ms.GetRealType(), null,
                                                                                       module, Page));
                    settingRow.Cells.Add(labelCell);
                    settingRow.Cells.Add(controlCell);
                    plcCustomSettings.Controls.Add(settingRow);
                }
            }
            pnlCustomSettings.Visible = mt.ModuleSettings.Count > 0;
        }

        private void BindSectionControls()
        {
            txtTitle.Text = _activeSection.Title;
            chkShowTitle.Checked = _activeSection.ShowTitle;
            txtCacheDuration.Text = _activeSection.CacheDuration.ToString();
        }

        private void BindModules()
        {
            if (_activeSection.ModuleType != null)
            {
                // A module is attached, there could be data already in it, so we don't give the option to change it
                lblModule.Text = _activeSection.ModuleType.FullName;
                ddlModule.Visible = false;
                lblModule.Visible = true;
            }
            else
            {
                // Note: this._availableModuleTypes are preloaded in LoadSection.
                foreach (ModuleType moduleType in _availableModuleTypes)
                {
                    ddlModule.Items.Add(new ListItem(moduleType.FullName, moduleType.ModuleTypeId.ToString()));
                }
                if (_activeSection.ModuleType != null)
                {
                    ddlModule.Items.FindByValue(_activeSection.ModuleType.ModuleTypeId.ToString()).Selected = true;
                }
                ddlModule.Visible = true;
                lblModule.Visible = false;
            }
        }

        private void BindPlaceholders()
        {
            if (ActiveNode != null)
            {
                if (ActiveNode.Template != null)
                {
                    try
                    {
                        // Read template control and get the containers (placeholders)
                        string templatePath = UrlHelper.GetApplicationPath() + ActiveNode.Template.Path;
                        BaseTemplate template = (BaseTemplate) LoadControl(templatePath);
                        ddlPlaceholder.DataSource = template.Containers;
                        ddlPlaceholder.DataValueField = "Key";
                        ddlPlaceholder.DataTextField = "Key";
                        ddlPlaceholder.DataBind();
                        ListItem li = ddlPlaceholder.Items.FindByValue(_activeSection.PlaceholderId);
                        if (!string.IsNullOrEmpty(_activeSection.PlaceholderId)
                            && li != null)
                        {
                            li.Selected = true;
                        }
                        // Create url for lookup
                        hplLookup.NavigateUrl = "javascript:;";
                        hplLookup.Attributes.Add("onclick"
                                                 ,
                                                 String.Format(
                                                     "window.open(\"TemplatePreview.aspx?TemplateId={0}&Control={1}\", \"Preview\", \"width=760 height=400\")"
                                                     , ActiveNode.Template.Id
                                                     , ddlPlaceholder.ClientID)
                            );
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }
                }
            }
            else
            {
                ddlPlaceholder.Enabled = false;
            }
        }

        private void BindCustomSettings()
        {
            if (_activeSection.Settings.Count > 0)
            {
                foreach (ModuleSetting ms in _activeSection.ModuleType.ModuleSettings)
                {
                    Control ctrl = TemplateControl.FindControl(ms.Name);
                    if (_activeSection.Settings[ms.Name] != null)
                    {
                        string settingValue = _activeSection.Settings[ms.Name].ToString();
                        if (ctrl is TextBox)
                        {
                            ((TextBox) ctrl).Text = settingValue;
                        }
                        else if (ctrl is CheckBox)
                        {
                            ((CheckBox) ctrl).Checked = Boolean.Parse(settingValue);
                        }
                        else if (ctrl is DropDownList)
                        {
                            DropDownList ddl = (DropDownList) ctrl;
                            ListItem li = ddl.Items.FindByValue(settingValue);
                            if (li != null)
                            {
                                li.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        private void BindConnections()
        {
            // First test if connections are possible
            if (_activeSection.ModuleType != null)
            {
                ModuleBase moduleInstance = base.ModuleLoader.GetModuleFromSection(_activeSection);
                if (moduleInstance is IActionProvider)
                {
                    IActionProvider actionProvider = (IActionProvider) moduleInstance;
                    // OK, show connections panel
                    pnlConnections.Visible = true;
                    rptConnections.DataSource = _activeSection.Connections;
                    rptConnections.DataBind();
                    if (_activeSection.Connections.Count < actionProvider.GetOutboundActions().Count)
                    {
                        hplNewConnection.Visible = true;
                        if (ActiveNode != null)
                        {
                            hplNewConnection.NavigateUrl =
                                String.Format("~/Admin/ConnectionEdit.aspx?NodeId={0}&SectionId={1}", ActiveNode.Id,
                                              _activeSection.Id);
                        }
                        else
                        {
                            hplNewConnection.NavigateUrl = String.Format("~/Admin/ConnectionEdit.aspx?SectionId={0}",
                                                                         _activeSection.Id);
                        }
                    }
                    else
                    {
                        hplNewConnection.Visible = false;
                    }
                }
            }
        }

        private void BindRoles()
        {
            IList roles = base.CoreRepository.GetAll(typeof (Role), "PermissionLevel");
            rptRoles.ItemDataBound += rptRoles_ItemDataBound;
            rptRoles.DataSource = roles;
            rptRoles.DataBind();
        }

        private void SaveSection()
        {
            if (_activeSection.Id > 0)
            {
                base.CoreRepository.UpdateObject(_activeSection);
            }
            else
            {
                base.CoreRepository.SaveObject(_activeSection);
                if (ActiveNode != null)
                {
                    Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", ActiveNode.Id));
                }
                else
                {
                    Context.Response.Redirect("Sections.aspx");
                }
            }
        }

        private void SetCustomSettings()
        {
            foreach (ModuleSetting ms in _activeSection.ModuleType.ModuleSettings)
            {
                Control ctrl = TemplateControl.FindControl(ms.Name);
                object val = null;
                if (ctrl is TextBox)
                {
                    string text = ((TextBox) ctrl).Text;
                    if (ms.IsRequired && text == String.Empty)
                    {
                        throw new Exception(String.Format("The value for {0} is required.", ms.FriendlyName));
                    }
                    val = text;
                }
                else if (ctrl is CheckBox)
                {
                    val = ((CheckBox) ctrl).Checked;
                }
                else if (ctrl is DropDownList)
                {
                    val = ((DropDownList) ctrl).SelectedValue;
                }
                else if (ctrl is CustomTypeSettingControl)
                {
                    val = ((CustomTypeSettingControl) ctrl).SelectedValue;
                }
                try
                {
                    // Check if the datatype is correct -> brute force casting :)
                    Type type = ms.GetRealType();
                    if (type.IsEnum && val is string)
                    {
                        val = Enum.Parse(type, val.ToString());
                    }
                    else if (type.IsSubclassOf(typeof (EnumrableSetting)))
                    {
                        // There is no need to worry, but if we can check, it's better
                    }
                    else if (type.IsSubclassOf(typeof (TreeViewSetting)))
                    {
                        // There is no need to worry, but if we can check, it's better
                    }
                    else
                    {
                        if (val.ToString().Length > 0)
                        {
                            object testObj = Convert.ChangeType(val, type);
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    throw new Exception(String.Format("Giá trị nhập cho {0}: {1} không hợp lệ", ms.FriendlyName, val),
                                        ex);
                }
                _activeSection.Settings[ms.Name] = val.ToString();
            }
        }

        private void ValidateSettings()
        {
            if (_activeSection != null)
            {
                ModuleBase moduleInstance = base.ModuleLoader.GetModuleFromSection(_activeSection);
                moduleInstance.ValidateSectionSettings();
            }
        }


        private void SetRoles()
        {
            _activeSection.SectionPermissions.Clear();
            foreach (RepeaterItem ri in rptRoles.Items)
            {
                // HACK: RoleId is stored in the ViewState because the repeater doesn't have a DataKeys property.
                CheckBox chkView = (CheckBox) ri.FindControl("chkViewAllowed");
                CheckBox chkEdit = (CheckBox) ri.FindControl("chkEditAllowed");
                CheckBox chkInsertAllowed = (CheckBox) ri.FindControl("chkInsertAllowed");
                CheckBox chkModifyAllowed = (CheckBox) ri.FindControl("chkModifyAllowed");
                CheckBox chkDeleteAllowed = (CheckBox) ri.FindControl("chkDeleteAllowed");
                if (chkView.Checked || chkEdit.Checked)
                {
                    SectionPermission sp = new SectionPermission();
                    sp.Section = _activeSection;
                    sp.Role = (Role) base.CoreRepository.GetObjectById(typeof (Role), (int) ViewState[ri.ClientID]);
                    sp.ViewAllowed = chkView.Checked;
                    sp.EditAllowed = chkEdit.Checked;
                    sp.ModifyAllowed = chkModifyAllowed.Checked;
                    sp.InsertAllowed = chkInsertAllowed.Checked;
                    sp.DeleteAllowed = chkDeleteAllowed.Checked;
                    _activeSection.SectionPermissions.Add(sp);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (ActiveNode != null)
            {
                Context.Response.Redirect(String.Format("NodeEdit.aspx?NodeId={0}", ActiveNode.Id));
            }
            else
            {
                Context.Response.Redirect("Sections.aspx");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Remember the previous PlaceholderId and Position to detect changes
                string oldPlaceholderId = _activeSection.PlaceholderId;
                int oldPosition = _activeSection.Position;

                try
                {
                    _activeSection.Title = txtTitle.Text;
                    _activeSection.ShowTitle = chkShowTitle.Checked;
                    _activeSection.Node = ActiveNode;
                    if (ActiveNode != null)
                    {
                        _activeSection.Node.Sections.Add(_activeSection);
                    }
                    if (ddlModule.Visible)
                    {
                        _activeSection.ModuleType = (ModuleType) CoreRepository.GetObjectById(
                                                                     typeof (ModuleType),
                                                                     Int32.Parse(ddlModule.SelectedValue));
                    }
                    _activeSection.PlaceholderId = ddlPlaceholder.SelectedValue;
                    _activeSection.CacheDuration = Int32.Parse(txtCacheDuration.Text);

                    // Calculate new position if the section is new or when the PlaceholderId has changed
                    if (_activeSection.Id == -1 || _activeSection.PlaceholderId != oldPlaceholderId)
                    {
                        _activeSection.CalculateNewPosition();
                    }

                    // Custom settings
                    SetCustomSettings();

                    // Validate settings
                    ValidateSettings();

                    // Roles
                    SetRoles();

                    // Detect a placeholderId change and change positions of adjacent sections if necessary.					
                    if (oldPosition != -1 && oldPlaceholderId != _activeSection.PlaceholderId)
                        _activeSection.ChangeAndUpdatePositionsAfterPlaceholderChange(oldPlaceholderId, oldPosition,
                                                                                      true);

                    // Save the active section
                    SaveSection();

                    // Clear cached sections.
                    base.CoreRepository.ClearCollectionCache("CMS.Core.Domain.Node.Sections");

                    ShowMessage("Vùng phân hệ đã được lưu.");
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void rptRoles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Role role = e.Item.DataItem as Role;
            if (role != null)
            {
                CheckBox chkView = (CheckBox) e.Item.FindControl("chkViewAllowed");
                chkView.Checked = _activeSection.ViewAllowed(role);
                CheckBox chkEdit = (CheckBox) e.Item.FindControl("chkEditAllowed");
                CheckBox chkInsertAllowed = (CheckBox) e.Item.FindControl("chkInsertAllowed");
                CheckBox chkModifyAllowed = (CheckBox) e.Item.FindControl("chkModifyAllowed");
                CheckBox chkDeleteAllowed = (CheckBox) e.Item.FindControl("chkDeleteAllowed");
                if (role.HasPermission(AccessLevel.Editor) || role.HasPermission(AccessLevel.Administrator))
                {
                    chkEdit.Checked = _activeSection.EditAllowed(role);
                    chkInsertAllowed.Checked = _activeSection.InsertAllowed(role);
                    chkModifyAllowed.Checked = _activeSection.ModifyAllowed(role);
                    chkDeleteAllowed.Checked = _activeSection.DeleteAllowed(role);
                }
                else
                {
                    chkEdit.Visible = false;
                    chkInsertAllowed.Visible = false;
                    chkModifyAllowed.Visible = false;
                    chkDeleteAllowed.Visible = false;
                }
                // Add RoleId to the ViewState with the ClientID of the repeateritem as key.
                ViewState[e.Item.ClientID] = role.Id;
            }
        }

        private void rptConnections_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteConnection")
            {
                string actionName = e.CommandArgument.ToString();

                try
                {
                    _activeSection.Connections.Remove(actionName);
                    base.CoreRepository.UpdateObject(_activeSection);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
                BindConnections();
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
            // Load the section here because we need to create dynamic controls based on the 
            // ModuleType of the section. This method comes after base.OnInit because there,
            // the ActiveNode is set and we need that to attach that one to the section.
            LoadSection();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rptConnections.ItemCommand +=
                new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.rptConnections_ItemCommand);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion
    }
}
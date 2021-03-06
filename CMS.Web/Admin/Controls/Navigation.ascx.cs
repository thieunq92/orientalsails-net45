using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.Web.Admin.UI;

namespace CMS.Web.Admin.Controls
{
    /// <summary>
    ///		Summary description for Navigation.
    /// </summary>
    public class Navigation : UserControl
    {
        private AdminBasePage _page;
        protected HyperLink hplModules;

        protected HyperLink hplNew;
        protected HyperLink hplRebuild;
        protected HyperLink hplRoles;
        protected HyperLink hplSections;
        protected HyperLink hplTemplates;
        protected HyperLink hplUsers;
        protected Image i1;
        protected Image i2;
        protected Image i3;
        protected Image i4;
        protected Image i5;
        protected Image i6;
        protected Image i7;
        protected Image inew;
        protected PlaceHolder plhNodes;

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _page = (AdminBasePage) Page;
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("This control requires a Page of the type AdminBasePage.", ex);
            }
            BuildNodeTree();
        }

        private void BuildNodeTree()
        {
            IList sites = _page.SiteService.GetAllSites();
            DisplaySites(sites);
        }

        private void DisplaySites(IList sites)
        {
            foreach (Site site in sites)
            {
                plhNodes.Controls.Add(CreateDisplaySite(site));
                DisplayNodes(site.RootNodes);
                if (_page.ActiveNode != null)
                {
                    plhNodes.Controls.Add(new LiteralControl("<br/>"));
                    plhNodes.Controls.Add(CreateNewChildNodeControl());
                }
                plhNodes.Controls.Add(new LiteralControl("<br/>"));
                plhNodes.Controls.Add(CreateNewNodeControl(site));
                plhNodes.Controls.Add(new LiteralControl("<br/>"));
            }
        }

        private Control CreateNewChildNodeControl()
        {
            HtmlGenericControl container = new HtmlGenericControl("div");
            if (_page.ActiveNode != null)
            {
                container.Attributes.Add("class", "node");
                container.Attributes.Add("style", String.Format("padding-left: {0}px", 20));
                Image img = new Image();
                img.ImageUrl = "../Images/new.gif";
                img.ImageAlign = ImageAlign.Left;
                img.AlternateText = Resources.Strings.textAddNewChildNode;
                container.Controls.Add(img);
                HyperLink hpl = new HyperLink();
                hpl.Text = Resources.Strings.textAddNewChildNode;
                hpl.NavigateUrl = String.Format("../NodeEdit.aspx?NodeId=-1&ParentNodeId={0}", _page.ActiveNode.Id);
                hpl.CssClass = "nodeLink";
                container.Controls.Add(hpl);
            }
            return container;
        }

        private void DisplayNodes(IList nodes)
        {
            foreach (Node node in nodes)
            {
                plhNodes.Controls.Add(CreateDisplayNode(node));
                if (_page.ActiveNode != null
                    && node.Level <= _page.ActiveNode.Level
                    && node.Id == _page.ActiveNode.Trail[node.Level])
                {
                    // The node is in the trail, expand.
                    DisplayNodes(node.ChildNodes);
                    if (_page.ActiveNode.Id == node.Id)
                    {
                        // HACK: Replace the activenode with the one found while building the node tree to reduce future 
                        // database calls.
                        _page.ActiveNode = node;
                    }
                }
            }
        }

        private Control CreateDisplaySite(Site site)
        {
            HtmlGenericControl container = new HtmlGenericControl("div");
            container.Attributes.Add("class", "site");
            Image img = new Image();
            img.ImageUrl = "../Images/internet.gif";
            img.ImageAlign = ImageAlign.Left;
            img.AlternateText = "Site";
            container.Controls.Add(img);
            HyperLink hpl = new HyperLink();
            hpl.Text = String.Format("{0} ({1})", site.Name, site.SiteUrl);
            hpl.NavigateUrl = String.Format("../SiteEdit.aspx?SiteId={0}", site.Id.ToString());
            hpl.CssClass = "nodeLink";
            container.Controls.Add(hpl);
            return container;
        }

        private Control CreateNewNodeControl(Site site)
        {
            HtmlGenericControl container = new HtmlGenericControl("div");
            container.Attributes.Add("class", "node");
            container.Attributes.Add("style", String.Format("padding-left: {0}px", 20));
            Image img = new Image();
            img.ImageUrl = "../Images/new.gif";
            img.ImageAlign = ImageAlign.Left;
            img.AlternateText = Resources.Strings.textAddNewRootNode;
            container.Controls.Add(img);
            HyperLink hpl = new HyperLink();
            hpl.Text = Resources.Strings.textAddNewRootNode;
            hpl.NavigateUrl = String.Format("../NodeEdit.aspx?SiteId={0}&NodeId=-1", site.Id.ToString());
            hpl.CssClass = "nodeLink";
            container.Controls.Add(hpl);
            return container;
        }

        private Control CreateDisplayNode(Node node)
        {
            string nodeText;
            string imgUrl;
            // Display root nodes with their culture.
            if (node.Level == 0)
            {
                nodeText = node.Title + " (" + node.Culture + ")";
                imgUrl = "../Images/home.gif";
            }
            else
            {
                nodeText = node.Title;
                if (node.ShowInNavigation)
                {
                    imgUrl = "../Images/doc2.gif";
                }
                else
                {
                    imgUrl = "../Images/doc2-disabled.gif";
                }
            }
            int indent = node.Level*20 + 20;
            HtmlGenericControl container = new HtmlGenericControl("div");
            container.Attributes.Add("class", "node");
            container.Attributes.Add("style", String.Format("padding-left: {0}px", indent.ToString()));
            Image img = new Image();
            img.ImageUrl = imgUrl;
            img.ImageAlign = ImageAlign.Left;
            img.AlternateText = "Node";
            container.Controls.Add(img);
            if (_page.ActiveNode != null && node.Id == _page.ActiveNode.Id)
            {
                Label lbl = new Label();
                lbl.CssClass = "nodeActive";
                lbl.Text = nodeText;
                container.Controls.Add(lbl);
            }
            else
            {
                HyperLink hpl = new HyperLink();
                hpl.Text = nodeText;
                hpl.NavigateUrl = String.Format("../NodeEdit.aspx?NodeId={0}", node.Id.ToString());
                hpl.CssClass = "nodeLink";
                container.Controls.Add(hpl);
            }
            return container;
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
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion
    }
}
using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.Web.UI;
using CMS.Web.Util;

namespace CMS.Web.Controls.Navigation
{
    public partial class HierarchicalMenuLevel2 : UserControl
    {
        private PageEngine _page;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Page is PageEngine)
            {
                _page = (PageEngine)Page;
                BuildNavigationTree();
            }
        }

        private void BuildNavigationTree()
        {
            HtmlGenericControl mainList = new HtmlGenericControl("ul");
            IList NodeList;

            NodeList = _page.RootNode.ChildNodes;
            foreach (Node root in NodeList)
            {
                //if (root.ChildNodes.Count>0 && ((Node) root.ChildNodes[0]).ChildNodes.Count >0)
                //{
                //    foreach (Node node in root.ChildNodes)
                //    {
                //        if (node.ShowInNavigation && node.ViewAllowed(_page.CuyahogaUser))
                //        {
                //            HtmlControl listItem;
                //            if (node.ChildNodes.Count > 0)
                //            {
                //                listItem = BuildListItemButNotLink(node);
                //            }
                //            else
                //            {
                //                listItem = BuildListItemFromNode(node);
                //            }
                //            if (node.ChildNodes.Count > 0)
                //            {
                //                listItem.Controls.Add(BuildListFromNodes(node.ChildNodes));
                //            }
                //            mainList.Controls.Add(listItem);
                //        }
                //    }
                //}

                if (root.ChildNodes.Count > 0 && CheckNode(root))
                {
                    foreach (Node node in root.ChildNodes)
                    {
                        if (node.ShowInNavigation && node.ViewAllowed(_page.CuyahogaUser))
                        {
                            HtmlControl listItem;
                            if (node.ChildNodes.Count > 0)
                            {
                                listItem = BuildListItemButNotLink(node);
                            }
                            else
                            {
                                listItem = BuildListItemFromNode(node);
                            }
                            if (node.ChildNodes.Count > 0)
                            {
                                listItem.Controls.Add(BuildListFromNodes(node.ChildNodes));
                            }
                            mainList.Controls.Add(listItem);
                        }
                    }
                }
                //else if (root.ChildNodes.Count > 0)
                //{
                //    HtmlControl listItem;
                //    listItem = BuildListItemButNotLink(root);
                //    listItem.Controls.Add(BuildListFromNodes(root.ChildNodes));
                //    mainList.Controls.Add(listItem);
                //}
            }
            plhNodes.Controls.Add(mainList);
        }

        private bool CheckNode(Node root)
        {
            Node currentNode = _page.ActiveNode;
            while (currentNode.ParentNode!=null)
            {
                if (currentNode.ParentNode == root)
                {
                    return true;
                }
                currentNode = currentNode.ParentNode;
            }
            return false;
        }

        private HtmlControl BuildListItemFromNode(Node node)
        {
            HtmlGenericControl listItem = new HtmlGenericControl("li");
            HyperLink hpl = new HyperLink();
            hpl.NavigateUrl = UrlHelper.GetUrlFromNode(node);
            UrlHelper.SetHyperLinkTarget(hpl, node);
            hpl.Text = node.Title;
            // Little dirty trick to highlight the active item :)
            if (node.Id == _page.ActiveNode.Id && node.ParentNode!=_page.RootNode)
            {
                hpl.Attributes.Add("class","subselected");
            }
            listItem.Controls.Add(hpl);
            return listItem;
        }

        /// <summary>
        /// Hàm lấy tên node làm li item
        /// </summary>
        /// <param name="node">Node cần lấy</param>
        /// <returns>Trả về một HtmlControl là HyperLink nhưng không có Url, trick: để nó không là link </returns>
        private HtmlControl BuildListItemButNotLink(Node node)
        {
            HtmlGenericControl listItem = new HtmlGenericControl("li");
            HyperLink hpl = new HyperLink();
            hpl.Text = node.Title;
            // Little dirty trick to highlight the active item :)
            if (node.Id == _page.ActiveNode.Id && node.ParentNode != _page.RootNode)
            {
                hpl.CssClass = "subselected";
            }
            listItem.Controls.Add(hpl);
            return listItem;
        }

        private HtmlControl BuildListFromNodes(IList nodes)
        {
            HtmlGenericControl list = new HtmlGenericControl("ul");
            foreach (Node node in nodes)
            {
                if (node.ViewAllowed(_page.CuyahogaUser) && node.ShowInNavigation)
                {
                    HtmlControl listItem = BuildListItemFromNode(node);
                    if (node.Level <= _page.ActiveNode.Level
                        && node.Id == _page.ActiveNode.Trail[node.Level]
                        && node.ChildNodes.Count > 0)
                    {
                        listItem.Controls.Add(BuildListFromNodes(node.ChildNodes));
                    }
                    list.Controls.Add(listItem);
                }
            }
            return list;
        }
    }
}
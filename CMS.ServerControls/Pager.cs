using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace CMS.ServerControls
{
    /// <summary>
    /// The Pager control enables paging WebControls that can bind to a DataSource.
    /// </summary>
    [DefaultProperty("Text"),
     ToolboxData("<{0}:Pager runat=server></{0}:Pager>")]
    public class Pager : WebControl, INamingContainer
    {
        private const int _defaultCacheDuration = 30;
        private const int _defaultMaxDisplayPages = 10;
        private const int _defaultPageSize = 10;
        private const bool _showNumberOfPages = false;

        private readonly int _cacheDuration;
        private Control _controlToPage;

        private bool _isPageChanged; // Biến xác nhận có sự kiện page change
        private PagedDataSource _pagedDataSource;

        #region properties

        /// <summary>
        /// Property ControlToPage (String)
        /// </summary>
        public string ControlToPage
        {
            get
            {
                if (ViewState["ControlToPage"] != null)
                    return ViewState["ControlToPage"].ToString();
                return String.Empty;
            }
            set { ViewState["ControlToPage"] = value; }
        }

        /// <summary>
        /// Property CacheDataSource (bool)
        /// </summary>
        [DefaultValue(false)]
        public bool CacheDataSource
        {
            get
            {
                if (ViewState["CacheDataSource"] != null)
                    return (bool)ViewState["CacheDataSource"];
                return false;
            }
            set { ViewState["CacheDataSource"] = value; }
        }

        /// <summary>
        /// Property CacheParams (string)
        /// </summary>
        [TypeConverter(typeof(ParamsConverter))]
        public string[] CacheVaryByParams
        {
            get
            {
                if (ViewState["CacheVaryByParams"] != null)
                    return (string[])ViewState["CacheVaryByParams"];
                return null;
            }
            set { ViewState["CacheVaryByParams"] = value; }
        }

        /// <summary>
        /// Property CacheDuration (int)
        /// </summary>
        [DefaultValue(_defaultCacheDuration)]
        public int CacheDuration
        {
            get
            {
                if (ViewState["CacheDuration"] != null)
                    return (int)ViewState["CacheDuration"];
                return _cacheDuration;
            }
            set { ViewState["CacheDuration"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public int PageCount
        {
            get { return TotalPages; }
        }

        protected int TotalPages
        {
            get
            {
                if (ViewState["TotalPages"] != null)
                    return (int)ViewState["TotalPages"];
                return _pagedDataSource.PageCount;
            }
            set { ViewState["TotalPages"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get { return _pagedDataSource.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public int CurrentPageIndex
        {
            get
            {
                if (ViewState["CurrentPageIndex"] != null)
                    return (int)ViewState["CurrentPageIndex"];
                return -1;
            }
            set { ViewState["CurrentPageIndex"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(_defaultPageSize)]
        public int PageSize
        {
            get
            {
                if (ViewState["PageSize"] != null)
                    return (int)ViewState["PageSize"];
                return _defaultPageSize;
            }
            set
            {
                ViewState["PageSize"] = value;
                if (_pagedDataSource != null)
                {
                    _pagedDataSource.PageSize = value;
                }
                ChildControlsCreated = false;
            }
        }

        /// <summary>
        /// Allow custom paging (for example, when limiting items with a query)?
        /// </summary>
        [DefaultValue(false)]
        public bool AllowCustomPaging
        {
            get
            {
                if (ViewState["AllowCustomPaging"] != null)
                    return (bool)ViewState["AllowCustomPaging"];
                return _pagedDataSource.AllowCustomPaging;
            }
            set
            {
                _pagedDataSource.AllowCustomPaging = value;
                ViewState["AllowCustomPaging"] = value;
            }
        }

        /// <summary>
        /// Virtual number of items.
        /// </summary>
        [Browsable(false)]
        public int VirtualItemCount
        {
            get
            {
                if (ViewState["VirtualItemCount"] != null)
                    return (int)ViewState["VirtualItemCount"];
                return _pagedDataSource.VirtualCount;
            }
            set
            {
                _pagedDataSource.VirtualCount = value;
                ViewState["VirtualItemCount"] = value;
            }
        }

        /// <summary>
        /// The maximal number of pages that are clickable in in the pager.
        /// If the number of pages exceeds this limit, an option is presented
        /// to navigate to the next (or previous) set of pages.
        /// </summary>
        [DefaultValue(_defaultMaxDisplayPages)]
        public int MaxDisplayPages
        {
            get
            {
                if (ViewState["MaxDisplayPages"] != null)
                    return (int)ViewState["MaxDisplayPages"];
                return _defaultMaxDisplayPages;
            }
            set { ViewState["MaxDisplayPages"] = value; }
        }

        /// <summary>
        /// Hide the pager when there is only one page?
        /// </summary>
        [DefaultValue(false)]
        public bool HideWhenOnePage
        {
            get
            {
                if (ViewState["HideWhenOnePage"] != null)
                    return (bool)ViewState["HideWhenOnePage"];
                return false;
            }
            set { ViewState["HideWhenOnePage"] = value; }
        }

        /// <summary>
        /// The validator causes the form to validate?
        /// </summary>
        [DefaultValue(false)]
        public bool CausesValidation
        {
            get
            {
                if (ViewState["CausesValidation"] != null)
                    return (bool)ViewState["CausesValidation"];
                return false;
            }
            set { ViewState["CausesValidation"] = value; }
        }

        /// <summary>
        /// The link behavior for the pager controls.
        /// </summary>
        [DefaultValue(PagerLinkMode.LinkButton)]
        public PagerLinkMode PagerLinkMode
        {
            get
            {
                if (ViewState["PagerLinkMode"] != null)
                {
                    return (PagerLinkMode)ViewState["PagerLinkMode"];
                }
                return PagerLinkMode.LinkButton;
            }
            set { ViewState["PagerLinkMode"] = value; }
        }

        /// <summary>
        /// Show the number of pages
        /// </summary>
        [DefaultValue(_showNumberOfPages)]
        public bool ShowTotalPages
        {
            get
            {
                if (ViewState["ShowTotalPages"] != null)
                {
                    return (bool)ViewState["ShowTotalPages"];
                }
                return _showNumberOfPages;
            }
            set { ViewState["ShowTotalPages"] = value; }
        }

        /// <summary>
        /// The base url of the page where the pager is put on.
        /// </summary>
        [Description("Set this if you want a specific url for the the pages that are being paged.")]
        public string PageUrl
        {
            get
            {
                if (ViewState["PageUrl"] != null)
                {
                    return (string)ViewState["PageUrl"];
                }
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.RawUrl;
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    // Only set the PageUrl when it is not already in the current url.
                    if (HttpContext.Current.Request.RawUrl.Contains(value))
                    {
                        ViewState["PageUrl"] = HttpContext.Current.Request.RawUrl;
                    }
                    else
                    {
                        ViewState["PageUrl"] = value;
                    }
                }
            }
        }

        #endregion

        #region events

        public event PageChangedEventHandler PageChanged;

        protected virtual void OnPageChanged(PageChangedEventArgs e)
        {
            CurrentPageIndex = e.CurrentPage;
            if (PageChanged != null)
                PageChanged(this, e);
        }

        public event EventHandler CacheEmpty;

        protected virtual void OnCacheEmpty(EventArgs e)
        {
            if (CacheEmpty != null)
                CacheEmpty(this, e);
        }

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pager()
        {
            _cacheDuration = _defaultCacheDuration;
        }

        #region methods

        protected override void OnInit(EventArgs e)
        {
            if (ControlToPage != String.Empty && ControlToPage != null)
            {
                _controlToPage = Parent.FindControl(ControlToPage);
                if (_controlToPage != null)
                    _controlToPage.DataBinding += ControlToPage_DataBinding;
                else
                    throw new NullReferenceException("The ControlToPage was not found on the page.");
            }
            else
                throw new NullReferenceException(
                    "The ControlToPage property has to be set to the ID of another control on the page.");

            InitPagedDataSource();

            // Check if we have pathinfo or querystring parameters that set a specific page number.
            if (HttpContext.Current.Request.QueryString.HasKeys() ||
                !String.IsNullOrEmpty(HttpContext.Current.Request.PathInfo))
            {
                string pathInfoPattern = @"\/page\/(\d+)";
                string queryStringPattern = @"(\?|\&)page=(\d+)$";

                if (Regex.IsMatch(PageUrl, pathInfoPattern))
                {
                    int pageNumber = Int32.Parse(Regex.Match(PageUrl, pathInfoPattern).Groups[1].Value);
                    CurrentPageIndex = pageNumber - 1;
                    _isPageChanged = true;
                }
                else if (Regex.IsMatch(PageUrl, queryStringPattern))
                {
                    int pageNumber = Int32.Parse(Regex.Match(PageUrl, queryStringPattern).Groups[2].Value);
                    CurrentPageIndex = pageNumber - 1;
                    _isPageChanged = true;
                }
            }

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            // Nếu có gán biến trang thì gọi sự kiện thay đổi trang
            if (_isPageChanged)
            {
                OnPageChanged(new PageChangedEventArgs(-1, CurrentPageIndex));
            }
            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            BuildNavigationControls();
            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Site != null && Site.DesignMode)
            {
                writer.Write("1 2 3 ...");
            }
            base.Render(writer);
        }

        private void InitPagedDataSource()
        {
            _pagedDataSource = new PagedDataSource();
            _pagedDataSource.AllowCustomPaging = AllowCustomPaging;
            _pagedDataSource.AllowPaging = true;
            _pagedDataSource.PageSize = PageSize;
            _pagedDataSource.CurrentPageIndex = CurrentPageIndex;
        }

        private void BuildNavigationControls()
        {
            if (_controlToPage != null && CurrentPageIndex != -1)
            {
                int currentPageGroupIndex = GetCurrentPageGroupIndex();
                int totalPageGroups = GetTotalPageGroups();

                // First
                Control firstPageControl = CreateLinkControl(ButtonAction.First, "<<", 0);
                firstPageControl.Visible = (!_pagedDataSource.IsFirstPage);
                Controls.Add(firstPageControl);

                // Prev
                Control prevPageControl = CreateLinkControl(ButtonAction.Prev, "<", CurrentPageIndex - 1);
                prevPageControl.Visible = (!_pagedDataSource.IsFirstPage);
                Controls.Add(prevPageControl);

                // Previous page group
                int prevPageGroupPageIndex = (currentPageGroupIndex - 1) * MaxDisplayPages;
                Control prevGroupControl = CreateLinkControl(ButtonAction.PrevGroup, "...", prevPageGroupPageIndex);
                prevGroupControl.Visible = currentPageGroupIndex > 0;
                Controls.Add(prevGroupControl);

                // Numbers
                int beginPageNumberIndex = currentPageGroupIndex * MaxDisplayPages;
                int endPageNumberIndex = beginPageNumberIndex + MaxDisplayPages - 1;
                if (TotalPages <= endPageNumberIndex)
                {
                    endPageNumberIndex = TotalPages - 1;
                    if (endPageNumberIndex - MaxDisplayPages >= 0)
                    {
                        beginPageNumberIndex = endPageNumberIndex - MaxDisplayPages;
                    }
                }
                for (int i = 0; i < TotalPages; i++)
                {
                    string pageNumberString = Convert.ToString(i + 1);
                    Control numberControl = CreateLinkControl(ButtonAction.Page, pageNumberString, i);
                    if (i >= beginPageNumberIndex && i <= endPageNumberIndex)
                    {
                        if (_pagedDataSource.CurrentPageIndex == i)
                        {
                            if (!(TotalPages == 1 && HideWhenOnePage))
                            {
                                Label currentPageLabel = new Label();
                                currentPageLabel.Text = pageNumberString;
                                currentPageLabel.Font.Bold = true;
                                numberControl = currentPageLabel;
                            }
                            else
                            {
                                numberControl.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        numberControl.Visible = false;
                    }
                    Controls.Add(numberControl);
                }

                // Next page group
                int nextPageGroupPageIndex = (currentPageGroupIndex + 1) * MaxDisplayPages;
                Control nextGroupControl = CreateLinkControl(ButtonAction.NextGroup, "...", nextPageGroupPageIndex);
                nextGroupControl.Visible = currentPageGroupIndex < totalPageGroups - 1;
                Controls.Add(nextGroupControl);

                // Next
                Control nextPageControl = CreateLinkControl(ButtonAction.Next, ">", CurrentPageIndex + 1);
                nextPageControl.Visible = (_pagedDataSource.DataSource == null || !_pagedDataSource.IsLastPage);
                Controls.Add(nextPageControl);

                // Last
                Control lastPageControl = CreateLinkControl(ButtonAction.Last, ">>", TotalPages - 1);
                lastPageControl.Visible = (_pagedDataSource.DataSource == null || !_pagedDataSource.IsLastPage);
                Controls.Add(lastPageControl);

                // Number of pages
                HtmlGenericControl numberOfPage = new HtmlGenericControl("span");
                numberOfPage.InnerHtml = (CurrentPageIndex + 1) + "/" + TotalPages;
                numberOfPage.Attributes.Add("class", "page_count");
                numberOfPage.Visible = (firstPageControl.Visible || lastPageControl.Visible);
                Controls.AddAt(0, numberOfPage);
            }
        }

        private string GetCacheKey()
        {
            string cacheKey = "Pager_" + _controlToPage.ID;
            if (CacheVaryByParams != null && CacheVaryByParams.Length > 0)
            {
                // Add the values of the individual cache parameters to the cache key
                foreach (string param in CacheVaryByParams)
                {
                    cacheKey += "_" + HttpContext.Current.Request.Params[param];
                }
            }
            return cacheKey;
        }

        //private LinkButton CreateLinkButton()
        //{
        //    LinkButton lbt = new LinkButton();
        //    lbt.CausesValidation = CausesValidation;
        //    return lbt;
        //}

        private Control CreateLinkControl(ButtonAction buttonAction, string buttonText, int pageIndex)
        {
            Control control;
            if (PagerLinkMode == PagerLinkMode.LinkButton)
            {
                LinkButton lbt = new LinkButton();
                lbt.CausesValidation = false;
                lbt.ID = buttonAction.ToString();
                lbt.Text = buttonText;

                switch (buttonAction)
                {
                    case ButtonAction.First:
                        lbt.Click += First_Click;
                        break;
                    case ButtonAction.Prev:
                        lbt.Click += Prev_Click;
                        break;
                    case ButtonAction.PrevGroup:
                        lbt.Click += PrevGroup_Click;
                        break;
                    case ButtonAction.Page:
                        // Override ID with the page index. 
                        lbt.ID = pageIndex.ToString();
                        lbt.Click += Number_Click;
                        break;
                    case ButtonAction.NextGroup:
                        lbt.Click += NextGroup_Click;
                        break;
                    case ButtonAction.Next:
                        lbt.Click += Next_Click;
                        break;
                    case ButtonAction.Last:
                        lbt.Click += Last_Click;
                        break;
                }

                control = lbt;
            }
            else
            {
                HyperLink hpl = new HyperLink();
                hpl.EnableViewState = false;
                hpl.Text = buttonText;

                if (PagerLinkMode == PagerLinkMode.HyperLinkPathInfo)
                {
                    hpl.NavigateUrl = GetPageUrlWithPageNumberToPathInfo(pageIndex);
                }
                else
                {
                    hpl.NavigateUrl = GetPageUrlWithPageNumberToQueryString(pageIndex);
                }

                control = hpl;
            }

            return control;
        }


        private string GetPageUrlWithPageNumberToPathInfo(int pageIndex)
        {
            //string pathInfoPagePattern = @"\/page\/\d+$";
            string pathInfoPagePattern = @"\/page\/\d+";
            string urlWithoutPageInfo = Regex.Replace(PageUrl, pathInfoPagePattern, String.Empty);
            if (pageIndex > 0)
            {
                if (urlWithoutPageInfo.Contains("?"))
                {
                    return urlWithoutPageInfo.Insert(urlWithoutPageInfo.IndexOf("?"), String.Format("/page/{0}", pageIndex + 1));
                }
                return urlWithoutPageInfo + String.Format("/page/{0}", pageIndex + 1);
            }
            return urlWithoutPageInfo; // Trang 1 thì không cần phải đường dẫn
        }

        private string GetPageUrlWithPageNumberToQueryString(int pageIndex)
        {
            const string queryStringPagePattern = @"(\?|\&)page=\d+$";
            string urlWithoutPageInfo = Regex.Replace(PageUrl, queryStringPagePattern, String.Empty);
            if (urlWithoutPageInfo.Contains("?"))
            {
                return urlWithoutPageInfo + String.Format("&page={0}", pageIndex + 1);
            }
            return urlWithoutPageInfo + String.Format("?page={0}", pageIndex + 1);
        }

        private int GetCurrentPageGroupIndex()
        {
            return (int)Math.Floor((CurrentPageIndex / (double)MaxDisplayPages));
        }

        private int GetTotalPageGroups()
        {
            double temp = (TotalPages / (double)MaxDisplayPages);
            return (int)Math.Ceiling(temp);
        }

        #endregion

        #region event handlers

        private void ControlToPage_DataBinding(object sender, EventArgs e)
        {
            // Take the datasource and hand it over to the internal PagedDataSource.
            // We need a little reflection here.
            PropertyInfo pi = _controlToPage.GetType().GetProperty("DataSource");
            if (pi != null)
            {
                IEnumerable controlDataSource = (IEnumerable)pi.GetValue(_controlToPage, null);
                // We don't have to do anything special when the datasource of the controlToPage already is 
                // a PagedDataSource.
                if (!(controlDataSource is PagedDataSource))
                {
                    // Maybe we have a cached datasource
                    string cacheKey = GetCacheKey();
                    if (CacheDataSource && controlDataSource == null)
                    {
                        _pagedDataSource = (PagedDataSource)HttpContext.Current.Cache[cacheKey];
                        // The cache can be empty. If so, raise the CacheEmpty event so that the DataSource 
                        // of the controlToPage can be set (again).
                        if (_pagedDataSource == null)
                        {
                            InitPagedDataSource();
                            OnCacheEmpty(EventArgs.Empty);
                            // Re-fetch the DataSource property value
                            controlDataSource = (IEnumerable)pi.GetValue(_controlToPage, null);
                            _pagedDataSource.DataSource = controlDataSource;
                        }
                    }
                    else
                    {
                        _pagedDataSource.DataSource = controlDataSource;
                    }
                    if (CurrentPageIndex == -1)
                        CurrentPageIndex = 0;
                    _pagedDataSource.CurrentPageIndex = CurrentPageIndex;
                    // Don't swap the datasource when using custom paging
                    if (!AllowCustomPaging)
                    {
                        pi.SetValue(_controlToPage, _pagedDataSource, null);
                        // Call databind again, but now with the pageddatasource attached.
                        _controlToPage.DataBind();
                    }
                    TotalPages = _pagedDataSource.PageCount;
                    // ChildControls have to be created again.
                    ChildControlsCreated = false;
                    // Cache the datasource when required
                    if (CacheDataSource)
                    {
                        HttpContext.Current.Cache.Insert(cacheKey, _pagedDataSource, null,
                                                         DateTime.Now.AddSeconds(CacheDuration), TimeSpan.Zero);
                    }
                }
            }
            else
                throw new InvalidOperationException("The ControlToPage doesn't have a DataSource property.");
        }

        private void First_Click(object sender, EventArgs e)
        {
            int nextPage = 0;
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        private void Prev_Click(object sender, EventArgs e)
        {
            int nextPage = CurrentPageIndex - 1;
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        private void Number_Click(object sender, EventArgs e)
        {
            int nextPage = Int32.Parse(((LinkButton)sender).ID);
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        private void Next_Click(object sender, EventArgs e)
        {
            int nextPage = CurrentPageIndex + 1;
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        private void Last_Click(object sender, EventArgs e)
        {
            int nextPage = TotalPages - 1;
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        private void PrevGroup_Click(object sender, EventArgs e)
        {
            int nextPage = (GetCurrentPageGroupIndex() - 1) * MaxDisplayPages;
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        private void NextGroup_Click(object sender, EventArgs e)
        {
            int nextPage = (GetCurrentPageGroupIndex() + 1) * MaxDisplayPages;
            PageChangedEventArgs args = new PageChangedEventArgs(CurrentPageIndex, nextPage);
            OnPageChanged(args);
        }

        #endregion

        #region Nested type: ButtonAction

        private enum ButtonAction
        {
            First,
            Prev,
            PrevGroup,
            Page,
            NextGroup,
            Next,
            Last
        }

        #endregion
    }

    #region PageChangedEvent

    /// <summary>
    /// EventArgs class for Pager events.
    /// </summary>
    public class PageChangedEventArgs
    {
        private readonly int _currentPage;
        private readonly int _prevPage;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prevPage"></param>
        /// <param name="currentPage"></param>
        public PageChangedEventArgs(int prevPage, int currentPage)
        {
            _prevPage = prevPage;
            _currentPage = currentPage;
        }

        /// <summary>
        /// The page index of the previous page.
        /// </summary>
        public int PrevPage
        {
            get { return _prevPage; }
        }

        /// <summary>
        /// The page index of the current page (after clicking next).
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
        }
    }

    /// <summary>
    /// Delegate for pager events.
    /// </summary>
    public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs e);

    #endregion

    #region Type Converters

    public class ParamsConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return new string[0];
            }

            if (value is string)
            {
                string s = (string)value;
                if (s.Length == 0)
                {
                    return new string[0];
                }
                string[] parts = s.Split(culture.TextInfo.ListSeparator[0]);
                return parts;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (value != null)
            {
                if (!(value is string[]))
                {
                    throw new ArgumentException("Invalid string array", "value");
                }
            }

            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return String.Empty;
                }

                string[] prms = (string[])value;
                if (prms.Length == 0)
                {
                    return String.Empty;
                }
                return String.Join(culture.TextInfo.ListSeparator, prms);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion

    /// <summary>
    /// The way pager links are generated.
    /// </summary>
    public enum PagerLinkMode
    {
        /// <summary>
        /// Pager links are linkbuttons and cause a postback
        /// </summary>
        LinkButton,
        /// <summary>
        /// Pager links are plain hyperlinks and add '/page/pagenumber' to the end of the pathinfo
        /// </summary>
        HyperLinkPathInfo,
        /// <summary>
        /// Pager links are plain hyperlink and '&page=pagenumber' to the querystring.
        /// </summary>
        HyperLinkQueryString
    }
}

#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Waveface.Component.MozBar;

#endregion

namespace Waveface.Component.FirefoxDialog
{
    public partial class FirefoxDialog : UserControl
    {
        private PropertyPage m_activePage;

        private Dictionary<string, PageProp> m_pages = new Dictionary<string, PageProp>();

        public FirefoxDialog()
        {
            InitializeComponent();
        }

        #region Private

        private void AddPage(MozItem item, PropertyPage page)
        {
            PageProp _pageProp = new PageProp();

            _pageProp.Page = page;
            _pageProp.MozItem = item;

            mozPane.Items.Add(item);

            m_pages.Add(item.Name, _pageProp);
        }

        private MozItem GetMozItem(string text)
        {
            return GetMozItem(text, ImageList == null ? 0 : m_pages.Count);
        }

        private MozItem GetMozItem(string text, int imageIndex)
        {
            MozItem _item = new MozItem();

            _item.Name = "mozItem" + m_pages.Count + 1;

            _item.Text = text;

            if (imageIndex < ImageList.Images.Count)
            {
                _item.Images.NormalImage = ImageList.Images[imageIndex];
            }

            return _item;
        }

        #region Activate Page

        private void mozPane1_ItemClick(object sender, MozItemClickEventArgs e)
        {
            ActivatePage(e.MozItem);
        }

        private bool ActivatePage(MozItem item)
        {
            if (!m_pages.ContainsKey(item.Name))
            {
                return false;
            }

            PageProp _pageProp = m_pages[item.Name];

            PropertyPage _page = _pageProp.Page;

            if (m_activePage != null)
            {
                m_activePage.Visible = false;
            }

            m_activePage = _page;

            if (m_activePage != null)
            {
                mozPane.SelectByName(item.Name);

                m_activePage.Visible = true;

                if (!_page.IsInit)
                {
                    _page.OnInit();

                    _page.IsInit = true;
                }

                m_activePage.OnSetActive();
            }

            return true;
        }

        #endregion

        #endregion

        #region Public Interface

        #region Properties

        public Dictionary<string, PageProp> Pages
        {
            get
            {
                return m_pages;
            }
        }

        public ImageList ImageList
        {
            get
            {
                return mozPane.ImageList;
            }
            set
            {
                mozPane.ImageList = value;
            }
        }

        #endregion

        #region Methods

        public void AddPage(string text, PropertyPage page)
        {
            AddPage(GetMozItem(text), page);
        }

        public void AddPage(string text, int imageIndex, PropertyPage page)
        {
            AddPage(GetMozItem(text, imageIndex), page);
        }

        public void Init()
        {
            foreach (PageProp _pageProp in m_pages.Values)
            {
                PropertyPage _page = _pageProp.Page;

                pagePanel.Controls.Add(_page);
                _page.Dock = DockStyle.Fill;
                _page.Visible = false;
            }

            if (m_pages.Count != 0)
            {
                ActivatePage(mozPane.Items[0]);
            }
        }

        #endregion

        #endregion

        #region Dialog Buttons

        private void btnOK_Click(object sender, EventArgs e)
        {
            Apply();

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void Apply()
        {
            foreach (PageProp _pageProp in m_pages.Values)
            {
                if (_pageProp.Page.IsInit)
                {
                    _pageProp.Page.OnApply();
                }
            }
        }

        private void Close()
        {
            if (ParentForm != null)
            {
                ParentForm.Close();
            }
        }

        #endregion
    }
}
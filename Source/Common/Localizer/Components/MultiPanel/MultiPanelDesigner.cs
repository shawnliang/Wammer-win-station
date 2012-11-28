#region

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace Waveface.Component.MultiPage.Design
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class MultiPanelDesigner : ParentControlDesigner
    {
        private MultiPanel m_mpanel;
        private DesignerVerbCollection m_verbs;

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (m_verbs == null)
                {
                    m_verbs = new DesignerVerbCollection();
                    m_verbs.Add(new DesignerVerb("Add Page", AddPage));
                    m_verbs.Add(new DesignerVerb("Remove Page", RemovePage));
                }

                return m_verbs;
            }
        }

        public override ICollection AssociatedComponents
        {
            get
            {
                return m_mpanel.Controls;
            }
        }

        public override void Initialize(IComponent component)
        {
            m_mpanel = component as MultiPanel;
           
            if (m_mpanel == null)
            {
                DisplayError(new ArgumentException("Tried to use the MultiPanelControlDesign with a class that does not inherit from MultiPanel.", "component"));
                return;
            }

            base.Initialize(component);

            IComponentChangeService _iccs = (IComponentChangeService) GetService(typeof (IComponentChangeService));
           
            if (_iccs != null)
            {
                _iccs.ComponentRemoved += ComponentRemoved;
            }

            ISelectionService _s = (ISelectionService) GetService(typeof (ISelectionService));
            
            if (_s != null)
            {
                _s.SelectionChanged += s_SelectionChanged;
            }
        }

        public override void DoDefaultAction()
        {
        }

        private void AddPage(object sender, EventArgs ea)
        {
            IDesignerHost _dh = (IDesignerHost) GetService(typeof (IDesignerHost));
            
            if (_dh != null)
            {
                DesignerTransaction _dt = _dh.CreateTransaction("Added new page");

                MultiPanelPage _before = m_mpanel.SelectedPage;

                string _name = GetNewPageName();
                MultiPanelPage _ytp = _dh.CreateComponent(typeof (MultiPanelPage), _name) as MultiPanelPage;
                _ytp.Text = _name;

                m_mpanel.Controls.Add(_ytp);
                m_mpanel.SelectedPage = _ytp;

                RaiseComponentChanging(TypeDescriptor.GetProperties(Control)["SelectedPage"]);
                RaiseComponentChanged(TypeDescriptor.GetProperties(Control)["SelectedPage"], _before, _ytp);

                _dt.Commit();
            }
        }

        private void RemovePage(object sender, EventArgs ea)
        {
            IDesignerHost _dh = (IDesignerHost) GetService(typeof (IDesignerHost));
            
            if (_dh != null)
            {
                DesignerTransaction _dt = _dh.CreateTransaction("Removed page");

                MultiPanelPage _page = m_mpanel.SelectedPage;
               
                if (_page != null)
                {
                    MultiPanelPage _ytp = m_mpanel.SelectedPage;
                    m_mpanel.Controls.Remove(_ytp);
                    _dh.DestroyComponent(_ytp);

                    if (m_mpanel.Controls.Count > 0)
                        m_mpanel.SelectedPage = (MultiPanelPage) m_mpanel.Controls[0];
                    else
                        m_mpanel.SelectedPage = null;

                    RaiseComponentChanging(TypeDescriptor.GetProperties(Control)["SelectedPage"]);
                    RaiseComponentChanged(TypeDescriptor.GetProperties(Control)["SelectedPage"], _ytp, m_mpanel.SelectedPage);
                }

                _dt.Commit();
            }
        }

        private string GetNewPageName()
        {
            int i = 1;
            Hashtable _h = new Hashtable(m_mpanel.Controls.Count);
            
            foreach (Control _c in m_mpanel.Controls)
                _h[_c.Name] = null;

            while (_h.ContainsKey("Page_" + i))
                i++;

            return "Page_" + i;
        }

        #region Private Methods

        private void s_SelectionChanged(object sender, EventArgs e)
        {
            ISelectionService _s = (ISelectionService) GetService(typeof (ISelectionService));
            
            if (_s != null)
            {
                if (_s.PrimarySelection != null)
                {
                    MultiPanelPage _page = GetMultiPanelPage((Control) _s.PrimarySelection);
                   
                    if (_page != null)
                        m_mpanel.SelectedPage = _page;
                }
            }
        }

        private MultiPanelPage GetMultiPanelPage(Control ctrl)
        {
            if (ctrl is MultiPanelPage)
            {
                MultiPanelPage _p = (MultiPanelPage) ctrl;
                
                if (ReferenceEquals(m_mpanel, _p.Parent))
                    return _p;
                else
                    return null;
            }
            else if (ctrl.Parent != null)
                return GetMultiPanelPage(ctrl.Parent);
            else
                return null;
        }

        private void ComponentRemoved(object sender, ComponentEventArgs cea)
        {
        }

        #endregion
    }
}
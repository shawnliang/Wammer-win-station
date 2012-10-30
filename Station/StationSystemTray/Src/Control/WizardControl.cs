using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	/// <summary>
	/// 
	/// </summary>
	public class WizardControl:Control
	{
		#region Var
		private TabControlEx _tabControl;
		private IEnumerable<StepPageControl> _wizardPages;
		private WizardParameters m_parameters = new WizardParameters();
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ tab control.
		/// </summary>
		/// <value>The m_ tab control.</value>
		private TabControlEx m_TabControl
		{
			get
			{
				return _tabControl ?? (_tabControl = new TabControlEx() 
				{
					HideTabs = true,
					Dock = DockStyle.Fill 
				});
			}
		}
		#endregion


		#region Public Property

		public WizardParameters Parameters
		{
			get { return m_parameters; }
		}

		/// <summary>
		/// Gets or sets the wizard pages.
		/// </summary>
		/// <value>The wizard pages.</value>
		public IEnumerable<StepPageControl> WizardPages
		{
			get
			{
				return _wizardPages;
			}
			private set
			{
				if (_wizardPages == value)
					return;
				OnWizardPagesChanging(EventArgs.Empty);
				_wizardPages = value;
				foreach (var page in _wizardPages)
				{
					page.WizardControl = this;
				}
				OnWizardPagesChanged(EventArgs.Empty);
			}
		}

		public StepPageControl GetPage(int index)
		{
			return (StepPageControl)((TabPage)m_TabControl.GetControl(index)).Controls[0];
		}


		/// <summary>
		/// Gets or sets the index of the page.
		/// </summary>
		/// <value>The index of the page.</value>
		public int PageIndex 
		{
			get
			{
				return m_TabControl.SelectedIndex + 1;
			}
			set
			{
				m_TabControl.SelectedIndex = value - 1;
			}
		}

		/// <summary>
		/// Gets the page count.
		/// </summary>
		/// <value>The page count.</value>
		public int PageCount 
		{
			get 
			{
				return m_TabControl.TabPages.Count;
			}
		}

		/// <summary>
		/// Gets the current page.
		/// </summary>
		/// <value>The current page.</value>
		public StepPageControl CurrentPage 
		{
			get
			{
				var selectedTab = m_TabControl.SelectedTab;
				return (selectedTab == null || selectedTab.Controls.Count == 0) ? null : (StepPageControl)selectedTab.Controls[0];
			}
		}
		#endregion


		#region Event
		public event EventHandler WizardPagesChanging;
		public event EventHandler WizardPagesChanged;
		public event EventHandler PageChanged;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="WizardControl"/> class.
		/// </summary>
		public WizardControl()
		{
			m_TabControl.SelectedIndexChanged += new EventHandler(m_TabControl_SelectedIndexChanged);
			this.WizardPagesChanged += new EventHandler(WizardControl_WizardPagesChanged);
			this.Controls.Add(m_TabControl);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WizardControl"/> class.
		/// </summary>
		/// <param name="wizardPages">The wizard pages.</param>
		public WizardControl(IEnumerable<StepPageControl> wizardPages)
			: this()
		{
			SetWizardPages(wizardPages);
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Inits the wizard pages.
		/// </summary>
		private void InitWizardPages()
		{
			InitWizardPages(this.WizardPages);
		}

		/// <summary>
		/// Inits the wizard pages.
		/// </summary>
		/// <param name="wizardPages">The wizard pages.</param>
		private void InitWizardPages(IEnumerable<StepPageControl> wizardPages)
		{
			var tabPages = new List<TabPage>();
			foreach (var wizardPage in wizardPages)
			{
				var tabPage = new TabPage();

				wizardPage.Dock = DockStyle.Fill;
				tabPage.Controls.Add(wizardPage);

				tabPages.Add(tabPage);
			}
			m_TabControl.TabPages.Clear();
			m_TabControl.TabPages.AddRange(tabPages.ToArray());
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:WizardPagesChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnWizardPagesChanging(EventArgs e)
		{
			if (WizardPagesChanging == null)
				return;
			WizardPagesChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:WizardPagesChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnWizardPagesChanged(EventArgs e)
		{
			if (WizardPagesChanged == null)
				return;
			WizardPagesChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:PageChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnPageChanged(EventArgs e)
		{
			if (PageChanged == null)
				return;
			PageChanged(this, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Sets the wizard pages.
		/// </summary>
		/// <param name="wizardPages">The wizard pages.</param>
		public void SetWizardPages(IEnumerable<StepPageControl> wizardPages)
		{
			this.WizardPages = wizardPages;
		}

		/// <summary>
		/// Firsts the page.
		/// </summary>
		public void FirstPage()
		{
			this.PageIndex = 1;
		}

		/// <summary>
		/// Lasts the page.
		/// </summary>
		public void LastPage()
		{
			this.PageIndex = this.PageCount;
		}

		/// <summary>
		/// Previouses the page.
		/// </summary>
		public void PreviousPage()
		{
			var pageIndex = this.PageIndex;
			if (pageIndex <= 1)
				return;

			this.PageIndex = pageIndex - 1;

			CurrentPage.OnEnteringStep(m_parameters);
		}

		/// <summary>
		/// Nexts the page.
		/// </summary>
		public void NextPage()
		{
			var pageIndex = this.PageIndex;
			if (pageIndex >= this.PageCount)
				return;

			CurrentPage.OnLeavingStep(m_parameters);
			this.PageIndex = pageIndex + 1;
			CurrentPage.OnEnteringStep(m_parameters);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the WizardPagesChanged event of the WizardControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void WizardControl_WizardPagesChanged(object sender, EventArgs e)
		{
			InitWizardPages();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the m_TabControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void m_TabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnPageChanged(EventArgs.Empty);
		}
		#endregion
	}
}

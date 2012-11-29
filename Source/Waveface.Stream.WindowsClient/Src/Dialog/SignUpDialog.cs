using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public partial class SignUpDialog : Form
	{
		#region Var
		private List<Image> _enTutorials;
		private List<Image> _twTutorials;
		private List<Image> _tutorials;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets the m_ EN tutorials.
		/// </summary>
		/// <value>The m_ EN tutorials.</value>
		private List<Image> m_ENTutorials
		{
			get
			{
				return _enTutorials ?? (_enTutorials = new List<Image>() { Resources.windows_en_1, Resources.windows_en_2 });
			}
		}

		/// <summary>
		/// Gets the m_ TW tutorials.
		/// </summary>
		/// <value>The m_ TW tutorials.</value>
		private List<Image> m_TWTutorials
		{
			get
			{
				return _twTutorials ?? (_twTutorials = new List<Image>() { Resources.windows_zh_tw_1, Resources.windows_zh_tw_2 });
			}
		}

		/// <summary>
		/// Gets the m_ tutorials.
		/// </summary>
		/// <value>The m_ tutorials.</value>
		private List<Image> m_Tutorials
		{
			get
			{
				return _tutorials ?? (_tutorials = (Thread.CurrentThread.CurrentCulture.Name == "zh-TW") ? m_TWTutorials : m_ENTutorials);
			}
		}

		/// <summary>
		/// Gets or sets the index of the m_.
		/// </summary>
		/// <value>The index of the m_.</value>
		private int m_Index { get; set; }
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the browser.
		/// </summary>
		/// <value>The browser.</value>
		public WebBrowser Browser
		{
			get
			{
				return webBrowser1;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="SignUpDialog"/> class.
		/// </summary>
		public SignUpDialog()
		{
			InitializeComponent();
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Canges the tutorial.
		/// </summary>
		/// <param name="index">The index.</param>
		private void CangeTutorial(int index)
		{
			if (index < 0)
				return;

			if (m_Tutorials.Count <= index)
				return;

			pictureBox1.BackgroundImage = m_Tutorials[index];
			m_Index = index;

			AdjustButton();
		}

		/// <summary>
		/// Adjusts the button.
		/// </summary>
		private void AdjustButton()
		{
			button1.Text = (m_Index + 1 == m_Tutorials.Count) ? Resources.CLOSE_BUTTON_TEXT : Resources.NEXT_BUTTON_TEXT;
			button2.Text = Resources.PREVIOUS_BUTTON_TEXT;
			button2.Visible = m_Index > 0;
		}
		#endregion


		#region Protected Method
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//prevent ctrl+tab to switch signin pages
			if (keyData == (Keys.Control | Keys.Tab))
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion


		#region Public Method
		public void ShowTutorial()
		{
			tabControlEx1.SelectedTab = tabPage2;
		}

		public void ShowSignUpPage()
		{
			tabControlEx1.SelectedTab = tabPage1;
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Load event of the TutorialDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TutorialDialog_Load(object sender, EventArgs e)
		{
			CangeTutorial(0);
		}

		/// <summary>
		/// Handles the Click event of the button1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button1_Click(object sender, EventArgs e)
		{
			if (m_Index + 1 == m_Tutorials.Count)
				this.Close();
			else
				CangeTutorial(m_Index + 1);
		}

		/// <summary>
		/// Handles the Click event of the button2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button2_Click(object sender, EventArgs e)
		{
			CangeTutorial(m_Index - 1);
		} 
		#endregion
	}
}

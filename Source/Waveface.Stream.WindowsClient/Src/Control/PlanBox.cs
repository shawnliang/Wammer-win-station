using System;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public class PlanBox : Control
	{
		#region Enum
		public enum PlanType
		{
			Free,
			Plan1,
			Plan2
		}
		#endregion

		#region Var
		private PlanControl _planControl;
		private PlanType _type;
		#endregion

		#region Private Property
		private PlanControl m_PlanControl
		{
			get
			{
				return _planControl ?? (_planControl = new PlanControl());
			}
		}
		#endregion


		#region Public Property
		public PlanType Type
		{
			get
			{
				return _type;
			}
			set
			{
				if (_type == value)
					return;

				_type = value;
				UpdateUI();
			}
		}

		public Boolean HeaderVisibile
		{
			get
			{
				return m_PlanControl.HeaderVisible;
			}
			set
			{
				m_PlanControl.HeaderVisible = value;
			}
		}
		#endregion


		#region Constructor
		public PlanBox()
		{
			m_PlanControl.Dock = DockStyle.Fill;

			this.Controls.Add(m_PlanControl);

			UpdateUI();
		}
		#endregion


		#region Private Method
		private void UpdateUI()
		{
			switch (this.Type)
			{
				case PlanType.Free:
					m_PlanControl.HeaderIconText = "Free";
					m_PlanControl.HeaderText = Resources.FREE_PLAN_HEADER;
					m_PlanControl.RTFDescription = Resources.FreePlan;
					break;
				case PlanType.Plan1:
					m_PlanControl.HeaderIconText = "250";
					m_PlanControl.HeaderText = Resources.PLAN1_HEADER;
					m_PlanControl.RTFDescription = Resources.Plan1;
					break;
				case PlanType.Plan2:
					m_PlanControl.HeaderIconText = "500";
					m_PlanControl.HeaderText = Resources.PLAN2_HEADER;
					m_PlanControl.RTFDescription = Resources.Plan2;
					break;
				default:
					break;
			}
		}
		#endregion

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);

		}
	}
}

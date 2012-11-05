using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StationSystemTray
{
	public partial class StepNavigator : UserControl
	{
		#region Var
		private int _stepCount;
		private int _stepIndex;
		private bool _enableManualNavigate;
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the step count.
		/// </summary>
		/// <value>The step count.</value>
		public int StepCount
		{
			get
			{
				return _stepCount;
			}
			set
			{
				if (_stepCount == value)
					return;

				OnStepCountChanging(EventArgs.Empty);
				_stepCount = value;
				OnStepCountChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the index of the step.
		/// </summary>
		/// <value>The index of the step.</value>
		public int StepIndex
		{
			get
			{
				return _stepIndex;
			}
			set 
			{
				if (_stepIndex == value)
					return;

				OnStepIndexChanging(EventArgs.Empty);
				_stepIndex = value;

				if (centralLayoutPanel1.Controls.Count > 0)
				{
					var curStepRadio = centralLayoutPanel1.Controls[_stepIndex - 1] as RadioButton;
					curStepRadio.Checked = true;
				}

				OnStepIndexChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [enable manual navigate].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [enable manual navigate]; otherwise, <c>false</c>.
		/// </value>
		public bool EnableManualNavigate
		{
			get
			{
				return _enableManualNavigate;
			}
			set
			{
				if (_enableManualNavigate == value)
					return;

				OnManualNavigateEnableStateChanging(EventArgs.Empty);
				_enableManualNavigate = value;
				OnManualNavigateEnableStateChanged(EventArgs.Empty);
			}
		}
		#endregion


		#region Event
		public event EventHandler StepCountChanging;
		public event EventHandler StepCountChanged;
		public event EventHandler StepIndexChanging;
		public event EventHandler StepIndexChanged;
		public event EventHandler ManualNavigateEnableStateChanging;
		public event EventHandler ManualNavigateEnableStateChanged;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="StepNavigator"/> class.
		/// </summary>
		public StepNavigator()
		{
			InitializeComponent();
			AdjustSteps();

			this.StepCountChanged += new EventHandler(StepNavigator_StepCountChanged);
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Adjusts the steps.
		/// </summary>
		private void AdjustSteps()
		{
			centralLayoutPanel1.Controls.Clear();

			if (StepCount == 0)
				return;

			var stepControls = Enumerable.Range(1, StepCount).Select((index) => new RadioButton()
			{
				AutoSize = true,
				Padding = new System.Windows.Forms.Padding(10, 0, 10, 0),
				Tag = index
			}).ToArray();

			centralLayoutPanel1.Controls.AddRange(stepControls);

			foreach (var stepControl in stepControls)
			{
				stepControl.DataBindings.Add("Enabled", this, "EnableManualNavigate");

				stepControl.CheckedChanged += new EventHandler(stepControl_CheckedChanged);
				stepControl.ParentChanged += new EventHandler(stepControl_ParentChanged);
			}

			var firstStepControl = stepControls.FirstOrDefault();
			firstStepControl.Checked = true;
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:StepCountChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnStepCountChanging(EventArgs e)
		{
			if (StepCountChanging == null)
				return;

			StepCountChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:StepCountChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnStepCountChanged(EventArgs e)
		{
			if (StepCountChanged == null)
				return;

			StepCountChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:StepIndexChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnStepIndexChanging(EventArgs e)
		{
			if (StepIndexChanging == null)
				return;

			StepIndexChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:StepIndexChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnStepIndexChanged(EventArgs e)
		{
			if (StepIndexChanged == null)
				return;

			StepIndexChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:ManualNavigateEnableStateChanging"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnManualNavigateEnableStateChanging(EventArgs e)
		{
			if (ManualNavigateEnableStateChanging == null)
				return;

			ManualNavigateEnableStateChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:ManualNavigateEnableStateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnManualNavigateEnableStateChanged(EventArgs e)
		{
			if (ManualNavigateEnableStateChanged == null)
				return;

			ManualNavigateEnableStateChanged(this, e);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the CheckedChanged event of the stepControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void stepControl_CheckedChanged(object sender, EventArgs e)
		{
			var stepControl = sender as RadioButton;

			if (stepControl.Checked)
			{
				StepIndex = (int)stepControl.Tag;
			}
		}

		/// <summary>
		/// Handles the ParentChanged event of the stepControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void stepControl_ParentChanged(object sender, EventArgs e)
		{
			var stepControl = sender as RadioButton;

			stepControl.DataBindings.Clear();
			stepControl.CheckedChanged -= new EventHandler(stepControl_CheckedChanged);
		}

		/// <summary>
		/// Handles the StepCountChanged event of the StepNavigator control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void StepNavigator_StepCountChanged(object sender, EventArgs e)
		{
			AdjustSteps();
		} 
		#endregion
	}
}

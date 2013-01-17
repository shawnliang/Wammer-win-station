using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using System.Collections.Generic;

namespace Waveface.Stream.WindowsClient
{
	public partial class PersonalCloudStatusControl2 : StepPageControl
	{
		#region Var
		private IPersonalCloudStatus service;
		private string user_id;
		private string session_token;
		private Timer _refreshTimer;
		private object cs = new object(); 
		#endregion

		#region Private Property
		public Timer m_RefreshTimer
		{
			get 
			{
				if (_refreshTimer == null)
				{
					_refreshTimer = new Timer();
					_refreshTimer.Interval = 2000;
					_refreshTimer.Tick += timer1_Tick;
				}
				return _refreshTimer;
			}
			set
			{
				if (_refreshTimer != null)
					_refreshTimer.Dispose();

				_refreshTimer = value;
			}
		}
		#endregion

		#region Public Property
		public Boolean EnableAutoRefreshStatus 
		{
			get
			{
				return m_RefreshTimer.Enabled;
			}
			set
			{
				m_RefreshTimer.Enabled = value;
			}
		}
		#endregion

		public PersonalCloudStatusControl2()
			: this(new PersonalCloudStatusService())
		{
		}

		public PersonalCloudStatusControl2(IPersonalCloudStatus service)
		{
			InitializeComponent();

			CustomSize = new Size(710, 437);
			this.service = service;
			this.CustomLabelForNextStep = "Start Stream!";
		}

		private void updateStatus()
		{
			lock (cs)
			{
				try
				{
					var nodes = service.GetNodes(user_id, session_token, StationAPI.API_KEY);

					this.Invoke(new MethodInvoker(() =>
					{
						formatNodes(nodes);
					}));
				}
				catch (Exception ex)
				{
				}
			}
		}

		private void formatNodes(IEnumerable<PersonalCloudNode> nodes)
		{
			try
			{
				listView1.SuspendLayout();
				listView1.BeginUpdate();

				foreach (var node in nodes)
				{

					if (listView1.Items.ContainsKey(node.Id))
					{
						var item = listView1.Items[node.Id];
						if (!item.Text.Equals(node.Name))
							item.Text = node.Name;

						var sub1 = item.SubItems["profile"];
						if (!sub1.Text.Equals(node.Profile))
							sub1.Text = node.Profile;
					}
					else
					{
						listView1.Items.Add(node.Id, node.Name, 0);

						var item = listView1.Items[node.Id];

						item.SubItems.Add(
							new ListViewItem.ListViewSubItem
							{
								Name = "profile",
								Text = node.Profile
							});

						if (node.Type == NodeType.Station)
							item.Group = listView1.Groups["station"];
						else if (node.Type == NodeType.Tablet)
							item.Group = listView1.Groups["tablet"];
						else if (node.Type == NodeType.Phone)
							item.Group = listView1.Groups["phone"];


					}
				}


				int phoneCount = nodes.Count((x) => x.Type == NodeType.Phone);

				if (phoneCount == 0)
				{
					if (!listView1.Items.ContainsKey("no phone"))
					{
						var defaultItem = listView1.Items.Add("no phone", "You have no linked phone.", 0);
						defaultItem.Group = listView1.Groups["phone"];
						defaultItem.ForeColor = Color.DimGray;
					}

				}
				else
				{
					if (listView1.Items.ContainsKey("no phone"))
						listView1.Items.RemoveByKey("no phone");
				}

				int tabletCount = nodes.Count((x) => x.Type == NodeType.Tablet);

				if (tabletCount == 0)
				{
					if (!listView1.Items.ContainsKey("no tablet"))
					{
						var defaultItem = listView1.Items.Add("no tablet", "You have no linked tablet.", 0);
						defaultItem.Group = listView1.Groups["tablet"];
						defaultItem.ForeColor = Color.DimGray;
					}
				}
				else
				{
					if (listView1.Items.ContainsKey("no tablet"))
						listView1.Items.RemoveByKey("no tablet");
				}
			}
			finally
			{
				listView1.EndUpdate();
				listView1.ResumeLayout();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Waveface.Stream.ClientFramework.UserInfo.Instance.Clear();
			updateStatus();
		}

		private void PersonalCloudStatusControl2_Load(object sender, EventArgs e)
		{
			if (this.IsDesignMode())
				return;

			listView1.SetDoubleBuffered();

			listView1.Columns[1].Width = listView1.ClientSize.Width - listView1.Columns[0].Width;

			var user = StreamClient.Instance.LoginedUser;
			user_id = user.UserID;
			session_token = user.SessionToken;

			updateStatus();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var dialog = new AddDeviceDialog();
			dialog.ShowDialog(this);
		}
	}
}

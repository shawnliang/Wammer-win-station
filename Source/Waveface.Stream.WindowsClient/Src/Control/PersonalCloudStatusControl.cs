using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;

namespace Waveface.Stream.WindowsClient
{
	public partial class PersonalCloudStatusControl2 : UserControl
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
					_refreshTimer.Interval = 15000;
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

		public int RefreshInterval
		{
			get
			{
				return m_RefreshTimer.Interval;
			}
			set
			{
				m_RefreshTimer.Interval = value;
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
			this.service = service;
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
							item.Group = listView1.Groups["computers"];
						else
							item.Group = listView1.Groups["devices"];
					}
				}


				int deviceCount = nodes.Count((x) => x.Type != NodeType.Station);

				if (deviceCount == 0)
				{
					if (!listView1.Items.ContainsKey("no device"))
					{
						var defaultItem = listView1.Items.Add("no device", "You have no linked device.", 0);
						defaultItem.Group = listView1.Groups["devices"];
						defaultItem.ForeColor = Color.DimGray;
					}

				}
				else
				{
					if (listView1.Items.ContainsKey("no device"))
						listView1.Items.RemoveByKey("no device");
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

		private void btnGetApp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Todo: open aostream download web page");
		}

		private void btnInstallChromeExtension_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Todo: open aostream chrome extension page on Chrome Web Store");
		}
	}
}

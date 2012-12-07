using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;

namespace Waveface.Stream.WindowsClient
{
	public partial class PersonalCloudStatusControl2 : StepPageControl
	{
		private IPersonalCloudStatus service;
		private string user_id;
		private string session_token;
		private Timer timer;
		private object cs = new object();

		public PersonalCloudStatusControl2(IPersonalCloudStatus service)
		{
			InitializeComponent();
			CustomSize = new Size(710, 437);
			this.service = service;
			this.CustomLabelForNextStep = "Start Stream!";
		}

		public override void OnEnteringStep(WizardParameters parameters)
		{
			user_id = parameters.Get("user_id") as string;
			session_token = parameters.Get("session_token") as string;

			listView1.Items.Clear();
			updateStatus();

			timer = new Timer();
			timer.Interval = 2000;
			timer.Tick += timer1_Tick;
			timer.Start();
		}

		public override void OnLeavingStep(WizardParameters parameters)
		{
		}

		private void updateStatus()
		{
			try
			{
				var nodes = service.GetNodes(user_id, session_token, StationAPI.API_KEY);
				nodes = nodes.ToList();
				this.Invoke(new MethodInvoker(() =>
				{
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

				}));
			}
			catch (Exception ex)
			{

				// HACK - The best place to stop timer is at OnLeavingStep()
				//        but there is a bug causing last step's OnLeavingStep() not called
				//        so tempararily place the code here.
				if (timer != null)
				{
					timer.Stop();
					timer.Dispose();
					timer = null;
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				lock (cs)
				{
					updateStatus();
				}
			}
			catch
			{
			}
		}

		private static void SetDoubleBuffered(System.Windows.Forms.Control c)
		{
			//Taxes: Remote Desktop Connection and painting
			//http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
			if (System.Windows.Forms.SystemInformation.TerminalServerSession)
				return;

			System.Reflection.PropertyInfo aProp =
				  typeof(System.Windows.Forms.Control).GetProperty(
						"DoubleBuffered",
						System.Reflection.BindingFlags.NonPublic |
						System.Reflection.BindingFlags.Instance);

			aProp.SetValue(c, true, null);
		}

		private void PersonalCloudStatusControl2_Load(object sender, EventArgs e)
		{
			SetDoubleBuffered(listView1);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var dialog = new AddDeviceDialog();
			dialog.ShowDialog(this);
		}
	}
}

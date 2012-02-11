#region

using System;
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.ListBarControl
{
    /// <summary>
    /// Summary description for frmVsNetListBarTest.
    /// </summary>
    public class frmVsNetListBarTest : Form
    {
        private ComboBox cboGroups;
        private IContainer components;
        private PropertyGrid groupPropertyGrid;
        private ImageList ilsIconsLarge;
        private ImageList ilsIconsSmall;
        private Panel pnlProperties;
        private Splitter splitProperties;
        private ToolTip toolTips;
        private TreeView tvwCustom;
        private VSNetListBar vsNetListBar1;

        public frmVsNetListBarTest()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.Run(new frmVsNetListBarTest());
        }


        private void frmVsNetListBarTest_Load(object sender, EventArgs e)
        {
            Random randGen = new Random();

            // Add some items to the TreeView:
            tvwCustom.ImageList = ilsIconsSmall;
            tvwCustom.ShowLines = true;
            tvwCustom.ShowPlusMinus = true;
            TreeNode[] childNodes = new TreeNode[10];
            for (int i = 0; i < 10; i++)
            {
                int iconIndex = randGen.Next(ilsIconsSmall.Images.Count);
                childNodes[i] = new TreeNode(
                    String.Format("TreeItem {0}", i),
                    iconIndex, iconIndex);
            }
            TreeNode nodTop = new TreeNode("Connections", 0, 0, childNodes);
            tvwCustom.Nodes.Add(nodTop);
            nodTop.Expand();

            // Add some items to the ListBar:
            vsNetListBar1.LargeImageList = ilsIconsLarge;
            vsNetListBar1.SmallImageList = ilsIconsSmall;
            vsNetListBar1.ToolTip = toolTips;

            // Add some items to the ListBar:
            for (int i = 0; i < 4; i++)
            {
                int jMax = 2 + randGen.Next(10);
                VSNetListBarItem[] subItems = new VSNetListBarItem[jMax];
                for (int j = 0; j < jMax; j++)
                {
                    subItems[j] = new VSNetListBarItem(
                        String.Format("Test Item {0} in Bar {1}", j + 1, i + 1),
                        j%4,
                        String.Format("Tooltip text for test item {0}", j + 1));
                    if (j == 2)
                    {
                        subItems[j].Enabled = false;
                    }
                }
                vsNetListBar1.Groups.Add(
                    new VSNetListBarGroup(String.Format("Test {0}", i + 1),
                                          subItems));
            }
            // Add a bar containing the Tree control:
            VSNetListBarGroup treeGroup = vsNetListBar1.Groups.Add("Tree");
            treeGroup.ChildControl = tvwCustom;

            // Configure ListBar events:
            vsNetListBar1.ItemClicked += listBar1_ItemClicked;
            vsNetListBar1.ItemDoubleClicked += listBar1_ItemDoubleClicked;
            vsNetListBar1.GroupClicked += listBar1_GroupClicked;
            vsNetListBar1.SelectedGroupChanged += listBar1_SelectedGroupChanged;

            // Group property editor
            cboGroups.SelectedIndexChanged += cboGroups_SelectedIndexChanged;
            pnlProperties.SizeChanged += pnlProperties_SizeChanged;

            showGroupProperties();
        }


        private void listBar1_SelectedGroupChanged(object sender, EventArgs e)
        {
            showGroupProperties();
        }

        private void listBar1_ItemClicked(object sender, ItemClickedEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Right)
            {
                //contextItem = e.Item;
                //itemContextMenu.Show(listBar1, e.Location);
            }
        }

        private void listBar1_GroupClicked(object sender, GroupClickedEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Right)
            {
                //contextGroup = e.Group;
                //groupContextMenu.Show(listBar1, e.Location);
            }
        }

        private void listBar1_ItemDoubleClicked(object sender, ItemClickedEventArgs e)
        {
        }


        private void cboGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            VSNetListBarGroup grp = (VSNetListBarGroup) cboGroups.SelectedItem;
            groupPropertyGrid.SelectedObject = grp;
            groupPropertyGrid.Update();
        }

        private void pnlProperties_SizeChanged(object sender, EventArgs e)
        {
            int height = pnlProperties.Height - groupPropertyGrid.Top - 4;
            if (height > 0)
            {
                groupPropertyGrid.Height = height;
            }
            int width = pnlProperties.Width - cboGroups.Left*2;
            if (width > 0)
            {
                cboGroups.Width = width;
                groupPropertyGrid.Width = width;
            }
        }

        private void showGroupProperties()
        {
            cboGroups.Items.Clear();
            foreach (VSNetListBarGroup group in vsNetListBar1.Groups)
            {
                int newIndex = cboGroups.Items.Add(group);
                if (group.Selected)
                {
                    cboGroups.SelectedIndex = newIndex;
                }
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVsNetListBarTest));
            this.splitProperties = new System.Windows.Forms.Splitter();
            this.ilsIconsSmall = new System.Windows.Forms.ImageList(this.components);
            this.ilsIconsLarge = new System.Windows.Forms.ImageList(this.components);
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.cboGroups = new System.Windows.Forms.ComboBox();
            this.groupPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tvwCustom = new System.Windows.Forms.TreeView();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.vsNetListBar1 = new VSNetListBar();
            this.pnlProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitProperties
            // 
            this.splitProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitProperties.Location = new System.Drawing.Point(261, 0);
            this.splitProperties.MinExtra = 4;
            this.splitProperties.Name = "splitProperties";
            this.splitProperties.Size = new System.Drawing.Size(3, 398);
            this.splitProperties.TabIndex = 8;
            this.splitProperties.TabStop = false;
            // 
            // ilsIconsSmall
            // 
            this.ilsIconsSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilsIconsSmall.ImageStream")));
            this.ilsIconsSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.ilsIconsSmall.Images.SetKeyName(0, "");
            this.ilsIconsSmall.Images.SetKeyName(1, "");
            this.ilsIconsSmall.Images.SetKeyName(2, "");
            this.ilsIconsSmall.Images.SetKeyName(3, "");
            this.ilsIconsSmall.Images.SetKeyName(4, "");
            // 
            // ilsIconsLarge
            // 
            this.ilsIconsLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilsIconsLarge.ImageStream")));
            this.ilsIconsLarge.TransparentColor = System.Drawing.Color.Transparent;
            this.ilsIconsLarge.Images.SetKeyName(0, "");
            this.ilsIconsLarge.Images.SetKeyName(1, "");
            this.ilsIconsLarge.Images.SetKeyName(2, "");
            this.ilsIconsLarge.Images.SetKeyName(3, "");
            this.ilsIconsLarge.Images.SetKeyName(4, "");
            this.ilsIconsLarge.Images.SetKeyName(5, "");
            // 
            // pnlProperties
            // 
            this.pnlProperties.Controls.Add(this.cboGroups);
            this.pnlProperties.Controls.Add(this.groupPropertyGrid);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlProperties.Location = new System.Drawing.Point(264, 0);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(208, 398);
            this.pnlProperties.TabIndex = 7;
            // 
            // cboGroups
            // 
            this.cboGroups.Location = new System.Drawing.Point(4, 5);
            this.cboGroups.Name = "cboGroups";
            this.cboGroups.Size = new System.Drawing.Size(200, 20);
            this.cboGroups.TabIndex = 4;
            this.cboGroups.Text = "comboBox1";
            // 
            // groupPropertyGrid
            // 
            this.groupPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.groupPropertyGrid.Location = new System.Drawing.Point(4, 32);
            this.groupPropertyGrid.Name = "groupPropertyGrid";
            this.groupPropertyGrid.Size = new System.Drawing.Size(200, 420);
            this.groupPropertyGrid.TabIndex = 3;
            // 
            // tvwCustom
            // 
            this.tvwCustom.Location = new System.Drawing.Point(92, 272);
            this.tvwCustom.Name = "tvwCustom";
            this.tvwCustom.Size = new System.Drawing.Size(121, 112);
            this.tvwCustom.TabIndex = 9;
            // 
            // vsNetListBar1
            // 
            this.vsNetListBar1.AllowDragGroups = true;
            this.vsNetListBar1.AllowDragItems = true;
            this.vsNetListBar1.AllowDrop = true;
            this.vsNetListBar1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.vsNetListBar1.DrawStyle = ListBarDrawStyle.ListBarDrawStyleOfficeXP;
            this.vsNetListBar1.LargeImageList = null;
            this.vsNetListBar1.Location = new System.Drawing.Point(35, 32);
            this.vsNetListBar1.Name = "vsNetListBar1";
            this.vsNetListBar1.SelectOnMouseDown = true;
            this.vsNetListBar1.Size = new System.Drawing.Size(164, 325);
            this.vsNetListBar1.SmallImageList = null;
            this.vsNetListBar1.TabIndex = 0;
            this.vsNetListBar1.ToolTip = null;
            // 
            // frmVsNetListBarTest
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(472, 398);
            this.Controls.Add(this.tvwCustom);
            this.Controls.Add(this.vsNetListBar1);
            this.Controls.Add(this.splitProperties);
            this.Controls.Add(this.pnlProperties);
            this.Name = "frmVsNetListBarTest";
            this.Text = "frmVsNetListBarTest";
            this.Load += new System.EventHandler(this.frmVsNetListBarTest_Load);
            this.pnlProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
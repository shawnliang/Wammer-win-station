using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Net;

namespace Manina.Windows.Forms
{
    public partial class DemoForm : Form
    {
        #region Member variables
        private BackgroundWorker bw = new BackgroundWorker();
        #endregion

        #region Renderer and color combobox items
        /// <summary>
        /// Represents an item in the renderer combobox.
        /// </summary>
        private struct RendererComboBoxItem
        {
            public string Name;
            public string FullName;

            public override string ToString()
            {
                return Name;
            }

            public RendererComboBoxItem(Type type)
            {
                Name = type.Name;
                FullName = type.FullName;
            }
        }

        /// <summary>
        /// Represents an item in the custom color combobox.
        /// </summary>
        private struct ColorComboBoxItem
        {
            public string Name;
            public PropertyInfo Field;

            public override string ToString()
            {
                return Name;
            }

            public ColorComboBoxItem(PropertyInfo field)
            {
                Name = field.Name;
                Field = field;
            }
        }
        #endregion

        #region Constructor
        public DemoForm()
        {
            InitializeComponent();

            // Setup the background worker
            Application.Idle += new EventHandler(Application_Idle);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            // Find and add built-in renderers
            Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));
            int i = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType == typeof(ImageListView.ImageListViewRenderer))
                {
                    renderertoolStripComboBox.Items.Add(new RendererComboBoxItem(type));
                    if (type.Name == "DefaultRenderer")
                        renderertoolStripComboBox.SelectedIndex = i;
                    i++;
                }
            }
            // Find and add custom colors
            Type colorType = typeof(ImageListViewColor);
            i = 0;
            foreach (PropertyInfo field in colorType.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                colorToolStripComboBox.Items.Add(new ColorComboBoxItem(field));
                if (field.Name == "Default")
                    colorToolStripComboBox.SelectedIndex = i;
                i++;
            }
            // Dynamically add aligment values
            foreach (object o in Enum.GetValues(typeof(ContentAlignment)))
            {
                ToolStripMenuItem item1 = new ToolStripMenuItem(o.ToString());
                item1.Tag = o;
                item1.Click += new EventHandler(checkboxAlignmentToolStripButton_Click);
                checkboxAlignmentToolStripMenuItem.DropDownItems.Add(item1);
                ToolStripMenuItem item2 = new ToolStripMenuItem(o.ToString());
                item2.Tag = o;
                item2.Click += new EventHandler(iconAlignmentToolStripButton_Click);
                iconAlignmentToolStripMenuItem.DropDownItems.Add(item2);
            }

            imageListView.AllowDuplicateFileNames = true;
            imageListView.SetRenderer(new ImageListViewRenderers.DefaultRenderer());

            TreeNode node = new TreeNode("Loading...", 3, 3);
            node.Tag = null;
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(node);
            while (bw.IsBusy) ;
            bw.RunWorkerAsync(node);
        }

        #endregion

        #region Update UI while idle
        void Application_Idle(object sender, EventArgs e)
        {
            detailsToolStripButton.Checked = (imageListView.View == View.Details);
            thumbnailsToolStripButton.Checked = (imageListView.View == View.Thumbnails);
            galleryToolStripButton.Checked = (imageListView.View == View.Gallery);
            paneToolStripButton.Checked = (imageListView.View == View.Pane);

            integralScrollToolStripMenuItem.Checked = imageListView.IntegralScroll;

            showCheckboxesToolStripMenuItem.Checked = imageListView.ShowCheckBoxes;
            showFileIconsToolStripMenuItem.Checked = imageListView.ShowFileIcons;

            x96ToolStripMenuItem.Checked = imageListView.ThumbnailSize == new Size(96, 96);
            x120ToolStripMenuItem.Checked = imageListView.ThumbnailSize == new Size(120, 120);
            x200ToolStripMenuItem.Checked = imageListView.ThumbnailSize == new Size(200, 200);

            allowCheckBoxClickToolStripMenuItem.Checked = imageListView.AllowCheckBoxClick;
            allowColumnClickToolStripMenuItem.Checked = imageListView.AllowColumnClick;
            allowColumnResizeToolStripMenuItem.Checked = imageListView.AllowColumnResize;
            allowPaneResizeToolStripMenuItem.Checked = imageListView.AllowPaneResize;
            multiSelectToolStripMenuItem.Checked = imageListView.MultiSelect;
            allowDragToolStripMenuItem.Checked = imageListView.AllowDrag;
            allowDropToolStripMenuItem.Checked = imageListView.AllowDrop;
            allowDuplicateFilenamesToolStripMenuItem.Checked = imageListView.AllowDuplicateFileNames;
            continuousCacheModeToolStripMenuItem.Checked = (imageListView.CacheMode == CacheMode.Continuous);

            ContentAlignment ca = imageListView.CheckBoxAlignment;
            foreach (ToolStripMenuItem item in checkboxAlignmentToolStripMenuItem.DropDownItems)
                item.Checked = (ContentAlignment)item.Tag == ca;
            ContentAlignment ia = imageListView.IconAlignment;
            foreach (ToolStripMenuItem item in iconAlignmentToolStripMenuItem.DropDownItems)
                item.Checked = (ContentAlignment)item.Tag == ia;

            toolStripStatusLabel1.Text = string.Format("{0} Items: {1} Selected, {2} Checked",
                imageListView.Items.Count, imageListView.SelectedItems.Count, imageListView.CheckedItems.Count);

            groupAscendingToolStripMenuItem.Checked = imageListView.GroupOrder == SortOrder.Ascending;
            groupDescendingToolStripMenuItem.Checked = imageListView.GroupOrder == SortOrder.Descending;
            sortAscendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Ascending;
            sortDescendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Descending;
        }
        #endregion

        #region Set ImageListView options
        private void checkboxAlignmentToolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ContentAlignment aligment = (ContentAlignment)item.Tag;
            imageListView.CheckBoxAlignment = aligment;
        }

        private void iconAlignmentToolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ContentAlignment aligment = (ContentAlignment)item.Tag;
            imageListView.IconAlignment = aligment;
        }

        private void renderertoolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ImageListView));
            RendererComboBoxItem item = (RendererComboBoxItem)renderertoolStripComboBox.SelectedItem;
            ImageListView.ImageListViewRenderer renderer = (ImageListView.ImageListViewRenderer)assembly.CreateInstance(item.FullName);
            if (renderer == null)
            {
                assembly = Assembly.GetExecutingAssembly();
                renderer = (ImageListView.ImageListViewRenderer)assembly.CreateInstance(item.FullName);
            }
            colorToolStripComboBox.Enabled = renderer.CanApplyColors;
            imageListView.SetRenderer(renderer);
            imageListView.Focus();
        }

        private void colorToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyInfo field = ((ColorComboBoxItem)colorToolStripComboBox.SelectedItem).Field;
            ImageListViewColor color = (ImageListViewColor)field.GetValue(null, null);
            imageListView.Colors = color;
        }

        private void detailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = View.Details;
        }

        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = View.Thumbnails;
        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = View.Gallery;
        }

        private void paneToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.View = View.Pane;
        }

        private void clearThumbsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.ClearThumbnailCache();
        }

        private void x96ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.ThumbnailSize = new Size(96, 96);
        }

        private void x120ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.ThumbnailSize = new Size(120, 120);
        }

        private void x200ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.ThumbnailSize = new Size(200, 200);
        }

        private void showCheckboxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.ShowCheckBoxes = !imageListView.ShowCheckBoxes;
        }

        private void showFileIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.ShowFileIcons = !imageListView.ShowFileIcons;
        }

        private void allowCheckBoxClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowCheckBoxClick = !imageListView.AllowCheckBoxClick;
        }

        private void allowColumnClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowColumnClick = !imageListView.AllowColumnClick;
        }

        private void allowColumnResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowColumnResize = !imageListView.AllowColumnResize;
        }

        private void allowPaneResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowPaneResize = !imageListView.AllowPaneResize;
        }

        private void multiSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.MultiSelect = !imageListView.MultiSelect;
        }

        private void allowDragToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowDrag = !imageListView.AllowDrag;
        }

        private void allowDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowDrop = !imageListView.AllowDrop;
        }

        private void allowDuplicateFilenamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.AllowDuplicateFileNames = !imageListView.AllowDuplicateFileNames;
        }

        private void continuousCacheModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imageListView.CacheMode == CacheMode.Continuous)
                imageListView.CacheMode = CacheMode.OnDemand;
            else
                imageListView.CacheMode = CacheMode.Continuous;
        }

        private void integralScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.IntegralScroll = !imageListView.IntegralScroll;
        }

        private void imageListView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if ((e.Buttons & MouseButtons.Right) != MouseButtons.None)
            {
                // Group menu
                for (int j = groupByToolStripMenuItem.DropDownItems.Count - 1; j >= 0; j--)
                {
                    if (groupByToolStripMenuItem.DropDownItems[j].Tag != null)
                        groupByToolStripMenuItem.DropDownItems.RemoveAt(j);
                }
                int i = 0;

                foreach (ImageListView.ImageListViewColumnHeader col in imageListView.Columns)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(col.Text);
                    item.Checked = (imageListView.GroupColumn == i);
                    item.Tag = i;
                    item.Click += new EventHandler(groupColumnMenuItem_Click);
                    groupByToolStripMenuItem.DropDownItems.Insert(i, item);
                    i++;
                }
                if (i == 0)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem("None");
                    item.Enabled = false;
                    groupByToolStripMenuItem.DropDownItems.Insert(0, item);
                }

                // Sort menu
                for (int j = sortByToolStripMenuItem.DropDownItems.Count - 1; j >= 0; j--)
                {
                    if (sortByToolStripMenuItem.DropDownItems[j].Tag != null)
                        sortByToolStripMenuItem.DropDownItems.RemoveAt(j);
                }

                i = 0;

                foreach (ImageListView.ImageListViewColumnHeader col in imageListView.Columns)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(col.Text);
                    item.Checked = (imageListView.SortColumn == i);
                    item.Tag = i;
                    item.Click += new EventHandler(sortColumnMenuItem_Click);
                    sortByToolStripMenuItem.DropDownItems.Insert(i, item);
                    i++;
                }

                if (i == 0)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem("None");
                    item.Enabled = false;
                    sortByToolStripMenuItem.DropDownItems.Insert(0, item);
                }

                // Show menu
                columnContextMenu.Show(imageListView, e.Location);
            }
        }

        private void groupColumnMenuItem_Click(object sender, EventArgs e)
        {
            int i = (int)((ToolStripMenuItem)sender).Tag;
            imageListView.GroupColumn = i;
        }

        private void sortColumnMenuItem_Click(object sender, EventArgs e)
        {
            int i = (int)((ToolStripMenuItem)sender).Tag;
            imageListView.SortColumn = i;
        }

        private void groupAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.GroupOrder = SortOrder.Ascending;
        }

        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.SortOrder = SortOrder.Ascending;
        }

        private void groupDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.GroupOrder = SortOrder.Descending;
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.SortOrder = SortOrder.Descending;
        }
        #endregion

        #region Set selected image to PropertyGrid
        private void imageListView1_SelectionChanged(object sender, EventArgs e)
        {
            ImageListViewItem sel = null;
            if (imageListView.SelectedItems.Count > 0)
                sel = imageListView.SelectedItems[0];
            propertyGrid1.SelectedObject = sel;
        }
        #endregion

        #region Change Selection/Checkboxes
        private void imageListView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.A)
                    imageListView.SelectAll();
                else if (e.KeyCode == Keys.U)
                    imageListView.ClearSelection();
                else if (e.KeyCode == Keys.I)
                    imageListView.InvertSelection();
            }
            else if (e.Alt)
            {
                if (e.KeyCode == Keys.A)
                    imageListView.CheckAll();
                else if (e.KeyCode == Keys.U)
                    imageListView.UncheckAll();
                else if (e.KeyCode == Keys.I)
                    imageListView.InvertCheckState();
            }
        }
        #endregion

        #region Update folder list asynchronously
        private void PopulateListView(DirectoryInfo path)
        {
            imageListView.Items.Clear();
            imageListView.SuspendLayout();
            foreach (FileInfo p in path.GetFiles("*.*"))
            {
                if (p.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".cur", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".emf", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".wmf", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                    imageListView.Items.Add(p.FullName);
            }
            imageListView.ResumeLayout();
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            KeyValuePair<DirectoryInfo, bool> ktag = (KeyValuePair<DirectoryInfo, bool>)node.Tag;
            if (ktag.Value == true)
                return;
            node.Nodes.Clear();
            node.Nodes.Add("", "Loading...", 3, 3);
            while (bw.IsBusy) ;
            bw.RunWorkerAsync(node);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;
            KeyValuePair<DirectoryInfo, bool> ktag = (KeyValuePair<DirectoryInfo, bool>)e.Node.Tag;
            PopulateListView(ktag.Key);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            KeyValuePair<TreeNode, List<TreeNode>> kv = (KeyValuePair<TreeNode, List<TreeNode>>)e.Result;
            TreeNode rootNode = kv.Key;
            List<TreeNode> nodes = kv.Value;
            if (rootNode.Tag == null)
            {
                treeView1.Nodes.Clear();
                foreach (TreeNode node in nodes)
                    treeView1.Nodes.Add(node);
            }
            else
            {
                KeyValuePair<DirectoryInfo, bool> ktag = (KeyValuePair<DirectoryInfo, bool>)rootNode.Tag;
                rootNode.Tag = new KeyValuePair<DirectoryInfo, bool>(ktag.Key, true);
                rootNode.Nodes.Clear();
                foreach (TreeNode node in nodes)
                    rootNode.Nodes.Add(node);
            }
        }

        private static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TreeNode rootNode = e.Argument as TreeNode;

            List<TreeNode> nodes = GetNodes(rootNode);

            e.Result = new KeyValuePair<TreeNode, List<TreeNode>>(rootNode, nodes);
        }

        private static List<TreeNode> GetNodes(TreeNode rootNode)
        {
            if (rootNode.Tag == null)
            {
                List<TreeNode> volNodes = new List<TreeNode>();
                foreach (DriveInfo info in System.IO.DriveInfo.GetDrives())
                {
                    if (info.IsReady)
                    {
                        DirectoryInfo rootPath = info.RootDirectory;
                        TreeNode volNode = new TreeNode(info.VolumeLabel + " (" + info.Name + ")", 0, 0);
                        volNode.Tag = new KeyValuePair<DirectoryInfo, bool>(rootPath, false);
                        List<TreeNode> nodes = GetNodes(volNode);
                        volNode.Tag = new KeyValuePair<DirectoryInfo, bool>(rootPath, true);
                        volNode.Nodes.Clear();
                        foreach (TreeNode node in nodes)
                            volNode.Nodes.Add(node);

                        volNode.Expand();
                        volNodes.Add(volNode);
                    }
                }

                return volNodes;
            }
            else
            {
                KeyValuePair<DirectoryInfo, bool> kv = (KeyValuePair<DirectoryInfo, bool>)rootNode.Tag;
                bool done = kv.Value;
                if (done)
                    return new List<TreeNode>();

                DirectoryInfo rootPath = kv.Key;
                List<TreeNode> nodes = new List<TreeNode>();

                DirectoryInfo[] dirs = new DirectoryInfo[0];
                try
                {
                    dirs = rootPath.GetDirectories();
                }
                catch
                {
                    return new List<TreeNode>();
                }
                foreach (DirectoryInfo info in dirs)
                {
                    if ((info.Attributes & FileAttributes.System) != FileAttributes.System)
                    {
                        TreeNode aNode = new TreeNode(info.Name, 1, 2);
                        aNode.Tag = new KeyValuePair<DirectoryInfo, bool>(info, false);
                        GetDirectories(aNode);
                        nodes.Add(aNode);
                    }
                }
                return nodes;
            }
        }

        private static void GetDirectories(TreeNode node)
        {
            KeyValuePair<DirectoryInfo, bool> ktag = (KeyValuePair<DirectoryInfo, bool>)node.Tag;
            DirectoryInfo rootPath = ktag.Key;

            DirectoryInfo[] dirs = new DirectoryInfo[0];
            try
            {
                dirs = rootPath.GetDirectories();
            }
            catch
            {
                return;
            }
            foreach (DirectoryInfo info in dirs)
            {
                if ((info.Attributes & FileAttributes.System) != FileAttributes.System)
                {
                    TreeNode aNode = new TreeNode(info.Name, 1, 2);
                    aNode.Tag = new KeyValuePair<DirectoryInfo, bool>(info, false);
                    if (GetDirCount(info) != 0)
                    {
                        aNode.Nodes.Add("Dummy1");
                    }
                    node.Nodes.Add(aNode);
                }
            }
            node.Tag = new KeyValuePair<DirectoryInfo, bool>(ktag.Key, true);
        }

        private static int GetDirCount(DirectoryInfo rootPath)
        {
            DirectoryInfo[] dirs = new DirectoryInfo[0];
            try
            {
                dirs = rootPath.GetDirectories();
            }
            catch
            {
                return 0;
            }

            return dirs.Length;
        }
        #endregion

        private void imageListView_ItemHover(object sender, ItemHoverEventArgs e)
        {
            if (e.PreviousItem != null)
                toolTip.SetToolTip(imageListView, e.PreviousItem.FileName);

            if (e.Item != null)
                toolTip.SetToolTip(imageListView, e.Item.FileName);
        }
    }
}

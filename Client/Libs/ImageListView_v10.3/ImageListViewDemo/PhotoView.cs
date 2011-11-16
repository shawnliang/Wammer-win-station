#region

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace Manina.Windows.Forms
{
    public partial class PhotoView : Form
    {
        #region Renderer and color combobox items

        #region Nested type: ColorComboBoxItem

        /// <summary>
        /// Represents an item in the custom color combobox.
        /// </summary>
        private struct ColorComboBoxItem
        {
            public PropertyInfo Field;
            public string Name;

            public ColorComboBoxItem(PropertyInfo field)
            {
                Name = field.Name;
                Field = field;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        #endregion

        #region Nested type: RendererComboBoxItem

        /// <summary>
        /// Represents an item in the renderer combobox.
        /// </summary>
        private struct RendererComboBoxItem
        {
            public string FullName;
            public string Name;

            public RendererComboBoxItem(Type type)
            {
                Name = type.Name;
                FullName = type.FullName;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        #endregion

        #endregion

        #region Constructor

        public PhotoView()
        {
            InitializeComponent();

            // Setup the background worker
            Application.Idle += Application_Idle;

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
                item1.Click += checkboxAlignmentToolStripButton_Click;
                ToolStripMenuItem item2 = new ToolStripMenuItem(o.ToString());
                item2.Tag = o;
                item2.Click += iconAlignmentToolStripButton_Click;
            }

            imageListView1.AllowDuplicateFileNames = true;
            imageListView1.SetRenderer(new ImageListViewRenderers.DefaultRenderer());
        }

        #endregion

        #region Update UI while idle

        private void Application_Idle(object sender, EventArgs e)
        {
            detailsToolStripButton.Checked = (imageListView1.View == View.Details);
            thumbnailsToolStripButton.Checked = (imageListView1.View == View.Thumbnails);
            galleryToolStripButton.Checked = (imageListView1.View == View.Gallery);
            paneToolStripButton.Checked = (imageListView1.View == View.Pane);

            x96ToolStripMenuItem.Checked = imageListView1.ThumbnailSize == new Size(96, 96);
            x120ToolStripMenuItem.Checked = imageListView1.ThumbnailSize == new Size(120, 120);
            x200ToolStripMenuItem.Checked = imageListView1.ThumbnailSize == new Size(200, 200);

            ContentAlignment ca = imageListView1.CheckBoxAlignment;

            ContentAlignment ia = imageListView1.IconAlignment;


            toolStripStatusLabel1.Text = string.Format("{0} Items: {1} Selected, {2} Checked",
                                                       imageListView1.Items.Count, imageListView1.SelectedItems.Count, imageListView1.CheckedItems.Count);

            groupAscendingToolStripMenuItem.Checked = imageListView1.GroupOrder == SortOrder.Ascending;
            groupDescendingToolStripMenuItem.Checked = imageListView1.GroupOrder == SortOrder.Descending;
            sortAscendingToolStripMenuItem.Checked = imageListView1.SortOrder == SortOrder.Ascending;
            sortDescendingToolStripMenuItem.Checked = imageListView1.SortOrder == SortOrder.Descending;
        }

        #endregion

        #region Set ImageListView options

        private void checkboxAlignmentToolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ContentAlignment aligment = (ContentAlignment)item.Tag;
            imageListView1.CheckBoxAlignment = aligment;
        }

        private void iconAlignmentToolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ContentAlignment aligment = (ContentAlignment)item.Tag;
            imageListView1.IconAlignment = aligment;
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
            imageListView1.SetRenderer(renderer);
            imageListView1.Focus();
        }

        private void colorToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyInfo field = ((ColorComboBoxItem)colorToolStripComboBox.SelectedItem).Field;
            ImageListViewColor color = (ImageListViewColor)field.GetValue(null, null);
            imageListView1.Colors = color;
        }

        private void detailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.Details;
        }

        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.Thumbnails;
        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.Gallery;
        }

        private void paneToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView1.View = View.Pane;
        }

        private void x96ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.ThumbnailSize = new Size(96, 96);
        }

        private void x120ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.ThumbnailSize = new Size(120, 120);
        }

        private void x200ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.ThumbnailSize = new Size(200, 200);
        }

        private void groupAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.GroupOrder = SortOrder.Ascending;
        }

        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.SortOrder = SortOrder.Ascending;
        }

        private void groupDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.GroupOrder = SortOrder.Descending;
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView1.SortOrder = SortOrder.Descending;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.WebCam;

namespace Waveface.PostUI
{
    public partial class Photo : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private long m_avail_month_total_objects;
        private long m_month_total_objects;
        private MyImageListViewRenderer m_imageListViewRenderer;

        public PostForm MyParent { get; set; }

        public Dictionary<string, string> FileNameMapping { get; set; }

        public List<string> Files
        {
            set
            {
                AddFileToImageListView(value.ToArray(), true);
            }
        }

        public Photo()
        {
            InitializeComponent();

            FileNameMapping = new Dictionary<string, string>();

            InitImageListView();
        }

        public void ChangeToEditModeUI(Post post)
        {
            btnSend.Text = "更改"; //@ I18n

            btnAddPhoto.Visible = false;
            btnDeletePhoto.Visible = false;
            labelSummary.Location = new Point(4, labelSummary.Location.Y);
        }

        #region ImageListView

        private void InitImageListView()
        {
            Application.Idle += Application_Idle;

            m_imageListViewRenderer = new MyImageListViewRenderer();

            imageListView.SetRenderer(m_imageListViewRenderer);
            imageListView.BackColor = Color.FromArgb(243, 242, 238);
            imageListView.Colors.BackColor = Color.FromArgb(243, 242, 238);
            imageListView.Colors.PaneBackColor = Color.FromArgb(243, 242, 238);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            removeToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);

            rotateCCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);
            rotateCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);

            sortAscendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Ascending;
            sortDescendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Descending;
        }

        private void AddFileToImageListView(string[] images, bool forceOrigin)
        {
            foreach (string _pic in images)
            {
                ImageListViewItem _item = new ImageListViewItem(_pic);

                if (MyParent.EditMode && !forceOrigin)
                    _item.Tag = EditModePhotoType.NewAdd;
                else
                    _item.Tag = EditModePhotoType.Origin;

                imageListView.Items.Add(_item);
            }
        }

        private void addToolStripButton_Click(object sender, EventArgs e)
        {
            AddPhoto();
        }

        private void removeToolStripButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedPhoto();
        }

        private void removeAllToolStripButton_Click(object sender, EventArgs e)
        {
            RemoveAllAndReturnToParent();
        }

        private void imageListView_ItemCollectionChanged(object sender, ItemCollectionChangedEventArgs e)
        {
            labelSummary.Text = string.Format(I18n.L.T("Photo.Summary"), imageListView.Items.Count);
        }

        private void toolStripButtonCamera_Click(object sender, EventArgs e)
        {
            WebCamForm _camera = new WebCamForm();

            DialogResult _dialogResult = _camera.ShowDialog();

            if (_dialogResult == DialogResult.Yes)
            {
                imageListView.Items.Add(_camera.CapturedImagePath);
            }
        }

        #region Image Rotate

        private void rotateCCWToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Rotating will overwrite original images. Are you sure you want to continue?",
                                "Waveface", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                foreach (ImageListViewItem _item in imageListView.SelectedItems)
                {
                    _item.BeginEdit();

                    using (Image _img = Image.FromFile(_item.FileName))
                    {
                        _img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        _img.Save(_item.FileName);
                    }

                    _item.Update();
                    _item.EndEdit();
                }
            }
        }

        private void rotateCWToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Rotating will overwrite original images. Are you sure you want to continue?",
                                "Waveface", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                foreach (ImageListViewItem _item in imageListView.SelectedItems)
                {
                    _item.BeginEdit();

                    using (Image _img = Image.FromFile(_item.FileName))
                    {
                        _img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        _img.Save(_item.FileName);
                    }

                    _item.Update();
                    _item.EndEdit();
                }
            }
        }

        #endregion

        #endregion

        #region Send, BatchPost

        private void btnSend_Click(object sender, EventArgs e)
        {
            btnBatchPost_Click(sender, e);
        }

        private void SendPureText()
        {
            if (MyParent.pureTextBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("TextEmpty"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (DoRealPost(""))
                {
                    MyParent.SetDialogResult_Yes_AndClose();
                }
            }
        }

        private bool DoRealPost(string files)
        {
            string _type = (files != "") ? "image" : "text";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(StringUtility.RichTextBox_ReplaceNewline(MyParent.pureTextBox.Text), files, "", _type);

                if (_np == null)
                {
                    MessageBox.Show(I18n.L.T("PostForm.PostError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                MessageBox.Show(I18n.L.T("PostForm.PostSuccess"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);


                return false;
            }
        }

        private void btnBatchPost_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (imageListView.Items.Count == 0)
            {
                SendPureText();
                return;
            }

            long _storagesUsage = CheckStoragesUsage();

            if (_storagesUsage == long.MinValue)
            {
                MessageBox.Show(I18n.L.T("SystemError"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (_storagesUsage < 0)
            {
                MessageBox.Show(string.Format(I18n.L.T("PhotoStorageQuotaExceeded"), m_month_total_objects), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            NewPostItem _newPostItem = new NewPostItem();
            _newPostItem.PostType = PostType.Photo;
            _newPostItem.Text = StringUtility.RichTextBox_ReplaceNewline(MyParent.pureTextBox.Text);
            _newPostItem.ResizeRatio = toolStripComboBoxResize.Text;
            _newPostItem.OrgPostTime = DateTime.Now;

            foreach (ImageListViewItem _vi in imageListView.Items)
            {
                _newPostItem.Files.Add(_vi.FileName);
            }

            MyParent.NewPostItem = _newPostItem;
            MyParent.SetDialogResult_OK_AndClose();
        }

        private long CheckStoragesUsage()
        {
            try
            {
                MR_storages_usage _storagesUsage = Main.Current.RT.REST.Storages_Usage();

                if (_storagesUsage != null)
                {
                    int _queuedUnsendFiles = Main.Current.NewPostManager.GetQueuedUnsendFilesCount();
                    m_avail_month_total_objects = _storagesUsage.storages.waveface.available.avail_month_total_objects;
                    m_month_total_objects = _storagesUsage.storages.waveface.quota.month_total_objects;

                    //Hack
                    if (m_month_total_objects == -1)
                    {
                        return long.MaxValue;
                    }

                    return m_avail_month_total_objects - _queuedUnsendFiles - imageListView.Items.Count;
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "CheckStoragesUsage");
            }

            return long.MinValue;
        }

        #endregion

        public void AddPhoto()
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AddFileToImageListView(openFileDialog.FileNames, false);
            }
            else
            {
                if (imageListView.Items.Count == 0)
                {
                    MyParent.toPureText_Mode();
                    return;
                }
            }
        }

        private void RemoveAllAndReturnToParent()
        {
            if (imageListView.Items.Count == 0)
            {
                MyParent.toPureText_Mode();
                return;
            }

            DialogResult _dr = MessageBox.Show(I18n.L.T("RemoveAllFiles"), "Waveface", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (_dr == DialogResult.Yes)
            {
                imageListView.Items.Clear();

                MyParent.toPureText_Mode();
            }
        }

        private void RemoveSelectedPhoto()
        {
            DialogResult _dr = MessageBox.Show(I18n.L.T("RemoveSelectedFiles"), "Waveface", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (_dr == DialogResult.Yes)
            {
                imageListView.SuspendLayout();

                foreach (var _item in imageListView.SelectedItems)
                    imageListView.Items.Remove(_item);

                imageListView.ResumeLayout(true);
            }
        }

        private void btnAddPhoto_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            AddPhoto();
        }

        private void btnDeletePhoto_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (imageListView.SelectedItems.Count == 0)
            {
                RemoveAllAndReturnToParent();
            }
            else
            {
                RemoveSelectedPhoto();
            }
        }

        #region Sort

        private void columnContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CreateSortMenuItems();

            imageListView.SetRenderer(new ImageListViewRenderers.TilesRenderer());
        }

        private void CreateSortMenuItems()
        {
            for (int j = sortByToolStripMenuItem.DropDownItems.Count - 1; j >= 0; j--)
            {
                if (sortByToolStripMenuItem.DropDownItems[j].Tag != null)
                    sortByToolStripMenuItem.DropDownItems.RemoveAt(j);
            }

            int i = 0;

            foreach (ImageListView.ImageListViewColumnHeader col in imageListView.Columns)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(col.Text);
                item.Checked = (imageListView.SortColumn == i);
                item.Tag = i;
                item.Click += sortColumnMenuItem_Click;
                sortByToolStripMenuItem.DropDownItems.Insert(i, item);
                i++;
            }

            if (i == 0)
            {
                ToolStripMenuItem item = new ToolStripMenuItem("None");
                item.Enabled = false;
                sortByToolStripMenuItem.DropDownItems.Insert(0, item);
            }
        }

        private void sortColumnMenuItem_Click(object sender, EventArgs e)
        {
            int i = (int)((ToolStripMenuItem)sender).Tag;
            imageListView.SortColumn = i;
        }

        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.SortOrder = SortOrder.Ascending;
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageListView.SortOrder = SortOrder.Descending;
        }

        private void columnContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            imageListView.SetRenderer(new MyImageListViewRenderer());
        }

        #endregion

        private void Photo_Resize(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(243, 242, 238); //Hack
        }

        private void imageListView_ItemHover(object sender, ItemHoverEventArgs e)
        {
            if (e.Item == null)
            {
                Cursor = Cursors.Default;

                labelSummary.Text = string.Format(I18n.L.T("Photo.Summary"), imageListView.Items.Count);
            }
            else
            {
                Cursor = Cursors.SizeAll;

                string _filePath = e.Item.FileName;
                string _fileName = new FileInfo(e.Item.FileName).Name;

                if (FileNameMapping.ContainsKey(_fileName))
                    _filePath = FileNameMapping[_fileName];

                labelSummary.Text = _filePath;
            }
        }
    }
}

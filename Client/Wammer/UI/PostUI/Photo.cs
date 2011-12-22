
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Manina.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.WebCam;

namespace Waveface.PostUI
{
    public partial class Photo : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private ProgressDialog m_progressDialog;
        private long m_avail_month_total_objects;
        private long m_month_total_objects;

        public PostForm MyParent { get; set; }

        public List<string> Files
        {
            set
            {
                imageListView.Items.AddRange(value.ToArray());
            }
        }

        public Photo()
        {
            InitializeComponent();

            InitImageListView();
        }

        #region ImageListView

        private void InitImageListView()
        {
            Application.Idle += Application_Idle;

            imageListView.SetRenderer(new ImageListViewRenderers.NoirRenderer());
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            removeToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);

            rotateCCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);
            rotateCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);

            sortAscendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Ascending;
            sortDescendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Descending;
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
            setBatchButtonVisible();
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

        private void setBatchButtonVisible()
        {
            if (imageListView.Items.Count > 0)
            {
                //@ btnBatchPost.Visible = true;
                return;
            }

            //@ btnBatchPost.Visible = false;
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
            return;

            if (!Main.Current.CheckNetworkStatus())
                return;

            //有圖片要上傳
            if (imageListView.Items.Count > 0)
            {
                m_progressDialog = new ProgressDialog("Upload Photo ..."
                         , delegate(object[] Params)
                         {
                             string _resizeRatio = (string)Params[0];

                             string _ids = "[";

                             int i = 0;

                             foreach (ImageListViewItem _item in imageListView.Items)
                             {
                                 if (m_progressDialog.WasCancelled)
                                     return "*ERROR*";

                                 try
                                 {
                                     string _resizedImage = ImageUtility.ResizeImage(_item.FileName, _item.Text, _resizeRatio, 100);

                                     MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_item.Text, _resizedImage, "", true);

                                     if (_uf == null)
                                     {
                                         return "*ERROR*";
                                     }

                                     _ids += "\"" + _uf.object_id + "\"" + ",";
                                 }
                                 catch
                                 {
                                     return "*ERROR*";
                                 }

                                 i++;

                                 m_progressDialog.RaiseUpdateProgress(i * 100 / imageListView.Items.Count);
                             }

                             _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
                             _ids += "]";

                             return (_ids);
                         },
                         toolStripComboBoxResize.Text); // This value will be passed to the method

                // Then all you need to do is 
                m_progressDialog.ShowDialog();

                if (!m_progressDialog.WasCancelled && ((string)m_progressDialog.Result) != "*ERROR*")
                {
                    try
                    {
                        if (DoRealPost((string)m_progressDialog.Result))
                        {
                            MyParent.SetDialogResult_Yes_AndClose();
                        }

                        return;
                    }
                    catch (Exception _e)
                    {
                        MessageBox.Show(_e.Message);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Action canceled");
                }

                return;
            }

            SendPureText();
        }

        private void SendPureText()
        {
            if (MyParent.richTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("Text cannot be empty!");
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
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(MyParent.richTextBox.Text, files, "", _type);

                if (_np == null)
                {
                    MessageBox.Show("Post Error!");
                    return false;
                }

                MessageBox.Show("Post success!");
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
                MessageBox.Show("抱歉, 系統發生未預期錯誤, 請稍後再試.", "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (_storagesUsage < 0)
            {
                MessageBox.Show(string.Format("抱歉! 您可以上傳的照片數已經超過系統允許值({0}).", m_month_total_objects), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            NewPostItem _newPostItem = new NewPostItem();
            _newPostItem.PostType = PostType.Photo;
            _newPostItem.Text = MyParent.richTextBox.Text;
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
                    int _queuedUnsendFiles = NewPostManager.Current.GetQueuedUnsendFilesCount();
                    m_avail_month_total_objects = _storagesUsage.storages.waveface.available.avail_month_total_objects;
                    m_month_total_objects = _storagesUsage.storages.waveface.quota.month_total_objects;
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
                imageListView.Items.AddRange(openFileDialog.FileNames);
            }
        }

        private void RemoveAllAndReturnToParent()
        {
            imageListView.Items.Clear();

            MyParent.toPureText_Mode();
        }

        private void RemoveSelectedPhoto()
        {
            imageListView.SuspendLayout();

            foreach (var _item in imageListView.SelectedItems)
                imageListView.Items.Remove(_item);

            imageListView.ResumeLayout(true);
        }

        private void btnAddPhoto_Click(object sender, EventArgs e)
        {
            AddPhoto();
        }

        private void btnDeletePhoto_Click(object sender, EventArgs e)
        {
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
            imageListView.SetRenderer(new ImageListViewRenderers.NoirRenderer());
        }

        #endregion


    }
}

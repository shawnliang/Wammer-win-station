
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.DetailUI;
using Waveface.WebCam;

namespace Waveface.PostUI
{
    public partial class Photo : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private long m_avail_month_total_objects;
        private long m_month_total_objects;
        private MyImageListViewRenderer m_imageListViewRenderer;
        private List<string> m_editModeOriginPhotoFiles;
        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;
        private string m_coverAttachGUID;
        private ImageListViewItem m_selectedItem;

        public PostForm MyParent { get; set; }

        public Dictionary<string, string> FileNameMapping { get; set; }

        public Photo()
        {
            InitializeComponent();

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper(false);
            FileNameMapping = new Dictionary<string, string>();
            m_editModeOriginPhotoFiles = new List<string>();

            InitImageListView();

            UIHack();

            HackDPI();
        }

        public void ChangeToEditModeUI(Post post)
        {
            btnSend.Text = I18n.L.T("Update");
        }

        #region Hack

        private void UIHack()
        {
            if (btnDeletePhoto.Width > Properties.Resources.FB_blue_btn.Width)
            {
                btnDeletePhoto.Image = Properties.Resources.FB_blue_btn_2;
                btnDeletePhoto.ImageDisable = Properties.Resources.FB_blue_btn_hl_2;
                btnDeletePhoto.ImageHover = Properties.Resources.FB_blue_btn_hl_2;
            }
        }

        private void HackDPI()
        {
            float _r = getDPIRatio();

            if (_r != 0)
            {
                Font _old = btnAddPhoto.Font;
                Font _new = new Font(_old.Name, _old.Size * _r, _old.Style);

                btnAddPhoto.Font = _new;
                btnDeletePhoto.Font = _new;
            }
        }

        private float getDPIRatio()
        {
            using (Graphics _g = CreateGraphics())
            {
                if (_g.DpiX == 120)
                    return 0.85f;
            }

            return 1;
        }

        private void Photo_Resize(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(226, 226, 226); //Hack

            ChengeThumbnailSize();
        }

        #endregion

        #region ImageListView

        private void InitImageListView()
        {
            Application.Idle += Application_Idle;

            m_imageListViewRenderer = new MyImageListViewRenderer();

            imageListView.SetRenderer(m_imageListViewRenderer);

            imageListView.BackColor = Color.FromArgb(226, 226, 226);
            imageListView.Colors.BackColor = Color.FromArgb(226, 226, 226);
            imageListView.Colors.PaneBackColor = Color.FromArgb(226, 226, 226);
        }

        public void AddNewPostPhotoFiles(List<string> files)
        {
            foreach (string _pic in files)
            {
                ImageListViewItem _item = new ImageListViewItem(_pic);

                EditModeImageListViewItemTag _tag = new EditModeImageListViewItemTag();
                _tag.caGUID = Guid.NewGuid().ToString();
                _tag.AddPhotoType = EditModePhotoType.NewPostOrigin;

                _item.Tag = _tag;

                imageListView.Items.Add(_item);
            }
        }

        public void AddEditModePhotoFiles(List<string> files, Post post)
        {
            m_editModeOriginPhotoFiles = files;

            int i = 0;

            foreach (string _pic in m_editModeOriginPhotoFiles)
            {
                ImageListViewItem _item = new ImageListViewItem(_pic);

                EditModeImageListViewItemTag _tag = new EditModeImageListViewItemTag();
                _tag.caGUID = Guid.NewGuid().ToString();
                _tag.AddPhotoType = EditModePhotoType.EditModeOrigin;
                _tag.ObjectID = post.attachment_id_array[i];

                if (_tag.ObjectID == post.cover_attach)
                {
                    _tag.IsCoverImage_UI = true;

                    m_coverAttachGUID = _tag.caGUID;
                }

                _item.Tag = _tag;

                imageListView.Items.Add(_item);

                i++;
            }
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

        private void imageListView_DropFiles(object sender, DropFileEventArgs e)
        {
            try
            {
                List<string> _pics = new List<string>();

                string[] _dropFils = e.FileNames;

                foreach (string _file in _dropFils)
                {
                    if (Directory.Exists(_file))
                    {
                        DirectoryInfo _d = new DirectoryInfo(_file);

                        FileInfo[] _fileInfos = _d.GetFiles();

                        foreach (FileInfo _f in _fileInfos)
                        {
                            FileAttributes _attributes = File.GetAttributes(_f.FullName);

                            // 過濾隱藏檔
                            if ((_attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                continue;

                            string _mime = FileUtility.GetMimeType(_f).ToLower();

                            if (_mime.IndexOf("image") >= 0)
                                _pics.Add(_f.FullName);
                        }
                    }
                    else
                    {
                        string _mime = FileUtility.GetMimeType(new FileInfo(_file)).ToLower();

                        if (_mime.IndexOf("image") >= 0)
                            _pics.Add(_file);
                    }
                }

                if (_pics.Count > 0)
                {
                    AddPhotos(_pics.ToArray(), e.Index);
                }
            }
            catch
            {
            }

            e.Cancel = true;
        }

        private void imageListView_ItemCollectionChanged(object sender, ItemCollectionChangedEventArgs e)
        {
            labelSummary.Text = string.Format(I18n.L.T("Photo.Summary"), imageListView.Items.Count);

            if (e.Action != CollectionChangeAction.Refresh)
                SetCoverImageUI();

            ChengeThumbnailSize();
        }

        private void SetCoverImageUI()
        {
            bool _setCoverImage_UI = false;

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                EditModeImageListViewItemTag _tag = _item.Tag as EditModeImageListViewItemTag;
                _tag.IsCoverImage_UI = false;

                if (_tag.caGUID == m_coverAttachGUID)
                {
                    _tag.IsCoverImage_UI = true;

                    _setCoverImage_UI = true;
                }
            }

            // ToDo: Rule?
            if (!_setCoverImage_UI)
            {
                if (imageListView.Items.Count > 0)
                {
                    (imageListView.Items[0].Tag as EditModeImageListViewItemTag).IsCoverImage_UI = true;
                }
            }
        }

        private void imageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_selectedItem = e.Item;
        }

        #region Drag&Drop

        private void imageListView_DragDrop(object sender, DragEventArgs e)
        {
            MyParent.IsDirty = true;

            ImageListView.HitInfo _hitInfo;
            imageListView.HitTest(imageListView.PointToClient(new Point(e.X, e.Y)), out _hitInfo);

            List<string> _pics = m_dragDropClipboardHelper.Drag_Drop_HtmlImage(e);

            if (_pics != null)
            {
                if ((_hitInfo.ItemIndex < 0) || (_hitInfo.ItemIndex >= imageListView.Items.Count))
                    AddPhotos(_pics.ToArray(), 0);
                else
                    AddPhotos(_pics.ToArray(), _hitInfo.ItemIndex);
            }

            FlashWindow.Stop(MyParent);
        }

        private void imageListView_DragEnter(object sender, DragEventArgs e)
        {
            FlashWindow.Start(MyParent);

            m_dragDropClipboardHelper.Drag_Enter_HtmlImage(e, false);

            DragHitTest(e);
        }

        private void imageListView_DragLeave(object sender, EventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Leave_HtmlImage();

            FlashWindow.Stop(MyParent);
        }

        private void imageListView_DragOver(object sender, DragEventArgs e)
        {
            m_dragDropClipboardHelper.Drag_Over_HtmlImage(e, false);

            DragHitTest(e);
        }

        private void DragHitTest(DragEventArgs e)
        {
            ImageListView.HitInfo _hitInfo;
            imageListView.HitTest(imageListView.PointToClient(new Point(e.X, e.Y)), out _hitInfo);

            if (_hitInfo.InItemArea)
                e.Effect = DragDropEffects.Move;

            if (_hitInfo.ItemHit)
                e.Effect = DragDropEffects.Copy;
        }

        #endregion

        #endregion

        #region Send, BatchPost

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!Main.Current.CheckNetworkStatus())
                return;

            if (MyParent.EditMode)
            {
                EditModePost();
            }
            else
            {
                BatchPost();
            }
        }

        private void EditModePost()
        {
            List<string> _objectIDs = new List<string>();
            List<string> _objectIDs_Edit = new List<string>();
            List<string> _files = new List<string>();
            int _newAdd = 0;

            foreach (ImageListViewItem _vi in imageListView.Items)
            {
                EditModeImageListViewItemTag _tag = (EditModeImageListViewItemTag)_vi.Tag;

                if (_tag.AddPhotoType == EditModePhotoType.EditModeNewAdd)
                {
                    _files.Add(_vi.FileName);

                    string _guid = Guid.NewGuid().ToString();

                    _objectIDs.Add(_guid);
                    _objectIDs_Edit.Add(_guid);

                    _newAdd++;
                }
                else
                {
                    _objectIDs.Add(_tag.ObjectID);
                }
            }

            if (_newAdd == 0)
            {
                Dictionary<string, string> _params = new Dictionary<string, string>();

                if (!MyParent.pureTextBox.Text.Trim().Equals(MyParent.OldText))
                {
                    _params.Add("content", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)));
                }

                if (EditModePhotosChanged())
                {
                    _params.Add("attachment_id_array", getNewAttachmentIdArray());
                    _params.Add("type", "image");
                }

                int _coverAttachIndex = GetCoverAttachIndex();

                if (_coverAttachIndex != -1)
                {
                    EditModeImageListViewItemTag _tag = imageListView.Items[_coverAttachIndex].Tag as EditModeImageListViewItemTag;
                    _params.Add("cover_attach", _tag.ObjectID);
                }

                if (_params.Count != 0)
                {
                    Main.Current.PostUpdate(MyParent.Post, _params);
                }

                MyParent.SetDialogResult_Yes_AndClose();
            }
            else
            {
                if (Environment.GetCommandLineArgs().Length == 1)
                {
                    // check quota only in cloud mode because station will handle over quota
                    long _storagesUsage = CheckStoragesUsage(_newAdd);

                    if (_storagesUsage == long.MinValue)
                    {
                        MessageBox.Show(I18n.L.T("SystemError"), "Stream", MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);

                        return;
                    }

                    if (_storagesUsage < 0)
                    {
                        MessageBox.Show(string.Format(I18n.L.T("PhotoStorageQuotaExceeded"), m_month_total_objects),
                                        "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }
                }

                string _text = string.Empty;

                if (!MyParent.pureTextBox.Text.Trim().Equals(MyParent.OldText))
                {
                    _text = StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000));
                }

                BatchPostItem _bpItem = new BatchPostItem();
                _bpItem.PostType = PostType.Photo;
                _bpItem.Text = StringUtility.RichTextBox_ReplaceNewline(_text);
                _bpItem.LongSideResizeOrRatio = toolStripComboBoxResize.Text;
                _bpItem.OrgPostTime = DateTime.Now;
                _bpItem.Files = _files;

                _bpItem.EditMode = true;
                _bpItem.ObjectIDs = _objectIDs;
                _bpItem.ObjectIDs_Edit = _objectIDs_Edit;
                _bpItem.Post = MyParent.Post;
                _bpItem.PostID = MyParent.Post.post_id;

                _bpItem.CoverAttachIndex = GetCoverAttachIndex();

                MyParent.BatchPostItem = _bpItem;
                MyParent.SetDialogResult_OK_AndClose();
            }
        }

        private bool EditModePhotosChanged()
        {
            if (imageListView.Items.Count == m_editModeOriginPhotoFiles.Count)
            {
                for (int i = 0; i < imageListView.Items.Count; i++)
                {
                    if (imageListView.Items[i].FileName != m_editModeOriginPhotoFiles[i])
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private string getNewAttachmentIdArray()
        {
            string _ids = string.Empty;

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                _ids += "\"" + ((EditModeImageListViewItemTag)_item.Tag).ObjectID + "\"" + ",";
            }

            _ids = _ids.Substring(0, _ids.Length - 1);
            _ids = string.Format("[{0}]", _ids);

            return _ids;
        }

        private void SendPureText()
        {
            if (MyParent.pureTextBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(I18n.L.T("TextEmpty"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MR_posts_new _np = Main.Current.RT.REST.Posts_New(StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)), files, "", _type, "");

                if (_np == null)
                {
                    MessageBox.Show(I18n.L.T("PostForm.PostError"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                MessageBox.Show(I18n.L.T("PostForm.PostSuccess"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);

                return false;
            }
        }

        private void BatchPost()
        {
            if (imageListView.Items.Count == 0)
            {
                SendPureText();
                return;
            }

            if (Environment.GetCommandLineArgs().Length == 1)
            {
                // check quota only in cloud mode because station will handle over quota
                long _storagesUsage = CheckStoragesUsage(imageListView.Items.Count);

                if (_storagesUsage == long.MinValue)
                {
                    MessageBox.Show(I18n.L.T("SystemError"), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }

                if (_storagesUsage < 0)
                {
                    MessageBox.Show(string.Format(I18n.L.T("PhotoStorageQuotaExceeded"), m_month_total_objects), "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
            }

            List<string> _objectIDs = new List<string>();

            BatchPostItem _bpItem = new BatchPostItem();
            _bpItem.PostType = PostType.Photo;
            _bpItem.Text = StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000));
            _bpItem.LongSideResizeOrRatio = toolStripComboBoxResize.Text;
            _bpItem.OrgPostTime = DateTime.Now;

            foreach (ImageListViewItem _vi in imageListView.Items)
            {
                _bpItem.Files.Add(_vi.FileName);

                _objectIDs.Add(Guid.NewGuid().ToString());
            }

            _bpItem.ObjectIDs = _objectIDs;

            _bpItem.CoverAttachIndex = GetCoverAttachIndex();

            MyParent.BatchPostItem = _bpItem;
            MyParent.SetDialogResult_OK_AndClose();
        }

        private int GetCoverAttachIndex()
        {
            int k = 0;

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                EditModeImageListViewItemTag _tag = _item.Tag as EditModeImageListViewItemTag;

                if (_tag.IsCoverImage_UI)
                {
                    return k;
                }

                k++;
            }

            return 0;
        }

        private long CheckStoragesUsage(int went)
        {
            try
            {
                MR_storages_usage _storagesUsage = Main.Current.RT.REST.Storages_Usage();

                if (_storagesUsage != null)
                {
                    int _queuedUnsendFiles = Main.Current.BatchPostManager.GetQueuedUnsendFilesCount(); //@ + EditMode ...
                    m_avail_month_total_objects = _storagesUsage.storages.waveface.available.avail_month_total_objects;
                    m_month_total_objects = _storagesUsage.storages.waveface.quota.month_total_objects;

                    if (m_month_total_objects == -1)
                    {
                        return long.MaxValue;
                    }

                    return m_avail_month_total_objects - _queuedUnsendFiles - went;
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "CheckStoragesUsage");
            }

            return long.MinValue;
        }

        public void ShowMessage(int went)
        {
            if (Environment.GetCommandLineArgs().Length == 1)
            {
                // check quota only in cloud mode because station will handle over quota
                long _storagesUsage = CheckStoragesUsage(went);

                if (_storagesUsage == long.MinValue)
                {
                    MessageBox.Show(I18n.L.T("SystemError"), "Stream", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);

                    return;
                }

                if (_storagesUsage < 0)
                {
                    MessageBox.Show(string.Format(I18n.L.T("PhotoStorageQuotaExceeded"), m_month_total_objects),
                                    "Stream", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
            }
        }

        #endregion

        #region Misc

        private void ChengeThumbnailSize()
        {
            Photo_DV.ChengeThumbnailSize(imageListView, 112, 24);
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

        private void Application_Idle(object sender, EventArgs e)
        {
            rotateCCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);
            rotateCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);

            sortAscendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Ascending;
            sortDescendingToolStripMenuItem.Checked = imageListView.SortOrder == SortOrder.Descending;
        }

        public void AddPhoto()
        {
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AddPhotos(openFileDialog.FileNames, -1);
            }
            else
            {
                if (imageListView.Items.Count == 0)
                {
                    if (!MyParent.EditMode)
                        MyParent.toPureText_Mode();

                    return;
                }
            }
        }

        public void AddPhotos(string[] files, int index)
        {
            MyParent.IsDirty = true;

            if (index >= 0)
                Array.Reverse(files);

            foreach (string _pic in files)
            {
                ImageListViewItem _item = new ImageListViewItem(_pic);

                EditModeImageListViewItemTag _tag = new EditModeImageListViewItemTag();
                _tag.caGUID = Guid.NewGuid().ToString();

                if (MyParent.EditMode)
                    _tag.AddPhotoType = EditModePhotoType.EditModeNewAdd;
                else
                    _tag.AddPhotoType = EditModePhotoType.NewPostNewAdd;

                _item.Tag = _tag;

                if (index < 0)
                    imageListView.Items.Add(_item);
                else
                    imageListView.Items.Insert(index, _item);
            }
        }

        private void RemoveAllAndReturnToParent()
        {
            if (imageListView.Items.Count == 0)
            {
                MyParent.IsDirty = true;

                MyParent.toPureText_Mode();

                return;
            }

            DialogResult _dr = MessageBox.Show(I18n.L.T("RemoveAllFiles"), "Stream", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (_dr == DialogResult.Yes)
            {
                MyParent.IsDirty = true;

                imageListView.Items.Clear();

                MyParent.toPureText_Mode();
            }
        }

        private void RemoveSelectedPhoto()
        {
            DialogResult _dr = MessageBox.Show(I18n.L.T("RemoveSelectedFiles"), "Stream", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (_dr == DialogResult.Yes)
            {
                MyParent.IsDirty = true;

                imageListView.SuspendLayout();

                foreach (var _item in imageListView.SelectedItems)
                    imageListView.Items.Remove(_item);

                imageListView.ResumeLayout(true);

                if (imageListView.Items.Count == 0)
                {
                    MyParent.toPureText_Mode();
                    return;
                }
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
                if (!MyParent.EditMode)
                    RemoveAllAndReturnToParent();
            }
            else
            {
                RemoveSelectedPhoto();
            }
        }

        #endregion

        #region ContextMenu

        private void columnContextMenu_Opening(object sender, CancelEventArgs e)
        {
            // CreateSortMenuItems();
            // imageListView.SetRenderer(new ImageListViewRenderers.TilesRenderer());
        }

        private void miSetCoverImage_Click(object sender, EventArgs e)
        {
            if (m_selectedItem != null)
            {
                EditModeImageListViewItemTag _tag = m_selectedItem.Tag as EditModeImageListViewItemTag;
                m_coverAttachGUID = _tag.caGUID;

                SetCoverImageUI();
            }
        }

        /*
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
                ToolStripMenuItem _item = new ToolStripMenuItem(col.Text);
                _item.Checked = (imageListView.SortColumn == i);
                _item.Tag = i;
                _item.Click += sortColumnMenuItem_Click;
                sortByToolStripMenuItem.DropDownItems.Insert(i, _item);
                i++;
            }

            if (i == 0)
            {
                ToolStripMenuItem _item = new ToolStripMenuItem("None");
                _item.Enabled = false;
                sortByToolStripMenuItem.DropDownItems.Insert(0, _item);
            }
        }

        private void sortColumnMenuItem_Click(object sender, EventArgs e)
        {
            int i = (int)((ToolStripMenuItem)sender).Tag;
            imageListView.SortColumn = i;
        }
         
        */

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
            // imageListView.SetRenderer(new MyImageListViewRenderer());
        }

        #endregion

        #region Image Rotate

        private void rotateCCWToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Rotating will overwrite original images. Are you sure you want to continue?",
                                "Stream", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
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
                                "Stream", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
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
    }
}
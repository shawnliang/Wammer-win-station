
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using Manina.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.DetailUI;
using System.Windows.Media.Imaging;
using Waveface.Properties;
using System.Diagnostics;
using Waveface.Upload;

namespace Waveface.PostUI
{
    public partial class Photo : UserControl
    {
		#region Var
		private static Logger s_logger = LogManager.GetCurrentClassLogger();
		private MyImageListViewRenderer m_imageListViewRenderer;
		private List<string> _editModeOriginPhotoFiles;
		private DragDrop_Clipboard_Helper _dragDropClipboardHelper;
		private string m_coverAttachGUID;
		private ImageListViewItem m_selectedItem;
		private Dictionary<string, string> _preUploadPhotosQueue;
		private Dictionary<string, string> _uploadedPhotos;
		public Dictionary<string, string> _fileNameMapping; 
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ drag drop clipboard helper.
		/// </summary>
		/// <value>The m_ drag drop clipboard helper.</value>
		private DragDrop_Clipboard_Helper m_DragDropClipboardHelper
		{
			get
			{
				return _dragDropClipboardHelper ?? (_dragDropClipboardHelper = new DragDrop_Clipboard_Helper(false));
			}
		}

		/// <summary>
		/// Gets the m_ edit mode origin photo files.
		/// </summary>
		/// <value>The m_ edit mode origin photo files.</value>
		private List<string> m_EditModeOriginPhotoFiles
		{
			get
			{
				return _editModeOriginPhotoFiles ?? (_editModeOriginPhotoFiles = new List<string>());
			}
		}

		/// <summary>
		/// Gets the m_ pre upload photos queue.
		/// </summary>
		/// <value>The m_ pre upload photos queue.</value>
		private Dictionary<string, string> m_PreUploadPhotosQueue
		{
			get
			{
				return _preUploadPhotosQueue ?? (_preUploadPhotosQueue = new Dictionary<string, string>());
			}
		}

		/// <summary>
		/// Gets the m_uploaded photos.
		/// </summary>
		/// <value>The m_uploaded photos.</value>
		private Dictionary<string, string> m_UploadedPhotos
		{
			get
			{
				return _uploadedPhotos ?? (_uploadedPhotos = new Dictionary<string, string>());
			}
		}
		#endregion


		#region Public Property
		public string PostId { get; set; }

		public PostForm MyParent { get; set; }

		/// <summary>
		/// Gets the file name mapping.
		/// </summary>
		/// <value>The file name mapping.</value>
		public Dictionary<string, string> FileNameMapping
		{
			get
			{
				return _fileNameMapping ?? (_fileNameMapping = new Dictionary<string, string>());
			}
			set
			{
				_fileNameMapping = value;
			}
		}
		#endregion

		#region Constructor
		public Photo()
		{
			DebugInfo.ShowMethod();

			InitializeComponent();

			InitImageListView();

			UIHack();
			HackDPI();
		} 
		#endregion


        public void ChangeToEditModeUI(Post post)
        {
			DebugInfo.ShowMethod();

			btnSend.Text = Resources.UPDATE;
        }

        #region Hack

        private void UIHack()
        {
			DebugInfo.ShowMethod();

            if (btnDeletePhoto.Width > Properties.Resources.FB_blue_btn.Width)
            {
                btnDeletePhoto.Image = Properties.Resources.FB_blue_btn_2;
                btnDeletePhoto.ImageDisable = Properties.Resources.FB_blue_btn_hl_2;
                btnDeletePhoto.ImageHover = Properties.Resources.FB_blue_btn_hl_2;
            }
        }

        private void HackDPI()
        {
			DebugInfo.ShowMethod();

            float _r = getDPIRatio();

            if (_r != 0)
            {
                Font _old = btnAddPhoto.Font;
                Font _new = new Font(_old.Name, _old.Size * _r, _old.Style);

                btnAddPhoto.Font = _new;
                btnDeletePhoto.Font = _new;
            }
        }

		/// <summary>
		/// Gets the DPI ratio.
		/// </summary>
		/// <returns></returns>
        private float getDPIRatio()
        {
			DebugInfo.ShowMethod();

            using (Graphics _g = CreateGraphics())
            {
                if (_g.DpiX == 120)
                    return 0.85f;
            }

            return 1;
        }

        private void Photo_Resize(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            BackColor = Color.FromArgb(226, 226, 226); //Hack

            ChengeThumbnailSize();
        }

        #endregion

        #region ImageListView

        private void InitImageListView()
        {
			DebugInfo.ShowMethod();


            m_imageListViewRenderer = new MyImageListViewRenderer();

            imageListView.SetRenderer(m_imageListViewRenderer);

            imageListView.BackColor = Color.FromArgb(226, 226, 226);
            imageListView.Colors.BackColor = Color.FromArgb(226, 226, 226);
            imageListView.Colors.PaneBackColor = Color.FromArgb(226, 226, 226);
        }

        public void AddNewPostPhotoFiles(List<string> files)
        {
			DebugInfo.ShowMethod();

			var linq = from file in files
					   where isValidImageFile(file)
					   select new ImageListViewItem(file)
					   {
						   Tag = new EditModeImageListViewItemTag()
						   {
							   AddPhotoType = EditModePhotoType.NewPostOrigin,
							   ObjectID = Guid.NewGuid().ToString()
						   }
					   };

			var items = linq.ToArray();
			imageListView.ItemCollectionChanged -= imageListView_ItemCollectionChanged;
			imageListView.Items.AddRange(items);
			imageListView.ItemCollectionChanged += imageListView_ItemCollectionChanged;
			imageListView_ItemCollectionChanged(null, null);

			MethodInvoker mi = new MethodInvoker(() =>
			{
				var uploadItems = from item in items
								  select new UploadItem
								  {
									  file_path = item.FileName,
									  object_id = (item.Tag as EditModeImageListViewItemTag).ObjectID,
									  post_id = PostId
								  };
				Main.Current.Uploader.Add(uploadItems);
			});

			mi.BeginInvoke((result) =>
			{
				mi.EndInvoke(result);
			}, null);
        }

        public void AddEditModePhotoFiles(List<string> files, Post post)
        {
			DebugInfo.ShowMethod();

			m_EditModeOriginPhotoFiles.Clear();
            m_EditModeOriginPhotoFiles.AddRange(files);

            int i = 0;

            foreach (string _pic in m_EditModeOriginPhotoFiles)
            {
                ImageListViewItem _item = new ImageListViewItem(_pic);

                EditModeImageListViewItemTag _tag = new EditModeImageListViewItemTag();
                _tag.AddPhotoType = EditModePhotoType.EditModeOrigin;
                _tag.ObjectID = post.attachment_id_array[i];

                if (_tag.ObjectID == post.cover_attach)
                {
                    _tag.IsCoverImage_UI = true;

                    m_coverAttachGUID = _tag.ObjectID;
                }

                _item.Tag = _tag;

                imageListView.Items.Add(_item);

                i++;
            }
        }

        private void imageListView_ItemHover(object sender, ItemHoverEventArgs e)
        {
			DebugInfo.ShowMethod();

            if (e.Item == null)
            {
                Cursor = Cursors.Default;

                labelSummary.Text = string.Format(Resources.POST_PHOTO_SUMMARY_PATTERN, imageListView.Items.Count);
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
			DebugInfo.ShowMethod();

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
			DebugInfo.ShowMethod();

			labelSummary.Text = string.Format(Resources.POST_PHOTO_SUMMARY_PATTERN, imageListView.Items.Count);

			if (e != null && e.Action != CollectionChangeAction.Refresh)
				SetCoverImageUI();

			ChengeThumbnailSize();
		}

        private void SetCoverImageUI()
        {
			DebugInfo.ShowMethod();

            bool _setCoverImage_UI = false;

            foreach (ImageListViewItem _item in imageListView.Items)
            {
                EditModeImageListViewItemTag _tag = _item.Tag as EditModeImageListViewItemTag;
                _tag.IsCoverImage_UI = false;

                if (_tag.ObjectID == m_coverAttachGUID)
                {
                    _tag.IsCoverImage_UI = true;

                    _setCoverImage_UI = true;
                }
            }

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
			DebugInfo.ShowMethod();

            m_selectedItem = e.Item;
        }

        #region Drag&Drop

        private void imageListView_DragDrop(object sender, DragEventArgs e)
        {
			DebugInfo.ShowMethod();

            MyParent.IsDirty = true;

            ImageListView.HitInfo _hitInfo;
            imageListView.HitTest(imageListView.PointToClient(new Point(e.X, e.Y)), out _hitInfo);

            List<string> _pics = m_DragDropClipboardHelper.Drag_Drop_HtmlImage(e);

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
			DebugInfo.ShowMethod();

            FlashWindow.Start(MyParent);

            m_DragDropClipboardHelper.Drag_Enter_HtmlImage(e, false);

            DragHitTest(e);
        }

        private void imageListView_DragLeave(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            m_DragDropClipboardHelper.Drag_Leave_HtmlImage();

            FlashWindow.Stop(MyParent);
        }

        private void imageListView_DragOver(object sender, DragEventArgs e)
        {
			DebugInfo.ShowMethod();

            m_DragDropClipboardHelper.Drag_Over_HtmlImage(e, false);

            DragHitTest(e);
        }

        private void DragHitTest(DragEventArgs e)
        {
			DebugInfo.ShowMethod();

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
			DebugInfo.ShowMethod();

            if (!Main.Current.CheckNetworkStatus())
                return;

            //AbortThread();

            Delay(2);

            if (MyParent.EditMode)
            {
                EditModePost();
            }
            else
            {
                BatchPost();
            }
        }

        private static void Delay(int seconds)
        {
			DebugInfo.ShowMethod();

            DateTime _dt = DateTime.Now;

            Main.Current.Cursor = Cursors.WaitCursor;

            while (_dt.AddSeconds(seconds) >= DateTime.Now)
                Application.DoEvents();

            Main.Current.Cursor = Cursors.Default;
        }

        private void ReturnToText_Mode()
        {
			DebugInfo.ShowMethod();

            lock (m_PreUploadPhotosQueue)
            {
                m_PreUploadPhotosQueue.Clear();
            }

            lock (m_UploadedPhotos)
            {
                m_UploadedPhotos.Clear();
            }

            MyParent.toPureText_Mode();
        }

        private void BatchPost()
        {
			DebugInfo.ShowMethod();

            if (imageListView.Items.Count == 0)
            {
                SendPureText();
                return;
            }

            var _objectIDs = imageListView.Items.Select(x => (x.Tag as EditModeImageListViewItemTag).ObjectID).ToArray();

            var post = Main.Current.RT.REST.Posts_New(
                PostId,
                StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)),
                "[" + string.Join(",", _objectIDs.Select(id=>"\"" + id + "\"").ToArray()) + "]",
                "",
                "image",
                _objectIDs[GetCoverAttachIndex()]);


            var sources = new Dictionary<string, string>();
            foreach (var item in imageListView.Items)
            {
                sources.Add((item.Tag as EditModeImageListViewItemTag).ObjectID,
                    item.FileName);
            }
            

            Main.Current.ReloadAllData(
                new PhotoPostInfo
                {
                    post_id = post.post.post_id,
                    sources = sources
                });

            MyParent.SetDialogResult_OK_AndClose();
        }

        private void EditModePost()
        {
			DebugInfo.ShowMethod();

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

        private bool EditModePhotosChanged()
        {
			DebugInfo.ShowMethod();

            if (imageListView.Items.Count == m_EditModeOriginPhotoFiles.Count)
            {
                for (int i = 0; i < imageListView.Items.Count; i++)
                {
                    if (imageListView.Items[i].FileName != m_EditModeOriginPhotoFiles[i])
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
			DebugInfo.ShowMethod();

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
			DebugInfo.ShowMethod();

            if (MyParent.pureTextBox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show(Resources.EMPTY_CONTENT, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			DebugInfo.ShowMethod();

            string _type = (files != "") ? "image" : "text";

            try
            {
                MR_posts_new _np = Main.Current.RT.REST.Posts_New("", StringUtility.RichTextBox_ReplaceNewline(StringUtility.LimitByteLength(MyParent.pureTextBox.Text, 80000)), files, "", _type, "");

                if (_np == null)
                {
					MessageBox.Show(Resources.POST_ERROR, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                MessageBox.Show(Resources.POST_SUCCESS, "Stream", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);

                return false;
            }
        }

        private int GetCoverAttachIndex()
        {
			DebugInfo.ShowMethod();

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

        #endregion

        #region Misc

        private void ChengeThumbnailSize()
        {
			DebugInfo.ShowMethod();

            Photo_DV.ChengeThumbnailSize(imageListView, 112, 24);
        }


        public void AddPhoto()
        {
			DebugInfo.ShowMethod();

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
                        ReturnToText_Mode();

                    return;
                }
            }
        }

        public void AddPhotos(string[] files, int index)
        {
			DebugInfo.ShowMethod();

            MyParent.IsDirty = true;

            if (index >= 0)
                Array.Reverse(files);

            foreach (string _pic in files)
            {
                if (!isValidImageFile(_pic))
                {
					Toast.MakeText(imageListView, Resources.INVAILD_IMAGE + _pic, Toast.LENGTH_SHORT).Show();
                    continue;
                }
                
                ImageListViewItem _item = new ImageListViewItem(_pic);

                EditModeImageListViewItemTag _tag = new EditModeImageListViewItemTag();
                _tag.ObjectID = Guid.NewGuid().ToString();

                if (MyParent.EditMode)
                    _tag.AddPhotoType = EditModePhotoType.EditModeNewAdd;
                else
                    _tag.AddPhotoType = EditModePhotoType.NewPostNewAdd;

                _item.Tag = _tag;

                if (index < 0)
                    imageListView.Items.Add(_item);
                else
                    imageListView.Items.Insert(index, _item);

                Main.Current.Uploader.Add(_pic, _tag.ObjectID, PostId);
            }
        }

        private static bool isValidImageFile(string file_path)
        {
			DebugInfo.ShowMethod();

            try
            {
                using (var m = File.OpenRead(file_path))
                {
                    var decoder = BitmapDecoder.Create(m, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    var frame = decoder.Frames[0];
                    return frame.PixelWidth > 0 && frame.PixelHeight > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private void RemoveAllAndReturnToParent()
        {
			DebugInfo.ShowMethod();

            if (imageListView.Items.Count == 0)
            {
                MyParent.IsDirty = true;

                ReturnToText_Mode();

                return;
            }

			DialogResult _dr = MessageBox.Show(Resources.REMOVE_ALL_FILES, "Stream", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (_dr == DialogResult.Yes)
            {
                MyParent.IsDirty = true;

                imageListView.Items.Clear();

                ReturnToText_Mode();
            }
        }

        private void RemoveSelectedPhoto()
        {
			DebugInfo.ShowMethod();

			DialogResult _dr = MessageBox.Show(Resources.REMOVE_SELECTED_FILES, "Stream", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (_dr == DialogResult.Yes)
            {
                MyParent.IsDirty = true;

                imageListView.SuspendLayout();

                foreach (var _item in imageListView.SelectedItems)
                {
                    lock (m_PreUploadPhotosQueue)
                    {
                        try
                        {
                            if (m_PreUploadPhotosQueue.Keys.Contains(_item.FileName))
                                m_PreUploadPhotosQueue.Remove(_item.FileName);
                        }
                        catch
                        {
                        }
                    }

                    imageListView.Items.Remove(_item);
                }

                imageListView.ResumeLayout(true);

                if (imageListView.Items.Count == 0)
                {
                    ReturnToText_Mode();
                    return;
                }
            }
        }

        private void btnAddPhoto_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            if (!Main.Current.CheckNetworkStatus())
                return;

            AddPhoto();
        }

        private void btnDeletePhoto_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

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

        private void miSetCoverImage_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            if (m_selectedItem != null)
            {
                EditModeImageListViewItemTag _tag = m_selectedItem.Tag as EditModeImageListViewItemTag;
                m_coverAttachGUID = _tag.ObjectID;

                SetCoverImageUI();
            }
        }


        private void sortAscendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            imageListView.SortOrder = SortOrder.Ascending;
        }

        private void sortDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            imageListView.SortOrder = SortOrder.Descending;
        }

        #endregion

    }
}
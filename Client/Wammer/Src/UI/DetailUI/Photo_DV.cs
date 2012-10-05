#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Libs.StationDB;
using MongoDB.Driver.Builders;
using System.Diagnostics;
using Waveface.Properties;
using MongoDB.Bson;

#endregion

namespace Waveface.DetailUI
{
	public class Photo_DV : UserControl, IDetailView
	{
		private Color BG_COLOR = Color.FromArgb(255, 255, 255);

		private static MongoDB.Driver.MongoCollection<BsonDocument> attachmentCollection = StationDB.GetCollection("attachments");

		#region Fields

		public static string PostID;
		public static int UnloadPhotosCount;

		private Panel panelMain;
		private AutoScrollPanel panelRight;
		private WebBrowser webBrowserTop;
		private ImageListView imageListView;
		private Post m_post;
		private Panel panelPictureInfo;
		private Label labelPictureInfo;
		private IContainer components;
		private Timer timer;

		private List<string> _filePathMediums;

		private ContextMenuStrip contextMenuStripTop;
		private ToolStripMenuItem miCopyTop;
		private WebBrowserContextMenuHandler m_topBrowserContextMenuHandler;

		private List<string> m_clickableURLs;
		private ContextMenuStrip contextMenuStripImageList;
		private ToolStripMenuItem miSetCoverImage;

		private bool m_canEdit;
		private ToolStripMenuItem miOpen;
		private ImageButton btnSaveAllPhotos;
		private ImageListViewItem m_selectedItem;
		private ToolStripMenuItem miDuplicate;
		private ToolStripMenuItem propertyToolStripMenuItem;
		private ToolStripMenuItem miLocateOnDisk;

		private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;
		#endregion


		#region Private Property

		/// <summary>
		/// Gets the m_file path mediums.
		/// </summary>
		/// <value>The m_file path mediums.</value>
		private List<string> m_filePathMediums
		{
			get
			{
				return _filePathMediums ?? (_filePathMediums = new List<string>());
			}
		}
		#endregion

		#region Properties

		public Post Post
		{
			get { return m_post; }
			set
			{
				//if (m_post == value)
				//    return;

				m_post = value;

				if (m_post != null)
				{
					m_canEdit = false;

					PostID = m_post.post_id;
					UnloadPhotosCount = m_post.attachment_id_array.Count;

					timer.Enabled = false;
					RefreshUI();
				}
			}
		}

		public User User { get; set; }

		public DetailView MyParent { get; set; }

		public ImageListView ImageListView
		{
			get { return imageListView; }
			set { imageListView = value; }
		}

		#endregion

		public Photo_DV()
		{
			DebugInfo.ShowMethod();

			InitializeComponent();

			MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer
			{
				ItemBorderless = true,
				ShowHovered = false
			};

			imageListView.SetRenderer(_imageListViewRenderer);

			imageListView.BackColor = BG_COLOR;
			imageListView.Colors.BackColor = BG_COLOR;
			imageListView.Colors.DisabledBackColor = BG_COLOR;
			imageListView.ThumbnailSize = new Size(144, 144);
			imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

			m_clickableURLs = new List<string>();

			m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper(false);

			Visible = false;

			webBrowserTop.NewWindow += webBrowser_NewWindow;
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Photo_DV));
			this.panelMain = new System.Windows.Forms.Panel();
			this.panelRight = new Waveface.Component.AutoScrollPanel();
			this.btnSaveAllPhotos = new Waveface.Component.ImageButton();
			this.imageListView = new Manina.Windows.Forms.ImageListView();
			this.contextMenuStripImageList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miSetCoverImage = new System.Windows.Forms.ToolStripMenuItem();
			this.miOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.miDuplicate = new System.Windows.Forms.ToolStripMenuItem();
			this.propertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miLocateOnDisk = new System.Windows.Forms.ToolStripMenuItem();
			this.panelPictureInfo = new System.Windows.Forms.Panel();
			this.labelPictureInfo = new System.Windows.Forms.Label();
			this.webBrowserTop = new System.Windows.Forms.WebBrowser();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStripTop = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miCopyTop = new System.Windows.Forms.ToolStripMenuItem();
			this.panelMain.SuspendLayout();
			this.panelRight.SuspendLayout();
			this.contextMenuStripImageList.SuspendLayout();
			this.panelPictureInfo.SuspendLayout();
			this.contextMenuStripTop.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.BackColor = System.Drawing.Color.White;
			this.panelMain.Controls.Add(this.panelRight);
			resources.ApplyResources(this.panelMain, "panelMain");
			this.panelMain.Name = "panelMain";
			// 
			// panelRight
			// 
			resources.ApplyResources(this.panelRight, "panelRight");
			this.panelRight.BackColor = System.Drawing.Color.White;
			this.panelRight.Controls.Add(this.btnSaveAllPhotos);
			this.panelRight.Controls.Add(this.imageListView);
			this.panelRight.Controls.Add(this.panelPictureInfo);
			this.panelRight.Controls.Add(this.webBrowserTop);
			this.panelRight.Name = "panelRight";
			// 
			// btnSaveAllPhotos
			// 
			this.btnSaveAllPhotos.CenterAlignImage = false;
			this.btnSaveAllPhotos.Image = global::Waveface.Properties.Resources.FB_saveall;
			this.btnSaveAllPhotos.ImageDisable = global::Waveface.Properties.Resources.FB_saveall_hl;
			this.btnSaveAllPhotos.ImageFront = null;
			this.btnSaveAllPhotos.ImageHover = global::Waveface.Properties.Resources.FB_saveall_hl;
			resources.ApplyResources(this.btnSaveAllPhotos, "btnSaveAllPhotos");
			this.btnSaveAllPhotos.Name = "btnSaveAllPhotos";
			this.btnSaveAllPhotos.TextShadow = true;
			// 
			// imageListView
			// 
			this.imageListView.AllowDrop = true;
			this.imageListView.AllowDuplicateFileNames = true;
			this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imageListView.CacheLimit = "0";
			this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.imageListView.ContextMenuStrip = this.contextMenuStripImageList;
			this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
			resources.ApplyResources(this.imageListView, "imageListView");
			this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
			this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.imageListView.Name = "imageListView";
			this.imageListView.ThumbnailSize = new System.Drawing.Size(128, 128);
			this.imageListView.UseEmbeddedThumbnails = Manina.Windows.Forms.UseEmbeddedThumbnails.Never;
			this.imageListView.DropFiles += new Manina.Windows.Forms.DropFilesEventHandler(this.imageListView_DropFiles);
			this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
			this.imageListView.ItemDoubleClick += new Manina.Windows.Forms.ItemDoubleClickEventHandler(this.imageListView_ItemDoubleClick);
			this.imageListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.imageListView_DragDrop);
			this.imageListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.imageListView_DragEnter);
			this.imageListView.DragOver += new System.Windows.Forms.DragEventHandler(this.imageListView_DragOver);
			this.imageListView.DragLeave += new System.EventHandler(this.imageListView_DragLeave);
			this.imageListView.Resize += new System.EventHandler(this.imageListView_Resize);
			// 
			// contextMenuStripImageList
			// 
			this.contextMenuStripImageList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSetCoverImage,
            this.miOpen,
            this.miLocateOnDisk,
            this.miDuplicate,
            this.propertyToolStripMenuItem});
			this.contextMenuStripImageList.Name = "contextMenuStripImageList";
			resources.ApplyResources(this.contextMenuStripImageList, "contextMenuStripImageList");
			this.contextMenuStripImageList.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripImageList_Opening);
			// 
			// miSetCoverImage
			// 
			this.miSetCoverImage.Image = global::Waveface.Properties.Resources.FB_cover;
			this.miSetCoverImage.Name = "miSetCoverImage";
			resources.ApplyResources(this.miSetCoverImage, "miSetCoverImage");
			this.miSetCoverImage.Click += new System.EventHandler(this.miSetCoverImage_Click);
			// 
			// miOpen
			// 
			this.miOpen.Image = global::Waveface.Properties.Resources.FB_openin;
			this.miOpen.Name = "miOpen";
			resources.ApplyResources(this.miOpen, "miOpen");
			this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
			// 
			// miDuplicate
			// 
			this.miDuplicate.Name = "miDuplicate";
			resources.ApplyResources(this.miDuplicate, "miDuplicate");
			// 
			// propertyToolStripMenuItem
			// 
			this.propertyToolStripMenuItem.Name = "propertyToolStripMenuItem";
			resources.ApplyResources(this.propertyToolStripMenuItem, "propertyToolStripMenuItem");
			this.propertyToolStripMenuItem.Click += new System.EventHandler(this.propertyToolStripMenuItem_Click);
			// 
			// miLocateOnDisk
			// 
			this.miLocateOnDisk.Name = "miLocateOnDisk";
			resources.ApplyResources(this.miLocateOnDisk, "miLocateOnDisk");
			this.miLocateOnDisk.Click += new System.EventHandler(this.miLocateOnDisk_Click);
			// 
			// panelPictureInfo
			// 
			this.panelPictureInfo.BackColor = System.Drawing.Color.White;
			this.panelPictureInfo.Controls.Add(this.labelPictureInfo);
			resources.ApplyResources(this.panelPictureInfo, "panelPictureInfo");
			this.panelPictureInfo.Name = "panelPictureInfo";
			// 
			// labelPictureInfo
			// 
			resources.ApplyResources(this.labelPictureInfo, "labelPictureInfo");
			this.labelPictureInfo.ForeColor = System.Drawing.SystemColors.InfoText;
			this.labelPictureInfo.Name = "labelPictureInfo";
			// 
			// webBrowserTop
			// 
			this.webBrowserTop.AllowWebBrowserDrop = false;
			resources.ApplyResources(this.webBrowserTop, "webBrowserTop");
			this.webBrowserTop.Name = "webBrowserTop";
			this.webBrowserTop.ScrollBarsEnabled = false;
			this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
			// 
			// timer
			// 
			this.timer.Interval = 3000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// contextMenuStripTop
			// 
			this.contextMenuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyTop});
			this.contextMenuStripTop.Name = "contextMenuStripTop";
			resources.ApplyResources(this.contextMenuStripTop, "contextMenuStripTop");
			// 
			// miCopyTop
			// 
			this.miCopyTop.Name = "miCopyTop";
			resources.ApplyResources(this.miCopyTop, "miCopyTop");
			// 
			// Photo_DV
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.panelMain);
			resources.ApplyResources(this, "$this");
			this.Name = "Photo_DV";
			this.Resize += new System.EventHandler(this.DetailView_Resize);
			this.panelMain.ResumeLayout(false);
			this.panelRight.ResumeLayout(false);
			this.contextMenuStripImageList.ResumeLayout(false);
			this.panelPictureInfo.ResumeLayout(false);
			this.contextMenuStripTop.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		#region Event Handlers

		protected override void OnPaint(PaintEventArgs e)
		{
			LinearGradientBrush _brush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(106, 112, 128),
																 Color.FromArgb(138, 146, 166),
																 LinearGradientMode.ForwardDiagonal);

			e.Graphics.FillRectangle(_brush, ClientRectangle);

			base.OnPaint(e);
		}

		#endregion

		public bool CanEdit()
		{
			DebugInfo.ShowMethod();

			return m_canEdit;
		}

		private void RefreshUI()
		{
			DebugInfo.ShowMethod();

			PhotoDownloader.PreloadPictures(m_post, true);

			Set_MainContent_Part();

			Set_Pictures();
		}

		private void ReLayout()
		{
			DebugInfo.ShowMethod();

			if (Post.attachment_id_array.Count > 0)
			{
				imageListView.Height = imageListView.VScrollBar.Maximum + 16;
			}
			else
			{
				imageListView.Height = 0;
			}
		}

		private void Set_MainContent_Part()
		{
			DebugInfo.ShowMethod();

			StringBuilder sb = new StringBuilder(256);

			sb.Append("<body bgcolor=\"rgb(255, 255, 255)\"><font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif'><p>");

			string content = HttpUtility.HtmlEncode(Post.content);
			content = content.Replace(Environment.NewLine, "<BR>");
			content = content.Replace("\n", "<BR>");
			content = content.Replace("\r", "<BR>");

			sb.Append(content);

			sb.Append("</p></font>");

			sb.Append(MyParent.GenCommentHTML(Post, false));

			sb.Append("</body>");

			var html = HtmlUtility.MakeLink(sb.ToString(), m_clickableURLs);

			webBrowserTop.DocumentText = HtmlUtility.TrimScript(html);
		}

		private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			DebugInfo.ShowMethod();

			Visible = true;

			int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
			webBrowserTop.Height = _h;

			m_topBrowserContextMenuHandler = new WebBrowserContextMenuHandler(webBrowserTop, miCopyTop);
			contextMenuStripTop.Opening += contextMenuStripTop_Opening;
			miCopyTop.Click += m_topBrowserContextMenuHandler.CopyCtxMenuClickHandler;
			webBrowserTop.Document.ContextMenuShowing += webBrowserTop_ContextMenuShowing;
		}


		private void webBrowser_NewWindow(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			string _browser = WebBrowserUtility.GetSystemDefaultBrowser();

			if (string.IsNullOrEmpty(_browser))
			{
				e.Cancel = false;
				return;
			}

			var _newUrl = webBrowserTop.Document.ActiveElement.GetAttribute("href");
			Process.Start(_browser, _newUrl);
		}


		private void Set_Pictures()
		{
			DebugInfo.ShowMethod();

			imageListView.Items.Clear();

			m_filePathMediums.Clear();

			if (Post.attachment_id_array == null || Post.attachment_id_array.Count == 0)
				return;

			foreach (var object_id in Post.attachment_id_array)
			{
				var mediumPath = RT_REST.attachments_getThumbnailFilePath(object_id, "medium");
				_filePathMediums.Add(mediumPath);
			}

			timer.Interval = ((Post.attachment_id_array.Count / 100) + 4) * 1000;

			InitImageListViewLoadingImage();

			FillImageListView(true);
		}

		private void InitImageListViewLoadingImage()
		{
			DebugInfo.ShowMethod();

			imageListView.SuspendLayout();

			string _cover_attach = m_post.getCoverImageId();

			bool _setCoverImage = false;

			try
			{
				for (int i = 0; i < Post.attachment_id_array.Count; i++)
				{
					imageListView.Items.Add(Main.Current.LoadingImagePath);

					DetailViewImageListViewItemTag _tag = new DetailViewImageListViewItemTag();
					_tag.Index = i.ToString();

					if (_cover_attach == Post.attachment_id_array[i])
					{
						if (Post.attachment_id_array.Count > 1)
						{
							_tag.IsCoverImage = true;
						}

						_setCoverImage = true;
					}
					else
					{
						_tag.IsCoverImage = false;
					}

					imageListView.Items[i].Tag = _tag;
				}
			}
			catch
			{
			}

			if (!_setCoverImage)
			{
				if (Post.attachment_id_array.Count > 1)
				{
					((DetailViewImageListViewItemTag)imageListView.Items[0].Tag).IsCoverImage = true;
				}
			}

			imageListView.ResumeLayout();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			FillImageListView(false);
		}

		private bool FillImageListView(bool firstTime)
		{
			DebugInfo.ShowMethod();

			UnloadPhotosCount = Post.attachment_id_array.Count - GetMediumPhotoReadyCount();

			if (IsAllMediumPhotoReady())
			{
				Main.Current.RefreshTimelineUI();

				timer.Enabled = !ShowImageListView(firstTime);
				return !timer.Enabled;
			}

			timer.Enabled = !ShowImageListView(firstTime);
			return !timer.Enabled;
		}

		private Boolean IsAllMediumPhotoReady()
		{
			DebugInfo.ShowMethod();

			return GetMediumPhotoReadyCount() == Post.attachment_id_array.Count;
		}

		private int GetMediumPhotoReadyCount()
		{
			DebugInfo.ShowMethod();

			int _count = 0;

			for (int i = 0; i < Post.attachment_id_array.Count; i++)
			{
				if (File.Exists(m_filePathMediums[i]))
				{
					_count++;
				}
			}
			return _count;
		}

		private bool ShowImageListView(bool firstTime)
		{
			DebugInfo.ShowMethod();

			int k = 0;


			imageListView.SuspendLayout();

			try
			{
				for (int i = 0; i < Post.attachment_id_array.Count; i++)
				{
					var object_id = Post.attachment_id_array[i];

					if (imageListView.Items[i].FileName == m_filePathMediums[i])
					{
						k++;
						continue;
					}

					if (File.Exists(m_filePathMediums[i]))
					{
						if (Path.GetFileName(imageListView.Items[i].FileName) == "LoadingImage.jpg")
							imageListView.Items[i].FileName = m_filePathMediums[i];
						k++;
						continue;
					}

					var origFilePath = Main.Current.RT.REST.attachments_getOriginFilePath(object_id);
					if (File.Exists(origFilePath))
					{
						if (Path.GetFileName(imageListView.Items[i].FileName) == "LoadingImage.jpg")
							imageListView.Items[i].FileName = origFilePath;
						k++;
						continue;
					}

					if (Post.Sources.ContainsKey(object_id))
					{
						var sourcePath = Post.Sources[object_id];
						if (File.Exists(sourcePath))
						{
							if (Path.GetFileName(imageListView.Items[i].FileName) == "LoadingImage.jpg")
								imageListView.Items[i].FileName = sourcePath;
							k++;
							continue;
						}
					}
				}
			}
			catch
			{
			}

			imageListView.ResumeLayout();

			bool _flag = (k == Post.attachment_id_array.Count);

			panelPictureInfo.Visible = !_flag;

			if (IsAllMediumPhotoReady())
			{
				m_canEdit = true;
				Post.Sources = new Dictionary<string, string>();
			}

			ReLayout();

			return m_canEdit;
		}

		private void ShowPhotoView(int index)
		{
			DebugInfo.ShowMethod();

			PhotoDownloader.PreloadPictures(m_post, true);

			List<string> _files = new List<string>();

			foreach (var _file in imageListView.Items)
			{
				_files.Add(_file.FileName);
			}

			using (PhotoView _photoView = new PhotoView(m_post, m_filePathMediums, index))
			{
				_photoView.ShowDialog();
			}
		}

		private void DetailView_Resize(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ReLayout();
		}

		#region ContextMenu

		void contextMenuStripTop_Opening(object sender, CancelEventArgs e)
		{
			m_topBrowserContextMenuHandler.UpdateButtons();
		}

		void webBrowserTop_ContextMenuShowing(object sender, HtmlElementEventArgs e)
		{
			contextMenuStripTop.Show(webBrowserTop.PointToScreen(e.MousePosition));
			e.ReturnValue = false;
		}

		#endregion

		private void imageListView_Resize(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			try
			{
				if ((webBrowserTop.Document != null) && (webBrowserTop.Document.Body != null))
					webBrowserTop.Height = webBrowserTop.Document.Body.ScrollRectangle.Height;
			}
			catch
			{
			}

			ChengeThumbnailSize(imageListView, 104, 14);
		}

		public static void ChengeThumbnailSize(ImageListView imgListView, int smallest, int padding)
		{
			DebugInfo.ShowMethod();

			int W;

			if (imgListView.Items.Count < 3)
			{
				W = (imgListView.Width - (padding * 2)) / 2;
			}
			else if (imgListView.Items.Count < 7)
			{
				W = (imgListView.Width - (padding * 3)) / 3;
			}
			else if (imgListView.Items.Count < 17)
			{
				W = (imgListView.Width - (padding * 4)) / 4;
			}
			else if (imgListView.Items.Count < 51)
			{
				W = (imgListView.Width - (padding * 5)) / 5;
			}
			else if (imgListView.Items.Count < 101)
			{
				W = (imgListView.Width - (padding * 6)) / 6;
			}
			else if ((imgListView.Items.Count >= 101) && (imgListView.Items.Count <= 150))
			{
				W = (imgListView.Width - (padding * 7)) / 7;
			}
			else if ((imgListView.Items.Count >= 151) && (imgListView.Items.Count <= 200))
			{
				W = (imgListView.Width - (padding * 8)) / 8;
			}
			else if ((imgListView.Items.Count > 200) && (imgListView.Items.Count <= 300))
			{
				W = (imgListView.Width - (padding * 9)) / 9;
			}
			else if ((imgListView.Items.Count > 300) && (imgListView.Items.Count <= 400))
			{
				W = (imgListView.Width - (padding * 10)) / 10;
			}
			else if ((imgListView.Items.Count > 400) && (imgListView.Items.Count <= 500))
			{
				W = (imgListView.Width - (padding * 11)) / 11;
			}
			else
			{
				W = (imgListView.Width - (padding * 12)) / 12;
			}

			if (W < smallest)
				W = smallest;

			imgListView.ThumbnailSize = new Size(W, W);
		}

		private void imageListView_ItemDoubleClick(object sender, ItemClickEventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowPhotoView(int.Parse(((DetailViewImageListViewItemTag)(e.Item.Tag)).Index));
		}

		private void miSetCoverImage_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			if (m_selectedItem != null)
			{
				Cursor = Cursors.WaitCursor;

				try
				{
					string _cover_attach = Post.attachment_id_array[int.Parse(((DetailViewImageListViewItemTag)(m_selectedItem.Tag)).Index)];

					if (_cover_attach != m_post.cover_attach)
					{
						Dictionary<string, string> _params = new Dictionary<string, string>();
						_params.Add("cover_attach", _cover_attach);

						Post _retPost = Main.Current.PostUpdate(m_post, _params);

						if (_retPost == null)
						{
							return;
						}

						m_post = _retPost;
					}

					Main.Current.ShowStatuMessage(Resources.CHANGED_COVER_IMAGE, true);
				}
				catch
				{
				}

				Cursor = Cursors.Default;
			}
		}

		private void contextMenuStripImageList_Opening(object sender, CancelEventArgs e)
		{
			DebugInfo.ShowMethod();

			this.miSetCoverImage.Enabled = m_canEdit;

			if (m_selectedItem == null)
				return;

			var selected = getSelectedAttachmentDoc();

			detectDuplication(selected);
			miLocateOnDisk.Visible = (selected != null && !string.IsNullOrEmpty(selected["saved_file_name"].AsString));
		}

		private void detectDuplication(BsonDocument selected)
		{
			if (selected != null)
			{
				var selectedMD5 = selected.GetValue("md5", string.Empty).AsString;

				if (!string.IsNullOrEmpty(selectedMD5))
				{
					var total = attachmentCollection.Find(
						Query.And(Query.EQ("md5", selectedMD5), Query.EQ("group_id", Main.Current.RT.CurrentGroupID)));
					var totalCount = total.Count();

					if (totalCount > 1)
					{
						
						miDuplicate.Text = string.Format(Properties.Resources.DUPLICATE_PHOTO, (totalCount - 1));
						return;
					}
				}
			}

			miDuplicate.Text = Properties.Resources.NO_DUP_PHOTO;
		}

		private void imageListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			DebugInfo.ShowMethod();

			m_selectedItem = e.Item;
		}

		public ImageButton GetMoreFonction1()
		{
			DebugInfo.ShowMethod();

			return btnSaveAllPhotos;
		}

		public void MoreFonction1()
		{
			DebugInfo.ShowMethod();

			SaveAllPics();
		}

		private void miOpen_Click(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			ShowPhotoView(int.Parse(((DetailViewImageListViewItemTag)(m_selectedItem.Tag)).Index));
		}

		private bool CheckIfLoadingImage(ImageListViewItem imageListViewItem)
		{
			DebugInfo.ShowMethod();

			return imageListViewItem.FileName.Contains("LoadingImage.jpg");
		}

		public void SaveAllPics()
		{
			DebugInfo.ShowMethod();

			string _fileName = string.Empty;

			FolderBrowserDialog _dialog = new FolderBrowserDialog();

			_dialog.Description = Resources.SELECT_SAVE_PATH;
			_dialog.ShowNewFolderButton = true;
			_dialog.RootFolder = Environment.SpecialFolder.Desktop;
			_dialog.SelectedPath = Environment.SpecialFolder.Desktop.ToString();

			if (_dialog.ShowDialog(this) == DialogResult.OK)
			{
				string _folder = _dialog.SelectedPath + "\\";

				for (int i = 0; i < imageListView.Items.Count; i++)
				{
					if (CheckIfLoadingImage(imageListView.Items[i]))
						continue;

					var attachmentId = Post.attachment_id_array[i];
					_fileName = queryFileName(attachmentId, Path.GetExtension(imageListView.Items[i].FileName));
					_fileName = FileUtility.saveFileWithoutOverwrite(_fileName, _folder);

					var origFilePath = Main.Current.RT.REST.attachments_getOriginFilePath(attachmentId);
					var _OriginFileExist = Main.Current.IsPrimaryStation && File.Exists(origFilePath);


					try
					{
						if (_OriginFileExist)
						{
							File.Copy(origFilePath, _fileName);
						}
						else
						{
							File.Copy(imageListView.Items[i].FileName, _fileName);
						}
					}
					catch
					{
					}
				}

				MessageBox.Show(Resources.SAVE_ALL_OK, "Stream", MessageBoxButtons.OK,
								MessageBoxIcon.Information);
			}
		}

		private string queryFileName(string objectId, string extension)
		{
			DebugInfo.ShowMethod();

			var fileName = AttachmentCollection.QueryFileName(objectId);

			if (string.IsNullOrEmpty(fileName))
				return objectId + extension;
			else
				return fileName;
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

							// �L�o������
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
					MyParent.ExistPostAddPhotos(_pics, e.Index);
				}
			}
			catch
			{
			}

			e.Cancel = true;
		}

		#region Drag&Drop

		private void imageListView_DragDrop(object sender, DragEventArgs e)
		{
			DebugInfo.ShowMethod();

			ImageListView.HitInfo _hitInfo;
			imageListView.HitTest(imageListView.PointToClient(new Point(e.X, e.Y)), out _hitInfo);

			List<string> _pics = m_dragDropClipboardHelper.Drag_Drop_HtmlImage(e);

			if (_pics != null)
			{
				if ((_hitInfo.ItemIndex < 0) || (_hitInfo.ItemIndex >= imageListView.Items.Count))
					MyParent.ExistPostAddPhotos(_pics, 0);
				else
					MyParent.ExistPostAddPhotos(_pics, _hitInfo.ItemIndex);
			}

			FlashWindow.Stop(Main.Current);
		}

		private void imageListView_DragEnter(object sender, DragEventArgs e)
		{
			DebugInfo.ShowMethod();

			FlashWindow.Start(Main.Current);

			m_dragDropClipboardHelper.Drag_Enter_HtmlImage(e, false);

			DragHitTest(e);
		}

		private void imageListView_DragLeave(object sender, EventArgs e)
		{
			DebugInfo.ShowMethod();

			m_dragDropClipboardHelper.Drag_Leave_HtmlImage();

			FlashWindow.Stop(Main.Current);
		}

		private void imageListView_DragOver(object sender, DragEventArgs e)
		{
			DebugInfo.ShowMethod();

			m_dragDropClipboardHelper.Drag_Over_HtmlImage(e, false);

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

		private void propertyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selected = getSelectedAttachmentDoc();

			if (selected != null)
			{
				var imageMeta = selected.GetValue("image_meta", null).AsBsonDocument;

				if (imageMeta == null)
					return;

				var exif = imageMeta.GetValue("exif", null);

				if (exif == null)
					return;

				var exifItems = new List<KeyValuePair<string, string>>();
				foreach (var item in exif.AsBsonDocument)
				{
					if (item.Value is BsonNull)
						continue;

					if (item.Name == "GPSInfo")
						continue;

					if (item.Name == "gps")
					{
						foreach (var subItem in item.Value.AsBsonDocument)
						{
							exifItems.Add(new KeyValuePair<string, string>(subItem.Name, subItem.Value.ToString()));
						}
						continue;
					}

					exifItems.Add(new KeyValuePair<string, string>(item.Name, item.Value.ToString()));
				}


				//var gps = exif.GetValue("gps", null);

				//if (gps != null)
				//{
				//    foreach (var item in gps.AsBsonDocument)
				//    {
				//        if (item.Value is BsonNull)
				//            continue;
				//        exifItems.Add(new KeyValuePair<string, string>(item.Name, item.Value.ToString()));
				//    }
				//}


				using (var dialog = new PropertyDialog(exifItems))
				{
					dialog.StartPosition = FormStartPosition.CenterParent;
					dialog.ShowDialog();
				}
			}
		}

		private string getSelectedObjectId()
		{
			if (m_selectedItem == null)
				return null;

			DebugInfo.ShowMethod();

			var index = Int32.Parse(((DetailViewImageListViewItemTag)m_selectedItem.Tag).Index);
			return Post.attachment_id_array[index];
		}


		private BsonDocument getSelectedAttachmentDoc()
		{
			var object_id = getSelectedObjectId();

			if (object_id == null)
				return null;

			var selected = attachmentCollection.FindOneById(object_id);

			return selected;
		}

		private void miLocateOnDisk_Click(object sender, EventArgs e)
		{
			var object_id = getSelectedObjectId();
			
			if (object_id!=null)
			{
				var file_path = Main.Current.RT.REST.attachments_getOriginFilePath(object_id);
				Process.Start("explorer.exe", "/select, \"" + file_path + "\"");
			}
		}

	}
}

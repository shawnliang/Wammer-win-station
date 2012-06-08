#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Localization;

#endregion

namespace Waveface.DetailUI
{
    public class NewBatchPostItem_DV : UserControl, IDetailView
    {
        private Color BG_COLOR = Color.FromArgb(255, 255, 255);

        private IContainer components;
        private Post m_post;
        private ImageListView imageListViewTo;
        private ImageListView imageListViewFrom;
        private CultureManager cultureManager;
        private int m_itemCounts;

        private BatchPostItem m_batchPostItem;

        public Post Post
        {
            get { return m_post; }
            set
            {
                m_post = value;

                Relayout();

                m_batchPostItem = Main.Current.BatchPostManager.PhotoItems[0];

                LoatPictureToImageListViewFrom();

                Main.Current.BatchPostManager.UpdateCountUI += UpdateCountUI;
                Main.Current.BatchPostManager.UploadDone += UploadDone;
            }
        }

        public DetailView MyParent { get; set; }

        public NewBatchPostItem_DV()
        {
            InitializeComponent();

            InitImageListView();
        }

        private void InitImageListView()
        {
            MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer
                                                                 {
                                                                     ItemBorderless = true,
                                                                     ShowHovered = false
                                                                 };

            imageListViewFrom.SetRenderer(_imageListViewRenderer);
            imageListViewFrom.BackColor = BG_COLOR;
            imageListViewFrom.Colors.BackColor = BG_COLOR;
            imageListViewFrom.Colors.DisabledBackColor = BG_COLOR;
            imageListViewFrom.ThumbnailSize = new Size(96, 96);
            imageListViewFrom.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            MyImageListViewRenderer _imageListViewRenderer2 = new MyImageListViewRenderer
                                                                 {
                                                                     ItemBorderless = true,
                                                                     ShowHovered = false
                                                                 };

            imageListViewTo.SetRenderer(_imageListViewRenderer2);
            imageListViewTo.BackColor = BG_COLOR;
            imageListViewTo.Colors.BackColor = BG_COLOR;
            imageListViewTo.Colors.DisabledBackColor = BG_COLOR;
            imageListViewTo.ThumbnailSize = new Size(96, 96);
            imageListViewTo.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;
        }

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        // private void InitializeComponent()
        // {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewBatchPostItem_DV));
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.imageListViewFrom = new Manina.Windows.Forms.ImageListView();
            this.imageListViewTo = new Manina.Windows.Forms.ImageListView();
            this.SuspendLayout();
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // imageListViewFrom
            // 
            this.imageListViewFrom.AllowDuplicateFileNames = true;
            resources.ApplyResources(this.imageListViewFrom, "imageListViewFrom");
            this.imageListViewFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageListViewFrom.CacheLimit = "0";
            this.imageListViewFrom.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListViewFrom.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListViewFrom.DefaultImage")));
            this.imageListViewFrom.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListViewFrom.ErrorImage")));
            this.imageListViewFrom.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListViewFrom.Name = "imageListViewFrom";
            this.imageListViewFrom.UseEmbeddedThumbnails = Manina.Windows.Forms.UseEmbeddedThumbnails.Never;
            // 
            // imageListViewTo
            // 
            this.imageListViewTo.AllowDuplicateFileNames = true;
            resources.ApplyResources(this.imageListViewTo, "imageListViewTo");
            this.imageListViewTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageListViewTo.CacheLimit = "0";
            this.imageListViewTo.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListViewTo.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListViewTo.DefaultImage")));
            this.imageListViewTo.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListViewTo.ErrorImage")));
            this.imageListViewTo.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListViewTo.Name = "imageListViewTo";
            this.imageListViewTo.UseEmbeddedThumbnails = Manina.Windows.Forms.UseEmbeddedThumbnails.Never;
            // 
            // NewBatchPostItem_DV
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.imageListViewTo);
            this.Controls.Add(this.imageListViewFrom);
            resources.ApplyResources(this, "$this");
            this.Name = "NewBatchPostItem_DV";
            this.Resize += new System.EventHandler(this.NewBatchPostItem_DV_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public bool CanEdit()
        {
            return false;
        }

        public ImageButton GetMoreFonction1()
        {
            return null;
        }

        public void MoreFonction1()
        {
        }

        public void Relayout()
        {
            int _d = 12;

            int _h = (Height - 3 * _d) / 2;
            int _w = Width - 2 * _d;

            imageListViewFrom.Top = _d;
            imageListViewFrom.Height = _h;
            imageListViewFrom.Left = _d;
            imageListViewFrom.Width = _w;

            imageListViewTo.Top = _d + _h + _d;
            imageListViewTo.Height = _h;
            imageListViewTo.Left = _d;
            imageListViewTo.Width = _w;
        }

        private void NewBatchPostItem_DV_Resize(object sender, EventArgs e)
        {
            Relayout();
        }

        private void LoatPictureToImageListViewFrom()
        {
            m_itemCounts = m_batchPostItem.Files.Count;

            if (m_batchPostItem == null)
                return;

            imageListViewFrom.SuspendLayout();
            imageListViewTo.SuspendLayout();

            for (int i = 0; i < m_itemCounts; i++)
            {
                imageListViewFrom.Items.Add(m_batchPostItem.Files[i]);

                imageListViewTo.Items.Add(Main.Current.LoadingImagePath);

                DetailViewImageListViewItemTag _tag = new DetailViewImageListViewItemTag();
                _tag.Index = i.ToString();
                _tag.IsCoverImage = false;

                imageListViewFrom.Items[i].Tag = _tag;
                imageListViewTo.Items[i].Tag = _tag;
            }

            imageListViewFrom.ResumeLayout();
            imageListViewTo.ResumeLayout();
        }

        void UploadDone(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { UploadDone(text); }
                           ));
            }
            else
            {
            }
        }

        void UpdateCountUI(int count)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { UpdateCountUI(count); }
                           ));
            }
            else
            {
                while (imageListViewFrom.Items.Count > (m_itemCounts - count))
                {
                    imageListViewFrom.Items.RemoveAt(0);
                }

                for (int i = 0; i < count; i++)
                {
                    if (imageListViewTo.Items[i].FileName != m_batchPostItem.Files[i])
                        imageListViewTo.Items[i].FileName = m_batchPostItem.Files[i];
                }
            }
        }
    }
}
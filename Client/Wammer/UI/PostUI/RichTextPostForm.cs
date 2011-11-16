using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;

namespace Waveface.PostUI
{
    public partial class RichTextPostForm : Form
    {
        private ProgressDialog m_progressDialog;

        public RichText MyParent { get; set; }
        private List<string> m_remoteImagePaths;

        public RichTextPostForm()
        {
            InitializeComponent();

            imageListView.SetRenderer(new MyImageListViewRenderer());
        }

        private void RichTextPostForm_Load(object sender, EventArgs e)
        {
            m_remoteImagePaths = HtmlUtility.FetchImagesPath(MyParent.HTML);

            ThreadPool.QueueUserWorkItem(state => { ThreadMethod(); });
        }

        private void btnAutoGenTitle_Click(object sender, EventArgs e)
        {
            richTextBox.Text = HtmlUtility.StripHTML(MyParent.HTML).Trim();
        }

        private void btnClearText_Click(object sender, EventArgs e)
        {
            richTextBox.Text = "";
        }

        #region Get Image

        private void ThreadMethod()
        {
            GenerateWabPage(MyParent.HTML);

            GetAllRemoteImages();

            ClearUI();
        }

        private void GetAllRemoteImages()
        {
            foreach (string _path in m_remoteImagePaths)
            {
                try
                {
                    string _localFile = MainForm.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".png";

                    Image _img = HttpHelp.DownloadImage(_path);

                    _img.Save(_localFile);

                    AddToImageListView(_localFile, _path);
                }
                catch
                { }
            }
        }

        public void AddToImageListView(string filePath, string url)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               AddToImageListView(filePath, url);
                           }
                           ));
            }
            else
            {
                ImageListViewItem _item = new ImageListViewItem(filePath);
                _item.Tag = url;

                imageListView.Items.Add(_item);

                labelImageCount.Text = "[" + imageListView.Items.Count + "/" + (m_remoteImagePaths.Count + 1) + "]";
            }
        }

        public void ClearUI()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               ClearUI();
                           }
                           ));
            }
            else
            {
                labelImageCount.Text = "";

                if (imageListView.Items.Count > 0)
                    imageListView.Items[0].Selected = true;
            }
        }

        #endregion

        #region Generate Wab Page Image

        public void GenerateWabPage(string html)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               GenerateWabPage(html);
                           }
                           ));
            }
            else
            {
                try
                {
                    Bitmap _bmp = GenerateScreenshot(html, MyParent.HtmlEditorWidth, -1);

                    if (_bmp != null)
                    {
                        string _imgLocalPath = MainForm.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".png";
                        string _imgLocalPath_1 = MainForm.GCONST.TempPath + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_1.png";

                        _bmp.Save(_imgLocalPath_1, ImageFormat.Png);

                        File.Copy(_imgLocalPath_1, _imgLocalPath); //Hack: 原圖被Lock :(

                        AddToImageListView(_imgLocalPath, "[Thumbnail]");
                    }
                }
                catch
                {
                }
            }
        }

        public Bitmap GenerateScreenshot(string html)
        {
            // This method gets a screenshot of the webpage rendered at its full size (height and width)
            return GenerateScreenshot(html, -1, -1);
        }

        public Bitmap GenerateScreenshot(string html, int width, int height)
        {
            // Load the webpage into a WebBrowser control
            WebBrowser _wb = new WebBrowser();
            _wb.ScrollBarsEnabled = false;
            _wb.ScriptErrorsSuppressed = true;
            _wb.DocumentText = html;
            _wb.ScriptErrorsSuppressed = true;

            while (_wb.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();

            // Set the size of the WebBrowser control
            _wb.Width = width;
            _wb.Height = height;

            if (width == -1)
            {
                // Take Screenshot of the web pages full width
                if (_wb.Document.Body.ScrollRectangle.Width != 0)
                    _wb.Width = _wb.Document.Body.ScrollRectangle.Width;
                else
                    _wb.Width = 1024;
            }

            if (height == -1)
            {
                // Take Screenshot of the web pages full height
                if (_wb.Document.Body.ScrollRectangle.Height != 0)
                    _wb.Height = _wb.Document.Body.ScrollRectangle.Height;
                else
                    _wb.Height = 768;
            }

            try
            {
                // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
                Bitmap _bitmap = new Bitmap(_wb.Width, _wb.Height);
                _wb.DrawToBitmap(_bitmap, new Rectangle(0, 0, _wb.Width, _wb.Height));

                _wb.Dispose();

                return _bitmap;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region richTextBox

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox.SelectedText);
            richTextBox.SelectedText = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox.SelectedText);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject _data = Clipboard.GetDataObject();

            if (_data.GetDataPresent(DataFormats.Text))
            {
                richTextBox.SelectedText = _data.GetData(DataFormats.Text).ToString();
            }
        }

        private void contextMenuStripEdit_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Clipboard.ContainsData(DataFormats.Text))
            {
                pasteToolStripMenuItem.Enabled = false;
            }
            else
            {
                pasteToolStripMenuItem.Enabled = true;
            }
        }

        #endregion

        private void btnSend_Click(object sender, EventArgs e)
        {
            m_progressDialog = new ProgressDialog("Upload ..."
                     , delegate
                     {
                         string _ids = "[";

                         //Upload Image
                         if (imageListView.SelectedItems.Count == 0)
                             return "*ERROR*";

                         ImageListViewItem _imgItem = imageListView.SelectedItems[0];

                         string _imageFile = _imgItem.FileName;
                         string _imageURL = _imgItem.Tag.ToString();

                         try
                         {
                             MR_attachments_upload _uf = MainForm.THIS.File_UploadFile(_imageURL, _imageFile, true);

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

                         m_progressDialog.RaiseUpdateProgress(50);

                         if (m_progressDialog.WasCancelled)
                             return "*ERROR*";

                         try
                         {
                             MR_attachments_upload _uf = MainForm.THIS.File_UploadFile("_RichText_", MyParent.GetHtmlFile(), false);

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

                         _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
                         _ids += "]";

                         return (_ids);
                     }); // This value will be passed to the method

            // Then all you need to do is 
            m_progressDialog.ShowDialog();

            if (!m_progressDialog.WasCancelled && ((string)m_progressDialog.Result) != "*ERROR*")
            {
                try
                {
                    if (DoRealPost((string)m_progressDialog.Result))
                    {
                        DialogResult = DialogResult.Yes;
                        Close();
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
        }

        private bool DoRealPost(string files)
        {
            try
            {
                MR_posts_new _np = MainForm.THIS.Post_CreateNewPost(richTextBox.Text, files, "", "rtf");

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
    }
}

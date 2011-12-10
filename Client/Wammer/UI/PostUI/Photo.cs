
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.WebCam;

namespace Waveface.PostUI
{
    public partial class Photo : UserControl
    {
        private ProgressDialog m_progressDialog;

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

            imageListView.SetRenderer(new MyImageListViewRenderer());
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            // Refresh UI cues
            removeToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);
            //removeAllToolStripButton.Enabled = (imageListView.Items.Count > 0);

            deleteToolStripMenuItem.Enabled = (imageListView.SelectedItems.Count > 0);

            rotateCCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);
            rotateCWToolStripButton.Enabled = (imageListView.SelectedItems.Count > 0);
        }

        private void addToolStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imageListView.Items.AddRange(openFileDialog.FileNames);
            }
        }

        private void removeToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.SuspendLayout();

            foreach (var _item in imageListView.SelectedItems)
                imageListView.Items.Remove(_item);

            imageListView.ResumeLayout(true);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeToolStripButton_Click(sender, e);
        }

        private void removeAllToolStripButton_Click(object sender, EventArgs e)
        {
            imageListView.Items.Clear();

            MyParent.toPureText_Mode();
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

                                     MR_attachments_upload _uf = MainForm.THIS.RT.REST.File_UploadFile(_item.Text, _resizedImage, "", true);

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

            //單純文字
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
                MR_posts_new _np = MainForm.THIS.RT.REST.Post_CreateNewPost(MyParent.richTextBox.Text, files, "", _type);

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
            NewPostItem _newPostItem = new NewPostItem();
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

        #endregion
    }
}

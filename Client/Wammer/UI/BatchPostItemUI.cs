#region

using System;
using System.Threading;
using System.Windows.Forms;
using Waveface.API.V2;
using Waveface.Properties;
using XPExplorerBar;

#endregion

namespace Waveface
{
    public partial class BatchPostItemUI : UserControl
    {
        private int m_count;
        private Expando m_expando;
        private LeftArea m_leftArea;
        private NewPostItem m_newPostItem;
        private bool m_startUpload;

        public BatchPostItemUI(LeftArea leftArea, Expando expando, NewPostItem newPostItem)
        {
            m_leftArea = leftArea;
            m_expando = expando;
            m_newPostItem = newPostItem;

            InitializeComponent();
        }

        private void BatchPostItemUI_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(state => { ThreadMethod(); }); 
        }

        private void ThreadMethod()
        {
            string _ids = "[";

            while (true)
            {
                if (m_startUpload)
                {
                    SetLight(true);

                    string _item = m_newPostItem.Files[m_count];

                    try
                    {
                        string _text = new FileName(_item).Name;
                        string _resizedImage = ImageUtility.ResizeImage(_item, _text, m_newPostItem.ResizeRatio, 100);

                        MR_attachments_upload _uf = MainForm.THIS.RT.REST.File_UploadFile(_text, _resizedImage, "", true);

                        if (_uf == null)
                        {
                            continue;
                        }

                        _ids += "\"" + _uf.object_id + "\"" + ",";
                    }
                    catch
                    {
                        continue;
                    }

                    m_count++;

                    int _count = m_newPostItem.Files.Count;

                    ChangeProgressBarUI(m_count*100/_count, m_count + "/" + _count);

                    if (m_count == _count)
                        break;
                }
                else
                {
                    SetLight(false);

                    Thread.Sleep(1000);
                }
            }

            _ids = _ids.Substring(0, _ids.Length - 1); // 去掉最後一個","
            _ids += "]";

            while (true)
            {
                if (m_startUpload)
                {
                    SetLight(true);

                    try
                    {
                        MR_posts_new _np = MainForm.THIS.RT.REST.Posts_New(m_newPostItem.Text, _ids, "", "image");

                        if (_np == null)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    break;
                }
                else
                {
                    SetLight(false);

                    Thread.Sleep(1000);
                }
            }

            PostDone("OK");
        }

        public void ChangeProgressBarUI(int count, string countText)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                               {
                                   ChangeProgressBarUI(count, countText);
                               }
                           ));
            }
            else
            {
                progressBar.Value = count;
                progressBar.ProgressText = countText;
            }
        }

        public void PostDone(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                               {
                                   PostDone(text);
                               }
                           ));
            }
            else
            {
                MainForm.THIS.AfterBatchPostDone();
                DeleteThis();
            }
        }

        public void SetLight(bool yes)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                               {
                                   SetLight(yes);
                               }
                           ));
            }
            else
            {
                if (yes)
                    pictureBoxStatu.Image = Resources.postItem_green;
                else
                    pictureBoxStatu.Image = Resources.postItem_red;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            m_startUpload = !m_startUpload;

            if (m_startUpload)
            {
                buttonStart.Image = Resources.postItem_stop;
                buttonDelete.Visible = false;
                buttonDetail.Visible = false;
            }
            else
            {
                buttonStart.Image = Resources.postItem_play;
                buttonDelete.Visible = true;
                buttonDetail.Visible = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DialogResult _dr = MessageBox.Show("Are you sure you want to delete this item?", "Waveface", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (_dr == DialogResult.Yes)
                DeleteThis();
        }

        private void DeleteThis()
        {
            m_leftArea.DeletePostItem(this, m_expando, m_newPostItem);
        }

        private void buttonDetail_Click(object sender, EventArgs e)
        {
            BatchPostViewerForm _form = new BatchPostViewerForm(m_newPostItem.Text, m_newPostItem.Files);
            _form.ShowDialog();
        }
    }
}
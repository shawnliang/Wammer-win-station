#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using WebCam_Capture;

#endregion

namespace Waveface.WebCam
{
    public partial class WebCamForm : Form
    {
        private const int FrameNumber = 30;
        private PictureBox m_frameImage;
        private WebCamCapture m_webCamCapture;
        private bool m_isCaptured;

        public string CapturedImagePath { get; set; }

        public WebCamForm()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeWebCam(ref imgVideo);

                Start();
            }
            catch
            {
                Close();
            }
        }

        #region WebCam

        public void InitializeWebCam(ref PictureBox ImageControl)
        {
            m_webCamCapture = new WebCamCapture();
            m_webCamCapture.FrameNumber = ((0ul));
            m_webCamCapture.TimeToCapture_milliseconds = FrameNumber;
            m_webCamCapture.ImageCaptured += webcam_ImageCaptured;
            m_frameImage = ImageControl;
        }

        private void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            m_frameImage.Image = e.WebCamImage;
        }

        private void Start()
        {
            m_webCamCapture.TimeToCapture_milliseconds = FrameNumber;
            m_webCamCapture.Start(0);
        }

        private void Stop()
        {
            m_webCamCapture.Stop();
        }

        /*
        private void Continue()
        {
            // change the capture time frame
            m_webCamCapture.TimeToCapture_milliseconds = FrameNumber;

            // resume the video capture from the stop
            m_webCamCapture.Start(m_webCamCapture.FrameNumber);
        }
        */

        private void ResolutionSetting()
        {
            m_webCamCapture.Config();
        }

        private void AdvanceSetting()
        {
            m_webCamCapture.Config2();
        }

        #endregion

        private void bntCapture_Click(object sender, EventArgs e)
        {
            imgCapture.Image = imgVideo.Image;
            m_isCaptured = true;
        }

        private void bntSave_Click(object sender, EventArgs e)
        {
            if (m_isCaptured)
            {
                string _path = SaveImage(imgCapture.Image);

                if (_path != string.Empty)
                {
                    CapturedImagePath = _path;
                    DialogResult = DialogResult.Yes;
                    Close();
                }
            }

            Close();
        }

        private void bntVideoFormat_Click(object sender, EventArgs e)
        {
            ResolutionSetting();
        }

        private void bntVideoSource_Click(object sender, EventArgs e)
        {
            AdvanceSetting();
        }

        private string SaveImage(Image image)
        {
            SaveFileDialog _s = new SaveFileDialog();
            _s.FileName = "" + DateTime.Now.ToString("yyyyMMddHHmmss");
            _s.DefaultExt = ".Jpg";
            _s.Filter = "Image (.jpg)|*.jpg";

            if (_s.ShowDialog() == DialogResult.OK)
            {
                string _filename = _s.FileName;
                FileStream _fstream = new FileStream(_filename, FileMode.Create);
                image.Save(_fstream, ImageFormat.Jpeg);
                _fstream.Close();

                return _filename;
            }

            return string.Empty;
        }

        private void WebCamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }
    }
}
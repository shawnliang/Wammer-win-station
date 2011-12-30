#region

using System.IO;
using System.Threading;
using Waveface.API.V2;

#endregion

namespace Waveface
{
    public class UploadOriginPhotosToStation
    {
        private bool m_exit;

        public bool Pause { get; set; }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(state => { ThreadMethod(); });
        }

        private void ThreadMethod()
        {
            while (!m_exit)
            {
                if (Main.Current.RT.StationMode)
                {
                    string[] _filePaths = Directory.GetFiles(Main.GCONST.ImageUploadCachePath);

                    foreach (string _file in _filePaths)
                    {
                        if (Pause)
                            break;

                        FileName _fileName = new FileName(_file);

                        MR_attachments_upload _uf = Main.Current.RT.REST.File_UploadFile(_fileName.Name, _file, _fileName.NameNoExt, true);

                        if (_uf != null)
                        {
                            File.Delete(_file);
                        }

                        if (m_exit)
                            return;
                    }
                }

                Thread.Sleep(5000);
            }
        }

        public void Exit()
        {
            m_exit = true;
        }
    }
}
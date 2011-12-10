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

        public int SleepTime { get; set; }
        public bool Pause { get; set; }

        public UploadOriginPhotosToStation()
        {
            SleepTime = 3000;

            ThreadPool.QueueUserWorkItem(state => { ThreadMethod(); });
        }

        private void ThreadMethod()
        {
            while (!m_exit)
            {
                if (MainForm.THIS.RT.IsStationOK)
                {
                    string[] _filePaths = Directory.GetFiles(MainForm.GCONST.ImageUploadCachePath);

                    foreach (string _file in _filePaths)
                    {
                        if (Pause)
                            break;

                        FileName _fileName = new FileName(_file);

                        MR_attachments_upload _uf = MainForm.THIS.RT.REST.File_UploadFile(_fileName.Name, _file, _fileName.NameNoExt, true);

                        if (_uf != null)
                        {
                            File.Delete(_file);
                        }

                        if (m_exit)
                            return;
                    }
                }

                Thread.Sleep(SleepTime);
            }
        }

        public void Exit()
        {
            m_exit = true;
        }
    }
}
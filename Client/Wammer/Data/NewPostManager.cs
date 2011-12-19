
using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using Newtonsoft.Json;

namespace Waveface
{
    public class NewPostManager
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        public List<NewPostItem> Items { get; set; }

        public NewPostManager()
        {
            Items = new List<NewPostItem>();
        }

        public void Add(NewPostItem item)
        {
            Items.Add(item);
        }

        public void Remove(NewPostItem item)
        {
            Items.Remove(item);
        }

        #region IO

        public bool Save()
        {
            try
            {
                string _json = JsonConvert.SerializeObject(this);

                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_NP.dat";

                using (StreamWriter _outfile = new StreamWriter(_filePath))
                {
                    _outfile.Write(_json);
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Save");

                return false;
            }

            s_logger.Trace("Save: OK");

            return true;
        }

        public NewPostManager Load()
        {
            try
            {
                string _json = string.Empty;
                string _filePath = Main.GCONST.CachePath + Main.Current.RT.Login.user.user_id + "_NP.dat";

                StreamReader _sr = File.OpenText(_filePath);
                _json = _sr.ReadToEnd();
                _sr.Close();

                NewPostManager _npm = JsonConvert.DeserializeObject<NewPostManager>(_json);

                s_logger.Trace("Load:OK");

                return _npm;
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "Load");
            }

            return null;
        }

        #endregion
    }
}

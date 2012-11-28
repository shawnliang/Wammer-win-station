using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class GenericCommand
    {
        public ErrorMsg error { get; set; }
        public ConnectMsg connect { get; set; }
        public SubscribeMSg subscribe { get; set; }
        public NotifyMsg notify { get; set; }


        #region Station only web socket msgs
        public ImportMsg import { get; set; }
        public FileImportedMsg file_imported { get; set; }
        public ImportDoneMsg import_done { get; set; }
        public MetadataUploadedMsg metadata_uploaded { get; set; }
        #endregion

    }
}

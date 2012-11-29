using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class AttachmentsEventArgs : EventArgs
    {
        #region Var
        private IEnumerable<String> _ids;
        #endregion 

        #region Property
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public IEnumerable<String> IDs 
        {
            get
            {
                return _ids ?? (_ids = new List<String>());
            }
            private set
            {
                _ids = value;
            }
        }
        #endregion

        #region Constructor
        public AttachmentsEventArgs(IEnumerable<String> ids)
        {
            this.IDs = ids;
        }
        #endregion
    }
}

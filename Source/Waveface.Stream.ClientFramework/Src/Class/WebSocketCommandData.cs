using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketCommandData
    {
        #region Var
        private Dictionary<string, object> _parameters;
        #endregion

        #region Public Method
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>
        /// The name of the command.
        /// </value>
        public String CommandName { get; private set; }

        /// <summary>
        /// Gets the params.
        /// </summary>
        /// <value>
        /// The params.
        /// </value>
        public Dictionary<string, object> Parameters
        {
            get
            {
                return _parameters ?? (_parameters = new Dictionary<string, object>());
            }
            private set 
            {
                _parameters = value;
            }
        }

        /// <summary>
        /// Gets the memo.
        /// </summary>
        /// <value>
        /// The memo.
        /// </value>
        public Object Memo { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketCommandData" /> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="memo">The memo.</param>
        public WebSocketCommandData(string commandName, Dictionary<string, object> parameters, Object memo = null)
        {
            this.CommandName = commandName;
            this.Parameters = parameters;
            this.Memo = memo;
        }
        #endregion
    }
}

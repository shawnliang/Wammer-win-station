
namespace Waveface.Stream.Model
{
	class NullPerfCounter : IPerfCounter
	{
		#region Static Var
        private static NullPerfCounter _instance;
        #endregion

        #region Public Static Property
        public static NullPerfCounter Instance
        { 
            get
            {
                return _instance ?? (_instance = new NullPerfCounter());
            }
        }
        #endregion


        #region Constructor
        private NullPerfCounter()
        {

        }
        #endregion

		public void Increment()
		{
		}

		public void IncrementBy(long value)
		{
		}

		public void Decrement()
		{
		}

		public float NextValue()
		{
			return 0f;
		}
	}
}

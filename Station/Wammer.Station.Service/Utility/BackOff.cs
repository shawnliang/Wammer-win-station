using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wammer.Station.Utility
{
	public class BackOff
	{
		#region Var
		private int _level;
		private Random _random;
		#endregion

		#region Private Property
		private ReadOnlyCollection<int> m_BackOffWindows { get; set; }

		private Random m_Random
		{
			get
			{
				if (_random == null)
					_random = new Random(Guid.NewGuid().GetHashCode());
				return _random;
			}
		}
		#endregion

		#region Public Property
		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		/// <value>The level.</value>
		public int Level
		{
			get
			{
				return _level;
			}
			set
			{
				if (value <= 0)
					throw new Exception("Level value must be bigger than zero!");

				if (value > m_BackOffWindows.Count)
					throw new Exception("Level value incorrect!");

				_level = value;
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="BackOff"/> class.
		/// </summary>
		/// <param name="backOffWindows">The back off windows.</param>
		public BackOff(params int[] backOffWindows)
		{
			CheckBackOffWindows(backOffWindows);

			this.m_BackOffWindows = new ReadOnlyCollection<int>(backOffWindows.ToList());
			ResetLevel();
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Checks the back off windows.
		/// </summary>
		/// <param name="backOffWindows">The back off windows.</param>
		private void CheckBackOffWindows(params int[] backOffWindows)
		{
			if (backOffWindows == null)
				throw new ArgumentNullException("backOffWindows");

			if (backOffWindows.Length == 0)
				throw new ArgumentException("backOffWindows must contains elements!");

			if (backOffWindows.First() <= 0)
				throw new ArgumentOutOfRangeException("First backOffWindow must bigger than zero!");

			var minValue = -1;
			foreach (var backOffWindow in backOffWindows)
			{
				if (backOffWindow < 0)
					throw new ArgumentOutOfRangeException("backOffWindows must bigger than zero!");

				if (minValue > backOffWindow)
					throw new ArgumentOutOfRangeException("backOffWindows must order by increase!");

				if (minValue == backOffWindow)
					throw new ArgumentOutOfRangeException("Can't contains duplicated elements!");

				minValue = backOffWindow;
			}
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Increases the level.
		/// </summary>
		/// <returns></returns>
		public Boolean IncreaseLevel()
		{
			if (Level >= m_BackOffWindows.Count)
				return false;

			Level += 1;
			return true;
		}


		/// <summary>
		/// Decreases the level.
		/// </summary>
		/// <returns></returns>
		public Boolean DecreaseLevel()
		{
			if (Level <= 1)
				return false;

			Level -= 1;
			return true;
		}

		public void ResetLevel()
		{
			Level = 1;
		}

		/// <summary>
		/// Gets the next value.
		/// </summary>
		/// <returns></returns>
		public int NextValue()
		{
			var minValue = (Level == 1) ? 0 : m_BackOffWindows[Level - 2];
			var maxValue = m_BackOffWindows[Level - 1];

			return m_Random.Next(minValue, maxValue);
		}
		#endregion
	}
}

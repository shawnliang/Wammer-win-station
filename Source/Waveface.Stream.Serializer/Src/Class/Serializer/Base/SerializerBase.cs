using System;
using System.IO;
using System.Threading;

namespace Waveface.Stream.Serializer
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SerializerBase : ISerializer
	{
		#region Const

		private const int BUFFER_SIZE = 1024;

		#endregion


		#region Public Method

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		public void Serialize<T>(T obj, string file)
		{
			Serialize(obj, file, BUFFER_SIZE);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		public void Serialize<T>(T obj, string file, int bufferSize)
		{
			if (ReferenceEquals(obj, null))
				throw new ArgumentNullException("obj");

			if (String.IsNullOrEmpty(file))
				throw new ArgumentNullException("file");

			using (FileStream fs = File.Open(file, FileMode.Create, FileAccess.Write))
			{
				using (var bs = new BufferedStream(fs, bufferSize))
				{
					Serialize(obj, bs);
					bs.Flush();
				}
			}
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="stream">The stream.</param>
		public abstract void Serialize<T>(T obj, System.IO.Stream stream);


		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		public T DeSerialize<T>(string file)
		{
			return DeSerialize<T>(file, BUFFER_SIZE);
		}


		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <returns></returns>
		public T DeSerialize<T>(string file, int bufferSize)
		{
			if (String.IsNullOrEmpty(file))
				throw new ArgumentNullException("file");

			using (var fs = File.Open(file, FileMode.Open, FileAccess.Read))
			{
				using (var bs = new BufferedStream(fs, bufferSize))
				{
					return DeSerialize<T>(bs);
				}
			}
		}

		/// <summary>
		/// DeSerialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public abstract T DeSerialize<T>(System.IO.Stream stream);

		#endregion
	}
}
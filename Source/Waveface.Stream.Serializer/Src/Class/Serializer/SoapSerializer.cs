using System;
using System.Runtime.Serialization.Formatters.Soap;

namespace Waveface.Stream.Serializer
{
	/// <summary>
	/// 
	/// </summary>
	public class SoapSerializer : SerializerBase
	{
		#region Public Method

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="stream">The stream.</param>
		public override void Serialize<T>(T obj, System.IO.Stream stream)
		{
			if (ReferenceEquals(obj, null))
				throw new ArgumentNullException("obj");

			if (stream == null)
				throw new ArgumentNullException("stream");

			if (!stream.CanWrite)
				throw new ArgumentException("UnWritable stram.");

			var serializer = new SoapFormatter();
			serializer.Serialize(stream, obj);
		}

		/// <summary>
		/// DeSerialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public override T DeSerialize<T>(System.IO.Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			if (!stream.CanRead)
				throw new ArgumentException("UnReadable stream.");

			var serializer = new SoapFormatter();
			return (T)serializer.Deserialize(stream);
		}

		#endregion
	}
}
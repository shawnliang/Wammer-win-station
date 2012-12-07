
namespace Waveface.Stream.Serializer
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISerializer
	{
		#region Method

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		void Serialize<T>(T obj, string file);

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		void Serialize<T>(T obj, string file, int bufferSize);

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="stream">The stream.</param>
		void Serialize<T>(T obj, System.IO.Stream stream);

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		T DeSerialize<T>(string file);

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <returns></returns>
		T DeSerialize<T>(string file, int bufferSize);

		/// <summary>
		/// DeSerialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		T DeSerialize<T>(System.IO.Stream stream);

		#endregion
	}
}
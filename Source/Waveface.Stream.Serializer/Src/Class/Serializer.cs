using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Waveface.Stream.Serializer
{
	public static class Serializer
	{
		#region Static Var

		private static Dictionary<SerializerType, ISerializer> _pool;

		#endregion


		#region Private Static Property

		/// <summary>
		/// Gets the pool.
		/// </summary>
		/// <value>The pool.</value>
		/// <returns></returns>
		/// <remarks></remarks>
		private static Dictionary<SerializerType, ISerializer> m_Pool
		{
			get { return _pool ?? (_pool = new Dictionary<SerializerType, ISerializer>(Enum.GetNames(typeof(SerializerType)).Length)); }
		}

		#endregion


		#region Public Static Method

		/// <summary>
		/// Get the serializer.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static ISerializer GetSerializer(SerializerType type)
		{
			if (!m_Pool.ContainsKey(type))
			{
				if (type == SerializerType.None)
					throw new ArgumentOutOfRangeException("type");

				if (!Enum.IsDefined(typeof(SerializerType), type))
					throw new ArgumentOutOfRangeException("type");

				var attribute = type.GetCustomAttribute<RelativedTypeAttribute>();

				Debug.Assert(attribute != null, string.Format("{0} should be append with RelativedTypeAttribute", type.ToString()));
				if (attribute == null)
					return null;

				m_Pool[type] = Activator.CreateInstance(attribute.Type) as ISerializer;
			}

			return m_Pool[type];
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="type">The type.</param>
		public static void Serialize<T>(T obj, string file, SerializerType type)
		{
			GetSerializer(type).Serialize(obj,file);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <param name="type">The type.</param>
		public static void Serialize<T>(T obj, string file, int bufferSize, SerializerType type)
		{
			GetSerializer(type).Serialize(obj, file, bufferSize);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="stream">The stream.</param>
		/// <param name="type">The type.</param>
		public static void Serialize<T>(T obj, System.IO.Stream stream, SerializerType type)
		{
			GetSerializer(type).Serialize(obj, stream);
		}


		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static T DeSerialize<T>(string file, SerializerType type)
		{
			return GetSerializer(type).DeSerialize<T>(file);
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static T DeSerialize<T>(string file, int bufferSize, SerializerType type)
		{
			return GetSerializer(type).DeSerialize<T>(file, bufferSize);
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static T DeSerialize<T>(System.IO.Stream stream, SerializerType type)
		{
			return GetSerializer(type).DeSerialize<T>(stream);
		}

		#endregion
	}
}

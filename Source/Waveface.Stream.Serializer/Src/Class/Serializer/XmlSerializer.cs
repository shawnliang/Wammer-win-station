using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Waveface.Stream.Serializer
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlSerializer : SerializerBase
	{
		#region Const
		private const string SERIALIZER_DLL_EXTENSION = ".XmlSerializers.dll";
		private const string SERIALIZER_DLL_NAMESPACE_PATTERN = "Microsoft.Xml.Serialization.GeneratedAssembly.{0}Serializer";
		private const int POOL_BUFFER_SIZE = 25;
		private const int BUFFER_SIZE = 1024;
		#endregion


		#region Var

		private static Dictionary<Type, System.Xml.Serialization.XmlSerializer> _pool;

		#endregion

		#region Private Static Property

		/// <summary>
		/// Gets the pool.
		/// </summary>
		/// <value>The pool.</value>
		/// <returns></returns>
		/// <remarks></remarks>
		private static Dictionary<Type, System.Xml.Serialization.XmlSerializer> m_Pool
		{
			get { return _pool ?? (_pool = new Dictionary<Type, System.Xml.Serialization.XmlSerializer>(POOL_BUFFER_SIZE)); }
		}

		#endregion

		#region Private Static Method

		/// <summary>
		/// Gets the XML serializer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <remarks></remarks>
		public static System.Xml.Serialization.XmlSerializer GetXmlSerializer<T>()
		{
			return GetXmlSerializer(typeof(T));
		}

		/// <summary>
		/// Gets the XML serializer.
		/// </summary>
		/// <param name="objType">Type of the obj.</param>
		/// <returns></returns>
		public static System.Xml.Serialization.XmlSerializer GetXmlSerializer(Type objType)
		{
			lock (m_Pool)
			{
				if (!m_Pool.ContainsKey(objType))
				{
					string asmFile = objType.Assembly.Location;
					string serializerAssemblyFile = asmFile.Substring(0, asmFile.LastIndexOf(".", StringComparison.Ordinal)) +
													SERIALIZER_DLL_EXTENSION;
					if (File.Exists(serializerAssemblyFile))
					{
						Assembly serializerAssembly = Assembly.LoadFile(serializerAssemblyFile);
						Type serializerAssemblyObjType =
							serializerAssembly.GetType(string.Format(SERIALIZER_DLL_NAMESPACE_PATTERN,
																	 objType.Name));
						m_Pool.Add(objType,
								   serializerAssemblyObjType == null
									? new System.Xml.Serialization.XmlSerializer(objType)
									: (System.Xml.Serialization.XmlSerializer)Activator.CreateInstance(serializerAssemblyObjType));
					}
					else
					{
						m_Pool.Add(objType, new System.Xml.Serialization.XmlSerializer(objType));
					}
				}
			}
			return m_Pool[objType];
		}

		#endregion


		#region Public Method

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="password">The password.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		public void Serialize<T>(T obj, string file, string salt, string password, int bufferSize = BUFFER_SIZE)
		{
			if (string.IsNullOrEmpty(salt)) throw new ArgumentNullException("salt");
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
			Serialize(obj, file, true, XmlCryptography.GetCryptographyKey(salt, password), bufferSize);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="key">The key.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		public void Serialize<T>(T obj, string file, SymmetricAlgorithm key, int bufferSize = BUFFER_SIZE)
		{
			if (key == null) throw new ArgumentNullException("key");
			Serialize(obj, file, true, key, bufferSize);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="file">The file.</param>
		/// <param name="needEncrypt">if set to <c>true</c> [need encrypt].</param>
		/// <param name="key">The key.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		public void Serialize<T>(T obj, string file, bool needEncrypt, SymmetricAlgorithm key = null, int bufferSize = BUFFER_SIZE)
		{
			if (ReferenceEquals(obj, null))
				throw new ArgumentNullException("obj");

			if (String.IsNullOrEmpty(file))
				throw new ArgumentNullException("file");

			using (var fs = File.Open(file, FileMode.Create, FileAccess.Write))
			{
				using (var bs = new BufferedStream(fs, bufferSize))
				{
					Serialize(obj, bs, needEncrypt, key);
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
		/// <param name="salt">The salt.</param>
		/// <param name="password">The password.</param>
		public void Serialize<T>(T obj, System.IO.Stream stream, string salt, string password)
		{
			if (string.IsNullOrEmpty(salt)) throw new ArgumentNullException("salt");
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
			Serialize(obj, stream, XmlCryptography.GetCryptographyKey(salt, password));
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="stream">The stream.</param>
		/// <param name="key">The key.</param>
		public void Serialize<T>(T obj, System.IO.Stream stream, SymmetricAlgorithm key)
		{
			if (key == null) throw new ArgumentNullException("key");
			Serialize(obj, stream, true, key);
		}

		/// <summary>
		/// Serializes the specified obj.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The obj.</param>
		/// <param name="stream">The stream.</param>
		/// <param name="needEncrypt">if set to <c>true</c> [need encrypt].</param>
		/// <param name="key">The key.</param>
		public void Serialize<T>(T obj, System.IO.Stream stream, bool needEncrypt, SymmetricAlgorithm key = null)
		{
			if (!needEncrypt)
			{
				Serialize(obj, stream);
				return;
			}

			if (ReferenceEquals(obj, null))
				throw new ArgumentNullException("obj");

			if (stream == null)
				throw new ArgumentNullException("stream");

			if (!stream.CanWrite)
				throw new ArgumentException("UnWritable stram.");

			using (MemoryStream ms = new MemoryStream(BUFFER_SIZE))
			{
				System.Xml.Serialization.XmlSerializer serializer = GetXmlSerializer<T>();
				serializer.Serialize(ms, obj);
				ms.Seek(0, SeekOrigin.Begin);
				byte[] buffer = new byte[Convert.ToInt32(ms.Length) + 1];
				ms.Read(buffer, 0, buffer.Length);
				string plantText = System.Text.Encoding.UTF8.GetString(buffer);
				string encryptText = XmlCryptography.EncryptXML(plantText, key);

				byte[] encryptBuffer = System.Text.Encoding.UTF8.GetBytes(encryptText);
				stream.Write(encryptBuffer, 0, encryptBuffer.Length);
			}
		}


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

			System.Xml.Serialization.XmlSerializer serializer = GetXmlSerializer<T>();
			serializer.Serialize(stream, obj);
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="password">The password.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <returns></returns>
		public T DeSerialize<T>(string file, string salt, string password, int bufferSize = BUFFER_SIZE)
		{
			if (string.IsNullOrEmpty(salt)) throw new ArgumentNullException("salt");
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
			return DeSerialize<T>(file, XmlCryptography.GetCryptographyKey(salt, password), bufferSize);
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="key">The key.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <returns></returns>
		public T DeSerialize<T>(string file, SymmetricAlgorithm key, int bufferSize = BUFFER_SIZE)
		{
			if (key == null) throw new ArgumentNullException("key");
			return DeSerialize<T>(file, true, key, bufferSize);
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="file">The file.</param>
		/// <param name="needDecrypt">if set to <c>true</c> [need decrypt].</param>
		/// <param name="key">The key.</param>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <returns></returns>
		public T DeSerialize<T>(string file, bool needDecrypt, SymmetricAlgorithm key = null, int bufferSize = BUFFER_SIZE)
		{
			if (String.IsNullOrEmpty(file))
				throw new ArgumentNullException("file");

			using (var fs = File.Open(file, FileMode.Open, FileAccess.Read))
			{
				using (var bs = new BufferedStream(fs, bufferSize))
				{
					return DeSerialize<T>(bs, needDecrypt, key);
				}
			}
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		public T DeSerialize<T>(System.IO.Stream stream, string salt, string password)
		{
			if (string.IsNullOrEmpty(salt)) throw new ArgumentNullException("salt");
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");
			return DeSerialize<T>(stream, XmlCryptography.GetCryptographyKey(salt, password));
		}


		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public T DeSerialize<T>(System.IO.Stream stream, SymmetricAlgorithm key)
		{
			if (key == null) throw new ArgumentNullException("key");
			return DeSerialize<T>(stream, true, key);
		}

		/// <summary>
		/// Des the serialize.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream">The stream.</param>
		/// <param name="needDecrypt">if set to <c>true</c> [need decrypt].</param>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public T DeSerialize<T>(System.IO.Stream stream, bool needDecrypt, SymmetricAlgorithm key = null)
		{
			if (!needDecrypt)
			{
				return DeSerialize<T>(stream);
			}

			if (stream == null)
				throw new ArgumentNullException("stream");

			if (!stream.CanRead)
				throw new ArgumentException("UnReadable stream.");

			using (MemoryStream ms = new MemoryStream(BUFFER_SIZE))
			{
				string encryptText = null;
				using (StreamReader streamReader = new StreamReader(stream))
				{
					encryptText = streamReader.ReadToEnd();
				}
				string plantText = XmlCryptography.DecryptXML(encryptText, key);
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(plantText);
				ms.Write(buffer, 0, buffer.Length);
				ms.Seek(0, SeekOrigin.Begin);

				return (T)GetXmlSerializer<T>().Deserialize(ms);
			}
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

			System.Xml.Serialization.XmlSerializer serializer = GetXmlSerializer<T>();
			return (T)serializer.Deserialize(stream);
		}

		#endregion
	}
}
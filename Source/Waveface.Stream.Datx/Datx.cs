using System;
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//Author: Larry Nung
//Date: 2008/4/23
//File: 
//Memo: 
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
#region "Imports"
using System.IO;
using System.Collections.Specialized;
using System.Security;
using System.Runtime.InteropServices;
using Waveface.Stream.Serializer;

#endregion


//***************************************************************************
//Author: Larry Nung
//Date: 2008/6/10
//Purpose: 
//Memo: 
//***************************************************************************
/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
public sealed class Datx
{


	#region "Const"
	const bool DEFAULT_NEED_ENCRYPT_XML = true;
	const int DEFAULT_BUFFER_SIZE = 4096;
	#endregion
	const CompressLevel DEFAULT_COMPRESS_LEVEL = CompressLevel.Level5;


	#region "Var"
	#endregion
	private static bool _needEncryptXML = DEFAULT_NEED_ENCRYPT_XML;


	#region "Property"

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/12/23
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Gets or sets a value indicating whether [need encrypt XML].
	/// </summary>
	/// <value><c>true</c> if [need encrypt XML]; otherwise, <c>false</c>.</value>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool NeedEncryptXML
	{
		get { return _needEncryptXML; }
		set { _needEncryptXML = value; }
	}
	#endregion


	#region "Constructer"

	private Datx()
	{
	}
	#endregion



	#region "Public Shared Method"

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/19
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Removes the file.
	/// </summary>
	/// <param name="zipFilePath">The zip file path.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <remarks></remarks>
	public static void RemoveFile(string zipFilePath, string relativeFilePath)
	{
		Zip.DeleteFile(zipFilePath, relativeFilePath);
	}

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/17
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Determines whether [is file exist] [the specified zip file path].
	/// </summary>
	/// <param name="zipFilePath">The zip file path.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <returns>
	/// <c>true</c> if [is file exist] [the specified zip file path]; otherwise, <c>false</c>.
	/// </returns>
	/// <remarks></remarks>
	public static bool IsFileExist(string zipFilePath, string relativeFilePath)
	{
		return Zip.IsFileExist(zipFilePath, relativeFilePath);
	}

	/// <summary>
	/// Inserts the specified obj.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj">The obj.</param>
	/// <param name="filePath">The file path.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="passWord">The pass word.</param>
	public static void Insert<T>(T obj, string filePath, string relativeFilePath, SecureString passWord)
	{
		IntPtr stringPointer = Marshal.SecureStringToBSTR(passWord);
		try
		{
			Insert<T>(obj, filePath, relativeFilePath, Marshal.PtrToStringBSTR(stringPointer));
		}
		finally
		{
			Marshal.ZeroFreeBSTR(stringPointer);
		}
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/10
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Inserts the specified obj.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj">The obj.</param>
	/// <param name="filePath">The file path.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="passWord">The pass word.</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when the <paramref name=" filePath "/> 、 <paramref name=" obj "/> or <paramref name=" relativeFilePath "/> is nothing or empty.
	/// </exception>
	/// <remarks></remarks>
	public static void Insert<T>(T obj, string filePath, string relativeFilePath, string passWord = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/3/20
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(filePath))
		{
			throw new ArgumentNullException("filePath");
		}
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}


		using (MemoryStream ms = new MemoryStream(DEFAULT_BUFFER_SIZE))
		{
			//-------------------------------
			//Author: Larry Nung   Date:2008/3/20
			//Memo: 序列化
			//-------------------------------
			(Serializer.GetSerializer(SerializerType.Xml) as XmlSerializer).Serialize<T>(obj, ms, NeedEncryptXML);
			ms.Seek(0, SeekOrigin.Begin);
			//-------------------------------
			//Author: Larry Nung   Date:2008/4/21
			//Memo: 插入壓縮檔
			//-------------------------------
			Zip.InsertFile(filePath, ms, relativeFilePath, passWord);
		}

	}


	public static void Write<T>(T obj, string filePath, string relativeFilePath, SecureString passWord)
	{
		IntPtr stringPointer = Marshal.SecureStringToBSTR(passWord);
		try
		{
			Write(obj, filePath, relativeFilePath, Marshal.PtrToStringBSTR(stringPointer));
		}
		finally
		{
			Marshal.ZeroFreeBSTR(stringPointer);
		}
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/23
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Writes the specified obj.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj">The obj.</param>
	/// <param name="filePath">The file path.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="passWord">The pass word.</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when the <paramref name=" filePath "/> 、 <paramref name=" obj "/> or <paramref name=" relativeFilePath "/> is nothing or empty.
	/// </exception>
	/// <remarks></remarks>
	public static void Write<T>(T obj, string filePath, string relativeFilePath, string passWord = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/3/20
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(filePath))
		{
			throw new ArgumentNullException("filePath");
		}
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}


		using (MemoryStream ms = new MemoryStream(DEFAULT_BUFFER_SIZE))
		{
			//-------------------------------
			//Author: Larry Nung   Date:2008/3/20
			//Memo: 序列化
			//-------------------------------
			(Serializer.GetSerializer(SerializerType.Xml) as XmlSerializer).Serialize<T>(obj, ms, NeedEncryptXML);
			ms.Seek(0, SeekOrigin.Begin);
			//-------------------------------
			//Author: Larry Nung   Date:2008/4/21
			//Memo: 壓縮
			//-------------------------------           
			Zip.CompressFile(ms, filePath, relativeFilePath, DEFAULT_COMPRESS_LEVEL, passWord);
		}

	}



	public static T Read<T>(string filePath, string relativeFilePath, SecureString passWord)
	{
		IntPtr stringPointer = Marshal.SecureStringToBSTR(passWord);
		try
		{
			return Read<T>(filePath, relativeFilePath, Marshal.PtrToStringBSTR(stringPointer));
		}
		finally
		{
			Marshal.ZeroFreeBSTR(stringPointer);
		}
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/23
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Reads the specified file path.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="filePath">The file path.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="passWord">The pass word.</param>
	/// <returns></returns>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when the <paramref name=" filePath "/> or <paramref name=" relativeFilePath "/> is nothing or emmpty.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when the <paramref name=" filePath "/> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static T Read<T>(string filePath, string relativeFilePath, string passWord = "")
	{
		T functionReturnValue = default(T);
		//-------------------------------
		//Author: Larry Nung   Date:2008/3/20
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(filePath))
		{
			throw new ArgumentNullException("filePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException(string.Format("File \"{0}\" not found", filePath), "filePath");
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/3/20
		//Memo: 解序列化
		//-------------------------------
		using (MemoryStream ms = new MemoryStream())
		{
			Zip.DeCompressFile(filePath, relativeFilePath, ms, passWord);
			ms.Seek(0, SeekOrigin.Begin);
			functionReturnValue = (Serializer.GetSerializer(SerializerType.Xml) as XmlSerializer).DeSerialize<T>(ms, NeedEncryptXML);
		}
		return functionReturnValue;
	}




	//***************************************************************************
	//Author: Larry Nung
	//Date: 2009/5/13
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static StringCollection GetFileList(string filePath)
	{
		return Zip.GetFileList(filePath);
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2009/6/2
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 
	/// </summary>
	/// <param name="filePath"></param>
	/// <param name="relativeFilePath"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static StringCollection GetFileList(string filePath, string relativeFilePath)
	{
		return Zip.GetFileList(filePath, relativeFilePath);
	}
	#endregion



}
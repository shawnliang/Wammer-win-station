using System;
using System.Collections;
using System.Collections.Generic;
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//Author: Larry Nung
//Date: 2008/4/15
//File: Zip.vb
//Memo: ZIP Compression Class
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
#region "Imports"
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Collections.Specialized;
using System.Security;
using System.Runtime.InteropServices;

#endregion

/// <summary>
/// ZIP Compression Class
/// </summary>
public sealed class Zip
{

	#region "Const"
	const string DEFAULT_COMPRESS_FILE_COMMENT = "Waveface Zip File";
	const int DEFAULT_BUFFER_SIZE = 4096;
	const CompressLevel DEFAULT_COMPRESS_LEVEL = CompressLevel.Level5;
	#endregion





	#region "Constructer"

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/18
	//Purpose: 私有建構子
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 私有建構子
	/// </summary>
	/// <remarks></remarks>
	private Zip()
	{
	}
	#endregion





	#region "Shared Privated Method"

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/18
	//Purpose: 取得相對路徑
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 取得相對路徑
	/// </summary>
	/// <param name="basePath">基礎路徑</param>
	/// <param name="processPath">要處理的路徑</param>
	/// <returns>相對路徑</returns>
	/// <exception cref="System.ArgumentNullException">
	/// Throw when <paramref name="basePath"/> or <paramref name="processPath"/> is nothing.
	/// </exception>
	/// <remarks></remarks>
	private static string GetRelativePath(string basePath, string processPath)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/5/7
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(basePath))
		{
			throw new ArgumentNullException("basePath");
		}
		if (string.IsNullOrEmpty(processPath))
		{
			throw new ArgumentNullException("processPath");
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/7
		//Memo: 
		//-------------------------------
		basePath = basePath.Replace("\\", "/");
		processPath = processPath.Replace("\\", "/");
		if (!basePath.EndsWith("/"))
			basePath += "/";
		int intIndex = -1;
		int intPos = basePath.IndexOf("/");
		while (intPos >= 0)
		{
			intPos += 1;
			if (string.Compare(basePath, 0, processPath, 0, intPos, true) != 0)
				break; // TODO: might not be correct. Was : Exit While
			intIndex = intPos;
			intPos = basePath.IndexOf("/", intPos);
		}


		if (intIndex >= 0)
		{
			processPath = processPath.Substring(intIndex);
			intPos = basePath.IndexOf("/", intIndex);
			while (intPos >= 0)
			{
				intPos = basePath.IndexOf("/", intPos + 1);
			}
		}

		return processPath;
	}

	#endregion





	#region "Shared Public Method"
	/// <summary>
	/// Compresses the file.
	/// </summary>
	/// <param name="inputStream">The input stream.</param>
	/// <param name="outputStream">The output stream.</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="level">The level.</param>
	/// <param name="passWord">The pass word.</param>
	/// <param name="comment">The comment.</param>
	public static void CompressFile(Stream inputStream, Stream outputStream, string relativeFilePath, CompressLevel level, SecureString passWord, string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		CompressFile(inputStream, outputStream, relativeFilePath, level, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)), comment);
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 壓縮檔案
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 壓縮檔案
	/// </summary>
	/// <param name="inputStream">要壓縮的資料流</param>
	/// <param name="outputStream">輸出的資料流</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="level">壓縮等級 (數字越大壓縮率越高)</param>
	/// <param name="passWord">壓縮檔密碼</param>
	/// <param name="comment">The comment.</param>
	/// <exception cref="System.ArgumentNullException">
	/// Throw when <paramref name="inputStream" /> 、<paramref name="outputStream" /> or <paramref name="relativeFilePath" /> is nothing.
	/// </exception>
	/// <exception cref="System.ArgumentException">
	/// Throw when the R/W state of <paramref name="inputStream" /> or <paramref name="outputStream" /> is error.
	/// </exception>
	/// <remarks></remarks>
	public static void CompressFile(Stream inputStream, Stream outputStream, string relativeFilePath, CompressLevel level = DEFAULT_COMPRESS_LEVEL, string passWord = "", string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (inputStream == null)
		{
			throw new ArgumentNullException("inputStream");
		}
		if (outputStream == null)
		{
			throw new ArgumentNullException("outputStream");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!outputStream.CanWrite)
		{
			throw new ArgumentException("Stream's state of R/W has something error", "outputStream");
		}
		if (!inputStream.CanRead)
		{
			throw new ArgumentException("Stream's state of R/W has something error", "inputStream");
		}


		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 
		//-------------------------------
		int readBytes = 0;
		byte[] buffer = new byte[DEFAULT_BUFFER_SIZE + 1];
		ZipEntry entry = default(ZipEntry);


		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 壓縮
		//-------------------------------
		relativeFilePath = relativeFilePath.Replace("\\", "/");
		using (ZipOutputStream compressStream = new ZipOutputStream(outputStream))
		{
			compressStream.SetLevel((int)level);
			compressStream.SetComment(comment);
			compressStream.Password = passWord;
			compressStream.UseZip64 = UseZip64.Off;


			//-------------------------------
			//Author: Larry Nung   Date:2008/4/15
			//Memo: 放入ZipEntry
			//-------------------------------
			entry = new ZipEntry(relativeFilePath);
			entry.DateTime = DateTime.Now;
			compressStream.PutNextEntry(entry);

			//-------------------------------
			//Author: Larry Nung   Date:2008/4/15
			//Memo: 寫入檔案內容
			//-------------------------------
			readBytes = 0;
			do
			{
				readBytes = inputStream.Read(buffer, 0, buffer.Length);
				compressStream.Write(buffer, 0, readBytes);
			} while (readBytes > 0);


		}

	}


	public static void CompressFile(string inputFile, Stream outputStream, CompressLevel level, SecureString passWord, string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		CompressFile(inputFile, outputStream, level, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)), comment);
	}

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 壓縮檔案
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 壓縮檔案
	/// </summary>
	/// <param name="inputFile">要壓縮的檔案完整路徑</param>
	/// <param name="outputStream">輸出的資料流</param>
	/// <param name="level">壓縮等級 (數字越大壓縮率越高)</param>
	/// <param name="passWord">壓縮檔密碼</param>
	/// <param name="comment">The comment.</param>
	/// <exception cref="System.ArgumentNullException">
	/// Throw when <paramref name=" inputFile " /> or <paramref name=" outputStream " /> is nothing.
	/// </exception>
	/// <exception cref=" System.ArgumentException ">
	/// Throw when <paramref name=" outputStream " /> can't write.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" inputFile  " /> is not found
	/// </exception>
	/// <remarks></remarks>
	public static void CompressFile(string inputFile, Stream outputStream, CompressLevel level = DEFAULT_COMPRESS_LEVEL, string passWord = "", string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(inputFile))
		{
			throw new ArgumentNullException("inputFile");
		}
		if (outputStream == null)
		{
			throw new ArgumentNullException("outputStream");
		}
		if (!outputStream.CanWrite)
		{
			throw new ArgumentException("Output stream must can write", "outputStream");
		}
		if (!File.Exists(inputFile))
		{
			throw new FileNotFoundException(inputFile);
		}

		using (FileStream fs = File.OpenRead(inputFile))
		{
			CompressFile(fs, outputStream, Path.GetFileName(inputFile), level, passWord, comment);
		}
	}


	public static void CompressFile(Stream inputStream, string zipFilePath, string relativeFilePath, CompressLevel level, SecureString passWord, string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		CompressFile(inputStream, zipFilePath, relativeFilePath, level, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)), comment);
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 壓縮檔案
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 壓縮檔案
	/// </summary>
	/// <param name="inputStream">要壓縮的資料流</param>
	/// <param name="zipFilePath">輸出的壓縮檔完整路徑</param>
	/// <param name="relativeFilePath">The relative file path.</param>
	/// <param name="level">壓縮等級 (數字越大壓縮率越高)</param>
	/// <param name="passWord">壓縮檔密碼</param>
	/// <param name="comment">The comment.</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" inputStream " />、<paramref name=" zipFilePath " /> or <paramref name=" relativeFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.ArgumentException ">
	/// Throw when <paramref name=" inputStream " /> can't read.
	/// </exception>
	/// <remarks></remarks>
	public static void CompressFile(Stream inputStream, string zipFilePath, string relativeFilePath, CompressLevel level = DEFAULT_COMPRESS_LEVEL, string passWord = "", string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (inputStream == null)
		{
			throw new ArgumentNullException("inputStream");
		}
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!inputStream.CanRead)
		{
			throw new ArgumentException("Input stream must can read", "inputStream");
		}

		string path = Path.GetDirectoryName(zipFilePath);

		if (path.Length > 0)
			Directory.CreateDirectory(path);

		using (FileStream fs = File.Create(zipFilePath))
		{
			CompressFile(inputStream, fs, relativeFilePath, level, passWord, comment);
		}



	}



	public static void CompressFile(string inputFile, string zipFilePath, CompressLevel level, SecureString passWord, string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		CompressFile(inputFile, zipFilePath, level, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)), comment);
	}

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 壓縮檔案
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 壓縮檔案
	/// </summary>
	/// <param name="inputFile">要壓縮的檔案完整路徑</param>
	/// <param name="zipFilePath">輸出的壓縮檔完整路徑</param>
	/// <param name="level">壓縮等級 (數字越大壓縮率越高)</param>
	/// <param name="passWord">壓縮檔密碼</param>
	/// <param name="comment">The comment.</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" inputFile " /> or <paramref name=" zipFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" inputFile " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static void CompressFile(string inputFile, string zipFilePath, CompressLevel level = DEFAULT_COMPRESS_LEVEL, string passWord = "", string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(inputFile))
		{
			throw new ArgumentNullException("inputFile");
		}
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (!File.Exists(inputFile))
		{
			throw new FileNotFoundException(inputFile);
		}

		string path = Path.GetDirectoryName(zipFilePath);
		Directory.CreateDirectory(path);

		using (FileStream outputStream = File.Create(zipFilePath))
		{
			using (FileStream inputStream = File.OpenRead(inputFile))
			{
				CompressFile(inputStream, outputStream, Path.GetFileName(inputFile), level, passWord, comment);
			}
		}


		//'-------------------------------
		//'Author: Larry Nung   Date:2008/4/18
		//'Memo: 
		//'-------------------------------
		//Dim readBytes As Integer
		//Dim buffer(DEFAULT_BUFFER_SIZE) As Byte
		//Dim entry As ZipEntry


		//'-------------------------------
		//'Author: Larry Nung   Date:2008/4/15
		//'Memo: 壓縮
		//'-------------------------------
		//Try

		//    Using compressStream As New ZipOutputStream(File.Create(zipFile))
		//        With compressStream
		//            .SetLevel(level)
		//            .SetComment(comment)
		//            .Password = passWord
		//            .UseZip64 = UseZip64.Off
		//        End With


		//        '-------------------------------
		//        'Author: Larry Nung   Date:2008/4/15
		//        'Memo: 放入ZipEntry
		//        '-------------------------------
		//        entry = New ZipEntry(Path.GetFileName(inputFile))
		//        entry.DateTime = DateTime.Now
		//        compressStream.PutNextEntry(entry)

		//        '-------------------------------
		//        'Author: Larry Nung   Date:2008/4/15
		//        'Memo: 寫入檔案內容
		//        '-------------------------------
		//        Using inputFileStream As FileStream = File.OpenRead(inputFile)
		//            readBytes = 0
		//            Do
		//                readBytes = inputFileStream.Read(buffer, 0, buffer.Length)
		//                compressStream.Write(buffer, 0, readBytes)
		//            Loop While readBytes > 0
		//        End Using

		//    End Using
		//Finally

		//    '-------------------------------
		//    'Author: Larry Nung   Date:2008/5/9
		//    'Memo: 釋放資源
		//    '-------------------------------
		//    entry = Nothing
		//    buffer = Nothing
		//End Try


	}


	public static void CompressFolder(string inputFolder, string zipFilePath, CompressLevel level, SecureString passWord, string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		CompressFolder(inputFolder, zipFilePath, level, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)), comment);
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 壓縮目錄
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 壓縮目錄
	/// </summary>
	/// <param name="inputFolder">要壓縮的目錄路徑</param>
	/// <param name="zipFilePath">輸出的壓縮檔完整路徑</param>
	/// <param name="level">壓縮等級 (數字越大壓縮率越高)</param>
	/// <param name="passWord">壓縮檔密碼</param>
	/// <param name="comment">The comment.</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" inputFolder " /> or <paramref name=" zipFilePath " /> is nothing.
	/// </exception>
	/// <remarks></remarks>
	public static void CompressFolder(string inputFolder, string zipFilePath, CompressLevel level = DEFAULT_COMPRESS_LEVEL, string passWord = "", string comment = DEFAULT_COMPRESS_FILE_COMMENT)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(inputFolder))
		{
			throw new ArgumentNullException("inputFolder");
		}
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}

		if (!Directory.Exists(inputFolder))
		{
			throw new DirectoryNotFoundException(inputFolder);
		}




		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 變數宣告
		//-------------------------------
		int readBytes = 0;
		byte[] buffer = new byte[DEFAULT_BUFFER_SIZE + 1];
		IEnumerable<string> files = null;
		ZipEntry entry = default(ZipEntry);

		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 壓縮
		//-------------------------------
		using (ZipOutputStream compressStream = new ZipOutputStream(File.Create(zipFilePath)))
		{
			compressStream.SetLevel((int)level);
			compressStream.SetComment(comment);
			compressStream.Password = passWord;
			compressStream.UseZip64 = UseZip64.Off;


			files = Directory.GetFiles(inputFolder, "*.*", SearchOption.AllDirectories);

			foreach (string inputFile in files)
			{
				//-------------------------------
				//Author: Larry Nung   Date:2008/4/15
				//Memo: 放入ZipEntry
				//-------------------------------
				entry = new ZipEntry(GetRelativePath(inputFolder, inputFile));
				entry.DateTime = DateTime.Now;
				compressStream.PutNextEntry(entry);

				//-------------------------------
				//Author: Larry Nung   Date:2008/4/15
				//Memo: 寫入檔案內容
				//-------------------------------
				using (FileStream inputFileStream = File.OpenRead(inputFile))
				{
					readBytes = 0;
					do
					{
						readBytes = inputFileStream.Read(buffer, 0, buffer.Length);
						compressStream.Write(buffer, 0, readBytes);
					} while (readBytes > 0);
				}
			}

		}
	}



	public static void DeCompressFiles(string zipFilePath, string outputPath, SecureString passWord)
	{
		DeCompressFiles(zipFilePath, outputPath, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)));
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/18
	//Purpose: 解壓縮全部
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 解壓縮全部
	/// </summary>
	/// <param name="zipFilePath">欲解壓縮的壓縮檔完整路徑</param>
	/// <param name="outputPath">解壓縮後檔案存放位置</param>
	/// <param name="passWord">解壓縮密碼</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> or <paramref name=" outputPath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found
	/// </exception>
	/// <remarks></remarks>
	public static void DeCompressFiles(string zipFilePath, string outputPath, string passWord = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(outputPath))
		{
			throw new ArgumentNullException("outputPath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/12
		//Memo: 變數宣告
		//-------------------------------
		FastZip zip = null;

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/12
		//Memo: 解壓縮
		//-------------------------------
		zip = new FastZip();

		zip.Password = passWord;
		zip.ExtractZip(zipFilePath, outputPath, FastZip.Overwrite.Always, null, string.Empty, null, false);

	}


	public static void DeCompressFile(string zipFilePath, string relativeFilePath, Stream outputStream, SecureString passWord)
	{
		DeCompressFile(zipFilePath, relativeFilePath, outputStream, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)));
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/18
	//Purpose: 解壓縮指定檔案
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 解壓縮指定檔案
	/// </summary>
	/// <param name="zipFilePath">欲解壓縮的壓縮檔完整路徑</param>
	/// <param name="relativeFilePath">指定的檔案位置</param>
	/// <param name="outputStream">The output stream.</param>
	/// <param name="passWord">解壓縮密碼</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " />、<paramref name=" relativeFilePath " /> or <paramref name=" outputStream " /> is nothing.
	/// </exception>
	/// <exception cref=" System.ArgumentException ">
	/// Throw when <paramref name=" outputStream " /> can't write.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static void DeCompressFile(string zipFilePath, string relativeFilePath, Stream outputStream, string passWord = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (outputStream == null)
		{
			throw new ArgumentNullException("outputStream");
		}
		if (!outputStream.CanWrite)
		{
			throw new ArgumentException("Output stream must can write", "outputStream");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		ZipFile zip = null;
		int readBytes = 0;
		byte[] buffer = new byte[DEFAULT_BUFFER_SIZE + 1];

		try
		{
			relativeFilePath = relativeFilePath.Replace("\\", "/");
			if (IsFileExist(zipFilePath, relativeFilePath))
			{
				zip = new ZipFile(zipFilePath);
				zip.Password = passWord;
				Stream stream = zip.GetInputStream(zip.GetEntry(relativeFilePath));
				do
				{
					readBytes = stream.Read(buffer, 0, DEFAULT_BUFFER_SIZE);
					outputStream.Write(buffer, 0, readBytes);
				} while (readBytes > 0);
			}
			else
			{
				throw new FileNotFoundException(relativeFilePath);
			}

		}
		finally
		{
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}


		//'-------------------------------
		//'Author: Larry Nung   Date:2008/4/18
		//'Memo: 
		//'-------------------------------
		//Dim readBytes As Integer
		//Dim buffer(DEFAULT_BUFFER_SIZE) As Byte
		//Dim entry As ZipEntry


		//'-------------------------------
		//'Author: Larry Nung   Date:2008/4/15
		//'Memo: 壓縮
		//'-------------------------------
		//Try
		//    relativeFilePath = relativeFilePath.Replace("\", "/")

		//    Using deCompressStream As New ZipInputStream(File.OpenRead(zipFile))
		//        deCompressStream.Password = passWord

		//        '-------------------------------
		//        'Author: Larry Nung   Date:2008/4/15
		//        'Memo: 放入ZipEntry
		//        '-------------------------------
		//        Do
		//            entry = deCompressStream.GetNextEntry()

		//            If entry Is Nothing Then
		//                Exit Do
		//            End If

		//            If String.Compare(entry.Name, relativeFilePath, True) = 0 Then
		//                '-------------------------------
		//                'Author: Larry Nung   Date:2008/4/15
		//                'Memo: 寫入檔案內容
		//                '-------------------------------
		//                readBytes = 0
		//                Do
		//                    readBytes = deCompressStream.Read(buffer, 0, buffer.Length)
		//                    outputStream.Write(buffer, 0, readBytes)
		//                Loop While readBytes > 0

		//                Exit Do
		//            End If

		//        Loop


		//    End Using
		//Finally

		//    '-------------------------------
		//    'Author: Larry Nung   Date:2008/5/9
		//    'Memo: 釋放資源
		//    '-------------------------------
		//    entry = Nothing
		//    buffer = Nothing
		//End Try


	}


	public static void DeCompressFile(string zipFilePath, string relativeFilePath, string outputFile, SecureString passWord)
	{
		DeCompressFile(zipFilePath, relativeFilePath, outputFile, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)));
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/18
	//Purpose: 解壓縮指定檔案
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 解壓縮指定檔案
	/// </summary>
	/// <param name="zipFilePath">欲解壓縮的壓縮檔完整路徑</param>
	/// <param name="relativeFilePath">指定的檔案位置</param>
	/// <param name="outputFile">The output file.</param>
	/// <param name="passWord">解壓縮密碼</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " />、<paramref name=" relativeFilePath " /> or <paramref name=" outputFile " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" outputFile " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static void DeCompressFile(string zipFilePath, string relativeFilePath, string outputFile, string passWord = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (string.IsNullOrEmpty(outputFile))
		{
			throw new ArgumentNullException("outputFile");
		}

		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/9
		//Memo: 
		//-------------------------------
		string dir = Path.GetDirectoryName(outputFile);
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}
		using (FileStream fs = File.Create(outputFile))
		{
			DeCompressFile(zipFilePath, relativeFilePath, fs, passWord);
		}

	}


	public static void DeCompressFolder(string zipFilePath, string relativeFolderPath, string outputPath, bool reservePath, SecureString passWord)
	{
		DeCompressFolder(zipFilePath, relativeFolderPath, outputPath, reservePath, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)));
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2009/10/14
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Des the compress folder.
	/// </summary>
	/// <param name="zipFilePath">The zip file path.</param>
	/// <param name="relativeFolderPath">The relative folder path.</param>
	/// <param name="outputPath">The output path.</param>
	/// <param name="passWord">The pass word.</param>
	public static void DeCompressFolder(string zipFilePath, string relativeFolderPath, string outputPath, bool reservePath, string passWord = "")
	{
		StringCollection files = GetFileList(zipFilePath);
		string outputFile = null;
		relativeFolderPath = relativeFolderPath.Replace("\\", "/");
		foreach (string file in files)
		{
			var relativeFile = file.Replace("\\", "/");
			if (relativeFile.StartsWith(relativeFolderPath) && !relativeFile.EndsWith("/"))
			{
				outputFile = Path.Combine(outputPath, reservePath ? relativeFile : relativeFile.Replace(relativeFolderPath, string.Empty));
				DeCompressFile(zipFilePath, relativeFile, outputFile, passWord);
			}
		}
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/5/7
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 取得壓縮檔內檔案清單
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <returns>壓縮檔內檔案清單</returns>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static StringCollection GetFileList(string zipFilePath)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}


		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 變數宣告
		//-------------------------------
		StringCollection files = new StringCollection();
		ZipFile zip = new ZipFile(zipFilePath);
		try
		{
			foreach (ZipEntry entry in zip)
			{
				files.Add(entry.Name);
			}
			return files;

		}
		finally
		{
			//-------------------------------
			//Author: Larry Nung   Date:2008/5/9
			//Memo: 釋放資源
			//-------------------------------
			zip.Close();
			zip = null;
		}

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
	/// <param name="zipFilePath"></param>
	/// <param name="relativeFilePath"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static StringCollection GetFileList(string zipFilePath, string relativeFilePath)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}


		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 變數宣告
		//-------------------------------
		StringCollection files = new StringCollection();
		ZipFile zip = new ZipFile(zipFilePath);
		string entryName = null;
		try
		{
			relativeFilePath = relativeFilePath.Replace("\\", "/");
			foreach (ZipEntry entry in zip)
			{
				entryName = entry.Name;
				if (entryName.StartsWith(relativeFilePath))
				{
					files.Add(entryName);
				}
			}
			return files;

		}
		finally
		{
			//-------------------------------
			//Author: Larry Nung   Date:2008/5/9
			//Memo: 釋放資源
			//-------------------------------
			zip.Close();
			zip = null;
		}

	}





	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/5/7
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 判斷指定的檔案是否存在於壓縮檔內
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <param name="relativeFilePath">壓縮檔內指定檔案的相對路徑</param>
	/// <returns>指定的檔案是否存在於壓縮檔內</returns>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> or <paramref name=" relativeFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static bool IsFileExist(string zipFilePath, string relativeFilePath)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		ZipFile zip = null;

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 回傳檔案是否存在
		//-------------------------------
		relativeFilePath = relativeFilePath.Replace("\\", "/");
		try
		{
			zip = new ZipFile(zipFilePath);
			return zip.FindEntry(relativeFilePath, true) > -1;
		}
		finally
		{
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}


	}








	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/5/7
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 取得ZipEntry
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <param name="relativeFilePath">壓縮檔內檔案的相對路徑</param>
	/// <returns>ZipEntry</returns>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> or <paramref name=" relativeFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static ZipEntry GetZipEntry(string zipFilePath, string relativeFilePath)
	{
		ZipEntry functionReturnValue = default(ZipEntry);
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/18
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}
		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		ZipFile zip = null;

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 取得特定的ZipEntry
		//-------------------------------
		relativeFilePath = relativeFilePath.Replace("\\", "/");
		try
		{
			zip = new ZipFile(zipFilePath);
			functionReturnValue = zip.GetEntry(relativeFilePath);
		}
		finally
		{
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}
		return functionReturnValue;


	}


	public static void RenameFile(string zipFilePath, string oldRelativeFilePath, string newRelativeFilePath, SecureString passWord)
	{
		RenameFile(zipFilePath, oldRelativeFilePath, newRelativeFilePath, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(passWord)));
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2009/5/20
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 
	/// </summary>
	/// <param name="zipFilePath"></param>
	/// <param name="oldRelativeFilePath"></param>
	/// <param name="newRelativeFilePath"></param>
	/// <param name="passWord"></param>
	/// <remarks></remarks>
	public static void RenameFile(string zipFilePath, string oldRelativeFilePath, string newRelativeFilePath, string passWord = "")
	{
		if (oldRelativeFilePath == newRelativeFilePath)
		{
			throw new ArgumentException("oldRelativeFilePath can't equal to newRelativeFilePath");
		}
		using (MemoryStream ms = new MemoryStream())
		{
			DeCompressFile(zipFilePath, oldRelativeFilePath, ms, passWord);
			ms.Seek(0, SeekOrigin.Begin);
			DeleteFile(zipFilePath, oldRelativeFilePath);
			InsertFile(zipFilePath, ms, newRelativeFilePath, passWord);
		}
	}





	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/5/9
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 取得壓縮檔內的檔案數
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <returns>壓縮檔內的檔案數</returns>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static long GetFileCount(string zipFilePath)
	{
		long functionReturnValue = 0;
		//-------------------------------
		//Author: Larry Nung   Date:2008/5/8
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		ZipFile zip = null;

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 回傳檔案數
		//-------------------------------
		try
		{
			zip = new ZipFile(zipFilePath);
			functionReturnValue = zip.Count;
		}
		finally
		{
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}
		return functionReturnValue;

	}


	public static void InsertFile(string zipFilePath, Stream inputStream, string relativeFilePath, SecureString password)
	{
		InsertFile(zipFilePath, inputStream, relativeFilePath, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password)));
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 插入檔案到壓縮檔
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 插入檔案到壓縮檔
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <param name="inputStream">欲插入的檔案資料流</param>
	/// <param name="relativeFilePath">欲存放到壓縮檔內的相對路徑</param>
	/// <param name="password">壓縮檔密碼</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" inputStream " />、<paramref name=" zipFilePath " /> or <paramref name=" relativeFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.ArgumentException ">
	/// Throw when <paramref name=" inputStream " /> can't read.
	/// </exception>
	/// <remarks></remarks>
	public static void InsertFile(string zipFilePath, Stream inputStream, string relativeFilePath, string password = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (inputStream == null)
		{
			throw new ArgumentNullException("inputStream");
		}
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!inputStream.CanRead)
		{
			throw new ArgumentException("Input stream must can read", "inputStream");
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/17
		//Memo: 檔案不存在=>叫用CompressFile
		//-------------------------------
		if (!File.Exists(zipFilePath))
		{
			CompressFile(inputStream, zipFilePath, relativeFilePath, DEFAULT_COMPRESS_LEVEL, password);
			return;
		}
		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 區域變數宣告
		//-------------------------------
		ZipFile zip = null;
		int entryIdx = -1;

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 加入檔案
		//-------------------------------
		try
		{
			relativeFilePath = relativeFilePath.Replace("\\", "/");
			zip = new ZipFile(zipFilePath);

			entryIdx = zip.FindEntry(relativeFilePath, true);
			zip.BeginUpdate();
			zip.UseZip64 = UseZip64.Off;
			zip.Password = password;
			zip.Add(new ZipSource(inputStream), entryIdx == -1 ? relativeFilePath : zip[entryIdx].Name);
			zip.CommitUpdate();
		}
		finally
		{
			//-------------------------------
			//Author: Larry Nung   Date:2008/5/9
			//Memo: 資源釋放
			//-------------------------------
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}

		}


	}


	public static void InsertFile(string zipFilePath, string inputFile, string relativeFilePath, SecureString password)
	{
		InsertFile(zipFilePath, inputFile, relativeFilePath, Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(password)));
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/4/15
	//Purpose: 插入檔案到壓縮檔
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 插入檔案到壓縮檔
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <param name="inputFile">欲插入的檔案完整路徑</param>
	/// <param name="relativeFilePath">欲存放到壓縮檔內的相對路徑</param>
	/// <param name="password">壓縮檔密碼</param>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" inputFile " /> is nothing.
	/// </exception>
	/// <remarks></remarks>
	public static void InsertFile(string zipFilePath, string inputFile, string relativeFilePath, string password = "")
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (!File.Exists(inputFile))
		{
			throw new FileNotFoundException(inputFile);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 加入檔案
		//-------------------------------
		using (FileStream fs = File.OpenRead(inputFile))
		{
			InsertFile(zipFilePath, fs, relativeFilePath, password);
		}

	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/5/9
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 刪除壓縮檔內的檔案
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <param name="relativeFilePath">欲刪除的壓縮檔內的檔案相對路徑</param>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> or <paramref name=" relativeFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static void DeleteFile(string zipFilePath, string relativeFilePath)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/4/15
		//Memo: 參數檢查
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (string.IsNullOrEmpty(relativeFilePath))
		{
			throw new ArgumentNullException("relativeFilePath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 區域變數宣告
		//-------------------------------
		ZipFile zip = null;



		//-------------------------------
		//Author: Larry Nung   Date:2008/5/9
		//Memo: 刪除檔案
		//-------------------------------
		try
		{
			relativeFilePath = relativeFilePath.Replace("\\", "/");
			if (IsFileExist(zipFilePath, relativeFilePath))
			{
				zip = new ZipFile(zipFilePath);

				zip.BeginUpdate();
				zip.Delete(zip[zip.FindEntry(relativeFilePath, true)]);
				zip.CommitUpdate();
			}
			else
			{
				throw new FileNotFoundException("relativeFilePath");
			}

		}
		finally
		{
			//-------------------------------
			//Author: Larry Nung   Date:2008/5/9
			//Memo: 資源釋放
			//-------------------------------
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}

	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/17
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// Creates the empty zip file.
	/// </summary>
	/// <param name="zipFilePath">The zip file path.</param>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is nothing.
	/// </exception>
	/// <remarks></remarks>
	public static void CreateEmptyZipFile(string zipFilePath)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/6/17
		//Memo: 
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/17
		//Memo: 
		//-------------------------------
		ZipFile zip = ZipFile.Create(zipFilePath);
		try
		{
			zip.BeginUpdate();
			zip.CommitUpdate();
		}
		finally
		{
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}

	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/10
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 測試壓縮檔是否完好
	/// </summary>
	/// <param name="zipFilePath">壓縮檔完整路徑</param>
	/// <param name="testData">if set to <c>true</c> [test data].</param>
	/// <returns></returns>
	/// <exception cref=" System.ArgumentNullException ">
	/// Throw when <paramref name=" zipFilePath " /> is nothing.
	/// </exception>
	/// <exception cref=" System.IO.FileNotFoundException ">
	/// Throw when <paramref name=" zipFilePath " /> is not found.
	/// </exception>
	/// <remarks></remarks>
	public static bool TestZipFile(string zipFilePath, bool testData = false)
	{
		bool functionReturnValue = false;
		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		if (string.IsNullOrEmpty(zipFilePath))
		{
			throw new ArgumentNullException("zipFilePath");
		}
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException(zipFilePath);
		}

		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		ZipFile zip = null;
		//-------------------------------
		//Author: Larry Nung   Date:2008/6/10
		//Memo: 
		//-------------------------------
		try
		{
			zip = new ZipFile(zipFilePath);
			functionReturnValue = zip.TestArchive(testData);
		}
		finally
		{
			if (zip != null)
			{
				zip.Close();
				zip = null;
			}
		}
		return functionReturnValue;

	}



	//Public Shared Function CheckPassword(ByVal zipFile As String, ByVal password As String) As Boolean
	//    '-------------------------------
	//    'Author: Larry Nung   Date:2008/4/15
	//    'Memo: 參數檢查
	//    '-------------------------------
	//    If zipFile Is Nothing OrElse password Is Nothing Then
	//        Throw New ArgumentNullException
	//    End If
	//    zipFile = zipFile.Trim
	//    password = password.Trim
	//    If zipFile.Length = 0 Then
	//        Throw New ArgumentException
	//    End If
	//    If Not My.Computer.FileSystem.FileExists(zipFile) Then
	//        Throw New FileNotFoundException
	//    End If
	//    '-------------------------------
	//    'Author: Larry Nung   Date:2008/5/9
	//    'Memo: 區域變數宣告
	//    '-------------------------------
	//    Dim zip As ZipFile = Nothing

	//    Try
	//        zip = New ZipFile(zipFile)

	//        Return zip.Password = password
	//    Finally

	//        '-------------------------------
	//        'Author: Larry Nung   Date:2008/5/9
	//        'Memo: 資源釋放
	//        '-------------------------------
	//        If zip IsNot Nothing Then
	//            zip.Close()
	//            zip = Nothing
	//        End If
	//    End Try

	//End Function



	#endregion


}
using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Waveface
{
	// Exposes various file name parts (e.g. extension, path, name) as properties
	// and provides useful helper methods for working with file names.
	public sealed class FileName
	{
		public FileName()
		{
			m_strFileName = "";
		}

		public FileName(string strFileName)
		{
			m_strFileName = strFileName;
		}

		// Returns the extension with a leading '.' (e.g. .txt for Test.txt).  
		// If there's no extension, then an empty string is returned.
		public string Extension
		{
			get
			{
				char[] arChars = {'.', '\\', ':'};
				int i = m_strFileName.LastIndexOfAny(arChars);
				
				if ((i == c_iNPos) || (m_strFileName[i] != '.'))
					return ("");
				else
					return m_strFileName.Substring(i);
			}
			set
			{
				char[] arChars = {'.', '\\', ':'};
				int i = m_strFileName.LastIndexOfAny(arChars);
				
				if ((i == c_iNPos) || (m_strFileName[i] != '.'))
					m_strFileName += value;
				else
					m_strFileName = m_strFileName.Substring(0, i) + value;
			}
		}


		// Returns the name of the file with the path removed.
		public string Name
		{
			get
			{
				char[] arChars = {'\\', ':'};
				int i = m_strFileName.LastIndexOfAny(arChars);
				
				if (i >= m_strFileName.Length)
					return "";
				else
					return m_strFileName.Substring(i + 1);
			}
			set { m_strFileName = this.Path + value; }
		}


		// Returns the name of the file with the path and extension removed.
		public string NameNoExt
		{
			get
			{
				string strName = this.Name;
				int i = strName.LastIndexOf('.');
				
				if (i == c_iNPos)
					return strName;
				else
					return strName.Substring(0, i);
			}
		}


		// The path (e.g. C:\Test\ for C:\Test\File.txt).
		public string Path
		{
			get
			{
				char[] arChars = {'\\', ':'};
				int i = m_strFileName.LastIndexOfAny(arChars);
				
				if (i == c_iNPos)
					return "";
				else if (i >= m_strFileName.Length)
					return m_strFileName;
				else
					return m_strFileName.Substring(0, i + 1);
			}
			set
			{
				string strPath = value;
				
				if (strPath.Length > 0 && !strPath.EndsWith("\\"))
					strPath += "\\";

				m_strFileName = strPath + this.Name;
			}
		}

		// Drive letter (e.g. C:) or UNC server and share (e.g. \\TestServer\Share).
		public string Drive
		{
			get
			{
				int iLen = m_strFileName.Length;
				
				if ((iLen >= 2) && (m_strFileName[1] == ':'))
					return m_strFileName.Substring(0, 2);
				else if (iLen > 2 && m_strFileName.StartsWith(@"\\"))
				{
					int j = 0;
					int i = 2;
					
					while ((i < (iLen - 1)) && (j < 2))
					{
						if (m_strFileName[i] == '\\') 
							j++;

						if (j < 2)
							i++;
					}

					if (m_strFileName[i] == '\\')
						i--;

					return m_strFileName.Substring(0, i + 1);
				}
				else
					return "";
			}
		}

		// Returns the full file name.
		public string FullName
		{
			get { return m_strFileName; }
			set { m_strFileName = value; }
		}

		// Returns the full file name as a string.
		// [retrun] The full file name as a string.
		public override string ToString()
		{
			return m_strFileName;
		}

		// Fully expanded name and path
		public string ExpandedName
		{
			get { return GetFullPathName(m_strFileName); }
		}

		// Allows a FileName instance to be used as a string.
		// <param name="FN">A FileName instance</param>
		// [retrun] The string representation of the current filename.
		public static implicit operator string(FileName FN)
		{
			return FN.m_strFileName;
		}

		// Gets a FileInfo instance for the current file name.
		public FileInfo FileInfo
		{
			get { return new FileInfo(m_strFileName); }
		}

		// Returns true if the file exists on disk.
		public bool Exists
		{
			get { return System.IO.File.Exists(m_strFileName); }
		}

		// Gets the short 8.3 version of the full FileName.
		public string GetShortVersion()
		{
			return GetShortPathName(m_strFileName);
		}

		// Gets the long (non-8.3) version of the full FileName.
		public string GetLongVersion()
		{
			return GetLongPathName(m_strFileName);
		}

		// Loads the text from a file.
		public string LoadFromFile()
		{
			using (StreamReader SR = new StreamReader(m_strFileName))
			{
				SR.BaseStream.Seek(0, SeekOrigin.Begin);
				return SR.ReadToEnd();
			}
		}

		// Loads the lines of text from a file.
		public string[] LoadLinesFromFile()
		{
			using (StreamReader SR = new StreamReader(m_strFileName))
			{
				StringCollection Lines = new StringCollection();

				SR.BaseStream.Seek(0, SeekOrigin.Begin);
				
				while (SR.Peek() > -1)
					Lines.Add(SR.ReadLine());

				string[] arLines = new string[Lines.Count];
				Lines.CopyTo(arLines, 0);
				return arLines;
			}
		}

		// Saves the specified text to a file.
		// <param name="strText">The text to save.</param>
		public void SaveToFile(string strText)
		{
			SaveToFile(strText, false);
		}

		// Saves the specified text to a file.
		// <param name="strText">The text to save.</param>
		// <param name="bAppend">Whether the text is appended to an existing file.</param>
		public void SaveToFile(string strText, bool bAppend)
		{
			using (StreamWriter SW = new StreamWriter(m_strFileName, bAppend))
			{
				SW.Write(strText);
			}
		}

		// Returns the file path with a trailing '\'.
		// <param name="strFullName">The full name of a file.</param>
		public static string GetPath(string strFullName)
		{
			FileName FN = new FileName(strFullName);
			return FN.Path;
		}

		// Gets the extension for a file.  The return
		// value is always lowercase with a leading '.'.
		// <param name="strFullName">The full name of a file.</param>
		public static string GetExt(string strFullName)
		{
			FileName FN = new FileName(strFullName);
			return FN.Extension.ToLower();
		}

		#region Private File Methods

		[DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
		//I have no idea if the last parameter is correct.
		//The other parameters work, and that's all I care about.
		private static extern int GetFullPathName(string strFileName, int iBufSize, StringBuilder strBuffer, ref char chFilePart);

		[DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
		private static extern uint GetLongPathName(string strShortPath, StringBuilder bufLongPath, int uiBufferSize);

		[DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
		private static extern uint GetShortPathName(string strLongPath, StringBuilder bufLongPath, int uiBufferSize);

		/// Returns the name of the file with the path removed
		/// and optionally the extension removed.
		/// <param name="strFullName">The full name of a file.</param>
		/// <param name="bRemoveExt">Whether the extension should be removed.</param>
		public static string GetFileName(string strFullName, bool bRemoveExt)
		{
			FileName FN = new FileName(strFullName);
			
			if (bRemoveExt)
				return FN.NameNoExt;
			else
				return FN.Name;
		}

		public static string GetFullPathName(string strFileName)
		{
			StringBuilder strBuffer = new StringBuilder(4096);
			char chFilePart = '\0';
			GetFullPathName(strFileName, strBuffer.Capacity, strBuffer, ref chFilePart);
			return strBuffer.ToString();
		}

		public static string GetLongPathName(string strShortPath)
		{
			StringBuilder SB = new StringBuilder(260);
			uint uiResult = GetLongPathName(strShortPath, SB, SB.Capacity);
			
			if (uiResult > SB.Capacity)
			{
				SB = new StringBuilder((int) uiResult + 1);
				uiResult = GetLongPathName(strShortPath, SB, SB.Capacity);
			}

			if (uiResult == 0)
				throw new ArgumentException("Unable to get the long path name for: " + strShortPath);

			return SB.ToString();
		}

		public static string GetShortPathName(string strLongPath)
		{
			StringBuilder SB = new StringBuilder(260);
			uint uiResult = GetShortPathName(strLongPath, SB, SB.Capacity);
			
			if (uiResult > SB.Capacity)
			{
				SB = new StringBuilder((int) uiResult + 1);
				uiResult = GetShortPathName(strLongPath, SB, SB.Capacity);
			}

			if (uiResult == 0)
				throw new ArgumentException("Unable to get the short path name for: " + strLongPath);

			return SB.ToString();
		}

		#endregion

		private string m_strFileName;
		private const int c_iNPos = -1;
	}
}
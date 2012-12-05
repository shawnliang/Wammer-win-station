using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//Author: Larry Nung
//Date: 2008/6/9
//File: 
//Memo: 
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
#region "Imports"
using ICSharpCode.SharpZipLib.Zip;
#endregion


//***************************************************************************
//Author: Larry Nung
//Date: 2008/6/9
//Purpose: 
//Memo: 
//***************************************************************************
/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
internal class ZipSource : IStaticDataSource
{

	#region "Var"
	private System.IO.Stream _sourceStream;
	#endregion

	
	#region "Constructer"
	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/9
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 
	/// </summary>
	/// <param name="stream"></param>
	/// <remarks></remarks>
	public ZipSource(System.IO.Stream stream)
	{
		_sourceStream = stream;
	}
	#endregion

	#region "Implement IStaticDataSource"
	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/6/9
	//Purpose: 
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public System.IO.Stream GetSource()
	{
		return _sourceStream;
	}
	#endregion
}
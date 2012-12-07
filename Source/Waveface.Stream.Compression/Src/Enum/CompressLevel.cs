
//***************************************************************************
//Author: Larry Nung
//Date: 2008/4/15
//Purpose: 壓縮等級列舉
//Memo: 數字越大壓縮率越高
//***************************************************************************
/// <summary>
/// 壓縮等級列舉 (數字越大壓縮率越高)
/// </summary>
/// <remarks></remarks>
public enum CompressLevel : int
{
	/// <summary>
	/// 壓縮等級0 (壓縮率最小,速度最快)
	/// </summary>
	Level0,
	/// <summary>
	/// 壓縮等級1
	/// </summary>
	Level1,
	/// <summary>
	/// 壓縮等級2
	/// </summary>
	Level2,
	/// <summary>
	/// 壓縮等級3
	/// </summary>
	Level3,
	/// <summary>
	/// 壓縮等級4
	/// </summary>
	Level4,
	/// <summary>
	/// 壓縮等級5
	/// </summary>
	Level5,
	/// <summary>
	/// 壓縮等級6
	/// </summary>
	Level6,
	/// <summary>
	/// 壓縮等級7
	/// </summary>
	Level7,
	/// <summary>
	/// 壓縮等級8
	/// </summary>
	Level8,
	/// <summary>
	/// 壓縮等級9 (壓縮率最大,速度最慢)
	/// </summary>
	Level9
}
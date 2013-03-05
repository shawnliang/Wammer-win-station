//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//Author: Larry Nung
//Date: 2008/1/2
//File: DebugInfo.vb
//Memo: 
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
#region "Imports"
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
#endregion


//***************************************************************************
//Author: Larry Nung
//Date: 2008/6/25
//Purpose: 
//Memo: 
//***************************************************************************
/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
public sealed class DebugInfo
{
	#region "Const"
	private const string METHOD_NAME_PATTERN = "{0}.{1}";
	private const string SHOW_METHOD_PATTERN = "Execute Method: {0} ({1}) => {2}";
	private const string BUG_EXPORT_DLG_TITLE = "Export Bug Report";
	private const string BUG_FILE_EXT_FILE = "Bug";
	private const string BUG_FILE_FILTER = "Bug Report File(*.Brf)|*.Brf";
	private const string BUG_REPORT_TITLE = "Error occur";
	private const string BUG_REPORT_MSG = @"Some error occur.
												Please export bug report,and send to engineer.";
	#endregion

	#region "Constructer"

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: 建構子
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 建構子
	/// </summary>
	/// <remarks></remarks>

	private DebugInfo()
	{
	}
	#endregion

	#region "Public Method"

	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: 顯示目前執行的Method名稱(只有在Debug模式下會執行到此副程式)
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 顯示目前執行的Method名稱(只有在Debug模式下會執行到此副程式)
	/// </summary>
	/// <remarks></remarks>
	[Conditional("DEBUG")]
	public static void ShowMethod()
	{
		System.Diagnostics.Debug.WriteLine(GetMethod(2));
	}

	public static string GetMethod()
	{
		return GetMethod(2);
	}

	public static string GetMethod(int frameNum)
	{
		string MethodName = null;
		string FileName = null;
		string LineNum = null;

		StackTrace st = new StackTrace(true);
		StackFrame sf = st.GetFrame(frameNum);
		MethodBase mb = sf.GetMethod();

		MethodName = string.Format(METHOD_NAME_PATTERN, mb.ReflectedType.FullName, mb.Name);
		FileName = System.IO.Path.GetFileName(sf.GetFileName());
		LineNum = sf.GetFileLineNumber().ToString();

		return string.Format(SHOW_METHOD_PATTERN, FileName, LineNum, MethodName);
	}


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: 設定當錯誤發生..彈出BugReport
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 設定當錯誤發生..彈出BugReport
	/// </summary>
	/// <remarks></remarks>
	public static void ShowBugReportOnError()
	{
		AppDomain.CurrentDomain.UnhandledException += OnThreadException;
	}


	#endregion

	#region "Private Method"


	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: 取得詳細的錯誤資訊
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 取得詳細的錯誤資訊
	/// </summary>
	/// <param name="ex">Exception型態變數</param>
	/// <returns>詳細的錯誤資訊</returns>
	/// <remarks></remarks>
	private static string GetDetailErrorMsg(System.Exception ex)
	{
		if (ex == null)
		{
			return string.Empty;
		}
		StringBuilder errorMsg = new StringBuilder(256);

		//Larry 2008/1/2 拼湊詳細錯誤資訊
		errorMsg.AppendLine("Error Time: " + DateTime.Now.ToString() + Environment.NewLine);
		errorMsg.AppendLine("Error Source: " + ex.Source + Environment.NewLine);
		errorMsg.AppendLine("Error Msg: " + Environment.NewLine + ex.Message + Environment.NewLine);
		errorMsg.AppendLine("Inner Error Msg: " + (ex.InnerException == null ? "None" : Environment.NewLine + ex.InnerException.Message + Environment.NewLine));
		errorMsg.AppendLine("Detail Error Msg: " + Environment.NewLine + ex.ToString() + Environment.NewLine);
		errorMsg.AppendLine("========== Modules ==========");
		using (Process process = Process.GetCurrentProcess())
		{
			foreach (ProcessModule ProcessModule in process.Modules)
			{
				errorMsg.Append("Module Name: ");
				errorMsg.AppendLine(ProcessModule.ModuleName);
				errorMsg.Append("Module Version: ");
				errorMsg.AppendLine(ProcessModule.FileVersionInfo.FileVersion);
				errorMsg.AppendLine();
			}
		}
		return errorMsg.ToString();
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: ThreadException錯誤發生..彈出BugReport
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// ThreadException錯誤發生..彈出BugReport
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The <see cref="System.Threading.ThreadExceptionEventArgs" /> instance containing the event data.</param>
	/// <remarks></remarks>
	private static void OnThreadException(object sender, UnhandledExceptionEventArgs e)
	{
		ShowBugReport(e.ExceptionObject as Exception);
	}



	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: 顯示Bug Report對話框
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 顯示Bug Report對話框
	/// </summary>
	/// <param name="ex">Exception型態變數</param>
	/// <remarks></remarks>
	private static void ShowBugReport(System.Exception ex)
	{
		//-------------------------------
		//Author: Larry Nung   Date:2008/6/20
		//Memo: 區域變數宣告
		//-------------------------------
		Label lblErrorMsg = null;
		TextBox tbxDetailErrorMsg = null;
		Button btnReport = null;
		Button btnCancel = null;
		try
		{
			using (Form bugReportDlg = new Form())
			{
				int Padding = 5;

				//-------------------------------
				//Author: Larry Nung   Date:2008/1/2
				//Memo: 設定Bug Report表單
				//-------------------------------
				bugReportDlg.Text = BUG_REPORT_TITLE;
				bugReportDlg.MaximizeBox = false;
				bugReportDlg.MinimizeBox = false;
				bugReportDlg.Size = new Size(416, 310);
				bugReportDlg.FormBorderStyle = FormBorderStyle.Sizable;
				bugReportDlg.StartPosition = FormStartPosition.CenterScreen;

				//-------------------------------
				//Author: Larry Nung   Date:2008/1/2
				//Memo: 設定Bug Report對話框顯示的Error Msg
				//-------------------------------
				lblErrorMsg = new Label();
				lblErrorMsg.Text = BUG_REPORT_MSG;
				lblErrorMsg.AutoSize = true;
				lblErrorMsg.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
				lblErrorMsg.Parent = bugReportDlg;
				lblErrorMsg.Location = new Point(Padding, Padding);


				//-------------------------------
				//Author: Larry Nung   Date:2008/1/21
				//Memo: 
				//-------------------------------
				tbxDetailErrorMsg = new TextBox();
				tbxDetailErrorMsg.ReadOnly = true;
				tbxDetailErrorMsg.ScrollBars = ScrollBars.Both;
				tbxDetailErrorMsg.Multiline = true;
				tbxDetailErrorMsg.Text = GetDetailErrorMsg(ex);
				tbxDetailErrorMsg.Parent = bugReportDlg;
				tbxDetailErrorMsg.TabStop = false;
				tbxDetailErrorMsg.Bounds = new System.Drawing.Rectangle(Padding, lblErrorMsg.Bottom + Padding, 400, 200);
				tbxDetailErrorMsg.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;


				//-------------------------------
				//Author: Larry Nung   Date:2008/1/2
				//Memo: 設定Export Bug Report按鈕
				//-------------------------------
				btnReport = new Button();
				btnReport.Parent = bugReportDlg;
				btnReport.Text = "Export Bug Report";
				btnReport.AutoSize = true;
				btnReport.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				btnReport.Top = tbxDetailErrorMsg.Bottom + Padding;
				btnReport.Left = Padding;
				btnReport.DialogResult = DialogResult.OK;
				btnReport.TabIndex = 0;


				//-------------------------------
				//Author: Larry Nung   Date:2008/1/2
				//Memo: 設定Cancel按鈕
				//-------------------------------
				btnCancel = new Button();
				btnCancel.Parent = bugReportDlg;
				btnCancel.Text = "Cancel";
				btnCancel.AutoSize = true;
				btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				btnCancel.Top = tbxDetailErrorMsg.Bottom + Padding;
				btnCancel.Left = btnReport.Right + Padding;
				btnCancel.DialogResult = DialogResult.Cancel;
				btnCancel.TabIndex = 1;


				if (bugReportDlg.ShowDialog() == DialogResult.OK)
				{
					ExportBugReport(tbxDetailErrorMsg.Text);
				}
			}
		}
		finally
		{
			if (lblErrorMsg != null)
			{
				lblErrorMsg.Dispose();
				lblErrorMsg = null;
			}
			if (tbxDetailErrorMsg != null)
			{
				tbxDetailErrorMsg.Dispose();
				tbxDetailErrorMsg = null;
			}
			if (btnReport != null)
			{
				btnReport.Dispose();
				btnReport = null;
			}
			if (btnCancel != null)
			{
				btnCancel.Dispose();
				btnCancel = null;
			}
		}

	}




	//***************************************************************************
	//Author: Larry Nung
	//Date: 2008/1/30
	//Purpose: 匯出Bug Report檔
	//Memo: 
	//***************************************************************************
	/// <summary>
	/// 匯出Bug Report檔
	/// </summary>
	/// <param name="detailErrorMsg">詳細的錯誤訊息</param>
	/// <remarks></remarks>
	private static void ExportBugReport(string detailErrorMsg)
	{
		//Larry 2008/1/2 設定儲存對話框
		using (SaveFileDialog saveDlg = new SaveFileDialog())
		{
			saveDlg.Filter = BUG_FILE_FILTER;
			saveDlg.DefaultExt = BUG_FILE_EXT_FILE;
			saveDlg.Title = BUG_EXPORT_DLG_TITLE;
			saveDlg.RestoreDirectory = true;
			if (saveDlg.ShowDialog() == DialogResult.OK)
			{
				//Larry 2008/1/2 把詳細的錯誤資訊儲存到所指定的檔案位置
				System.IO.File.WriteAllText(saveDlg.FileName, detailErrorMsg);
			}
		}
	}
	#endregion
}

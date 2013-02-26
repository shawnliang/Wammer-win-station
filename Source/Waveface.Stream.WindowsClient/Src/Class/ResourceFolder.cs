using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	static class ResourceFolder
	{
		public static string GetResFolder(string user_id)
		{
			var user = DriverCollection.Instance.FindOneById(user_id);

			if (user == null)
				throw new Exception("Unknown user: " + user_id);

			return user.folder;
		}

		public static void Change(string user_id, string session_token, Action<string> moveCompleteCB)
		{
			try
			{
				var oldFolder = GetResFolder(user_id);

				using (FolderBrowserDialog dialog = new FolderBrowserDialog())
				{
					dialog.SelectedPath = Path.GetDirectoryName(oldFolder);

					if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) // cancelled
						return;

					var newFolder = Path.Combine(dialog.SelectedPath, "aostream");

					if (newFolder.Equals(oldFolder, StringComparison.InvariantCultureIgnoreCase)) // not changed
					{
						MessageBox.Show(Resources.MoveFolderSameFolder, Resources.MoveFolderSelectError, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					DialogResult confirm = MessageBox.Show(
						string.Format(Resources.MoveFolderConfirmMsg, oldFolder, newFolder),
						Resources.MoveFolderConfirmTitle, MessageBoxButtons.OKCancel,
						MessageBoxIcon.Question);

					if (confirm != System.Windows.Forms.DialogResult.OK)	// cancelled
						return;

					if (!Directory.Exists(newFolder))
						Directory.CreateDirectory(newFolder);

					var progressing = new ProcessingDialog();

					var bgworker = new BackgroundWorker();
					bgworker.DoWork += (sender, arg) =>
					{
						StationAPI.MoveFolder(StreamClient.Instance.LoginedUser.UserID, newFolder, StreamClient.Instance.LoginedUser.SessionToken);
					};

					bgworker.RunWorkerCompleted += (sender, arg) =>
					{
						progressing.Close();

						if (arg.Error != null)
						{
							MessageBox.Show(arg.Error.GetDisplayDescription(), Resources.MoveFolderError);
						}
						else
						{
							if (moveCompleteCB != null)
								moveCompleteCB(newFolder);
						}
					};
					bgworker.RunWorkerAsync();

					progressing.Text = Resources.MovingResourceFolder;
					progressing.StartPosition = FormStartPosition.CenterParent;
					progressing.ProgressStyle = ProgressBarStyle.Marquee;
					progressing.ShowDialog();
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, Resources.MoveFolderError, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}

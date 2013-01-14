using Wammer.Cloud;
using Waveface.Stream.Model;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Linq;
using System;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/Import")]
	public class ImportAPIHandler : HttpHandler
	{
		#region Public Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			const string PATHS_KEY = "paths";
			CheckParameter(PATHS_KEY, CloudServer.PARAM_GROUP_ID);

			var apiKey = Parameters[CloudServer.PARAM_API_KEY];
			var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
			var groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			var paths = Parameters[PATHS_KEY];

			var user = DriverCollection.Instance.FindDriverByGroupId(groupID);
			if (user == null)
				throw new WammerStationException("user not exist", (int)StationApiError.UserNotExist);

			var task = new ImportTask(apiKey, sessionToken, groupID, paths);
			task.TaskStarted += new EventHandler<TaskStartedEventArgs>(task_TaskStarted);
			task.FilesEnumerated += new System.EventHandler<FilesEnumeratedArgs>(task_FilesEnumerated);
			task.FileIndexed += new EventHandler<FileImportEventArgs>(task_FileIndexed);
			task.FileSkipped += new EventHandler<FileImportEventArgs>(task_FileSkipped);
			task.FileImported += new System.EventHandler<FileImportEventArgs>(task_FileImported);
			task.FileImportFailed += new System.EventHandler<FileImportEventArgs>(task_FileImportFailed);
			task.ImportDone += new System.EventHandler<ImportDoneEventArgs>(task_ImportDone);

			var taskStatus = new ImportTaskStaus { Id = task.TaskId, UserId = user.user_id, Sources = task.Paths.ToList(), Time = DateTime.Now };
			TaskStatusCollection.Instance.Save(taskStatus);

			TaskQueue.Enqueue(task, TaskPriority.VeryLow);

			RespondSuccess(new ImportResponse { task_id = task.TaskId.ToString() });
		}

		void task_TaskStarted(object sender, TaskStartedEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Set("IsStarted", true));
		}

		void task_FilesEnumerated(object sender, FilesEnumeratedArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Set("Total", e.TotalCount));
		}

		void task_FileIndexed(object sender, FileImportEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Inc("Indexed", 1));
		}

		void task_FileSkipped(object sender, FileImportEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Inc("Skipped", 1));
		}

		void task_FileImported(object sender, FileImportEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Inc("Copied", 1));
		}

		void task_FileImportFailed(object sender, FileImportEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Push("CopyFailed", e.file.ToBsonDocument()));
		}

		void task_ImportDone(object sender, ImportDoneEventArgs e)
		{
			var update = Update.Set("IsCopyComplete", true);

			if (e.Error != null)
				update = update.Set("Error", e.Error.Message);

			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), update);
		}

		

		

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}
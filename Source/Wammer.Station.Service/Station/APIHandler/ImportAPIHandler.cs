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

		void task_FileImportFailed(object sender, FileImportEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Push("FailedFiles", e.file.ToBsonDocument()));
		}

		void task_ImportDone(object sender, ImportDoneEventArgs e)
		{
			var update = Update.Set("IsComplete", true);

			if (e.SkippedFiles != null)
				update.Set("SkippedFiles", new BsonArray(e.SkippedFiles));

			if (e.Error != null)
				update.Set("Error", e.Error.Message);


			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), update);
		}

		void task_FileImported(object sender, FileImportEventArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Inc("SuccessCount", 1));
		}

		void task_FilesEnumerated(object sender, FilesEnumeratedArgs e)
		{
			TaskStatusCollection.Instance.Update(Query.EQ("_id", e.TaskId), Update.Set("TotalFiles", e.TotalCount));
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
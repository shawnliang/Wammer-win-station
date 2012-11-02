using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using MongoDB.Driver.Builders;
using WebSocketSharp;
using Wammer.Station.Notify;
using Wammer.Utility;
using Wammer.Station;

namespace StationSystemTray
{
	class PersonalCloudStatusService : IPersonalCloudStatusService
	{
		public IEnumerable<StreamDevice> GetDeviceList(string user_id, string apikey, string session_token)
		{
			var session = Wammer.Cloud.User.GetLoginInfo(user_id, apikey, session_token);

			return session.user.devices.Select((x) =>
			{
				var connection = ConnectionCollection.Instance.FindOne(Query.EQ("device.device_id", x.device_id));

				bool isConnected = (x.device_id == StationRegistry.GetValue("stationId", "") as string) ? true : connection != null;

				return new StreamDevice(x.device_name, isConnected, x.device_type);
			});
		}

		public PersonalCloudStatus GetStatus(string user_id)
		{
			var user = DriverCollection.Instance.FindOneById(user_id);
			var group_id = user.groups[0].group_id;

			var photos = AttachmentCollection.Instance.Find(Query.EQ("group_id", group_id));
			var events = PostCollection.Instance.Find(
				Query.And(
					Query.EQ("group_id", group_id),
					Query.EQ("hidden", "false"), 
					Query.EQ("import", false))
			);

			return new PersonalCloudStatus((int)photos.Count(), (int)events.Count());
		}
	}
}

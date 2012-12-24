using AutoMapper;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
	public class Post
	{
		#region Public Static Method
		public static void Save(PostInfo postInfo)
		{
			if (!postInfo.code_name.Equals("StreamEvent", StringComparison.CurrentCultureIgnoreCase))
				return;

			if (string.IsNullOrEmpty(postInfo.post_id))
				return;

			var post = Mapper.Map<PostInfo, PostDBData>(postInfo);

			var friendInfos = postInfo.people;

			if (friendInfos != null)
			{
				var friendIDs = new List<String>(friendInfos.Count);

				foreach (var friendInfo in friendInfos)
				{
					var existedFriend = FriendDBDataCollection.Instance.FindOne(Query.And(Query.EQ("name", friendInfo.name), Query.EQ("avatar", friendInfo.avatar)));
					var personID = (existedFriend == null) ? Guid.NewGuid().ToString() : existedFriend.ID;

					if (existedFriend == null)
					{
						var person = Mapper.Map<FriendInfo, FriendDBData>(friendInfo);
						person.ID = personID;

						FriendDBDataCollection.Instance.Save(person);
					}

					friendIDs.Add(personID);
				}

				post.FriendIDs = friendIDs;
			}
						

			var postGPS = postInfo.gps;

			if (postGPS != null)
			{
				//Debug.Assert(postGPS.latitude != null);
				//Debug.Assert(postGPS.longitude != null);

				if (postGPS.latitude != null && postGPS.longitude != null)
				{
					var existedLocation = LocationDBDataCollection.Instance.FindOne(Query.And(Query.EQ("latitude", postGPS.latitude), Query.EQ("longitude", postGPS.longitude)));
					var locationID = (existedLocation == null) ? Guid.NewGuid().ToString() : existedLocation.ID;

					var location = Mapper.Map<PostGps, LocationDBData>(postGPS);
					location.ID = locationID;

					LocationDBDataCollection.Instance.Save(location);
				}
			}


			var checkIns = postInfo.checkins;
			
			if(checkIns != null)
			{
				var checkInLocations = new List<String>(checkIns.Count);

				foreach(var checkIn in checkIns)
				{
					//Debug.Assert(checkIn.latitude != null);
					//Debug.Assert(checkIn.longitude != null);

					if (checkIn.latitude != null && checkIn.longitude != null)
					{
						var existedLocation = LocationDBDataCollection.Instance.FindOne(Query.And(Query.EQ("latitude", checkIn.latitude), Query.EQ("longitude", checkIn.longitude)));
						var locationID = (existedLocation == null) ? Guid.NewGuid().ToString() : existedLocation.ID;

						if (existedLocation == null)
						{
							var location = Mapper.Map<PostCheckIn, LocationDBData>(checkIn);
							location.ID = locationID;

							LocationDBDataCollection.Instance.Save(location);
						}

						if (checkIn.latitude == postGPS.latitude && checkIn.longitude == postGPS.longitude)
							checkInLocations.Insert(0, locationID);
						else
							checkInLocations.Add(locationID);
					}
				}

				post.CheckinIDs = checkInLocations;
			}

			PostDBDataCollection.Instance.Update(post);


			//if(post.FriendIDs == null || !post.FriendIDs.Any())
			//	return;

			//var user = DriverCollection.Instance.FindOneById(postInfo.creator_id);

			//Debug.Assert(user != null);
			//if (user == null)
			//	return;

			//var existedFriendIDs = (user.FriendIDs == null ? new HashSet<String>() : new HashSet<String>(user.FriendIDs));

			//user.FriendIDs = existedFriendIDs.Union(new HashSet<String>(post.FriendIDs));

			//DriverCollection.Instance.Save(user);
		} 
		#endregion
	}
}

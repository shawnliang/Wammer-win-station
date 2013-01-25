using AutoMapper;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waveface.Stream.Model
{
	public class Post
	{
		#region Public Static Method
		public static void Save(PostInfo postInfo)
		{
			if (!postInfo.type.Equals("event"))
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

			var userID = post.CreatorID;

			var postGPS = postInfo.gps;

			if (postGPS != null)
			{
				//Debug.Assert(postGPS.latitude != null);
				//Debug.Assert(postGPS.longitude != null);

				if (postGPS.latitude != null && postGPS.longitude != null)
				{
					var latitude = postGPS.latitude.Value;
					var longitude = postGPS.longitude.Value;

					var existedLocation = LocationDBDataCollection.Instance.FindOne(Query.And(Query.EQ("creator_id", userID), Query.EQ("latitude", latitude), Query.EQ("longitude", longitude)));
					var locationID = (existedLocation == null) ? Guid.NewGuid().ToString() : existedLocation.ID;

					var location = Mapper.Map<PostGps, LocationDBData>(postGPS);
					location.ID = locationID;
					location.CreatorID = userID;

					LocationDBDataCollection.Instance.Save(location);

					post.LocationID = locationID;
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
						var latitude = checkIn.latitude.Value;
						var longitude = checkIn.longitude.Value;

						var existedLocation = LocationDBDataCollection.Instance.FindOne(Query.And(Query.EQ("creator_id", userID), Query.EQ("latitude", latitude), Query.EQ("longitude", longitude)));
						var locationID = (existedLocation == null) ? Guid.NewGuid().ToString() : existedLocation.ID;

						if (existedLocation == null)
						{
							var location = Mapper.Map<PostCheckIn, LocationDBData>(checkIn);
							location.ID = locationID;
							location.CreatorID = userID;

							LocationDBDataCollection.Instance.Save(location);
						}

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

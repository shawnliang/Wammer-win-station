
namespace Wammer.Station
{
	public enum StationApiError
	{
		Error = -1,
		NotFound = -10,
		DriverExist = -30,
		BadPath = -31,
		AuthFailed = -32,
		AlreadyHasStaion = -33,
		ConnectToCloudError = -34,
		ServerOffline = -35,
		InvalidDriver = -36,
		NotReady = -37,
		InvalidGroup = -38
	}

	public enum DropboxApiError
	{
		Base = -200,
		DropboxNotInstalled = Base + 1,
		DropboxNotConnected = Base + 2,
		NoSyncFolder = Base + 3,
		GetOAuthFailed = Base + 4,
		ConnectDropboxFailed = Base + 5,
		LinkWrongAccount = Base + 6
	}

	public enum PostApiError
	{
		Base = 0x3000,
		PostNotExist = Base + 1,
		InvalidPreview = Base + 2,
		PermissionDenied = Base + 3,
		CreateEmptyPost = Base + 4,
		InvalidComponentOption = Base + 5,
		InvalidPostIdList = Base + 6,
		InvalidParameterLimit = Base + 7,
		PostNotFound = Base + 8,
		InvalidAttachmentIdArray = Base + 9,
		NothingToUpdate = Base + 10,
		InvalidFavorite = Base + 11,
		InvalidCoverAttach = Base + 12,
		NeedMerge = Base + 13
	}
}

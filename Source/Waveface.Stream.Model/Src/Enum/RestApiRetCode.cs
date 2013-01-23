namespace Waveface.Stream.Model
{
	public enum StationLocalApiError
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
		InvalidGroup = -38,
		AccessDenied = -39,
		InvalidImage = -40,
		ImageTooLarge = -41
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


	public enum AuthApiError
	{
		Base = 0x1000,
		InvalidEmailPassword = Base + 1,
		EmailAlreadyRegisterd = Base + 2,
		NoStationInstalled = Base + 3,
		InvalidSNSParameters = Base + 4
	}

	public enum UserApiError
	{
		Base = 0x2000,
		UserNotExist = Base + 1,
		InvalidPassword = Base + 2,
		PermissionDenied = Base + 3,
		AlreadyConnectToOtherSNSAccount = Base + 4
	}

	public enum UserTrackApiError
	{
		Base = 0x0000B000,
		PermissionDenied = Base + 1,
		InvalidParamIncludeEntities = Base + 2,
		InvalidParamSinceSeqNum = Base + 3,
		SeqNumPurged = Base + 4,
		NoData = Base + 5,
		InvalidParamSince = Base + 6,
		TooManyRecord = Base + 7
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
		NeedMerge = Base + 13,
		InvalidTimeStamp = Base + 14,
		InvalidLastUpdateTime = Base + 15,
		InvalidUpdateTime = Base + 16,
		InvalidPostId = Base + 17
	}

	public enum StationApiError
	{
		Base = 0x4000,
		StationNotExist = Base + 1,
		StationAlreadyRegisterd = Base + 2,
		AlreadyRegisteredAnotherStation = Base + 3,
		UserNotExist = Base + 4,
		StationOfflineOrUnknown = Base + 5
	}

	public enum AttachmentApiError
	{
		Base = 0x6000,
		AttachmentNotExist = Base + 1,
		NotSupportedType = Base + 2,
		NotSupportedDocument = Base + 3,
		ErrorOnS3Upload = Base + 4,
		ErrorOnS3GetFile = Base + 5,
		ErrorOnConvertOfficeFile = Base + 6,
		InvalidValue = Base + 7,
		InternalError = Base + 8,
		InvalidLocation = Base + 9,
		InvalidImage = Base + 10,
		OverQuota = Base + 11,
		PermissionDenied = Base + 12,
		InvalidMetaType = Base + 13,
		FileExisted = Base + 14,
		InvalidObjectId = Base + 15,
		InvalidPostId = Base + 16
	}
}
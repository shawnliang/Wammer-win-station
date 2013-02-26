using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using Wammer.Station;

namespace UT_WammerStation.MoveFolder
{
	[TestClass]
	public class TestMoveFolder
	{
		[TestMethod]
		public void testUseMoveIfOnSameDrive()
		{
			var src = @"c:\users\shawn\AOStream";
			var dest = @"c:\users\shawn\AOStream2";

			var util = new Mock<IFolderUtility>();

			util.Setup(u => u.IsOnSameDrive(src, dest)).Returns(true).Verifiable();
			util.Setup(u => u.MoveOnSameDrive(src, dest)).Verifiable();
			var mv = new FolderMover(util.Object);

			mv.MoveFolder(src, dest);

			util.VerifyAll();
		}

		[TestMethod]
		public void testCopyIfOnDiffDrive()
		{
			var src = @"c:\users\shawn\AOStream";
			var dest = @"c:\users\shawn\AOStream2";

			var util = new Mock<IFolderUtility>();

			util.Setup(u => u.IsOnSameDrive(src, dest)).Returns(false).Verifiable();
			util.Setup(u => u.RecursiveCopy(src, dest)).Verifiable();
			util.Setup(u => u.RecursiveDelete(src)).Verifiable();


			var mv = new FolderMover(util.Object);
			mv.MoveFolder(src, dest);

			util.VerifyAll();
		}

		[TestMethod]
		public void deletingSrcFailureIsOK()
		{
			var src = @"c:\users\shawn\AOStream";
			var dest = @"c:\users\shawn\AOStream2";

			var util = new Mock<IFolderUtility>();

			util.Setup(u => u.IsOnSameDrive(src, dest)).Returns(false).Verifiable();
			util.Setup(u => u.RecursiveCopy(src, dest)).Verifiable();
			util.Setup(u => u.RecursiveDelete(src)).Throws(new IOException("error")).Verifiable();


			var mv = new FolderMover(util.Object);
			mv.MoveFolder(src, dest);

			util.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof(DestinationExistException))]
		public void testSubDirExist()
		{
			var src = @"c:\users\shawn\AOStream";
			var dest = @"c:\users\shawn\AOStream2";

			var util = new Mock<IFolderUtility>();

			util.Setup(u => u.GetSubDirectories(src)).Returns(new string[] { src + @"\a", src + @"\b" }).Verifiable();
			util.Setup(u => u.GetSubEntries(dest)).Returns(new string[] { src + @"\c", src + @"\b" }).Verifiable();

			var mv = new FolderMover(util.Object);
			mv.MoveFolder(src, dest);
		}
	}
}
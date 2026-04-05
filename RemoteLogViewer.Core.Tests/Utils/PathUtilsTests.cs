using RemoteLogViewer.Core.Services.Ssh;
using RemoteLogViewer.Core.Utils;
using Shouldly;

namespace RemoteLogViewer.Core.Tests.Utils;

public class PathUtilsTests {
	[Theory]
	[InlineData("/home/user", "file.txt", FileSystemObjectType.File, "/home/user/file.txt")]
	[InlineData("/home/user/", "file.txt", FileSystemObjectType.File, "/home/user/file.txt")]
	[InlineData("/home/user", "subdir", FileSystemObjectType.Directory, "/home/user/subdir/")]
	[InlineData("/home/user/", "subdir", FileSystemObjectType.Directory, "/home/user/subdir/")]
	[InlineData("/home/user", "/etc/config", FileSystemObjectType.File, "/etc/config")]
	[InlineData("/home/user", "link", FileSystemObjectType.SymlinkFile, "/home/user/link")]
	[InlineData("/home/user", "linkdir", FileSystemObjectType.SymlinkDirectory, "/home/user/linkdir/")]
	[InlineData("/home/user", "subdir/", FileSystemObjectType.Directory, "/home/user/subdir//")]
	public void CombineUnixPath_ShouldCombinePathsCorrectly(string path1, string path2, FileSystemObjectType fsoType, string expected) {
		// Act
		var result = PathUtils.CombineUnixPath(path1, path2, fsoType);

		// Assert
		result.ShouldBe(expected);
	}

	[Theory]
	[InlineData("/home/user/file.txt", "file.txt")]
	[InlineData("/home/user/subdir/", "subdir")]
	[InlineData("/home/user/subdir", "subdir")]
	[InlineData("file.txt", "file.txt")]
	[InlineData("/", "")]
	[InlineData("", "")]
	public void GetFileOrDirectoryName_ShouldReturnCorrectName(string path, string expected) {
		// Act
		var result = PathUtils.GetFileOrDirectoryName(path);

		// Assert
		result.ShouldBe(expected);
	}
}
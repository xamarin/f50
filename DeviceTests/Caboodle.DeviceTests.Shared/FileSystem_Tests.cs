﻿using System.IO;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Caboodle;

namespace Caboodle.DeviceTests
{
	public class FileSystem_Tests
	{
		private const string BundleFileContents = "This file was in the app bundle.";

		[Fact]
		public void CacheDirectory_Is_Valid()
		{
			Assert.False(string.IsNullOrWhiteSpace(FileSystem.CacheDirectory));
		}

		[Fact]
		public void AppDataDirectory_Is_Valid()
		{
			Assert.False(string.IsNullOrWhiteSpace(FileSystem.AppDataDirectory));
		}

		[Fact]
		public void UserDataDirectory_Is_Valid()
		{
			Assert.False(string.IsNullOrWhiteSpace(FileSystem.UserDataDirectory));
		}

		[Theory]
		[InlineData("AppBundleFile.txt", BundleFileContents)]
		[InlineData("AppBundleFile_NoExtension", BundleFileContents)]
		[InlineData("Folder/AppBundleFile_Nested.txt", BundleFileContents)]
		[InlineData("Folder\\AppBundleFile_Nested.txt", BundleFileContents)]
		public async Task OpenAppBundleFileAsync_Can_Load_File(string filename, string contents)
		{
			using (var stream = await FileSystem.OpenAppBundleFileAsync(filename))
			{
				Assert.NotNull(stream);

				using (var reader = new StreamReader(stream))
				{
					var text = await reader.ReadToEndAsync();

					Assert.Equal(contents, text);
				}
			}
		}

		[Fact]
		public async Task OpenAppBundleFileAsync_Throws_If_File_Is_Not_Found()
		{
			await Assert.ThrowsAsync<FileNotFoundException>(() => FileSystem.OpenAppBundleFileAsync("MissingFile.txt"));
		}
	}
}

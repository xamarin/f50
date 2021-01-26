﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Samples.ViewModel
{
    public class SaveToGalleryViewModel : BaseViewModel
    {
        const string jpgFileName = "Lomonosov.jpg";
        const string albumName = "Essentials";

        public SaveToGalleryViewModel()
        {
            SavevPngCommand = new Command(() => Save(PngUrl, "essential.png"));
            SaveJpgCommand = new Command(() => Save(JpgUrl, jpgFileName));
            SaveGifCommand = new Command(() => Save(GifUrl, "test.gif"));
            SaveVideoCommand = new Command(() => Save(VideoUrl, "essential.mov"));

            SaveFromCacheCommand = new Command(SaveFromCacheDirectory);
        }

        public ICommand SavevPngCommand { get; }

        public ICommand SaveJpgCommand { get; }

        public ICommand SaveGifCommand { get; }

        public ICommand SaveVideoCommand { get; }

        public ICommand SaveFromCacheCommand { get; }

        public string PngUrl
            => "https://raw.githubusercontent.com/xamarin/Essentials/main/Assets/xamarin.essentials_128x128.png";

        public string JpgUrl
            => "https://raw.githubusercontent.com/dimonovdd/Essentials/featureSaveToGalery/Assets/SaveToGaleryTestPhoto.jpg";

        public string GifUrl
            => "https://i.gifer.com/769R.gif";

        public string VideoUrl
            => "https://xvid.ru/play/tests/qt7.mov";

        async void Save(string url, string name)
        {
            try
            {
                var data = await DownloadFile(url);
                await SaveToGallery.SaveAsync(
                    url == VideoUrl ? MediaFileType.Video : MediaFileType.Image,
                    data,
                    name,
                    albumName);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(ex.Message);
            }
        }

        async void SaveFromCacheDirectory()
        {
            try
            {
                var filePath = SaveFileToCache(await DownloadFile(JpgUrl), jpgFileName);
                await SaveToGallery.SaveAsync(MediaFileType.Image, filePath, albumName);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync(ex.Message);
            }
        }

        string SaveFileToCache(byte[] data, string fileName)
        {
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            if (!File.Exists(filePath))
                File.WriteAllBytes(filePath, data);

            return filePath;
        }

        async Task<byte[]> DownloadFile(string url)
        {
            using var client = new WebClient();
            return await client.DownloadDataTaskAsync(url);
        }
    }
}

﻿using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Samples.ViewModel
{
    public class FileSystemViewModel : BaseViewModel
    {
        private const string templateFileName = "FileSystemTemplate.txt";
        private const string localFileName = "TheFile.txt";

        private static string localPath = Path.Combine(FileSystem.AppDataDirectory, localFileName);

        private string currentContents;

        public FileSystemViewModel()
        {
            LoadFileCommand = new Command(() => DoLoadFile());
            SaveFileCommand = new Command(() => DoSaveFile());
            DeleteFileCommand = new Command(() => DoDeleteFile());

            DoLoadFile();
        }

        public ICommand LoadFileCommand { get; }

        public ICommand SaveFileCommand { get; }

        public ICommand DeleteFileCommand { get; }

        public string CurrentContents
        {
            get => currentContents;
            set => SetProperty(ref currentContents, value);
        }

        private async void DoLoadFile()
        {
            if (File.Exists(localPath))
            {
                CurrentContents = File.ReadAllText(localPath);
            }
            else
            {
                using (var stream = await FileSystem.OpenAppPackageFileAsync(templateFileName))
                using (var reader = new StreamReader(stream))
                {
                    CurrentContents = await reader.ReadToEndAsync();
                }
            }
        }

        private void DoSaveFile()
        {
            File.WriteAllText(localPath, CurrentContents);
        }

        private void DoDeleteFile()
        {
            if (File.Exists(localPath))
                File.Delete(localPath);
        }
    }
}

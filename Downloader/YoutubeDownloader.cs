using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    public class YoutubeDownloader : IDownloader
    {
        private Process youtubeDownloader;

        public string Download(string url, MainWindow form, string pathToSave)
        {
            string end;
            using (Process process = Process.Start(new ProcessStartInfo()
            {
                Arguments = $" -o \"{pathToSave}\\%(id)s.%(ext)s\" -f \"bestvideo[height<=?1080][ext=mp4]+bestaudio\" " + url,
                FileName = "youtube-dl.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }))
            {
                form.UpdateLog("Start PROCESS");
                using (StreamReader standardOutput = process.StandardOutput)
                {
                    end = standardOutput.ReadToEnd();
                    form.UpdateLog("Process READTOEND END");
                }
            }
            return end;
        }
    }
}

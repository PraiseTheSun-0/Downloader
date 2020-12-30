using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    public class CoubDownloader : IDownloader
    {
        private Process coubDownloader;

        public string Download(string url, MainWindow form, string pathToSave)
        {
            string end;
            using (Process process = Process.Start(new ProcessStartInfo()
            {
                Arguments = " coub.py " + url + " --path " + $"\"{pathToSave}\"" + " --connections 1 --ext mp4",
                FileName = "python.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }))
            {
                form.UpdateLog("Start PROCESS");
                //form.UpdateLog();
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

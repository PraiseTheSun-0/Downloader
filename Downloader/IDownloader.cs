﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    interface IDownloader
    {
        string Download(string url, MainWindow form, string pathToSave);
    }
}

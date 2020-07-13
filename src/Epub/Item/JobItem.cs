﻿using Book.Item;
using Book.Util;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Epub.Item
{
    public class JobItem : IJobItem, IProgress<double>
    {
        public int Id => Title.GetHashCode();
        public string Title => _file.Name;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        private readonly FileInfo _file;

        public JobItem(FileInfo file)
        {
            _file = file;
        }

        public Task<bool> Run()
        {
            try
            {
                using (ZipInputStream zipStream = new ZipInputStream(new ProgressStream(_file.OpenRead(), this)))
                {
                    zipStream.UnpackAll(_file.Directory.CreateSubdirectory(Path.GetFileNameWithoutExtension(_file.Name)));
                }
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                return Task.FromResult(false);
            }
        }

        public void Report(double progress)
        {
            ProgressChanged(this, new ProgressEventArgs(progress));
        }
    }
}

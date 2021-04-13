using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OsxPhotos;

namespace SofiBot
{
    public class OsxPhotoService
    {
        private IServiceProvider services;

        public PhotoCollection PhotoCollection { get; private set; }

        public OsxPhotoService(IServiceProvider services)
        {
            this.services = services;
            Reload();
        }

        public void Reload()
        {
            var photoJson = services.GetRequiredService<ShellCommand>()
                .Run($"./venv/bin/python -m osxphotos query --album \"Sofi\" --shared --json ");

            var timer = Stopwatch.StartNew();
            Console.WriteLine("[OsxPhotoService] Reloading");
            PhotoCollection = PhotoCollection.Deserialize(photoJson);
            Console.WriteLine($"[OsxPhotoService] Reloaded in: {timer.Elapsed}");
        }
    }
}
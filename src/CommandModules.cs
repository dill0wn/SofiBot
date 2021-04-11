using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using OsxPhotos;

namespace SofiBot.Commands
{
    [Group("sofi")]
    public class GetModule : ModuleBase<CommandContext>
    {
        public IServiceProvider services { get; set; }

        [Command("file")]
        [Summary("Posts a File.")]
        public async Task GetFileAsync()
        {
            string fileName = "IMG_1586-90.jpeg";
            string filePath = $"/tmp/sofi/{fileName}";
            using (var stream = File.OpenRead(filePath))
            {
                // upload/embed image https://stackoverflow.com/a/51010769

                var embed = new EmbedBuilder()
                       .WithFields(
                           new EmbedFieldBuilder { Name = "Option 1️⃣", Value = "Bar1", IsInline = true },
                           new EmbedFieldBuilder { Name = "Option 2️⃣", Value = "Bar2", IsInline = true },
                           new EmbedFieldBuilder { Name = "Option 3️⃣", Value = "Bar3", IsInline = false }
                       )
                       .WithTitle("Got Meme")
                       .WithDescription("This is a long message with some rich text.")
                       .WithColor(0x0000ff)
                       .WithImageUrl($"attachment://{fileName}");

                // send message
                // FIXME: catch > 8MB error and send error message
                // FIXME: .HEIC files seem to trigger silent error, libheif and 
                var message = await Context.Channel.SendFileAsync(stream, fileName, embed: embed.Build());
                Console.WriteLine($"Embeded image to {message.Channel.Id}, {message.Author.Id}, {message.GetJumpUrl()}");
            }
        }

        [Command("random", RunMode = RunMode.Async)]
        [Summary("Posts a Photo of Sofi.")]
        public async Task GetRandomSofiAsync()
        {
            Stopwatch overall = Stopwatch.StartNew();
            Stopwatch timer = new Stopwatch();
            timer.Start();

            Console.WriteLine("[Stopwatch] Starting 'query'");
            var photoJson = services.GetRequiredService<ShellCommand>()
                .Run("./venv/bin/python -m osxphotos query --album \"Sofi\" --shared --json");
            Console.WriteLine($"[Stopwatch] 'query' took: {timer.Elapsed}");

            timer.Restart();
            Console.WriteLine("[Stopwatch] Starting 'deserialize'");
            var photoCollection = PhotoCollection.Deserialize(photoJson);
            Console.WriteLine($"[Stopwatch] 'deserialize' took: {timer.Elapsed}");

            var photo = photoCollection.Photos[0];

            Console.WriteLine($"parsed photos, first photo: {photo.path}");

            string exportPath = "exported-photos";
            Directory.CreateDirectory(exportPath);
            timer.Restart();
            Console.WriteLine("[Stopwatch] Starting 'export'");
            var exportOpts = new string[]{
                $"--update",
                $"--uuid {photo.uuid}",
                // "--convert-to-jpeg --jpeg-quality 0.9 --jpeg-ext jpg",
                "--download-missing",
                "--verbose",
                exportPath
            };
            var photoExport = services.GetRequiredService<ShellCommand>()
            .Run($"./venv/bin/python -m osxphotos export {string.Join(" ", exportOpts)}");
            Console.WriteLine($"[Stopwatch] 'export' took: {timer.Elapsed}");

            string fileName = photo.ExportedFilename;
            var exportedFile = Path.GetFullPath($"{exportPath}/{photo.ExportedFilename}");

            if (!File.Exists(exportedFile))
            {
                throw new FileNotFoundException(exportedFile);
            }

            using (var stream = File.OpenRead(exportedFile))
            {
                // upload/embed image https://stackoverflow.com/a/51010769

                var embed = new EmbedBuilder()
                       .WithTitle("It's Sofi!")
                       .WithDescription("This is a photo of sofi")
                       .WithColor(0x0000ff)
                       .WithImageUrl($"attachment://{fileName}");

                timer.Restart();
                Console.WriteLine("[Stopwatch] Starting 'upload'");
                // send message
                // FIXME: catch > 8MB error and send error message
                // FIXME: .HEIC files seem to trigger silent error, libheif and 
                var message = await Context.Channel.SendFileAsync(stream, fileName, embed: embed.Build(), messageReference: Context.Message.Reference);
                Console.WriteLine($"[Stopwatch] 'upload' took: {timer.Elapsed}");

                Console.WriteLine($"Embedded image to {message.Channel.Id}, {message.Author.Id}, {message.GetJumpUrl()}");
            }
            Console.WriteLine($"[Stopwatch] 'overall' took: {overall.Elapsed}");

            await Task.CompletedTask;
        }
    }
}
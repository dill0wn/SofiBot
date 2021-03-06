using System;
using System.Collections.Generic;
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

        [Command("refresh", RunMode = RunMode.Async)]
        [Summary("Reloads Photo list")]
        public async Task RefreshDatabase()
        {
            var message = await Context.Message.ReplyAsync("Refreshing...");
            var photos = services.GetRequiredService<OsxPhotoService>();
            photos.Reload();
            await message.ReplyAsync($"Refreshed. There are now {photos.PhotoCollection.Photos.Count} photos.");
        }

        [Command("photo", RunMode = RunMode.Async)]
        [Summary("Posts a Photo of Sofi.")]
        public async Task GetRandomSofiAsync(params string[] commentKeywords)
        {
            var placeholder =  Context.Message.ReplyAsync("Picking a photo... This may take a moment.");
            var photoService = services.GetRequiredService<OsxPhotoService>();
            await placeholder;
            string matchingKeyword = null;
            List<Photo> photos = photoService.PhotoCollection.Photos.Where(photo => photo.isphoto).ToList();
            if (commentKeywords.Length > 0)
            {
                photos = photoService.PhotoCollection.Photos.Where(photo =>
                {
                    return photo.comments.Any(comment =>
                    {
                        return commentKeywords.Any(keyword =>
                        {
                            if (comment.text.Contains(keyword))
                            {
                                matchingKeyword = keyword;
                                return true;
                            };
                            return false;
                        });
                    });
                }).ToList();
            }

            Console.WriteLine($"Found {photos.Count} photos that matched keywords {commentKeywords}");

            var length = photos.Count;

            if (length < 1)
            {
                var message = await Context.Message.ReplyAsync("Photo list is like super empty, bro. Try running 'refresh'");
                return;
            }

            var random = new Random();
            var i = random.Next(length);
            var photo = photos[i];

            Console.WriteLine($"Picked random photo {i} out of {length} photos: {photo.path}");

            string exportPath = "exported-photos";
            Directory.CreateDirectory(exportPath);

            Console.WriteLine("[Stopwatch] Starting 'export'");
            var exportOpts = new string[]{
                $"--update",
                $"--uuid {photo.uuid}",
                "--convert-to-jpeg --jpeg-quality 0.9 --jpeg-ext jpg",
                "--download-missing",
                "--verbose",
                exportPath
            };
            var photoExport = services.GetRequiredService<ShellCommand>()
            .Run($"./venv/bin/python -m osxphotos export {string.Join(" ", exportOpts)}");

            string fileName = photo.ExportedFilename;
            var exportedFile = Path.GetFullPath($"{exportPath}/{photo.ExportedFilename}");

            if (!File.Exists(exportedFile))
            {
                throw new FileNotFoundException(exportedFile);
            }
            Console.WriteLine($"Exported file {exportedFile}, sending...");

            using (var stream = File.OpenRead(exportedFile))
            {
                var embedFields = new List<EmbedFieldBuilder>();
                if (commentKeywords.Length > 0)
                {
                    embedFields.Add(new EmbedFieldBuilder { Name = "Keyword(s)", Value = string.Join(", ", commentKeywords) });
                    embedFields.Add(new EmbedFieldBuilder { Name = "Photos Matching Keyword(s)", Value = photos.Count().ToString() });
                }
                else
                {

                }
                // embedFields.Add(new EmbedFieldBuilder { Name = "Filename", Value = photo.original_filename });
                // embedFields.Add(new EmbedFieldBuilder { Name = "Comments", Value = StringifyComments(photo.comments) });
                var embed = new EmbedBuilder()
                       .WithTitle("Hey look! It's Sofi!")
                    //    .WithDescription("This is a photo of sofi")
                       .WithFields(embedFields)
                       .WithColor(0x0000ff)
                       .WithImageUrl($"attachment://{fileName}")
                       ;

                var message = await Context.Channel.SendFileAsync(stream, fileName, embed: embed.Build(), messageReference: Context.Message.Reference);
                Console.WriteLine($"Embedded image to {message.Channel.Id}, {message.Author.Id}, {message.GetJumpUrl()}");
            }

            await Task.CompletedTask;
        }

        private string StringifyComments(IEnumerable<PhotoComment> comments)
        {
            return "TBA";
        }
    }
}
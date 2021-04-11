using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SofiBot.Commands
{
    [Group("get")]
    public class GetModule : ModuleBase<CommandContext>
    {
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
    }
}
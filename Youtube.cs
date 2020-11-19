// ReSharper disable RedundantUsingDirective

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable HeapView.BoxingAllocation

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable HeapView.ObjectAllocation.Evident

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace GamerBot
{
    [Group("youtube")]
    public class Youtube : BaseCommandModule
    {
        [Command("download"), Description("Downloads a youtube video and sends the file back. (MP3 and MP4)")]
        public async Task Download(CommandContext ctx, [Description("Link to the video you want to download.")]
            string link = null, [Description("Choose what type of file you want to download. Will default to mp4.")]
            string type = "mp4")
        {
            if (link == null)
            {
                await ctx.RespondAsync("Please specify a link.");
            }
            else if (!link.StartsWith("https://youtube.com"))
            {
                await ctx.RespondAsync("Please specify a valid link.");
            }
            else
            {
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(link);

                if (!File.Exists(@".\videos\"))
                {
                    Directory.CreateDirectory(@".\videos\");
                }

                Random random = new Random();
                var name = @$".\videos\{random.Next(1, 232123)}";
                
                await ctx.RespondAsync($"Okay, downloading `{video.Title}`...");

                switch (type.ToLower())
                {
                    case "mp4":
                    {
                        if (true)
                        {
                            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link.Split("=").Last());

                            var streamInfo = streamManifest
                                .GetMuxed()
                                .Where(s => s.Container == Container.Mp4)
                                .WithHighestVideoQuality();

                            if (streamInfo != null)
                            {
                                // Download the stream to file
                                await youtube.Videos.Streams.DownloadAsync(streamInfo,
                                    $"{name}.{streamInfo.Container}");

                                await ctx.RespondAsync($"Here is your download of `{video.Title}`:");
                                await ctx.RespondWithFileAsync($"{name}.{streamInfo.Container}");
                            }
                        }

                        break;
                    }
                    case "webm":
                    {
                        if (true)
                        {
                            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link.Split("=").Last());

                            var streamInfo = streamManifest
                                .GetAudio()
                                .Where(s => s.Container == Container.WebM)
                                .WithHighestBitrate();

                            if (streamInfo != null)
                            {
                                // Download the stream to file
                                await youtube.Videos.Streams.DownloadAsync(streamInfo,
                                    $"{name}.{streamInfo.Container}");

                                await ctx.RespondAsync($"Here is your download of `{video.Title}`:");
                                await ctx.RespondWithFileAsync($"{name}.{streamInfo.Container}");
                            }
                        }

                        break;
                    }
                }
            }
        }
    }
}

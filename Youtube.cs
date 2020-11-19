// ReSharper disable RedundantUsingDirective
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace GamerBot
{
    [Group("youtube")]
    internal class Youtube : BaseCommandModule
    {
        [Command("download"), Description("Downloads a youtube video and sends the file back. (MP3 and MP4)")]
        public async Task Download(CommandContext ctx, [Description("Link to the video you want to download.")] string link = null, [Description("Choose what type of file you want to download.")] string type = "mp4")
        {
            await ctx.RespondAsync("");
        }
    }
}

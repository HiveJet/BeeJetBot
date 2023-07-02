using Discord;
using Discord.Commands;
using System.Text;

namespace BeeJet.Bot.Extensions
{
    internal static class CommandExtensions
    {
        internal static string ToFriendlyString(this CommandInfo command)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Command !{command.Name} - {command.Summary}");
            foreach (var parameter in command.Parameters)
                sb.AppendLine($"Parameter: {parameter.Name} {parameter.Type} {(parameter.IsOptional ? " (Optional)" : "")}{(parameter.IsRemainder ? " (Remainder)" : "")}");

            return sb.ToString();
        }

        internal static Task RespondEphemeralAsync(this ISlashCommandInteraction interaction, string text = null, Embed[] embeds = null, bool isTTS = false, AllowedMentions allowedMentions = null, MessageComponent components = null, Embed embed = null, RequestOptions options = null)
        {
            return interaction.RespondAsync(text, embeds, isTTS, true, allowedMentions, components, embed, options);
        }
    }
}

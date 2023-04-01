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
    }
}

using ImproveGame.Common.ModSystems;

namespace ImproveGame.Common.Commands
{
    public class ConfigPasswordCommand : ModCommand
    {
        public override CommandType Type => CommandType.Server | CommandType.Console;
        public override string Usage => "/qolpassword <password>";
        public override string Command => "qolpassword";
        public override string Description => GetText("Config.OnlyHostByPassword.CommandDescription");

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (!Config.OnlyHostByPassword)
            {
                caller.Reply(GetText("Config.OnlyHostByPassword.NotOn"), new(206, 212, 106));
                return;
            }

            if (caller.CommandType == CommandType.Console)
            {
                caller.Reply(GetTextWith("Config.OnlyHostByPassword.ServerPasswordLog", new { Password = NetPasswordSystem.ConfigPassword}));
                return;
            }

            if (args.Length != 1 || args[0].Length != 4) {
                caller.Reply("Usage: /qolpassword <password>", new(240, 40, 40));
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (char.IsLetter(args[0][i]))
                    continue;
                caller.Reply(GetText("Config.OnlyHostByPassword.Format"), new(240, 40, 40));
                return;
            }

            string password = args[0];
            if (password.ToUpper() == NetPasswordSystem.ConfigPassword) {
                caller.Reply(GetText("Config.OnlyHostByPassword.Correct"), new(40, 240, 40));
                NetPasswordSystem.Registered[caller.Player.whoAmI] = true;
            }
            else {
                caller.Reply(GetText("Config.OnlyHostByPassword.Incorrect"), new(240, 40, 40));
            }
        }
    }
}

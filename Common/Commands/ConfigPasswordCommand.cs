using ImproveGame.Common.Systems;

namespace ImproveGame.Common.Commands
{
    public class ConfigPasswordCommand : ModCommand
    {
        public override CommandType Type => CommandType.Server;
        public override string Usage => "/qolpassword <password>";
        public override string Command => "qolpassword";
        public override string Description => MyUtils.GetText("Config.OnlyHostByPassword.CommandDescription");

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length != 1 || args[0].Length != 8) {
                caller.Reply("Usage: /qolpassword <password>", new(240, 40, 40));
                return;
            }

            for (int i = 0; i < 8; i++) {
                if (!char.IsNumber(args[0][i])) {
                    caller.Reply("Usage: /qolpassword <password>", new(240, 40, 40));
                    return;
                }
            }

            string password = args[0];
            if (password == NetPasswordSystem.ConfigPassword) {
                caller.Reply(MyUtils.GetText("Config.OnlyHostByPassword.Correct"), new(40, 240, 40));
                NetPasswordSystem.Registered[caller.Player.whoAmI] = true;
            }
            else {
                caller.Reply(MyUtils.GetText("Config.OnlyHostByPassword.Incorrect"), new(240, 40, 40));
            }
        }
    }
}

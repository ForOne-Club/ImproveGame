using ImproveGame.Common.GlobalPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ImproveGame.Commands
{
    public class SetPlayerName : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "playername";

        public override string Usage => "玩家名字修改成功";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            caller.Player.name = args[0];
            throw new NotImplementedException();
        }
    }

    public class IgnoreDefense : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "nodef";

        public override string Usage => "作弊模式已启用";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            if (args[0] == "1")
            {
                player.GetModPlayer<SaveAndLoadDataPlayer>().IgnoreDefense = true;
            }
            else
            {
                player.GetModPlayer<SaveAndLoadDataPlayer>().IgnoreDefense = false;
            }
            throw new NotImplementedException();
        }
    }
}

﻿using System.IO;

namespace ImproveGame.Common.Utils
{
    internal class NetHelper
    {
        public static void HandlePacket(BinaryReader reader, int sender) {
            MessageType type = (MessageType)reader.ReadByte();
            NetAutofish.HandlePacket(reader, sender, type);
            NetBigBag.HandlePacket(reader, sender, type);
            NetBuffTracker.HandlePacket(reader, sender, type);
            NetGeneric.HandlePacket(reader, sender, type);
        }
    }

    internal enum MessageType : byte
    {
        ServerReceivePlrItemUsing,
        ClientReceivePlrItemUsing,
        ReceiveRefreshTravelShop,
        Autofish_ServerReceiveItem,
        Autofish_ClientReceiveItem,
        Autofish_ServerSyncItem,
        Autofish_ClientReceiveSyncItem,
        Autofish_ServerReceiveLocatePoint,
        Autofish_ClientReceiveLocatePoint,
        Autofish_ReceiveStackChange,
        Autofish_ClientReceiveTipChange,
        Autofish_ServerReceiveAutofisherPosition,
        Autofish_ClientReceiveOpenRequest,
        Autofish_ServerReceiveOpenRequest,
        Autofish_ClientReceivePlayersToggle,
        Autofish_ServerReceiveFilterSet,
        BuffTracker_ServerReceiveSpawnRateSlider,
        BuffTracker_ClientReceiveSpawnRateSlider,
        BigBag_ReceiveSlot,
        BigBag_ReceiveAllSlot
    }
}

using ImproveGame.Common.ModPlayers;
using ImproveGame.Packets.NetAutofisher;

namespace ImproveGame.UI.Autofisher;

public partial class AutofisherGUI
{
    private void ChangeAccessorySlot(Item item, bool rightClick)
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is null)
            return;

        autofisher.accessory = item;
        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, ItemSyncPacket.Accessory)
                .Send(runLocally: false);
        }
    }

    private void ChangeFishingPoleSlot(Item item, bool rightClick)
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is null)
            return;

        autofisher.fishingPole = item;
        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, ItemSyncPacket.FishingPole)
                .Send(runLocally: false);
        }
    }

    private void ChangeBaitSlot(Item item, bool rightClick)
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is null)
            return;

        autofisher.bait = item;
        if (Main.netMode is NetmodeID.MultiplayerClient && !rightClick)
        {
            ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, ItemSyncPacket.Bait).Send(runLocally: false);
        }
    }

    private void ChangeBaitSlotStack(Item item, int stackChange, bool typeChange)
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is null)
            return;
        if (!typeChange && stackChange != 0)
        {
            ItemsStackChangePacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, ItemSyncPacket.Bait, stackChange)
                .Send(runLocally: false);
        }
    }

    private void ChangeFishSlot(Item item, int i, bool rightClick)
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is null)
            return;

        autofisher.fish[i] = item;
        if (Main.netMode == NetmodeID.MultiplayerClient && !rightClick)
        {
            ItemSyncPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, (byte)i).Send(runLocally: false);
        }
    }

    private void ChangeFishSlotStack(Item item, int i, int stackChange, bool typeChange)
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is null)
            return;
        if (Main.netMode is NetmodeID.MultiplayerClient && !typeChange && stackChange != 0)
        {
            ItemsStackChangePacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, (byte)i, stackChange)
                .Send(runLocally: false);
        }
    }

    public void RefreshItems(byte slotType = ItemSyncPacket.All)
    {
        if (Main.netMode is NetmodeID.MultiplayerClient)
        {
            if (RequireRefresh)
            {
                SyncFromTileEntity();
                RequireRefresh = false;
            }
            else
            {
                RequestItemPacket.Get(AutofishPlayer.LocalPlayer.Autofisher.ID, slotType).Send(runLocally: false);
            }
        }
        else
        {
            SyncFromTileEntity();
        }
    }

    private void SyncFromTileEntity()
    {
        var autofisher = AutofishPlayer.LocalPlayer.Autofisher;
        if (autofisher is not null)
        {
            fishingPoleSlot.Item = autofisher.fishingPole;
            baitSlot.Item = autofisher.bait;
            accessorySlot.Item = autofisher.accessory;
            for (int i = 0; i < fishSlot.Length; i++)
            {
                fishSlot[i].Item = autofisher.fish[i];
            }
        }
    }
}
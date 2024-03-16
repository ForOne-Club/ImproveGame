using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetAutofisher;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.Autofisher;

internal class CatchCratesFilter : AutofisherFilterButton
{
    internal CatchCratesFilter(SUIPanel panel) : base(ItemID.WoodenCrate, panel) { }

    internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchCrates;

    internal override void Clicked(TEAutofisher autofisher)
    {
        FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchCrates, 0).Send(runLocally: true);
    }
}

internal class CatchAccessoriesFilter : AutofisherFilterButton
{
    internal CatchAccessoriesFilter(SUIPanel panel) : base(ItemID.FrogLeg, panel) { }

    internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchAccessories;

    internal override void Clicked(TEAutofisher autofisher)
    {
        FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchAccessories, 1).Send(runLocally: true);
    }
}

internal class CatchToolsFilter : AutofisherFilterButton
{
    internal CatchToolsFilter(SUIPanel panel) : base(ItemID.CrystalSerpent, panel) { }

    internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchTools;

    internal override void Clicked(TEAutofisher autofisher)
    {
        FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchTools, 2).Send(runLocally: true);
    }
}

internal class CatchWhiteRarityCatchesFilter : AutofisherFilterButton
{
    internal CatchWhiteRarityCatchesFilter(SUIPanel panel) : base(ItemID.Bass, panel) { }

    internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchWhiteRarityCatches;

    internal override void Clicked(TEAutofisher autofisher)
    {
        FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchWhiteRarityCatches, 3).Send(runLocally: true);
    }
}

internal class CatchNormalCatchesFilter : AutofisherFilterButton
{
    internal CatchNormalCatchesFilter(SUIPanel panel) : base(ItemID.GoldenCarp, panel) { }

    internal override bool IsActivated(TEAutofisher autofisher) => autofisher.CatchNormalCatches;

    internal override void Clicked(TEAutofisher autofisher)
    {
        FishFiltersPacket.Get(autofisher.Position, !autofisher.CatchNormalCatches, 4).Send(runLocally: true);
    }
}
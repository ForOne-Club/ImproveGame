# Quality of Life, a mod to greatly improve your gameplay experience
This mod aims to raise quality of gameplay to the next level, to benefit the community

[Chinese version here | 中文版](README.md)

[Change log](ChangeLog-en.md)

# Mod.Call
## IgnoreInfItem
```"IgnoreInfItem", int/List<int> ItemID(s)```

Ignore infinite buff(s) provided by certain item(s), hence it/they no longer provide(s) infinite buff(s)
## AddPotion
```"AddPotion", int ItemID, int BuffID```

Add infinite buff(s) for a potion, especially effective to items which do not have Item.buffType or have multiple buffs. 30 potions are needed to activate the effect
## AddStation
```"AddStation", int ItemID, int BuffID```

Add infinite buff(s) for a buff station, usually you have to call manually for mod buff station. Only 1 item is needed to activate its effect
## AddPortableCraftingStation
```"AddPortableCraftingStation", int ItemID, int/List<int> TileID(s)```

Add portable crafting station support for a specified item. This is useful for items that do not have Item.createTile set but should act as some sort of crafting station. Multiple crafting stations can be specified.

If you want it to act as a water source, you should set the TileID to TileID.Sink

## Example
Here is an example of adding infinite buffs for 2 mod buff stations
```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("ImproveGame", out Mod improveGame)) {
        improveGame.Call(
            "AddStation", // Add your first buff station
            ModContent.ItemType<MyStation1>(), // ItemID 1
            ModContent.BuffType<MyStationBuff1>() // BuffID 1
        );
        improveGame.Call(
            "AddStation", // Add your second buff station
            ModContent.ItemType<MyStation2>(), // ItemID 2
            ModContent.BuffType<MyStationBuff2>() // BuffID 2
        );
    }
}
```
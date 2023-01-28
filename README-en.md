# Quality of Life, a mod to greatly improve your gameplay experience
This mod aims to raise quality of gameplay to the next level, to benefit the community

[Chinese version here | 中文版](README.md)

[Change log](ChangeLog-en.md)

# 如何编译&使用
更好的体验由于使用了BepInEx.AssemblyPublicizer.MSBuild，因此无法使用tModLoader进行编译，需要使用IDE进行编译
1. 使用IDE，如Visual Studio 2022、Rider等打开项目
2. 如果先前在tModLoader中启用了此Mod，先禁用，然后重新加载
3. 使用IDE编译项目
4. 在tModLoader中打开此Mod，重新加载
5. 成了
BepInEx.AssemblyPublicizer.MSBuild是一个很强的NuGet包，它可以将私有的程序集公开化，使得Mod可以访问私有的程序集，而不需要通过反射等手段

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
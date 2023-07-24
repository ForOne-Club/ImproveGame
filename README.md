# 更好的体验，一个让模组游玩体验大大提升的Mod
本模组致力于给模组游玩体验提一个档次，成为一个广泛使用的辅助Mod

[English version here | 英文版](README-en.md)

[更新日志](ChangeLog.md)

# 如何编译&使用
更好的体验由于使用了BepInEx.AssemblyPublicizer.MSBuild，因此无法使用tModLoader进行编译，需要使用IDE进行编译
1. 使用IDE，如Visual Studio 2022、Rider等打开项目
2. 如果先前在tModLoader中启用了此Mod，先禁用，然后重新加载
3. 使用IDE编译项目
4. 在tModLoader中打开此Mod，重新加载
5. 成了
BepInEx.AssemblyPublicizer.MSBuild是一个很强的NuGet包，它可以将私有的程序集公开化，使得Mod可以访问私有的程序集，而不需要通过反射等手段

# 版权声明
本模组的“显示物品、NPC所属模组”功能部分代码来源于模组“WMITF”，本模组与模组“WMITF”皆在MIT许可协议下开源，意味着“被授权人有权利使用、复制、修改、合并、出版发行、散布、再授权及贩售软件及软件的副本，并允许软件提供者这样做，但须满足以下条件：

上述版权声明和本许可声明应包含在本软件的所有副本或主要部分中。

因此，该模组不存在版权侵犯问题。
ChevyRay 的协程类也受 MIT 许可，与上面相同。
TextureGIF.cs的部分代码来自ProjectStarlight.Interchange，该项目也获得MIT许可，与上面相同。

附:
本模组开源链接: https://gitee.com/MyGoold/improve-game (此为原仓库，Github仓库是由原仓库镜像而来)
WMITF开源链接: https://github.com/gardenappl/WMITF
ChevyRay的协程类: https://github.com/ChevyRay/Coroutines
ProjectStarlight.Interchange: https://github.com/ProjectStarlight/ProjectStarlight.Interchange

# Mod.Call
## IgnoreInfItem
```"IgnoreInfItem", int/List<int> ItemID(s)```

为某个/些指定的物品添加无尽增益忽略，以防止在拥有30个时无限提供增益
## AddPotion
```"AddPotion", int ItemID, int BuffID```

为某个指定的物品添加无尽增益支持(药水类)，对于没有设置Item.buffType或拥有多个增益的物品十分有用，药水类指需要达到30堆叠才能提供增益
## AddStation
```"AddStation", int ItemID, int BuffID```

为某个指定的物品添加无尽增益支持(放置站类)，放置站类指有一个即可提供增益。放置站一般需要手动提供支持
## AddPortableCraftingStation
```"AddPortableCraftingStation", int ItemID, int/List<int> TileID(s)```

为某个指定的物品添加便携制作站支持，对于没有设置Item.createTile但是应该充当某种制作站的物品十分有用，你可以指定多个制作站。

如果你想要使其充当水源，应将TileID设置为水槽 (TileID.Sink)

## 使用例
以下是一个为自己的放置站添加支持的例子
```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("ImproveGame", out Mod improveGame)) {
        improveGame.Call(
            "AddStation",//加入你自己的增益站1
            ModContent.ItemType<MyStation1>(), // 物品ID 1
            ModContent.BuffType<MyStationBuff1>() // BuffID 1
        );
        improveGame.Call(
            "AddStation",//加入你自己的增益站2
            ModContent.ItemType<MyStation2>(), // 物品ID 2
            ModContent.BuffType<MyStationBuff2>() // BuffID 2
    }
}
```
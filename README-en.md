# 更好的体验，一个让模组游玩体验大大提升的Mod
本模组致力于给模组游玩体验提一个档次，成为一个广泛使用的辅助Mod

[Chinese version here | 中文版](README.md)

[Change log](ChangeLog-en.md)

# Mod.Call
## IgnoreInfItem
```"IgnoreInfItem", int/List<int> ItemID(s)```
为某个/些指定的物品添加无尽增益忽略，以防止在拥有30个时无限提供增益
## AddPotion
```"AddPotion", int ItemID, int BuffID```
为某个指定的物品添加无尽增益支持(药水类)，对于没有设置Item.buffType或拥有多个增益的物品十分有用，药水类指需要达到30堆叠才能提供增益
## AddStation
```"AddPotion", int ItemID, int BuffID```
为某个指定的物品添加无尽增益支持(放置站类)，放置站类指有一个即可提供增益。放置站一般需要手动提供支持

## 使用例
以下是一个为自己的放置站添加支持的例子
```CSharp
public override void PostSetupContent() {
    if (ModLoader.TryGetMod("ImproveGame", out Mod improveGame)) {
        improveGame.Call(
            "AddStation",
            ModContent.ItemType<MyStation>(), // 物品ID
            ModContent.BuffType<MyStationBuff>() // Buff ID
        ); // 只有当玩家朝右时才会显示
    }
}
```
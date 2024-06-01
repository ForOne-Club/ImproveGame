<h1 align="center">更好的体验</h1>

<div align="center">

[English](README-en.md) | 简体中文

[更新日志](ChangeLog.md)

一个致力于给模组游玩体验提一个档次的辅助Mod

</div>

## ✨ 功能
详细列表请到模组配置或更新日志中查看
1. 物品最大堆叠等辅助Mod普遍功能(对标老牌辅助Luiafk)
2. 非时装饰品也可放置在时装栏（启用模组直接生效）
3. 城镇NPC入住机制修改: 夜晚入住、图鉴解锁后无视条件入住
4. 空间魔杖、建筑魔杖、法爆魔杖和钓鱼机等大大优化游戏体验的模组物品
5. 渔夫任务无冷却、控制墓碑是否掉落等辅助模组功能的整合
6. 随身增益、随身制作站等节省时间的功能
7. 与同队好友共享无尽增益、制作站等专为联机设置的功能
8. 含100格的超大背包，再也不用担心旗帜和药水放在哪了
9. 药水袋与旗帜盒，更方便地将你的药水旗帜整合在一个物品，节省空间
10. 几乎所有功能都是可以调节的，自行选择适合你的功能

## ⬇️ 下载
本Mod已在Steam创意工坊发布，可直接订阅下载: https://steamcommunity.com/sharedfiles/filedetails/?id=2797518634

## 💻 编译
更好的体验由于使用了NuGet，因此无法使用tModLoader进行编译，需要使用IDE进行编译
1. 使用IDE，如Visual Studio 2022、Rider等打开项目
2. 如果先前在tModLoader中启用了此Mod，先禁用，然后重新加载
3. 使用IDE编译项目
4. 在tModLoader中打开此Mod，重新加载
5. 成了

## 📗 版权声明
本模组的“显示物品、NPC所属模组”功能部分代码来源于模组“WMITF”，本模组与模组“WMITF”皆在MIT许可协议下开源，意味着“被授权人有权利使用、复制、修改、合并、出版发行、散布、再授权及贩售软件及软件的副本，并允许软件提供者这样做，但须满足以下条件：

上述版权声明和本许可声明应包含在本软件的所有副本或主要部分中。

因此，该模组不存在版权侵犯问题。
ChevyRay 的协程类也受 MIT 许可，与上面相同。
TextureGIF.cs的部分代码来自ProjectStarlight.Interchange，该项目也受 MIT 许可，与上面相同。
本模组的自动存钱功能大部分代码来自模组“Auto Piggy Bank”，该模组也受 MIT 许可，与上面相同。
本模组的StorageCommunicator部分代码来自模组“Magic Storage”，该模组也受 MIT 许可，与上面相同。

附:
本模组开源链接: https://gitee.com/MyGoold/improve-game (此为原仓库，Github仓库是由原仓库镜像而来)
WMITF开源链接: https://github.com/gardenappl/WMITF
ChevyRay的协程类: https://github.com/ChevyRay/Coroutines
ProjectStarlight.Interchange: https://github.com/ProjectStarlight/ProjectStarlight.Interchange
Auto Piggy Bank开源链接: https://github.com/diniamo/auto-piggy-bank
Magic Storage开源链接: https://github.com/blushiemagic/MagicStorage

## 🤝 跨Mod支持 (Mod.Call)

如果你是一名玩家，并想要让其他模组修复与更好的体验不兼容的问题，你可以向其他模组作者提出请求，让他们阅读此文档并添加跨Mod支持。

除了以 `GetXX` 开头的，其他Mod.Call的返回值是一个 `bool`，指示这个操作是否成功执行

### GetAmmoChainSequence
获取指定物品的弹药链序列
建议将本模组源码的 [AmmoChain.cs](Content\Functions\ChainedAmmo\AmmoChain.cs) 和 [ItemTypeData.cs](Core\ItemTypeData.cs) 复制到你的模组源码中，以便操作弹药链
#### 参数
- `Item` 要获取弹药链序列的物品实例
#### 返回值
- `TagCompound` 以TagCompound形式存储的弹药链数据，数据读取方式参考AmmoChain.cs和ItemTypeData.cs。如果物品没有弹药链，返回 `null`

### GetUniversalAmmoId
获取“任意弹药”物品的ID，这是一个用于弹药链中，表示该位置弹药任意的物品，搭配弹药链使用
#### 参数
无
#### 返回值
- `int` “任意弹药”物品的ID

### GetBigBagItems
获取大背包中的物品
#### 参数
- `Player` 大背包所属玩家的实例
#### 返回值
- `List<Item>` 大背包中共100格的物品的实例，包括空气

### IgnoreInfItem
为某个/些指定的物品添加无尽增益忽略，以防止在拥有30个时无限提供增益
#### 参数
- `int/List<int>` 不提供无限增益的某个物品/一些物品的ID

### AddPotion
为某个指定的物品添加无尽增益支持(药水类)，对于没有设置Item.buffType或拥有多个增益的物品十分有用，药水类指需要达到30堆叠才能提供增益
#### 参数
- `int` 添加药水类无尽增益支持的物品的ID
- `int/List<int>` 该物品提供的一个/一些增益的ID

### AddStation
为某个指定的物品添加无尽增益支持(放置站类)，放置站类指有一个即可提供增益。放置站一般需要手动提供支持
#### 参数
- `int` 添加放置站类无尽增益支持的物品的ID
- `int/List<int>` 该物品提供的一个/一些增益的ID

### AddPortableCraftingStation
为某个指定的物品添加便携制作站支持，对于没有设置Item.createTile但是应该充当某种制作站的物品十分有用，你可以指定多个制作站。
如果你想要使其充当水源，应将“该物品充当的制作站的物块ID”(即第二个参数)设置为水槽 (TileID.Sink)
#### 参数
- `int` 添加便携制作站支持的物品的ID
- `int/List<int>` 该物品可充当的一个/一些制作站的物块ID

### AddFishingAccessory
为某个指定的物品添加自动钓鱼机的钓鱼饰品支持，可设置钓鱼速度加成、渔力加成、是否应被视为钓具箱和是否可在岩浆钓鱼。一般需要手动提供饰品支持
#### 参数
- `int` 添加自动钓鱼机的钓鱼饰品支持的物品的ID
- `float` 该物品提供的钓鱼速度加成
- `float` 该物品提供的渔力加成
- `bool` 该物品是否应被视为钓具箱
- `bool` 该物品是否给予在熔岩中钓鱼的能力

### AddStatCategory
向属性面板添加一个属性类别，后续可以使用 `AddStat` 添加属性
#### 参数
- `string` 添加的属性类别的字符串标识符
- `Texture2D` 指示属性类别的图标
- `string` 该属性类别的名称的本地化键
- `Texture2D` 指示该属性类别所属模组的图标，建议使用模组的icon_small贴图

### AddStat
向某个属性类别添加一个属性
#### 参数
- `string` 该属性要被添加到的属性类别的字符串标识符
- `string` 该属性的名称的本地化键
- `Func<string>` 该属性的值的获取函数

### AddHomeTpItem
添加回家物品
#### 参数
- `int/List<int>` 你要添加的某个物品/一些物品的ID
- `bool` 该物品是否应被视为药水，若为药水则需要堆叠超过无尽药水需求才能快捷使用
- `bool` 该物品是否应被视为折返药水，在回程时会创造一个折返传送门

### 使用例
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
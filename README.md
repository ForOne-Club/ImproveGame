<div align="center" style="padding-top: 25px;display: flex;flex-direction: column;align-items: center">

<img src="./icon_workshop.png" width="150"/>
<h3>更好的体验</h3>

[English](README-en.md) | 简体中文

[更新日志](ChangeLog.md)\
[跨 Mod 支持](./CrossModSupport.md)

</div>

## 📖 简介

**更好的体验** 模组的开发方向主要围绕以下三大模块展开：

### 1. 原版基础机制
允许玩家自由调整一些游戏基础机制，例如：
**物品最大堆叠数量**、**物品拾取范围**、**工具使用速度**、**额外 BUFF 栏**、**草药正常不受**、**重生加速** 等。

### 2.强力道具
新增一系列功能强大或玩法新颖的道具，例如：
**法爆魔杖**、**空间魔杖**、**科技魔杖**、**液体魔杖**、**虚空魔杖**、**钓鱼机**、**存储管理器** 等。

### 3. 玩家能力强化系统
提升角色便利性与自动化体验，例如：
**大背包扩容**、**扩展背包可拾取物品**、**无限药水 & 增益站**、**自动垃圾桶**、**自动开袋**、**快速微光净化**、**弹药链系统** 等。

📌 补充说明：

- 部分 **能力增强** 功能需通过特定道具激活，如：
**天气之书**、**定位球**、**瓶中微光船** 等。

- **更好的体验** 拥有独树一帜的 **UI 界面设计**，视觉风格与交互逻辑均与其他模组显著不同，是本模组的重要特色之一。

## ✨ 功能

绝大多数功能都在 **模组配置** 中可见，具体还请在游戏中查看。

1. 物品最大堆叠等辅助模组普遍功能
2. 非时装饰品也可放置在时装栏（启用模组直接生效）
3. 城镇NPC入住机制修改: 夜晚入住、图鉴解锁后无视条件入住
4. 空间魔杖、建筑魔杖、法爆魔杖和钓鱼机等大大优化游戏体验的模组物品
5. 渔夫任务无冷却、控制墓碑是否掉落等辅助模组功能的整合
6. 随身增益、随身制作站等节省时间的功能
7. 与同队好友共享无尽增益、制作站等专为联机设置的功能
8. 含100格的超大背包，再也不用担心旗帜和药水放在哪了
9. 药水袋与旗帜盒，更方便地将你的药水旗帜整合在一个物品，节省空间

> 几乎所有功能都是 **可以调节** 的，自行选择适合你的功能！

## ⬇️ 下载

模组已在 **Steam** 上 **tModLoader 创意工坊** 中发布，直接搜索：[更好的体验](https://steamcommunity.com/sharedfiles/filedetails/?id=2797518634)，即可找到此模组。

## 💻 构建项目

更好的体验由于使用了 **NuGet**，因此无法使用 **tModLoader** 进行编译，需要使用 **IDE** 进行编译

> 推荐 **IDE**：Rider、Visual Studio 2022、Visual Studio Code

### 克隆项目

由于本项目使用了 **Git 子模块**，因此需要使用 `--recurse-submodules` 参数克隆项目。且由于解决方案引用的项目应与ImproveGame文件夹平行，克隆后需要运行 `CreateLinks.bat` 脚本创建符号链接。（或手动在ImproveGame父目录下分别克隆这两个仓库）

在Git Bash或终端中运行以下命令：

```cmd
git clone --recurse-submodules https://github.com/ForOne-Club/ImproveGame.git
cd ImproveGame
.\CreateLinks.bat
```

### 编译项目

1. 使用 **IDE** 打开项目
2. 使用 **IDE** 编译项目（通常使用快捷键 `F5` 可快速启动）

## 📗 版权声明

本模组的“显示物品、NPC所属模组”功能部分代码来源于模组“WMITF”，本模组与模组“WMITF”皆在MIT许可协议下开源，意味着“被授权人有权利使用、复制、修改、合并、出版发行、散布、再授权及贩售软件及软件的副本，并允许软件提供者这样做，但须满足以下条件：

上述版权声明和本许可声明应包含在本软件的所有副本或主要部分中。

因此，该模组不存在版权侵犯问题。\
ChevyRay 的协程类也受 MIT 许可，与上面相同。\
TextureGIF.cs的部分代码来自ProjectStarlight.Interchange，该项目也受 MIT 许可，与上面相同。
本模组的自动存钱功能大部分代码来自模组“Auto Piggy Bank”，该模组也受 MIT 许可，与上面相同。
本模组的StorageCommunicator部分代码来自模组“Magic Storage”，该模组也受 MIT 许可，与上面相同。

附:\
本模组开源链接: <https://github.com/ForOne-Club/ImproveGame>\
WMITF开源链接: <https://github.com/gardenappl/WMITF>\
ChevyRay的协程类: <https://github.com/ChevyRay/Coroutines>\
ProjectStarlight.Interchange: <https://github.com/ProjectStarlight/ProjectStarlight.Interchange>\
Auto Piggy Bank开源链接: <https://github.com/diniamo/auto-piggy-bank>\
Magic Storage开源链接: <https://github.com/blushiemagic/MagicStorage>

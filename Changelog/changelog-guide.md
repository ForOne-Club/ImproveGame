<h1 align="center">更新日志编写指南</h1>

更新一个新版本时，需要完成以下步骤：
1. 确定版本号。按照 `v主版本号.次版本号.修订号.次修订号` 的格式命名，例如 `v1.7.0.0`。版本号应为四位，不足的补0，如 `v1.7` 应补为 `v1.7.0.0`。
   - 主版本号：当进行了不兼容的API修改，或属于tModLoader大版本适配更新时，增加主版本号。
   - 次版本号：当增加了大量新功能，并距离上次更新时间较长时，增加次版本号。
   - 修订号：当进行了大量Bug修复，或添加了部分新功能时，增加修订号。
   - 次修订号：当进行少量了Bug修复时，增加修订版本号。
2. 在 `Changelog/zh` 和 `Changelog/en` 目录下各创建一个新的 `.md` 文件，文件名格式为 `v版本号.md`，例如 `v1.7.0.0.md`。
3. 在新创建的 `.md` 文件中按照[下面的格式](#日志文件格式)编写更新日志。
4. 在根目录下的 `ChangeLog.md` 和 `ChangeLog-en.md` 文件中，按照版本号的大小顺序插入新版本的相对链接。

## 日志文件格式

### 中文

```markdown
<h1 align="center">v版本号</h1>

<div align="center">

中文 | [English](../en/v版本号.md)

</div>

## 新增内容

（内容）

## BUG 修复

（内容）

## 调整内容

（内容）
```

### 英文

```markdown
<h1 align="center">v版本号</h1>

<div align="center">

[中文](../zh/v版本号.md) | English

</div>

## Additions

(Content)

## BUG Fixes

(Content)

## Adjustments

(Content)
```
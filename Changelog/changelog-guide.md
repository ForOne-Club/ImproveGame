<h1 align="center">更新日志编写指南</h1>

更新一个新版本时，需要完成以下步骤：

1. 确定版本号。按照 `va.b.c.x` 的格式命名，例如 `v1.7.0.0`。版本号应为四位，不足的补0，如 `v1.7` 应补为 `v1.7.0.0`。
   - a: `tML` 进行大型更新（例如 `Terraria` 本体大型更新时 `qot` 适配 `tML` 随之的更新），或者 `qot` 对整体进行了重构级别的大型更新时更新。
   - b：当增加了 `多项创意型新功能` 时，例如：`属性面板` `自动开袋` `自动微光`。
   - c：添加新的小功能时，例如：`添加了控制抓取范围的配置功能` `添加了控制树木生成的配置功能` （此类原版简单机制控制）。
   - x：修复 Bug 时，增加修订版本号。
2. 在 `Changelog/zh` 和 `Changelog/en` 目录下各创建一个新的 `.md` 文件，文件名格式为 `v a.b.c.x.md`，例如 `v1.7.0.x.md`。
3. 在新创建的 `.md` 文件中按照[下面的格式](#日志文件格式)编写更新日志。

## 日志文件格式

### 中文

```markdown
<h1 align="center">va.b.c.x</h1>

<div align="center">

中文 | [English](../en/va.b.c.x.md)

[全部更新日志](../../ChangeLog.md)

</div>

## v1.8.1.2

### 新增内容

（内容）

### BUG 修复

（内容）

### 调整内容

（内容）

## v1.8.1.1

### 新增内容

（内容）

### BUG 修复

（内容）

### 调整内容

（内容）

## v1.8.1.0

### 新增内容

（内容）

### BUG 修复

（内容）

### 调整内容

（内容）
```

### 英文

```markdown
<h1 align="center">v版本号</h1>

<div align="center">

[中文](../zh/va.b.c.x.md) | English

</div>

## v1.8.1.2

## Additions

(Content)

## BUG Fixes

(Content)

## Adjustments

(Content)

## v1.8.1.1

## Additions

(Content)

## BUG Fixes

(Content)

## Adjustments

(Content)

## v1.8.1.0

## Additions

(Content)

## BUG Fixes

(Content)

## Adjustments

(Content)
```

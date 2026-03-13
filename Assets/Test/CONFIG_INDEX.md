# UGC 配置系统说明文档

> **AI 必读**：本文档是 UGC 配置系统的完整说明。阅读本文档后，你将了解如何通过 JSON 文件修改游戏内容，并可以根据用户需求**自由创作任意主题风格**的配置。

---

## 一、系统概述

### 什么是 UGC 配置系统？

这是一个**运行时配置热更新系统**，允许在不重新打包游戏的情况下，通过 JSON 文件修改游戏内容（技能名称、数值、敌人属性等）。

### 核心能力

| 能力 | 说明 |
|------|------|
| **内容热更新** | 通过更新 JSON 文件修改游戏内容，无需重新发版 |
| **主题换皮** | 同一套游戏逻辑，通过不同 JSON 配置实现不同主题风格 |
| **自由创作** | AI 可以根据用户需求，创作任意主题的配置（科幻、中世纪、武侠、萌系等） |
| **CDN 动态加载** | 支持从远程服务器下载配置 |

### 工作原理

```
JSON 配置文件 → RuntimeConfigLoader 加载 → 覆盖 ScriptableObject 属性 → 游戏显示新内容
```

---

## 二、关键文件路径

| 文件 | 路径 | 作用 |
|------|------|------|
| **本文档** | `The Archer/Config/CONFIG_INDEX.md` | UGC 系统说明文档 |
| **字段参考手册** | `The Archer/Config/UGC_Configuration_Guide.md` | 完整 SO 字段结构（180+ 个配置） |
| **配置加载器** | `The Archer/Scripts/UGC/RuntimeConfigLoader.cs` | 核心代码，负责加载和应用 JSON 配置 |
| **运行时配置目录** | `StreamingAssets/UGC_Configs/` | 存放所有 JSON 配置文件（运行时读取） |
| **配置清单** | `StreamingAssets/UGC_Configs/manifest.txt` | 列出所有要加载的配置文件名 |

### 文档与配置的区别

| 类型 | 位置 | 说明 |
|------|------|------|
| **说明文档** | `The Archer/Config/` | 给开发者/AI 阅读，不打包到游戏 |
| **运行时配置** | `StreamingAssets/UGC_Configs/` | JSON 配置文件，打包到游戏运行时读取 |

---

## 三、当前示例主题：赛博朋克（仅供参考）

> ⚠️ **注意**：以下配置是一个**示例主题**，用于演示 JSON 配置的格式和用法。
> 
> **你可以根据用户需求，创作全新的主题配置**，例如：
> - 中世纪骑士风格
> - 日式武侠风格
> - 萌系可爱风格
> - 克苏鲁恐怖风格
> - 或任何用户想要的主题
>
> 创作新主题时，请参考 `The Archer/Config/UGC_Configuration_Guide.md` 获取完整的可配置字段列表。

### 当前示例配置文件

| 文件名 | 对应资源 | 原始名称 → 示例主题名称 |
|--------|----------|------------------------|
| `Enemies_Database_UGC.json` | 敌人数据库 | 所有敌人名称改为赛博朋克风格 |
| `Freeze_Ability_UGC.json` | 冰冻技能 | 冰冻 → 电磁脉冲 |
| `Ignite_Ability_UGC.json` | 点燃技能 | 点燃 → 等离子灼烧 |
| `Fire_Spirit_Ability_UGC.json` | 火焰精灵 | 火焰精灵 → 战斗无人机 |
| `Fire_Orbs_Ability_UGC.json` | 火焰球 | 火焰球 → 激光卫星 |
| `Second_Chance_Ability_UGC.json` | 第二次机会 | 第二次机会 → 紧急修复协议 |
| `Minor_Health_Boost_Ability_UGC.json` | 生命提升 | 生命提升 → 纳米强化 |
| `Minor_Power_Boost_Ability_UGC.json` | 力量提升 | 力量提升 → 能量增幅 |
| `Critical_Strike_Ability_UGC.json` | 暴击 | 暴击 → 精准打击系统 |
| `Bouncy_Shot_Ability_UGC.json` | 弹射箭 | 弹射箭 → 弹射射击 |
| `Rear_Shot_Ability_UGC.json` | 后射 | 后射 → 后方射击 |

---

## 四、如何创作新主题配置

### 步骤 1：确定要修改的内容

参考 `The Archer/Config/UGC_Configuration_Guide.md`，找到要修改的 ScriptableObject 及其字段。

### 步骤 2：创建 JSON 配置文件

在 `StreamingAssets/UGC_Configs/` 目录下创建 JSON 文件，每个文件必须包含 `assetPath` 字段：

```json
{
  "assetPath": "Assets/The Archer/Scriptables/Abilities/Freeze.asset",
  "name": "你的新名称",
  "description": "你的新描述"
}
```

### 步骤 3：更新 manifest.txt

将新创建的 JSON 文件名添加到 `StreamingAssets/UGC_Configs/manifest.txt` 中。

### 可修改的字段类型

| 类型 | 示例 | 说明 |
|------|------|------|
| 字符串 | `"name": "新名称"` | 名称、描述等文本 |
| 数值 | `"damage": 100` | 伤害、冷却时间等 |
| 布尔值 | `"isActive": true` | 开关类属性 |
| 枚举 | `"rarity": 2` | 稀有度等（使用数字） |

### 不可修改的字段（Unity 资源引用）

- Sprite（图标、贴图）
- GameObject（预制体）
- AudioClip（音效）
- 其他 ScriptableObject 引用

---

## 五、配置加载入口

- **加载器**：`The Archer/Scripts/UGC/RuntimeConfigLoader.cs`
- **清单文件**：`StreamingAssets/UGC_Configs/manifest.txt`（列出所有要加载的配置文件）
- **加载时机**：游戏启动时自动加载（`[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]`）

---

## 六、快速定位资源

如果你需要找某个功能的配置，参考 `The Archer/Config/UGC_Configuration_Guide.md`：

| 功能类型 | SO 资源目录 | JSON 配置命名规则 |
|----------|----------|-------------|
| 技能 | `The Archer/Scriptables/Abilities/` | `{技能名}_Ability_UGC.json` |
| 敌人 | `The Archer/Scriptables/Enemies Database.asset` | `Enemies_Database_UGC.json` |
| 装备 | `The Archer/Scriptables/Items/` | `{装备名}_Item_UGC.json` |
| 关卡 | `The Archer/Scriptables/Levels/` | `{关卡名}_Level_UGC.json` |

---

## 七、CDN 模式（可选）

如果使用 CDN 加载配置：

1. 将所有 JSON 文件上传到 CDN
2. 创建 `manifest.json`：
   ```json
   {
     "version": "1.0.0",
     "files": [
       {"name": "Enemies_Database_UGC.json", "hash": "abc123", "size": 1024}
     ]
   }
   ```
3. 在代码中配置：
   ```csharp
   RuntimeConfigLoader.SetCDNUrl("https://your-cdn.com/ugc/", useCDN: true);
   ```

---

## 八、AI 协作指南

### 给新 AI 的提示词

```
请阅读 `The Archer/Config/CONFIG_INDEX.md` 了解 UGC 配置系统，
然后参考 `The Archer/Config/UGC_Configuration_Guide.md` 获取完整字段列表，
帮我创作一个 [你想要的主题] 风格的配置。
```

### 创作新主题的建议

1. **先确定主题风格**：科幻、奇幻、武侠、萌系、恐怖等
2. **参考字段手册**：`UGC_Configuration_Guide.md` 包含 180+ 个可配置项
3. **保持风格统一**：所有名称、描述应符合同一主题
4. **只修改文本和数值**：不要尝试修改 Sprite、AudioClip 等资源引用

### 主题创意示例

| 主题 | 技能改名示例 | 敌人改名示例 |
|------|-------------|-------------|
| 赛博朋克 | 冰冻 → 电磁脉冲 | 史莱姆 → 纳米虫群 |
| 中世纪骑士 | 冰冻 → 寒冰咒语 | 史莱姆 → 泥沼怪 |
| 日式武侠 | 冰冻 → 冰刃斩 | 史莱姆 → 妖泥 |
| 萌系可爱 | 冰冻 → 冰淇淋魔法 | 史莱姆 → 软糖宝宝 |
| 克苏鲁恐怖 | 冰冻 → 虚空冻结 | 史莱姆 → 蠕动之物 |

---

## 九、目录结构总览

```
Assets/
├── The Archer/
│   ├── Config/                              # 📚 文档目录（不打包）
│   │   ├── CONFIG_INDEX.md                  # 本文档 - UGC 系统说明
│   │   └── UGC_Configuration_Guide.md       # 字段参考手册
│   ├── Scripts/
│   │   └── UGC/
│   │       └── RuntimeConfigLoader.cs       # 配置加载器
│   └── Scriptables/                         # SO 资源文件
│       ├── Abilities/
│       ├── Enemies Database.asset
│       └── ...
└── StreamingAssets/
    └── UGC_Configs/                         # 🎮 运行时配置（打包）
        ├── manifest.txt                     # 配置清单
        └── *.json                           # JSON 配置文件
```

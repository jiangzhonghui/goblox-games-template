# UGC 配置指南 (AI Configuration Guide)

> **用途**: 本文档供 AI 阅读，用于快速定位和修改游戏配置
> 
> **核心原则**: 根据要修改的功能，找到对应的 JSON 文件，只读取需要的文件以减少 Token 消耗

---

## 一、JSON 文件完整索引

### 1.1 数据库文件（Database）- 总索引

这些是各模块的**总索引文件**，包含该模块所有条目的引用：

| 文件名 | 功能 | 包含内容 |
|--------|------|----------|
| `Enemies Database.json` | 敌人总表 | 所有敌人的基础配置 |
| `Abilities Database.json` | 技能总表 | 所有技能的索引引用 |
| `Armory Database.json` | 装备总表 | 所有装备的索引引用 |
| `Stage Database.json` | 关卡总表 | 所有关卡的索引引用 |
| `Drop Database.json` | 掉落总表 | 掉落物类型配置 |
| `Chests Database.json` | 宝箱总表 | 宝箱类型配置 |
| `Currencies Database.json` | 货币总表 | 货币类型配置 |
| `Upgrades Database.json` | 升级总表 | 永久升级索引 |
| `Experience Data.json` | 经验曲线 | 升级所需经验配置 |
| `Testing Database.json` | 测试预设 | 调试用技能预设 |
| `World Space Indicators Database.json` | 指示器 | 世界空间UI指示器 |
| `Scene Settings.json` | 场景设置 | 场景相关配置 |

---

### 1.2 技能配置（Abilities）- 74个文件

#### 武器技能 (Weapon Abilities) - 14个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Bouncy Shot Ability Data.json` | 弹射箭 | 箭矢在敌人间弹射 |
| `Close Combat Ability Data.json` | 近战 | 近距离伤害加成 |
| `Diagonal Shot Ability Data.json` | 斜射 | 斜向发射箭矢 |
| `Flying Weapon Ability Data.json` | 飞行武器 | 武器自动攻击 |
| `Multi Shot Ability Data.json` | 多重射击 | 同时发射多支箭 |
| `Piercing Strike Ability Data.json` | 穿透打击 | 箭矢穿透敌人 |
| `Quick Shot Ability Data.json` | 快速射击 | 提高攻击速度 |
| `Rear Shot Ability Data.json` | 后射 | 向后发射箭矢 |
| `Ricochet Ability Data.json` | 反弹 | 箭矢反弹 |
| `Sniper Ability Data.json` | 狙击 | 远距离伤害加成 |
| `Split Shot Ability Data.json` | 分裂射击 | 箭矢分裂 |
| `Traversing Shot Ability Data.json` | 穿越射击 | 箭矢穿越障碍 |

#### 元素技能 (Elemental Abilities) - 11个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Freeze Ability Data.json` | 冰冻 | 冻结敌人 |
| `Ignite Ability Data.json` | 点燃 | 燃烧伤害 |
| `Poison Ability Data.json` | 毒素 | 持续毒伤害 |
| `Shock Ability Data.json` | 电击 | 连锁闪电 |
| `Meteor Strike Ability Data.json` | 陨石打击 | 召唤陨石 |
| `Thunder Ability Data.json` | 雷电 | 雷电攻击 |
| `Blizzard Ability Data.json` | 暴风雪 | 范围冰冻 |
| `Inferno Ability Data.json` | 地狱火 | 范围燃烧 |
| `Venom Ability Data.json` | 剧毒 | 强化毒素 |
| `Lightning Ability Data.json` | 闪电 | 闪电链 |
| `Elemental Mastery Ability Data.json` | 元素精通 | 元素伤害加成 |

#### 属性技能 (Stat Abilities) - 17个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Minor Power Boost Ability Data.json` | 小幅力量提升 | 伤害+10% |
| `Major Power Boost Ability Data.json` | 大幅力量提升 | 伤害+25% |
| `Minor Health Boost Ability Data.json` | 小幅生命提升 | 生命+10% |
| `Major Health Boost Ability Data.json` | 大幅生命提升 | 生命+25% |
| `Minor Speed Increase Ability Data.json` | 小幅速度提升 | 移速+10% |
| `Minor Attack Speed Boost Ability Data.json` | 攻速提升 | 攻速+10% |
| `Minor Swiftness Ability Data.json` | 小幅敏捷 | 综合速度提升 |
| `Major Swiftness Ability Data.json` | 大幅敏捷 | 综合速度大幅提升 |
| `Boss Slayer Ability Data.json` | Boss杀手 | 对Boss伤害加成 |
| `Titan Form Ability Data.json` | 泰坦形态 | 体型和属性增大 |
| `Trifecta Ability Data.json` | 三连击 | 三重攻击 |
| `Critical Strike Ability Data.json` | 暴击 | 暴击率提升 |
| `Bloodlust Ability Data.json` | 嗜血 | 击杀回血 |
| `Berserker Ability Data.json` | 狂战士 | 低血量伤害加成 |
| `Glass Cannon Ability Data.json` | 玻璃大炮 | 高伤害低生命 |
| `Tank Ability Data.json` | 坦克 | 高生命低伤害 |
| `Balanced Ability Data.json` | 平衡 | 均衡属性 |

#### 防御技能 (Defensive Abilities) - 9个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Heal Ability Data.json` | 治疗 | 升级时回血 |
| `Power Heal Ability Data.json` | 强力治疗 | 大量回血 |
| `Cure Ability Data.json` | 治愈 | 持续回血 |
| `Second Chance Ability Data.json` | 第二次机会 | 复活 |
| `Safety Bubble Ability Data.json` | 安全气泡 | 护盾 |
| `Protector Ability Data.json` | 保护者 | 减伤 |
| `Strong Aura Ability Data.json` | 强力光环 | 范围减伤 |
| `Dodge Ability Data.json` | 闪避 | 闪避几率 |
| `Reflect Ability Data.json` | 反射 | 反弹伤害 |

#### 召唤技能 (Spirit Abilities) - 7个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Fire Spirit Ability Data.json` | 火焰精灵 | 召唤火精灵 |
| `Ice Spirit Ability Data.json` | 冰霜精灵 | 召唤冰精灵 |
| `Lightning Spirit Ability Data.json` | 闪电精灵 | 召唤电精灵 |
| `Poison Spirit Ability Data.json` | 毒素精灵 | 召唤毒精灵 |
| `Guardian Spirit Ability Data.json` | 守护精灵 | 召唤守护者 |
| `Attack Spirit Ability Data.json` | 攻击精灵 | 召唤攻击者 |
| `Support Spirit Ability Data.json` | 辅助精灵 | 召唤辅助者 |

#### 环绕球技能 (Orb Abilities) - 8个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Fire Orbs Ability Data.json` | 火焰球 | 环绕火球 |
| `Ice Orbs Ability Data.json` | 冰霜球 | 环绕冰球 |
| `Shock Orbs Ability Data.json` | 电击球 | 环绕电球 |
| `Poison Orbs Ability Data.json` | 毒素球 | 环绕毒球 |
| `Ultimate Orbs Ability Data.json` | 终极球 | 强化环绕球 |
| `Defensive Orbs Ability Data.json` | 防御球 | 护盾球 |
| `Healing Orbs Ability Data.json` | 治疗球 | 回血球 |
| `Explosive Orbs Ability Data.json` | 爆炸球 | 爆炸伤害 |

#### 增益技能 (Buff Abilities) - 6个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Combo Ability Data.json` | 连击 | 连击加成 |
| `Rage Ability Data.json` | 狂怒 | 低血量增伤 |
| `Focus Ability Data.json` | 专注 | 暴击加成 |
| `Momentum Ability Data.json` | 动量 | 移动加成 |
| `Adrenaline Ability Data.json` | 肾上腺素 | 攻速加成 |
| `Bloodthirst Ability Data.json` | 嗜血 | 吸血效果 |

#### 终局技能 (Endgame Abilities) - 8个
| 文件名 | 技能名 | 功能 |
|--------|--------|------|
| `Armageddon Ability Data.json` | 末日 | 终极伤害 |
| `Immortality Ability Data.json` | 不朽 | 无敌 |
| `Omnipotence Ability Data.json` | 全能 | 全属性提升 |
| `Apocalypse Ability Data.json` | 天启 | 范围毁灭 |
| `Divine Ability Data.json` | 神圣 | 神圣伤害 |
| `Chaos Ability Data.json` | 混沌 | 随机效果 |
| `Void Ability Data.json` | 虚空 | 虚空伤害 |
| `Infinity Ability Data.json` | 无限 | 无限能力 |

---

### 1.3 装备配置（Armory）- 12个文件

#### 武器 (Weapons) - 2个
| 文件名 | 装备名 | 功能 |
|--------|--------|------|
| `Bow Data.json` | 弓 | 默认远程武器 |
| `Wand Data.json` | 法杖 | 魔法武器 |

#### 防具 (Armor) - 6个
| 文件名 | 装备名 | 功能 |
|--------|--------|------|
| `Armor Data.json` | 护甲 | 身体防具 |
| `Coat Data.json` | 外套 | 轻型防具 |
| `Helmet Data.json` | 头盔 | 头部防具 |
| `Hat Data.json` | 帽子 | 轻型头部 |
| `Ring 1 Data.json` | 戒指1 | 饰品 |
| `Ring 2 Data.json` | 戒指2 | 饰品 |

#### 英雄 (Heroes) - 2个
| 文件名 | 英雄名 | 功能 |
|--------|--------|------|
| `Hero Data 001.json` | 英雄1 | 默认角色 |
| `Hero Data 002.json` | 英雄2 | 解锁角色 |

#### 动画集 (Animation Sets) - 2个
| 文件名 | 功能 |
|--------|------|
| `Bow Animations Set.json` | 弓箭动画 |
| `Wand Animations Set.json` | 法杖动画 |

---

### 1.4 关卡配置（Stages）- 3个文件

| 文件名 | 功能 |
|--------|------|
| `Stage Database.json` | 关卡总索引 |
| `Stage Data 001.json` | 第一关配置 |
| `Stage Data 002.json` | 第二关配置 |

---

### 1.5 升级配置（Upgrades）- 10个文件

| 文件名 | 升级名 | 功能 |
|--------|--------|------|
| `Upgrades Database.json` | 升级总索引 | 所有升级引用 |
| `Power Upgrade.json` | 力量升级 | 永久伤害提升 |
| `Base HP Upgrade.json` | 生命升级 | 永久生命提升 |
| `Attack Speed Upgrade.json` | 攻速升级 | 永久攻速提升 |
| `Move Speed Upgrade.json` | 移速升级 | 永久移速提升 |
| `Crit Chance Upgrade.json` | 暴击升级 | 永久暴击提升 |
| `HP Recovery Upgrade.json` | 回血升级 | 永久回血提升 |
| `Dodge Upgrade.json` | 闪避升级 | 永久闪避提升 |
| `Shield Upgrade.json` | 护盾升级 | 永久护盾提升 |
| `Revive Upgrade.json` | 复活升级 | 永久复活次数 |

---

### 1.6 状态效果（Effects）- 10个文件

#### 状态效果 (Status Effects) - 4个
| 文件名 | 效果名 | 功能 |
|--------|--------|------|
| `Freeze Status Effect Data.json` | 冰冻效果 | 冻结状态配置 |
| `Ignite Status Effect Data.json` | 燃烧效果 | 燃烧状态配置 |
| `Poision Status Effect Data.json` | 中毒效果 | 中毒状态配置 |
| `Shock Status Effect Data.json` | 电击效果 | 电击状态配置 |

#### 边缘效果 (Rim Effects) - 6个
| 文件名 | 功能 |
|--------|------|
| `Freeze Rim Data.json` | 冰冻边缘光效 |
| `Ignite Rim Data.json` | 燃烧边缘光效 |
| `Poison Rim Data.json` | 中毒边缘光效 |
| `Shock Rim Data.json` | 电击边缘光效 |
| `Hit Rim Data.json` | 受击边缘光效 |
| `Player Hit Rim Data.json` | 玩家受击光效 |

---

### 1.7 音频配置（Audio）- 50+个文件

#### 音乐 (Music) - 3个
| 文件名 | 功能 |
|--------|------|
| `Main Menu Music.json` | 主菜单音乐 |
| `Stage Music.json` | 关卡音乐 |
| `Boss Music.json` | Boss战音乐 |

#### UI音效 (UI) - 7个
| 文件名 | 功能 |
|--------|------|
| `Button Click.json` | 按钮点击 |
| `Stage Complete.json` | 关卡完成 |
| `Stage Failed.json` | 关卡失败 |
| `Level Up.json` | 升级音效 |
| `Card Reveal.json` | 卡牌揭示 |
| `Cards Scroll.json` | 卡牌滚动 |
| `Rarity Increase.json` | 稀有度提升 |

#### 状态效果音效 (Status Effects) - 6个
| 文件名 | 功能 |
|--------|------|
| `Crit.json` | 暴击音效 |
| `Freeze.json` | 冰冻音效 |
| `Ignite.json` | 燃烧音效 |
| `Poison.json` | 中毒音效 |
| `Shock.json` | 电击音效 |
| `Shock Tick.json` | 电击持续音效 |

#### 武器音效 (Weapons & Items) - 11个
| 文件名 | 功能 |
|--------|------|
| `Bow Shooting.json` | 弓射击 |
| `Bow Enemy Hit.json` | 弓命中 |
| `Wand Enemy Hit.json` | 法杖命中 |
| `Damage.json` | 伤害音效 |
| `Enemy Hit.json` | 敌人受击 |
| `Heal.json` | 治疗音效 |
| `Gem Pick Up.json` | 宝石拾取 |
| `Gem Spawn.json` | 宝石生成 |
| `Item Drop.json` | 物品掉落 |
| `Item Pickup.json` | 物品拾取 |
| `Item Spawn.json` | 物品生成 |

#### 敌人音效 (Enemies) - 30+个
按敌人分类，每个敌人有独立的音效配置文件夹：
- `Bat/` - 蝙蝠音效
- `Bomb/` - 炸弹音效
- `Cactus/` - 仙人掌Boss音效 (8个)
- `Candle/` - 蜡烛音效
- `Cauldron/` - 大锅音效
- `Eye/` - 眼球音效
- `Golem/` - 石魔Boss音效 (3个)
- `Ice Slime/` - 冰史莱姆Boss音效 (8个)
- `Jellyfish/` - 水母音效
- `Mask/` - 面具Boss音效 (7个)
- `Plant/` - 植物音效
- `Pumpkin/` - 南瓜音效
- `Queen Wasp/` - 蜂后Boss音效 (3个)
- `Slime/` - 史莱姆音效
- `Totem/` - 图腾Boss音效 (6个)
- `Trap/` - 陷阱音效
- `Wasp/` - 黄蜂音效

---

## 二、快速定位指南

### 2.1 我要改什么？→ 找哪个文件？

| 修改目标 | 文件位置 | 修改类型 |
|----------|----------|----------|
| **敌人血量/伤害** | `Enemies Database.json` | 📊 数值 |
| **敌人名称** | `Enemies Database.json` | 📝 文本 |
| **敌人外观** | `Enemies Database.json` | 🎨 资源 |
| **技能数值** | `Abilities/[分类]/[技能名] Ability Data.json` | 📊 数值 |
| **技能名称/描述** | `Abilities/[分类]/[技能名] Ability Data.json` | 📝 文本 |
| **技能图标** | `Abilities/[分类]/[技能名] Ability Data.json` | 🎨 资源 |
| **武器数值** | `Armory/Items/[武器名] Data.json` | 📊 数值 |
| **武器名称** | `Armory/Items/[武器名] Data.json` | 📝 文本 |
| **装备数值** | `Armory/Items/[装备名] Data.json` | 📊 数值 |
| **关卡难度** | `Stages/Stage Data [N].json` | 📊 数值 |
| **升级效果** | `Upgrades/[升级名] Upgrade.json` | 📊 数值 |
| **经验曲线** | `Experience Data.json` | 📊 数值 |
| **掉落配置** | `Drop Database.json` | 📊 数值 |
| **状态效果** | `Effects/[效果名] Status Effect Data.json` | 📊 数值 |
| **音效配置** | `Audio/[分类]/[音效名].json` | 🎨 资源 |

---

## 三、修改类型说明

### 3.1 三种修改类型

| 类型 | 图标 | 字段特征 | 操作方式 |
|------|------|----------|----------|
| **文本** | 📝 | `title`, `description`, `name`, `itemName` | 直接改字符串 |
| **数值** | 📊 | `damage`, `hp`, `cost`, `*Multiplier`, `*Duration` | 直接改数字 |
| **资源** | 🎨 | `icon`, `prefab`, `sprite`, `*Prefab`, `clip` | 需要资源文件存在 |

### 3.2 字段识别规则

| 字段名特征 | 类型 | 操作 |
|------------|------|------|
| `title`, `name`, `description`, `*Name` | 📝 文本 | 直接改字符串 |
| `damage`, `hp`, `cost`, `chance` | 📊 数值 | 直接改数字 |
| `*Multiplier`, `*Duration`, `*Interval` | 📊 数值 | 直接改浮点数 |
| `*Type`, `rarity` | 📊 枚举 | 改为枚举值(整数) |
| `is*`, `has*`, `enable*` | 📊 布尔 | true/false |
| `icon`, `sprite`, `prefab`, `*Prefab` | 🎨 资源 | Unity资源路径 |
| `guid`, `id`, `type`, `assetPath` | ⚠️ 系统 | **勿修改** |

---

## 四、常用枚举值

### 4.1 稀有度 (Rarity)
| 值 | 名称 | 颜色 |
|----|------|------|
| 0 | Common | 白色 |
| 1 | Rare | 蓝色 |
| 2 | Mystic | 紫色 |
| 3 | Legendary | 金色 |

### 4.2 物品类型 (ItemType)
| 值 | 名称 |
|----|------|
| 0 | Weapon |
| 1 | Armor |
| 2 | Helmet |
| 3 | Ring |

### 4.3 属性类型 (StatType)
| 值 | 名称 | 说明 |
|----|------|------|
| 0 | HP | 生命值 |
| 1 | Damage | 伤害 |
| 2 | AttackSpeed | 攻击速度 |
| 3 | MovementSpeed | 移动速度 |
| 4 | CriticalChance | 暴击率 |
| 5 | DamageReduction | 减伤 |
| 6 | HPRecovery | 生命恢复 |

### 4.4 敌人类型 (EnemyType)
| 值 | 名称 | 值 | 名称 |
|----|------|----|----|
| 0 | Cauldron | 8 | Bomb |
| 1 | Jelly | 9 | Pumpkin |
| 2 | Plant | 10 | Bat |
| 3 | Wasp | 11 | Hand |
| 4 | Eye | 12 | Mole |
| 5 | Slime | 13 | Trap |
| 6 | SmallSlime | 14 | Candle |
| 7 | TinySlime | 15 | Spike |
| 100 | QueenWasp (Boss) | 103 | IceSlime (Boss) |
| 101 | Cactus (Boss) | 104 | Mask (Boss) |
| 102 | Totem (Boss) | 105 | Golem (Boss) |

---

## 五、注意事项

### 5.1 系统字段（勿修改）
- `assetPath` - 配置映射路径
- `guid` - 物品唯一ID
- `type` (技能) - 技能唯一ID
- `enemyType` - 敌人类型ID

### 5.2 资源路径格式
```
Assets/The Archer/Icons/xxx.png      ✅ 正确
Assets/The Archer/Prefabs/xxx.prefab ✅ 正确
Icons/xxx.png                        ❌ 错误
```

### 5.3 部分更新
只需包含要修改的字段，其他字段保持原值：
```json
{
  "assetPath": "Assets/The Archer/Scriptables/xxx.asset",
  "levels": [{ "attackDamageMultiplier": 2.0 }]
}
```

---

## 六、目录结构

```
Assets/The Archer/Scriptables/
├── Abilities/                    # 技能配置 (74个)
│   ├── Weapon Abilities/         # 武器技能 (14个)
│   ├── Elemental Abilities/      # 元素技能 (11个)
│   ├── Stat Abilities/           # 属性技能 (17个)
│   ├── Defensive Abilities/      # 防御技能 (9个)
│   ├── Spirit Abilities/         # 召唤技能 (7个)
│   ├── Orb Abilities/            # 环绕球技能 (8个)
│   ├── Buff Abilities/           # 增益技能 (6个)
│   └── Endgame Abilities/        # 终局技能 (8个)
├── Armory/                       # 装备配置 (12个)
│   ├── Items/                    # 装备数据
│   └── Heroes/                   # 英雄数据
├── Stages/                       # 关卡配置 (3个)
├── Upgrades/                     # 升级配置 (10个)
├── Effects/                      # 效果配置 (10个)
├── Audio/                        # 音频配置 (50+个)
│   ├── Music/
│   ├── UI/
│   ├── Abilities/
│   ├── Enemies/
│   └── Weapons & Items/
└── *.asset                       # 数据库文件 (12个)
```

---

*文档版本: 3.0*  
*总配置文件数: 约180个*  
*适用于: The Archer Roguelike UGC 配置系统*

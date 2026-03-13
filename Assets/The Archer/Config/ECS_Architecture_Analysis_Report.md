# 🎮 The Archer Roguelike - ECS架构数据模型分析报告

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **项目路径**: `/Users/jiangzhonghui/Documents/projects/DroneRacingGame/TheArcherRougueLike`

---

## 目录

1. [游戏整体架构概览](#一游戏整体架构概览)
2. [Meta Data 定义](#二meta-data-定义)
3. [关卡系统 (Stage)](#三关卡系统-stage)
4. [玩家系统 (Player)](#四玩家系统-player)
5. [敌人系统 (Enemy)](#五敌人系统-enemy)
6. [能力系统 (Abilities)](#六能力系统-abilities)
7. [装备系统 (Armory)](#七装备系统-armory)
8. [配置表结构设计](#八配置表结构设计)
9. [平衡公式说明](#九平衡公式说明)
10. [后续建议](#十后续建议)

---

## 一、游戏整体架构概览

### 1.1 ECS架构映射

| ECS层级 | 游戏系统 | 核心类 |
|---------|----------|--------|
| **Entity (实体)** | 玩家、敌人、关卡、能力、装备 | PlayerBehavior, EnemyBehavior, StageData, AbilityData, ItemData |
| **Component (组件)** | 属性、状态、配置数据 | Stats, HealthbarBehavior, NavigationHandler, StatusEffectsHandler |
| **System (系统)** | 管理器、控制器 | StageController, AbilitiesManager, ArmoryManager, EnemiesSpawner |

### 1.2 核心Entity定义

```
┌─────────────────────────────────────────────────────────────────────┐
│                        GAME ENTITIES                                 │
├─────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐ │
│  │   PLAYER    │  │   ENEMY     │  │   STAGE     │  │  ABILITY    │ │
│  │  Entity     │  │  Entity     │  │  Entity     │  │  Entity     │ │
│  ├─────────────┤  ├─────────────┤  ├─────────────┤  ├─────────────┤ │
│  │ HealthComp  │  │ HealthComp  │  │ RoomComp    │  │ LevelComp   │ │
│  │ StatsComp   │  │ StatsComp   │  │ WaveComp    │  │ EffectComp  │ │
│  │ WeaponComp  │  │ AIComp      │  │ SpawnComp   │  │ RarityComp  │ │
│  │ AbilityComp │  │ DropComp    │  │ DiffComp    │  │             │ │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘ │
│                                                                      │
│  ┌─────────────┐  ┌─────────────┐                                   │
│  │   WEAPON    │  │    ITEM     │                                   │
│  │  Entity     │  │  Entity     │                                   │
│  ├─────────────┤  ├─────────────┤                                   │
│  │ DamageComp  │  │ StatComp    │                                   │
│  │ ProjectComp │  │ RarityComp  │                                   │
│  │ AnimComp    │  │ LevelComp   │                                   │
│  └─────────────┘  └─────────────┘                                   │
└─────────────────────────────────────────────────────────────────────┘
```

### 1.3 数据流向

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  Config JSON │ ──► │ ScriptableObj│ ──► │   Runtime    │
│  (外部配置)   │     │  (Unity资产) │     │  (游戏实例)   │
└──────────────┘     └──────────────┘     └──────────────┘
```

---

## 二、Meta Data 定义

### 2.1 枚举类型定义

#### EnemyType (敌人类型)

| 枚举值 | 名称 | 类型 |
|--------|------|------|
| 0 | Cauldron | 普通 |
| 1 | Jelly | 普通 |
| 2 | Plant | 普通 |
| 3 | Wasp | 普通 |
| 4 | Eye | 普通 |
| 5 | Slime | 普通 |
| 6 | SmallSlime | 普通 |
| 7 | TinySlime | 普通 |
| 8 | Bomb | 普通 |
| 9 | Pumpkin | 普通 |
| 10 | Bat | 普通 |
| 11 | Hand | 普通 |
| 12 | Mole | 普通 |
| 13 | Trap | 普通 |
| 14 | Candle | 普通 |
| 15 | Spike | 普通 |
| 100 | QueenWasp | Boss |
| 101 | Cactus | Boss |
| 102 | Totem | Boss |
| 103 | IceSlime | Boss |
| 104 | Mask | Boss |
| 105 | Golem | Boss |

#### AbilityCategory (能力类别)

| 类别 | 数量 | 说明 |
|------|------|------|
| Weapon | 14种 | 武器能力 (弹射、后射、狙击等) |
| Elemental | 11种 | 元素能力 (冰冻、燃烧、雷电、毒素) |
| Stat | 17种 | 属性能力 (生命、攻击、速度加成) |
| Spirit | 7种 | 灵魂能力 (召唤灵魂助战) |
| Orb | 8种 | 球体能力 (环绕球体攻击) |
| Defence | 9种 | 防御能力 (治疗、护盾、复活) |
| Buff | 6种 | 增益能力 (狂暴、无敌) |
| Endgame | 8种 | 终局能力 (通关奖励) |

#### Rarity (稀有度)

| 等级 | 名称 | 颜色 | 掉落概率 |
|------|------|------|----------|
| 0 | Common | #FFFFFF | 60% |
| 1 | Rare | #00FF00 | 25% |
| 2 | Mystic | #9900FF | 12% |
| 3 | Legendary | #FFD700 | 3% |

#### StatType (属性类型)

| 类型 | 说明 | 默认值 |
|------|------|--------|
| Damage | 伤害 | 0 |
| HP | 生命值 | 0 |
| AttackSpeed | 攻击速度(乘数) | 1.0 |
| MovementSpeed | 移动速度(乘数) | 1.0 |
| HPRecovery | 生命恢复 | 0 |
| CriticalChance | 暴击几率 | 0 |
| DamageReduction | 伤害减免(乘数) | 1.0 |

#### ItemType (物品类型)

| 类型 | 说明 |
|------|------|
| Weapon | 武器 |
| Armor | 护甲 |
| Helmet | 头盔 |
| Ring | 戒指 |

---

## 三、关卡系统 (Stage)

### 3.1 核心类结构

```
StageData (ScriptableObject)
├── stageName: string                    // 关卡名称
├── stageObjective: string               // 关卡目标
├── showAbilitySelector: bool            // 是否显示技能选择器
├── enemyDamageMulitplier: float         // 基础敌人伤害倍率
├── enemyDamageMultiplierRoomStep: float // 每房间伤害增长率
├── enemyDamageMultiplierWaveStep: float // 每波次伤害增长率
├── enemyHPMulitplier: float             // 基础敌人血量倍率
├── enemyHPMultiplierRoomStep: float     // 每房间血量增长率
├── enemyHPMultiplierWaveStep: float     // 每波次血量增长率
└── rooms: List<RoomData>                // 房间列表
    └── RoomData
        ├── groupId: int                 // 分组ID(用于随机)
        ├── playerSpawnPoint: Vector3    // 玩家出生点
        ├── firstWaveStartDelay: float   // 首波延迟
        ├── delayBetweenWaves: float     // 波次间隔
        ├── delayBeforeExitSpawn: float  // 出口延迟
        ├── cameraMovementType: enum     // 相机类型
        └── waves: WaveData[]            // 波次列表
            └── WaveData
                ├── groupId: int                    // 分组ID
                ├── waveDropExpericence: float      // 经验奖励
                ├── waveDropGold: int               // 金币奖励
                ├── customWaveEndDelay: float       // 结束延迟
                ├── enemySpawns: List<EnemySpawnData>
                │   └── EnemySpawnData
                │       ├── enemyType: EnemyType
                │       ├── transformData: TransformData
                │       └── overrideData: EnemyOverrideData
                └── chestSpawns: List<ChestSpawnData>
```

### 3.2 难度系数计算公式

```csharp
// 敌人伤害倍率计算
EnemyDamageMultiplier = StageData.EnemyDamageMultiplier +
    StageData.EnemyDamageMultiplierRoomStep * CurrentRoomIndex +
    StageData.EnemyDamageMultiplierWaveStep * CurrentWaveIndex

// 敌人血量倍率计算
EnemyHPMultiplier = StageData.EnemyHPMultiplier +
    StageData.EnemyHPMultiplierRoomStep * CurrentRoomIndex +
    StageData.EnemyHPMultiplierWaveStep * CurrentWaveIndex
```

### 3.3 关卡配置表

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| stage_id | int | ✅ | 关卡ID |
| stage_name | string | ✅ | 关卡名称 |
| stage_objective | string | ❌ | 关卡目标描述 |
| show_objective | bool | ❌ | 是否显示目标 |
| show_ability_selector | bool | ❌ | 是否显示技能选择器 |
| base_enemy_damage_multiplier | float | ✅ | 基础敌人伤害倍率 |
| enemy_damage_room_step | float | ✅ | 每房间伤害增长率 |
| enemy_damage_wave_step | float | ✅ | 每波次伤害增长率 |
| base_enemy_hp_multiplier | float | ✅ | 基础敌人血量倍率 |
| enemy_hp_room_step | float | ✅ | 每房间血量增长率 |
| enemy_hp_wave_step | float | ✅ | 每波次血量增长率 |

---

## 四、玩家系统 (Player)

### 4.1 核心类结构

```
PlayerBehavior : MonoBehaviour, IProjectileTarget
├── healthbar: HealthbarBehavior         // 血条组件
├── navigationHandler: NavigationHandler // 导航组件
├── orbsManager: PlayerOrbsManager       // 球体管理
├── spiritsManager: PlayerSpiritsManager // 灵魂管理
└── statsHandler: PlayerStatsHandler     // 属性统计
    └── PlayerStatsHandler
        ├── MaxHPStat: MultiplicativeStat           // 最大生命值
        ├── HealingStat: MultiplicativeStat         // 治疗加成
        ├── MovementSpeedStat: MultiplicativeStat   // 移动速度
        ├── AttackDamageStat: MultiplicativeStat    // 攻击伤害
        ├── AttackSpeedStat: MultiplicativeStat     // 攻击速度
        ├── DamageReduction: MultiplicativeStat     // 伤害减免
        ├── DodgeChanceStat: AdditiveStat           // 闪避几率
        ├── CritChanceStat: AdditiveStat            // 暴击几率
        ├── HPRecoveryStat: AdditiveStat            // 生命恢复
        ├── CritDamageMultiplierStat: MultiplicativeStat // 暴击伤害
        ├── RevivesStat: AdditiveStat               // 复活次数
        │
        │ // 箭矢属性
        ├── FrontArrowDamageStat: MultiplicativeStat
        ├── RearArrowDamageStat: MultiplicativeStat
        ├── DiagonalArrowDamageStat: MultiplicativeStat
        ├── FrontArrowsCountStat: AdditiveStat
        ├── RearArrowsCountStat: AdditiveStat
        ├── DiagonalArrowsCountStat: AdditiveStat
        ├── ArrowBounceCountStat: AdditiveStat
        ├── ArrowBounceDamageStat: MultiplicativeStat
        ├── ArrowRicochetCountStat: AdditiveStat
        ├── ArrowRicochetDamageStat: MultiplicativeStat
        ├── ArrowSizeStat: MultiplicativeStat
        ├── ArrowSpreadStat: AdditiveStat
        ├── ArrowRangeStat: MultiplicativeStat
        ├── MultishotStat: AdditiveStat
        ├── SplitArrowCountStat: AdditiveStat
        ├── SpliArrowDamageMultiplierStat: MultiplicativeStat
        ├── MagnetismStat: MultiplicativeStat
        │
        │ // 元素属性
        ├── PoisonDamageMultiplierStat: MultiplicativeStat
        ├── FreezeDamageMultiplierStat: MultiplicativeStat
        ├── BurnDamageMultiplierStat: MultiplicativeStat
        └── ShockDamageMultiplierStat: MultiplicativeStat
```

### 4.2 属性系统设计

#### AdditiveStat (加法属性)

```csharp
public class AdditiveStat
{
    protected int initialValue;        // 初始值
    public int Value { get; }          // 当前值
    protected List<StatAdder> adders;  // 加法器列表
}
// 计算公式: Value = initialValue + Σ(adders) + Σ(childrenStats)
```

#### MultiplicativeStat (乘法属性)

```csharp
public class MultiplicativeStat
{
    protected float initialValue;           // 初始值
    public float Value { get; }             // 当前值
    protected List<StatMultiplier> multipliers; // 乘法器列表
}
// 计算公式: Value = initialValue × Π(multipliers) × Π(childrenStats)
```

### 4.3 玩家基础属性配置表

| 字段名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| max_hp | float | 100 | 最大生命值 |
| attack_damage | float | 10 | 攻击伤害 |
| movement_speed | float | 5 | 移动速度 |
| attack_speed | float | 1.0 | 攻击速度 |
| damage_reduction | float | 0 | 伤害减免(0-1) |
| critical_chance | float | 5 | 暴击几率(0-100) |
| crit_damage_multiplier | float | 1.5 | 暴击伤害乘数 |
| hp_recovery | float | 0 | 生命恢复 |
| dodge_chance | float | 0 | 闪避几率(0-100) |
| healing_multiplier | float | 1.0 | 治疗加成 |

### 4.4 玩家箭矢属性配置表

| 字段名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| front_arrow_damage | float | 1.0 | 前方箭矢伤害乘数 |
| rear_arrow_damage | float | 1.0 | 后方箭矢伤害乘数 |
| diagonal_arrow_damage | float | 1.0 | 对角箭矢伤害乘数 |
| front_arrows_count | int | 1 | 前方箭矢数量 |
| rear_arrows_count | int | 0 | 后方箭矢数量 |
| diagonal_arrows_count | int | 0 | 对角箭矢数量 |
| arrow_bounce_count | int | 0 | 箭矢反弹次数 |
| arrow_bounce_damage | float | 1.0 | 反弹伤害乘数 |
| arrow_ricochet_count | int | 0 | 箭矢折射次数 |
| arrow_ricochet_damage | float | 1.0 | 折射伤害乘数 |
| arrow_size | float | 1.0 | 箭矢大小乘数 |
| arrow_spread | float | 0 | 箭矢扩散角度 |
| arrow_range | float | 10 | 箭矢射程 |
| multishot | int | 0 | 多重射击数量 |
| split_arrow_count | int | 0 | 分裂箭矢数量 |
| split_arrow_damage | float | 1.0 | 分裂箭矢伤害乘数 |
| magnetism | float | 0 | 磁力强度 |

---

## 五、敌人系统 (Enemy)

### 5.1 核心类结构

```
EnemyBehavior : MonoBehaviour
├── baseHealth: Float                    // 基础生命值
├── baseDamage: Float                    // 基础伤害
├── collisionDamageMultiplier: Float     // 碰撞伤害倍率
├── useKnockback: bool                   // 是否使用击退
├── knockbackMagnitude: Float            // 击退力度
├── knockbackCooldown: Float             // 击退冷却
├── hitScale: Vector3                    // 受击缩放
├── hitScaleDuration: Float              // 受击缩放持续时间
└── 子类特有字段...
```

### 5.2 敌人类型详细配置

#### 普通敌人

| 敌人 | 基础血量 | 基础伤害 | 特殊机制 |
|------|----------|----------|----------|
| Cauldron | 30 | 8 | 静止型 |
| Jelly | 25 | 6 | 多方向投射物 |
| Plant | 35 | 10 | 地下潜行 |
| Wasp | 40 | 12 | 冲锋攻击 |
| Eye | 30 | 8 | 激光警告 |
| Slime | 50 | 10 | 死亡分裂 |
| SmallSlime | 25 | 6 | 分裂产物 |
| TinySlime | 10 | 3 | 分裂产物 |
| Bomb | 20 | 25 | 自爆 |
| Pumpkin | 45 | 12 | 近战 |
| Bat | 20 | 8 | 飞行投射物 |
| Hand | 35 | 15 | 抓取 |
| Mole | 40 | 12 | 地下突袭 |
| Trap | 30 | 10 | 磁力吸附 |
| Candle | 25 | 8 | 火焰 |
| Spike | 15 | 20 | 静止陷阱 |

#### Boss敌人

| Boss | 基础血量 | 基础伤害 | 特殊机制 |
|------|----------|----------|----------|
| QueenWasp | 500 | 30 | 多阶段战斗 |
| Cactus | 600 | 25 | 范围攻击 |
| Totem | 700 | 20 | 召唤小怪 |
| IceSlime | 550 | 28 | 冰冻效果 |
| Mask | 800 | 35 | 三种攻击模式 |
| Golem | 1000 | 40 | 高防御 |

### 5.3 敌人行为配置示例

#### Wasp (黄蜂) 行为配置

| 参数 | 类型 | 值 | 说明 |
|------|------|-----|------|
| idle_duration | float | 2.5 | 空闲时间 |
| walk_duration | float | 3-5 | 移动时间 |
| walk_speed | float | 2-3 | 移动速度 |
| attack_speed | float | 7-9 | 冲锋速度 |
| max_attack_distance | float | 4-6 | 最大攻击距离 |
| attack_duration | float | 4.0 | 攻击持续时间 |

#### Bomb (炸弹) 行为配置

| 参数 | 类型 | 值 | 说明 |
|------|------|-----|------|
| explosion_trigger_radius | float | 2.0 | 触发爆炸半径 |
| explosion_radius | float | 3.0 | 爆炸伤害半径 |
| explosion_damage_multiplier | float | 0.9-1.1 | 爆炸伤害倍率 |
| walking_duration | float | 2.0 | 移动时间 |
| idle_duration | float | 1.0 | 空闲时间 |

#### Slime (史莱姆) 行为配置

| 参数 | 类型 | 值 | 说明 |
|------|------|-----|------|
| spawn_children_on_death | bool | true | 死亡时生成子敌人 |
| children_count | int | 3 | 子敌人数量 |
| child_enemy_type | int | 6 | 子敌人类型(SmallSlime) |
| idle_after_spawn | float | 0.5-1.0 | 生成后空闲时间 |
| idle_duration | float | 1.0-2.0 | 空闲时间 |
| movement_duration | float | 8.0-12.0 | 移动时间 |

### 5.4 敌人配置表

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| enemy_id | int | ✅ | 敌人ID |
| enemy_name | string | ✅ | 敌人名称 |
| is_boss | bool | ✅ | 是否为Boss |
| base_health | float | ✅ | 基础生命值 |
| base_damage | float | ✅ | 基础伤害 |
| collision_damage_multiplier | float | ✅ | 碰撞伤害倍率 |
| use_knockback | bool | ✅ | 是否使用击退 |
| knockback_magnitude | float | ❌ | 击退力度 |
| knockback_cooldown | float | ❌ | 击退冷却 |
| prefab_path | string | ✅ | 预制体路径 |

---

## 六、能力系统 (Abilities)

### 6.1 核心类结构

```
AbilityData : ScriptableObject
├── type: AbilityType                    // 能力类型
├── prerequisites: List<AbilityType>     // 前置能力
├── rarity: AbilityRarity                // 稀有度
├── isRepeatedAbiltiy: bool              // 是否可重复选择
├── isEndgameAbility: bool               // 是否为终局能力
├── title: string                        // 标题
├── description: string                  // 描述
├── icon: Sprite                         // 图标
├── prefab: GameObject                   // 行为预制体
└── levels: AbilityLevel[]               // 等级配置
    └── AbilityLevel (各子类不同)
        ├── damageMultiplier
        ├── attackSpeedMultiplier
        ├── effectDuration
        ├── effectDamageMultiplier
        └── ...
```

### 6.2 能力类型详细列表

#### 武器能力 (14种)

| 能力名称 | 稀有度 | 效果说明 |
|----------|--------|----------|
| BouncyShot | Rare | 箭矢在敌人间弹射 |
| RearShot | Common | 向后方发射箭矢 |
| Sniper | Rare | 距离越远伤害越高 |
| QuickShot | Common | 提高攻击速度 |
| CloseCombat | Rare | 近距离伤害加成 |
| PiercingStrike | Rare | 箭矢穿透敌人 |
| FrontProjectile | Common | 增加前方箭矢 |
| Ricochet | Rare | 箭矢折射 |
| SplitShot | Mystic | 箭矢分裂 |
| FlyingWeapon | Mystic | 飞行武器 |
| DiagonalShot | Common | 对角线发射 |
| TraversingShot | Rare | 穿越射击 |
| Bullseye | Legendary | 必中目标 |
| MultiShot | Mystic | 多重射击 |

#### 元素能力 (11种)

| 能力名称 | 稀有度 | 效果说明 |
|----------|--------|----------|
| Freeze | Rare | 冰冻敌人 |
| Shock | Rare | 雷电链接 |
| Ignite | Rare | 燃烧伤害 |
| Poison | Rare | 毒素伤害 |
| MeteorStrike | Mystic | 陨石打击 |
| MeteorRain | Legendary | 陨石雨 |
| IgniteEnhancement | Rare | 燃烧强化 |
| PoisonEnhancement | Rare | 毒素强化 |
| ShockEnhancement | Rare | 雷电强化 |
| FreezeEnhancement | Rare | 冰冻强化 |
| UltimateMeteor | Legendary | 终极陨石 |

#### 属性能力 (17种)

| 能力名称 | 稀有度 | 效果说明 |
|----------|--------|----------|
| MinorAttackSpeedBoost | Common | 小幅攻速提升 |
| BossSlayer | Rare | 对Boss伤害加成 |
| MinorHealthBoost | Common | 小幅生命提升 |
| MinorSpeedIncrease | Common | 小幅移速提升 |
| MinorPowerBoost | Common | 小幅攻击提升 |
| Starfall | Mystic | 星落 |
| MinorSwiftness | Common | 小幅敏捷提升 |
| CriticalStrike | Rare | 暴击几率 |
| LuckyCharm | Rare | 幸运加成 |
| FourLeafClover | Mystic | 四叶草 |
| MediumPower | Rare | 中等攻击提升 |
| TitanForm | Legendary | 巨人形态 |
| GnomeForm | Rare | 侏儒形态 |
| MajorSwiftness | Mystic | 大幅敏捷提升 |
| MajorHealthBoost | Mystic | 大幅生命提升 |
| MajorPowerBoost | Mystic | 大幅攻击提升 |
| Trifecta | Legendary | 三重加成 |

### 6.3 能力等级配置示例

#### Freeze (冰冻) 等级配置

| 等级 | 效果持续时间 | 箭矢伤害倍率 | 持续伤害 | 伤害倍率 | 伤害间隔 |
|------|--------------|--------------|----------|----------|----------|
| 1 | 2.0s | 0.8 | ❌ | - | - |
| 2 | 2.5s | 0.85 | ❌ | - | - |
| 3 | 3.0s | 0.9 | ✅ | 0.05 | 0.5s |
| 4 | 3.5s | 0.95 | ✅ | 0.08 | 0.5s |
| 5 | 4.0s | 1.0 | ✅ | 0.1 | 0.5s |

#### Critical Strike (暴击) 等级配置

| 等级 | 暴击几率加成 | 暴击伤害倍率 |
|------|--------------|--------------|
| 1 | +5% | 1.5x |
| 2 | +8% | 1.6x |
| 3 | +12% | 1.7x |
| 4 | +15% | 1.8x |
| 5 | +20% | 2.0x |

### 6.4 能力配置表

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| ability_id | int | ✅ | 能力ID |
| ability_name | string | ✅ | 能力名称 |
| ability_desc | string | ✅ | 能力描述 |
| category | string | ✅ | 能力类别 |
| rarity | int | ✅ | 稀有度(0-3) |
| is_repeated | bool | ✅ | 是否可重复选择 |
| is_endgame | bool | ✅ | 是否为终局能力 |
| max_level | int | ✅ | 最大等级 |
| prerequisites | string | ❌ | 前置能力ID(逗号分隔) |
| icon_path | string | ✅ | 图标资源路径 |
| prefab_path | string | ✅ | 行为预制体路径 |

---

## 七、装备系统 (Armory)

### 7.1 核心类结构

```
ItemData : ScriptableObject
├── guid: string                         // 唯一标识
├── itemType: ItemType                   // 物品类型
├── itemName: string                     // 物品名称
├── icon: Sprite                         // 图标
├── isDefaultItem: bool                  // 是否默认物品
├── isUnlockedByDefault: bool            // 是否默认解锁
└── itemLevels: List<ItemLevel>          // 等级列表
    └── ItemLevel
        ├── cost: int                    // 升级成本
        ├── itemRarity: ItemRarityType   // 稀有度
        ├── stats: List<StatData>        // 属性列表
        │   └── StatData
        │       ├── statType: StatType
        │       ├── value: float
        │       └── isVisibleOnUI: bool
        └── attachedAbilities: List<AbilityType>

WeaponData : ItemData
├── weaponPrefab: GameObject             // 武器预制体
└── isRightHandWeapon: bool              // 是否右手武器
```

### 7.2 武器配置示例

#### Iron Bow (铁弓)

| 等级 | 成本 | 稀有度 | 伤害 | 攻击速度 |
|------|------|--------|------|----------|
| 1 | 0 | Common | 10 | 1.0 |
| 2 | 100 | Common | 12 | 1.05 |
| 3 | 200 | Rare | 15 | 1.1 |
| 4 | 400 | Rare | 18 | 1.15 |
| 5 | 800 | Mystic | 22 | 1.2 |

#### Crossbow (弩)

| 等级 | 成本 | 稀有度 | 伤害 | 攻击速度 | 暴击几率 |
|------|------|--------|------|----------|----------|
| 1 | 500 | Rare | 15 | 0.8 | 5% |
| 2 | 200 | Rare | 18 | 0.85 | 8% |
| 3 | 400 | Mystic | 22 | 0.9 | 10% |
| 4 | 800 | Mystic | 26 | 0.95 | 12% |
| 5 | 1500 | Legendary | 32 | 1.0 | 15% |

### 7.3 装备配置表

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| item_id | string | ✅ | 物品唯一标识 |
| item_type | string | ✅ | 物品类型 |
| item_name | string | ✅ | 物品名称 |
| is_default | bool | ✅ | 是否默认物品 |
| is_unlocked_by_default | bool | ✅ | 是否默认解锁 |
| icon_path | string | ✅ | 图标路径 |

---

## 八、配置表结构设计

### 8.1 配置表清单

| 表名 | 说明 | 主键 |
|------|------|------|
| stage_config | 关卡配置 | stage_id |
| room_config | 房间配置 | room_id |
| wave_config | 波次配置 | wave_id |
| enemy_spawn_config | 敌人生成配置 | spawn_id |
| enemy_config | 敌人基础配置 | enemy_id |
| enemy_behavior_config | 敌人行为配置 | enemy_id |
| enemy_drop_config | 敌人掉落配置 | drop_id |
| player_base_config | 玩家基础属性 | config_id |
| player_arrow_config | 玩家箭矢属性 | config_id |
| player_elemental_config | 玩家元素属性 | config_id |
| ability_config | 能力配置 | ability_id |
| ability_level_config | 能力等级配置 | ability_id + level |
| item_config | 物品配置 | item_id |
| item_level_config | 物品等级配置 | item_id + level |
| weapon_config | 武器配置 | item_id |
| rarity_config | 稀有度配置 | rarity_id |

### 8.2 表关系图

```
┌─────────────────┐
│  stage_config   │
└────────┬────────┘
         │ 1:N
         ▼
┌─────────────────┐
│  room_config    │
└────────┬────────┘
         │ 1:N
         ▼
┌─────────────────┐
│  wave_config    │
└────────┬────────┘
         │ 1:N
         ▼
┌─────────────────┐     ┌─────────────────┐
│enemy_spawn_config│────►│  enemy_config   │
└─────────────────┘     └────────┬────────┘
                                 │ 1:1
                                 ▼
                        ┌─────────────────┐
                        │enemy_behavior   │
                        │    _config      │
                        └─────────────────┘

┌─────────────────┐     ┌─────────────────┐
│ ability_config  │────►│ability_level    │
└─────────────────┘     │    _config      │
                        └─────────────────┘

┌─────────────────┐     ┌─────────────────┐
│  item_config    │────►│ item_level      │
└────────┬────────┘     │    _config      │
         │              └─────────────────┘
         │ (Weapon)
         ▼
┌─────────────────┐
│ weapon_config   │
└─────────────────┘
```

---

## 九、平衡公式说明

### 9.1 伤害计算

```
// 玩家最终伤害
FinalDamage = BaseDamage 
            × AttackDamageMultiplier 
            × WeaponDamageMultiplier 
            × AbilityMultipliers

// 暴击伤害
CritDamage = FinalDamage × CritDamageMultiplier

// 持续伤害(DOT)
DOTDamage = BaseDamage × EffectDamageMultiplier (每间隔触发一次)
```

### 9.2 敌人属性计算

```
// 敌人最终伤害
EnemyFinalDamage = BaseDamage × (1 + DamageMultiplier 
                 + RoomStep × RoomIndex 
                 + WaveStep × WaveIndex)

// 敌人最终血量
EnemyFinalHP = BaseHP × (1 + HPMultiplier 
             + RoomStep × RoomIndex 
             + WaveStep × WaveIndex)
```

### 9.3 属性叠加规则

| 属性类型 | 叠加方式 | 示例 |
|----------|----------|------|
| AdditiveStat | 加法叠加 | 暴击几率: 5% + 10% = 15% |
| MultiplicativeStat | 乘法叠加 | 攻击力: 1.0 × 1.2 × 1.1 = 1.32 |

---

## 十、后续建议

### 10.1 配置加载器实现

建议创建 `ConfigLoader.cs` 类来解析JSON并应用到游戏中：

```csharp
public class ConfigLoader : MonoBehaviour
{
    public static GameConfig LoadConfig(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameConfig>(json);
    }
    
    public void ApplyConfig(GameConfig config)
    {
        // 应用玩家配置
        // 应用敌人配置
        // 应用能力配置
        // ...
    }
}
```

### 10.2 热更新支持

- 将配置文件放到服务器，实现远程配置更新
- 使用版本号管理配置变更
- 支持增量更新

### 10.3 Excel转换工具

- 使用工具将JSON转换为Excel表格，方便策划编辑
- 支持Excel导出为JSON
- 数据验证和格式检查

### 10.4 版本管理

- 对配置文件进行版本控制
- 记录每次数值调整的原因
- 支持配置回滚

---

## 附录：配置文件位置

- **JSON配置文件**: `Assets/The Archer/Config/game_config.json`
- **分析报告**: `Assets/The Archer/Config/ECS_Architecture_Analysis_Report.md`

---

*报告生成时间: 2025-01-29*  
*分析工具: Aone Copilot*

# 🎮 游戏模板化核心设计文档

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **核心理念**: 一套代码，无限玩法

---

## 一、核心思想

### 1.1 传统游戏 vs 模板化游戏

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           传统游戏开发模式                                       │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  代码 + 资源 + 配置 = 一个固定的游戏                                            │
│                                                                                  │
│  修改任何内容 → 重新编译 → 重新发布 → 用户重新下载                              │
│                                                                                  │
│  问题:                                                                           │
│  • 每个游戏都是独立项目，开发成本高                                             │
│  • 修改需要重新编译发布，迭代慢                                                 │
│  • 无法复用已验证的游戏框架                                                     │
│  • 创作门槛高，需要专业开发团队                                                 │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘

                                    ↓ 转变 ↓

┌─────────────────────────────────────────────────────────────────────────────────┐
│                           模板化游戏开发模式                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  游戏模板 (固定) + 资源包 (可变) = 无限种游戏                                   │
│                                                                                  │
│  游戏模板: 引擎能力 + 通用框架 + 运行时                                         │
│  资源包:   配置 + 资源 + 脚本 → 定义具体玩法                                    │
│                                                                                  │
│  优势:                                                                           │
│  • 一次开发，无限复用                                                           │
│  • 热更新，无需重新发布客户端                                                   │
│  • 降低创作门槛，非程序员也能创作                                               │
│  • AI 可以辅助生成内容                                                          │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 1.2 核心公式

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           模板化核心公式                                         │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  游戏模板 = 引擎能力 (固定) + 框架逻辑 (固定) + Meta Data (定义可变范围)        │
│                                                                                  │
│  具体游戏 = 游戏模板 + 资源包 (配置 + 资源 + 脚本)                              │
│                                                                                  │
│  无限游戏 = 一个模板 × 无限资源包                                               │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 二、三层架构设计

### 2.1 架构总览

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           游戏模板化三层架构                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  Layer 1: 游戏引擎层 (Game Engine Layer)                                 │   │
│  │  ═══════════════════════════════════════════════════════════════════════│   │
│  │                                                                          │   │
│  │  职责: 提供底层能力，完全固定，不可修改                                  │   │
│  │                                                                          │   │
│  │  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐           │   │
│  │  │ 渲染系统   │ │ 物理系统   │ │ 音频系统   │ │ 输入系统   │           │   │
│  │  └────────────┘ └────────────┘ └────────────┘ └────────────┘           │   │
│  │  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐           │   │
│  │  │ 资源加载   │ │ 对象池     │ │ 网络通信   │ │ 存储系统   │           │   │
│  │  └────────────┘ └────────────┘ └────────────┘ └────────────┘           │   │
│  │                                                                          │   │
│  │  实现: Unity C# (编译为 WebGL/Native)                                    │   │
│  │  特点: 编译时固定，运行时不可变                                          │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         │ 提供 API                              │
│                                         ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  Layer 2: 游戏框架层 (Game Framework Layer)                              │   │
│  │  ═══════════════════════════════════════════════════════════════════════│   │
│  │                                                                          │   │
│  │  职责: 提供游戏类型的通用框架，按模板类型固定                            │   │
│  │                                                                          │   │
│  │  包含:                                                                   │   │
│  │  • C# 桥接层 (Bridge Layer)                                              │   │
│  │  • Luau 运行时 (Script Runtime)                                          │   │
│  │  • 系统框架 (System Frameworks)                                          │   │
│  │                                                                          │   │
│  │  实现: C# 桥接 + Luau 运行时                                             │   │
│  │  特点: 定义游戏类型的通用逻辑框架                                        │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         │ 加载并执行                            │
│                                         ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  Layer 3: 游戏内容层 (Game Content Layer)                                │   │
│  │  ═══════════════════════════════════════════════════════════════════════│   │
│  │                                                                          │   │
│  │  职责: 定义具体游戏内容，完全可变，支持热更新                            │   │
│  │                                                                          │   │
│  │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐                     │   │
│  │  │ 配置数据     │ │ 美术资源     │ │ 游戏脚本     │                     │   │
│  │  │ (JSON)       │ │ (PNG/MP3)    │ │ (Lua)        │                     │   │
│  │  └──────────────┘ └──────────────┘ └──────────────┘                     │   │
│  │                                                                          │   │
│  │  实现: JSON 配置 + 图片/音频资源 + Lua 脚本                              │   │
│  │  特点: 运行时动态加载，支持热更新                                        │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 2.2 各层详细说明

#### Layer 1: 游戏引擎层

| 系统 | 职责 | 实现 |
|------|------|------|
| **渲染系统** | 2D/3D 渲染、Sprite 显示、粒子效果 | Unity SpriteRenderer, ParticleSystem |
| **物理系统** | 碰撞检测、刚体物理 | Unity Physics2D |
| **音频系统** | 音效播放、背景音乐 | Unity AudioSource |
| **输入系统** | 触摸、键盘、手柄输入 | Unity Input System |
| **资源加载** | 动态加载远程资源 | UnityWebRequest + 自定义加载器 |
| **对象池** | 对象复用、性能优化 | 自定义 ObjectPool |
| **网络通信** | HTTP 请求、WebSocket | UnityWebRequest |
| **存储系统** | 本地存储、云存储 | PlayerPrefs + 云端 API |

#### Layer 2: 游戏框架层

**肉鸽模板框架示例:**

| 系统 | 职责 | 接口 |
|------|------|------|
| **关卡系统** | 管理关卡、房间、波次 | StageManager, RoomManager, WaveManager |
| **玩家系统** | 玩家实体、属性、状态 | PlayerEntity, PlayerStats, PlayerInventory |
| **敌人系统** | 敌人实体、AI、生成 | EnemyEntity, EnemyAI, EnemySpawner |
| **能力系统** | 能力、效果、升级 | AbilityManager, EffectSystem, UpgradeSystem |
| **战斗系统** | 伤害计算、投射物、碰撞 | DamageSystem, ProjectileManager, CollisionHandler |

#### Layer 3: 游戏内容层

| 内容类型 | 格式 | 说明 |
|----------|------|------|
| **配置数据** | JSON | 玩家属性、敌人配置、能力数值、关卡设计 |
| **美术资源** | PNG/GIF/MP3 | 角色图集、敌人图集、技能特效、场景背景、音效音乐 |
| **游戏脚本** | Lua | 玩家控制、敌人 AI、技能效果、关卡逻辑 |

---

## 三、Meta Data 驱动设计

### 3.1 核心思想

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           Meta Data 驱动设计                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  核心思想: 模板定义 "能做什么"，资源包定义 "具体做什么"                         │
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         模板 Meta Data                                   │   │
│  │                                                                          │   │
│  │  定义:                                                                   │   │
│  │  • 有哪些可配置的系统 (玩家、敌人、能力、关卡...)                        │   │
│  │  • 每个系统有哪些可配置的属性                                            │   │
│  │  • 每个属性的类型、范围、默认值                                          │   │
│  │  • 有哪些可替换的资源类型                                                │   │
│  │  • 有哪些可扩展的脚本接口                                                │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         编辑器动态生成 UI                                │   │
│  │                                                                          │   │
│  │  Meta Data → 解析 → 动态生成配置编辑界面                                 │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         资源包 (用户创作)                                │   │
│  │                                                                          │   │
│  │  用户修改的配置 + 用户上传的资源 + 模板默认脚本                          │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 3.2 Meta Data 结构定义

```json
{
  "template_id": "roguelike_archer",
  "template_name": "弓箭传说类肉鸽",
  "template_version": "1.0.0",
  "template_description": "经典的肉鸽射击游戏模板，支持自定义角色、敌人、能力和关卡",
  
  "systems": {
    "player": {
      "name": "玩家系统",
      "icon": "person",
      "description": "配置玩家的基础属性",
      "properties": {
        "base_hp": {
          "name": "基础生命值",
          "type": "number",
          "default": 100,
          "min": 50,
          "max": 500,
          "step": 10,
          "description": "玩家的初始生命值"
        },
        "base_damage": {
          "name": "基础攻击力",
          "type": "number",
          "default": 10,
          "min": 5,
          "max": 50,
          "step": 1,
          "description": "玩家的基础攻击伤害"
        },
        "move_speed": {
          "name": "移动速度",
          "type": "number",
          "default": 5.0,
          "min": 2.0,
          "max": 10.0,
          "step": 0.5,
          "description": "玩家的移动速度"
        },
        "attack_speed": {
          "name": "攻击速度",
          "type": "number",
          "default": 1.0,
          "min": 0.5,
          "max": 3.0,
          "step": 0.1,
          "description": "每秒攻击次数"
        },
        "crit_chance": {
          "name": "暴击率",
          "type": "percentage",
          "default": 0.05,
          "min": 0,
          "max": 0.5,
          "description": "暴击触发概率"
        },
        "crit_multiplier": {
          "name": "暴击倍率",
          "type": "number",
          "default": 2.0,
          "min": 1.5,
          "max": 5.0,
          "step": 0.1,
          "description": "暴击伤害倍率"
        }
      },
      "resources": {
        "sprite": {
          "name": "玩家立绘",
          "type": "sprite",
          "category": "characters",
          "default": "player_default",
          "description": "玩家角色的图片资源"
        },
        "animations": {
          "name": "玩家动画",
          "type": "animation_set",
          "required": ["idle", "walk", "attack", "hurt", "death"],
          "description": "玩家角色的动画集"
        }
      }
    },
    
    "enemies": {
      "name": "敌人系统",
      "icon": "bug_report",
      "description": "配置敌人类型和属性",
      "is_array": true,
      "item_template": {
        "id": {
          "type": "string",
          "auto_generate": true,
          "description": "敌人唯一标识"
        },
        "name": {
          "type": "string",
          "required": true,
          "description": "敌人名称"
        },
        "base_health": {
          "type": "number",
          "default": 50,
          "min": 10,
          "max": 1000,
          "description": "基础生命值"
        },
        "base_damage": {
          "type": "number",
          "default": 10,
          "min": 1,
          "max": 100,
          "description": "基础攻击力"
        },
        "move_speed": {
          "type": "number",
          "default": 2.0,
          "min": 0.5,
          "max": 8.0,
          "description": "移动速度"
        },
        "attack_delay": {
          "type": "number",
          "default": 1.5,
          "min": 0.5,
          "max": 5.0,
          "description": "攻击间隔"
        },
        "experience_reward": {
          "type": "number",
          "default": 10,
          "min": 1,
          "max": 100,
          "description": "击杀经验奖励"
        },
        "gold_reward": {
          "type": "number",
          "default": 5,
          "min": 0,
          "max": 50,
          "description": "击杀金币奖励"
        },
        "sprite": {
          "type": "sprite",
          "category": "enemies",
          "description": "敌人图片资源"
        },
        "ai_type": {
          "type": "enum",
          "options": ["chase", "ranged", "patrol", "boss"],
          "default": "chase",
          "description": "AI 行为类型"
        }
      }
    },
    
    "abilities": {
      "name": "能力系统",
      "icon": "auto_awesome",
      "description": "配置可解锁的能力",
      "is_array": true,
      "item_template": {
        "id": {
          "type": "number",
          "auto_generate": true,
          "description": "能力唯一标识"
        },
        "name": {
          "type": "string",
          "required": true,
          "description": "能力名称"
        },
        "description": {
          "type": "string",
          "description": "能力描述"
        },
        "category": {
          "type": "enum",
          "options": ["offensive", "defensive", "utility", "elemental"],
          "default": "offensive",
          "description": "能力类别"
        },
        "rarity": {
          "type": "enum",
          "options": ["common", "rare", "epic", "legendary"],
          "default": "common",
          "description": "稀有度"
        },
        "max_level": {
          "type": "number",
          "default": 5,
          "min": 1,
          "max": 10,
          "description": "最大等级"
        },
        "icon": {
          "type": "sprite",
          "category": "icons",
          "description": "能力图标"
        },
        "levels": {
          "type": "array",
          "description": "各等级效果配置",
          "item_template": {
            "level": { "type": "number" },
            "effect_value": { "type": "number" },
            "description": { "type": "string" }
          }
        }
      }
    },
    
    "stages": {
      "name": "关卡系统",
      "icon": "map",
      "description": "配置关卡和难度",
      "is_array": true,
      "item_template": {
        "id": {
          "type": "number",
          "auto_generate": true,
          "description": "关卡唯一标识"
        },
        "name": {
          "type": "string",
          "required": true,
          "description": "关卡名称"
        },
        "difficulty": {
          "type": "number",
          "default": 1,
          "min": 1,
          "max": 10,
          "description": "难度等级"
        },
        "enemy_hp_multiplier": {
          "type": "number",
          "default": 1.0,
          "min": 0.5,
          "max": 5.0,
          "description": "敌人生命值倍率"
        },
        "enemy_damage_multiplier": {
          "type": "number",
          "default": 1.0,
          "min": 0.5,
          "max": 5.0,
          "description": "敌人攻击力倍率"
        },
        "wave_count": {
          "type": "number",
          "default": 5,
          "min": 1,
          "max": 20,
          "description": "波次数量"
        },
        "background": {
          "type": "sprite",
          "category": "backgrounds",
          "description": "关卡背景图"
        }
      }
    }
  },
  
  "resource_categories": {
    "characters": {
      "name": "角色资源",
      "description": "玩家和 NPC 角色图片",
      "formats": ["png", "gif"],
      "max_size": "512x512",
      "ai_generatable": true
    },
    "enemies": {
      "name": "敌人资源",
      "description": "敌人角色图片",
      "formats": ["png", "gif"],
      "max_size": "256x256",
      "ai_generatable": true
    },
    "icons": {
      "name": "图标资源",
      "description": "能力和物品图标",
      "formats": ["png"],
      "max_size": "128x128",
      "ai_generatable": true
    },
    "backgrounds": {
      "name": "背景资源",
      "description": "关卡背景图片",
      "formats": ["png", "jpg"],
      "max_size": "1920x1080",
      "ai_generatable": true
    },
    "effects": {
      "name": "特效资源",
      "description": "技能和战斗特效",
      "formats": ["png", "gif"],
      "max_size": "256x256",
      "ai_generatable": true
    },
    "audio": {
      "name": "音频资源",
      "description": "音效和背景音乐",
      "formats": ["mp3", "ogg"],
      "max_duration": 30,
      "ai_generatable": false
    }
  },
  
  "scripts": {
    "player_controller": {
      "name": "玩家控制器",
      "file": "player_controller.lua",
      "editable": true,
      "description": "控制玩家移动和攻击逻辑",
      "interfaces": ["OnUpdate", "OnAttack", "OnHurt", "OnDeath"]
    },
    "enemy_ai": {
      "name": "敌人 AI",
      "file": "enemy_ai.lua",
      "editable": true,
      "description": "控制敌人行为逻辑",
      "interfaces": ["OnUpdate", "OnSpawn", "OnAttack", "OnDeath"]
    },
    "ability_effects": {
      "name": "能力效果",
      "file": "ability_effects.lua",
      "editable": true,
      "description": "定义能力的具体效果",
      "interfaces": ["OnActivate", "OnDeactivate", "OnLevelUp"]
    },
    "stage_logic": {
      "name": "关卡逻辑",
      "file": "stage_logic.lua",
      "editable": true,
      "description": "控制关卡流程和事件",
      "interfaces": ["OnStageStart", "OnWaveStart", "OnWaveComplete", "OnStageComplete"]
    }
  },
  
  "default_package": {
    "name": "默认资源包",
    "launcher_key": "default",
    "description": "模板自带的默认游戏内容"
  }
}
```

### 3.3 属性类型定义

| 类型 | 说明 | UI 组件 | 示例 |
|------|------|---------|------|
| `number` | 数值类型 | Slider + Input | 生命值、攻击力 |
| `string` | 字符串类型 | TextField | 名称、描述 |
| `boolean` | 布尔类型 | Switch | 是否启用 |
| `percentage` | 百分比类型 | Slider (0-100%) | 暴击率、闪避率 |
| `enum` | 枚举类型 | Dropdown | AI 类型、稀有度 |
| `array` | 数组类型 | List Editor | 等级配置列表 |
| `object` | 对象类型 | Nested Form | 复杂配置 |
| `sprite` | 图片资源 | Image Picker | 角色图片 |
| `audio` | 音频资源 | Audio Picker | 音效文件 |
| `animation_set` | 动画集 | Animation Editor | 角色动画 |

---

## 四、资源包结构

### 4.1 资源包目录结构

```
package/
├── manifest.json           # 资源包清单
├── game_config.json        # 游戏配置
├── atlases/                # 图集资源
│   ├── characters.atlas.png
│   ├── characters.atlas.json
│   ├── enemies.atlas.png
│   ├── enemies.atlas.json
│   ├── icons.atlas.png
│   ├── icons.atlas.json
│   └── effects.atlas.png
├── audio/                  # 音频资源
│   ├── bgm/
│   │   └── main_theme.mp3
│   └── sfx/
│       ├── attack.mp3
│       ├── hit.mp3
│       └── levelup.mp3
└── scripts/                # Lua 脚本
    ├── player_controller.lua
    ├── enemy_ai.lua
    ├── ability_effects.lua
    └── stage_logic.lua
```

### 4.2 manifest.json 结构

```json
{
  "manifest_version": "1.0",
  "package_info": {
    "id": "pkg_1234567890",
    "name": "太空生存者",
    "description": "太空主题的肉鸽射击游戏",
    "version": "1.0.0",
    "author": "creator_001",
    "created_at": "2025-01-29T10:00:00Z",
    "updated_at": "2025-01-29T10:00:00Z"
  },
  "template": {
    "id": "roguelike_archer",
    "version": "1.0.0"
  },
  "launcher_key": "creator_001-space_survivor-1.0.0-abc123",
  "content": {
    "config": {
      "game_config": "game_config.json"
    },
    "atlases": [
      {
        "name": "characters",
        "image": "atlases/characters.atlas.png",
        "data": "atlases/characters.atlas.json"
      },
      {
        "name": "enemies",
        "image": "atlases/enemies.atlas.png",
        "data": "atlases/enemies.atlas.json"
      },
      {
        "name": "icons",
        "image": "atlases/icons.atlas.png",
        "data": "atlases/icons.atlas.json"
      }
    ],
    "audio": {
      "bgm": ["audio/bgm/main_theme.mp3"],
      "sfx": ["audio/sfx/attack.mp3", "audio/sfx/hit.mp3", "audio/sfx/levelup.mp3"]
    },
    "scripts": [
      "scripts/player_controller.lua",
      "scripts/enemy_ai.lua",
      "scripts/ability_effects.lua",
      "scripts/stage_logic.lua"
    ]
  },
  "checksum": "sha256:abc123..."
}
```

---

## 五、运行时架构

### 5.1 游戏启动流程

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           游戏启动流程                                           │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  Step 1: 解析 Launcher Key                                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  launcher_key: "creator_001-space_survivor-1.0.0-abc123"                 │   │
│  │                                                                          │   │
│  │  解析结果:                                                               │   │
│  │  • creator_id: creator_001                                               │   │
│  │  • package_id: space_survivor                                            │   │
│  │  • version: 1.0.0                                                        │   │
│  │  • checksum: abc123                                                      │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 2: 获取资源包 URL                                                          │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  请求: GET /api/packages/{launcher_key}                                  │   │
│  │  响应: { "cdn_url": "https://cdn.example.com/packages/xxx.zip" }         │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 3: 下载资源包                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 检查本地缓存                                                          │   │
│  │  • 如果缓存有效，直接使用                                                │   │
│  │  • 如果缓存无效，从 CDN 下载                                             │   │
│  │  • 验证 checksum                                                         │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 4: 加载资源                                                                │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 解析 manifest.json                                                    │   │
│  │  • 加载 game_config.json                                                 │   │
│  │  • 加载图集资源                                                          │   │
│  │  • 加载音频资源                                                          │   │
│  │  • 加载 Lua 脚本                                                         │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 5: 初始化游戏                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 初始化 Luau 运行时                                                    │   │
│  │  • 注册 C# 桥接函数                                                      │   │
│  │  • 执行 Lua 脚本                                                         │   │
│  │  • 创建游戏实体                                                          │   │
│  │  • 开始游戏循环                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Luau 运行时架构

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           Luau 运行时架构                                        │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         Unity C# 层                                      │   │
│  │                                                                          │   │
│  │  ┌──────────────────────────────────────────────────────────────────┐  │   │
│  │  │                    C# 桥接层 (Bridge Layer)                       │  │   │
│  │  │                                                                   │  │   │
│  │  │  // 注册到 Lua 的函数                                             │  │   │
│  │  │  EntityBridge.CreateEntity(type, x, y)                            │  │   │
│  │  │  EntityBridge.DestroyEntity(id)                                   │  │   │
│  │  │  EntityBridge.Move(id, vx, vy)                                    │  │   │
│  │  │  EntityBridge.PlayAnimation(id, name)                             │  │   │
│  │  │  ResourceBridge.GetSprite(atlasName, spriteName)                  │  │   │
│  │  │  AudioBridge.PlaySound(soundName)                                 │  │   │
│  │  │  InputBridge.GetMoveInput()                                       │  │   │
│  │  │  ConfigBridge.GetConfig(path)                                     │  │   │
│  │  │                                                                   │  │   │
│  │  └──────────────────────────────────────────────────────────────────┘  │   │
│  │                                    │                                    │   │
│  │                                    │ 调用                               │   │
│  │                                    ▼                                    │   │
│  │  ┌──────────────────────────────────────────────────────────────────┐  │   │
│  │  │                    Luau 运行时                                    │  │   │
│  │  │                                                                   │  │   │
│  │  │  • Native: Luau C++ 库                                            │  │   │
│  │  │  • WebGL: luau.wasm (Emscripten 编译)                             │  │   │
│  │  │                                                                   │  │   │
│  │  └──────────────────────────────────────────────────────────────────┘  │   │
│  │                                    │                                    │   │
│  │                                    │ 执行                               │   │
│  │                                    ▼                                    │   │
│  │  ┌──────────────────────────────────────────────────────────────────┐  │   │
│  │  │                    Lua 脚本 (文本文件)                            │  │   │
│  │  │                                                                   │  │   │
│  │  │  -- player_controller.lua                                         │  │   │
│  │  │  local Player = {}                                                │  │   │
│  │  │                                                                   │  │   │
│  │  │  function Player:Update(dt)                                       │  │   │
│  │  │      local x, y = Input.GetMoveInput()                            │  │   │
│  │  │      if x ~= 0 or y ~= 0 then                                     │  │   │
│  │  │          Entity.Move(self.id, x * self.speed, y * self.speed)     │  │   │
│  │  │          Entity.PlayAnimation(self.id, "walk")                    │  │   │
│  │  │      else                                                         │  │   │
│  │  │          Entity.PlayAnimation(self.id, "idle")                    │  │   │
│  │  │      end                                                          │  │   │
│  │  │  end                                                              │  │   │
│  │  │                                                                   │  │   │
│  │  │  return Player                                                    │  │   │
│  │  │                                                                   │  │   │
│  │  └──────────────────────────────────────────────────────────────────┘  │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 六、模板化工作流

### 6.1 完整工作流

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           模板化完整工作流                                       │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  Phase 1: 模板开发 (开发团队)                                                    │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                                                                          │   │
│  │  1. 开发游戏原型 (Unity + C#)                                            │   │
│  │     • 实现完整的游戏功能                                                 │   │
│  │     • 验证玩法可行性                                                     │   │
│  │     ↓                                                                    │   │
│  │  2. 识别可配置点                                                         │   │
│  │     • 分析哪些内容可以让用户修改                                         │   │
│  │     • 定义配置的类型和范围                                               │   │
│  │     ↓                                                                    │   │
│  │  3. 分离引擎层和逻辑层                                                   │   │
│  │     • 引擎层: C# 保留 (渲染、物理、音频等)                               │   │
│  │     • 逻辑层: 迁移到 Lua (玩家控制、敌人AI等)                            │   │
│  │     ↓                                                                    │   │
│  │  4. 设计 Meta Data Schema                                                │   │
│  │     • 定义所有可配置的系统和属性                                         │   │
│  │     • 定义资源类型和脚本接口                                             │   │
│  │     ↓                                                                    │   │
│  │  5. 创建默认资源包                                                       │   │
│  │     • 提供默认配置                                                       │   │
│  │     • 提供默认资源                                                       │   │
│  │     • 提供默认脚本                                                       │   │
│  │     ↓                                                                    │   │
│  │  6. 发布模板                                                             │   │
│  │     • 发布游戏客户端 (App Store / Google Play / Web)                     │   │
│  │     • 发布模板 Meta Data                                                 │   │
│  │     • 发布默认资源包                                                     │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Phase 2: 内容创作 (创作者)                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                                                                          │   │
│  │  1. 选择模板                                                             │   │
│  │     • 浏览可用模板                                                       │   │
│  │     • 选择适合的游戏类型                                                 │   │
│  │     ↓                                                                    │   │
│  │  2. 编辑器加载 Meta Data                                                 │   │
│  │     • 解析模板 Meta Data                                                 │   │
│  │     • 动态生成配置编辑 UI                                                │   │
│  │     ↓                                                                    │   │
│  │  3. 修改配置                                                             │   │
│  │     • 调整数值 (生命值、攻击力等)                                        │   │
│  │     • 添加/修改敌人、能力、关卡                                          │   │
│  │     ↓                                                                    │   │
│  │  4. 替换资源                                                             │   │
│  │     • 上传自己的图片资源                                                 │   │
│  │     • 使用 AI 生成资源                                                   │   │
│  │     • 使用 AI 优化资源                                                   │   │
│  │     ↓                                                                    │   │
│  │  5. (可选) 修改脚本                                                      │   │
│  │     • 仅 Mac 版编辑器支持                                                │   │
│  │     • 修改游戏逻辑                                                       │   │
│  │     ↓                                                                    │   │
│  │  6. 预览测试                                                             │   │
│  │     • 实时预览修改效果                                                   │   │
│  │     • 测试游戏玩法                                                       │   │
│  │     ↓                                                                    │   │
│  │  7. 发布资源包                                                           │   │
│  │     • 资源合并打包                                                       │   │
│  │     • 上传到 CDN                                                         │   │
│  │     • 生成 launcher_key                                                  │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Phase 3: 游戏运行 (玩家)                                                        │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                                                                          │   │
│  │  1. 获取 launcher_key                                                    │   │
│  │     • 通过分享链接                                                       │   │
│  │     • 通过游戏市场                                                       │   │
│  │     ↓                                                                    │   │
│  │  2. 启动游戏                                                             │   │
│  │     • 输入 launcher_key                                                  │   │
│  │     • 或扫描二维码                                                       │   │
│  │     ↓                                                                    │   │
│  │  3. 下载资源包                                                           │   │
│  │     • 从 CDN 下载                                                        │   │
│  │     • 本地缓存                                                           │   │
│  │     ↓                                                                    │   │
│  │  4. 运行游戏                                                             │   │
│  │     • 加载配置和资源                                                     │   │
│  │     • 执行 Lua 脚本                                                      │   │
│  │     • 开始游戏                                                           │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 七、成功关键因素

### 7.1 关键因素清单

| 因素 | 说明 | 重要性 |
|------|------|--------|
| **清晰的分层** | 引擎层、框架层、内容层职责明确，互不干扰 | ⭐⭐⭐⭐⭐ |
| **完善的 Meta Data** | 定义所有可配置点、类型、范围、默认值 | ⭐⭐⭐⭐⭐ |
| **稳定的桥接层** | C# ↔ Lua 接口稳定可靠，向后兼容 | ⭐⭐⭐⭐⭐ |
| **灵活的脚本系统** | Lua 脚本可以覆盖默认行为，扩展功能 | ⭐⭐⭐⭐ |
| **高效的资源加载** | 动态加载远程资源，支持增量更新 | ⭐⭐⭐⭐ |
| **友好的编辑器** | 基于 Meta Data 动态生成 UI，易于使用 | ⭐⭐⭐⭐ |
| **AI 辅助创作** | AI 生成资源、优化配置、自然语言交互 | ⭐⭐⭐ |

### 7.2 设计原则

1. **配置优于代码** - 能用配置解决的，不要写代码
2. **约定优于配置** - 提供合理的默认值，减少配置负担
3. **渐进式复杂度** - 简单需求简单实现，复杂需求可扩展
4. **向后兼容** - 新版本模板兼容旧版本资源包
5. **安全沙箱** - Lua 脚本在沙箱中运行，无法访问系统资源

---

## 八、总结

### 8.1 核心价值

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           游戏模板化核心价值                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  对开发团队:                                                                     │
│  ✅ 一次开发，无限复用                                                          │
│  ✅ 降低维护成本                                                                │
│  ✅ 快速迭代验证                                                                │
│                                                                                  │
│  对创作者:                                                                       │
│  ✅ 无需编程即可创作游戏                                                        │
│  ✅ AI 辅助降低创作门槛                                                         │
│  ✅ 快速发布和迭代                                                              │
│                                                                                  │
│  对玩家:                                                                         │
│  ✅ 更多游戏选择                                                                │
│  ✅ 更快的内容更新                                                              │
│  ✅ 个性化游戏体验                                                              │
│                                                                                  │
│  对平台:                                                                         │
│  ✅ 丰富的内容生态                                                              │
│  ✅ 低成本扩展游戏库                                                            │
│  ✅ 用户粘性提升                                                                │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 8.2 最终公式

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           模板化最终公式                                         │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  游戏模板 = 引擎能力 (固定) + 框架逻辑 (固定) + Meta Data (定义可变范围)        │
│                                                                                  │
│  具体游戏 = 游戏模板 + 资源包 (配置 + 资源 + 脚本)                              │
│                                                                                  │
│  无限游戏 = 一个模板 × 无限资源包                                               │
│                                                                                  │
│  ═══════════════════════════════════════════════════════════════════════════    │
│                                                                                  │
│  一套代码，无限玩法                                                              │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

**文档结束**

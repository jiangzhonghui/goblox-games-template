# 🤖 AI友好的资源包规范设计

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **目标**: 设计一套分散的、自定义格式的资源包规范，便于 AI Agent 对每个资源进行独立处理和优化

---

## 一、设计理念

### 1.1 核心原则

| 原则 | 说明 |
|------|------|
| **资源分散** | 每个资源独立存储，便于单独处理 |
| **元数据丰富** | 每个资源附带完整的元数据，便于 AI 理解上下文 |
| **格式标准化** | 使用通用格式（PNG、JSON），无需编译 |
| **AI可处理** | 资源格式便于 AI 读取、分析、生成和优化 |
| **版本可追溯** | 支持资源版本管理和变更历史 |

### 1.2 AI Agent 处理能力矩阵

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           AI Agent 资源处理能力                                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                 │
│  │  图像生成 Agent  │  │  图像优化 Agent  │  │  动画生成 Agent  │                 │
│  │                 │  │                 │  │                 │                 │
│  │ • 角色立绘生成   │  │ • 图片放大      │  │ • 帧动画生成    │                 │
│  │ • 图标生成      │  │ • 背景移除      │  │ • 动画优化      │                 │
│  │ • 特效图生成    │  │ • 风格迁移      │  │ • 关键帧插值    │                 │
│  │ • 瓦片图生成    │  │ • 颜色校正      │  │ • 动作平滑      │                 │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘                 │
│                                                                                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                 │
│  │  数值平衡 Agent  │  │  文案生成 Agent  │  │  音频处理 Agent  │                 │
│  │                 │  │                 │  │                 │                 │
│  │ • 属性平衡分析   │  │ • 技能描述生成   │  │ • 音效生成      │                 │
│  │ • 难度曲线优化   │  │ • 物品描述生成   │  │ • 音频优化      │                 │
│  │ • 掉落率优化    │  │ • 剧情文案生成   │  │ • 音量标准化    │                 │
│  │ • 经济系统平衡   │  │ • 本地化翻译    │  │ • 格式转换      │                 │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘                 │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 二、资源包目录结构

### 2.1 完整目录结构

```
package_root/
├── manifest.json                           # 资源包清单
├── game_config.json                        # 游戏主配置
│
├── entities/                               # 实体定义（分散存储）
│   ├── player/                             # 玩家
│   │   ├── player.entity.json              # 玩家实体定义
│   │   ├── player.meta.json                # AI元数据
│   │   └── assets/
│   │       ├── idle.png
│   │       ├── walk_sheet.png
│   │       ├── attack_sheet.png
│   │       └── player.anim.json
│   │
│   ├── enemies/                            # 敌人（每个敌人独立文件夹）
│   │   ├── skeleton/
│   │   │   ├── skeleton.entity.json
│   │   │   ├── skeleton.meta.json
│   │   │   └── assets/
│   │   │       ├── idle.png
│   │   │       ├── walk_sheet.png
│   │   │       ├── attack_sheet.png
│   │   │       └── skeleton.anim.json
│   │   ├── goblin/
│   │   │   └── ...
│   │   └── boss_dragon/
│   │       └── ...
│   │
│   └── projectiles/                        # 投射物
│       ├── arrow/
│       │   ├── arrow.entity.json
│       │   ├── arrow.meta.json
│       │   └── assets/
│       │       ├── arrow.png
│       │       └── arrow.anim.json
│       └── fireball/
│           └── ...
│
├── abilities/                              # 能力（每个能力独立文件夹）
│   ├── bouncy_shot/
│   │   ├── bouncy_shot.ability.json
│   │   ├── bouncy_shot.meta.json
│   │   └── assets/
│   │       ├── icon.png
│   │       └── effect.particle.json
│   ├── freeze/
│   │   └── ...
│   └── fire_spirit/
│       └── ...
│
├── items/                                  # 物品（每个物品独立文件夹）
│   ├── weapons/
│   │   ├── iron_bow/
│   │   │   ├── iron_bow.item.json
│   │   │   ├── iron_bow.meta.json
│   │   │   └── assets/
│   │   │       ├── icon.png
│   │   │       ├── weapon.png
│   │   │       └── projectile.png
│   │   └── crossbow/
│   │       └── ...
│   ├── armors/
│   │   └── ...
│   └── accessories/
│       └── ...
│
├── stages/                                 # 关卡（每个关卡独立文件夹）
│   ├── stage_01/
│   │   ├── stage_01.stage.json
│   │   ├── stage_01.meta.json
│   │   └── rooms/
│   │       ├── room_01/
│   │       │   ├── room_01.room.json
│   │       │   └── assets/
│   │       │       ├── tileset.png
│   │       │       └── background.png
│   │       └── room_02/
│   │           └── ...
│   └── stage_02/
│       └── ...
│
├── effects/                                # 特效（每个特效独立文件夹）
│   ├── explosion/
│   │   ├── explosion.effect.json
│   │   ├── explosion.meta.json
│   │   └── assets/
│   │       ├── explosion_sheet.png
│   │       └── explosion.particle.json
│   ├── fire/
│   │   └── ...
│   └── ice/
│       └── ...
│
├── audio/                                  # 音频（分类存储）
│   ├── bgm/
│   │   ├── stage_01/
│   │   │   ├── stage_01_bgm.audio.json
│   │   │   ├── stage_01_bgm.meta.json
│   │   │   └── stage_01_bgm.ogg
│   │   └── boss/
│   │       └── ...
│   └── sfx/
│       ├── bow_shoot/
│       │   ├── bow_shoot.audio.json
│       │   ├── bow_shoot.meta.json
│       │   └── bow_shoot.ogg
│       └── explosion/
│           └── ...
│
├── ui/                                     # UI资源
│   ├── icons/
│   │   └── ...
│   └── backgrounds/
│       └── ...
│
└── localization/                           # 本地化
    ├── en.json
    └── zh.json
```

---

## 三、资源元数据规范 (*.meta.json)

每个资源都附带一个 `.meta.json` 文件，包含 AI 处理所需的完整上下文信息。

### 3.1 通用元数据结构

```json
{
  "meta_version": "1.0",
  "resource_id": "enemy_skeleton",
  "resource_type": "entity",
  "resource_subtype": "enemy",
  
  "creation_info": {
    "created_at": "2025-01-29T10:00:00Z",
    "created_by": "user_001",
    "creation_method": "ai_generated",
    "ai_model": "stable-diffusion-xl",
    "ai_prompt": "A skeletal warrior with glowing red eyes, pixel art style, 64x64"
  },
  
  "modification_history": [
    {
      "modified_at": "2025-01-29T12:00:00Z",
      "modified_by": "ai_agent_image_optimizer",
      "modification_type": "upscale",
      "details": "Upscaled from 64x64 to 128x128 using Real-ESRGAN"
    },
    {
      "modified_at": "2025-01-29T14:00:00Z",
      "modified_by": "user_001",
      "modification_type": "manual_edit",
      "details": "Adjusted color palette"
    }
  ],
  
  "ai_context": {
    "description": "A skeletal undead warrior, common enemy in dungeon stages",
    "visual_style": "pixel_art",
    "color_palette": ["#FFFFFF", "#808080", "#FF0000", "#000000"],
    "theme": "dark_fantasy",
    "difficulty_tier": "easy",
    "tags": ["undead", "skeleton", "melee", "dungeon"]
  },
  
  "ai_processing_hints": {
    "preferred_upscale_method": "pixel_perfect",
    "maintain_pixel_grid": true,
    "color_count_limit": 16,
    "transparency_required": true,
    "animation_style": "frame_by_frame"
  },
  
  "quality_metrics": {
    "resolution": {"width": 128, "height": 128},
    "file_size_kb": 12,
    "color_depth": 32,
    "has_transparency": true,
    "animation_frames": 24,
    "ai_quality_score": 0.85
  },
  
  "relationships": {
    "belongs_to_stage": ["stage_01", "stage_02"],
    "drops_items": ["gold", "bone"],
    "weak_to_abilities": ["fire", "holy"],
    "spawns_with": ["skeleton_archer"]
  }
}
```

### 3.2 图像资源元数据

```json
{
  "meta_version": "1.0",
  "resource_id": "skeleton_idle",
  "resource_type": "sprite",
  
  "image_info": {
    "format": "png",
    "width": 128,
    "height": 128,
    "color_mode": "RGBA",
    "bit_depth": 8,
    "has_alpha": true,
    "file_size_bytes": 12456
  },
  
  "sprite_info": {
    "sprite_type": "character",
    "is_sprite_sheet": false,
    "pivot": {"x": 0.5, "y": 0.0},
    "pixels_per_unit": 100
  },
  
  "ai_generation_params": {
    "model": "stable-diffusion-xl",
    "prompt": "skeleton warrior idle pose, pixel art, 128x128, transparent background",
    "negative_prompt": "blurry, low quality, 3d render",
    "seed": 12345678,
    "steps": 30,
    "cfg_scale": 7.5,
    "sampler": "euler_a"
  },
  
  "ai_optimization_status": {
    "upscaled": true,
    "upscale_factor": 2,
    "upscale_method": "Real-ESRGAN-x4plus-anime",
    "background_removed": true,
    "color_corrected": false,
    "noise_reduced": true
  },
  
  "ai_analysis": {
    "detected_objects": ["skeleton", "sword", "shield"],
    "dominant_colors": ["#E0E0E0", "#808080", "#400000"],
    "style_classification": "pixel_art",
    "quality_assessment": {
      "sharpness": 0.92,
      "color_consistency": 0.88,
      "edge_clarity": 0.95,
      "overall_score": 0.91
    }
  }
}
```

### 3.3 动画资源元数据

```json
{
  "meta_version": "1.0",
  "resource_id": "skeleton_walk",
  "resource_type": "animation",
  
  "animation_info": {
    "animation_type": "sprite_sheet",
    "total_frames": 6,
    "frame_width": 128,
    "frame_height": 128,
    "columns": 6,
    "rows": 1,
    "duration_seconds": 0.6,
    "loop": true
  },
  
  "ai_generation_params": {
    "model": "animate-diff",
    "base_image": "skeleton_idle.png",
    "motion_prompt": "walking cycle, side view, smooth movement",
    "frame_count": 6,
    "interpolation_method": "linear"
  },
  
  "ai_optimization_hints": {
    "smooth_transitions": true,
    "maintain_volume": true,
    "foot_contact_frames": [2, 5],
    "key_poses": [0, 3],
    "easing_curve": "ease_in_out"
  },
  
  "ai_analysis": {
    "motion_smoothness": 0.87,
    "loop_seamlessness": 0.95,
    "timing_consistency": 0.90,
    "silhouette_clarity": 0.88
  }
}
```

---

## 四、实体定义规范 (*.entity.json)

### 4.1 敌人实体定义

```json
{
  "entity_version": "1.0",
  "entity_id": "skeleton",
  "entity_type": "enemy",
  "entity_name": "Skeleton Warrior",
  
  "display": {
    "name_key": "enemy.skeleton.name",
    "description_key": "enemy.skeleton.desc"
  },
  
  "stats": {
    "base_health": 30,
    "base_damage": 8,
    "movement_speed": 2.5,
    "attack_speed": 1.0,
    "attack_range": 1.5,
    "knockback_resistance": 0.2,
    "collision_damage_multiplier": 0.5
  },
  
  "behavior": {
    "ai_type": "melee_chase",
    "detection_range": 8.0,
    "attack_cooldown": 1.5,
    "patrol_enabled": true,
    "patrol_radius": 3.0
  },
  
  "drops": [
    {"type": "experience", "amount": {"min": 5, "max": 10}, "chance": 100},
    {"type": "gold", "amount": {"min": 1, "max": 5}, "chance": 50},
    {"type": "item", "item_id": "bone", "chance": 10}
  ],
  
  "assets": {
    "sprites": {
      "idle": "assets/idle.png",
      "walk_sheet": "assets/walk_sheet.png",
      "attack_sheet": "assets/attack_sheet.png",
      "hurt": "assets/hurt.png",
      "death_sheet": "assets/death_sheet.png"
    },
    "animation": "assets/skeleton.anim.json",
    "audio": {
      "spawn": "audio/sfx/skeleton_spawn.ogg",
      "attack": "audio/sfx/skeleton_attack.ogg",
      "hurt": "audio/sfx/skeleton_hurt.ogg",
      "death": "audio/sfx/skeleton_death.ogg"
    },
    "effects": {
      "death": "effects/bone_scatter/bone_scatter.effect.json"
    }
  },
  
  "collision": {
    "type": "box",
    "offset": {"x": 0, "y": 16},
    "size": {"x": 24, "y": 40}
  },
  
  "scaling": {
    "health_per_stage": 1.2,
    "damage_per_stage": 1.15,
    "speed_per_stage": 1.05
  }
}
```

### 4.2 能力定义

```json
{
  "ability_version": "1.0",
  "ability_id": "freeze",
  "ability_type": "elemental",
  "ability_name": "Freeze",
  
  "display": {
    "name_key": "ability.freeze.name",
    "description_key": "ability.freeze.desc",
    "icon": "assets/icon.png"
  },
  
  "rarity": 1,
  "is_repeated": false,
  "is_endgame": false,
  "max_level": 5,
  "prerequisites": [],
  
  "levels": [
    {
      "level": 1,
      "effect_duration": 2.0,
      "arrow_damage_multiplier": 0.8,
      "effect_deals_dot": false
    },
    {
      "level": 2,
      "effect_duration": 2.5,
      "arrow_damage_multiplier": 0.85,
      "effect_deals_dot": false
    },
    {
      "level": 3,
      "effect_duration": 3.0,
      "arrow_damage_multiplier": 0.9,
      "effect_deals_dot": true,
      "effect_damage_multiplier": 0.05,
      "effect_damage_interval": 0.5
    },
    {
      "level": 4,
      "effect_duration": 3.5,
      "arrow_damage_multiplier": 0.95,
      "effect_deals_dot": true,
      "effect_damage_multiplier": 0.08,
      "effect_damage_interval": 0.5
    },
    {
      "level": 5,
      "effect_duration": 4.0,
      "arrow_damage_multiplier": 1.0,
      "effect_deals_dot": true,
      "effect_damage_multiplier": 0.1,
      "effect_damage_interval": 0.5
    }
  ],
  
  "assets": {
    "icon": "assets/icon.png",
    "projectile_effect": "assets/ice_projectile.effect.json",
    "hit_effect": "assets/ice_hit.effect.json",
    "status_effect": "assets/frozen_status.effect.json",
    "audio": {
      "activate": "audio/sfx/freeze_activate.ogg",
      "hit": "audio/sfx/freeze_hit.ogg"
    }
  },
  
  "visual_config": {
    "projectile_tint": "#00BFFF",
    "trail_enabled": true,
    "trail_color": "#87CEEB"
  }
}
```

---

## 五、动画格式规范 (*.anim.json)

### 5.1 精灵动画定义

```json
{
  "animation_version": "1.0",
  "animation_id": "skeleton_animations",
  "entity_id": "skeleton",
  
  "sprite_sheets": {
    "idle": {
      "path": "assets/idle.png",
      "frame_width": 128,
      "frame_height": 128,
      "columns": 1,
      "rows": 1
    },
    "walk": {
      "path": "assets/walk_sheet.png",
      "frame_width": 128,
      "frame_height": 128,
      "columns": 6,
      "rows": 1
    },
    "attack": {
      "path": "assets/attack_sheet.png",
      "frame_width": 128,
      "frame_height": 128,
      "columns": 6,
      "rows": 1
    },
    "hurt": {
      "path": "assets/hurt.png",
      "frame_width": 128,
      "frame_height": 128,
      "columns": 2,
      "rows": 1
    },
    "death": {
      "path": "assets/death_sheet.png",
      "frame_width": 128,
      "frame_height": 128,
      "columns": 5,
      "rows": 1
    }
  },
  
  "animations": {
    "idle": {
      "sprite_sheet": "idle",
      "frames": [0],
      "frame_duration": 1.0,
      "loop": true,
      "events": []
    },
    "walk": {
      "sprite_sheet": "walk",
      "frames": [0, 1, 2, 3, 4, 5],
      "frame_duration": 0.1,
      "loop": true,
      "events": [
        {"frame": 1, "event": "footstep_left"},
        {"frame": 4, "event": "footstep_right"}
      ]
    },
    "attack": {
      "sprite_sheet": "attack",
      "frames": [0, 1, 2, 3, 4, 5],
      "frame_duration": 0.08,
      "loop": false,
      "events": [
        {"frame": 3, "event": "attack_hit"},
        {"frame": 5, "event": "attack_end"}
      ]
    },
    "hurt": {
      "sprite_sheet": "hurt",
      "frames": [0, 1],
      "frame_duration": 0.15,
      "loop": false,
      "events": [
        {"frame": 1, "event": "hurt_end"}
      ]
    },
    "death": {
      "sprite_sheet": "death",
      "frames": [0, 1, 2, 3, 4],
      "frame_duration": 0.12,
      "loop": false,
      "events": [
        {"frame": 4, "event": "death_complete"}
      ]
    }
  },
  
  "state_machine": {
    "default_state": "idle",
    "states": {
      "idle": {
        "animation": "idle",
        "transitions": [
          {"to": "walk", "trigger": "move_start"},
          {"to": "attack", "trigger": "attack"},
          {"to": "hurt", "trigger": "take_damage"},
          {"to": "death", "trigger": "die"}
        ]
      },
      "walk": {
        "animation": "walk",
        "transitions": [
          {"to": "idle", "trigger": "move_stop"},
          {"to": "attack", "trigger": "attack"},
          {"to": "hurt", "trigger": "take_damage"},
          {"to": "death", "trigger": "die"}
        ]
      },
      "attack": {
        "animation": "attack",
        "transitions": [
          {"to": "idle", "trigger": "attack_end"},
          {"to": "hurt", "trigger": "take_damage"},
          {"to": "death", "trigger": "die"}
        ]
      },
      "hurt": {
        "animation": "hurt",
        "transitions": [
          {"to": "idle", "trigger": "hurt_end"},
          {"to": "death", "trigger": "die"}
        ]
      },
      "death": {
        "animation": "death",
        "transitions": []
      }
    }
  },
  
  "render_config": {
    "pivot": {"x": 0.5, "y": 0.0},
    "pixels_per_unit": 100,
    "sorting_layer": "Entities",
    "sorting_order": 0
  }
}
```

---

## 六、粒子特效格式规范 (*.particle.json)

### 6.1 粒子系统定义

```json
{
  "particle_version": "1.0",
  "effect_id": "fire_burst",
  "effect_name": "Fire Burst",
  "effect_type": "burst",
  
  "emitter": {
    "duration": 0.5,
    "loop": false,
    "start_delay": 0,
    "prewarm": false,
    
    "emission": {
      "rate_over_time": 0,
      "bursts": [
        {"time": 0, "count": {"min": 20, "max": 30}, "cycles": 1}
      ]
    },
    
    "shape": {
      "type": "circle",
      "radius": 0.5,
      "arc": 360,
      "emit_from_edge": false,
      "randomize_direction": 1.0
    }
  },
  
  "particle": {
    "lifetime": {"min": 0.3, "max": 0.6},
    
    "sprite": {
      "path": "assets/fire_particle.png",
      "mode": "single",
      "animation": null
    },
    
    "size": {
      "start": {"min": 0.3, "max": 0.5},
      "end": {"min": 0.1, "max": 0.2},
      "curve": "ease_out"
    },
    
    "speed": {
      "start": {"min": 2.0, "max": 4.0},
      "end": {"min": 0.5, "max": 1.0},
      "curve": "linear"
    },
    
    "rotation": {
      "start": {"min": 0, "max": 360},
      "speed": {"min": -90, "max": 90}
    },
    
    "color": {
      "start": {"r": 255, "g": 200, "b": 50, "a": 255},
      "end": {"r": 255, "g": 100, "b": 0, "a": 0},
      "curve": "linear"
    },
    
    "gravity": {"x": 0, "y": -2.0},
    
    "blend_mode": "additive"
  },
  
  "sub_emitters": [
    {
      "trigger": "death",
      "effect_path": "effects/smoke_puff/smoke_puff.particle.json",
      "inherit_velocity": 0.3,
      "probability": 0.5
    }
  ],
  
  "audio": {
    "on_start": "audio/sfx/fire_burst.ogg",
    "volume": 0.8
  },
  
  "render_config": {
    "sorting_layer": "Effects",
    "sorting_order": 100,
    "max_particles": 100
  }
}
```

---

## 七、资源处理与合并工作流

### 7.1 完整工作流概览

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                    资源处理与合并完整工作流                                           │
├─────────────────────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────────────────────┐   │
│  │                              Phase 1: 原始资源 (创作者编辑)                                  │   │
│  │                                                                                              │   │
│  │  source/                          ← 创作者在编辑器中操作的目录                                │   │
│  │  ├── entities/                                                                               │   │
│  │  │   ├── skeleton/                                                                           │   │
│  │  │   │   ├── skeleton.entity.json    ← 实体配置                                              │   │
│  │  │   │   ├── skeleton.meta.json      ← AI元数据                                              │   │
│  │  │   │   └── assets/                                                                         │   │
│  │  │   │       ├── idle.png            ← 单独的原始图片                                        │   │
│  │  │   │       ├── walk_01.png                                                                 │   │
│  │  │   │       ├── walk_02.png                                                                 │   │
│  │  │   │       └── ...                                                                         │   │
│  │  │   └── goblin/                                                                             │   │
│  │  │       └── ...                                                                             │   │
│  │  ├── abilities/                                                                              │   │
│  │  │   ├── freeze/                                                                             │   │
│  │  │   │   ├── freeze.ability.json                                                             │   │
│  │  │   │   └── assets/                                                                         │   │
│  │  │   │       └── icon.png            ← 单独的图标                                            │   │
│  │  │   └── ...                                                                                 │   │
│  │  └── effects/                                                                                │   │
│  │      └── ...                                                                                 │   │
│  │                                                                                              │   │
│  └─────────────────────────────────────────────────────────────────────────────────────────────┘   │
│                                              │                                                      │
│                                              ▼                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────────────────────┐   │
│  │                              Phase 2: AI处理 (自动化)                                        │   │
│  │                                                                                              │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │   │
│  │  │ 图像生成    │  │ 图像优化    │  │ 动画生成    │  │ 文案生成    │  │ 数值平衡    │       │   │
│  │  │ Agent      │  │ Agent      │  │ Agent      │  │ Agent      │  │ Agent      │       │   │
│  │  │            │  │            │  │            │  │            │  │            │       │   │
│  │  │ • 生成图标  │  │ • 放大图片  │  │ • 生成帧动画│  │ • 生成描述  │  │ • 平衡分析  │       │   │
│  │  │ • 生成角色  │  │ • 去背景    │  │ • 插值补帧  │  │ • 本地化    │  │ • 优化建议  │       │   │
│  │  │ • 生成特效  │  │ • 调色      │  │ • 平滑动画  │  │            │  │            │       │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘       │   │
│  │                                                                                              │   │
│  │  处理后资源仍保存在 source/ 目录，更新 *.meta.json 记录处理历史                               │   │
│  │                                                                                              │   │
│  └─────────────────────────────────────────────────────────────────────────────────────────────┘   │
│                                              │                                                      │
│                                              ▼                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────────────────────┐   │
│  │                              Phase 3: 资源合并与打包 (自动化)                                 │   │
│  │                                                                                              │   │
│  │  ┌─────────────────────────────────────────────────────────────────────────────────────┐   │   │
│  │  │                           资源合并器 (Resource Packer)                               │   │   │
│  │  │                                                                                      │   │   │
│  │  │  1. 扫描所有原始资源                                                                  │   │   │
│  │  │  2. 按类型分组 (角色、图标、特效、瓦片等)                                              │   │   │
│  │  │  3. 生成图集 (Texture Atlas)                                                         │   │   │
│  │  │  4. 合并配置JSON                                                                     │   │   │
│  │  │  5. 生成资源映射表                                                                    │   │   │
│  │  │  6. 压缩和优化                                                                        │   │   │
│  │  │                                                                                      │   │   │
│  │  └─────────────────────────────────────────────────────────────────────────────────────┘   │   │
│  │                                                                                              │   │
│  └─────────────────────────────────────────────────────────────────────────────────────────────┘   │
│                                              │                                                      │
│                                              ▼                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────────────────────┐   │
│  │                              Phase 4: 发布包 (客户端加载)                                    │   │
│  │                                                                                              │   │
│  │  build/                           ← 客户端实际加载的目录                                     │   │
│  │  ├── manifest.json                   ← 资源包清单                                           │   │
│  │  ├── game_config.json                ← 合并后的游戏配置                                     │   │
│  │  ├── atlases/                        ← 图集目录                                             │   │
│  │  │   ├── characters.atlas.json       ← 角色图集描述                                         │   │
│  │  │   ├── characters.atlas.png        ← 角色图集图片                                         │   │
│  │  │   ├── icons.atlas.json            ← 图标图集描述                                         │   │
│  │  │   ├── icons.atlas.png             ← 图标图集图片                                         │   │
│  │  │   ├── effects.atlas.json          ← 特效图集描述                                         │   │
│  │  │   ├── effects.atlas.png           ← 特效图集图片                                         │   │
│  │  │   └── tiles.atlas.png             ← 瓦片图集                                             │   │
│  │  ├── animations/                     ← 合并后的动画配置                                     │   │
│  │  │   └── animations.json             ← 所有动画定义                                         │   │
│  │  ├── particles/                      ← 合并后的粒子配置                                     │   │
│  │  │   └── particles.json              ← 所有粒子定义                                         │   │
│  │  ├── audio/                          ← 音频文件                                             │   │
│  │  │   ├── audio_manifest.json         ← 音频清单                                             │   │
│  │  │   ├── bgm/                                                                               │   │
│  │  │   └── sfx/                                                                               │   │
│  │  └── localization/                   ← 本地化文件                                           │   │
│  │      ├── en.json                                                                            │   │
│  │      └── zh.json                                                                            │   │
│  │                                                                                              │   │
│  └─────────────────────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                                      │
└─────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

### 7.2 图集格式定义 (*.atlas.json)

```json
{
  "atlas_version": "1.0",
  "atlas_id": "characters",
  "atlas_type": "character_sprites",
  
  "texture": {
    "file": "characters.atlas.png",
    "width": 2048,
    "height": 2048,
    "format": "RGBA32",
    "filter_mode": "point",
    "wrap_mode": "clamp"
  },
  
  "sprites": {
    "skeleton_idle": {
      "x": 0,
      "y": 0,
      "width": 128,
      "height": 128,
      "pivot": {"x": 0.5, "y": 0.0},
      "border": {"left": 0, "right": 0, "top": 0, "bottom": 0},
      "source": "entities/skeleton/assets/idle.png"
    },
    "skeleton_walk_01": {
      "x": 128,
      "y": 0,
      "width": 128,
      "height": 128,
      "pivot": {"x": 0.5, "y": 0.0},
      "source": "entities/skeleton/assets/walk_01.png"
    },
    "skeleton_walk_02": {
      "x": 256,
      "y": 0,
      "width": 128,
      "height": 128,
      "pivot": {"x": 0.5, "y": 0.0},
      "source": "entities/skeleton/assets/walk_02.png"
    },
    "goblin_idle": {
      "x": 0,
      "y": 128,
      "width": 128,
      "height": 128,
      "pivot": {"x": 0.5, "y": 0.0},
      "source": "entities/goblin/assets/idle.png"
    }
  },
  
  "sprite_sheets": {
    "skeleton_walk": {
      "sprites": ["skeleton_walk_01", "skeleton_walk_02", "skeleton_walk_03", "skeleton_walk_04", "skeleton_walk_05", "skeleton_walk_06"],
      "frame_duration": 0.1,
      "loop": true
    },
    "skeleton_attack": {
      "sprites": ["skeleton_attack_01", "skeleton_attack_02", "skeleton_attack_03", "skeleton_attack_04", "skeleton_attack_05", "skeleton_attack_06"],
      "frame_duration": 0.08,
      "loop": false
    }
  },
  
  "packing_info": {
    "algorithm": "maxrects",
    "padding": 2,
    "extrude": 1,
    "trim_alpha": true,
    "rotation_allowed": false,
    "packed_at": "2025-01-29T10:00:00Z",
    "source_count": 156,
    "efficiency": 0.87
  }
}
```

### 7.3 合并后的游戏配置 (game_config.json)

```json
{
  "config_version": "1.0",
  "package_id": "pkg_001",
  "build_time": "2025-01-29T10:00:00Z",
  
  "atlas_references": {
    "characters": "atlases/characters.atlas.json",
    "icons": "atlases/icons.atlas.json",
    "effects": "atlases/effects.atlas.json",
    "tiles": "atlases/tiles.atlas.json"
  },
  
  "animation_config": "animations/animations.json",
  "particle_config": "particles/particles.json",
  "audio_config": "audio/audio_manifest.json",
  
  "entities": {
    "player": {
      "entity_id": "player",
      "display_name": "Archer",
      "stats": {
        "base_hp": 100,
        "base_damage": 10,
        "movement_speed": 5.0,
        "attack_speed": 1.0
      },
      "sprites": {
        "idle": "characters:player_idle",
        "walk": "characters:player_walk",
        "attack": "characters:player_attack"
      },
      "animations": {
        "idle": "player_idle",
        "walk": "player_walk",
        "attack": "player_attack"
      }
    },
    
    "enemies": [
      {
        "entity_id": "skeleton",
        "display_name": "Skeleton Warrior",
        "enemy_type": 1,
        "is_boss": false,
        "stats": {
          "base_health": 30,
          "base_damage": 8,
          "movement_speed": 2.5
        },
        "sprites": {
          "idle": "characters:skeleton_idle",
          "walk": "characters:skeleton_walk",
          "attack": "characters:skeleton_attack",
          "hurt": "characters:skeleton_hurt",
          "death": "characters:skeleton_death"
        },
        "animations": {
          "idle": "skeleton_idle",
          "walk": "skeleton_walk",
          "attack": "skeleton_attack",
          "hurt": "skeleton_hurt",
          "death": "skeleton_death"
        },
        "drops": [
          {"type": "experience", "amount": {"min": 5, "max": 10}, "chance": 100},
          {"type": "gold", "amount": {"min": 1, "max": 5}, "chance": 50}
        ]
      }
    ]
  },
  
  "abilities": [
    {
      "ability_id": 10,
      "ability_name": "Freeze",
      "category": "Elemental",
      "rarity": 1,
      "max_level": 5,
      "icon": "icons:ability_freeze",
      "effects": {
        "projectile": "effects:ice_projectile",
        "hit": "effects:ice_hit",
        "status": "effects:frozen_status"
      },
      "particles": {
        "hit": "ice_explosion"
      },
      "levels": [
        {"level": 1, "effect_duration": 2.0, "damage_multiplier": 0.8},
        {"level": 2, "effect_duration": 2.5, "damage_multiplier": 0.85},
        {"level": 3, "effect_duration": 3.0, "damage_multiplier": 0.9},
        {"level": 4, "effect_duration": 3.5, "damage_multiplier": 0.95},
        {"level": 5, "effect_duration": 4.0, "damage_multiplier": 1.0}
      ]
    }
  ],
  
  "items": [
    {
      "item_id": "weapon_001",
      "item_type": "Weapon",
      "item_name": "Iron Bow",
      "icon": "icons:item_iron_bow",
      "weapon_sprite": "characters:weapon_iron_bow",
      "projectile_sprite": "effects:arrow",
      "levels": [
        {"level": 1, "cost": 0, "stats": {"Damage": 10, "AttackSpeed": 1.0}},
        {"level": 2, "cost": 100, "stats": {"Damage": 12, "AttackSpeed": 1.05}}
      ]
    }
  ],
  
  "stages": [
    {
      "stage_id": 1,
      "stage_name": "Dark Forest",
      "difficulty_multipliers": {
        "enemy_damage": 1.0,
        "enemy_hp": 1.0
      },
      "rooms": [
        {
          "room_id": 1,
          "tilemap": "tiles:room_01",
          "waves": [
            {
              "wave_id": 1,
              "enemy_spawns": [
                {"enemy_id": "skeleton", "count": 3, "spawn_delay": 0.5}
              ],
              "rewards": {
                "experience": 50,
                "gold": 20
              }
            }
          ]
        }
      ]
    }
  ]
}
```

### 7.4 合并后的动画配置 (animations.json)

```json
{
  "animation_version": "1.0",
  "atlas_references": {
    "characters": "atlases/characters.atlas.json"
  },
  
  "animations": {
    "skeleton_idle": {
      "atlas": "characters",
      "frames": ["skeleton_idle"],
      "frame_duration": 1.0,
      "loop": true,
      "events": []
    },
    "skeleton_walk": {
      "atlas": "characters",
      "frames": ["skeleton_walk_01", "skeleton_walk_02", "skeleton_walk_03", "skeleton_walk_04", "skeleton_walk_05", "skeleton_walk_06"],
      "frame_duration": 0.1,
      "loop": true,
      "events": [
        {"frame": 1, "event": "footstep_left"},
        {"frame": 4, "event": "footstep_right"}
      ]
    },
    "skeleton_attack": {
      "atlas": "characters",
      "frames": ["skeleton_attack_01", "skeleton_attack_02", "skeleton_attack_03", "skeleton_attack_04", "skeleton_attack_05", "skeleton_attack_06"],
      "frame_duration": 0.08,
      "loop": false,
      "events": [
        {"frame": 3, "event": "attack_hit"},
        {"frame": 5, "event": "attack_end"}
      ]
    },
    "skeleton_hurt": {
      "atlas": "characters",
      "frames": ["skeleton_hurt_01", "skeleton_hurt_02"],
      "frame_duration": 0.15,
      "loop": false,
      "events": [
        {"frame": 1, "event": "hurt_end"}
      ]
    },
    "skeleton_death": {
      "atlas": "characters",
      "frames": ["skeleton_death_01", "skeleton_death_02", "skeleton_death_03", "skeleton_death_04", "skeleton_death_05"],
      "frame_duration": 0.12,
      "loop": false,
      "events": [
        {"frame": 4, "event": "death_complete"}
      ]
    }
  },
  
  "state_machines": {
    "skeleton": {
      "default_state": "idle",
      "states": {
        "idle": {
          "animation": "skeleton_idle",
          "transitions": [
            {"to": "walk", "trigger": "move_start"},
            {"to": "attack", "trigger": "attack"},
            {"to": "hurt", "trigger": "take_damage"},
            {"to": "death", "trigger": "die"}
          ]
        },
        "walk": {
          "animation": "skeleton_walk",
          "transitions": [
            {"to": "idle", "trigger": "move_stop"},
            {"to": "attack", "trigger": "attack"},
            {"to": "hurt", "trigger": "take_damage"},
            {"to": "death", "trigger": "die"}
          ]
        },
        "attack": {
          "animation": "skeleton_attack",
          "transitions": [
            {"to": "idle", "trigger": "attack_end"},
            {"to": "hurt", "trigger": "take_damage"},
            {"to": "death", "trigger": "die"}
          ]
        },
        "hurt": {
          "animation": "skeleton_hurt",
          "transitions": [
            {"to": "idle", "trigger": "hurt_end"},
            {"to": "death", "trigger": "die"}
          ]
        },
        "death": {
          "animation": "skeleton_death",
          "transitions": []
        }
      }
    }
  }
}
```

### 7.5 资源合并器实现

```typescript
// resource-packer.ts - 资源合并打包器

interface PackerConfig {
  sourceDir: string;      // 原始资源目录 (source/)
  outputDir: string;      // 输出目录 (build/)
  atlasMaxSize: number;   // 图集最大尺寸
  padding: number;        // 精灵间距
  trimAlpha: boolean;     // 是否裁剪透明边缘
}

class ResourcePacker {
  private config: PackerConfig;
  private sprites: Map<string, SpriteInfo> = new Map();
  private animations: Map<string, AnimationInfo> = new Map();
  private entities: Map<string, EntityInfo> = new Map();
  
  constructor(config: PackerConfig) {
    this.config = config;
  }
  
  // 主打包流程
  async pack(): Promise<PackResult> {
    console.log('=== 开始资源打包 ===');
    
    // Step 1: 扫描原始资源
    console.log('Step 1: 扫描原始资源...');
    await this.scanSourceAssets();
    
    // Step 2: 分类资源
    console.log('Step 2: 分类资源...');
    const categorized = this.categorizeAssets();
    
    // Step 3: 生成图集
    console.log('Step 3: 生成图集...');
    const atlases = await this.generateAtlases(categorized);
    
    // Step 4: 合并动画配置
    console.log('Step 4: 合并动画配置...');
    const animationConfig = this.mergeAnimations(atlases);
    
    // Step 5: 合并粒子配置
    console.log('Step 5: 合并粒子配置...');
    const particleConfig = await this.mergeParticles();
    
    // Step 6: 合并实体配置
    console.log('Step 6: 合并实体配置...');
    const gameConfig = this.mergeGameConfig(atlases, animationConfig);
    
    // Step 7: 复制音频文件
    console.log('Step 7: 处理音频文件...');
    const audioManifest = await this.processAudio();
    
    // Step 8: 生成清单
    console.log('Step 8: 生成资源清单...');
    const manifest = this.generateManifest(atlases, animationConfig, particleConfig, audioManifest);
    
    // Step 9: 写入输出
    console.log('Step 9: 写入输出文件...');
    await this.writeOutput(manifest, atlases, animationConfig, particleConfig, gameConfig, audioManifest);
    
    console.log('=== 资源打包完成 ===');
    
    return {
      success: true,
      atlasCount: atlases.length,
      spriteCount: this.sprites.size,
      animationCount: this.animations.size,
      totalSize: await this.calculateTotalSize()
    };
  }
  
  // 扫描原始资源
  private async scanSourceAssets(): Promise<void> {
    const entityDirs = ['entities', 'abilities', 'items', 'effects'];
    
    for (const dir of entityDirs) {
      const fullPath = path.join(this.config.sourceDir, dir);
      if (await fs.pathExists(fullPath)) {
        await this.scanDirectory(fullPath, dir);
      }
    }
  }
  
  // 分类资源
  private categorizeAssets(): CategorizedAssets {
    return {
      characters: this.sprites.filter(s => s.category === 'character'),
      icons: this.sprites.filter(s => s.category === 'icon'),
      effects: this.sprites.filter(s => s.category === 'effect'),
      tiles: this.sprites.filter(s => s.category === 'tile'),
      projectiles: this.sprites.filter(s => s.category === 'projectile')
    };
  }
  
  // 生成图集
  private async generateAtlases(categorized: CategorizedAssets): Promise<AtlasInfo[]> {
    const atlases: AtlasInfo[] = [];
    
    for (const [category, sprites] of Object.entries(categorized)) {
      if (sprites.length === 0) continue;
      
      // 使用 MaxRects 算法打包精灵
      const packer = new MaxRectsPacker(
        this.config.atlasMaxSize,
        this.config.atlasMaxSize,
        this.config.padding,
        { smart: true, pot: true, square: false }
      );
      
      // 添加所有精灵
      for (const sprite of sprites) {
        const image = await sharp(sprite.path).metadata();
        packer.add(image.width, image.height, { sprite });
      }
      
      // 生成图集
      for (let i = 0; i < packer.bins.length; i++) {
        const bin = packer.bins[i];
        const atlasName = packer.bins.length > 1 ? `${category}_${i}` : category;
        
        // 创建图集图片
        const atlasImage = await this.createAtlasImage(bin);
        
        // 生成图集描述
        const atlasJson = this.createAtlasJson(atlasName, bin);
        
        atlases.push({
          name: atlasName,
          image: atlasImage,
          json: atlasJson,
          sprites: bin.rects.map(r => r.data.sprite)
        });
      }
    }
    
    return atlases;
  }
  
  // 创建图集图片
  private async createAtlasImage(bin: Bin): Promise<Buffer> {
    const canvas = sharp({
      create: {
        width: bin.width,
        height: bin.height,
        channels: 4,
        background: { r: 0, g: 0, b: 0, alpha: 0 }
      }
    });
    
    const composites = [];
    for (const rect of bin.rects) {
      const sprite = rect.data.sprite;
      composites.push({
        input: sprite.path,
        left: rect.x,
        top: rect.y
      });
    }
    
    return canvas.composite(composites).png().toBuffer();
  }
  
  // 创建图集JSON
  private createAtlasJson(name: string, bin: Bin): AtlasJson {
    const sprites: Record<string, SpriteRect> = {};
    
    for (const rect of bin.rects) {
      const sprite = rect.data.sprite;
      sprites[sprite.id] = {
        x: rect.x,
        y: rect.y,
        width: rect.width,
        height: rect.height,
        pivot: sprite.pivot || { x: 0.5, y: 0 },
        source: sprite.sourcePath
      };
    }
    
    return {
      atlas_version: '1.0',
      atlas_id: name,
      texture: {
        file: `${name}.atlas.png`,
        width: bin.width,
        height: bin.height,
        format: 'RGBA32',
        filter_mode: 'point'
      },
      sprites,
      packing_info: {
        algorithm: 'maxrects',
        padding: this.config.padding,
        packed_at: new Date().toISOString(),
        source_count: bin.rects.length,
        efficiency: this.calculateEfficiency(bin)
      }
    };
  }
  
  // 合并动画配置
  private mergeAnimations(atlases: AtlasInfo[]): AnimationsJson {
    const animations: Record<string, AnimationDef> = {};
    const stateMachines: Record<string, StateMachine> = {};
    
    // 从原始动画文件中读取并转换
    for (const [id, anim] of this.animations) {
      // 将原始帧引用转换为图集引用
      const atlasRef = this.findAtlasForSprite(anim.frames[0], atlases);
      
      animations[id] = {
        atlas: atlasRef.atlasName,
        frames: anim.frames.map(f => this.getSpriteIdInAtlas(f, atlasRef)),
        frame_duration: anim.frameDuration,
        loop: anim.loop,
        events: anim.events
      };
    }
    
    // 合并状态机
    for (const entity of this.entities.values()) {
      if (entity.stateMachine) {
        stateMachines[entity.id] = entity.stateMachine;
      }
    }
    
    return {
      animation_version: '1.0',
      atlas_references: this.buildAtlasReferences(atlases),
      animations,
      state_machines: stateMachines
    };
  }
  
  // 写入输出
  private async writeOutput(
    manifest: Manifest,
    atlases: AtlasInfo[],
    animationConfig: AnimationsJson,
    particleConfig: ParticlesJson,
    gameConfig: GameConfig,
    audioManifest: AudioManifest
  ): Promise<void> {
    const outputDir = this.config.outputDir;
    
    // 创建目录结构
    await fs.ensureDir(path.join(outputDir, 'atlases'));
    await fs.ensureDir(path.join(outputDir, 'animations'));
    await fs.ensureDir(path.join(outputDir, 'particles'));
    await fs.ensureDir(path.join(outputDir, 'audio/bgm'));
    await fs.ensureDir(path.join(outputDir, 'audio/sfx'));
    await fs.ensureDir(path.join(outputDir, 'localization'));
    
    // 写入清单
    await fs.writeJson(path.join(outputDir, 'manifest.json'), manifest, { spaces: 2 });
    
    // 写入游戏配置
    await fs.writeJson(path.join(outputDir, 'game_config.json'), gameConfig, { spaces: 2 });
    
    // 写入图集
    for (const atlas of atlases) {
      await fs.writeFile(
        path.join(outputDir, 'atlases', `${atlas.name}.atlas.png`),
        atlas.image
      );
      await fs.writeJson(
        path.join(outputDir, 'atlases', `${atlas.name}.atlas.json`),
        atlas.json,
        { spaces: 2 }
      );
    }
    
    // 写入动画配置
    await fs.writeJson(
      path.join(outputDir, 'animations/animations.json'),
      animationConfig,
      { spaces: 2 }
    );
    
    // 写入粒子配置
    await fs.writeJson(
      path.join(outputDir, 'particles/particles.json'),
      particleConfig,
      { spaces: 2 }
    );
    
    // 写入音频清单
    await fs.writeJson(
      path.join(outputDir, 'audio/audio_manifest.json'),
      audioManifest,
      { spaces: 2 }
    );
  }
}

// 使用示例
const packer = new ResourcePacker({
  sourceDir: './source',
  outputDir: './build',
  atlasMaxSize: 2048,
  padding: 2,
  trimAlpha: true
});

await packer.pack();
```

### 7.6 Unity客户端加载合并后的资源

```csharp
// PackageLoader.cs - 加载合并后的资源包

public class PackageLoader : MonoBehaviour
{
    private string packagePath;
    private GameConfig gameConfig;
    private Dictionary<string, AtlasData> atlases = new Dictionary<string, AtlasData>();
    private AnimationsConfig animationsConfig;
    private ParticlesConfig particlesConfig;
    
    // 加载资源包
    public async Task<bool> LoadPackage(string path)
    {
        packagePath = path;
        
        try
        {
            // 1. 加载清单
            var manifestJson = await File.ReadAllTextAsync(Path.Combine(path, "manifest.json"));
            var manifest = JsonUtility.FromJson<Manifest>(manifestJson);
            
            // 2. 加载游戏配置
            var configJson = await File.ReadAllTextAsync(Path.Combine(path, "game_config.json"));
            gameConfig = JsonUtility.FromJson<GameConfig>(configJson);
            
            // 3. 加载所有图集
            foreach (var atlasRef in gameConfig.atlas_references)
            {
                await LoadAtlas(atlasRef.Key, atlasRef.Value);
            }
            
            // 4. 加载动画配置
            var animJson = await File.ReadAllTextAsync(Path.Combine(path, gameConfig.animation_config));
            animationsConfig = JsonUtility.FromJson<AnimationsConfig>(animJson);
            
            // 5. 加载粒子配置
            var particleJson = await File.ReadAllTextAsync(Path.Combine(path, gameConfig.particle_config));
            particlesConfig = JsonUtility.FromJson<ParticlesConfig>(particleJson);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load package: {e.Message}");
            return false;
        }
    }
    
    // 加载图集
    private async Task LoadAtlas(string atlasName, string atlasPath)
    {
        var atlasJsonPath = Path.Combine(packagePath, atlasPath);
        var atlasJson = await File.ReadAllTextAsync(atlasJsonPath);
        var atlasData = JsonUtility.FromJson<AtlasJson>(atlasJson);
        
        // 加载图集图片
        var texturePath = Path.Combine(Path.GetDirectoryName(atlasJsonPath), atlasData.texture.file);
        var textureBytes = await File.ReadAllBytesAsync(texturePath);
        
        var texture = new Texture2D(atlasData.texture.width, atlasData.texture.height);
        texture.LoadImage(textureBytes);
        texture.filterMode = atlasData.texture.filter_mode == "point" ? FilterMode.Point : FilterMode.Bilinear;
        
        // 创建所有精灵
        var sprites = new Dictionary<string, Sprite>();
        foreach (var spriteData in atlasData.sprites)
        {
            var rect = new Rect(spriteData.Value.x, 
                               texture.height - spriteData.Value.y - spriteData.Value.height, // Unity Y轴翻转
                               spriteData.Value.width, 
                               spriteData.Value.height);
            var pivot = new Vector2(spriteData.Value.pivot.x, spriteData.Value.pivot.y);
            
            var sprite = Sprite.Create(texture, rect, pivot, 100f);
            sprite.name = spriteData.Key;
            sprites[spriteData.Key] = sprite;
        }
        
        atlases[atlasName] = new AtlasData
        {
            Texture = texture,
            Sprites = sprites,
            Json = atlasData
        };
    }
    
    // 获取精灵 (格式: "atlasName:spriteName")
    public Sprite GetSprite(string spriteRef)
    {
        var parts = spriteRef.Split(':');
        if (parts.Length != 2) return null;
        
        var atlasName = parts[0];
        var spriteName = parts[1];
        
        if (atlases.TryGetValue(atlasName, out var atlas))
        {
            if (atlas.Sprites.TryGetValue(spriteName, out var sprite))
            {
                return sprite;
            }
        }
        
        return null;
    }
    
    // 获取动画帧
    public Sprite[] GetAnimationFrames(string animationId)
    {
        if (!animationsConfig.animations.TryGetValue(animationId, out var anim))
            return null;
        
        var frames = new List<Sprite>();
        foreach (var frameName in anim.frames)
        {
            var sprite = GetSprite($"{anim.atlas}:{frameName}");
            if (sprite != null)
                frames.Add(sprite);
        }
        
        return frames.ToArray();
    }
    
    // 获取实体配置
    public EntityConfig GetEntityConfig(string entityId)
    {
        // 从 gameConfig 中查找实体
        foreach (var enemy in gameConfig.entities.enemies)
        {
            if (enemy.entity_id == entityId)
                return enemy;
        }
        return null;
    }
    
    // 获取能力配置
    public AbilityConfig GetAbilityConfig(int abilityId)
    {
        return gameConfig.abilities.FirstOrDefault(a => a.ability_id == abilityId);
    }
}

// 数据类
[Serializable]
public class AtlasData
{
    public Texture2D Texture;
    public Dictionary<string, Sprite> Sprites;
    public AtlasJson Json;
}
```

---

## 八、AI Agent 处理规范

### 7.1 图像生成 Agent

```json
{
  "agent_id": "image_generator",
  "agent_name": "Image Generation Agent",
  "agent_version": "1.0",
  
  "capabilities": [
    "character_sprite_generation",
    "icon_generation",
    "effect_sprite_generation",
    "tileset_generation",
    "background_generation"
  ],
  
  "supported_models": [
    {
      "model_id": "stable-diffusion-xl",
      "model_type": "text_to_image",
      "best_for": ["character_sprites", "icons", "effects"]
    },
    {
      "model_id": "dall-e-3",
      "model_type": "text_to_image",
      "best_for": ["backgrounds", "concept_art"]
    }
  ],
  
  "processing_pipeline": {
    "character_sprite": {
      "steps": [
        {
          "step": 1,
          "action": "generate_base_image",
          "params": {
            "size": "512x512",
            "style": "pixel_art",
            "background": "transparent"
          }
        },
        {
          "step": 2,
          "action": "remove_background",
          "params": {
            "method": "rembg",
            "threshold": 0.5
          }
        },
        {
          "step": 3,
          "action": "resize",
          "params": {
            "target_size": "128x128",
            "method": "pixel_perfect"
          }
        },
        {
          "step": 4,
          "action": "optimize_palette",
          "params": {
            "max_colors": 16,
            "preserve_transparency": true
          }
        }
      ]
    },
    
    "icon": {
      "steps": [
        {
          "step": 1,
          "action": "generate_base_image",
          "params": {
            "size": "256x256",
            "style": "game_icon",
            "background": "transparent"
          }
        },
        {
          "step": 2,
          "action": "remove_background",
          "params": {
            "method": "rembg"
          }
        },
        {
          "step": 3,
          "action": "resize",
          "params": {
            "target_size": "64x64",
            "method": "lanczos"
          }
        },
        {
          "step": 4,
          "action": "add_border",
          "params": {
            "border_width": 2,
            "border_color": "#000000"
          }
        }
      ]
    }
  },
  
  "prompt_templates": {
    "enemy_sprite": "{enemy_name}, {visual_style} style, {pose} pose, {view_angle} view, game sprite, transparent background, {additional_details}",
    "ability_icon": "{ability_name} icon, {visual_style} style, game UI icon, centered composition, {color_scheme} colors, transparent background",
    "weapon_sprite": "{weapon_name}, {visual_style} style, game weapon sprite, {orientation}, transparent background"
  },
  
  "quality_requirements": {
    "min_resolution": {"width": 64, "height": 64},
    "max_file_size_kb": 500,
    "required_transparency": true,
    "color_depth": 32
  }
}
```

### 7.2 图像优化 Agent

```json
{
  "agent_id": "image_optimizer",
  "agent_name": "Image Optimization Agent",
  "agent_version": "1.0",
  
  "capabilities": [
    "upscale",
    "background_removal",
    "color_correction",
    "noise_reduction",
    "style_transfer",
    "palette_optimization"
  ],
  
  "processing_rules": {
    "upscale": {
      "trigger_conditions": [
        {"condition": "resolution_below", "value": {"width": 128, "height": 128}}
      ],
      "methods": {
        "pixel_art": {
          "algorithm": "xbrz",
          "scale_factors": [2, 4],
          "preserve_edges": true
        },
        "general": {
          "algorithm": "real_esrgan",
          "model": "x4plus-anime",
          "scale_factor": 4
        }
      }
    },
    
    "background_removal": {
      "trigger_conditions": [
        {"condition": "has_background", "value": true}
      ],
      "methods": {
        "default": {
          "algorithm": "rembg",
          "model": "u2net",
          "alpha_matting": true
        }
      }
    },
    
    "color_correction": {
      "trigger_conditions": [
        {"condition": "color_consistency_below", "value": 0.8}
      ],
      "methods": {
        "default": {
          "auto_levels": true,
          "saturation_boost": 1.1,
          "contrast_boost": 1.05
        }
      }
    },
    
    "palette_optimization": {
      "trigger_conditions": [
        {"condition": "color_count_above", "value": 256},
        {"condition": "style_is", "value": "pixel_art"}
      ],
      "methods": {
        "pixel_art": {
          "algorithm": "median_cut",
          "max_colors": 16,
          "dithering": false
        }
      }
    }
  },
  
  "batch_processing": {
    "enabled": true,
    "max_concurrent": 5,
    "priority_order": ["upscale", "background_removal", "color_correction", "palette_optimization"]
  }
}
```

### 7.3 动画生成 Agent

```json
{
  "agent_id": "animation_generator",
  "agent_name": "Animation Generation Agent",
  "agent_version": "1.0",
  
  "capabilities": [
    "walk_cycle_generation",
    "attack_animation_generation",
    "idle_animation_generation",
    "death_animation_generation",
    "frame_interpolation"
  ],
  
  "processing_pipeline": {
    "walk_cycle": {
      "input": "idle_sprite",
      "output": "walk_sprite_sheet",
      "steps": [
        {
          "step": 1,
          "action": "analyze_character",
          "params": {
            "detect_limbs": true,
            "detect_center_of_mass": true
          }
        },
        {
          "step": 2,
          "action": "generate_key_poses",
          "params": {
            "pose_count": 4,
            "motion_type": "walk_cycle",
            "style": "match_input"
          }
        },
        {
          "step": 3,
          "action": "interpolate_frames",
          "params": {
            "total_frames": 6,
            "interpolation_method": "linear",
            "smooth_transitions": true
          }
        },
        {
          "step": 4,
          "action": "create_sprite_sheet",
          "params": {
            "layout": "horizontal",
            "padding": 0
          }
        }
      ]
    },
    
    "attack_animation": {
      "input": "idle_sprite",
      "output": "attack_sprite_sheet",
      "steps": [
        {
          "step": 1,
          "action": "analyze_character",
          "params": {
            "detect_weapon": true,
            "detect_attack_arm": true
          }
        },
        {
          "step": 2,
          "action": "generate_key_poses",
          "params": {
            "pose_count": 3,
            "motion_type": "melee_attack",
            "anticipation_frames": 1,
            "follow_through_frames": 2
          }
        },
        {
          "step": 3,
          "action": "interpolate_frames",
          "params": {
            "total_frames": 6,
            "easing": "ease_in_out"
          }
        },
        {
          "step": 4,
          "action": "add_motion_blur",
          "params": {
            "blur_frames": [2, 3],
            "blur_intensity": 0.3
          }
        },
        {
          "step": 5,
          "action": "create_sprite_sheet",
          "params": {
            "layout": "horizontal"
          }
        }
      ]
    }
  },
  
  "quality_checks": {
    "loop_seamlessness": {
      "enabled": true,
      "threshold": 0.9
    },
    "motion_smoothness": {
      "enabled": true,
      "threshold": 0.85
    },
    "style_consistency": {
      "enabled": true,
      "threshold": 0.9
    }
  }
}
```

### 7.4 数值平衡 Agent

```json
{
  "agent_id": "balance_analyzer",
  "agent_name": "Game Balance Analysis Agent",
  "agent_version": "1.0",
  
  "capabilities": [
    "difficulty_curve_analysis",
    "damage_balance_check",
    "economy_balance_check",
    "ability_power_ranking",
    "enemy_threat_assessment"
  ],
  
  "analysis_rules": {
    "difficulty_curve": {
      "metrics": [
        {
          "name": "enemy_power_progression",
          "formula": "enemy_hp * enemy_damage / player_base_damage",
          "expected_curve": "exponential",
          "growth_rate": 1.15
        },
        {
          "name": "time_to_kill",
          "formula": "enemy_hp / player_dps",
          "acceptable_range": {"min": 2, "max": 10}
        }
      ],
      "warnings": [
        {
          "condition": "sudden_spike",
          "threshold": 1.5,
          "message": "Difficulty spike detected between stage {stage_a} and {stage_b}"
        }
      ]
    },
    
    "ability_balance": {
      "metrics": [
        {
          "name": "damage_per_rarity",
          "expected_ratios": {
            "common": 1.0,
            "rare": 1.3,
            "mystic": 1.6,
            "legendary": 2.0
          }
        },
        {
          "name": "utility_score",
          "factors": ["damage", "crowd_control", "survivability", "mobility"]
        }
      ],
      "recommendations": {
        "underpowered": "Consider increasing {stat} by {percentage}%",
        "overpowered": "Consider reducing {stat} by {percentage}%"
      }
    },
    
    "economy_balance": {
      "metrics": [
        {
          "name": "gold_per_stage",
          "expected_progression": "linear",
          "base_value": 100,
          "growth_per_stage": 50
        },
        {
          "name": "item_affordability",
          "rule": "player should afford tier N item after completing stage N"
        }
      ]
    }
  },
  
  "output_format": {
    "report_type": "json",
    "include_visualizations": true,
    "suggestion_priority": ["critical", "warning", "info"]
  }
}
```

### 7.5 文案生成 Agent

```json
{
  "agent_id": "text_generator",
  "agent_name": "Game Text Generation Agent",
  "agent_version": "1.0",
  
  "capabilities": [
    "ability_description_generation",
    "item_description_generation",
    "enemy_description_generation",
    "lore_generation",
    "localization"
  ],
  
  "generation_templates": {
    "ability_description": {
      "style": "concise_gameplay",
      "max_length": 100,
      "template": "Generate a concise ability description for a {ability_type} ability named '{ability_name}' that {effect_description}. The description should be clear and gameplay-focused.",
      "examples": [
        {
          "input": {"ability_name": "Freeze", "ability_type": "elemental", "effect_description": "freezes enemies and deals damage over time"},
          "output": "Arrows have a chance to freeze enemies, slowing them and dealing cold damage over time."
        }
      ]
    },
    
    "enemy_description": {
      "style": "atmospheric",
      "max_length": 150,
      "template": "Generate an atmospheric description for an enemy named '{enemy_name}' of type '{enemy_type}'. It should convey the enemy's threat level ({threat_level}) and fighting style ({combat_style})."
    },
    
    "item_description": {
      "style": "item_flavor",
      "max_length": 80,
      "template": "Generate a flavor text for a {item_type} named '{item_name}' with {rarity} rarity. Include a hint about its stats: {stat_hints}."
    }
  },
  
  "localization": {
    "supported_languages": ["en", "zh", "ja", "ko", "es", "fr", "de"],
    "translation_quality": "professional",
    "preserve_formatting": true,
    "context_aware": true
  },
  
  "quality_checks": {
    "grammar_check": true,
    "length_validation": true,
    "tone_consistency": true,
    "terminology_consistency": true
  }
}
```

---

## 八、资源处理工作流

### 8.1 创作者上传资源流程

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           资源上传与AI处理流程                                    │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐   │
│  │ 1. 上传资源  │ ──► │ 2. 格式验证  │ ──► │ 3. AI分析   │ ──► │ 4. 生成元数据│   │
│  │             │     │             │     │             │     │             │   │
│  │ • 原始PNG   │     │ • 格式检查   │     │ • 内容识别   │     │ • meta.json │   │
│  │ • 草图      │     │ • 尺寸检查   │     │ • 风格分析   │     │ • 标签生成   │   │
│  │ • 参考图    │     │ • 透明度检查 │     │ • 质量评估   │     │ • 关系映射   │   │
│  └─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘   │
│                                                                                  │
│                                          │                                       │
│                                          ▼                                       │
│                                                                                  │
│  ┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐   │
│  │ 8. 保存资源  │ ◄── │ 7. 用户确认  │ ◄── │ 6. 预览展示  │ ◄── │ 5. AI优化   │   │
│  │             │     │             │     │             │     │             │   │
│  │ • 最终PNG   │     │ • 接受/拒绝  │     │ • 对比展示   │     │ • 放大      │   │
│  │ • meta.json │     │ • 手动调整   │     │ • 动画预览   │     │ • 去背景    │   │
│  │ • 版本记录   │     │ • 重新生成   │     │ • 效果预览   │     │ • 调色      │   │
│  └─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 8.2 AI批量处理流程

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           AI批量资源处理流程                                      │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  输入: 资源包目录                                                                 │
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │ Step 1: 扫描资源                                                         │   │
│  │                                                                          │   │
│  │ • 遍历所有 *.meta.json 文件                                               │   │
│  │ • 读取资源状态和处理历史                                                   │   │
│  │ • 识别需要处理的资源                                                       │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                          │                                       │
│                                          ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │ Step 2: 分类排队                                                         │   │
│  │                                                                          │   │
│  │ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐        │   │
│  │ │ 图像生成队列 │ │ 图像优化队列 │ │ 动画生成队列 │ │ 文案生成队列 │        │   │
│  │ │ (12 items)  │ │ (25 items)  │ │ (8 items)   │ │ (45 items)  │        │   │
│  │ └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘        │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                          │                                       │
│                                          ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │ Step 3: 并行处理                                                         │   │
│  │                                                                          │   │
│  │ ┌─────────────────────────────────────────────────────────────────────┐ │   │
│  │ │ Image Gen Agent ──► [████████░░] 80%                                │ │   │
│  │ │ Image Opt Agent ──► [██████████] 100%                               │ │   │
│  │ │ Animation Agent ──► [██████░░░░] 60%                                │ │   │
│  │ │ Text Gen Agent  ──► [████████░░] 80%                                │ │   │
│  │ └─────────────────────────────────────────────────────────────────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                          │                                       │
│                                          ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │ Step 4: 质量检查                                                         │   │
│  │                                                                          │   │
│  │ • 运行质量评估模型                                                        │   │
│  │ • 标记低质量资源                                                          │   │
│  │ • 生成处理报告                                                            │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                          │                                       │
│                                          ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │ Step 5: 更新元数据                                                       │   │
│  │                                                                          │   │
│  │ • 更新 *.meta.json 处理历史                                               │   │
│  │ • 记录 AI 处理参数                                                        │   │
│  │ • 更新质量评分                                                            │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
│  输出: 处理完成的资源包 + 处理报告                                                │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 九、Unity客户端资源加载器

### 9.1 资源加载器实现

```csharp
// PackageLoader.cs - 资源包加载器
public class PackageLoader
{
    private string packagePath;
    private ManifestData manifest;
    private Dictionary<string, Sprite> spriteCache;
    private Dictionary<string, AnimationData> animationCache;
    private Dictionary<string, ParticleData> particleCache;
    private Dictionary<string, AudioClip> audioCache;
    
    public async Task<bool> LoadPackage(string path)
    {
        packagePath = path;
        spriteCache = new Dictionary<string, Sprite>();
        animationCache = new Dictionary<string, AnimationData>();
        particleCache = new Dictionary<string, ParticleData>();
        audioCache = new Dictionary<string, AudioClip>();
        
        // 加载清单
        var manifestJson = await File.ReadAllTextAsync(Path.Combine(path, "manifest.json"));
        manifest = JsonUtility.FromJson<ManifestData>(manifestJson);
        
        return true;
    }
    
    // 加载实体
    public async Task<EntityData> LoadEntity(string entityPath)
    {
        var entityJson = await File.ReadAllTextAsync(Path.Combine(packagePath, entityPath));
        var entity = JsonUtility.FromJson<EntityData>(entityJson);
        
        // 预加载实体资源
        await PreloadEntityAssets(entity);
        
        return entity;
    }
    
    // 加载精灵
    public async Task<Sprite> LoadSprite(string relativePath)
    {
        if (spriteCache.TryGetValue(relativePath, out var cached))
            return cached;
        
        var fullPath = Path.Combine(packagePath, relativePath);
        var bytes = await File.ReadAllBytesAsync(fullPath);
        
        var texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        texture.filterMode = FilterMode.Point; // 像素风格
        
        var sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0f),
            100f
        );
        
        spriteCache[relativePath] = sprite;
        return sprite;
    }
    
    // 加载精灵表并切割
    public async Task<Sprite[]> LoadSpriteSheet(string relativePath, int frameWidth, int frameHeight)
    {
        var fullPath = Path.Combine(packagePath, relativePath);
        var bytes = await File.ReadAllBytesAsync(fullPath);
        
        var texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        texture.filterMode = FilterMode.Point;
        
        var sprites = new List<Sprite>();
        int columns = texture.width / frameWidth;
        int rows = texture.height / frameHeight;
        
        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < columns; x++)
            {
                var rect = new Rect(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
                var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0f), 100f);
                sprites.Add(sprite);
            }
        }
        
        return sprites.ToArray();
    }
    
    // 加载动画定义
    public async Task<AnimationData> LoadAnimation(string relativePath)
    {
        if (animationCache.TryGetValue(relativePath, out var cached))
            return cached;
        
        var fullPath = Path.Combine(packagePath, relativePath);
        var json = await File.ReadAllTextAsync(fullPath);
        var animation = JsonUtility.FromJson<AnimationData>(json);
        
        // 预加载动画精灵表
        foreach (var sheet in animation.sprite_sheets.Values)
        {
            await LoadSpriteSheet(sheet.path, sheet.frame_width, sheet.frame_height);
        }
        
        animationCache[relativePath] = animation;
        return animation;
    }
    
    // 加载粒子特效定义
    public async Task<ParticleData> LoadParticle(string relativePath)
    {
        if (particleCache.TryGetValue(relativePath, out var cached))
            return cached;
        
        var fullPath = Path.Combine(packagePath, relativePath);
        var json = await File.ReadAllTextAsync(fullPath);
        var particle = JsonUtility.FromJson<ParticleData>(json);
        
        // 预加载粒子精灵
        await LoadSprite(particle.particle.sprite.path);
        
        particleCache[relativePath] = particle;
        return particle;
    }
    
    // 加载音频
    public async Task<AudioClip> LoadAudio(string relativePath)
    {
        if (audioCache.TryGetValue(relativePath, out var cached))
            return cached;
        
        var fullPath = Path.Combine(packagePath, relativePath);
        using var www = UnityWebRequestMultimedia.GetAudioClip(
            "file://" + fullPath, 
            GetAudioType(relativePath)
        );
        await www.SendWebRequest();
        
        var clip = DownloadHandlerAudioClip.GetContent(www);
        audioCache[relativePath] = clip;
        return clip;
    }
    
    private AudioType GetAudioType(string path)
    {
        var ext = Path.GetExtension(path).ToLower();
        return ext switch
        {
            ".ogg" => AudioType.OGGVORBIS,
            ".mp3" => AudioType.MPEG,
            ".wav" => AudioType.WAV,
            _ => AudioType.UNKNOWN
        };
    }
}
```

---

## 十、总结

### 10.1 资源格式对照表

| 资源类型 | 文件格式 | 元数据格式 | AI处理能力 |
|----------|----------|------------|------------|
| 精灵图片 | PNG | *.meta.json | 生成、放大、去背景、调色 |
| 精灵表 | PNG + JSON | *.meta.json | 生成、切割、优化 |
| 动画 | *.anim.json | 内嵌 | 生成、插值、平滑 |
| 粒子特效 | *.particle.json | 内嵌 | 参数优化 |
| 音频 | OGG/MP3/WAV | *.meta.json | 生成、优化、标准化 |
| 实体配置 | *.entity.json | *.meta.json | 数值平衡 |
| 能力配置 | *.ability.json | *.meta.json | 数值平衡、文案生成 |
| 物品配置 | *.item.json | *.meta.json | 数值平衡、文案生成 |

### 10.2 AI Agent 能力总结

| Agent | 主要功能 | 输入 | 输出 |
|-------|----------|------|------|
| 图像生成 | 生成角色、图标、特效 | 文本描述 | PNG图片 |
| 图像优化 | 放大、去背景、调色 | PNG图片 | 优化后PNG |
| 动画生成 | 生成行走、攻击动画 | 静态图片 | 精灵表 |
| 数值平衡 | 分析游戏平衡性 | 配置JSON | 优化建议 |
| 文案生成 | 生成描述、本地化 | 实体信息 | 文本内容 |
| 音频处理 | 生成、优化音效 | 描述/音频 | 音频文件 |

这套分散的、AI友好的资源格式设计，使得每个资源都可以被独立处理和优化，大大提高了创作效率和资源质量。

# 🎮 Lua 脚本化游戏架构设计

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **目标**: 将游戏逻辑从 C# 迁移到 Lua，实现游戏逻辑的热更新和动态下发

---

## 一、架构概述

### 1.1 核心理念

**C# 提供引擎能力，Lua 实现游戏逻辑**

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Lua 脚本化架构                                      │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         资源包 (可热更新)                                │   │
│  │                                                                          │   │
│  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐         │   │
│  │  │  game_config    │  │  图集 + 动画    │  │  Lua 脚本       │         │   │
│  │  │  .json          │  │  .atlas.png     │  │  .lua           │         │   │
│  │  │                 │  │  .atlas.json    │  │                 │         │   │
│  │  │ • 数值配置      │  │ • 角色精灵      │  │ • 游戏逻辑      │         │   │
│  │  │ • 关卡配置      │  │ • 图标          │  │ • AI行为        │         │   │
│  │  │ • 敌人配置      │  │ • 特效          │  │ • 技能效果      │         │   │
│  │  │ • 能力配置      │  │ • 动画          │  │ • 战斗系统      │         │   │
│  │  └─────────────────┘  └─────────────────┘  └─────────────────┘         │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         │ 加载                                  │
│                                         ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         Unity 引擎层 (C#)                                │   │
│  │                                                                          │   │
│  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐         │   │
│  │  │  Lua 虚拟机     │  │  资源加载器     │  │  组件桥接层     │         │   │
│  │  │  (xLua/ToLua)   │  │                 │  │                 │         │   │
│  │  │                 │  │ • 图集加载      │  │ • Transform     │         │   │
│  │  │ • 脚本执行      │  │ • 音频加载      │  │ • SpriteRenderer│         │   │
│  │  │ • 热重载        │  │ • 配置解析      │  │ • Rigidbody2D   │         │   │
│  │  │ • 沙箱隔离      │  │                 │  │ • Collider2D    │         │   │
│  │  └─────────────────┘  └─────────────────┘  └─────────────────┘         │   │
│  │                                                                          │   │
│  │  ┌─────────────────────────────────────────────────────────────────┐   │   │
│  │  │                    Unity 核心组件 (不可热更新)                   │   │   │
│  │  │                                                                  │   │   │
│  │  │  • 渲染系统      • 物理系统      • 输入系统      • 音频系统     │   │   │
│  │  │  • 场景管理      • 对象池        • UI系统        • 网络通信     │   │   │
│  │  │                                                                  │   │   │
│  │  └─────────────────────────────────────────────────────────────────┘   │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 1.2 职责划分

| 层级 | 技术 | 职责 | 是否可热更新 |
|------|------|------|-------------|
| **资源层** | JSON + PNG + Lua | 游戏配置、资源、逻辑 | ✅ 是 |
| **脚本层** | Lua | 游戏逻辑、AI、技能效果 | ✅ 是 |
| **桥接层** | C# | Lua与Unity的接口 | ❌ 否 |
| **引擎层** | C# + Unity | 渲染、物理、输入等 | ❌ 否 |

---

## 二、资源包结构（含Lua脚本）

### 2.1 完整目录结构

```
package_root/
├── manifest.json                           # 资源包清单
├── game_config.json                        # 游戏配置
│
├── atlases/                                # 图集资源
│   ├── characters.atlas.json
│   ├── characters.atlas.png
│   ├── icons.atlas.json
│   ├── icons.atlas.png
│   └── effects.atlas.png
│
├── animations/                             # 动画配置
│   └── animations.json
│
├── particles/                              # 粒子配置
│   └── particles.json
│
├── audio/                                  # 音频资源
│   ├── audio_manifest.json
│   ├── bgm/
│   └── sfx/
│
├── scripts/                                # Lua 脚本目录
│   ├── main.lua                            # 入口脚本
│   ├── init.lua                            # 初始化脚本
│   │
│   ├── core/                               # 核心系统
│   │   ├── game_manager.lua                # 游戏管理器
│   │   ├── stage_manager.lua               # 关卡管理器
│   │   ├── entity_manager.lua              # 实体管理器
│   │   └── event_system.lua                # 事件系统
│   │
│   ├── entities/                           # 实体脚本
│   │   ├── player/
│   │   │   ├── player_controller.lua       # 玩家控制器
│   │   │   ├── player_stats.lua            # 玩家属性
│   │   │   └── player_combat.lua           # 玩家战斗
│   │   └── enemies/
│   │       ├── enemy_base.lua              # 敌人基类
│   │       ├── skeleton.lua                # 骷髅AI
│   │       ├── goblin.lua                  # 哥布林AI
│   │       └── boss_dragon.lua             # Boss龙AI
│   │
│   ├── abilities/                          # 能力脚本
│   │   ├── ability_base.lua                # 能力基类
│   │   ├── bouncy_shot.lua                 # 弹射箭
│   │   ├── freeze.lua                      # 冰冻
│   │   ├── ignite.lua                      # 点燃
│   │   └── fire_spirit.lua                 # 火焰精灵
│   │
│   ├── combat/                             # 战斗系统
│   │   ├── damage_system.lua               # 伤害系统
│   │   ├── projectile_system.lua           # 投射物系统
│   │   ├── effect_system.lua               # 效果系统
│   │   └── drop_system.lua                 # 掉落系统
│   │
│   ├── ui/                                 # UI逻辑
│   │   ├── hud.lua                         # HUD界面
│   │   ├── ability_select.lua              # 能力选择
│   │   └── pause_menu.lua                  # 暂停菜单
│   │
│   └── utils/                              # 工具库
│       ├── math_utils.lua                  # 数学工具
│       ├── timer.lua                       # 计时器
│       └── tween.lua                       # 缓动动画
│
└── localization/                           # 本地化
    ├── en.json
    └── zh.json
```

---

## 三、C# 引擎层设计

### 3.1 Lua 虚拟机管理器

```csharp
// LuaManager.cs - Lua虚拟机管理器
using XLua;
using System;
using System.IO;
using UnityEngine;

public class LuaManager : MonoBehaviour
{
    public static LuaManager Instance { get; private set; }
    
    private LuaEnv luaEnv;
    private string scriptsPath;
    
    // Lua 全局表
    private LuaTable gameTable;
    
    // Lua 回调
    private Action luaUpdate;
    private Action luaFixedUpdate;
    private Action luaLateUpdate;
    private Action<string> luaOnEvent;
    
    void Awake()
    {
        Instance = this;
        InitLuaEnv();
    }
    
    // 初始化 Lua 环境
    private void InitLuaEnv()
    {
        luaEnv = new LuaEnv();
        
        // 设置自定义 Loader
        luaEnv.AddLoader(CustomLoader);
        
        // 注册 C# 类型到 Lua
        RegisterTypes();
    }
    
    // 自定义 Lua 文件加载器
    private byte[] CustomLoader(ref string filepath)
    {
        // 从资源包加载 Lua 脚本
        string fullPath = Path.Combine(scriptsPath, filepath.Replace(".", "/") + ".lua");
        
        if (File.Exists(fullPath))
        {
            return File.ReadAllBytes(fullPath);
        }
        
        return null;
    }
    
    // 注册 C# 类型
    private void RegisterTypes()
    {
        // 注册 Unity 类型
        luaEnv.Global.Set("Vector2", typeof(Vector2));
        luaEnv.Global.Set("Vector3", typeof(Vector3));
        luaEnv.Global.Set("Quaternion", typeof(Quaternion));
        luaEnv.Global.Set("Color", typeof(Color));
        luaEnv.Global.Set("Time", typeof(Time));
        luaEnv.Global.Set("Input", typeof(Input));
        
        // 注册桥接类
        luaEnv.Global.Set("EntityBridge", typeof(EntityBridge));
        luaEnv.Global.Set("ResourceBridge", typeof(ResourceBridge));
        luaEnv.Global.Set("AudioBridge", typeof(AudioBridge));
        luaEnv.Global.Set("UIBridge", typeof(UIBridge));
        luaEnv.Global.Set("PhysicsBridge", typeof(PhysicsBridge));
    }
    
    // 加载资源包的 Lua 脚本
    public void LoadPackageScripts(string packagePath)
    {
        scriptsPath = Path.Combine(packagePath, "scripts");
        
        // 执行入口脚本
        luaEnv.DoString("require 'main'");
        
        // 获取全局游戏表
        gameTable = luaEnv.Global.Get<LuaTable>("Game");
        
        // 获取生命周期回调
        luaUpdate = gameTable.Get<Action>("Update");
        luaFixedUpdate = gameTable.Get<Action>("FixedUpdate");
        luaLateUpdate = gameTable.Get<Action>("LateUpdate");
        luaOnEvent = gameTable.Get<Action<string>>("OnEvent");
        
        // 调用初始化
        var luaInit = gameTable.Get<Action>("Init");
        luaInit?.Invoke();
    }
    
    void Update()
    {
        luaUpdate?.Invoke();
        luaEnv?.Tick();
    }
    
    void FixedUpdate()
    {
        luaFixedUpdate?.Invoke();
    }
    
    void LateUpdate()
    {
        luaLateUpdate?.Invoke();
    }
    
    // 发送事件到 Lua
    public void SendEvent(string eventName, params object[] args)
    {
        luaOnEvent?.Invoke(eventName);
    }
    
    // 调用 Lua 函数
    public object[] CallLuaFunction(string funcPath, params object[] args)
    {
        var func = luaEnv.Global.GetInPath<LuaFunction>(funcPath);
        return func?.Call(args);
    }
    
    // 热重载脚本
    public void HotReload()
    {
        luaEnv.DoString("require 'main'");
        Debug.Log("Lua scripts hot reloaded!");
    }
    
    void OnDestroy()
    {
        luaUpdate = null;
        luaFixedUpdate = null;
        luaLateUpdate = null;
        luaOnEvent = null;
        gameTable?.Dispose();
        luaEnv?.Dispose();
    }
}
```

### 3.2 实体桥接层

```csharp
// EntityBridge.cs - 实体桥接层，提供给 Lua 操作实体的接口
using UnityEngine;
using XLua;
using System.Collections.Generic;

[LuaCallCSharp]
public static class EntityBridge
{
    private static Dictionary<int, GameObject> entities = new Dictionary<int, GameObject>();
    private static int nextEntityId = 1;
    
    // 创建实体
    public static int CreateEntity(string entityType)
    {
        var prefab = GetEntityPrefab(entityType);
        var go = Object.Instantiate(prefab);
        
        int id = nextEntityId++;
        entities[id] = go;
        
        // 添加 Lua 组件
        var luaComponent = go.AddComponent<LuaEntityComponent>();
        luaComponent.EntityId = id;
        
        return id;
    }
    
    // 从对象池获取实体
    public static int SpawnEntity(string entityType, float x, float y)
    {
        var go = ObjectPool.Instance.Get(entityType);
        go.transform.position = new Vector3(x, y, 0);
        
        int id = nextEntityId++;
        entities[id] = go;
        
        var luaComponent = go.GetComponent<LuaEntityComponent>();
        if (luaComponent == null)
            luaComponent = go.AddComponent<LuaEntityComponent>();
        luaComponent.EntityId = id;
        
        return id;
    }
    
    // 销毁实体
    public static void DestroyEntity(int entityId)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            ObjectPool.Instance.Return(go);
            entities.Remove(entityId);
        }
    }
    
    // 获取位置
    public static Vector3 GetPosition(int entityId)
    {
        if (entities.TryGetValue(entityId, out var go))
            return go.transform.position;
        return Vector3.zero;
    }
    
    // 设置位置
    public static void SetPosition(int entityId, float x, float y)
    {
        if (entities.TryGetValue(entityId, out var go))
            go.transform.position = new Vector3(x, y, go.transform.position.z);
    }
    
    // 移动实体
    public static void Move(int entityId, float dx, float dy)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = new Vector2(dx, dy);
            else
                go.transform.position += new Vector3(dx, dy, 0) * Time.deltaTime;
        }
    }
    
    // 设置精灵
    public static void SetSprite(int entityId, string spriteRef)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            var sprite = ResourceBridge.GetSprite(spriteRef);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null && sprite != null)
                sr.sprite = sprite;
        }
    }
    
    // 播放动画
    public static void PlayAnimation(int entityId, string animationId)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            var animator = go.GetComponent<LuaAnimator>();
            animator?.Play(animationId);
        }
    }
    
    // 设置朝向
    public static void SetFacing(int entityId, int direction)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            var scale = go.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            go.transform.localScale = scale;
        }
    }
    
    // 获取实体数据
    public static LuaTable GetEntityData(int entityId)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            var luaComponent = go.GetComponent<LuaEntityComponent>();
            return luaComponent?.Data;
        }
        return null;
    }
    
    // 设置实体数据
    public static void SetEntityData(int entityId, LuaTable data)
    {
        if (entities.TryGetValue(entityId, out var go))
        {
            var luaComponent = go.GetComponent<LuaEntityComponent>();
            if (luaComponent != null)
                luaComponent.Data = data;
        }
    }
    
    // 获取距离
    public static float GetDistance(int entityId1, int entityId2)
    {
        if (entities.TryGetValue(entityId1, out var go1) && 
            entities.TryGetValue(entityId2, out var go2))
        {
            return Vector3.Distance(go1.transform.position, go2.transform.position);
        }
        return float.MaxValue;
    }
    
    // 查找范围内的实体
    public static int[] FindEntitiesInRange(float x, float y, float radius, string tag)
    {
        var results = new List<int>();
        var center = new Vector2(x, y);
        
        foreach (var kvp in entities)
        {
            if (string.IsNullOrEmpty(tag) || kvp.Value.CompareTag(tag))
            {
                if (Vector2.Distance(kvp.Value.transform.position, center) <= radius)
                {
                    results.Add(kvp.Key);
                }
            }
        }
        
        return results.ToArray();
    }
    
    private static GameObject GetEntityPrefab(string entityType)
    {
        // 从预制体池获取
        return PrefabPool.Instance.GetPrefab(entityType);
    }
}

// Lua 实体组件
public class LuaEntityComponent : MonoBehaviour
{
    public int EntityId { get; set; }
    public LuaTable Data { get; set; }
    
    void OnDestroy()
    {
        Data?.Dispose();
    }
}
```

### 3.3 资源桥接层

```csharp
// ResourceBridge.cs - 资源桥接层
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class ResourceBridge
{
    private static PackageLoader packageLoader;
    
    public static void Init(PackageLoader loader)
    {
        packageLoader = loader;
    }
    
    // 获取精灵
    public static Sprite GetSprite(string spriteRef)
    {
        return packageLoader.GetSprite(spriteRef);
    }
    
    // 获取动画帧
    public static Sprite[] GetAnimationFrames(string animationId)
    {
        return packageLoader.GetAnimationFrames(animationId);
    }
    
    // 获取配置
    public static string GetConfig(string key)
    {
        return packageLoader.GetConfigValue(key);
    }
    
    // 获取实体配置
    public static LuaTable GetEntityConfig(string entityId)
    {
        var config = packageLoader.GetEntityConfig(entityId);
        return ConvertToLuaTable(config);
    }
    
    // 获取能力配置
    public static LuaTable GetAbilityConfig(int abilityId)
    {
        var config = packageLoader.GetAbilityConfig(abilityId);
        return ConvertToLuaTable(config);
    }
    
    // 获取关卡配置
    public static LuaTable GetStageConfig(int stageId)
    {
        var config = packageLoader.GetStageConfig(stageId);
        return ConvertToLuaTable(config);
    }
    
    // 获取本地化文本
    public static string GetLocalizedText(string key)
    {
        return packageLoader.GetLocalizedText(key);
    }
    
    private static LuaTable ConvertToLuaTable(object obj)
    {
        // 将 C# 对象转换为 Lua Table
        var json = JsonUtility.ToJson(obj);
        var result = LuaManager.Instance.CallLuaFunction("json.decode", json);
        return result?[0] as LuaTable;
    }
}
```

### 3.4 物理桥接层

```csharp
// PhysicsBridge.cs - 物理桥接层
using UnityEngine;
using XLua;
using System.Collections.Generic;

[LuaCallCSharp]
public static class PhysicsBridge
{
    // 射线检测
    public static int[] Raycast(float startX, float startY, float dirX, float dirY, float distance, string layerMask)
    {
        var origin = new Vector2(startX, startY);
        var direction = new Vector2(dirX, dirY).normalized;
        var layer = LayerMask.GetMask(layerMask);
        
        var hits = Physics2D.RaycastAll(origin, direction, distance, layer);
        var results = new List<int>();
        
        foreach (var hit in hits)
        {
            var luaComponent = hit.collider.GetComponent<LuaEntityComponent>();
            if (luaComponent != null)
                results.Add(luaComponent.EntityId);
        }
        
        return results.ToArray();
    }
    
    // 圆形检测
    public static int[] OverlapCircle(float x, float y, float radius, string layerMask)
    {
        var center = new Vector2(x, y);
        var layer = LayerMask.GetMask(layerMask);
        
        var colliders = Physics2D.OverlapCircleAll(center, radius, layer);
        var results = new List<int>();
        
        foreach (var collider in colliders)
        {
            var luaComponent = collider.GetComponent<LuaEntityComponent>();
            if (luaComponent != null)
                results.Add(luaComponent.EntityId);
        }
        
        return results.ToArray();
    }
    
    // 添加力
    public static void AddForce(int entityId, float forceX, float forceY)
    {
        var go = GetGameObject(entityId);
        if (go != null)
        {
            var rb = go.GetComponent<Rigidbody2D>();
            rb?.AddForce(new Vector2(forceX, forceY));
        }
    }
    
    // 设置速度
    public static void SetVelocity(int entityId, float vx, float vy)
    {
        var go = GetGameObject(entityId);
        if (go != null)
        {
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = new Vector2(vx, vy);
        }
    }
    
    // 获取速度
    public static Vector2 GetVelocity(int entityId)
    {
        var go = GetGameObject(entityId);
        if (go != null)
        {
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
                return rb.velocity;
        }
        return Vector2.zero;
    }
    
    private static GameObject GetGameObject(int entityId)
    {
        // 从 EntityBridge 获取
        return null; // 实际实现需要访问 EntityBridge 的 entities 字典
    }
}
```

### 3.5 音频桥接层

```csharp
// AudioBridge.cs - 音频桥接层
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class AudioBridge
{
    private static AudioSource bgmSource;
    private static AudioSource[] sfxSources;
    private static int currentSfxIndex = 0;
    
    public static void Init(int sfxChannels = 8)
    {
        var go = new GameObject("AudioBridge");
        Object.DontDestroyOnLoad(go);
        
        bgmSource = go.AddComponent<AudioSource>();
        bgmSource.loop = true;
        
        sfxSources = new AudioSource[sfxChannels];
        for (int i = 0; i < sfxChannels; i++)
        {
            sfxSources[i] = go.AddComponent<AudioSource>();
        }
    }
    
    // 播放背景音乐
    public static void PlayBGM(string audioPath, float volume = 1.0f)
    {
        var clip = PackageLoader.Instance.LoadAudioSync(audioPath);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.volume = volume;
            bgmSource.Play();
        }
    }
    
    // 停止背景音乐
    public static void StopBGM()
    {
        bgmSource.Stop();
    }
    
    // 播放音效
    public static void PlaySFX(string audioPath, float volume = 1.0f)
    {
        var clip = PackageLoader.Instance.LoadAudioSync(audioPath);
        if (clip != null)
        {
            var source = sfxSources[currentSfxIndex];
            source.clip = clip;
            source.volume = volume;
            source.Play();
            
            currentSfxIndex = (currentSfxIndex + 1) % sfxSources.Length;
        }
    }
    
    // 在指定位置播放音效
    public static void PlaySFXAtPosition(string audioPath, float x, float y, float volume = 1.0f)
    {
        var clip = PackageLoader.Instance.LoadAudioSync(audioPath);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, new Vector3(x, y, 0), volume);
        }
    }
    
    // 设置BGM音量
    public static void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }
    
    // 暂停/恢复BGM
    public static void PauseBGM(bool pause)
    {
        if (pause)
            bgmSource.Pause();
        else
            bgmSource.UnPause();
    }
}
```

---

## 四、Lua 脚本层设计

### 4.1 入口脚本 (main.lua)

```lua
-- main.lua - 游戏入口脚本

-- 加载核心模块
require "init"
require "core.game_manager"
require "core.stage_manager"
require "core.entity_manager"
require "core.event_system"

-- 全局游戏对象
Game = {}

-- 初始化
function Game.Init()
    print("[Lua] Game initializing...")
    
    -- 初始化事件系统
    EventSystem.Init()
    
    -- 初始化实体管理器
    EntityManager.Init()
    
    -- 初始化游戏管理器
    GameManager.Init()
    
    -- 加载游戏配置
    GameManager.LoadConfig()
    
    print("[Lua] Game initialized!")
end

-- 每帧更新
function Game.Update()
    local dt = CS.UnityEngine.Time.deltaTime
    
    -- 更新计时器
    Timer.Update(dt)
    
    -- 更新实体
    EntityManager.Update(dt)
    
    -- 更新游戏管理器
    GameManager.Update(dt)
end

-- 固定更新（物理）
function Game.FixedUpdate()
    local dt = CS.UnityEngine.Time.fixedDeltaTime
    EntityManager.FixedUpdate(dt)
end

-- 延迟更新
function Game.LateUpdate()
    EntityManager.LateUpdate()
end

-- 事件处理
function Game.OnEvent(eventName, ...)
    EventSystem.Dispatch(eventName, ...)
end

return Game
```

### 4.2 游戏管理器 (game_manager.lua)

```lua
-- core/game_manager.lua - 游戏管理器

local GameManager = {}

-- 游戏状态
GameManager.State = {
    LOADING = 1,
    PLAYING = 2,
    PAUSED = 3,
    GAME_OVER = 4,
    VICTORY = 5
}

local currentState = GameManager.State.LOADING
local gameConfig = nil
local playerData = nil

function GameManager.Init()
    print("[GameManager] Initializing...")
    
    -- 注册事件监听
    EventSystem.On("player_died", GameManager.OnPlayerDied)
    EventSystem.On("stage_completed", GameManager.OnStageCompleted)
    EventSystem.On("pause_game", GameManager.OnPauseGame)
end

function GameManager.LoadConfig()
    -- 从资源桥接层加载配置
    gameConfig = ResourceBridge.GetConfig("game_config")
    
    -- 解析配置
    if type(gameConfig) == "string" then
        gameConfig = json.decode(gameConfig)
    end
    
    print("[GameManager] Config loaded!")
end

function GameManager.StartGame(stageId)
    print("[GameManager] Starting game, stage: " .. stageId)
    
    -- 初始化玩家数据
    playerData = {
        hp = gameConfig.player.base_hp,
        maxHp = gameConfig.player.base_hp,
        damage = gameConfig.player.base_damage,
        level = 1,
        experience = 0,
        gold = 0,
        abilities = {}
    }
    
    -- 加载关卡
    StageManager.LoadStage(stageId)
    
    -- 创建玩家
    local playerId = EntityManager.CreatePlayer(playerData)
    
    -- 切换状态
    currentState = GameManager.State.PLAYING
    
    -- 播放背景音乐
    local stageConfig = ResourceBridge.GetStageConfig(stageId)
    if stageConfig.bgm then
        AudioBridge.PlayBGM(stageConfig.bgm)
    end
end

function GameManager.Update(dt)
    if currentState ~= GameManager.State.PLAYING then
        return
    end
    
    -- 更新关卡
    StageManager.Update(dt)
end

function GameManager.OnPlayerDied()
    print("[GameManager] Player died!")
    currentState = GameManager.State.GAME_OVER
    
    -- 显示游戏结束UI
    UIBridge.ShowGameOver(playerData)
end

function GameManager.OnStageCompleted()
    print("[GameManager] Stage completed!")
    currentState = GameManager.State.VICTORY
    
    -- 显示胜利UI
    UIBridge.ShowVictory(playerData)
end

function GameManager.OnPauseGame(paused)
    if paused then
        currentState = GameManager.State.PAUSED
        CS.UnityEngine.Time.timeScale = 0
    else
        currentState = GameManager.State.PLAYING
        CS.UnityEngine.Time.timeScale = 1
    end
end

function GameManager.GetState()
    return currentState
end

function GameManager.GetPlayerData()
    return playerData
end

function GameManager.AddExperience(amount)
    playerData.experience = playerData.experience + amount
    
    -- 检查升级
    local expNeeded = GameManager.GetExpForLevel(playerData.level + 1)
    if playerData.experience >= expNeeded then
        GameManager.LevelUp()
    end
end

function GameManager.LevelUp()
    playerData.level = playerData.level + 1
    playerData.experience = 0
    
    print("[GameManager] Level up! Now level: " .. playerData.level)
    
    -- 触发升级事件
    EventSystem.Dispatch("player_level_up", playerData.level)
    
    -- 显示能力选择UI
    UIBridge.ShowAbilitySelect()
end

function GameManager.GetExpForLevel(level)
    return math.floor(100 * (level ^ 1.5))
end

return GameManager
```

### 4.3 实体管理器 (entity_manager.lua)

```lua
-- core/entity_manager.lua - 实体管理器

local EntityManager = {}

local entities = {}
local player = nil

function EntityManager.Init()
    print("[EntityManager] Initializing...")
    entities = {}
    player = nil
end

function EntityManager.CreatePlayer(playerData)
    print("[EntityManager] Creating player...")
    
    -- 通过桥接层创建实体
    local entityId = EntityBridge.SpawnEntity("player", 0, 0)
    
    -- 加载玩家脚本
    local PlayerController = require "entities.player.player_controller"
    
    player = {
        id = entityId,
        type = "player",
        controller = PlayerController.new(entityId, playerData),
        data = playerData
    }
    
    entities[entityId] = player
    
    return entityId
end

function EntityManager.CreateEnemy(enemyType, x, y, enemyConfig)
    print("[EntityManager] Creating enemy: " .. enemyType)
    
    -- 通过桥接层创建实体
    local entityId = EntityBridge.SpawnEntity(enemyType, x, y)
    
    -- 加载敌人脚本
    local EnemyScript = require("entities.enemies." .. enemyType)
    
    local enemy = {
        id = entityId,
        type = "enemy",
        enemyType = enemyType,
        controller = EnemyScript.new(entityId, enemyConfig),
        data = {
            hp = enemyConfig.base_health,
            maxHp = enemyConfig.base_health,
            damage = enemyConfig.base_damage
        }
    }
    
    entities[entityId] = enemy
    
    return entityId
end

function EntityManager.CreateProjectile(projectileType, x, y, dirX, dirY, damage, owner)
    local entityId = EntityBridge.SpawnEntity(projectileType, x, y)
    
    local ProjectileSystem = require "combat.projectile_system"
    
    local projectile = {
        id = entityId,
        type = "projectile",
        controller = ProjectileSystem.CreateProjectile(entityId, dirX, dirY, damage, owner),
        data = {
            damage = damage,
            owner = owner
        }
    }
    
    entities[entityId] = projectile
    
    return entityId
end

function EntityManager.DestroyEntity(entityId)
    local entity = entities[entityId]
    if entity then
        if entity.controller and entity.controller.OnDestroy then
            entity.controller:OnDestroy()
        end
        
        EntityBridge.DestroyEntity(entityId)
        entities[entityId] = nil
    end
end

function EntityManager.Update(dt)
    for id, entity in pairs(entities) do
        if entity.controller and entity.controller.Update then
            entity.controller:Update(dt)
        end
    end
end

function EntityManager.FixedUpdate(dt)
    for id, entity in pairs(entities) do
        if entity.controller and entity.controller.FixedUpdate then
            entity.controller:FixedUpdate(dt)
        end
    end
end

function EntityManager.LateUpdate()
    for id, entity in pairs(entities) do
        if entity.controller and entity.controller.LateUpdate then
            entity.controller:LateUpdate()
        end
    end
end

function EntityManager.GetEntity(entityId)
    return entities[entityId]
end

function EntityManager.GetPlayer()
    return player
end

function EntityManager.GetAllEnemies()
    local enemies = {}
    for id, entity in pairs(entities) do
        if entity.type == "enemy" then
            table.insert(enemies, entity)
        end
    end
    return enemies
end

function EntityManager.DamageEntity(entityId, damage, source)
    local entity = entities[entityId]
    if not entity then return end
    
    entity.data.hp = entity.data.hp - damage
    
    -- 触发受伤事件
    EventSystem.Dispatch("entity_damaged", entityId, damage, source)
    
    -- 播放受伤动画
    EntityBridge.PlayAnimation(entityId, entity.type .. "_hurt")
    
    -- 检查死亡
    if entity.data.hp <= 0 then
        EntityManager.KillEntity(entityId, source)
    end
end

function EntityManager.KillEntity(entityId, killer)
    local entity = entities[entityId]
    if not entity then return end
    
    -- 触发死亡事件
    EventSystem.Dispatch("entity_killed", entityId, killer)
    
    -- 播放死亡动画
    EntityBridge.PlayAnimation(entityId, entity.type .. "_death")
    
    -- 如果是敌人，处理掉落
    if entity.type == "enemy" then
        local DropSystem = require "combat.drop_system"
        DropSystem.ProcessDrops(entity, killer)
    end
    
    -- 如果是玩家，游戏结束
    if entity.type == "player" then
        EventSystem.Dispatch("player_died")
    end
    
    -- 延迟销毁
    Timer.After(0.5, function()
        EntityManager.DestroyEntity(entityId)
    end)
end

return EntityManager
```

### 4.4 玩家控制器 (player_controller.lua)

```lua
-- entities/player/player_controller.lua - 玩家控制器

local PlayerController = {}
PlayerController.__index = PlayerController

function PlayerController.new(entityId, playerData)
    local self = setmetatable({}, PlayerController)
    
    self.entityId = entityId
    self.data = playerData
    self.moveSpeed = 5.0
    self.attackSpeed = 1.0
    self.attackCooldown = 0
    self.facing = 1  -- 1 = 右, -1 = 左
    self.isAttacking = false
    self.abilities = {}
    
    -- 初始化
    self:Init()
    
    return self
end

function PlayerController:Init()
    -- 设置初始精灵
    EntityBridge.SetSprite(self.entityId, "characters:player_idle")
    
    -- 注册事件
    EventSystem.On("ability_acquired", function(abilityId, level)
        self:OnAbilityAcquired(abilityId, level)
    end)
end

function PlayerController:Update(dt)
    -- 处理输入
    self:HandleInput(dt)
    
    -- 更新攻击冷却
    if self.attackCooldown > 0 then
        self.attackCooldown = self.attackCooldown - dt
    end
    
    -- 更新能力
    for _, ability in ipairs(self.abilities) do
        if ability.Update then
            ability:Update(dt)
        end
    end
end

function PlayerController:HandleInput(dt)
    local Input = CS.UnityEngine.Input
    
    -- 移动输入
    local moveX = 0
    local moveY = 0
    
    if Input.GetKey(CS.UnityEngine.KeyCode.W) or Input.GetKey(CS.UnityEngine.KeyCode.UpArrow) then
        moveY = 1
    end
    if Input.GetKey(CS.UnityEngine.KeyCode.S) or Input.GetKey(CS.UnityEngine.KeyCode.DownArrow) then
        moveY = -1
    end
    if Input.GetKey(CS.UnityEngine.KeyCode.A) or Input.GetKey(CS.UnityEngine.KeyCode.LeftArrow) then
        moveX = -1
        self.facing = -1
    end
    if Input.GetKey(CS.UnityEngine.KeyCode.D) or Input.GetKey(CS.UnityEngine.KeyCode.RightArrow) then
        moveX = 1
        self.facing = 1
    end
    
    -- 应用移动
    if moveX ~= 0 or moveY ~= 0 then
        local speed = self.moveSpeed * self.data.movementSpeedMultiplier
        EntityBridge.Move(self.entityId, moveX * speed, moveY * speed)
        EntityBridge.PlayAnimation(self.entityId, "player_walk")
        EntityBridge.SetFacing(self.entityId, self.facing)
    else
        EntityBridge.PlayAnimation(self.entityId, "player_idle")
    end
    
    -- 攻击输入（自动攻击最近敌人）
    if self.attackCooldown <= 0 then
        self:TryAttack()
    end
end

function PlayerController:TryAttack()
    -- 查找最近的敌人
    local pos = EntityBridge.GetPosition(self.entityId)
    local enemies = PhysicsBridge.OverlapCircle(pos.x, pos.y, 10, "Enemy")
    
    if #enemies == 0 then return end
    
    -- 找最近的敌人
    local nearestEnemy = nil
    local nearestDist = math.huge
    
    for _, enemyId in ipairs(enemies) do
        local dist = EntityBridge.GetDistance(self.entityId, enemyId)
        if dist < nearestDist then
            nearestDist = dist
            nearestEnemy = enemyId
        end
    end
    
    if nearestEnemy then
        self:Attack(nearestEnemy)
    end
end

function PlayerController:Attack(targetId)
    local pos = EntityBridge.GetPosition(self.entityId)
    local targetPos = EntityBridge.GetPosition(targetId)
    
    -- 计算方向
    local dirX = targetPos.x - pos.x
    local dirY = targetPos.y - pos.y
    local len = math.sqrt(dirX * dirX + dirY * dirY)
    dirX = dirX / len
    dirY = dirY / len
    
    -- 播放攻击动画
    EntityBridge.PlayAnimation(self.entityId, "player_attack")
    
    -- 创建投射物
    local damage = self.data.damage * self.data.damageMultiplier
    EntityManager.CreateProjectile("arrow", pos.x, pos.y, dirX, dirY, damage, self.entityId)
    
    -- 播放音效
    AudioBridge.PlaySFX("audio/sfx/bow_shoot.ogg")
    
    -- 设置冷却
    self.attackCooldown = 1.0 / (self.attackSpeed * self.data.attackSpeedMultiplier)
    
    -- 触发攻击事件（用于能力处理）
    EventSystem.Dispatch("player_attack", self.entityId, targetId, dirX, dirY)
end

function PlayerController:OnAbilityAcquired(abilityId, level)
    -- 加载能力脚本
    local abilityConfig = ResourceBridge.GetAbilityConfig(abilityId)
    local AbilityScript = require("abilities." .. abilityConfig.script_name)
    
    local ability = AbilityScript.new(self.entityId, abilityConfig, level)
    table.insert(self.abilities, ability)
    
    print("[Player] Acquired ability: " .. abilityConfig.ability_name .. " level " .. level)
end

function PlayerController:OnDestroy()
    -- 清理
    for _, ability in ipairs(self.abilities) do
        if ability.OnDestroy then
            ability:OnDestroy()
        end
    end
end

return PlayerController
```

### 4.5 敌人AI示例 (skeleton.lua)

```lua
-- entities/enemies/skeleton.lua - 骷髅敌人AI

local EnemyBase = require "entities.enemies.enemy_base"
local Skeleton = setmetatable({}, {__index = EnemyBase})
Skeleton.__index = Skeleton

function Skeleton.new(entityId, config)
    local self = setmetatable(EnemyBase.new(entityId, config), Skeleton)
    
    self.attackRange = 1.5
    self.detectionRange = 8.0
    self.attackCooldown = 0
    self.attackDelay = 1.5
    self.state = "idle"
    
    return self
end

function Skeleton:Update(dt)
    -- 调用基类更新
    EnemyBase.Update(self, dt)
    
    -- 更新攻击冷却
    if self.attackCooldown > 0 then
        self.attackCooldown = self.attackCooldown - dt
    end
    
    -- 状态机
    if self.state == "idle" then
        self:IdleState(dt)
    elseif self.state == "chase" then
        self:ChaseState(dt)
    elseif self.state == "attack" then
        self:AttackState(dt)
    end
end

function Skeleton:IdleState(dt)
    -- 检测玩家
    local player = EntityManager.GetPlayer()
    if not player then return end
    
    local dist = EntityBridge.GetDistance(self.entityId, player.id)
    
    if dist <= self.detectionRange then
        self.state = "chase"
        self.target = player.id
    else
        -- 播放待机动画
        EntityBridge.PlayAnimation(self.entityId, "skeleton_idle")
    end
end

function Skeleton:ChaseState(dt)
    if not self.target then
        self.state = "idle"
        return
    end
    
    local dist = EntityBridge.GetDistance(self.entityId, self.target)
    
    -- 如果在攻击范围内，切换到攻击状态
    if dist <= self.attackRange then
        self.state = "attack"
        return
    end
    
    -- 如果超出检测范围，回到待机
    if dist > self.detectionRange * 1.5 then
        self.state = "idle"
        self.target = nil
        return
    end
    
    -- 追逐玩家
    local pos = EntityBridge.GetPosition(self.entityId)
    local targetPos = EntityBridge.GetPosition(self.target)
    
    local dirX = targetPos.x - pos.x
    local dirY = targetPos.y - pos.y
    local len = math.sqrt(dirX * dirX + dirY * dirY)
    
    if len > 0 then
        dirX = dirX / len
        dirY = dirY / len
        
        EntityBridge.Move(self.entityId, dirX * self.config.movement_speed, dirY * self.config.movement_speed)
        EntityBridge.SetFacing(self.entityId, dirX > 0 and 1 or -1)
        EntityBridge.PlayAnimation(self.entityId, "skeleton_walk")
    end
end

function Skeleton:AttackState(dt)
    if not self.target then
        self.state = "idle"
        return
    end
    
    local dist = EntityBridge.GetDistance(self.entityId, self.target)
    
    -- 如果目标离开攻击范围，追逐
    if dist > self.attackRange * 1.2 then
        self.state = "chase"
        return
    end
    
    -- 攻击
    if self.attackCooldown <= 0 then
        self:PerformAttack()
        self.attackCooldown = self.attackDelay
    end
end

function Skeleton:PerformAttack()
    -- 播放攻击动画
    EntityBridge.PlayAnimation(self.entityId, "skeleton_attack")
    
    -- 播放音效
    AudioBridge.PlaySFX("audio/sfx/skeleton_attack.ogg")
    
    -- 延迟造成伤害（等待动画播放到攻击帧）
    Timer.After(0.24, function()  -- 第3帧，0.08 * 3
        if self.target then
            local dist = EntityBridge.GetDistance(self.entityId, self.target)
            if dist <= self.attackRange then
                EntityManager.DamageEntity(self.target, self.config.base_damage, self.entityId)
            end
        end
    end)
end

return Skeleton
```

### 4.6 能力示例 (freeze.lua)

```lua
-- abilities/freeze.lua - 冰冻能力

local AbilityBase = require "abilities.ability_base"
local Freeze = setmetatable({}, {__index = AbilityBase})
Freeze.__index = Freeze

function Freeze.new(ownerId, config, level)
    local self = setmetatable(AbilityBase.new(ownerId, config, level), Freeze)
    
    -- 从配置获取等级数据
    local levelData = config.levels[level]
    self.effectDuration = levelData.effect_duration
    self.damageMultiplier = levelData.arrow_damage_multiplier
    self.dealsDot = levelData.effect_deals_dot or false
    self.dotDamage = levelData.effect_damage_multiplier or 0
    self.dotInterval = levelData.effect_damage_interval or 0.5
    
    -- 注册事件
    EventSystem.On("player_attack", function(playerId, targetId, dirX, dirY)
        if playerId == self.ownerId then
            self:OnPlayerAttack(targetId, dirX, dirY)
        end
    end)
    
    EventSystem.On("projectile_hit", function(projectileId, targetId, damage)
        self:OnProjectileHit(projectileId, targetId, damage)
    end)
    
    return self
end

function Freeze:OnPlayerAttack(targetId, dirX, dirY)
    -- 修改投射物外观（添加冰霜效果）
    -- 这里可以通过事件系统通知投射物系统
end

function Freeze:OnProjectileHit(projectileId, targetId, damage)
    -- 检查是否是自己的投射物
    local projectile = EntityManager.GetEntity(projectileId)
    if not projectile or projectile.data.owner ~= self.ownerId then
        return
    end
    
    -- 应用冰冻效果
    self:ApplyFreezeEffect(targetId)
end

function Freeze:ApplyFreezeEffect(targetId)
    local entity = EntityManager.GetEntity(targetId)
    if not entity then return end
    
    -- 播放冰冻特效
    local pos = EntityBridge.GetPosition(targetId)
    EffectSystem.PlayEffect("ice_hit", pos.x, pos.y)
    
    -- 播放音效
    AudioBridge.PlaySFX("audio/sfx/freeze_hit.ogg")
    
    -- 应用减速效果
    if entity.controller and entity.controller.ApplyStatusEffect then
        entity.controller:ApplyStatusEffect("frozen", {
            duration = self.effectDuration,
            speedMultiplier = 0.3,  -- 减速70%
            tint = {r = 0.5, g = 0.8, b = 1.0}  -- 冰蓝色
        })
    end
    
    -- 如果有DOT伤害
    if self.dealsDot then
        self:ApplyDotDamage(targetId)
    end
end

function Freeze:ApplyDotDamage(targetId)
    local ticks = math.floor(self.effectDuration / self.dotInterval)
    local baseDamage = GameManager.GetPlayerData().damage
    local dotDamage = baseDamage * self.dotDamage
    
    for i = 1, ticks do
        Timer.After(i * self.dotInterval, function()
            local entity = EntityManager.GetEntity(targetId)
            if entity and entity.data.hp > 0 then
                EntityManager.DamageEntity(targetId, dotDamage, self.ownerId)
                
                -- 播放DOT特效
                local pos = EntityBridge.GetPosition(targetId)
                EffectSystem.PlayEffect("ice_dot", pos.x, pos.y)
            end
        end)
    end
end

function Freeze:OnDestroy()
    -- 取消事件监听
    EventSystem.Off("player_attack", self.onPlayerAttack)
    EventSystem.Off("projectile_hit", self.onProjectileHit)
end

return Freeze
```

---

## 五、资源包结构更新

### 5.1 包含Lua脚本的manifest.json

```json
{
  "manifest_version": "1.0",
  "package_info": {
    "id": "pkg_fantasy_archer_001",
    "name": "Fantasy Archer Adventure",
    "version": "1.0.0",
    "lua_version": "5.3",
    "min_client_version": "2.0.0"
  },
  
  "config_files": {
    "game_config": "game_config.json"
  },
  
  "atlases": [
    {"name": "characters", "path": "atlases/characters.atlas.json"},
    {"name": "icons", "path": "atlases/icons.atlas.json"},
    {"name": "effects", "path": "atlases/effects.atlas.json"}
  ],
  
  "scripts": {
    "entry": "scripts/main.lua",
    "files": [
      "scripts/main.lua",
      "scripts/init.lua",
      "scripts/core/game_manager.lua",
      "scripts/core/stage_manager.lua",
      "scripts/core/entity_manager.lua",
      "scripts/core/event_system.lua",
      "scripts/entities/player/player_controller.lua",
      "scripts/entities/enemies/enemy_base.lua",
      "scripts/entities/enemies/skeleton.lua",
      "scripts/entities/enemies/goblin.lua",
      "scripts/abilities/ability_base.lua",
      "scripts/abilities/freeze.lua",
      "scripts/abilities/ignite.lua",
      "scripts/combat/damage_system.lua",
      "scripts/combat/projectile_system.lua",
      "scripts/combat/drop_system.lua",
      "scripts/utils/timer.lua",
      "scripts/utils/math_utils.lua"
    ],
    "total_count": 20
  },
  
  "audio": {
    "manifest": "audio/audio_manifest.json"
  },
  
  "localization": {
    "default": "en",
    "files": ["localization/en.json", "localization/zh.json"]
  }
}
```

---

## 六、优势与注意事项

### 6.1 架构优势

| 优势 | 说明 |
|------|------|
| **逻辑热更新** | 游戏逻辑可以随资源包一起下发，无需重新发布客户端 |
| **完全自定义** | 不同资源包可以有完全不同的游戏玩法 |
| **快速迭代** | 修改Lua脚本无需重新编译 |
| **安全隔离** | Lua沙箱环境，限制危险操作 |
| **跨平台** | Lua脚本在所有平台通用 |

### 6.2 注意事项

| 注意点 | 说明 |
|--------|------|
| **性能** | Lua比C#慢，热点代码需要优化或用C#实现 |
| **调试** | 需要Lua调试工具支持 |
| **安全** | 需要对Lua脚本进行沙箱限制 |
| **兼容性** | 桥接层API需要保持稳定 |

### 6.3 推荐的Lua框架

| 框架 | 特点 | 推荐场景 |
|------|------|----------|
| **xLua** | 腾讯开源，性能好，热更新支持 | ✅ 推荐 |
| **ToLua** | 成熟稳定，社区活跃 | 大型项目 |
| **MoonSharp** | 纯C#实现，无需原生库 | WebGL平台 |

---

## 七、总结

通过 Lua 脚本化架构，实现了：

1. ✅ **游戏逻辑可热更新** - Lua脚本随资源包下发
2. ✅ **完全自定义玩法** - 不同资源包可以有不同的游戏规则
3. ✅ **C#引擎层稳定** - 只提供基础能力，不包含游戏逻辑
4. ✅ **资源与逻辑统一** - 配置、资源、脚本都在资源包中
5. ✅ **快速迭代** - 修改脚本无需重新编译客户端

这样的架构真正实现了"一套引擎，无限玩法"的目标！

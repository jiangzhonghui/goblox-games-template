# 🔧 游戏模板改造指南

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **目标**: 将传统 Unity C# 游戏改造成支持热更新的 Luau 脚本化架构

---

## 一、改造概述

### 1.1 改造目标

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           游戏改造目标                                           │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  改造前 (传统 Unity 游戏):                                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                                                                          │   │
│  │  Unity 项目                                                              │   │
│  │  ├── Scripts/           ← C# 代码 (游戏逻辑 + 引擎逻辑 混合)             │   │
│  │  ├── Resources/         ← 静态资源 (编译时打包)                          │   │
│  │  ├── Prefabs/           ← 预制体 (编译时打包)                            │   │
│  │  └── ScriptableObjects/ ← 配置数据 (编译时打包)                          │   │
│  │                                                                          │   │
│  │  问题:                                                                   │   │
│  │  ❌ 所有内容编译时固定，无法热更新                                       │   │
│  │  ❌ 修改任何内容都需要重新发布                                           │   │
│  │  ❌ 无法支持 UGC 创作                                                    │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  改造后 (热更新架构):                                                            │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                                                                          │   │
│  │  Unity 项目 (引擎层 - 不可热更新)                                        │   │
│  │  ├── Scripts/Engine/    ← C# 引擎代码 (渲染、物理、输入等)               │   │
│  │  ├── Scripts/Bridge/    ← C# 桥接层 (Luau ↔ Unity)                       │   │
│  │  ├── Plugins/Luau/      ← Luau 运行时                                    │   │
│  │  └── Prefabs/Base/      ← 基础预制体模板                                 │   │
│  │                                                                          │   │
│  │  资源包 (玩法层 - 可热更新)                                              │   │
│  │  ├── scripts/           ← Luau 脚本 (游戏逻辑)                           │   │
│  │  ├── game_config.json   ← 游戏配置 (数值、关卡等)                        │   │
│  │  ├── atlases/           ← 图集资源 (动态加载)                            │   │
│  │  └── audio/             ← 音频资源 (动态加载)                            │   │
│  │                                                                          │   │
│  │  优势:                                                                   │   │
│  │  ✅ 游戏逻辑可热更新                                                     │   │
│  │  ✅ 资源可动态下发                                                       │   │
│  │  ✅ 支持 UGC 创作                                                        │   │
│  │  ✅ 一套引擎，无限玩法                                                   │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 1.2 改造原则

| 原则 | 说明 |
|------|------|
| **逻辑分离** | 将游戏逻辑从 C# 迁移到 Luau |
| **资源动态化** | 将静态资源改为运行时加载 |
| **配置外置** | 将 ScriptableObject 改为 JSON 配置 |
| **接口标准化** | 定义清晰的 C# ↔ Luau 桥接接口 |
| **保持兼容** | 改造后游戏行为与原版一致 |

---

## 二、改造步骤

### 2.1 改造流程图

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           游戏改造完整流程                                       │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  Step 1: 代码分析与分类                                                          │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  分析现有 C# 代码，分类为:                                               │   │
│  │  • 引擎层代码 (保留 C#)                                                  │   │
│  │  • 游戏逻辑代码 (迁移到 Luau)                                            │   │
│  │  • 配置数据 (迁移到 JSON)                                                │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 2: 设计桥接层                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  定义 Luau 可调用的 C# 接口:                                             │   │
│  │  • EntityBridge (实体操作)                                               │   │
│  │  • ResourceBridge (资源加载)                                             │   │
│  │  • PhysicsBridge (物理操作)                                              │   │
│  │  • AudioBridge (音频播放)                                                │   │
│  │  • UIBridge (UI 操作)                                                    │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 3: 迁移游戏逻辑                                                            │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  将 C# 游戏逻辑改写为 Luau:                                              │   │
│  │  • 玩家控制器 → player_controller.lua                                   │   │
│  │  • 敌人 AI → enemy_*.lua                                                 │   │
│  │  • 技能系统 → ability_*.lua                                              │   │
│  │  • 战斗系统 → combat_system.lua                                          │   │
│  │  • 关卡管理 → stage_manager.lua                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 4: 资源动态化                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  将静态资源改为动态加载:                                                 │   │
│  │  • Sprite → 图集 (atlas.png + atlas.json)                                │   │
│  │  • Animation → 动画配置 (animations.json)                                │   │
│  │  • AudioClip → 音频文件 (audio/)                                         │   │
│  │  • ScriptableObject → JSON 配置                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 5: 创建默认资源包                                                          │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  打包默认资源包:                                                         │   │
│  │  • manifest.json (资源清单)                                              │   │
│  │  • game_config.json (游戏配置)                                           │   │
│  │  • scripts/ (Luau 脚本)                                                  │   │
│  │  • atlases/ (图集资源)                                                   │   │
│  │  • audio/ (音频资源)                                                     │   │
│  │  • 生成 default_launcher_key                                             │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 6: 测试与验证                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  验证改造结果:                                                           │   │
│  │  • 游戏行为与原版一致                                                    │   │
│  │  • 热更新功能正常                                                        │   │
│  │  • 性能无明显下降                                                        │   │
│  │  • 多平台兼容                                                            │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 7: 发布游戏模板                                                            │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  发布内容:                                                               │   │
│  │  • 游戏客户端 (内置默认资源包)                                           │   │
│  │  • 模板 Schema (配置验证)                                                │   │
│  │  • 模板文档 (创作指南)                                                   │   │
│  │  • default_launcher_key                                                  │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 三、代码分类与迁移

### 3.1 代码分类标准

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           代码分类标准                                           │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    保留在 C# (引擎层)                                    │   │
│  │                                                                          │   │
│  │  特征:                                                                   │   │
│  │  • 与 Unity API 深度绑定                                                 │   │
│  │  • 性能敏感的底层操作                                                    │   │
│  │  • 平台相关的功能                                                        │   │
│  │                                                                          │   │
│  │  示例:                                                                   │   │
│  │  • 渲染管理 (SpriteRenderer, Camera)                                     │   │
│  │  • 物理系统 (Rigidbody2D, Collider2D)                                    │   │
│  │  • 输入处理 (Input, Touch)                                               │   │
│  │  • 音频播放 (AudioSource)                                                │   │
│  │  • 对象池管理                                                            │   │
│  │  • 资源加载器                                                            │   │
│  │  • Luau 运行时管理                                                       │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    迁移到 Luau (游戏逻辑层)                              │   │
│  │                                                                          │   │
│  │  特征:                                                                   │   │
│  │  • 游戏规则和玩法逻辑                                                    │   │
│  │  • 可能需要频繁调整的代码                                                │   │
│  │  • 与具体游戏内容相关                                                    │   │
│  │                                                                          │   │
│  │  示例:                                                                   │   │
│  │  • 玩家控制逻辑                                                          │   │
│  │  • 敌人 AI 行为                                                          │   │
│  │  • 技能效果实现                                                          │   │
│  │  • 战斗伤害计算                                                          │   │
│  │  • 关卡流程控制                                                          │   │
│  │  • 掉落物生成                                                            │   │
│  │  • 升级系统                                                              │   │
│  │  • 成就系统                                                              │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    迁移到 JSON (配置数据)                                │   │
│  │                                                                          │   │
│  │  特征:                                                                   │   │
│  │  • 数值配置                                                              │   │
│  │  • 静态数据表                                                            │   │
│  │  • 可由非程序员编辑                                                      │   │
│  │                                                                          │   │
│  │  示例:                                                                   │   │
│  │  • 玩家属性配置                                                          │   │
│  │  • 敌人属性配置                                                          │   │
│  │  • 技能数值配置                                                          │   │
│  │  • 关卡配置                                                              │   │
│  │  • 物品配置                                                              │   │
│  │  • 本地化文本                                                            │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 3.2 以 The Archer 为例的代码分类

基于之前的分析，以下是 The Archer 项目的代码分类：

#### 3.2.1 保留在 C# 的代码

```csharp
// ========== 引擎层代码 (保留 C#) ==========

// 1. 基础组件管理
public class EntityComponent : MonoBehaviour
{
    public int EntityId { get; set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    public Collider2D Collider { get; private set; }
    public LuaAnimator Animator { get; private set; }
    
    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Animator = GetComponent<LuaAnimator>();
    }
}

// 2. 对象池管理
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    
    private Dictionary<string, Queue<GameObject>> pools = new();
    private Dictionary<string, GameObject> prefabs = new();
    
    public GameObject Get(string prefabName)
    {
        if (pools.TryGetValue(prefabName, out var pool) && pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        if (prefabs.TryGetValue(prefabName, out var prefab))
        {
            return Instantiate(prefab);
        }
        
        return null;
    }
    
    public void Return(GameObject obj, string prefabName)
    {
        obj.SetActive(false);
        
        if (!pools.ContainsKey(prefabName))
            pools[prefabName] = new Queue<GameObject>();
            
        pools[prefabName].Enqueue(obj);
    }
}

// 3. 动画播放器 (基于图集)
public class LuaAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Dictionary<string, AnimationData> animations = new();
    private AnimationData currentAnimation;
    private int currentFrame;
    private float frameTimer;
    
    public void LoadAnimations(string animationsJson)
    {
        var config = JsonUtility.FromJson<AnimationsConfig>(animationsJson);
        foreach (var anim in config.animations)
        {
            animations[anim.id] = anim;
        }
    }
    
    public void Play(string animationId)
    {
        if (animations.TryGetValue(animationId, out var anim))
        {
            currentAnimation = anim;
            currentFrame = 0;
            frameTimer = 0;
        }
    }
    
    void Update()
    {
        if (currentAnimation == null) return;
        
        frameTimer += Time.deltaTime;
        if (frameTimer >= currentAnimation.frame_duration)
        {
            frameTimer = 0;
            currentFrame++;
            
            if (currentFrame >= currentAnimation.frames.Length)
            {
                if (currentAnimation.loop)
                    currentFrame = 0;
                else
                    currentFrame = currentAnimation.frames.Length - 1;
            }
            
            // 更新精灵
            var spriteRef = currentAnimation.frames[currentFrame];
            var sprite = ResourceBridge.GetSprite(spriteRef);
            if (sprite != null)
                spriteRenderer.sprite = sprite;
        }
    }
}

// 4. 碰撞检测转发
public class CollisionForwarder : MonoBehaviour
{
    public int EntityId { get; set; }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        var otherEntity = other.GetComponent<EntityComponent>();
        if (otherEntity != null)
        {
            LuaManager.Instance.CallFunction("OnCollisionEnter", EntityId, otherEntity.EntityId);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        var otherEntity = other.GetComponent<EntityComponent>();
        if (otherEntity != null)
        {
            LuaManager.Instance.CallFunction("OnCollisionExit", EntityId, otherEntity.EntityId);
        }
    }
}

// 5. 输入管理器
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    private Vector2 moveInput;
    private bool attackPressed;
    
    void Update()
    {
        // 键盘输入
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        attackPressed = Input.GetButton("Fire1");
        
        // 触摸输入 (移动端)
        #if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            // 处理虚拟摇杆
        }
        #endif
    }
    
    public Vector2 GetMoveInput() => moveInput;
    public bool IsAttackPressed() => attackPressed;
}
```

#### 3.2.2 迁移到 Luau 的代码

```lua
-- ========== 游戏逻辑层代码 (迁移到 Luau) ==========

-- 1. 玩家控制器 (原 PlayerBehavior.cs)
-- player_controller.lua

local PlayerController = {}
PlayerController.__index = PlayerController

function PlayerController.new(entityId, config)
    local self = setmetatable({}, PlayerController)
    
    self.entityId = entityId
    self.config = config
    
    -- 属性 (原 PlayerStatsHandler)
    self.stats = {
        maxHp = config.base_hp,
        currentHp = config.base_hp,
        damage = config.base_damage,
        moveSpeed = config.base_movement_speed,
        attackSpeed = config.base_attack_speed,
        critChance = config.base_crit_chance,
        critDamage = config.base_crit_damage
    }
    
    -- 属性修正器
    self.modifiers = {
        damageMultiplier = 1.0,
        moveSpeedMultiplier = 1.0,
        attackSpeedMultiplier = 1.0,
        damageAdditive = 0,
        moveSpeedAdditive = 0
    }
    
    -- 状态
    self.attackCooldown = 0
    self.facing = 1
    self.abilities = {}
    self.level = 1
    self.experience = 0
    
    return self
end

function PlayerController:Update(dt)
    -- 处理移动输入
    local moveX, moveY = InputBridge.GetMoveInput()
    
    if moveX ~= 0 or moveY ~= 0 then
        local speed = self:GetMoveSpeed()
        EntityBridge.Move(self.entityId, moveX * speed, moveY * speed)
        EntityBridge.PlayAnimation(self.entityId, "player_walk")
        
        if moveX ~= 0 then
            self.facing = moveX > 0 and 1 or -1
            EntityBridge.SetFacing(self.entityId, self.facing)
        end
    else
        EntityBridge.PlayAnimation(self.entityId, "player_idle")
    end
    
    -- 处理攻击
    self.attackCooldown = self.attackCooldown - dt
    if self.attackCooldown <= 0 then
        self:TryAttack()
    end
    
    -- 更新能力
    for _, ability in ipairs(self.abilities) do
        if ability.Update then
            ability:Update(dt)
        end
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
    
    -- 计算伤害
    local damage = self:GetDamage()
    local isCrit = math.random() < self.stats.critChance
    if isCrit then
        damage = damage * self.stats.critDamage
    end
    
    -- 创建投射物
    local projectileId = EntityManager.CreateProjectile("arrow", pos.x, pos.y, dirX, dirY, damage, self.entityId)
    
    -- 触发攻击事件 (用于能力处理)
    EventSystem.Dispatch("player_attack", self.entityId, targetId, projectileId, isCrit)
    
    -- 播放音效
    AudioBridge.PlaySFX("audio/sfx/bow_shoot.ogg")
    
    -- 设置冷却
    self.attackCooldown = 1.0 / self:GetAttackSpeed()
end

function PlayerController:GetDamage()
    return (self.stats.damage + self.modifiers.damageAdditive) * self.modifiers.damageMultiplier
end

function PlayerController:GetMoveSpeed()
    return (self.stats.moveSpeed + self.modifiers.moveSpeedAdditive) * self.modifiers.moveSpeedMultiplier
end

function PlayerController:GetAttackSpeed()
    return self.stats.attackSpeed * self.modifiers.attackSpeedMultiplier
end

function PlayerController:TakeDamage(damage, sourceId)
    self.stats.currentHp = self.stats.currentHp - damage
    
    -- 播放受伤动画
    EntityBridge.PlayAnimation(self.entityId, "player_hurt")
    
    -- 播放音效
    AudioBridge.PlaySFX("audio/sfx/player_hurt.ogg")
    
    -- 触发受伤事件
    EventSystem.Dispatch("player_damaged", self.entityId, damage, sourceId)
    
    -- 检查死亡
    if self.stats.currentHp <= 0 then
        self:Die()
    end
end

function PlayerController:Die()
    EventSystem.Dispatch("player_died", self.entityId)
end

function PlayerController:AddAbility(abilityId, level)
    local abilityConfig = ResourceBridge.GetAbilityConfig(abilityId)
    local AbilityClass = require("abilities." .. abilityConfig.script_name)
    
    local ability = AbilityClass.new(self.entityId, abilityConfig, level)
    table.insert(self.abilities, ability)
    
    -- 应用被动效果
    if ability.ApplyPassive then
        ability:ApplyPassive(self.modifiers)
    end
    
    EventSystem.Dispatch("ability_acquired", abilityId, level)
end

return PlayerController


-- 2. 敌人 AI (原 EnemyBehavior.cs)
-- enemies/skeleton.lua

local EnemyBase = require("entities.enemies.enemy_base")
local Skeleton = setmetatable({}, {__index = EnemyBase})
Skeleton.__index = Skeleton

function Skeleton.new(entityId, config)
    local self = setmetatable(EnemyBase.new(entityId, config), Skeleton)
    
    self.attackRange = 1.5
    self.detectionRange = 8.0
    self.attackCooldown = 0
    self.attackDelay = config.attack_delay or 1.5
    self.state = "idle"
    self.target = nil
    
    return self
end

function Skeleton:Update(dt)
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
    
    -- 延迟造成伤害
    Timer.After(0.24, function()
        if self.target then
            local dist = EntityBridge.GetDistance(self.entityId, self.target)
            if dist <= self.attackRange then
                EntityManager.DamageEntity(self.target, self.config.base_damage, self.entityId)
            end
        end
    end)
end

return Skeleton


-- 3. 技能效果 (原 Ability 相关代码)
-- abilities/freeze.lua

local AbilityBase = require("abilities.ability_base")
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
    self.onProjectileHit = function(projectileId, targetId, damage)
        self:OnProjectileHit(projectileId, targetId, damage)
    end
    EventSystem.On("projectile_hit", self.onProjectileHit)
    
    return self
end

function Freeze:ApplyPassive(modifiers)
    modifiers.damageMultiplier = modifiers.damageMultiplier * self.damageMultiplier
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
            speedMultiplier = 0.3,
            tint = {r = 0.5, g = 0.8, b = 1.0}
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
                
                local pos = EntityBridge.GetPosition(targetId)
                EffectSystem.PlayEffect("ice_dot", pos.x, pos.y)
            end
        end)
    end
end

function Freeze:OnDestroy()
    EventSystem.Off("projectile_hit", self.onProjectileHit)
end

return Freeze
```

#### 3.2.3 迁移到 JSON 的配置

```json
// ========== 配置数据 (迁移到 JSON) ==========

// 原 PlayerData.asset → game_config.json 中的 player 部分
{
  "player": {
    "base_hp": 100,
    "base_damage": 10,
    "base_movement_speed": 5.0,
    "base_attack_speed": 1.0,
    "base_crit_chance": 0.05,
    "base_crit_damage": 1.5,
    "hp_per_level": 10,
    "damage_per_level": 2,
    "experience_curve": [100, 200, 350, 550, 800, 1100, 1450, 1850, 2300, 2800]
  }
}

// 原 EnemyData.asset → game_config.json 中的 enemies 部分
{
  "enemies": {
    "skeleton": {
      "id": "skeleton",
      "name": "骷髅兵",
      "base_health": 50,
      "base_damage": 10,
      "movement_speed": 2.0,
      "attack_delay": 1.5,
      "knockback_resistance": 0.2,
      "experience_reward": 10,
      "gold_reward": 5,
      "sprite_ref": "characters:skeleton_idle",
      "script": "skeleton"
    }
  }
}

// 原 AbilityData.asset → game_config.json 中的 abilities 部分
{
  "abilities": {
    "freeze": {
      "id": 1,
      "name": "冰冻",
      "description": "攻击附带冰冻效果",
      "category": "elemental",
      "rarity": "common",
      "max_level": 5,
      "icon_ref": "icons:ability_freeze",
      "script_name": "freeze",
      "levels": [
        {
          "level": 1,
          "effect_duration": 1.0,
          "arrow_damage_multiplier": 1.0,
          "effect_deals_dot": false
        },
        {
          "level": 2,
          "effect_duration": 1.5,
          "arrow_damage_multiplier": 1.1,
          "effect_deals_dot": false
        },
        {
          "level": 3,
          "effect_duration": 2.0,
          "arrow_damage_multiplier": 1.2,
          "effect_deals_dot": true,
          "effect_damage_multiplier": 0.1,
          "effect_damage_interval": 0.5
        }
      ]
    }
  }
}
```

---

## 四、桥接层设计

### 4.1 桥接层接口定义

```csharp
// ========== 桥接层接口 ==========

// EntityBridge.cs - 实体操作桥接
[LuaCallCSharp]
public static class EntityBridge
{
    // 创建实体
    public static int CreateEntity(string entityType, float x, float y);
    public static int SpawnEntity(string entityType, float x, float y);
    public static void DestroyEntity(int entityId);
    
    // 位置操作
    public static Vector3 GetPosition(int entityId);
    public static void SetPosition(int entityId, float x, float y);
    public static void Move(int entityId, float vx, float vy);
    
    // 外观操作
    public static void SetSprite(int entityId, string spriteRef);
    public static void SetFacing(int entityId, int direction);
    public static void SetColor(int entityId, float r, float g, float b, float a);
    public static void SetScale(int entityId, float scaleX, float scaleY);
    
    // 动画操作
    public static void PlayAnimation(int entityId, string animationId);
    public static void StopAnimation(int entityId);
    
    // 物理操作
    public static void SetVelocity(int entityId, float vx, float vy);
    public static Vector2 GetVelocity(int entityId);
    public static void AddForce(int entityId, float fx, float fy);
    
    // 碰撞操作
    public static void SetColliderEnabled(int entityId, bool enabled);
    public static void SetTrigger(int entityId, bool isTrigger);
    
    // 实体查询
    public static float GetDistance(int entityId1, int entityId2);
    public static int[] FindEntitiesInRange(float x, float y, float radius, string tag);
}

// ResourceBridge.cs - 资源加载桥接
[LuaCallCSharp]
public static class ResourceBridge
{
    // 精灵加载
    public static Sprite GetSprite(string spriteRef);
    public static Sprite[] GetAnimationFrames(string animationId);
    
    // 配置加载
    public static string GetConfig(string key);
    public static LuaTable GetEntityConfig(string entityId);
    public static LuaTable GetAbilityConfig(int abilityId);
    public static LuaTable GetStageConfig(int stageId);
    
    // 本地化
    public static string GetLocalizedText(string key);
}

// AudioBridge.cs - 音频播放桥接
[LuaCallCSharp]
public static class AudioBridge
{
    public static void PlayBGM(string audioPath, float volume);
    public static void StopBGM();
    public static void PauseBGM(bool pause);
    public static void SetBGMVolume(float volume);
    
    public static void PlaySFX(string audioPath, float volume);
    public static void PlaySFXAtPosition(string audioPath, float x, float y, float volume);
}

// PhysicsBridge.cs - 物理操作桥接
[LuaCallCSharp]
public static class PhysicsBridge
{
    public static int[] Raycast(float startX, float startY, float dirX, float dirY, float distance, string layerMask);
    public static int[] OverlapCircle(float x, float y, float radius, string layerMask);
    public static int[] OverlapBox(float x, float y, float width, float height, string layerMask);
}

// InputBridge.cs - 输入桥接
[LuaCallCSharp]
public static class InputBridge
{
    public static (float, float) GetMoveInput();
    public static bool IsAttackPressed();
    public static bool IsButtonDown(string buttonName);
    public static bool IsButtonUp(string buttonName);
    public static (float, float) GetMousePosition();
    public static (float, float) GetTouchPosition(int touchIndex);
}

// UIBridge.cs - UI 操作桥接
[LuaCallCSharp]
public static class UIBridge
{
    public static void ShowHUD();
    public static void HideHUD();
    public static void UpdateHP(float current, float max);
    public static void UpdateExperience(float current, float max);
    public static void UpdateLevel(int level);
    public static void ShowAbilitySelect(LuaTable abilities);
    public static void ShowGameOver(LuaTable playerData);
    public static void ShowVictory(LuaTable playerData);
    public static void ShowPauseMenu();
    public static void ShowDamageNumber(float x, float y, int damage, bool isCrit);
}

// EffectBridge.cs - 特效桥接
[LuaCallCSharp]
public static class EffectBridge
{
    public static void PlayEffect(string effectId, float x, float y);
    public static void PlayEffect(string effectId, float x, float y, float rotation, float scale);
    public static int CreateParticle(string particleId, float x, float y);
    public static void StopParticle(int particleId);
}
```

---

## 五、默认资源包结构

### 5.1 默认资源包目录

```
default_package/
├── manifest.json                        # 资源包清单
├── game_config.json                     # 游戏配置
│
├── scripts/                             # Luau 脚本
│   ├── main.lua                         # 入口脚本
│   ├── init.lua                         # 初始化
│   │
│   ├── core/                            # 核心系统
│   │   ├── game_manager.lua
│   │   ├── stage_manager.lua
│   │   ├── entity_manager.lua
│   │   └── event_system.lua
│   │
│   ├── entities/                        # 实体脚本
│   │   ├── player/
│   │   │   └── player_controller.lua
│   │   └── enemies/
│   │       ├── enemy_base.lua
│   │       ├── skeleton.lua
│   │       ├── goblin.lua
│   │       └── ...
│   │
│   ├── abilities/                       # 能力脚本
│   │   ├── ability_base.lua
│   │   ├── freeze.lua
│   │   ├── ignite.lua
│   │   └── ...
│   │
│   ├── combat/                          # 战斗系统
│   │   ├── damage_system.lua
│   │   ├── projectile_system.lua
│   │   └── drop_system.lua
│   │
│   └── utils/                           # 工具库
│       ├── timer.lua
│       └── math_utils.lua
│
├── build/                               # 构建产物
│   ├── atlases/
│   │   ├── characters.atlas.json
│   │   ├── characters.atlas.png
│   │   ├── icons.atlas.json
│   │   ├── icons.atlas.png
│   │   └── effects.atlas.png
│   │
│   ├── animations/
│   │   └── animations.json
│   │
│   └── audio/
│       ├── audio_manifest.json
│       ├── bgm/
│       └── sfx/
│
└── localization/
    ├── en.json
    └── zh.json
```

### 5.2 默认 manifest.json

```json
{
  "manifest_version": "1.0",
  
  "package_info": {
    "id": "default_package",
    "name": "The Archer - Default",
    "description": "弓箭传说默认玩法包",
    "version": "1.0.0",
    "author": {
      "id": "system",
      "name": "System"
    }
  },
  
  "template": {
    "id": "template_roguelike",
    "version": "1.0.0"
  },
  
  "launcher_key": "system-default_package-1.0.0-default",
  
  "is_default": true,
  
  "content": {
    "config": {
      "game_config": "game_config.json"
    },
    "scripts": {
      "entry": "scripts/main.lua"
    },
    "atlases": [
      {"name": "characters", "path": "build/atlases/characters.atlas.json"},
      {"name": "icons", "path": "build/atlases/icons.atlas.json"},
      {"name": "effects", "path": "build/atlases/effects.atlas.json"}
    ],
    "audio": {
      "manifest": "build/audio/audio_manifest.json"
    },
    "localization": {
      "default": "zh",
      "files": {
        "en": "localization/en.json",
        "zh": "localization/zh.json"
      }
    }
  }
}
```

---

## 六、AI 辅助创作流程

### 6.1 自然语言创作流程

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                        AI 辅助自然语言创作流程                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  Step 1: 启动编辑器，加载默认资源包                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  创作者: "我想基于弓箭传说模板创建一个太空主题的游戏"                     │   │
│  │                                                                          │   │
│  │  AI: "好的，我来帮你创建太空主题的弓箭传说。让我先：                      │   │
│  │       1. 复制默认资源包作为基础                                          │   │
│  │       2. 准备修改美术风格和配置"                                         │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 2: AI 生成美术资源                                                         │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  创作者: "把主角改成宇航员，敌人改成外星人"                               │   │
│  │                                                                          │   │
│  │  AI: "正在生成太空风格的角色..."                                         │   │
│  │       → 生成宇航员精灵 (idle, walk, attack 动画)                         │   │
│  │       → 生成外星人精灵 (多种类型)                                        │   │
│  │       → 生成太空背景                                                     │   │
│  │       → 生成激光特效 (替代箭矢)                                          │   │
│  │                                                                          │   │
│  │  [预览] 显示生成的资源                                                   │   │
│  │  创作者: "外星人的颜色改成绿色"                                          │   │
│  │  AI: "好的，正在调整..." → 重新生成                                      │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 3: AI 调整游戏配置                                                         │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  创作者: "我想让游戏更难一点，敌人更多更快"                               │   │
│  │                                                                          │   │
│  │  AI: "我来调整难度配置：                                                 │   │
│  │       • 敌人生成数量 +50%                                                │   │
│  │       • 敌人移动速度 +30%                                                │   │
│  │       • 敌人生命值 +20%                                                  │   │
│  │       • 波次间隔 -20%"                                                   │   │
│  │                                                                          │   │
│  │  创作者: "再加一个新技能，叫'黑洞'，可以吸引周围的敌人"                   │   │
│  │                                                                          │   │
│  │  AI: "好的，我来创建黑洞技能：                                           │   │
│  │       1. 生成黑洞图标                                                    │   │
│  │       2. 生成黑洞特效                                                    │   │
│  │       3. 编写黑洞技能脚本                                                │   │
│  │       4. 添加到技能配置"                                                 │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 4: 实时预览测试                                                            │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  创作者: "让我试玩一下"                                                  │   │
│  │                                                                          │   │
│  │  AI: "正在启动预览..."                                                   │   │
│  │       → 热加载修改后的资源包                                             │   │
│  │       → 启动游戏预览模式                                                 │   │
│  │                                                                          │   │
│  │  [游戏预览窗口]                                                          │   │
│  │                                                                          │   │
│  │  创作者: "黑洞技能太强了，吸引范围减小一点"                               │   │
│  │  AI: "好的，将黑洞范围从 5 减小到 3" → 热重载                            │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 5: 发布新资源包                                                            │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  创作者: "满意了，发布这个游戏"                                          │   │
│  │                                                                          │   │
│  │  AI: "正在打包发布..."                                                   │   │
│  │       1. 打包图集资源                                                    │   │
│  │       2. 合并配置文件                                                    │   │
│  │       3. 压缩资源包                                                      │   │
│  │       4. 生成 launcher_key                                               │   │
│  │       5. 上传到服务器                                                    │   │
│  │                                                                          │   │
│  │  AI: "发布成功！                                                         │   │
│  │       游戏名称: 太空幸存者                                               │   │
│  │       launcher_key: creator_001-space_survivor-1.0.0-abc123              │   │
│  │       分享链接: https://game.example.com/play/abc123"                    │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 6.2 AI Agent 能力清单

```json
{
  "ai_agents": {
    "sprite_generator": {
      "name": "精灵生成 Agent",
      "capabilities": [
        "generate_character",
        "generate_enemy",
        "generate_icon",
        "generate_effect",
        "generate_background",
        "remove_background",
        "change_color",
        "upscale_image",
        "style_transfer"
      ],
      "prompts": {
        "generate_character": "生成一个{style}风格的{description}角色，{size}像素，透明背景",
        "generate_enemy": "生成一个{style}风格的{description}敌人，{size}像素，透明背景",
        "style_transfer": "将这个角色转换为{target_style}风格"
      }
    },
    
    "animation_generator": {
      "name": "动画生成 Agent",
      "capabilities": [
        "generate_walk_cycle",
        "generate_attack_animation",
        "generate_idle_animation",
        "generate_death_animation",
        "interpolate_frames"
      ]
    },
    
    "config_optimizer": {
      "name": "配置优化 Agent",
      "capabilities": [
        "adjust_difficulty",
        "balance_stats",
        "create_ability",
        "modify_ability",
        "create_enemy",
        "modify_enemy",
        "create_stage",
        "modify_stage"
      ],
      "natural_language_commands": [
        {"pattern": "让游戏更难", "action": "increase_difficulty"},
        {"pattern": "让游戏更简单", "action": "decrease_difficulty"},
        {"pattern": "增加新技能", "action": "create_ability"},
        {"pattern": "修改技能", "action": "modify_ability"},
        {"pattern": "增加新敌人", "action": "create_enemy"}
      ]
    },
    
    "code_generator": {
      "name": "代码生成 Agent",
      "capabilities": [
        "generate_ability_script",
        "generate_enemy_script",
        "modify_script",
        "fix_bug",
        "explain_code"
      ]
    },
    
    "localization_generator": {
      "name": "本地化 Agent",
      "capabilities": [
        "translate_text",
        "generate_description",
        "adapt_cultural_content"
      ]
    }
  }
}
```

---

## 七、改造检查清单

### 7.1 代码改造检查

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 玩家控制逻辑迁移到 Luau | ⬜ | player_controller.lua |
| 敌人 AI 迁移到 Luau | ⬜ | enemies/*.lua |
| 技能系统迁移到 Luau | ⬜ | abilities/*.lua |
| 战斗系统迁移到 Luau | ⬜ | combat/*.lua |
| 关卡管理迁移到 Luau | ⬜ | stage_manager.lua |
| 掉落系统迁移到 Luau | ⬜ | drop_system.lua |
| 升级系统迁移到 Luau | ⬜ | level_system.lua |
| C# 桥接层实现 | ⬜ | EntityBridge, ResourceBridge 等 |
| Luau 运行时集成 | ⬜ | LuaManager.cs |

### 7.2 资源改造检查

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 精灵资源导出为图集 | ⬜ | atlases/*.atlas.png |
| 动画配置导出为 JSON | ⬜ | animations.json |
| 音频资源整理 | ⬜ | audio/ 目录 |
| ScriptableObject 转 JSON | ⬜ | game_config.json |
| 本地化文本导出 | ⬜ | localization/*.json |

### 7.3 功能验证检查

| 检查项 | 状态 | 说明 |
|--------|------|------|
| 游戏基本流程正常 | ⬜ | 开始、游戏、结束 |
| 玩家控制正常 | ⬜ | 移动、攻击 |
| 敌人行为正常 | ⬜ | 追逐、攻击 |
| 技能效果正常 | ⬜ | 所有技能 |
| 关卡流程正常 | ⬜ | 波次、Boss |
| 热更新功能正常 | ⬜ | 脚本、配置、资源 |
| 多平台兼容 | ⬜ | PC、移动、WebGL |

---

## 八、总结

通过本指南的改造流程，可以将传统的 Unity C# 游戏改造成支持热更新的架构：

1. ✅ **代码分离** - 引擎层 (C#) + 游戏逻辑层 (Luau)
2. ✅ **资源动态化** - 静态资源 → 运行时加载的图集/配置
3. ✅ **配置外置** - ScriptableObject → JSON 配置
4. ✅ **默认资源包** - 内置完整可运行的默认玩法
5. ✅ **AI 辅助创作** - 自然语言驱动的游戏创作
6. ✅ **热更新发布** - 通过 launcher_key 分发新玩法

改造完成后，游戏将支持：
- 通过 `default_launcher_key` 运行默认玩法
- 通过编辑器创建新的资源包
- 通过 AI 辅助快速调整和创新
- 通过新的 `launcher_key` 发布和分享新玩法

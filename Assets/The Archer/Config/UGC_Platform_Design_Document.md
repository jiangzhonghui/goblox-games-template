# 🎮 UGC Roguelike 游戏平台设计文档

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **项目名称**: The Archer Roguelike - UGC Platform

---

## 目录

1. [概述](#一概述)
2. [系统架构](#二系统架构)
3. [Launcher Key 机制](#三launcher-key-机制)
4. [配置包结构](#四配置包结构)
5. [客户端架构](#五客户端架构)
6. [服务端架构](#六服务端架构)
7. [Meta UI 编辑器](#七meta-ui-编辑器)
8. [AI 多模态集成](#八ai-多模态集成)
9. [资源管理系统](#九资源管理系统)
10. [发布流程](#十发布流程)
11. [技术实现方案](#十一技术实现方案)
12. [安全与审核](#十二安全与审核)

---

## 一、概述

### 1.1 核心理念

**一套代码，无限玩法** - 通过 `launcher_key` 机制，让同一个游戏客户端能够加载不同的配置包和资源包，实现完全不同的游戏体验。

### 1.2 核心特性

| 特性 | 说明 |
|------|------|
| **配置驱动** | 游戏玩法完全由配置文件驱动，无需修改代码 |
| **资源热加载** | 支持动态下载和加载角色、敌人、图标等资源 |
| **UGC创作** | 玩家可以通过可视化界面创作自己的游戏配置 |
| **AI辅助** | 集成多模态大模型，辅助生成和优化游戏资源 |
| **云端发布** | 一键发布配置包，生成唯一的 launcher_key |

### 1.3 用户角色

```
┌─────────────────────────────────────────────────────────────────┐
│                        用户角色定义                              │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │
│  │   玩家      │  │   创作者    │  │   审核员    │              │
│  │  (Player)   │  │  (Creator)  │  │ (Moderator) │              │
│  ├─────────────┤  ├─────────────┤  ├─────────────┤              │
│  │ 输入Key启动 │  │ 编辑配置   │  │ 审核内容    │              │
│  │ 体验游戏   │  │ 生成资源   │  │ 管理发布    │              │
│  │ 评价分享   │  │ 发布作品   │  │ 处理举报    │              │
│  └─────────────┘  └─────────────┘  └─────────────┘              │
└─────────────────────────────────────────────────────────────────┘
```

---

## 二、系统架构

### 2.1 整体架构图

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              UGC Platform                                │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌──────────────────────────────────────────────────────────────────┐   │
│  │                         客户端 (Unity)                            │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐               │   │
│  │  │ Launcher    │  │ Config      │  │ Resource    │               │   │
│  │  │ Manager     │  │ Loader      │  │ Manager     │               │   │
│  │  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘               │   │
│  │         │                │                │                       │   │
│  │  ┌──────▼──────────────────────────────────────────────────────┐ │   │
│  │  │                    Game Runtime Engine                       │ │   │
│  │  │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐            │ │   │
│  │  │  │ Stage   │ │ Player  │ │ Enemy   │ │Abilities│            │ │   │
│  │  │  │ System  │ │ System  │ │ System  │ │ System  │            │ │   │
│  │  │  └─────────┘ └─────────┘ └─────────┘ └─────────┘            │ │   │
│  │  └─────────────────────────────────────────────────────────────┘ │   │
│  │                                                                   │   │
│  │  ┌─────────────────────────────────────────────────────────────┐ │   │
│  │  │                    Meta UI Editor                            │ │   │
│  │  │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐            │ │   │
│  │  │  │ Stage   │ │ Player  │ │ Enemy   │ │ Asset   │            │ │   │
│  │  │  │ Editor  │ │ Editor  │ │ Editor  │ │ Editor  │            │ │   │
│  │  │  └─────────┘ └─────────┘ └─────────┘ └─────────┘            │ │   │
│  │  └─────────────────────────────────────────────────────────────┘ │   │
│  └──────────────────────────────────────────────────────────────────┘   │
│                                    │                                     │
│                                    │ HTTPS                               │
│                                    ▼                                     │
│  ┌──────────────────────────────────────────────────────────────────┐   │
│  │                         服务端 (Cloud)                            │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐               │   │
│  │  │ API Gateway │  │ Config      │  │ Asset       │               │   │
│  │  │             │  │ Service     │  │ CDN         │               │   │
│  │  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘               │   │
│  │         │                │                │                       │   │
│  │  ┌──────▼──────────────────────────────────────────────────────┐ │   │
│  │  │                    Backend Services                          │ │   │
│  │  │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐            │ │   │
│  │  │  │ Auth    │ │ Package │ │ AI      │ │ Review  │            │ │   │
│  │  │  │ Service │ │ Service │ │ Service │ │ Service │            │ │   │
│  │  │  └─────────┘ └─────────┘ └─────────┘ └─────────┘            │ │   │
│  │  └─────────────────────────────────────────────────────────────┘ │   │
│  │                                                                   │   │
│  │  ┌─────────────────────────────────────────────────────────────┐ │   │
│  │  │                    Storage Layer                             │ │   │
│  │  │  ┌─────────┐ ┌─────────┐ ┌─────────┐                        │ │   │
│  │  │  │ MongoDB │ │ Redis   │ │ OSS/S3  │                        │ │   │
│  │  │  │(Config) │ │(Cache)  │ │(Assets) │                        │ │   │
│  │  │  └─────────┘ └─────────┘ └─────────┘                        │ │   │
│  │  └─────────────────────────────────────────────────────────────┘ │   │
│  └──────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 2.2 数据流向

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ 输入        │     │ 服务端      │     │ 客户端      │     │ 游戏运行    │
│ launcher_key│ ──► │ 验证&获取   │ ──► │ 下载&解析   │ ──► │ 加载&启动   │
└─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
```

---

## 三、Launcher Key 机制

### 3.1 Key 结构设计

```
launcher_key 格式: {creator_id}-{package_id}-{version}-{checksum}

示例: USR001-PKG0042-V1.2.0-A3B5C7D9

组成部分:
├── creator_id  : 创作者ID (6位)
├── package_id  : 配置包ID (7位)
├── version     : 版本号 (语义化版本)
└── checksum    : 校验码 (8位十六进制)
```

### 3.2 Key 生命周期

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   创建      │     │   审核      │     │   发布      │     │   使用      │
│  (Draft)    │ ──► │ (Pending)   │ ──► │ (Published) │ ──► │ (Active)    │
└─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
                                                                   │
                                                                   ▼
                                                            ┌─────────────┐
                                                            │   过期/下架  │
                                                            │ (Deprecated)│
                                                            └─────────────┘
```

### 3.3 Key 数据库结构

```json
{
  "launcher_key": "USR001-PKG0042-V1.2.0-A3B5C7D9",
  "creator_id": "USR001",
  "package_id": "PKG0042",
  "version": "1.2.0",
  "checksum": "A3B5C7D9",
  "status": "published",
  "created_at": "2025-01-29T10:00:00Z",
  "published_at": "2025-01-29T12:00:00Z",
  "download_count": 1234,
  "rating": 4.5,
  "config_url": "https://cdn.example.com/packages/PKG0042/v1.2.0/config.json",
  "assets_manifest_url": "https://cdn.example.com/packages/PKG0042/v1.2.0/manifest.json",
  "metadata": {
    "title": "Dark Forest Adventure",
    "description": "A challenging roguelike in a dark forest",
    "tags": ["dark", "forest", "hard"],
    "thumbnail": "https://cdn.example.com/packages/PKG0042/thumbnail.png",
    "preview_images": [...]
  }
}
```

---

## 四、配置包结构

### 4.1 包目录结构

```
package_root/
├── manifest.json              # 包清单文件
├── config/
│   ├── game_config.json       # 游戏主配置
│   ├── stage_config.json      # 关卡配置
│   ├── player_config.json     # 玩家配置
│   ├── enemy_config.json      # 敌人配置
│   ├── ability_config.json    # 能力配置
│   └── item_config.json       # 物品配置
├── assets/
│   ├── icons/                 # 图标资源
│   │   ├── abilities/
│   │   ├── items/
│   │   └── enemies/
│   ├── sprites/               # 精灵图资源
│   │   ├── player/
│   │   ├── enemies/
│   │   └── effects/
│   ├── models/                # 3D模型资源 (AssetBundle)
│   │   ├── player.bundle
│   │   ├── enemies.bundle
│   │   └── weapons.bundle
│   ├── audio/                 # 音频资源
│   │   ├── bgm/
│   │   └── sfx/
│   └── ui/                    # UI资源
│       ├── loading.png
│       ├── background.png
│       └── logo.png
└── localization/              # 本地化文件
    ├── en.json
    ├── zh-CN.json
    └── ja.json
```

### 4.2 Manifest 文件结构

```json
{
  "manifest_version": "1.0",
  "package_info": {
    "id": "PKG0042",
    "name": "Dark Forest Adventure",
    "version": "1.2.0",
    "min_client_version": "2.0.0",
    "author": {
      "id": "USR001",
      "name": "GameCreator",
      "avatar": "https://..."
    },
    "created_at": "2025-01-29T10:00:00Z",
    "updated_at": "2025-01-29T12:00:00Z"
  },
  "config_files": [
    {
      "name": "game_config",
      "path": "config/game_config.json",
      "hash": "sha256:abc123...",
      "size": 15360
    },
    {
      "name": "stage_config",
      "path": "config/stage_config.json",
      "hash": "sha256:def456...",
      "size": 8192
    }
  ],
  "asset_bundles": [
    {
      "name": "player",
      "path": "assets/models/player.bundle",
      "hash": "sha256:ghi789...",
      "size": 2048000,
      "dependencies": []
    },
    {
      "name": "enemies",
      "path": "assets/models/enemies.bundle",
      "hash": "sha256:jkl012...",
      "size": 5120000,
      "dependencies": ["shared_materials"]
    }
  ],
  "loose_assets": [
    {
      "type": "icon",
      "path": "assets/icons/abilities/*.png",
      "count": 80
    },
    {
      "type": "audio",
      "path": "assets/audio/bgm/*.ogg",
      "count": 5
    }
  ],
  "total_size": 52428800,
  "checksum": "sha256:xyz..."
}
```

---

## 五、客户端架构

### 5.1 核心模块

```csharp
// 启动管理器
public class LauncherManager : MonoBehaviour
{
    public static LauncherManager Instance { get; private set; }
    
    [Header("Configuration")]
    public string serverBaseUrl = "https://api.roguelike-platform.com";
    
    private string currentLauncherKey;
    private PackageManifest currentManifest;
    
    // 启动流程
    public async Task<bool> LaunchWithKey(string launcherKey)
    {
        // 1. 验证 Key
        var validation = await ValidateKey(launcherKey);
        if (!validation.isValid) return false;
        
        // 2. 获取 Manifest
        currentManifest = await FetchManifest(validation.manifestUrl);
        
        // 3. 检查本地缓存
        var downloadList = await CheckLocalCache(currentManifest);
        
        // 4. 下载缺失资源
        await DownloadAssets(downloadList, OnProgressUpdate);
        
        // 5. 加载配置
        await ConfigLoader.LoadAllConfigs(currentManifest);
        
        // 6. 预加载资源
        await ResourceManager.PreloadAssets(currentManifest);
        
        // 7. 启动游戏
        GameManager.Instance.StartGame();
        
        return true;
    }
}
```

```csharp
// 配置加载器
public class ConfigLoader : MonoBehaviour
{
    public static GameConfig GameConfig { get; private set; }
    public static StageConfig StageConfig { get; private set; }
    public static PlayerConfig PlayerConfig { get; private set; }
    public static EnemyConfig EnemyConfig { get; private set; }
    public static AbilityConfig AbilityConfig { get; private set; }
    
    public static async Task LoadAllConfigs(PackageManifest manifest)
    {
        var tasks = new List<Task>
        {
            LoadConfig<GameConfig>("game_config", manifest),
            LoadConfig<StageConfig>("stage_config", manifest),
            LoadConfig<PlayerConfig>("player_config", manifest),
            LoadConfig<EnemyConfig>("enemy_config", manifest),
            LoadConfig<AbilityConfig>("ability_config", manifest)
        };
        
        await Task.WhenAll(tasks);
    }
    
    private static async Task LoadConfig<T>(string name, PackageManifest manifest)
    {
        var configFile = manifest.GetConfigFile(name);
        var json = await FileManager.ReadTextAsync(configFile.localPath);
        var config = JsonUtility.FromJson<T>(json);
        
        // 使用反射设置静态属性
        typeof(ConfigLoader).GetProperty($"{typeof(T).Name}")
            .SetValue(null, config);
    }
}
```

```csharp
// 资源管理器
public class ResourceManager : MonoBehaviour
{
    private static Dictionary<string, AssetBundle> loadedBundles = new();
    private static Dictionary<string, UnityEngine.Object> assetCache = new();
    
    public static async Task PreloadAssets(PackageManifest manifest)
    {
        // 加载 AssetBundle
        foreach (var bundle in manifest.assetBundles)
        {
            await LoadAssetBundle(bundle);
        }
        
        // 预加载常用资源
        await PreloadIcons();
        await PreloadPlayerAssets();
        await PreloadCommonEnemies();
    }
    
    public static T GetAsset<T>(string path) where T : UnityEngine.Object
    {
        if (assetCache.TryGetValue(path, out var cached))
            return cached as T;
            
        // 从 AssetBundle 加载
        var asset = LoadFromBundle<T>(path);
        assetCache[path] = asset;
        return asset;
    }
    
    public static async Task<Sprite> LoadIcon(string iconPath)
    {
        // 支持从本地缓存或远程加载
        if (File.Exists(GetLocalPath(iconPath)))
        {
            return await LoadLocalSprite(iconPath);
        }
        return await DownloadAndCacheSprite(iconPath);
    }
}
```

### 5.2 游戏运行时适配

```csharp
// 敌人工厂 - 根据配置动态创建敌人
public class EnemyFactory : MonoBehaviour
{
    public static EnemyBehavior CreateEnemy(int enemyType, Vector3 position)
    {
        // 从配置获取敌人数据
        var enemyData = ConfigLoader.EnemyConfig.GetEnemy(enemyType);
        
        // 从资源管理器获取预制体
        var prefab = ResourceManager.GetAsset<GameObject>(enemyData.prefab_path);
        
        // 实例化
        var instance = Instantiate(prefab, position, Quaternion.identity);
        var behavior = instance.GetComponent<EnemyBehavior>();
        
        // 应用配置
        behavior.ApplyConfig(enemyData);
        
        return behavior;
    }
}

// 敌人行为 - 支持配置驱动
public partial class EnemyBehavior
{
    public void ApplyConfig(EnemyConfigData config)
    {
        baseHealth = config.base_health;
        baseDamage = config.base_damage;
        collisionDamageMultiplier = config.collision_damage_multiplier;
        useKnockback = config.use_knockback;
        knockbackMagnitude = config.knockback_magnitude;
        knockbackCooldown = config.knockback_cooldown;
        
        // 应用行为配置
        ApplyBehaviorConfig(config.behavior);
    }
}
```

---

## 六、服务端架构

### 6.1 API 设计

```yaml
# API 端点设计

# 认证相关
POST   /api/v1/auth/login              # 用户登录
POST   /api/v1/auth/register           # 用户注册
POST   /api/v1/auth/refresh            # 刷新Token

# Launcher Key 相关
GET    /api/v1/launcher/validate/{key} # 验证 Key
GET    /api/v1/launcher/manifest/{key} # 获取 Manifest
GET    /api/v1/launcher/popular        # 热门配置包
GET    /api/v1/launcher/search         # 搜索配置包

# 配置包管理
POST   /api/v1/packages                # 创建配置包
GET    /api/v1/packages/{id}           # 获取配置包详情
PUT    /api/v1/packages/{id}           # 更新配置包
DELETE /api/v1/packages/{id}           # 删除配置包
POST   /api/v1/packages/{id}/publish   # 发布配置包

# 资源上传
POST   /api/v1/assets/upload           # 上传资源
GET    /api/v1/assets/{id}             # 获取资源信息
DELETE /api/v1/assets/{id}             # 删除资源

# AI 服务
POST   /api/v1/ai/generate-icon        # AI 生成图标
POST   /api/v1/ai/optimize-sprite      # AI 优化精灵图
POST   /api/v1/ai/generate-model       # AI 生成模型
POST   /api/v1/ai/balance-config       # AI 平衡数值

# 社区功能
GET    /api/v1/community/feed          # 社区动态
POST   /api/v1/community/rate          # 评分
POST   /api/v1/community/comment       # 评论
POST   /api/v1/community/report        # 举报
```

### 6.2 数据库设计

```sql
-- 用户表
CREATE TABLE users (
    id VARCHAR(36) PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('player', 'creator', 'moderator', 'admin') DEFAULT 'player',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 配置包表
CREATE TABLE packages (
    id VARCHAR(36) PRIMARY KEY,
    creator_id VARCHAR(36) NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    version VARCHAR(20) NOT NULL,
    status ENUM('draft', 'pending', 'published', 'deprecated') DEFAULT 'draft',
    config_url VARCHAR(500),
    manifest_url VARCHAR(500),
    thumbnail_url VARCHAR(500),
    total_size BIGINT DEFAULT 0,
    download_count INT DEFAULT 0,
    rating_sum INT DEFAULT 0,
    rating_count INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    published_at TIMESTAMP NULL,
    FOREIGN KEY (creator_id) REFERENCES users(id)
);

-- Launcher Key 表
CREATE TABLE launcher_keys (
    key_string VARCHAR(50) PRIMARY KEY,
    package_id VARCHAR(36) NOT NULL,
    version VARCHAR(20) NOT NULL,
    checksum VARCHAR(16) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NULL,
    FOREIGN KEY (package_id) REFERENCES packages(id)
);

-- 资源表
CREATE TABLE assets (
    id VARCHAR(36) PRIMARY KEY,
    package_id VARCHAR(36) NOT NULL,
    type ENUM('icon', 'sprite', 'model', 'audio', 'ui') NOT NULL,
    name VARCHAR(100) NOT NULL,
    path VARCHAR(500) NOT NULL,
    url VARCHAR(500) NOT NULL,
    hash VARCHAR(64) NOT NULL,
    size BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (package_id) REFERENCES packages(id)
);

-- 评分表
CREATE TABLE ratings (
    id VARCHAR(36) PRIMARY KEY,
    user_id VARCHAR(36) NOT NULL,
    package_id VARCHAR(36) NOT NULL,
    score INT NOT NULL CHECK (score >= 1 AND score <= 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY unique_rating (user_id, package_id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (package_id) REFERENCES packages(id)
);
```

---

## 七、Meta UI 编辑器

### 7.1 编辑器架构

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          Meta UI Editor                                  │
├─────────────────────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │                         工具栏 (Toolbar)                         │   │
│  │  [新建] [打开] [保存] [预览] [发布] | [撤销] [重做] | [AI助手]   │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
│  ┌───────────────┐  ┌─────────────────────────────┐  ┌───────────────┐ │
│  │   导航面板    │  │        编辑区域              │  │   属性面板    │ │
│  │  (Navigator)  │  │       (Editor Area)         │  │  (Inspector)  │ │
│  │               │  │                             │  │               │ │
│  │  📁 关卡配置  │  │  ┌─────────────────────┐   │  │  名称: ___    │ │
│  │    ├─ 关卡1   │  │  │                     │   │  │  类型: ___    │ │
│  │    ├─ 关卡2   │  │  │   可视化编辑器      │   │  │  数值: ___    │ │
│  │    └─ 关卡3   │  │  │                     │   │  │               │ │
│  │  📁 玩家配置  │  │  │   (根据选择显示     │   │  │  ┌─────────┐  │ │
│  │    ├─ 属性    │  │  │    不同的编辑界面)  │   │  │  │ AI 优化 │  │ │
│  │    └─ 技能    │  │  │                     │   │  │  └─────────┘  │ │
│  │  📁 敌人配置  │  │  └─────────────────────┘   │  │               │ │
│  │  📁 能力配置  │  │                             │  │  ┌─────────┐  │ │
│  │  📁 装备配置  │  │  ┌─────────────────────┐   │  │  │ 预览    │  │ │
│  │  📁 资源管理  │  │  │    预览窗口          │   │  │  └─────────┘  │ │
│  │               │  │  └─────────────────────┘   │  │               │ │
│  └───────────────┘  └─────────────────────────────┘  └───────────────┘ │
│                                                                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │                         状态栏 (Status Bar)                      │   │
│  │  [已保存] | 配置包大小: 52.4 MB | 资源数: 156 | 版本: 1.2.0      │   │
│  └─────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────┘
```

### 7.2 编辑器功能模块

#### 关卡编辑器

```csharp
public class StageEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform stageListContainer;
    public Transform roomListContainer;
    public Transform waveListContainer;
    public Transform enemySpawnContainer;
    
    [Header("Prefabs")]
    public GameObject stageItemPrefab;
    public GameObject roomItemPrefab;
    public GameObject waveItemPrefab;
    public GameObject enemySpawnItemPrefab;
    
    // 当前编辑的数据
    private StageConfigData currentStage;
    private RoomConfigData currentRoom;
    private WaveConfigData currentWave;
    
    // 添加关卡
    public void AddStage()
    {
        var newStage = new StageConfigData
        {
            stage_id = GenerateId(),
            stage_name = "New Stage",
            base_enemy_damage_multiplier = 1.0f,
            base_enemy_hp_multiplier = 1.0f,
            rooms = new List<RoomConfigData>()
        };
        
        EditorDataManager.AddStage(newStage);
        RefreshStageList();
    }
    
    // 难度曲线编辑
    public void EditDifficultyCurve()
    {
        DifficultyCurveEditor.Show(currentStage, OnDifficultyCurveChanged);
    }
    
    // 波次可视化编辑
    public void OpenWaveVisualEditor()
    {
        WaveVisualEditor.Show(currentWave, OnWaveChanged);
    }
}
```

#### 敌人编辑器

```csharp
public class EnemyEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField enemyNameInput;
    public Slider healthSlider;
    public Slider damageSlider;
    public Toggle isBossToggle;
    public Image enemyPreviewImage;
    public Button selectModelButton;
    public Button aiGenerateButton;
    
    private EnemyConfigData currentEnemy;
    
    // 选择模型
    public void OnSelectModel()
    {
        AssetBrowser.Show(AssetType.Model, OnModelSelected);
    }
    
    // AI 生成敌人模型
    public async void OnAIGenerateModel()
    {
        var prompt = $"Generate a {currentEnemy.enemy_name} enemy model for a roguelike game";
        var result = await AIService.GenerateModel(prompt);
        
        if (result.success)
        {
            currentEnemy.prefab_path = result.modelPath;
            RefreshPreview();
        }
    }
    
    // 应用更改
    public void ApplyChanges()
    {
        currentEnemy.enemy_name = enemyNameInput.text;
        currentEnemy.base_health = healthSlider.value;
        currentEnemy.base_damage = damageSlider.value;
        currentEnemy.is_boss = isBossToggle.isOn;
        
        EditorDataManager.UpdateEnemy(currentEnemy);
    }
}
```

#### 资源管理器

```csharp
public class AssetManagerUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform assetGridContainer;
    public TMP_Dropdown assetTypeFilter;
    public TMP_InputField searchInput;
    public Button uploadButton;
    public Button aiGenerateButton;
    
    // 上传资源
    public async void OnUploadAsset()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Asset", "", "png,jpg,fbx,ogg", true);
        
        foreach (var path in paths)
        {
            var asset = await AssetUploader.Upload(path);
            EditorDataManager.AddAsset(asset);
        }
        
        RefreshAssetGrid();
    }
    
    // AI 生成图标
    public async void OnAIGenerateIcon()
    {
        AIIconGeneratorDialog.Show(async (prompt, style) =>
        {
            var result = await AIService.GenerateIcon(prompt, style);
            if (result.success)
            {
                EditorDataManager.AddAsset(result.asset);
                RefreshAssetGrid();
            }
        });
    }
    
    // 批量 AI 优化
    public async void OnBatchAIOptimize()
    {
        var selectedAssets = GetSelectedAssets();
        
        foreach (var asset in selectedAssets)
        {
            var result = await AIService.OptimizeAsset(asset);
            if (result.success)
            {
                EditorDataManager.UpdateAsset(result.optimizedAsset);
            }
        }
        
        RefreshAssetGrid();
    }
}
```

---

## 八、AI 多模态集成

### 8.1 AI 服务架构

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          AI Service Layer                                │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │                      AI Gateway                                  │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │   │
│  │  │ Rate Limit  │  │ Auth Check  │  │ Load Balance│              │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘              │   │
│  └──────────────────────────┬──────────────────────────────────────┘   │
│                             │                                           │
│  ┌──────────────────────────▼──────────────────────────────────────┐   │
│  │                      AI Services                                 │   │
│  │                                                                  │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │   │
│  │  │ Image Gen   │  │ Model Gen   │  │ Text Gen    │              │   │
│  │  │ (DALL-E/SD) │  │ (Custom)    │  │ (GPT/Claude)│              │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘              │   │
│  │                                                                  │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │   │
│  │  │ Image Edit  │  │ Style Trans │  │ Balance     │              │   │
│  │  │ (Inpaint)   │  │ (Neural)    │  │ Advisor     │              │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘              │   │
│  │                                                                  │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │                      Model Providers                             │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │   │
│  │  │ OpenAI      │  │ Stability   │  │ Custom      │              │   │
│  │  │ API         │  │ AI          │  │ Models      │              │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘              │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 8.2 AI 功能实现

```csharp
// AI 服务客户端
public class AIService
{
    private static readonly string API_BASE = "https://api.roguelike-platform.com/ai";
    
    // 生成图标
    public static async Task<AIResult<Sprite>> GenerateIcon(string prompt, IconStyle style)
    {
        var request = new IconGenerationRequest
        {
            prompt = prompt,
            style = style.ToString(),
            size = "128x128",
            format = "png"
        };
        
        var response = await HttpClient.PostAsync($"{API_BASE}/generate-icon", request);
        
        if (response.success)
        {
            var texture = await DownloadTexture(response.imageUrl);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), Vector2.one * 0.5f);
            return AIResult<Sprite>.Success(sprite);
        }
        
        return AIResult<Sprite>.Failure(response.error);
    }
    
    // 优化精灵图
    public static async Task<AIResult<Sprite>> OptimizeSprite(Sprite original, OptimizeOptions options)
    {
        var imageData = original.texture.EncodeToPNG();
        
        var request = new SpriteOptimizeRequest
        {
            imageBase64 = Convert.ToBase64String(imageData),
            upscale = options.upscale,
            removeBackground = options.removeBackground,
            enhanceColors = options.enhanceColors
        };
        
        var response = await HttpClient.PostAsync($"{API_BASE}/optimize-sprite", request);
        
        if (response.success)
        {
            var texture = await DownloadTexture(response.imageUrl);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            return AIResult<Sprite>.Success(sprite);
        }
        
        return AIResult<Sprite>.Failure(response.error);
    }
    
    // 数值平衡建议
    public static async Task<AIResult<BalanceSuggestion>> GetBalanceSuggestion(GameConfig config)
    {
        var request = new BalanceAnalysisRequest
        {
            playerConfig = config.player_base_config,
            enemyConfigs = config.enemies,
            abilityConfigs = config.abilities,
            stageConfigs = config.stages
        };
        
        var response = await HttpClient.PostAsync($"{API_BASE}/balance-config", request);
        
        if (response.success)
        {
            return AIResult<BalanceSuggestion>.Success(response.suggestion);
        }
        
        return AIResult<BalanceSuggestion>.Failure(response.error);
    }
    
    // 生成敌人描述
    public static async Task<AIResult<string>> GenerateEnemyDescription(EnemyConfigData enemy)
    {
        var prompt = $@"
            Generate a creative description for a roguelike game enemy:
            Name: {enemy.enemy_name}
            Type: {(enemy.is_boss ? "Boss" : "Normal")}
            Health: {enemy.base_health}
            Damage: {enemy.base_damage}
            
            The description should be 2-3 sentences, atmospheric and engaging.
        ";
        
        var response = await HttpClient.PostAsync($"{API_BASE}/generate-text", new { prompt });
        
        if (response.success)
        {
            return AIResult<string>.Success(response.text);
        }
        
        return AIResult<string>.Failure(response.error);
    }
}
```

### 8.3 AI 辅助功能

| 功能 | 输入 | 输出 | 使用场景 |
|------|------|------|----------|
| **图标生成** | 文字描述 + 风格 | PNG图标 | 能力图标、物品图标 |
| **精灵优化** | 原始图片 | 优化后图片 | 提升画质、去背景 |
| **模型生成** | 文字描述 | 3D模型 | 敌人模型、武器模型 |
| **数值平衡** | 完整配置 | 平衡建议 | 游戏平衡调整 |
| **文案生成** | 关键词 | 描述文本 | 能力描述、敌人描述 |
| **风格迁移** | 原始资源 + 目标风格 | 风格化资源 | 统一美术风格 |

---

## 九、资源管理系统

### 9.1 资源类型定义

```csharp
public enum AssetType
{
    Icon,           // 图标 (PNG, 128x128)
    Sprite,         // 精灵图 (PNG, 各种尺寸)
    Model,          // 3D模型 (AssetBundle)
    Audio_BGM,      // 背景音乐 (OGG)
    Audio_SFX,      // 音效 (OGG)
    UI_Image,       // UI图片 (PNG)
    Animation,      // 动画 (AssetBundle)
    Particle,       // 粒子效果 (AssetBundle)
}

public class AssetInfo
{
    public string id;
    public AssetType type;
    public string name;
    public string localPath;
    public string remoteUrl;
    public string hash;
    public long size;
    public DateTime createdAt;
    public Dictionary<string, string> metadata;
}
```

### 9.2 资源缓存策略

```csharp
public class AssetCacheManager
{
    private const long MAX_CACHE_SIZE = 500 * 1024 * 1024; // 500MB
    private const int MAX_CACHE_AGE_DAYS = 30;
    
    private string cacheRoot;
    private Dictionary<string, CacheEntry> cacheIndex;
    
    // 获取资源（优先本地缓存）
    public async Task<byte[]> GetAsset(AssetInfo asset)
    {
        var localPath = GetLocalPath(asset);
        
        // 检查本地缓存
        if (File.Exists(localPath))
        {
            var cachedHash = ComputeHash(localPath);
            if (cachedHash == asset.hash)
            {
                UpdateAccessTime(asset.id);
                return await File.ReadAllBytesAsync(localPath);
            }
        }
        
        // 下载并缓存
        var data = await DownloadAsset(asset.remoteUrl);
        await CacheAsset(asset, data);
        return data;
    }
    
    // 缓存清理
    public async Task CleanupCache()
    {
        var entries = cacheIndex.Values
            .OrderBy(e => e.lastAccessTime)
            .ToList();
        
        long currentSize = entries.Sum(e => e.size);
        
        while (currentSize > MAX_CACHE_SIZE)
        {
            var oldest = entries.First();
            await DeleteCacheEntry(oldest);
            entries.RemoveAt(0);
            currentSize -= oldest.size;
        }
        
        // 删除过期缓存
        var expiredEntries = entries
            .Where(e => (DateTime.Now - e.lastAccessTime).TotalDays > MAX_CACHE_AGE_DAYS)
            .ToList();
            
        foreach (var entry in expiredEntries)
        {
            await DeleteCacheEntry(entry);
        }
    }
}
```

### 9.3 增量更新机制

```csharp
public class IncrementalUpdater
{
    // 检查更新
    public async Task<UpdateInfo> CheckForUpdates(string launcherKey, string currentVersion)
    {
        var response = await API.CheckUpdate(launcherKey, currentVersion);
        
        if (!response.hasUpdate)
            return UpdateInfo.NoUpdate();
        
        var changedFiles = new List<FileChange>();
        
        foreach (var file in response.manifest.files)
        {
            var localHash = GetLocalFileHash(file.path);
            
            if (localHash != file.hash)
            {
                changedFiles.Add(new FileChange
                {
                    path = file.path,
                    action = localHash == null ? ChangeAction.Add : ChangeAction.Update,
                    size = file.size
                });
            }
        }
        
        // 检查删除的文件
        var remoteFiles = response.manifest.files.Select(f => f.path).ToHashSet();
        var localFiles = GetLocalFiles();
        
        foreach (var localFile in localFiles)
        {
            if (!remoteFiles.Contains(localFile))
            {
                changedFiles.Add(new FileChange
                {
                    path = localFile,
                    action = ChangeAction.Delete,
                    size = 0
                });
            }
        }
        
        return new UpdateInfo
        {
            hasUpdate = true,
            newVersion = response.newVersion,
            changes = changedFiles,
            totalDownloadSize = changedFiles.Where(c => c.action != ChangeAction.Delete).Sum(c => c.size)
        };
    }
    
    // 执行更新
    public async Task ApplyUpdate(UpdateInfo updateInfo, IProgress<float> progress)
    {
        int completed = 0;
        int total = updateInfo.changes.Count;
        
        foreach (var change in updateInfo.changes)
        {
            switch (change.action)
            {
                case ChangeAction.Add:
                case ChangeAction.Update:
                    await DownloadAndSaveFile(change.path);
                    break;
                case ChangeAction.Delete:
                    DeleteLocalFile(change.path);
                    break;
            }
            
            completed++;
            progress.Report((float)completed / total);
        }
        
        // 更新本地版本号
        SaveLocalVersion(updateInfo.newVersion);
    }
}
```

---

## 十、发布流程

### 10.1 发布流程图

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   创作完成   │     │   本地验证   │     │   上传打包   │     │   提交审核   │
│             │ ──► │             │ ──► │             │ ──► │             │
└─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
                                                                   │
                                                                   ▼
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   分享Key   │     │   发布上线   │     │   审核通过   │     │   人工审核   │
│             │ ◄── │             │ ◄── │             │ ◄── │             │
└─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
```

### 10.2 发布验证规则

```csharp
public class PublishValidator
{
    public ValidationResult Validate(PackageData package)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        
        // 基础信息验证
        if (string.IsNullOrEmpty(package.name))
            errors.Add("配置包名称不能为空");
        if (package.name.Length > 50)
            errors.Add("配置包名称不能超过50个字符");
        if (string.IsNullOrEmpty(package.description))
            warnings.Add("建议添加配置包描述");
            
        // 配置完整性验证
        if (package.stages.Count == 0)
            errors.Add("至少需要一个关卡");
        if (package.enemies.Count == 0)
            errors.Add("至少需要一种敌人");
        if (package.abilities.Count == 0)
            warnings.Add("建议添加至少一个能力");
            
        // 数值平衡验证
        foreach (var enemy in package.enemies)
        {
            if (enemy.base_health <= 0)
                errors.Add($"敌人 {enemy.enemy_name} 的生命值必须大于0");
            if (enemy.base_damage <= 0)
                errors.Add($"敌人 {enemy.enemy_name} 的伤害必须大于0");
        }
        
        // 资源完整性验证
        var missingAssets = ValidateAssetReferences(package);
        foreach (var missing in missingAssets)
        {
            errors.Add($"缺少资源: {missing}");
        }
        
        // 资源大小验证
        if (package.totalSize > 100 * 1024 * 1024) // 100MB
            warnings.Add("配置包较大，可能影响下载速度");
        if (package.totalSize > 500 * 1024 * 1024) // 500MB
            errors.Add("配置包超过500MB限制");
            
        return new ValidationResult
        {
            isValid = errors.Count == 0,
            errors = errors,
            warnings = warnings
        };
    }
}
```

### 10.3 发布 API

```csharp
public class PublishService
{
    // 发布配置包
    public async Task<PublishResult> PublishPackage(PackageData package)
    {
        // 1. 验证
        var validation = Validator.Validate(package);
        if (!validation.isValid)
            return PublishResult.Failed(validation.errors);
        
        // 2. 打包资源
        var packageBundle = await PackageBuilder.Build(package);
        
        // 3. 上传到 CDN
        var uploadResult = await CDNUploader.Upload(packageBundle);
        
        // 4. 生成 Manifest
        var manifest = ManifestGenerator.Generate(package, uploadResult);
        
        // 5. 创建 Launcher Key
        var launcherKey = KeyGenerator.Generate(package);
        
        // 6. 保存到数据库
        await Database.SavePackage(new PackageRecord
        {
            launcherKey = launcherKey,
            manifest = manifest,
            status = PackageStatus.Pending,
            createdAt = DateTime.UtcNow
        });
        
        // 7. 提交审核
        await ReviewService.SubmitForReview(launcherKey);
        
        return PublishResult.Success(launcherKey);
    }
}
```

---

## 十一、技术实现方案

### 11.1 技术栈选型

本平台采用**三端分离架构**：游戏客户端(Unity)、创作编辑器(独立应用)、服务端(Cloud)。

#### 11.1.1 整体架构图

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              UGC Platform - 三端分离架构                          │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────┐    ┌─────────────────────────────────────┐ │
│  │      游戏客户端 (Unity)          │    │      创作编辑器 (独立应用)           │ │
│  │      Game Client                │    │      Creator Studio                 │ │
│  │  ┌─────────────────────────┐   │    │  ┌─────────────────────────────┐   │ │
│  │  │ • 输入 launcher_key     │   │    │  │ • 可视化配置编辑            │   │ │
│  │  │ • 下载配置和资源        │   │    │  │ • AI 辅助资源生成           │   │ │
│  │  │ • 运行游戏              │   │    │  │ • 实时预览                  │   │ │
│  │  │ • 玩家体验              │   │    │  │ • 发布配置包                │   │ │
│  │  └─────────────────────────┘   │    │  └─────────────────────────────┘   │ │
│  │                                 │    │                                     │ │
│  │  平台: PC / Mobile / Console    │    │  平台: PC (Win/Mac) / Mobile        │ │
│  │  技术: Unity 2022 LTS           │    │  技术: Flutter / Electron / Tauri   │ │
│  └─────────────────────────────────┘    └─────────────────────────────────────┘ │
│                      │                                    │                      │
│                      │            HTTPS/WebSocket         │                      │
│                      └──────────────────┬─────────────────┘                      │
│                                         ▼                                        │
│  ┌───────────────────────────────────────────────────────────────────────────┐  │
│  │                              服务端 (Cloud)                                │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │  │
│  │  │ API Gateway │  │ Config Svc  │  │ AI Service  │  │ Asset CDN   │       │  │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘       │  │
│  └───────────────────────────────────────────────────────────────────────────┘  │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

#### 11.1.2 技术栈详细选型

| 层级 | 技术选型 | 说明 |
|------|----------|------|
| **游戏客户端** | | |
| 游戏引擎 | Unity 2022 LTS | 跨平台游戏运行时 |
| 网络通信 | UnityWebRequest + Newtonsoft.Json | HTTP通信 |
| 本地存储 | SQLite + PlayerPrefs | 配置缓存和玩家数据 |
| 资源加载 | Addressables + AssetBundle | 动态资源加载 |
| **创作编辑器** | | |
| 跨平台框架 | **Flutter 3.x** (推荐) / Electron / Tauri | 独立桌面+移动端应用 |
| UI框架 | Flutter Material Design 3 | 现代化UI组件 |
| 状态管理 | Riverpod / Bloc | 响应式状态管理 |
| 本地存储 | Hive / SQLite | 本地项目存储 |
| 网络通信 | Dio + WebSocket | HTTP + 实时通信 |
| 3D预览 | Flutter GL / WebGL | 模型和场景预览 |
| 图片编辑 | image_editor / custom canvas | 图标和精灵编辑 |
| **服务端** | | |
| API框架 | Node.js + NestJS / Go + Gin | RESTful API服务 |
| 实时通信 | Socket.io / WebSocket | 实时协作和预览 |
| 数据库 | MongoDB + Redis | 数据存储 + 缓存 |
| 对象存储 | 阿里云OSS / AWS S3 | 资源文件存储 |
| CDN | 阿里云CDN / CloudFlare | 全球资源分发 |
| **AI服务** | | |
| 图像生成 | OpenAI DALL-E / Stability AI | 图标和精灵生成 |
| 图像编辑 | Stability AI Inpainting | 图像修改和优化 |
| 文本生成 | OpenAI GPT-4 / Claude | 描述和文案生成 |
| 数值平衡 | Custom ML Model | 游戏平衡分析 |

#### 11.1.3 创作编辑器 (Creator Studio) 详细设计

##### 技术选型对比

| 方案 | 优点 | 缺点 | 推荐场景 |
|------|------|------|----------|
| **Flutter** | 跨平台(PC+Mobile)、性能好、UI美观、热重载 | 桌面端生态较新 | ✅ 推荐：需要同时支持PC和移动端 |
| **Electron** | 生态成熟、Web技术栈、插件丰富 | 内存占用大、包体积大 | 仅PC端，团队熟悉Web技术 |
| **Tauri** | 轻量、性能好、安全 | 生态较新、移动端支持有限 | 仅PC端，追求轻量 |
| **React Native** | 移动端成熟 | 桌面端支持弱 | 仅移动端 |

##### Flutter 创作编辑器架构

```
creator_studio/
├── lib/
│   ├── main.dart                      # 应用入口
│   ├── app/
│   │   ├── app.dart                   # 应用配置
│   │   ├── routes.dart                # 路由配置
│   │   └── theme.dart                 # 主题配置
│   ├── core/
│   │   ├── api/                       # API 客户端
│   │   │   ├── api_client.dart
│   │   │   ├── auth_api.dart
│   │   │   ├── package_api.dart
│   │   │   └── ai_api.dart
│   │   ├── models/                    # 数据模型
│   │   │   ├── game_config.dart
│   │   │   ├── stage_config.dart
│   │   │   ├── enemy_config.dart
│   │   │   ├── ability_config.dart
│   │   │   └── item_config.dart
│   │   ├── services/                  # 业务服务
│   │   │   ├── auth_service.dart
│   │   │   ├── project_service.dart
│   │   │   ├── asset_service.dart
│   │   │   └── ai_service.dart
│   │   └── utils/                     # 工具类
│   │       ├── validators.dart
│   │       ├── file_utils.dart
│   │       └── export_utils.dart
│   ├── features/
│   │   ├── auth/                      # 登录注册
│   │   ├── home/                      # 首页/项目列表
│   │   ├── editor/                    # 核心编辑器
│   │   │   ├── stage_editor/          # 关卡编辑器
│   │   │   ├── player_editor/         # 玩家编辑器
│   │   │   ├── enemy_editor/          # 敌人编辑器
│   │   │   ├── ability_editor/        # 能力编辑器
│   │   │   ├── item_editor/           # 物品编辑器
│   │   │   └── asset_manager/         # 资源管理器
│   │   ├── preview/                   # 预览功能
│   │   ├── ai_assistant/              # AI 助手
│   │   ├── publish/                   # 发布功能
│   │   └── settings/                  # 设置
│   └── widgets/                       # 通用组件
│       ├── editors/                   # 编辑器组件
│       ├── previews/                  # 预览组件
│       └── common/                    # 通用组件
├── assets/                            # 静态资源
├── test/                              # 测试
└── pubspec.yaml                       # 依赖配置
```

##### 核心功能模块

```dart
// 项目数据模型
class GameProject {
  final String id;
  final String name;
  final String description;
  final DateTime createdAt;
  final DateTime updatedAt;
  final GameConfig config;
  final List<AssetFile> assets;
  final ProjectStatus status;
  
  // 导出为配置包
  Future<PackageBundle> export() async {
    return PackageBuilder.build(this);
  }
  
  // 发布到服务器
  Future<String> publish() async {
    final bundle = await export();
    return await PackageApi.publish(bundle);
  }
}

// 关卡编辑器状态管理
@riverpod
class StageEditorNotifier extends _$StageEditorNotifier {
  @override
  StageEditorState build() => StageEditorState.initial();
  
  void addStage(StageConfig stage) {
    state = state.copyWith(
      stages: [...state.stages, stage],
    );
  }
  
  void updateStage(int index, StageConfig stage) {
    final stages = [...state.stages];
    stages[index] = stage;
    state = state.copyWith(stages: stages);
  }
  
  void addRoom(int stageIndex, RoomConfig room) {
    final stages = [...state.stages];
    stages[stageIndex] = stages[stageIndex].copyWith(
      rooms: [...stages[stageIndex].rooms, room],
    );
    state = state.copyWith(stages: stages);
  }
  
  void addWave(int stageIndex, int roomIndex, WaveConfig wave) {
    // ... 添加波次逻辑
  }
  
  void addEnemySpawn(int stageIndex, int roomIndex, int waveIndex, EnemySpawn spawn) {
    // ... 添加敌人生成点逻辑
  }
}

// AI 服务集成
class AIService {
  final Dio _dio;
  
  AIService(this._dio);
  
  // 生成图标
  Future<Uint8List> generateIcon({
    required String prompt,
    required IconStyle style,
    int size = 128,
  }) async {
    final response = await _dio.post('/ai/generate-icon', data: {
      'prompt': prompt,
      'style': style.name,
      'size': size,
    });
    return base64Decode(response.data['image']);
  }
  
  // 优化图片
  Future<Uint8List> optimizeImage({
    required Uint8List image,
    required OptimizeOptions options,
  }) async {
    final response = await _dio.post('/ai/optimize-image', data: {
      'image': base64Encode(image),
      'upscale': options.upscale,
      'removeBackground': options.removeBackground,
      'enhanceColors': options.enhanceColors,
    });
    return base64Decode(response.data['image']);
  }
  
  // 获取数值平衡建议
  Future<BalanceSuggestion> getBalanceSuggestion(GameConfig config) async {
    final response = await _dio.post('/ai/balance-analysis', data: config.toJson());
    return BalanceSuggestion.fromJson(response.data);
  }
  
  // 生成敌人描述
  Future<String> generateDescription({
    required String entityType,
    required Map<String, dynamic> attributes,
  }) async {
    final response = await _dio.post('/ai/generate-text', data: {
      'type': entityType,
      'attributes': attributes,
    });
    return response.data['text'];
  }
}
```

##### 编辑器界面设计

```dart
// 主编辑器界面
class EditorScreen extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Scaffold(
      body: Row(
        children: [
          // 左侧导航面板
          NavigationPanel(
            items: [
              NavItem(icon: Icons.layers, label: '关卡', route: '/editor/stages'),
              NavItem(icon: Icons.person, label: '玩家', route: '/editor/player'),
              NavItem(icon: Icons.bug_report, label: '敌人', route: '/editor/enemies'),
              NavItem(icon: Icons.auto_awesome, label: '能力', route: '/editor/abilities'),
              NavItem(icon: Icons.inventory, label: '装备', route: '/editor/items'),
              NavItem(icon: Icons.folder, label: '资源', route: '/editor/assets'),
            ],
          ),
          
          // 中间编辑区域
          Expanded(
            flex: 3,
            child: EditorContent(),
          ),
          
          // 右侧属性面板
          PropertyPanel(
            width: 300,
          ),
        ],
      ),
      
      // 底部工具栏
      bottomNavigationBar: EditorToolbar(
        actions: [
          ToolbarAction(icon: Icons.save, label: '保存', onTap: _save),
          ToolbarAction(icon: Icons.preview, label: '预览', onTap: _preview),
          ToolbarAction(icon: Icons.smart_toy, label: 'AI助手', onTap: _openAI),
          ToolbarAction(icon: Icons.publish, label: '发布', onTap: _publish),
        ],
      ),
    );
  }
}

// 敌人编辑器
class EnemyEditorPanel extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final enemies = ref.watch(enemyListProvider);
    final selectedEnemy = ref.watch(selectedEnemyProvider);
    
    return Row(
      children: [
        // 敌人列表
        SizedBox(
          width: 250,
          child: EnemyListView(
            enemies: enemies,
            selectedId: selectedEnemy?.id,
            onSelect: (enemy) => ref.read(selectedEnemyProvider.notifier).select(enemy),
            onAdd: () => _showAddEnemyDialog(context, ref),
            onDelete: (enemy) => ref.read(enemyListProvider.notifier).remove(enemy),
          ),
        ),
        
        // 敌人详情编辑
        if (selectedEnemy != null)
          Expanded(
            child: EnemyDetailEditor(
              enemy: selectedEnemy,
              onChanged: (updated) => ref.read(enemyListProvider.notifier).update(updated),
            ),
          ),
      ],
    );
  }
}

// 敌人详情编辑器
class EnemyDetailEditor extends StatelessWidget {
  final EnemyConfig enemy;
  final ValueChanged<EnemyConfig> onChanged;
  
  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // 基础信息
          SectionCard(
            title: '基础信息',
            children: [
              TextFormField(
                initialValue: enemy.name,
                decoration: InputDecoration(labelText: '敌人名称'),
                onChanged: (v) => onChanged(enemy.copyWith(name: v)),
              ),
              SwitchListTile(
                title: Text('是否为Boss'),
                value: enemy.isBoss,
                onChanged: (v) => onChanged(enemy.copyWith(isBoss: v)),
              ),
            ],
          ),
          
          // 属性配置
          SectionCard(
            title: '属性配置',
            children: [
              SliderField(
                label: '基础生命值',
                value: enemy.baseHealth,
                min: 1,
                max: 1000,
                onChanged: (v) => onChanged(enemy.copyWith(baseHealth: v)),
              ),
              SliderField(
                label: '基础伤害',
                value: enemy.baseDamage,
                min: 1,
                max: 100,
                onChanged: (v) => onChanged(enemy.copyWith(baseDamage: v)),
              ),
              SliderField(
                label: '碰撞伤害倍率',
                value: enemy.collisionDamageMultiplier,
                min: 0,
                max: 2,
                divisions: 20,
                onChanged: (v) => onChanged(enemy.copyWith(collisionDamageMultiplier: v)),
              ),
            ],
          ),
          
          // 行为配置
          SectionCard(
            title: '行为配置',
            child: EnemyBehaviorEditor(
              behavior: enemy.behavior,
              onChanged: (b) => onChanged(enemy.copyWith(behavior: b)),
            ),
          ),
          
          // 模型和图标
          SectionCard(
            title: '资源配置',
            children: [
              AssetPicker(
                label: '敌人模型',
                assetType: AssetType.model,
                currentPath: enemy.prefabPath,
                onChanged: (p) => onChanged(enemy.copyWith(prefabPath: p)),
              ),
              AssetPicker(
                label: '敌人图标',
                assetType: AssetType.icon,
                currentPath: enemy.iconPath,
                onChanged: (p) => onChanged(enemy.copyWith(iconPath: p)),
              ),
              // AI 生成按钮
              ElevatedButton.icon(
                icon: Icon(Icons.auto_awesome),
                label: Text('AI 生成图标'),
                onPressed: () => _generateIconWithAI(context),
              ),
            ],
          ),
          
          // 掉落配置
          SectionCard(
            title: '掉落配置',
            child: DropConfigEditor(
              drops: enemy.drops,
              onChanged: (d) => onChanged(enemy.copyWith(drops: d)),
            ),
          ),
        ],
      ),
    );
  }
}
```

##### 与游戏客户端的数据交互

```dart
// 配置包导出格式 (与Unity客户端兼容)
class PackageExporter {
  static Future<PackageBundle> export(GameProject project) async {
    // 1. 生成配置JSON
    final gameConfig = project.config.toJson();
    
    // 2. 收集资源文件
    final assets = await _collectAssets(project.assets);
    
    // 3. 生成Manifest
    final manifest = _generateManifest(project, assets);
    
    // 4. 打包
    return PackageBundle(
      manifest: manifest,
      configJson: jsonEncode(gameConfig),
      assets: assets,
    );
  }
  
  static Map<String, dynamic> _generateManifest(GameProject project, List<AssetFile> assets) {
    return {
      'manifest_version': '1.0',
      'package_info': {
        'id': project.id,
        'name': project.name,
        'version': project.version,
        'min_client_version': '2.0.0',
        'created_at': project.createdAt.toIso8601String(),
        'updated_at': project.updatedAt.toIso8601String(),
      },
      'config_files': [
        {'name': 'game_config', 'path': 'config/game_config.json'},
      ],
      'asset_bundles': assets
          .where((a) => a.type == AssetType.model)
          .map((a) => {
            'name': a.name,
            'path': a.path,
            'hash': a.hash,
            'size': a.size,
          })
          .toList(),
      'loose_assets': _groupAssetsByType(assets),
      'total_size': assets.fold<int>(0, (sum, a) => sum + a.size),
    };
  }
}
```

#### 11.1.4 Unity Cloud Build 资源打包服务

使用 **Unity Cloud Build** 作为资源打包服务，无需自建Unity服务器。

##### Unity Cloud Build 集成架构

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                     Unity Cloud Build 集成架构                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    创作编辑器 (Flutter)                                  │   │
│  │                                                                          │   │
│  │  用户上传原始资源:                                                        │   │
│  │  • 3D模型 (FBX, OBJ, GLTF)                                               │   │
│  │  • 贴图 (PNG, JPG, TGA)                                                  │   │
│  │  • 音频 (MP3, WAV, OGG)                                                  │   │
│  │  • 配置JSON                                                              │   │
│  │                                                                          │   │
│  └──────────────────────────────┬──────────────────────────────────────────┘   │
│                                 │ HTTP Upload                                   │
│                                 ▼                                               │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    后端API服务器                                         │   │
│  │                                                                          │   │
│  │  1. 接收上传的原始资源                                                    │   │
│  │  2. 验证资源格式和大小                                                    │   │
│  │  3. 提交到Git仓库 (触发Cloud Build)                                      │   │
│  │  4. 监听构建状态                                                         │   │
│  │                                                                          │   │
│  └──────────────────────────────┬──────────────────────────────────────────┘   │
│                                 │ Git Push / Webhook                            │
│                                 ▼                                               │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    Unity Cloud Build                                     │   │
│  │                                                                          │   │
│  │  ┌─────────────────────────────────────────────────────────────────┐    │   │
│  │  │                    自动构建流程                                  │    │   │
│  │  │                                                                  │    │   │
│  │  │  1. 检测Git仓库变更                                              │    │   │
│  │  │  2. 拉取最新代码和资源                                           │    │   │
│  │  │  3. 导入资源到Unity项目                                          │    │   │
│  │  │  4. 执行自定义打包脚本                                           │    │   │
│  │  │  5. 生成多平台AssetBundle                                        │    │   │
│  │  │  6. 上传构建产物                                                 │    │   │
│  │  │  7. 发送Webhook通知                                              │    │   │
│  │  │                                                                  │    │   │
│  │  └─────────────────────────────────────────────────────────────────┘    │   │
│  │                                                                          │   │
│  │  支持平台:                                                                │   │
│  │  • Windows (x64)                                                         │   │
│  │  • macOS (Intel/Apple Silicon)                                           │   │
│  │  • Linux                                                                 │   │
│  │  • Android                                                               │   │
│  │  • iOS                                                                   │   │
│  │  • WebGL                                                                 │   │
│  │                                                                          │   │
│  └──────────────────────────────┬──────────────────────────────────────────┘   │
│                                 │ Webhook / Download                            │
│                                 ▼                                               │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    CDN分发 (阿里云OSS / AWS S3)                          │   │
│  │                                                                          │   │
│  │  存储构建产物:                                                            │   │
│  │  • /packages/{package_id}/windows/assets.bundle                          │   │
│  │  • /packages/{package_id}/android/assets.bundle                          │   │
│  │  • /packages/{package_id}/ios/assets.bundle                              │   │
│  │  • /packages/{package_id}/manifest.json                                  │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

##### Unity Cloud Build 配置

```yaml
# unity-cloud-build-config.yaml
# Unity Cloud Build 项目配置

project:
  name: "UGC-AssetBundle-Builder"
  unity_version: "2022.3.20f1"
  
build_targets:
  - name: "Windows-AssetBundles"
    platform: standalonewindows64
    build_method: "CloudBuildHelper.BuildAssetBundles"
    advanced:
      pre_build_script: "Editor/CloudBuild/PreBuild.cs"
      post_build_script: "Editor/CloudBuild/PostBuild.cs"
      
  - name: "Android-AssetBundles"
    platform: android
    build_method: "CloudBuildHelper.BuildAssetBundles"
    
  - name: "iOS-AssetBundles"
    platform: ios
    build_method: "CloudBuildHelper.BuildAssetBundles"
    
  - name: "WebGL-AssetBundles"
    platform: webgl
    build_method: "CloudBuildHelper.BuildAssetBundles"

webhooks:
  - url: "https://api.your-platform.com/webhooks/unity-cloud-build"
    events:
      - "build_started"
      - "build_success"
      - "build_failed"
```

##### Cloud Build 打包脚本

```csharp
// Assets/Editor/CloudBuild/CloudBuildHelper.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;
using System.Collections.Generic;

public class CloudBuildHelper
{
    // Unity Cloud Build 调用的入口方法
    public static void BuildAssetBundles()
    {
        Debug.Log("=== Starting Cloud Build AssetBundle Process ===");
        
        try
        {
            // 1. 获取构建参数
            var buildManifest = GetCloudBuildManifest();
            var targetPlatform = GetBuildTarget();
            
            Debug.Log($"Building for platform: {targetPlatform}");
            Debug.Log($"Build Number: {buildManifest.buildNumber}");
            
            // 2. 设置AssetBundle名称
            SetupAssetBundleNames();
            
            // 3. 构建AssetBundle
            var outputPath = "Assets/StreamingAssets/AssetBundles";
            Directory.CreateDirectory(outputPath);
            
            var manifest = BuildPipeline.BuildAssetBundles(
                outputPath,
                BuildAssetBundleOptions.ChunkBasedCompression |
                BuildAssetBundleOptions.ForceRebuildAssetBundle |
                BuildAssetBundleOptions.StrictMode,
                targetPlatform
            );
            
            if (manifest == null)
            {
                throw new Exception("AssetBundle build failed!");
            }
            
            // 4. 生成清单文件
            GenerateManifestJson(outputPath, manifest);
            
            Debug.Log("=== Cloud Build AssetBundle Process Completed ===");
        }
        catch (Exception e)
        {
            Debug.LogError($"Cloud Build failed: {e.Message}\n{e.StackTrace}");
            throw;
        }
    }
    
    private static void SetupAssetBundleNames()
    {
        // 自动为DynamicAssets目录下的资源设置AssetBundle名称
        var dynamicAssetsPath = "Assets/DynamicAssets";
        
        if (!Directory.Exists(dynamicAssetsPath))
        {
            Debug.LogWarning("DynamicAssets directory not found");
            return;
        }
        
        // 按文件夹分组设置AssetBundle
        var directories = Directory.GetDirectories(dynamicAssetsPath);
        foreach (var dir in directories)
        {
            var bundleName = Path.GetFileName(dir).ToLower();
            var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                if (file.EndsWith(".meta")) continue;
                
                var importer = AssetImporter.GetAtPath(file);
                if (importer != null)
                {
                    importer.assetBundleName = bundleName;
                    importer.SaveAndReimport();
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private static void GenerateManifestJson(string outputPath, AssetBundleManifest manifest)
    {
        var bundleNames = manifest.GetAllAssetBundles();
        var manifestData = new ManifestData
        {
            version = "1.0",
            buildTime = DateTime.UtcNow.ToString("o"),
            platform = EditorUserBuildSettings.activeBuildTarget.ToString(),
            bundles = new List<BundleInfo>()
        };
        
        foreach (var bundleName in bundleNames)
        {
            var bundlePath = Path.Combine(outputPath, bundleName);
            var fileInfo = new FileInfo(bundlePath);
            
            manifestData.bundles.Add(new BundleInfo
            {
                name = bundleName,
                hash = manifest.GetAssetBundleHash(bundleName).ToString(),
                size = fileInfo.Length,
                dependencies = manifest.GetAllDependencies(bundleName)
            });
        }
        
        var json = JsonUtility.ToJson(manifestData, true);
        File.WriteAllText(Path.Combine(outputPath, "manifest.json"), json);
    }
    
    private static BuildTarget GetBuildTarget()
    {
        // 从环境变量或Cloud Build参数获取目标平台
        var platformEnv = Environment.GetEnvironmentVariable("BUILD_TARGET");
        
        if (!string.IsNullOrEmpty(platformEnv))
        {
            if (Enum.TryParse<BuildTarget>(platformEnv, out var target))
                return target;
        }
        
        return EditorUserBuildSettings.activeBuildTarget;
    }
    
    private static CloudBuildManifest GetCloudBuildManifest()
    {
        // Unity Cloud Build 会注入这些信息
        return new CloudBuildManifest
        {
            buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "local",
            scmCommitId = Environment.GetEnvironmentVariable("SCM_COMMIT_ID") ?? "unknown",
            scmBranch = Environment.GetEnvironmentVariable("SCM_BRANCH") ?? "main"
        };
    }
    
    [Serializable]
    private class CloudBuildManifest
    {
        public string buildNumber;
        public string scmCommitId;
        public string scmBranch;
    }
    
    [Serializable]
    private class ManifestData
    {
        public string version;
        public string buildTime;
        public string platform;
        public List<BundleInfo> bundles;
    }
    
    [Serializable]
    private class BundleInfo
    {
        public string name;
        public string hash;
        public long size;
        public string[] dependencies;
    }
}
```

##### 后端集成 Unity Cloud Build API

```typescript
// unity-cloud-build-service.ts
import axios from 'axios';

interface CloudBuildConfig {
  orgId: string;
  projectId: string;
  apiKey: string;
}

interface BuildTarget {
  buildtargetid: string;
  name: string;
  platform: string;
}

interface Build {
  build: number;
  buildtargetid: string;
  buildStatus: string;
  links: {
    download_primary: { href: string };
  };
}

export class UnityCloudBuildService {
  private baseUrl = 'https://build-api.cloud.unity3d.com/api/v1';
  private config: CloudBuildConfig;
  
  constructor(config: CloudBuildConfig) {
    this.config = config;
  }
  
  private get headers() {
    return {
      'Authorization': `Basic ${this.config.apiKey}`,
      'Content-Type': 'application/json'
    };
  }
  
  // 获取所有构建目标
  async getBuildTargets(): Promise<BuildTarget[]> {
    const url = `${this.baseUrl}/orgs/${this.config.orgId}/projects/${this.config.projectId}/buildtargets`;
    const response = await axios.get(url, { headers: this.headers });
    return response.data;
  }
  
  // 触发构建
  async triggerBuild(buildTargetId: string, options?: {
    commit?: string;
    clean?: boolean;
  }): Promise<Build> {
    const url = `${this.baseUrl}/orgs/${this.config.orgId}/projects/${this.config.projectId}/buildtargets/${buildTargetId}/builds`;
    
    const response = await axios.post(url, {
      clean: options?.clean ?? false,
      delay: 0,
      commit: options?.commit
    }, { headers: this.headers });
    
    return response.data[0];
  }
  
  // 触发所有平台构建
  async triggerAllPlatformBuilds(packageId: string): Promise<Build[]> {
    const targets = await this.getBuildTargets();
    const builds: Build[] = [];
    
    for (const target of targets) {
      const build = await this.triggerBuild(target.buildtargetid);
      builds.push(build);
      
      // 记录构建信息
      await this.saveBuildRecord(packageId, target.platform, build.build);
    }
    
    return builds;
  }
  
  // 获取构建状态
  async getBuildStatus(buildTargetId: string, buildNumber: number): Promise<Build> {
    const url = `${this.baseUrl}/orgs/${this.config.orgId}/projects/${this.config.projectId}/buildtargets/${buildTargetId}/builds/${buildNumber}`;
    const response = await axios.get(url, { headers: this.headers });
    return response.data;
  }
  
  // 获取构建产物下载链接
  async getArtifactUrl(buildTargetId: string, buildNumber: number): Promise<string> {
    const build = await this.getBuildStatus(buildTargetId, buildNumber);
    return build.links.download_primary.href;
  }
  
  // 下载构建产物并上传到CDN
  async downloadAndUploadToCDN(
    buildTargetId: string, 
    buildNumber: number, 
    packageId: string,
    platform: string
  ): Promise<string> {
    // 1. 获取下载链接
    const artifactUrl = await this.getArtifactUrl(buildTargetId, buildNumber);
    
    // 2. 下载构建产物
    const response = await axios.get(artifactUrl, { responseType: 'arraybuffer' });
    
    // 3. 解压并上传到CDN
    const cdnPath = `packages/${packageId}/${platform}/`;
    await this.uploadToCDN(response.data, cdnPath);
    
    return cdnPath;
  }
  
  private async saveBuildRecord(packageId: string, platform: string, buildNumber: number) {
    // 保存构建记录到数据库
  }
  
  private async uploadToCDN(data: Buffer, path: string) {
    // 上传到阿里云OSS或AWS S3
  }
}

// Webhook处理器
export async function handleCloudBuildWebhook(payload: any) {
  const { buildStatus, buildTargetId, build, projectId } = payload;
  
  console.log(`Build ${build} for ${buildTargetId}: ${buildStatus}`);
  
  if (buildStatus === 'success') {
    // 构建成功，下载产物并上传到CDN
    const service = new UnityCloudBuildService({
      orgId: process.env.UNITY_ORG_ID!,
      projectId: projectId,
      apiKey: process.env.UNITY_API_KEY!
    });
    
    // 从数据库获取packageId
    const packageId = await getPackageIdByBuild(build);
    const platform = await getPlatformByBuildTarget(buildTargetId);
    
    await service.downloadAndUploadToCDN(buildTargetId, build, packageId, platform);
    
    // 检查是否所有平台都构建完成
    await checkAndFinalizePackage(packageId);
  } else if (buildStatus === 'failed') {
    // 构建失败，通知用户
    await notifyBuildFailure(build, buildTargetId);
  }
}
```

##### 完整的资源发布流程

```typescript
// publish-service.ts
export class PublishService {
  private cloudBuild: UnityCloudBuildService;
  private gitService: GitService;
  
  // 发布配置包
  async publishPackage(packageId: string, assets: UploadedAsset[], config: GameConfig) {
    try {
      // 1. 创建发布记录
      await this.createPublishRecord(packageId, 'processing');
      
      // 2. 将资源提交到Git仓库
      const commitId = await this.commitAssetsToGit(packageId, assets, config);
      
      // 3. 触发Unity Cloud Build
      const builds = await this.cloudBuild.triggerAllPlatformBuilds(packageId);
      
      // 4. 保存构建信息
      await this.saveBuildInfo(packageId, builds);
      
      // 5. 返回状态（实际完成通过Webhook通知）
      return {
        status: 'building',
        packageId,
        builds: builds.map(b => ({
          platform: b.buildtargetid,
          buildNumber: b.build
        }))
      };
    } catch (error) {
      await this.updatePublishRecord(packageId, 'failed', error.message);
      throw error;
    }
  }
  
  private async commitAssetsToGit(
    packageId: string, 
    assets: UploadedAsset[], 
    config: GameConfig
  ): Promise<string> {
    // 1. 克隆/更新构建仓库
    await this.gitService.cloneOrPull();
    
    // 2. 清理DynamicAssets目录
    await this.gitService.cleanDirectory('Assets/DynamicAssets');
    
    // 3. 复制资源到对应目录
    for (const asset of assets) {
      const targetPath = this.getAssetTargetPath(asset);
      await this.gitService.copyFile(asset.localPath, targetPath);
    }
    
    // 4. 保存配置文件
    await this.gitService.writeFile(
      'Assets/DynamicAssets/config/game_config.json',
      JSON.stringify(config, null, 2)
    );
    
    // 5. 提交并推送
    const commitId = await this.gitService.commitAndPush(
      `Package ${packageId}: Update assets and config`
    );
    
    return commitId;
  }
  
  private getAssetTargetPath(asset: UploadedAsset): string {
    const typeMap: Record<string, string> = {
      'model': 'Assets/DynamicAssets/models',
      'texture': 'Assets/DynamicAssets/textures',
      'audio': 'Assets/DynamicAssets/audio',
      'icon': 'Assets/DynamicAssets/icons'
    };
    
    const dir = typeMap[asset.type] || 'Assets/DynamicAssets/misc';
    return `${dir}/${asset.filename}`;
  }
}
```

##### Unity Cloud Build 优势

| 优势 | 说明 |
|------|------|
| **无需自建服务器** | Unity官方托管，无需维护构建服务器 |
| **自动多平台构建** | 一次提交，自动为所有平台构建 |
| **许可证包含** | 使用Unity订阅即可，无需额外购买服务器许可证 |
| **自动缓存** | 增量构建，加快构建速度 |
| **Webhook集成** | 构建完成自动通知 |
| **构建历史** | 保留构建历史，支持回滚 |

##### 费用说明

| 计划 | 并发构建数 | 构建时间/月 | 费用 |
|------|-----------|------------|------|
| Free | 1 | 200分钟 | 免费 |
| Plus | 3 | 3000分钟 | $9/月 |
| Pro | 5 | 无限 | 包含在Pro订阅中 |

#### 11.1.5 游戏客户端 (Unity) 配置加载

游戏客户端只负责**加载配置和运行游戏**，不包含编辑功能：

```csharp
// Unity 游戏客户端 - 配置加载器
public class ConfigLoader : MonoBehaviour
{
    private const string CONFIG_CACHE_PATH = "ConfigCache";
    
    public static GameConfig GameConfig { get; private set; }
    
    // 从服务器加载配置
    public static async Task<bool> LoadFromServer(string launcherKey)
    {
        try
        {
            // 1. 验证 Key 并获取 Manifest URL
            var validation = await API.ValidateLauncherKey(launcherKey);
            if (!validation.isValid) return false;
            
            // 2. 下载 Manifest
            var manifest = await DownloadManifest(validation.manifestUrl);
            
            // 3. 检查本地缓存
            var needsDownload = await CheckCacheValidity(manifest);
            
            // 4. 下载缺失的配置和资源
            if (needsDownload.Count > 0)
            {
                await DownloadFiles(needsDownload);
            }
            
            // 5. 加载配置
            var configJson = await LoadLocalConfig(manifest);
            GameConfig = JsonConvert.DeserializeObject<GameConfig>(configJson);
            
            // 6. 预加载资源
            await ResourceManager.PreloadAssets(manifest);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load config: {e.Message}");
            return false;
        }
    }
}
```

### 11.2 关键代码实现

#### 启动流程

```csharp
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private LauncherUI launcherUI;
    [SerializeField] private LoadingUI loadingUI;
    
    private async void Start()
    {
        // 显示启动界面
        launcherUI.Show();
        
        // 等待用户输入 Key
        var launcherKey = await launcherUI.WaitForKeyInput();
        
        // 隐藏启动界面，显示加载界面
        launcherUI.Hide();
        loadingUI.Show();
        
        try
        {
            // 启动游戏
            var success = await LauncherManager.Instance.LaunchWithKey(
                launcherKey,
                new Progress<LoadingProgress>(p => loadingUI.UpdateProgress(p))
            );
            
            if (success)
            {
                loadingUI.Hide();
                // 进入游戏主菜单
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                loadingUI.ShowError("启动失败，请检查 Key 是否正确");
            }
        }
        catch (Exception e)
        {
            loadingUI.ShowError($"启动出错: {e.Message}");
        }
    }
}
```

#### 配置驱动的游戏系统

```csharp
// 配置驱动的关卡生成
public class StageGenerator
{
    public static Stage GenerateStage(int stageId)
    {
        var stageConfig = ConfigLoader.StageConfig.GetStage(stageId);
        
        var stage = new Stage
        {
            id = stageConfig.stage_id,
            name = stageConfig.stage_name,
            difficultyMultiplier = new DifficultyMultiplier
            {
                baseDamage = stageConfig.base_enemy_damage_multiplier,
                damageRoomStep = stageConfig.enemy_damage_room_step,
                damageWaveStep = stageConfig.enemy_damage_wave_step,
                baseHP = stageConfig.base_enemy_hp_multiplier,
                hpRoomStep = stageConfig.enemy_hp_room_step,
                hpWaveStep = stageConfig.enemy_hp_wave_step
            }
        };
        
        // 生成房间
        foreach (var roomConfig in stageConfig.rooms)
        {
            var room = GenerateRoom(roomConfig);
            stage.rooms.Add(room);
        }
        
        return stage;
    }
    
    private static Room GenerateRoom(RoomConfigData roomConfig)
    {
        var room = new Room
        {
            id = roomConfig.room_id,
            playerSpawnPoint = roomConfig.player_spawn.ToVector3(),
            firstWaveDelay = roomConfig.first_wave_delay,
            waveDelay = roomConfig.wave_delay
        };
        
        // 生成波次
        foreach (var waveConfig in roomConfig.waves)
        {
            var wave = GenerateWave(waveConfig);
            room.waves.Add(wave);
        }
        
        return room;
    }
    
    private static Wave GenerateWave(WaveConfigData waveConfig)
    {
        var wave = new Wave
        {
            id = waveConfig.wave_id,
            expReward = waveConfig.exp_reward,
            goldReward = waveConfig.gold_reward
        };
        
        // 生成敌人生成点
        foreach (var spawnConfig in waveConfig.enemy_spawns)
        {
            wave.enemySpawns.Add(new EnemySpawn
            {
                enemyType = spawnConfig.enemy_type,
                position = spawnConfig.position.ToVector3(),
                overrideHP = spawnConfig.override_hp ? spawnConfig.hp_value : null,
                overrideDamage = spawnConfig.override_damage ? spawnConfig.damage_value : null
            });
        }
        
        return wave;
    }
}
```

---

## 十二、安全与审核

### 12.1 安全措施

| 安全层面 | 措施 | 说明 |
|----------|------|------|
| **传输安全** | HTTPS + 证书固定 | 防止中间人攻击 |
| **数据完整性** | SHA256 校验 | 防止资源篡改 |
| **访问控制** | JWT Token + 权限验证 | 用户身份认证 |
| **内容安全** | AI审核 + 人工审核 | 防止违规内容 |
| **防作弊** | 服务端验证 + 数据加密 | 防止数据篡改 |

### 12.2 内容审核流程

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   提交审核   │     │   AI 初审   │     │   人工复审   │
│             │ ──► │             │ ──► │  (可疑内容)  │
└─────────────┘     └─────────────┘     └─────────────┘
                           │                    │
                           ▼                    ▼
                    ┌─────────────┐     ┌─────────────┐
                    │   自动通过   │     │   审核结果   │
                    │  (安全内容)  │     │ (通过/拒绝)  │
                    └─────────────┘     └─────────────┘
```

### 12.3 审核规则

```csharp
public class ContentReviewer
{
    // AI 内容审核
    public async Task<ReviewResult> AIReview(PackageData package)
    {
        var issues = new List<ReviewIssue>();
        
        // 文本内容审核
        var textContents = ExtractTextContents(package);
        foreach (var text in textContents)
        {
            var result = await AIContentModerator.CheckText(text);
            if (result.hasIssue)
            {
                issues.Add(new ReviewIssue
                {
                    type = IssueType.Text,
                    content = text,
                    reason = result.reason,
                    severity = result.severity
                });
            }
        }
        
        // 图片内容审核
        var images = ExtractImages(package);
        foreach (var image in images)
        {
            var result = await AIContentModerator.CheckImage(image);
            if (result.hasIssue)
            {
                issues.Add(new ReviewIssue
                {
                    type = IssueType.Image,
                    content = image.path,
                    reason = result.reason,
                    severity = result.severity
                });
            }
        }
        
        // 判断是否需要人工审核
        var needsManualReview = issues.Any(i => i.severity >= Severity.Medium);
        
        return new ReviewResult
        {
            passed = issues.Count == 0,
            needsManualReview = needsManualReview,
            issues = issues
        };
    }
}
```

---

## 附录

### A. 配置文件示例

完整的配置文件示例请参考: `Assets/The Archer/Config/game_config.json`

### B. API 文档

详细的 API 文档请参考: `docs/api/README.md`

### C. 开发指南

创作者开发指南请参考: `docs/creator-guide/README.md`

---

*文档版本: 1.0.0*  
*最后更新: 2025-01-29*

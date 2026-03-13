# UGC平台上传指南

## 文件准备清单

### 已准备的CSV文件
1. `The_Archer_Roguelike_项目分析表格.csv` - 主项目信息表
2. `The_Archer_Roguelike_敌人数据表.csv` - 敌人系统数据
3. `The_Archer_Roguelike_装备数据表.csv` - 装备系统数据
4. `The_Archer_Roguelike_技能数据表.csv` - 技能系统数据

### ScriptableObject 转 JSON 工具

项目提供了 SO-JSON 双向转换工具，方便 AI 读取和修改 Unity 配置数据。

#### 使用方法
1. Unity 菜单：`Tools → UGC → SO-JSON Converter`
2. 点击 **"导出所有 SO 到 JSON"** → 导出到 `Assets/UGC_Configs/`
3. AI 读取并修改 JSON 文件
4. 点击 **"从 JSON 导入回 SO"** → 应用修改回 Unity

#### 导出的 JSON 格式
```json
{
  "assetPath": "Assets/The Archer/Scriptables/Enemies Database.asset",
  "enemies": [
    {
      "name": "Cauldron",
      "prefab": "Assets/The Archer/Prefabs/Enemies/Cauldron.prefab",
      "icon": "Assets/The Archer/Sprites/Icons/cauldron.png",
      "enemyType": 0,
      "isBoss": false,
      "drops": [
        {
          "dropType": 3,
          "itemData": null,
          "hpZones": [
            {
              "hp": 1,
              "dropAmount": { "value": 0, "min": 3, "max": 4, "propertyType": 1 },
              "chance": 100
            }
          ],
          "selectedZoneId": 0
        }
      ]
    }
  ]
}
```

#### 字段说明
- `assetPath`: SO 文件路径（必须保留）
- 资源引用：直接使用路径字符串
- 枚举：直接使用数值
- 嵌套对象：完整保留结构

#### 注意事项
- 不要删除 `assetPath` 字段
- 修改资源路径时确保路径存在

## 上传步骤

### 步骤1: 检查文件格式
- ✅ 确保CSV文件编码为UTF-8
- ✅ 确保没有特殊字符导致的格式问题
- ✅ 检查表头是否完整

### 步骤2: 平台上传
1. 登录UGC平台
2. 选择"数据导入"或"批量上传"功能
3. 选择对应的数据类型（项目信息/敌人数据/装备数据/技能数据）
4. 上传对应的CSV文件
5. 预览并确认数据正确性
6. 提交上传

### 步骤3: 数据验证
- 检查上传后的数据是否完整
- 验证数值计算是否正确
- 确认关联关系是否正常

## 注意事项

### 数据完整性
- 所有必填字段都已填写
- 数值范围符合游戏逻辑
- 关联数据保持一致性

### 格式兼容性
- CSV使用逗号分隔
- 文本字段用双引号包围
- 数值字段不包含格式化符号

### 平台特殊要求
- 某些平台可能需要特定的字段名称
- 部分平台对数据量有限制
- 可能需要分批上传大量数据

## 备用方案

如果CSV格式不兼容，可以：
1. 转换为Excel格式
2. 调整字段名称和格式
3. 分割大文件为小文件
4. 使用平台提供的模板格式

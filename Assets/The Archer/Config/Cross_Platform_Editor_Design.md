# 🎨 跨平台游戏编辑器设计文档

> **版本**: 1.0.0  
> **日期**: 2025-01-29  
> **目标**: 基于 Flutter 的跨平台游戏编辑器，支持 Web/Mac/Android/iOS

---

## 一、编辑器概述

### 1.1 核心功能

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                        跨平台游戏编辑器核心功能                                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         编辑器功能矩阵                                   │   │
│  │                                                                          │   │
│  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐      │   │
│  │  │  模板选择        │  │  配置编辑        │  │  资源管理        │      │   │
│  │  │                  │  │                  │  │                  │      │   │
│  │  │  • 肉鸽模板      │  │  • 数值配置      │  │  • 图片导入      │      │   │
│  │  │  • 休闲模板      │  │  • 关卡配置      │  │  • AI 生成       │      │   │
│  │  │  • 跑酷模板      │  │  • 敌人配置      │  │  • AI 优化       │      │   │
│  │  │  • 塔防模板      │  │  • 能力配置      │  │  • 资源预览      │      │   │
│  │  └──────────────────┘  └──────────────────┘  └──────────────────┘      │   │
│  │                                                                          │   │
│  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐      │   │
│  │  │  AI 对话        │  │  发布打包        │  │  预览测试        │      │   │
│  │  │                  │  │                  │  │                  │      │   │
│  │  │  • 自然语言      │  │  • 资源合并      │  │  • launcher_key  │      │   │
│  │  │  • 资源生成      │  │  • 图集打包      │  │  • 在线预览      │      │   │
│  │  │  • 配置调整      │  │  • CDN 上传      │  │  • 分享链接      │      │   │
│  │  │  • 智能建议      │  │  • 生成 key      │  │                  │      │   │
│  │  └──────────────────┘  └──────────────────┘  └──────────────────┘      │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 1.2 平台功能差异

| 功能 | Web | Mac | Android | iOS |
|------|-----|-----|---------|-----|
| **模板选择** | ✅ | ✅ | ✅ | ✅ |
| **配置编辑** | ✅ | ✅ | ✅ | ✅ |
| **资源导入** | ✅ | ✅ | ✅ | ✅ |
| **AI 资源生成** | ✅ | ✅ | ✅ | ✅ |
| **AI 资源优化** | ✅ | ✅ | ✅ | ✅ |
| **Luau 脚本编辑** | ❌ | ✅ (可选) | ❌ | ❌ |
| **发布打包** | ✅ | ✅ | ✅ | ✅ |
| **预览测试** | ✅ | ✅ | ✅ | ✅ |

> **注意**: Web/Android/iOS 平台不支持 Luau 脚本编辑，保持模板默认代码

---

## 二、系统架构

### 2.1 整体架构

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           编辑器系统架构                                         │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         Flutter 应用层                                   │   │
│  │                                                                          │   │
│  │  ┌──────────────────────────────────────────────────────────────────┐  │   │
│  │  │                         UI 层                                     │  │   │
│  │  │                                                                   │  │   │
│  │  │  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐    │  │   │
│  │  │  │ 模板选择页 │ │ 配置编辑页 │ │ 资源管理页 │ │ AI对话页   │    │  │   │
│  │  │  └────────────┘ └────────────┘ └────────────┘ └────────────┘    │  │   │
│  │  │  ┌────────────┐ ┌────────────┐ ┌────────────┐                   │  │   │
│  │  │  │ 发布页     │ │ 预览页     │ │ 设置页     │                   │  │   │
│  │  │  └────────────┘ └────────────┘ └────────────┘                   │  │   │
│  │  └──────────────────────────────────────────────────────────────────┘  │   │
│  │                                                                          │   │
│  │  ┌──────────────────────────────────────────────────────────────────┐  │   │
│  │  │                         业务逻辑层                                │  │   │
│  │  │                                                                   │  │   │
│  │  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐     │  │   │
│  │  │  │ TemplateService│  │ ConfigService  │  │ ResourceService│     │  │   │
│  │  │  │                │  │                │  │                │     │  │   │
│  │  │  │ • 加载模板     │  │ • 解析 Schema  │  │ • 资源导入     │     │  │   │
│  │  │  │ • 获取 Meta    │  │ • 验证配置     │  │ • 资源预览     │     │  │   │
│  │  │  │ • 模板列表     │  │ • 保存配置     │  │ • 资源管理     │     │  │   │
│  │  │  └────────────────┘  └────────────────┘  └────────────────┘     │  │   │
│  │  │                                                                   │  │   │
│  │  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐     │  │   │
│  │  │  │ AIService      │  │ PublishService │  │ PreviewService │     │  │   │
│  │  │  │                │  │                │  │                │     │  │   │
│  │  │  │ • 自然语言处理 │  │ • 资源合并     │  │ • 生成预览链接 │     │  │   │
│  │  │  │ • 图像生成     │  │ • 图集打包     │  │ • 加载预览     │     │  │   │
│  │  │  │ • 图像优化     │  │ • CDN 上传     │  │                │     │  │   │
│  │  │  │ • 配置建议     │  │ • 生成 key     │  │                │     │  │   │
│  │  │  └────────────────┘  └────────────────┘  └────────────────┘     │  │   │
│  │  │                                                                   │  │   │
│  │  └──────────────────────────────────────────────────────────────────┘  │   │
│  │                                                                          │   │
│  │  ┌──────────────────────────────────────────────────────────────────┐  │   │
│  │  │                         数据层                                    │  │   │
│  │  │                                                                   │  │   │
│  │  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐     │  │   │
│  │  │  │ LocalStorage   │  │ APIClient      │  │ FileManager    │     │  │   │
│  │  │  │                │  │                │  │                │     │  │   │
│  │  │  │ • 本地缓存     │  │ • 后端 API     │  │ • 文件读写     │     │  │   │
│  │  │  │ • 草稿保存     │  │ • AI API       │  │ • 资源管理     │     │  │   │
│  │  │  │ • 用户设置     │  │ • CDN API      │  │                │     │  │   │
│  │  │  └────────────────┘  └────────────────┘  └────────────────┘     │  │   │
│  │  │                                                                   │  │   │
│  │  └──────────────────────────────────────────────────────────────────┘  │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                         后端服务                                         │   │
│  │                                                                          │   │
│  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐            │   │
│  │  │ 模板服务       │  │ AI 服务        │  │ 发布服务       │            │   │
│  │  │                │  │                │  │                │            │   │
│  │  │ • 模板列表     │  │ • 图像生成     │  │ • 资源打包     │            │   │
│  │  │ • 模板下载     │  │ • 图像优化     │  │ • CDN 上传     │            │   │
│  │  │ • Schema 获取  │  │ • 自然语言     │  │ • Key 生成     │            │   │
│  │  └────────────────┘  └────────────────┘  └────────────────┘            │   │
│  │                                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 三、核心功能设计

### 3.1 模板选择

#### 3.1.1 模板数据结构

```dart
// lib/models/template.dart

class GameTemplate {
  final String id;
  final String name;
  final String description;
  final String thumbnailUrl;
  final String category;
  final TemplateMetadata metadata;
  final List<String> features;
  final DateTime createdAt;
  final DateTime updatedAt;
  
  GameTemplate({
    required this.id,
    required this.name,
    required this.description,
    required this.thumbnailUrl,
    required this.category,
    required this.metadata,
    required this.features,
    required this.createdAt,
    required this.updatedAt,
  });
  
  factory GameTemplate.fromJson(Map<String, dynamic> json) {
    return GameTemplate(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      thumbnailUrl: json['thumbnail_url'],
      category: json['category'],
      metadata: TemplateMetadata.fromJson(json['metadata']),
      features: List<String>.from(json['features']),
      createdAt: DateTime.parse(json['created_at']),
      updatedAt: DateTime.parse(json['updated_at']),
    );
  }
}

class TemplateMetadata {
  final String schemaUrl;
  final String defaultConfigUrl;
  final String defaultResourcesUrl;
  final List<ConfigSection> configSections;
  final List<ResourceCategory> resourceCategories;
  
  TemplateMetadata({
    required this.schemaUrl,
    required this.defaultConfigUrl,
    required this.defaultResourcesUrl,
    required this.configSections,
    required this.resourceCategories,
  });
  
  factory TemplateMetadata.fromJson(Map<String, dynamic> json) {
    return TemplateMetadata(
      schemaUrl: json['schema_url'],
      defaultConfigUrl: json['default_config_url'],
      defaultResourcesUrl: json['default_resources_url'],
      configSections: (json['config_sections'] as List)
          .map((e) => ConfigSection.fromJson(e))
          .toList(),
      resourceCategories: (json['resource_categories'] as List)
          .map((e) => ResourceCategory.fromJson(e))
          .toList(),
    );
  }
}

class ConfigSection {
  final String id;
  final String name;
  final String icon;
  final List<ConfigField> fields;
  
  ConfigSection({
    required this.id,
    required this.name,
    required this.icon,
    required this.fields,
  });
  
  factory ConfigSection.fromJson(Map<String, dynamic> json) {
    return ConfigSection(
      id: json['id'],
      name: json['name'],
      icon: json['icon'],
      fields: (json['fields'] as List)
          .map((e) => ConfigField.fromJson(e))
          .toList(),
    );
  }
}

class ConfigField {
  final String id;
  final String name;
  final String type; // number, string, boolean, array, object, enum
  final dynamic defaultValue;
  final Map<String, dynamic>? constraints; // min, max, options, etc.
  final String? description;
  
  ConfigField({
    required this.id,
    required this.name,
    required this.type,
    required this.defaultValue,
    this.constraints,
    this.description,
  });
  
  factory ConfigField.fromJson(Map<String, dynamic> json) {
    return ConfigField(
      id: json['id'],
      name: json['name'],
      type: json['type'],
      defaultValue: json['default_value'],
      constraints: json['constraints'],
      description: json['description'],
    );
  }
}
```

#### 3.1.2 模板选择界面

```dart
// lib/features/template_selector/template_selector_view.dart

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class TemplateSelectorView extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final templatesAsync = ref.watch(templatesProvider);
    
    return Scaffold(
      appBar: AppBar(
        title: Text('选择游戏模板'),
      ),
      body: templatesAsync.when(
        loading: () => Center(child: CircularProgressIndicator()),
        error: (err, stack) => Center(child: Text('加载失败: $err')),
        data: (templates) => GridView.builder(
          padding: EdgeInsets.all(16),
          gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: _getCrossAxisCount(context),
            childAspectRatio: 0.8,
            crossAxisSpacing: 16,
            mainAxisSpacing: 16,
          ),
          itemCount: templates.length,
          itemBuilder: (context, index) {
            return TemplateCard(
              template: templates[index],
              onTap: () => _selectTemplate(context, ref, templates[index]),
            );
          },
        ),
      ),
    );
  }
  
  int _getCrossAxisCount(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    if (width < 600) return 2;      // 手机
    if (width < 1200) return 3;     // 平板
    return 4;                        // 桌面
  }
  
  void _selectTemplate(BuildContext context, WidgetRef ref, GameTemplate template) async {
    // 显示加载对话框
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        content: Row(
          children: [
            CircularProgressIndicator(),
            SizedBox(width: 16),
            Text('正在加载模板...'),
          ],
        ),
      ),
    );
    
    // 加载模板元数据
    await ref.read(currentProjectProvider.notifier).loadTemplate(template);
    
    Navigator.pop(context); // 关闭加载对话框
    
    // 跳转到编辑器主界面
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (context) => EditorMainView()),
    );
  }
}

class TemplateCard extends StatelessWidget {
  final GameTemplate template;
  final VoidCallback onTap;
  
  const TemplateCard({required this.template, required this.onTap});
  
  @override
  Widget build(BuildContext context) {
    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Expanded(
              flex: 3,
              child: Image.network(
                template.thumbnailUrl,
                fit: BoxFit.cover,
              ),
            ),
            Expanded(
              flex: 2,
              child: Padding(
                padding: EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      template.name,
                      style: Theme.of(context).textTheme.titleMedium,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    SizedBox(height: 4),
                    Text(
                      template.description,
                      style: Theme.of(context).textTheme.bodySmall,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    Spacer(),
                    Wrap(
                      spacing: 4,
                      children: template.features.take(3).map((f) => Chip(
                        label: Text(f, style: TextStyle(fontSize: 10)),
                        padding: EdgeInsets.zero,
                        materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                      )).toList(),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
```

### 3.2 配置编辑

#### 3.2.1 动态配置编辑器

```dart
// lib/features/config_editor/config_editor_view.dart

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class ConfigEditorView extends ConsumerStatefulWidget {
  @override
  _ConfigEditorViewState createState() => _ConfigEditorViewState();
}

class _ConfigEditorViewState extends ConsumerState<ConfigEditorView> {
  String? selectedSectionId;
  
  @override
  Widget build(BuildContext context) {
    final project = ref.watch(currentProjectProvider);
    final sections = project?.template.metadata.configSections ?? [];
    
    return Row(
      children: [
        // 左侧：配置分类导航
        SizedBox(
          width: 200,
          child: ListView.builder(
            itemCount: sections.length,
            itemBuilder: (context, index) {
              final section = sections[index];
              final isSelected = section.id == selectedSectionId;
              
              return ListTile(
                leading: Icon(_getIconData(section.icon)),
                title: Text(section.name),
                selected: isSelected,
                onTap: () => setState(() => selectedSectionId = section.id),
              );
            },
          ),
        ),
        
        VerticalDivider(width: 1),
        
        // 右侧：配置编辑区域
        Expanded(
          child: selectedSectionId == null
              ? Center(child: Text('请选择配置分类'))
              : ConfigSectionEditor(
                  section: sections.firstWhere((s) => s.id == selectedSectionId),
                  config: project!.config,
                  onChanged: (key, value) {
                    ref.read(currentProjectProvider.notifier).updateConfig(key, value);
                  },
                ),
        ),
      ],
    );
  }
  
  IconData _getIconData(String iconName) {
    switch (iconName) {
      case 'player': return Icons.person;
      case 'enemy': return Icons.bug_report;
      case 'ability': return Icons.auto_awesome;
      case 'stage': return Icons.map;
      case 'item': return Icons.inventory;
      default: return Icons.settings;
    }
  }
}

class ConfigSectionEditor extends StatelessWidget {
  final ConfigSection section;
  final Map<String, dynamic> config;
  final Function(String key, dynamic value) onChanged;
  
  const ConfigSectionEditor({
    required this.section,
    required this.config,
    required this.onChanged,
  });
  
  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      padding: EdgeInsets.all(16),
      itemCount: section.fields.length,
      itemBuilder: (context, index) {
        final field = section.fields[index];
        return Padding(
          padding: EdgeInsets.only(bottom: 16),
          child: ConfigFieldWidget(
            field: field,
            value: _getFieldValue(field.id),
            onChanged: (value) => onChanged(field.id, value),
          ),
        );
      },
    );
  }
  
  dynamic _getFieldValue(String fieldId) {
    final parts = fieldId.split('.');
    dynamic value = config;
    for (final part in parts) {
      if (value is Map) {
        value = value[part];
      } else {
        return null;
      }
    }
    return value;
  }
}

class ConfigFieldWidget extends StatelessWidget {
  final ConfigField field;
  final dynamic value;
  final Function(dynamic) onChanged;
  
  const ConfigFieldWidget({
    required this.field,
    required this.value,
    required this.onChanged,
  });
  
  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Text(
              field.name,
              style: Theme.of(context).textTheme.titleSmall,
            ),
            if (field.description != null) ...[
              SizedBox(width: 8),
              Tooltip(
                message: field.description!,
                child: Icon(Icons.info_outline, size: 16),
              ),
            ],
          ],
        ),
        SizedBox(height: 8),
        _buildFieldInput(),
      ],
    );
  }
  
  Widget _buildFieldInput() {
    switch (field.type) {
      case 'number':
        return _buildNumberInput();
      case 'string':
        return _buildStringInput();
      case 'boolean':
        return _buildBooleanInput();
      case 'enum':
        return _buildEnumInput();
      case 'array':
        return _buildArrayInput();
      case 'object':
        return _buildObjectInput();
      default:
        return Text('不支持的字段类型: ${field.type}');
    }
  }
  
  Widget _buildNumberInput() {
    final min = field.constraints?['min']?.toDouble();
    final max = field.constraints?['max']?.toDouble();
    final step = field.constraints?['step']?.toDouble() ?? 1.0;
    
    return Row(
      children: [
        Expanded(
          child: Slider(
            value: (value ?? field.defaultValue).toDouble(),
            min: min ?? 0,
            max: max ?? 100,
            divisions: max != null && min != null ? ((max - min) / step).round() : null,
            label: value?.toString(),
            onChanged: (v) => onChanged(v),
          ),
        ),
        SizedBox(width: 16),
        SizedBox(
          width: 80,
          child: TextField(
            controller: TextEditingController(text: value?.toString() ?? ''),
            keyboardType: TextInputType.number,
            decoration: InputDecoration(
              border: OutlineInputBorder(),
              isDense: true,
            ),
            onSubmitted: (v) => onChanged(double.tryParse(v) ?? field.defaultValue),
          ),
        ),
      ],
    );
  }
  
  Widget _buildStringInput() {
    return TextField(
      controller: TextEditingController(text: value?.toString() ?? ''),
      decoration: InputDecoration(
        border: OutlineInputBorder(),
        hintText: field.defaultValue?.toString(),
      ),
      onChanged: onChanged,
    );
  }
  
  Widget _buildBooleanInput() {
    return Switch(
      value: value ?? field.defaultValue ?? false,
      onChanged: onChanged,
    );
  }
  
  Widget _buildEnumInput() {
    final options = field.constraints?['options'] as List? ?? [];
    
    return DropdownButtonFormField<String>(
      value: value?.toString(),
      decoration: InputDecoration(
        border: OutlineInputBorder(),
      ),
      items: options.map((o) => DropdownMenuItem(
        value: o.toString(),
        child: Text(o.toString()),
      )).toList(),
      onChanged: (v) => onChanged(v),
    );
  }
  
  Widget _buildArrayInput() {
    // 数组编辑器
    return ArrayFieldEditor(
      value: value as List? ?? [],
      itemType: field.constraints?['item_type'] ?? 'string',
      onChanged: onChanged,
    );
  }
  
  Widget _buildObjectInput() {
    // 对象编辑器
    return ObjectFieldEditor(
      value: value as Map<String, dynamic>? ?? {},
      schema: field.constraints?['schema'] ?? {},
      onChanged: onChanged,
    );
  }
}
```

### 3.3 资源管理与 AI 优化

#### 3.3.1 资源管理界面

```dart
// lib/features/resource_manager/resource_manager_view.dart

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:file_picker/file_picker.dart';

class ResourceManagerView extends ConsumerStatefulWidget {
  @override
  _ResourceManagerViewState createState() => _ResourceManagerViewState();
}

class _ResourceManagerViewState extends ConsumerState<ResourceManagerView> {
  String selectedCategory = 'all';
  
  @override
  Widget build(BuildContext context) {
    final project = ref.watch(currentProjectProvider);
    final resources = project?.resources ?? [];
    final categories = project?.template.metadata.resourceCategories ?? [];
    
    return Column(
      children: [
        // 工具栏
        Padding(
          padding: EdgeInsets.all(16),
          child: Row(
            children: [
              // 分类筛选
              DropdownButton<String>(
                value: selectedCategory,
                items: [
                  DropdownMenuItem(value: 'all', child: Text('全部')),
                  ...categories.map((c) => DropdownMenuItem(
                    value: c.id,
                    child: Text(c.name),
                  )),
                ],
                onChanged: (v) => setState(() => selectedCategory = v!),
              ),
              
              Spacer(),
              
              // 导入按钮
              ElevatedButton.icon(
                icon: Icon(Icons.upload_file),
                label: Text('导入资源'),
                onPressed: _importResources,
              ),
              
              SizedBox(width: 8),
              
              // AI 生成按钮
              ElevatedButton.icon(
                icon: Icon(Icons.auto_awesome),
                label: Text('AI 生成'),
                onPressed: () => _showAIGenerateDialog(context),
              ),
            ],
          ),
        ),
        
        Divider(height: 1),
        
        // 资源网格
        Expanded(
          child: GridView.builder(
            padding: EdgeInsets.all(16),
            gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: _getCrossAxisCount(context),
              childAspectRatio: 1,
              crossAxisSpacing: 12,
              mainAxisSpacing: 12,
            ),
            itemCount: _getFilteredResources(resources).length,
            itemBuilder: (context, index) {
              final resource = _getFilteredResources(resources)[index];
              return ResourceTile(
                resource: resource,
                onTap: () => _showResourceDetail(context, resource),
                onAIOptimize: () => _showAIOptimizeDialog(context, resource),
              );
            },
          ),
        ),
      ],
    );
  }
  
  int _getCrossAxisCount(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    if (width < 600) return 3;
    if (width < 1200) return 5;
    return 8;
  }
  
  List<ResourceItem> _getFilteredResources(List<ResourceItem> resources) {
    if (selectedCategory == 'all') return resources;
    return resources.where((r) => r.category == selectedCategory).toList();
  }
  
  Future<void> _importResources() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: ['png', 'jpg', 'gif'],
      allowMultiple: true,
    );
    
    if (result != null) {
      for (var file in result.files) {
        await ref.read(currentProjectProvider.notifier).importResource(file);
      }
    }
  }
  
  void _showAIGenerateDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AIGenerateDialog(
        onGenerate: (prompt, style, category) async {
          Navigator.pop(context);
          await ref.read(currentProjectProvider.notifier).generateResource(
            prompt: prompt,
            style: style,
            category: category,
          );
        },
      ),
    );
  }
  
  void _showAIOptimizeDialog(BuildContext context, ResourceItem resource) {
    showDialog(
      context: context,
      builder: (context) => AIOptimizeDialog(
        resource: resource,
        onOptimize: (action, params) async {
          Navigator.pop(context);
          await ref.read(currentProjectProvider.notifier).optimizeResource(
            resource: resource,
            action: action,
            params: params,
          );
        },
      ),
    );
  }
}

class ResourceTile extends StatelessWidget {
  final ResourceItem resource;
  final VoidCallback onTap;
  final VoidCallback onAIOptimize;
  
  const ResourceTile({
    required this.resource,
    required this.onTap,
    required this.onAIOptimize,
  });
  
  @override
  Widget build(BuildContext context) {
    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Stack(
          children: [
            // 资源预览
            Positioned.fill(
              child: Image.memory(
                resource.thumbnailBytes,
                fit: BoxFit.cover,
              ),
            ),
            
            // 底部信息栏
            Positioned(
              left: 0,
              right: 0,
              bottom: 0,
              child: Container(
                color: Colors.black54,
                padding: EdgeInsets.all(4),
                child: Text(
                  resource.name,
                  style: TextStyle(color: Colors.white, fontSize: 10),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ),
            
            // AI 优化按钮
            Positioned(
              top: 4,
              right: 4,
              child: IconButton(
                icon: Icon(Icons.auto_awesome, color: Colors.white, size: 20),
                onPressed: onAIOptimize,
                style: IconButton.styleFrom(
                  backgroundColor: Colors.black54,
                  padding: EdgeInsets.all(4),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
```

#### 3.3.2 AI 优化对话框

```dart
// lib/features/resource_manager/ai_optimize_dialog.dart

import 'package:flutter/material.dart';

class AIOptimizeDialog extends StatefulWidget {
  final ResourceItem resource;
  final Function(String action, Map<String, dynamic> params) onOptimize;
  
  const AIOptimizeDialog({
    required this.resource,
    required this.onOptimize,
  });
  
  @override
  _AIOptimizeDialogState createState() => _AIOptimizeDialogState();
}

class _AIOptimizeDialogState extends State<AIOptimizeDialog> {
  String selectedAction = 'enhance';
  final TextEditingController promptController = TextEditingController();
  
  final List<AIOptimizeAction> actions = [
    AIOptimizeAction(
      id: 'enhance',
      name: '画质增强',
      description: '提升图片分辨率和清晰度',
      icon: Icons.hd,
    ),
    AIOptimizeAction(
      id: 'remove_bg',
      name: '去除背景',
      description: '自动去除图片背景，保留主体',
      icon: Icons.content_cut,
    ),
    AIOptimizeAction(
      id: 'change_style',
      name: '风格转换',
      description: '将图片转换为其他艺术风格',
      icon: Icons.palette,
    ),
    AIOptimizeAction(
      id: 'recolor',
      name: '重新配色',
      description: '调整图片的颜色方案',
      icon: Icons.color_lens,
    ),
    AIOptimizeAction(
      id: 'custom',
      name: '自定义修改',
      description: '使用自然语言描述你想要的修改',
      icon: Icons.edit,
    ),
  ];
  
  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text('AI 优化资源'),
      content: SizedBox(
        width: 400,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // 资源预览
            Container(
              height: 150,
              width: double.infinity,
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Image.memory(
                widget.resource.thumbnailBytes,
                fit: BoxFit.contain,
              ),
            ),
            
            SizedBox(height: 16),
            
            // 操作选择
            ...actions.map((action) => RadioListTile<String>(
              title: Row(
                children: [
                  Icon(action.icon, size: 20),
                  SizedBox(width: 8),
                  Text(action.name),
                ],
              ),
              subtitle: Text(action.description, style: TextStyle(fontSize: 12)),
              value: action.id,
              groupValue: selectedAction,
              onChanged: (v) => setState(() => selectedAction = v!),
            )),
            
            // 自定义输入
            if (selectedAction == 'custom') ...[
              SizedBox(height: 16),
              TextField(
                controller: promptController,
                decoration: InputDecoration(
                  labelText: '描述你想要的修改',
                  hintText: '例如：把角色的衣服改成红色',
                  border: OutlineInputBorder(),
                ),
                maxLines: 3,
              ),
            ],
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: Text('取消'),
        ),
        ElevatedButton.icon(
          icon: Icon(Icons.auto_awesome),
          label: Text('开始优化'),
          onPressed: () {
            widget.onOptimize(selectedAction, {
              'prompt': promptController.text,
            });
          },
        ),
      ],
    );
  }
}

class AIOptimizeAction {
  final String id;
  final String name;
  final String description;
  final IconData icon;
  
  AIOptimizeAction({
    required this.id,
    required this.name,
    required this.description,
    required this.icon,
  });
}
```

### 3.4 AI 自然语言对话

```dart
// lib/features/ai_chat/ai_chat_view.dart

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class AIChatView extends ConsumerStatefulWidget {
  @override
  _AIChatViewState createState() => _AIChatViewState();
}

class _AIChatViewState extends ConsumerState<AIChatView> {
  final TextEditingController _controller = TextEditingController();
  final ScrollController _scrollController = ScrollController();
  
  @override
  Widget build(BuildContext context) {
    final chatState = ref.watch(aiChatProvider);
    
    return Column(
      children: [
        // 消息列表
        Expanded(
          child: ListView.builder(
            controller: _scrollController,
            padding: EdgeInsets.all(16),
            itemCount: chatState.messages.length,
            itemBuilder: (context, index) {
              final message = chatState.messages[index];
              return ChatMessageWidget(message: message);
            },
          ),
        ),
        
        // 加载指示器
        if (chatState.isLoading)
          Padding(
            padding: EdgeInsets.all(8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                ),
                SizedBox(width: 8),
                Text('AI 正在思考...'),
              ],
            ),
          ),
        
        Divider(height: 1),
        
        // 输入区域
        Padding(
          padding: EdgeInsets.all(16),
          child: Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _controller,
                  decoration: InputDecoration(
                    hintText: '描述你想要的修改...',
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(24),
                    ),
                    contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                  ),
                  maxLines: null,
                  textInputAction: TextInputAction.send,
                  onSubmitted: _sendMessage,
                ),
              ),
              SizedBox(width: 8),
              IconButton(
                icon: Icon(Icons.send),
                onPressed: () => _sendMessage(_controller.text),
                style: IconButton.styleFrom(
                  backgroundColor: Theme.of(context).primaryColor,
                  foregroundColor: Colors.white,
                ),
              ),
            ],
          ),
        ),
        
        // 快捷操作
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          padding: EdgeInsets.symmetric(horizontal: 16),
          child: Row(
            children: [
              _buildQuickAction('生成新角色', Icons.person_add),
              _buildQuickAction('调整难度', Icons.tune),
              _buildQuickAction('添加新技能', Icons.auto_awesome),
              _buildQuickAction('修改配色', Icons.palette),
              _buildQuickAction('优化数值', Icons.analytics),
            ],
          ),
        ),
        
        SizedBox(height: 16),
      ],
    );
  }
  
  Widget _buildQuickAction(String label, IconData icon) {
    return Padding(
      padding: EdgeInsets.only(right: 8),
      child: ActionChip(
        avatar: Icon(icon, size: 16),
        label: Text(label),
        onPressed: () => _sendMessage(label),
      ),
    );
  }
  
  void _sendMessage(String text) async {
    if (text.trim().isEmpty) return;
    
    _controller.clear();
    
    await ref.read(aiChatProvider.notifier).sendMessage(text);
    
    // 滚动到底部
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _scrollController.animateTo(
        _scrollController.position.maxScrollExtent,
        duration: Duration(milliseconds: 300),
        curve: Curves.easeOut,
      );
    });
  }
}

class ChatMessageWidget extends StatelessWidget {
  final ChatMessage message;
  
  const ChatMessageWidget({required this.message});
  
  @override
  Widget build(BuildContext context) {
    final isUser = message.role == 'user';
    
    return Padding(
      padding: EdgeInsets.only(bottom: 16),
      child: Row(
        mainAxisAlignment: isUser ? MainAxisAlignment.end : MainAxisAlignment.start,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (!isUser) ...[
            CircleAvatar(
              backgroundColor: Colors.purple,
              child: Icon(Icons.auto_awesome, color: Colors.white, size: 20),
            ),
            SizedBox(width: 8),
          ],
          
          Flexible(
            child: Container(
              padding: EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: isUser ? Theme.of(context).primaryColor : Colors.grey[200],
                borderRadius: BorderRadius.circular(16),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    message.content,
                    style: TextStyle(
                      color: isUser ? Colors.white : Colors.black,
                    ),
                  ),
                  
                  // 显示 AI 执行的操作
                  if (message.actions != null && message.actions!.isNotEmpty) ...[
                    SizedBox(height: 8),
                    Divider(),
                    ...message.actions!.map((action) => Padding(
                      padding: EdgeInsets.only(top: 4),
                      child: Row(
                        children: [
                          Icon(
                            action.success ? Icons.check_circle : Icons.error,
                            color: action.success ? Colors.green : Colors.red,
                            size: 16,
                          ),
                          SizedBox(width: 4),
                          Expanded(
                            child: Text(
                              action.description,
                              style: TextStyle(fontSize: 12),
                            ),
                          ),
                        ],
                      ),
                    )),
                  ],
                ],
              ),
            ),
          ),
          
          if (isUser) ...[
            SizedBox(width: 8),
            CircleAvatar(
              backgroundColor: Colors.blue,
              child: Icon(Icons.person, color: Colors.white, size: 20),
            ),
          ],
        ],
      ),
    );
  }
}
```

### 3.5 发布打包

```dart
// lib/features/publish/publish_view.dart

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class PublishView extends ConsumerStatefulWidget {
  @override
  _PublishViewState createState() => _PublishViewState();
}

class _PublishViewState extends ConsumerState<PublishView> {
  final TextEditingController nameController = TextEditingController();
  final TextEditingController descriptionController = TextEditingController();
  
  @override
  Widget build(BuildContext context) {
    final publishState = ref.watch(publishProvider);
    
    return SingleChildScrollView(
      padding: EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            '发布游戏',
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          
          SizedBox(height: 24),
          
          // 基本信息
          Card(
            child: Padding(
              padding: EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('基本信息', style: Theme.of(context).textTheme.titleMedium),
                  SizedBox(height: 16),
                  
                  TextField(
                    controller: nameController,
                    decoration: InputDecoration(
                      labelText: '游戏名称',
                      border: OutlineInputBorder(),
                    ),
                  ),
                  
                  SizedBox(height: 16),
                  
                  TextField(
                    controller: descriptionController,
                    decoration: InputDecoration(
                      labelText: '游戏描述',
                      border: OutlineInputBorder(),
                    ),
                    maxLines: 3,
                  ),
                ],
              ),
            ),
          ),
          
          SizedBox(height: 16),
          
          // 发布进度
          if (publishState.isPublishing) ...[
            Card(
              child: Padding(
                padding: EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('发布进度', style: Theme.of(context).textTheme.titleMedium),
                    SizedBox(height: 16),
                    
                    _buildProgressStep('资源合并', publishState.step >= 1, publishState.step == 1),
                    _buildProgressStep('图集打包', publishState.step >= 2, publishState.step == 2),
                    _buildProgressStep('配置生成', publishState.step >= 3, publishState.step == 3),
                    _buildProgressStep('上传 CDN', publishState.step >= 4, publishState.step == 4),
                    _buildProgressStep('生成 Key', publishState.step >= 5, publishState.step == 5),
                    
                    SizedBox(height: 16),
                    
                    LinearProgressIndicator(value: publishState.progress),
                  ],
                ),
              ),
            ),
          ],
          
          // 发布结果
          if (publishState.launcherKey != null) ...[
            Card(
              color: Colors.green[50],
              child: Padding(
                padding: EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Icon(Icons.check_circle, color: Colors.green),
                        SizedBox(width: 8),
                        Text('发布成功！', style: Theme.of(context).textTheme.titleMedium),
                      ],
                    ),
                    
                    SizedBox(height: 16),
                    
                    Text('Launcher Key:'),
                    SizedBox(height: 8),
                    Container(
                      padding: EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: Colors.grey[200],
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Row(
                        children: [
                          Expanded(
                            child: SelectableText(
                              publishState.launcherKey!,
                              style: TextStyle(fontFamily: 'monospace'),
                            ),
                          ),
                          IconButton(
                            icon: Icon(Icons.copy),
                            onPressed: () => _copyToClipboard(publishState.launcherKey!),
                          ),
                        ],
                      ),
                    ),
                    
                    SizedBox(height: 16),
                    
                    Row(
                      children: [
                        ElevatedButton.icon(
                          icon: Icon(Icons.play_arrow),
                          label: Text('预览游戏'),
                          onPressed: () => _previewGame(publishState.launcherKey!),
                        ),
                        SizedBox(width: 8),
                        OutlinedButton.icon(
                          icon: Icon(Icons.share),
                          label: Text('分享'),
                          onPressed: () => _shareGame(publishState.launcherKey!),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
          
          SizedBox(height: 24),
          
          // 发布按钮
          if (!publishState.isPublishing && publishState.launcherKey == null)
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                icon: Icon(Icons.publish),
                label: Text('开始发布'),
                style: ElevatedButton.styleFrom(
                  padding: EdgeInsets.symmetric(vertical: 16),
                ),
                onPressed: _startPublish,
              ),
            ),
        ],
      ),
    );
  }
  
  Widget _buildProgressStep(String title, bool completed, bool current) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Icon(
            completed ? Icons.check_circle : (current ? Icons.radio_button_checked : Icons.radio_button_unchecked),
            color: completed ? Colors.green : (current ? Colors.blue : Colors.grey),
            size: 20,
          ),
          SizedBox(width: 8),
          Text(
            title,
            style: TextStyle(
              color: completed ? Colors.green : (current ? Colors.blue : Colors.grey),
              fontWeight: current ? FontWeight.bold : FontWeight.normal,
            ),
          ),
          if (current) ...[
            SizedBox(width: 8),
            SizedBox(
              width: 12,
              height: 12,
              child: CircularProgressIndicator(strokeWidth: 2),
            ),
          ],
        ],
      ),
    );
  }
  
  void _startPublish() async {
    await ref.read(publishProvider.notifier).publish(
      name: nameController.text,
      description: descriptionController.text,
    );
  }
  
  void _copyToClipboard(String text) {
    // 复制到剪贴板
  }
  
  void _previewGame(String launcherKey) {
    // 打开预览
  }
  
  void _shareGame(String launcherKey) {
    // 分享游戏
  }
}
```

---

## 四、发布流程

### 4.1 发布流程图

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           发布流程                                               │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  Step 1: 资源合并                                                                │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 收集所有修改过的资源                                                  │   │
│  │  • 按类型分组 (角色、图标、特效、背景)                                   │   │
│  │  • 验证资源完整性                                                        │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 2: 图集打包                                                                │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 使用 MaxRects 算法打包图集                                            │   │
│  │  • 生成 atlas.png 和 atlas.json                                          │   │
│  │  • 压缩优化图片                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 3: 配置生成                                                                │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 合并用户修改的配置                                                    │   │
│  │  • 生成 game_config.json                                                 │   │
│  │  • 生成 manifest.json                                                    │   │
│  │  • 生成动画配置、音频配置                                                │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 4: 上传 CDN                                                                │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 打包成 ZIP 文件                                                       │   │
│  │  • 上传到 CDN 服务器                                                     │   │
│  │  • 获取资源 URL                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                         │                                       │
│                                         ▼                                       │
│  Step 5: 生成 Launcher Key                                                       │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  • 生成唯一的 launcher_key                                               │   │
│  │  • 格式: {creator_id}-{package_id}-{version}-{checksum}                  │   │
│  │  • 注册到服务器                                                          │   │
│  │  • 返回预览链接                                                          │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 4.2 发布服务实现

```dart
// lib/services/publish_service.dart

import 'dart:typed_data';
import 'package:archive/archive.dart';

class PublishService {
  final ApiClient apiClient;
  final ResourcePacker resourcePacker;
  
  PublishService({
    required this.apiClient,
    required this.resourcePacker,
  });
  
  Stream<PublishProgress> publish(Project project, String name, String description) async* {
    // Step 1: 资源合并
    yield PublishProgress(step: 1, message: '正在合并资源...');
    final mergedResources = await _mergeResources(project);
    
    // Step 2: 图集打包
    yield PublishProgress(step: 2, message: '正在打包图集...');
    final atlases = await resourcePacker.packAtlases(mergedResources);
    
    // Step 3: 配置生成
    yield PublishProgress(step: 3, message: '正在生成配置...');
    final configs = await _generateConfigs(project, atlases);
    
    // Step 4: 上传 CDN
    yield PublishProgress(step: 4, message: '正在上传资源...');
    final packageZip = await _createPackageZip(atlases, configs);
    final cdnUrl = await apiClient.uploadPackage(packageZip);
    
    // Step 5: 生成 Launcher Key
    yield PublishProgress(step: 5, message: '正在生成 Key...');
    final launcherKey = await apiClient.registerPackage(
      name: name,
      description: description,
      cdnUrl: cdnUrl,
      templateId: project.template.id,
    );
    
    yield PublishProgress(
      step: 6,
      message: '发布成功！',
      launcherKey: launcherKey,
      previewUrl: 'https://game.example.com/play/$launcherKey',
    );
  }
  
  Future<List<MergedResource>> _mergeResources(Project project) async {
    final resources = <MergedResource>[];
    
    for (final resource in project.resources) {
      resources.add(MergedResource(
        id: resource.id,
        category: resource.category,
        data: resource.data,
        metadata: resource.metadata,
      ));
    }
    
    return resources;
  }
  
  Future<Map<String, dynamic>> _generateConfigs(Project project, List<Atlas> atlases) async {
    // 生成 manifest.json
    final manifest = {
      'manifest_version': '1.0',
      'package_info': {
        'id': 'pkg_${DateTime.now().millisecondsSinceEpoch}',
        'name': project.name,
        'version': '1.0.0',
      },
      'template': {
        'id': project.template.id,
        'version': project.template.metadata.version,
      },
      'content': {
        'config': {'game_config': 'game_config.json'},
        'atlases': atlases.map((a) => {
          'name': a.name,
          'path': 'atlases/${a.name}.atlas.json',
        }).toList(),
      },
    };
    
    // 生成 game_config.json
    final gameConfig = project.config;
    
    // 生成图集配置
    final atlasConfigs = <String, dynamic>{};
    for (final atlas in atlases) {
      atlasConfigs[atlas.name] = atlas.toJson();
    }
    
    return {
      'manifest.json': manifest,
      'game_config.json': gameConfig,
      'atlases': atlasConfigs,
    };
  }
  
  Future<Uint8List> _createPackageZip(List<Atlas> atlases, Map<String, dynamic> configs) async {
    final archive = Archive();
    
    // 添加配置文件
    archive.addFile(ArchiveFile(
      'manifest.json',
      jsonEncode(configs['manifest.json']).length,
      utf8.encode(jsonEncode(configs['manifest.json'])),
    ));
    
    archive.addFile(ArchiveFile(
      'game_config.json',
      jsonEncode(configs['game_config.json']).length,
      utf8.encode(jsonEncode(configs['game_config.json'])),
    ));
    
    // 添加图集文件
    for (final atlas in atlases) {
      archive.addFile(ArchiveFile(
        'atlases/${atlas.name}.atlas.json',
        jsonEncode(atlas.toJson()).length,
        utf8.encode(jsonEncode(atlas.toJson())),
      ));
      
      archive.addFile(ArchiveFile(
        'atlases/${atlas.name}.atlas.png',
        atlas.imageData.length,
        atlas.imageData,
      ));
    }
    
    return Uint8List.fromList(ZipEncoder().encode(archive)!);
  }
}

class PublishProgress {
  final int step;
  final String message;
  final String? launcherKey;
  final String? previewUrl;
  
  PublishProgress({
    required this.step,
    required this.message,
    this.launcherKey,
    this.previewUrl,
  });
}
```

---

## 五、预览与测试

### 5.1 预览方式

| 平台 | 预览方式 |
|------|----------|
| **Web** | 直接在浏览器中打开 Unity WebGL 预览 |
| **Mac** | 启动本地 Unity 应用预览 |
| **Android** | 跳转到已安装的游戏 App，传入 launcher_key |
| **iOS** | 跳转到已安装的游戏 App，传入 launcher_key |

### 5.2 预览实现

```dart
// lib/services/preview_service.dart

import 'package:url_launcher/url_launcher.dart';
import 'dart:io';

class PreviewService {
  Future<void> preview(String launcherKey) async {
    if (kIsWeb) {
      // Web 平台：打开 WebGL 预览
      await _previewWeb(launcherKey);
    } else if (Platform.isMacOS) {
      // Mac 平台：启动本地应用
      await _previewMac(launcherKey);
    } else if (Platform.isAndroid) {
      // Android 平台：跳转到游戏 App
      await _previewAndroid(launcherKey);
    } else if (Platform.isIOS) {
      // iOS 平台：跳转到游戏 App
      await _previewIOS(launcherKey);
    }
  }
  
  Future<void> _previewWeb(String launcherKey) async {
    final url = Uri.parse('https://game.example.com/play/$launcherKey');
    await launchUrl(url, mode: LaunchMode.externalApplication);
  }
  
  Future<void> _previewMac(String launcherKey) async {
    // 检查本地是否安装了游戏
    final gamePath = '/Applications/GamePlayer.app';
    if (await Directory(gamePath).exists()) {
      await Process.run('open', [
        '-a', gamePath,
        '--args', '--launcher-key', launcherKey,
      ]);
    } else {
      // 回退到 Web 预览
      await _previewWeb(launcherKey);
    }
  }
  
  Future<void> _previewAndroid(String launcherKey) async {
    // 使用 Deep Link 跳转到游戏 App
    final url = Uri.parse('gameplayer://play?key=$launcherKey');
    if (await canLaunchUrl(url)) {
      await launchUrl(url);
    } else {
      // 回退到 Web 预览
      await _previewWeb(launcherKey);
    }
  }
  
  Future<void> _previewIOS(String launcherKey) async {
    // 使用 Universal Link 跳转到游戏 App
    final url = Uri.parse('https://game.example.com/app/play/$launcherKey');
    await launchUrl(url, mode: LaunchMode.externalApplication);
  }
}
```

---

## 六、总结

### 6.1 编辑器功能矩阵

| 功能 | Web | Mac | Android | iOS |
|------|-----|-----|---------|-----|
| 模板选择 | ✅ | ✅ | ✅ | ✅ |
| 配置编辑 (基于 Meta 动态生成) | ✅ | ✅ | ✅ | ✅ |
| 资源导入 | ✅ | ✅ | ✅ | ✅ |
| AI 资源生成 | ✅ | ✅ | ✅ | ✅ |
| AI 资源优化 | ✅ | ✅ | ✅ | ✅ |
| AI 自然语言对话 | ✅ | ✅ | ✅ | ✅ |
| Luau 脚本编辑 | ❌ | ✅ | ❌ | ❌ |
| 发布打包 | ✅ | ✅ | ✅ | ✅ |
| 预览测试 | ✅ | ✅ | ✅ | ✅ |

### 6.2 核心流程

```
选择模板 → 加载 Meta → 动态生成编辑界面 → 编辑配置/资源 → AI 优化 → 发布 → 生成 launcher_key → 预览测试
```

这套编辑器设计实现了：

1. ✅ **跨平台支持** - Web/Mac/Android/iOS 统一体验
2. ✅ **动态配置编辑** - 基于模板 Meta 自动生成编辑界面
3. ✅ **AI 辅助创作** - 自然语言驱动资源生成和优化
4. ✅ **一键发布** - 资源合并、图集打包、CDN 上传
5. ✅ **即时预览** - 通过 launcher_key 快速预览测试

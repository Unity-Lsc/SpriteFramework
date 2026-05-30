# SpriteFramework

SpriteFramework 是一个基于 Unity 的客户端框架示例工程，项目集成了 YooAsset 资源管理、HybridCLR 热更新、Excel 配表生成、流程状态机、UI、场景、音频、对象池、HTTP、事件、任务、计时器等常用游戏客户端基础模块。

## 项目结构

```text
SpriteFramework/
├── Client/                         Unity 客户端工程目录
│   ├── Assets/
│   │   ├── Framework/              框架层代码、启动场景、编辑器工具、通用系统
│   │   ├── Game/                   业务层代码、热更资源、UI、场景、音频、配置 bytes
│   │   ├── HybridCLRData/          HybridCLR 相关生成数据
│   │   └── StreamingAssets/        YooAsset 内置资源输出目录
│   ├── Packages/                   Unity Package Manager 依赖
│   └── ProjectSettings/            Unity 工程设置
├── Public/
│   ├── Tables/                     Excel 配表源文件
│   └── ExcelTool/                  Excel 转 bytes 和 C# 数据类工具
├── AssetBundle和HybridCLR使用指南.txt
├── .gitignore
└── README.md
```

## 环境要求

- Unity：`2022.3.62f2c1`
- 推荐安装 Windows Build Support / IL2CPP 模块，HybridCLR 生成和真机包测试会用到。
- Unity Package 主要依赖：
  - HybridCLR：`com.code-philosophy.hybridclr`
  - YooAsset：`com.tuyoogame.yooasset 2.3.17`
  - UniTask
  - Newtonsoft Json
  - UGUI
- ExcelTool 依赖 NuGet 包，主要用于读取 `.xls` / `.xlsx` 并生成二进制配置与 C# 数据类。

## 快速启动

1. 使用 Unity Hub 打开 `Client` 目录。
2. 等待 Unity 恢复 Packages 和导入资源。
3. 打开启动场景：

```text
Client/Assets/Framework/Scene_Launch.unity
```

4. 在场景中的 `MainEntry` 对象上选择资源运行模式。
   - `EditorSimulateMode`：编辑器模拟模式，适合本地开发调试。
   - `OfflinePlayMode`：单机内置资源模式。
   - `HostPlayMode`：远端资源站点模式，用于测试资源下载和热更新。
   - `WebPlayMode`：WebGL / 小游戏相关模式。
5. 运行 `Scene_Launch`。

## 启动流程概览

项目启动大致分为两层：

1. `MainEntry`：负责 YooAsset 初始化、版本检查、资源下载、加载 HybridCLR 热更程序集，并加载 `GameEntry.prefab`。
2. `GameEntry`：框架主入口，初始化并驱动各个管理器。

核心管理器包括：

- `EventManager`：事件系统
- `FsmManager`：有限状态机
- `ProcedureManager`：流程控制
- `DataTableManager`：配置表管理
- `ModelManager`：数据模型管理
- `HttpManager`：HTTP 请求
- `PoolManager`：对象池和类对象池
- `SpriteSceneManager`：场景加载
- `LoaderManager`：资源加载封装
- `UIManager`：UI 打开、关闭、分组、缓存
- `TimeManager`：计时器
- `AudioManager`：BGM 和音效
- `PlayerPrefsManager`：本地存储
- `TaskManager`：任务调度

业务层入口是 `GameApp`，会注册业务配置表，并封装 UI、音频、场景、对话框、提示、HTTP 等业务服务。

## 主要目录说明

### Framework

```text
Client/Assets/Framework/
```

框架层，提供基础能力和编辑器工具。运行时代码主要在：

```text
Client/Assets/Framework/Runtime/
```

常用模块位于：

```text
Client/Assets/Framework/Runtime/Core/Managers/
```

### Game

```text
Client/Assets/Game/
```

业务层，包含业务脚本、可下载资源、配置表 bytes、UI 预制体、场景、音频和热更 DLL bytes。

重要子目录：

```text
Client/Assets/Game/Scripts/Application/       业务服务入口和封装
Client/Assets/Game/Scripts/Procedure/         游戏流程状态
Client/Assets/Game/Scripts/Proto/DataTable/   配表生成代码和扩展代码
Client/Assets/Game/Download/                  YooAsset 收集和热更新资源目录
```

### Public

```text
Public/Tables/
Public/ExcelTool/
```

`Tables` 存放 Excel 源表。`ExcelTool` 用于把 Excel 转成 Unity 运行时读取的 `.bytes` 文件，并生成对应的 Entity / DBModel C# 代码。

当前表包括：

- `Sys_UIForm.xls`
- `Sys_Tip.xls`
- `Sys_Scene.xls`
- `Sys_Dialog.xls`
- `Sys_BGM.xls`
- `Sys_Audio.xls`

## 配表工作流

ExcelTool 会读取配置文件 `config.txt` 中的三行路径：

```text
Excel 源目录
bytes 输出目录
C# 代码输出目录
```

建议配置为类似：

```text
..\Tables
..\..\Client\Assets\Game\Download\DataTable
..\..\Client\Assets\Game\Scripts\Proto\DataTable\Create
```

生成结果：

- `.bytes` 输出到 `Client/Assets/Game/Download/DataTable/`
- 自动生成的数据类输出到 `Client/Assets/Game/Scripts/Proto/DataTable/Create/`
- 自定义扩展代码放在 `Client/Assets/Game/Scripts/Proto/DataTable/ExtCus/`，避免被生成代码覆盖。

新增配置表后，需要在：

```text
Client/Assets/Game/Scripts/Proto/DataTable/GameDataTable.cs
```

注册对应的 DBModel。

## YooAsset 与 HybridCLR 热更新流程

### 1. 安装和生成 HybridCLR

在 Unity 菜单中执行：

```text
HybridCLR/Installer
HybridCLR/Generate/All
```

如果切换了目标平台或 Development Build 设置，建议重新执行 `HybridCLR/Generate/All`。

### 2. 生成热更程序集

在 Unity 菜单中执行：

```text
SpriteTools/生成并拷贝热更新程序集到Download文件夹
```

该菜单会编译当前平台的热更 DLL，并把 `Assembly-CSharp.dll.bytes` 以及 AOT 补充程序集复制到：

```text
Client/Assets/Game/Download/Hotfix/
```

### 3. 打包 AssetBundle

打开 YooAsset 的 AssetBundle Builder 界面进行构建。内置资源复制选项建议使用：

```text
CopyBuildinFileOption = ClearAndCopyByTags
Tag = Launch
```

资源收集配置位于：

```text
Client/Assets/Game/AssetBundleCollectorSetting.asset
```

### 4. 部署远端资源

`HostPlayMode` 会从 `CheckVersionCtrl.GetHostServerURL()` 返回的地址拉取资源。当前默认地址形如：

```text
http://127.0.0.1/CDN/PC/v1.0
http://127.0.0.1/CDN/Android/v1.0
http://127.0.0.1/CDN/IPhone/v1.0
http://127.0.0.1/CDN/WebGL/v1.0
```

部署时需要保证 CDN 目录结构和 YooAsset 构建输出一致。

### 5. 编辑器测试

1. 打开 `Scene_Launch`。
2. 将 `MainEntry` 的运行模式切到 `HostPlayMode`。
3. 确保本地或远端资源服务器可访问。
4. 运行场景，观察版本检查、下载、热更程序集加载和 `GameEntry.prefab` 加载流程。

### 6. 真机或 PC 包验证

如果编辑器 `HostPlayMode` 能正常下载资源，可以构建 Windows / Android / iOS / WebGL 包进行验证。验证热更新时，通常只需要重新生成热更 DLL 和 AssetBundle，并更新资源站点，不需要重新打基础包。

## 常用 Unity 菜单

```text
SpriteTools/打开persistentDataPath
SpriteTools/设置文件动画循环为true
SpriteTools/生成并拷贝热更新程序集到Download文件夹
SpriteTools/打开全局参数设置
SpriteTools/打开宏设置
SpriteTools/打开类对象池计数界面
Assets/SpriteMenuExt/收集多个文件的路径到剪切板
Assets/SpriteMenuExt/收集多个文件的路径到剪切板(Resources)
Assets/SpriteMenuExt/收集多个文件的名字到剪切板
```

## 开发注意事项

- `Client/Assets/Game/Scripts/Proto/DataTable/Create/` 是生成代码目录，手写扩展请放到 `ExtCus/`。
- `Client/Assets/Game/Download/` 下的资源会参与 YooAsset 收集和热更新，请保持路径稳定。
- 修改热更代码后，需要重新生成热更程序集并重新打包对应 AssetBundle。
- 修改 AOT、构建平台、IL2CPP 或 Development Build 设置后，建议重新执行 HybridCLR 生成流程。
- 下载失败时，先用浏览器访问日志中提示的 URL，确认资源站点路径和文件是否存在。
- 项目中部分旧说明或注释可能存在编码显示异常，README 中已按当前代码和目录重新整理了关键流程。

## 参考文档

- YooAsset 文档：https://www.yooasset.com/docs
- HybridCLR 文档：https://hybridclr.doc.code-philosophy.com/
- 微信小游戏 Unity WebGL 转换：https://gitee.com/wechat-minigame/minigame-unity-webgl-transform

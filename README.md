# 简介
- 适合中大型U3D项目的框架,参考了多款主流商业框架
- 对开发中的常用模块进行了封装,可以很大程度上规范开发进程,加快开发进度
- 上手难度适中,模块之间依赖很少
- 支持自行删除和扩展模块

# SpriteMain（框架初始运行模块）
- 框架的运行入口，在这里进行一些初始化和热更新检查的操作

# SpriteFramework(框架运行功能模块)
- 事件（Event）  使用观察者模式，进行事件的添加、移除和派发
- 定时器（Time）  定时功能，支持多种定时需求，比如技能CD，角色死亡复活等
- 数据表（DataTable）  数据表功能，针对游戏中常用的Excel配表形式，封装了表格数据的加载和读取(同时附带有Excel工具，生成对应的脚本)
- Web请求（Http）  提供使用短链接的功能，可以使用Get和Post的方法，向服务器发送请求数据并获取响应
- 多语言（Localization）  提供本地化多语言功能模块，附带有Text和Image两种类型的拓展脚本
- 对象池（Pool）  提供对象池缓存功能，目前支持类对象池(ClassObjectPool)和游戏物体对象池(GameObjectPool)
- 资源（Resource）  使用的是第三方的YooAsset插件
- 声音（Audio）  声音管理模块，支持全局音量调节, 音效音量单独配表
- 本地数据存储（PlayerPrefs）  基于Unity内置的PlayerPrefs进行封装，支持存储Object对象，性能有优化
- 有限状态机（FSM）  TODO
- 流程（Procedure）  TODO
- 界面（UI）  TODO
- 输入系统（Input）  TODO
- 任务（Task）  TODO

# 后续会增加的工具包
- 技能编辑器  TODO
- 相机控制  TODO
- 红点系统  TODO
- 新手引导  TODO
- UI无限滚动列表  TODO
- 摇杆  TODO

using UnityEngine;
using XLua;
using UnityEngine.UI;

namespace SpriteFramework
{
    /// <summary>
    /// Lua窗口
    /// </summary>
    public class LuaForm : UIFormBase
    {

        public delegate void OnAwakeHandler(Transform transform);
        private OnAwakeHandler _onAwake;

        public delegate void OnStartHandler();
        private OnStartHandler _onStart;

        public delegate void OnEnableHandler();
        private OnEnableHandler _onEnable;

        public delegate void OnDisableHandler();
        private OnDisableHandler _onDisable;

        public delegate void OnDestroyHandler();
        private OnDestroyHandler _onDestroy;

        private LuaTable _scriptEnv;
        private LuaEnv _luaEnv;

        [Header("Lua组件")]
        [SerializeField]
        private LuaComp[] _luaComps;
        public LuaComp[] LuaComps { get { return _luaComps; } }

        protected override void Awake() {
            base.Awake();
            //从LuaManager中获取，全局只有一个
            _luaEnv = LuaManager.LuaEnv;
            if (_luaEnv == null) return;

            _scriptEnv = _luaEnv.NewTable();
            LuaTable meta = _luaEnv.NewTable();
            meta.Set("__index", _luaEnv.Global);
            _scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            string prefabName = name;
            if (prefabName.Contains("(Clone)")) {
                prefabName = prefabName.Split(new string[] { "(Clone)" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
            }

            _onAwake = _scriptEnv.GetInPath<OnAwakeHandler>(prefabName + ".Awake");
            _onStart = _scriptEnv.GetInPath<OnStartHandler>(prefabName + ".Start");
            _onEnable = _scriptEnv.GetInPath<OnEnableHandler>(prefabName + ".OnEnable");
            _onDisable = _scriptEnv.GetInPath<OnDisableHandler>(prefabName + ".OnDisable");
            _onDestroy = _scriptEnv.GetInPath<OnDestroyHandler>(prefabName + ".OnDestroy");

            _scriptEnv.Set("self", this);
            if(_onAwake != null) {
                _onAwake(transform);
            }
        }

        /// <summary>
        /// 根据索引查找组件
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对应的组件</returns>
        public object GetLuaComp(int index) {
            if (_luaComps == null) return null;
            LuaComp comp = _luaComps[index];
            Transform compTran = comp.Tran;
            switch (comp.Type) {
                case LuaCompType.GameObject: return compTran.gameObject;
                case LuaCompType.Transform: return compTran;
                case LuaCompType.Button: return compTran.GetComponent<Button>();
                case LuaCompType.Image: return compTran.GetComponent<Image>();
                case LuaCompType.Text: return compTran.GetComponent<Text>();
                case LuaCompType.RawImage: return compTran.GetComponent<RawImage>();
                case LuaCompType.InputField: return compTran.GetComponent<InputField>();
                case LuaCompType.Scrollbar: return compTran.GetComponent<Scrollbar>();
                case LuaCompType.ScrollRect: return compTran.GetComponent<ScrollRect>();
                //case LuaCompType.UIMultiScroller: return compTran.GetComponent<UIMultiScroller>();
                //case LuaCompType.UIScroller: return compTran.GetComponent<UIScroller>();
            }
            return compTran;
        }

        protected override void Start() {
            base.Start();
            _onStart?.Invoke();
        }

        protected override void OnEnable() {
            base.OnEnable();
            _onEnable?.Invoke();
        }

        protected override void OnDisable() {
            base.OnDisable();
            _onDisable?.Invoke();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            _onDestroy?.Invoke();

            _onAwake = null;
            _onStart = null;
            _onEnable = null;
            _onDisable = null;
            _onDestroy = null;

            //卸载图片资源（不然图片资源的引用会一直存在内存里，没办法通过Resource.UnloadUnuseAsset去进行释放）
            if (_luaComps == null) return;
            int len = _luaComps.Length;
            for (int i = 0; i < len; i++) {
                var comp = _luaComps[i];
                switch (comp.Type) {
                    case LuaCompType.Button:
                    case LuaCompType.Image:
                        Image img = comp.Tran.GetComponent<Image>();
                        if (img != null) {
                            img.sprite = null;
                        }
                        break;
                    case LuaCompType.RawImage:
                        RawImage rawImage = comp.Tran.GetComponent<RawImage>();
                        if (rawImage != null)
                            rawImage.texture = null;
                        break;
                }
                comp.Tran = null;
                comp = null;
            }
        }

    }
}

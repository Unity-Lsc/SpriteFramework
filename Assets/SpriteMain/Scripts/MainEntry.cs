using SpriteFramework;
using System.Collections;
using UnityEngine;
using YooAsset;

namespace SpriteMain
{
    /// <summary>
    /// 项目启动(初始化资源加载,启动热更新,初始化框架)
    /// </summary>
    public class MainEntry : MonoBehaviour
    {

        public static MainEntry Instance { get; private set; }

        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

        public string RemoteURL = "http://127.0.0.1:6080";

        [SerializeField]
        private ParamsSettings m_ParamsSettings;
        /// <summary>
        /// 全局参数设置
        /// </summary>
        public static ParamsSettings ParamsSettings { get; private set; }

        private void Awake() {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            ParamsSettings = m_ParamsSettings;

            StartCoroutine(BootStartUp());
        }



        /// <summary>
        /// 项目启动入口
        /// </summary>
        IEnumerator BootStartUp() {

            //初始化YooAsset
            YooAssets.Initialize();
            YooAssets.SetOperationSystemMaxTimeSlice(30);
            //end

            //检查热更新
            yield return CheckHotUpdate();
            //end

            //框架初始化
            yield return InitFramework();
            //end
        }

        /// <summary>
        /// 检查热更新
        /// </summary>
        IEnumerator CheckHotUpdate() {
            PatchManager.Instance.Init(PlayMode, RemoteURL);
            yield return PatchManager.Instance.GameHotUpdate();
        }

        /// <summary>
        /// 框架初始化
        /// </summary>
        IEnumerator InitFramework() {
            var obj = Resources.Load<GameObject>("GameEntry");
            Instantiate(obj, transform).GetComponent<GameEntry>().Init();
            yield break;
        }

    }
}

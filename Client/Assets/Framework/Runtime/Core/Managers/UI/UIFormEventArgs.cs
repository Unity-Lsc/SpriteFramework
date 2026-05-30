using System;

namespace SpriteFramework
{
    /// <summary>
    /// UI打开成功事件参数
    /// </summary>
    public sealed class OpenUIFormSuccessEventArgs : EventArgs
    {
        public int SerialId;
        public string AssetFullPath;
        public UIFormBase UIForm;
        public object UserData;
    }

    /// <summary>
    /// UI打开失败事件参数
    /// </summary>
    public sealed class OpenUIFormFailureEventArgs : EventArgs
    {
        public int SerialId;
        public string AssetFullPath;
        public string ErrorMessage;
        public object UserData;
    }

    /// <summary>
    /// UI关闭完成事件参数
    /// </summary>
    public sealed class CloseUIFormCompleteEventArgs : EventArgs
    {
        public int SerialId;
        public string AssetFullPath;
        public UIFormBase UIForm;
        public object UserData;
    }

}

using System;

/// <summary>
/// 自定义控件配置 Model
/// </summary>
public class Widget {
    /// <summary>
    /// 控件唯一 ID，相同 ID 的不同实例添加后，后添加的控件会替换之前添加的控件
    /// </summary>
    public string viewId;
    /// <summary>
    /// 控件类型，当前仅支持 View/ImageView/TextView
    /// </summary>
    public string type;
    /// <summary>
    /// 控件坐标参数 相对布局左边距
    /// </summary>
    public int left = -1;
    /// <summary>
    /// 控件坐标参数 相对布局上边距
    /// </summary>
    public int top = -1;
    /// <summary>
    /// 控件坐标参数 相对布局右边距
    /// </summary>
    public int right = -1;
    /// <summary>
    /// 控件坐标参数 相对布局下边距
    /// </summary>
    public int bottom = -1;
    /// <summary>
    /// 控件宽度
    /// </summary>
    public int width = -1;
    /// <summary>
    /// 控件高度
    /// </summary>
    public int height = -1;
    /// <summary>
    /// 控件是否可点击，默认可点击，点击事件回调参考 OneLoginPluginCallback.onCustomViewClick 方法
    /// </summary>
    public bool clickable = true;
    /// <summary>
    /// 控件文本，仅对 type 为 TextView 的控件有效
    /// </summary>
    public string text;
    /// <summary>
    /// 控件文本字体大小，仅对 type 为 TextView 的控件有效
    /// </summary>
    public int textSize = -1;
    /// <summary>
    /// 控件文本字体颜色，仅对 type 为 TextView 的控件有效
    /// </summary>
    public uint textColor;
    /// <summary>
    /// 控件背景颜色
    /// </summary>
    public uint backgroundColor;
    /// <summary>
    /// 控件背景图片，仅对 type 为 TextView 和 ImageView 的控件有效
    /// 当前仅支持配置在 drawable 目录下的资源有效
    /// 可放置于 Assets\Plugins\Android\res\drawable 目录，但官方已不推荐该方式
    /// 建议二次封装 SDK unity 插件，增加资源文件 
    /// </summary>
    public string backgroundImgPath;
}

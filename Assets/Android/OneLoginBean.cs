using System;

/// <summary>
/// 一键登录授权页样式配置 Model，参数跟原生 SDK 保持一致
/// 由于 unity 不支持 Android 的原生 Typeface 字体类型，由分别增加 Name/Bold/Italic 后缀的三个字段替代，插件 SDK 内自动转换为原生支持的 Typeface 类型
/// </summary>
public class OneLoginBean  {
    /**
     * 状态栏颜色
     */
    public uint statusBarColor = 0;
    /**
     * 状态栏字体颜色 只能是黑白
     */
    public bool isLightColor = false;
    /**
     * 底部导航栏颜色
     */
    public uint navigationBarColor = 0;

    /**
     * 标题栏高度
     */
    public int authNavHeight = 49;
    /**
     * 标题栏是否去掉
     */
    public bool authNavGone = false;
    /**
     * 设置标题栏是否透明
     */
    public bool authNavTransparent = true;

    /**
     * 标题栏颜色
     */
    public uint navColor = 0xFF3973FF;
    /**
     * 标题栏标题文字
     */
    public string navText = "一键登录";
    /**
     * 授权页标题栏文字颜色
     */
    public uint navTextColor = 0xFFFFFFFF;
    /**
     * 授权页标题栏文字字体大小
     */
    public int navTextSize = 17;
    /**
     * 授权页标题栏文字字体样式
     */
    public string navTextTypefaceName = "";
    /**
     * 授权页标题栏文字字体是否加粗
     */
    public bool navTextTypefaceBold = false;
    /**
     * 授权页标题栏文字字体是否倾斜
     */
    public bool navTextTypefaceItalic = false;

    /**
     * 是否使用默认隐私条款页，默认是，否则需要用户自定义隐私条款页
     */
    public bool isUseNormalWebActivity = true;
    /**
     * 隐私条款页是否使用默认标题
     */
    public bool navWebTextNormal = false;
    /**
     * 隐私条款页默认标题
     */
    public string navWebText = "服务条款";
    /**
     * 隐私条款页标题栏文字颜色
     */
    public uint navWebTextColor = 0xFF000000;
    /**
     * 隐私条款页标题栏文字大小
     */
    public int navWebTextSize = 17;
    /**
     * 隐私条款页标题栏文字字体样式
     */
    public string navWebTextTypefaceName = "";
    /**
     * 隐私条款页标题栏文字字体是否加粗
     */
    public bool navWebTextTypefaceBold = false;
    /**
     * 隐私条款页标题栏文字字体是否倾斜
     */
    public bool navWebTextTypefaceItalic = false;

    /**
     * 标题栏返回图标
     */
    public string returnImgPath = "gt_one_login_ic_chevron_left_black";
    /**
     * 导航返回图片的宽度
     */
    public int returnImgWidth = 24;

    /**
     * 导航返回图片的高度
     */
    public int returnImgHeight = 24;
    /**
     * 返回按钮相对于x轴的偏移
     */
    public int returnImgOffsetX = 12;

    /**
     * 返回按钮是否垂直居中显示
     */
    public bool returnImgCenterInVertical = true;

    /**
     * 返回按钮相对于y轴的偏移
     */
    public int returnImgOffsetY = 0;
    /**
     * 设置返回图片是否隐藏
     */
    public bool returnImgHidden = false;

    /**
     * 授权页背景
     */
    public string authBGImgPath = "gt_one_login_bg";
    /**
     * 授权页视频背景
     */
    public string authBgVideoUri = null;


    /**
     * 是否使用弹窗模式
     */
    public bool isDialogTheme = false;
    /**
     * 设置协议条款页面是否开启弹窗
     */
    public bool isWebViewDialogTheme = false;

    /**
     * 授权页弹窗宽度
     */
    public int dialogWidth = 300;

    /**
     * 授权页弹窗高度
     */
    public int dialogHeight = 500;

    /**
     * 授权页弹窗X偏移量（以屏幕中 ⼼为原点）
     */
    public int dialogX = 0;

    /**
     * 授权页弹窗Y偏移量（以屏幕中 ⼼为原点）
     */
    public int dialogY = 0;

    /**
     * 授权页弹窗是否贴于屏幕底部： true：显示到屏幕底部， dialogY参数设置将⽆效 false：不显示到屏幕底部，以 dialogY参数为准
     */
    public bool isDialogBottom = false;

    /**
     * logo图片
     */
    public string logoImgPath = "gt_one_login_logo";
    /**
     * logo宽度
     */
    public int logoWidth = 71;
    /**
     * logo高度
     */
    public int logoHeight = 71;
    /**
     * 是否隐藏logo
     */
    public bool logoHidden = false;
    /**
     * logo x轴偏移量
     */
    public int logoOffsetX = 0;
    /**
     * 设置logo相对于标题栏下边缘y偏移
     */
    public int logoOffsetY = 125;
    /**
     * 设置logo相对于底部y偏移
     */
    public int logoOffsetY_B = 0;


    /**
     * 设置号码字体颜色
     */
    public uint numberColor = 0xFF3D424C;
    /**
     * 号码栏字体大小
     */
    public int numberSize = 24;
    /**
     * 号码栏字体样式
     */
    public string numberTypefaceName = "";
    /**
     * 号码栏字体是否加粗
     */
    public bool numberTypefaceBold = false;
    /**
     * 号码栏字体是否倾斜
     */
    public bool numberTypefaceItalic = false;

    /**
     * 号码栏偏移量
     */
    public int numberOffsetX = 0;
    /**
     * 号码栏相对于标题栏下边缘y偏移
     */
    public int numberOffsetY = 200;
    /**
     * 号码栏相对于底部y偏移
     */
    public int numberOffsetY_B = 0;

    /**
     * slogan文字颜色
     */
    public uint sloganColor = 0xFFA8A8A8;
    /**
     * slogan的字体大小
     */
    public int sloganSize = 10;
    /**
     * slogan的字体样式
     */
    public string sloganTypefaceName = "";
    /**
     * slogan的字体是否加粗
     */
    public bool sloganTypefaceBold = false;
    /**
     * slogan的字体是否倾斜
     */
    public bool sloganTypefaceItalic = false;

    /**
     * slogin的x轴偏移
     */
    public int sloganOffsetX = 0;
    /**
     * slogan相对于标题栏下边缘y偏移
     */
    public int sloganOffsetY = 382;

    /**
     * slogan相对于底部y偏移
     */
    public int sloganOffsetY_B = 0;

    /**
     * 登录按钮文字
     */
    public string logBtnText = "一键登录";
    /**
     * 登录按钮宽度
     */
    public int logBtnWidth = 268;

    /**
     * 登录按钮高度
     */
    public int logBtnHeight = 36;
    /**
     * 登录按钮文字颜色
     */
    public uint logBtnColor = 0xFFFFFFFF;
    /**
     * 登录按钮字体大小
     */
    public int logBtnTextSize = 15;
    /**
     * 登录按钮字体样式
     */
    public string logBtnTextTypefaceName = "";
    /**
     * 登录按钮字体是否加粗
     */
    public bool logBtnTextTypefaceBold = false;
    /**
     * 登录按钮字体是否倾斜
     */
    public bool logBtnTextTypefaceItalic = false;

    /**
     * 登录按钮背景图片
     */
    public string logBtnImgPath = "gt_one_login_btn_normal";
    /**
     * 登录按钮X轴偏移
     */
    public int logBtnOffsetX = 0;
    /**
     * 登录按钮相对于标题栏下边缘y偏移
     */
    public int logBtnOffsetY = 324;
    /**
     * 登录按钮相对于底部y偏移
     */
    public int logBtnOffsetY_B = 0;
    /**
     * 未勾选复选框时登录按钮是否可用
     */
    public bool disableBtnIfUnChecked = false;


    /**
     * loading图片地址
     */
    public string loadingView = "umcsdk_load_dot_white";
    /**
     * loading图片宽度
     */
    public int loadingViewWidth = 20;
    /**
     * loading图片高度
     */
    public int loadingViewHeight = 20;
    /**
     * loading对右边的偏移
     */
    public int loadingViewOffsetRight = 12;
    /**
     * loading默认居中
     */
    public bool loadingViewCenterInVertical = true;
    /**
     * loading Y轴偏移
     */
    public int loadingViewOffsetY = 0;

    /**
     * 切换账号文字
     */
    public string switchText = "切换账号";
    /**
     * 切换账号大小
     */
    public int switchSize = 14;
    /**
     * 切换账号字体颜色
     */
    public uint switchColor = 0xFF3973FF;
    /**
     * 切换账号字体样式
     */
    public string switchTypefaceName = "";
    /**
     * 切换账号字体是否加粗
     */
    public bool switchTypefaceBold = false;
    /**
     * 切换账号字体是否倾斜
     */
    public bool switchTypefaceItalic = false;

    /**
     * 切换账号是否隐藏
     */
    public bool switchHidden = false;

    /**
     * 切换账号x轴偏移
     */
    public int switchOffsetX = 0;

    /**
     * 切换账号相对于标题栏下边缘y偏移
     */
    public int switchOffsetY = 249;

    /**
     * 切换账号相对于底部y偏移
     */
    public int switchOffsetY_B = 0;

    /**
     * 切换帐号背景图片路径
     */
    public string switchImgPath = "";

    /**
     * 切换帐号背景宽
     */
    public int switchWidth = 80;

    /**
     * 切换帐号背景高
     */
    public int switchHeight = 25;

    /**
     * 隐私协议X轴
     */
    public int privacyOffsetX = 0;
    /**
     * 隐私条款相对于标题栏下边缘y偏移
     */
    public int privacyOffsetY = 0;

    /**
     * 隐私条款相对于底部y偏移
     */
    public int privacyOffsetY_B = 18;
    /**
     * 隐私协议宽度
     */
    public int privacyLayoutWidth = 256;

    /**
     * 隐私条款连接字符1
     */
    public string privacyTextViewTv1 = "登录即同意";
    /**
     * 隐私条款连接字符2
     */
    public string privacyTextViewTv2 = "和";
    /**
     * 隐私条款连接字符3
     */
    public string privacyTextViewTv3 = "、";
    /**
     * 隐私条款连接字符4
     */
    public string privacyTextViewTv4 = "并使用本机号码登录";

    /**
     * 多个开发者隐私条款
     * 按顺序设置，长度为 4 的倍数，配置后优先使用该接口的配置
     */
    public string[] privacyClauseTextStrings;
    /**
     * 开发者隐私条款1
     */
    public string clauseNameOne = "";
    /**
     * 开发者隐私URL1
     */
    public string clauseUrlOne = "";
    /**
     * 开发者隐私条款2
     */
    public string clauseNameTwo = "";
    /**
     * 开发者隐私URL2
     */
    public string clauseUrlTwo = "";
    /**
     * 开发者隐私条款3
     */
    public string clauseNameThree = "";
    /**
     * 开发者隐私URL3
     */
    public string clauseUrlThree = "";

    /**
     * 基础协议颜色
     */
    public uint baseClauseColor = 0xFFA8A8A8;
    /**
     * 协议颜色
     */
    public uint clauseColor = 0xFF3973FF;
    /**
     * 协议字体大小
     */
    public int privacyClauseTextSize = 10;
    /**
     * 协议栏基础文字字体样式
     */
    public string privacyClauseBaseTypefaceName = "";
    /**
     * 协议栏基础文字字体是否加粗
     */
    public bool privacyClauseBaseTypefaceBold = false;
    /**
     * 协议栏基础文字字体是否倾斜
     */
    public bool privacyClauseBaseTypefaceItalic = false;
    /**
     * 协议栏条款文字字体样式
     */
    public string privacyClauseTypefaceName = "";
    /**
     * 协议栏条款文字字体是否加粗
     */
    public bool privacyClauseTypefaceBold = false;
    /**
     * 协议栏条款文字字体是否倾斜
     */
    public bool privacyClauseTypefaceItalic = false;

    /**
     * 复选框选中图片
     */
    public string checkedImgPath = "gt_one_login_checked";
    /**
     * 复选框未选中图片
     */
    public string unCheckedImgPath = "gt_one_login_unchecked";
    /**
     * 隐私条款check框默认状态
     */
    public bool privacyState = true;
    /**
     * 隐私条款check框未选择时点击一键登录按钮提示文字
     */
    public string privacyUnCheckedToastText = "请同意服务条款";
    /**
     * 复选框宽度
     */
    public int privacyCheckBoxWidth = 9;
    /**
     * 复选框高度
     */
    public int privacyCheckBoxHeight = 9;
    /**
     * 复选框Y轴偏移
     */
    public int privacyCheckBoxOffsetY = 2;

    /**
     * 隐私条款是否增加书名号显示
     */
    public bool privacyAddFrenchQuotes = false;
    /**
     * 隐私协议文字对齐方式
     */
    public int privacyTextGravity = 48 | 8388611;// Gravity.TOP | Gravity.START;
}
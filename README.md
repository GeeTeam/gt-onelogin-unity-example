# Unity 插件集成文档

# 创建应用

登录极验后台创建应用获取 appId 和 key，具体步骤可参照 [账号创建](https://docs.geetest.com/onelogin/overview/account)

# Android


## 前置条件

- 极验 SDK 支持 Android Studio 2.1.3 及以上版本，Android 5.0及以上版本
- 极验 SDK uni-app 插件支持 HBuildX 2.7.5 及以上版本
- 极验 SDK 支持中国移动 4G/3G/2G、联通 4G/3G、电信4G（2G/3G 网络下时延相对较高，成功率相对较低） 的取号能力
- 极验 SDK 支持网络环境为：

1. 纯数据网络
2. 数据网络与 Wifi 网络同时开启

- 对于双卡手机，极验 SDK 取当前流量卡号

## 一键登录 API 使用说明

### 引用方式

```cs
AndroidJavaClass m_Jc = new AndroidJavaClass("com.geetest.onelogin.OneLoginManager");
AndroidJavaObject olManager = m_Jc.CallStatic<AndroidJavaObject>("with", new object[0]);
```

### 初始化

**方法描述**

SDK 初始化并预取号。

**代码示例**

```cs
olManager.Call("setLogEnabled", true);//打开日志开关
olManager.Call("register", "您在极验后台创建应用时生成的 appid", 8000, new GOLMethodCallback(this));
```

**参数说明**

参数|类型|说明
---|---|---
appid|String|极验后台配置唯一产品 APPID，请在官网申请
timeout|int|超时时间，单位:`ms`，取值范围:`1000~15000`，默认`5000`
registerListener|MethodCallback|初始化方法回调, 继承自 AndroidJavaProxy，对应插件 com.geetest.onelogin.callback.MethodCallback 类的回调

### 拉起授权页

- 拉起授权页`requestToken`

**方法描述**

在需要登录的地方调用`requestToken`接口拉起一键登录授权页，待用户点一键登录授权后获取运营商`token`，获取成功后即可请求服务端换取本机手机号码。（调用该方法前可以调用`isPreGetTokenResultValidate`判断预取号是否成功）建议在调用该方法前，提前一段时间进行预取号。请勿频繁的多次调用。

**代码示例**

```cs
/**
 * 授权页主题样式
 * 0:浮窗式
 * 1:弹窗式
 * 2:沉浸式
 * 3:横屏
 */
int themeStyle = 2;
// 授权页主题配置
OneLoginBean oneLoginBean = getThemeConfig(themeStyle);
string configStr = JsonUtility.ToJson(oneLoginBean, true);

// 授权页自定义控件配置
// 自定义控件的点击事件回调参考 OneLoginPluginCallback.onCustomViewClick 方法
Widget[] widgets = getWidgets();
int len = widgets.Length;
string[] widgetStrs = new string[len];
for (int i = 0; i < len; i++) {
    widgetStrs[i] = JsonUtility.ToJson(widgets[i], true);
}
// SDK 插件仅支持 json 格式授权页配置参数，调用前将 json 转换为 string 格式
olManager.Call("requestToken", configStr, widgetStrs, new OneLoginPluginCallback(this));
```


**参数说明**

- 接口参数说明：

授权页 UI 配置`string themeJsonString`与自定义 UI 配置`string[] customJsonString`，详细配置说明请参考「授权页 UI 配置」章节。

- 回调接口描述：

回调|含义|可用参数
---|---|---
onResult | 一键登录成功、失败、拉起授权页失败、授权页返回、切换账号等回调| json 格式返回结果，`status` 返回码
onPrivacyClick | 隐私条款点击 | `name`: 隐私条款名称<br>`url`: 隐私条款地址
onPrivacyCheckBoxClick | 授权页选择框 CheckBox 点击回调 | `isChecked`：CheckBox 是否勾选
onLoginButtonClick | 登录按钮点击回调 |
onSwitchButtonClick | 切换账号点击回调 |
onBackButtonClick | 返回按钮/返回键点击回调 |
onLoginLoading | 点一键登录时弹出自定义对话框式 loading 时机 |
onAuthActivityCreate | 授权页面拉起回调 |
onAuthWebActivityCreate | 隐私条款页面拉起回调 |
onRequestTokenSecurityPhone | 获取用于界面展示的脱敏手机号 | `phone`：脱敏手机号
自定义控件回调 | 自定义 UI 控件点击事件回调 | `viewId`：自定义控件 Id

- onResult status 返回码说明

返回信息包含一键登录成功、失败、拉起授权页失败、授权页返回、切换账号等回调信息，具体请参考官网一键登录[返回码](https://docs.geetest.com/onelogin/deploy/android#1%E3%80%81OneLogin%EF%BC%88%E4%B8%80%E9%94%AE%E7%99%BB%E5%BD%95%EF%BC%89)说明文档。当返回码为 200 时，返回信息包含一键登录的 token 信息，请将这些参数传递给后端开发人员，并参考「[服务端](https://docs.geetest.com/onelogin/deploy/server)」文档来实现获取手机号码的步骤。

### 销毁授权页

- 关闭授权页`dismissAuthActivity`

**方法描述**

用户主动关闭授权页。原生 SDK 除了返回按钮触发关闭以外，默认是不 finsih 授权页的，需要开发者**在回调结束后自行实现关闭授权页**。

**代码示例**

```cs
olManager.Call("dismissAuthActivity");
```


### 授权页 UI 配置

1. 授权页 UI 配置

配置该参数可实现对一键登录授权页进行个性化配置，每次调用拉起授权页方法时都需要传入该参数，否则授权页界面将按照默认配置进行展示。详细配置说明请参考「授权页 UI 配置」章节。

**代码示例**

```cs
// 设备屏幕信息
ScreenInfo screenInfo = getScreenInfo();
float density = screenInfo.density;
int width = (int)(screenInfo.screenWidth / density);
int height = (int)(screenInfo.screenHeight / density);
int popWidth = (int)(width * 4 / 5);
int popHeight = (int)(height * 3 / 5);

// 授权页配置 Model
OneLoginBean olb = new OneLoginBean();
switch (themeStyle) {
    case 0://浮窗式
        olb.isDialogTheme = true; olb.dialogWidth = width; olb.dialogHeight = 500; olb.dialogX = 0; olb.dialogY = 0; olb.isDialogBottom = true; olb.isWebViewDialogTheme = true;
        olb.authBGImgPath = "gt_one_login_bg";
        olb.statusBarColor = 0; olb.navigationBarColor = 0; olb.isLightColor = false;
        olb.navColor = 0xFF3973FF; olb.authNavHeight = 49; olb.authNavTransparent = true; olb.authNavGone = false;
        olb.navText = "一键登录"; olb.navTextColor = 0xFFFFFFFF; olb.navTextSize = 17; olb.navWebTextNormal = false; olb.navWebText = "服务条款"; olb.navWebTextColor = 0xFF000000; olb.navWebTextSize = 17;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 40; olb.returnImgHeight = 40; olb.returnImgHidden = false; olb.returnImgOffsetX = 8;
        olb.logoImgPath = "gt_one_login_logo"; olb.logoWidth = 71; olb.logoHeight = 71; olb.logoHidden = false; olb.logoOffsetY = 100; olb.logoOffsetY_B = 0; olb.logoOffsetX = 0;
        olb.sloganColor = 0xFFA8A8A8; olb.sloganSize = 10; olb.sloganOffsetY = 330; olb.sloganOffsetY_B = 0; olb.sloganOffsetX = 0;
        olb.numberColor = 0xFF3D424C; olb.numberSize = 24; olb.numberOffsetY = 160; olb.numberOffsetY_B = 0; olb.numberOffsetX = 0;
        olb.switchText = "切换账号"; olb.switchColor = 0xFF3973FF; olb.switchSize = 14; olb.switchHidden = false; olb.switchOffsetY = 230; olb.switchOffsetY_B = 0; olb.switchOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn_normal"; olb.logBtnWidth = 290; olb.logBtnHeight = 45; olb.logBtnOffsetY = 270; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 18;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.unCheckedImgPath = "gt_one_login_unchecked"; olb.checkedImgPath = "gt_one_login_checked"; olb.privacyState = true; olb.privacyCheckBoxWidth = 12; olb.privacyCheckBoxHeight = 12;
        olb.privacyLayoutWidth = 256; olb.privacyOffsetY = 0; olb.privacyOffsetY_B = 18; olb.privacyOffsetX = 0; olb.isUseNormalWebActivity = true;
        olb.baseClauseColor = 0xFFA8A8A8; olb.clauseColor = 0xFF3973FF; olb.privacyClauseTextSize = 10;
        olb.privacyTextViewTv1 = "登录即同意"; olb.privacyTextViewTv2 = "和"; olb.privacyTextViewTv3 = "、"; olb.privacyTextViewTv4 = "并使用本机号码登录";
        break;
    case 1://弹窗式
        olb.isDialogTheme = true; olb.dialogWidth = popWidth; olb.dialogHeight = popHeight; olb.dialogX = 0; olb.dialogY = 0; olb.isDialogBottom = false; olb.isWebViewDialogTheme = true;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 40; olb.returnImgHeight = 40; olb.returnImgHidden = false; olb.returnImgOffsetX = 8;
        olb.logoImgPath = "gt_one_login_logo"; olb.logoWidth = 71; olb.logoHeight = 71; olb.logoHidden = false; olb.logoOffsetY = 60; olb.logoOffsetY_B = 0; olb.logoOffsetX = 0;
        olb.sloganColor = 0xFFA8A8A8; olb.sloganSize = 10; olb.sloganOffsetY = 270; olb.sloganOffsetY_B = 0; olb.sloganOffsetX = 0;
        olb.numberColor = 0xFF3D424C; olb.numberSize = 24; olb.numberOffsetY = 125; olb.numberOffsetY_B = 0; olb.numberOffsetX = 0;
        olb.switchText = "切换账号"; olb.switchColor = 0xFF3973FF; olb.switchSize = 14; olb.switchHidden = false; olb.switchOffsetY = 165; olb.switchOffsetY_B = 0; olb.switchOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn_normal"; olb.logBtnWidth = 268; olb.logBtnHeight = 45; olb.logBtnOffsetY = 220; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 18;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.unCheckedImgPath = "gt_one_login_unchecked"; olb.checkedImgPath = "gt_one_login_checked"; olb.privacyState = true; olb.privacyCheckBoxWidth = 12; olb.privacyCheckBoxHeight = 12;
        olb.privacyLayoutWidth = 256; olb.privacyOffsetY = 0; olb.privacyOffsetY_B = 1; olb.privacyOffsetX = 0; olb.isUseNormalWebActivity = true;
        break;
    case 2://沉浸式
        olb.statusBarColor = 0xFFFFFFFF; olb.navigationBarColor = 0xFFFFFFFF; olb.isLightColor = true;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 40; olb.returnImgHeight = 40; olb.returnImgHidden = false; olb.returnImgOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn_normal"; olb.logBtnWidth = 290; olb.logBtnHeight = 45; olb.logBtnOffsetY = 310; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 18;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.clauseNameOne = "自定义服务条款1"; olb.clauseUrlOne = "https=//docs.geetest.com/onelogin/deploy/android"; olb.clauseNameTwo = "自定义服务条款2"; olb.clauseUrlTwo = "https=//docs.geetest.com/onelogin/changelog/android"; olb.clauseNameThree = ""; olb.clauseUrlThree = "";
        olb.privacyClauseTextStrings = new string[]{
            "登录即同意", "应用自定义服务条款一", "https=//docs.geetest.com/onelogin/deploy/android", "",
            "和", "应用自定义服务条款二", "https=//docs.geetest.com/onelogin/changelog/android", "",
            "和", "应用自定义服务条款三", "https=//docs.geetest.com/onelogin/help/tech", "",
            "", "", "", ""};
        break;
    case 3://横屏
        olb.authBGImgPath = "gt_one_login_bg";
        olb.isDialogTheme = false; olb.dialogWidth = 300; olb.dialogHeight = 500; olb.dialogX = 0; olb.dialogY = 0; olb.isDialogBottom = false; olb.isWebViewDialogTheme = false;
        olb.statusBarColor = 0xffffffff; olb.navigationBarColor = 0xffffffff; olb.isLightColor = true;
        olb.navColor = 0xFF3973FF; olb.authNavHeight = 49; olb.authNavTransparent = true; olb.authNavGone = false;
        olb.navText = "一键登录"; olb.navTextColor = 0xFFFFFFFF; olb.navTextSize = 17; olb.navWebTextNormal = false; olb.navWebText = "服务条款"; olb.navWebTextColor = 0xFF000000; olb.navWebTextSize = 17;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 45; olb.returnImgHeight = 45; olb.returnImgHidden = false; olb.returnImgOffsetX = 8;
        olb.logoImgPath = "gt_one_login_logo"; olb.logoWidth = 71; olb.logoHeight = 71; olb.logoHidden = false; olb.logoOffsetY = 55; olb.logoOffsetY_B = 0; olb.logoOffsetX = 0;
        olb.sloganColor = 0xFFA8A8A8; olb.sloganSize = 10; olb.sloganOffsetY = 226; olb.sloganOffsetY_B = 0; olb.sloganOffsetX = 0;
        olb.numberColor = 0xFF3D424C; olb.numberSize = 24; olb.numberOffsetY = 84; olb.numberOffsetY_B = 0; olb.numberOffsetX = 0;
        olb.switchText = "切换账号"; olb.switchColor = 0xFF3973FF; olb.switchSize = 14; olb.switchHidden = false; olb.switchOffsetY = 128; olb.switchOffsetY_B = 0; olb.switchOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn_normal"; olb.logBtnWidth = 268; olb.logBtnHeight = 36; olb.logBtnOffsetY = 169; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 15;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.unCheckedImgPath = "gt_one_login_unchecked"; olb.checkedImgPath = "gt_one_login_checked"; olb.privacyState = true; olb.privacyCheckBoxWidth = 9; olb.privacyCheckBoxHeight = 9;
        olb.privacyLayoutWidth = 512; olb.privacyOffsetY = 0; olb.privacyOffsetY_B = 1; olb.privacyOffsetX = 0; olb.isUseNormalWebActivity = true;
        olb.baseClauseColor = 0xFFA8A8A8; olb.clauseColor = 0xFF3973FF; olb.privacyClauseTextSize = 10;
        olb.privacyTextViewTv1 = "登录即同意"; olb.privacyTextViewTv2 = "和"; olb.privacyTextViewTv3 = "、"; olb.privacyTextViewTv4 = "并使用本机号码登录";
        break;
}
return olb;
```

授权页所有 UI 配置 OneLoginBean.cs 与插件内的 OneLoginBean.java 类关联，该类所有字段说明：

```java
/**
 * 状态栏颜色
 */
public int statusBarColor = 0;
/**
 * 状态栏字体颜色 只能是黑白
 */
public boolean isLightColor = false;
/**
 * 底部导航栏颜色
 */
public int navigationBarColor = 0;

/**
 * 标题栏高度
 */
public int authNavHeight = 49;
/**
 * 标题栏是否去掉
 */
public boolean authNavGone = false;
/**
 * 设置标题栏是否透明
 */
public boolean authNavTransparent = true;

/**
 * 标题栏颜色
 */
public int navColor = 0xFF3973FF;
/**
 * 标题栏标题文字
 */
public String navText = "一键登录";
/**
 * 授权页标题栏文字颜色
 */
public int navTextColor = 0xFFFFFFFF;
/**
 * 授权页标题栏文字字体大小
 */
public int navTextSize = 17;
/**
 * 授权页标题栏文字字体样式
 */
public String navTextTypefaceName = "";
/**
 * 授权页标题栏文字字体是否加粗
 */
public boolean navTextTypefaceBold = false;
/**
 * 授权页标题栏文字字体是否倾斜
 */
public boolean navTextTypefaceItalic = false;

/**
 * 是否使用默认隐私条款页，默认是，否则需要用户自定义隐私条款页
 */
public boolean isUseNormalWebActivity = true;
/**
 * 隐私条款页是否使用默认标题
 */
public boolean navWebTextNormal = false;
/**
 * 隐私条款页默认标题
 */
public String navWebText = "服务条款";
/**
 * 隐私条款页标题栏文字颜色
 */
public int navWebTextColor = 0xFF000000;
/**
 * 隐私条款页标题栏文字大小
 */
public int navWebTextSize = 17;
/**
 * 隐私条款页标题栏文字字体样式
 */
public String navWebTextTypefaceName = "";
/**
 * 隐私条款页标题栏文字字体是否加粗
 */
public boolean navWebTextTypefaceBold = false;
/**
 * 隐私条款页标题栏文字字体是否倾斜
 */
public boolean navWebTextTypefaceItalic = false;

/**
 * 标题栏返回图标
 */
public String returnImgPath = "gt_one_login_ic_chevron_left_black";
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
 * 返回按钮是否垂直居中显示，配置垂直居中后y轴偏移配置失效
 */
public boolean returnImgCenterInVertical = true;

/**
 * 返回按钮相对于y轴的偏移
 */
public int returnImgOffsetY = 0;
/**
 * 设置返回图片是否隐藏
 */
public boolean returnImgHidden = false;

/**
 * 授权页背景
 */
public String authBGImgPath = "gt_one_login_bg";
/**
 * 授权页视频背景
 */
public String authBgVideoUri = null;

/**
 * 是否使用弹窗模式
 */
public boolean isDialogTheme = false;
/**
 * 设置协议条款页面是否开启弹窗
 */
public boolean isWebViewDialogTheme = false;

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
public boolean isDialogBottom = false;

/**
 * logo图片
 */
public String logoImgPath = "gt_one_login_logo";
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
public boolean logoHidden = false;
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
public int numberColor = 0xFF3D424C;
/**
 * 号码栏字体大小
 */
public int numberSize = 24;
/**
 * 号码栏字体样式
 */
public String numberTypefaceName = "";
/**
 * 号码栏字体是否加粗
 */
public boolean numberTypefaceBold = false;
/**
 * 号码栏字体是否倾斜
 */
public boolean numberTypefaceItalic = false;

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
public int sloganColor = 0xFFA8A8A8;
/**
 * slogan的字体大小
 */
public int sloganSize = 10;
/**
 * slogan的字体样式
 */
public String sloganTypefaceName = "";
/**
 * slogan的字体是否加粗
 */
public boolean sloganTypefaceBold = false;
/**
 * slogan的字体是否倾斜
 */
public boolean sloganTypefaceItalic = false;

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
public String logBtnText = "一键登录";
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
public int logBtnColor = 0xFFFFFFFF;
/**
 * 登录按钮字体大小
 */
public int logBtnTextSize = 15;
/**
 * 登录按钮字体样式
 */
public String logBtnTextTypefaceName = "";
/**
 * 登录按钮字体是否加粗
 */
public boolean logBtnTextTypefaceBold = false;
/**
 * 登录按钮字体是否倾斜
 */
public boolean logBtnTextTypefaceItalic = false;

/**
 * 登录按钮背景图片
 */
public String logBtnImgPath = "gt_one_login_btn_normal";
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
public boolean disableBtnIfUnChecked = false;


/**
 * loading图片地址
 */
public String loadingView = "umcsdk_load_dot_white";
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
 * loading默认居中，配置垂直居中后y轴偏移配置失效
 */
public boolean loadingViewCenterInVertical = true;
/**
 * loading Y轴偏移
 */
public int loadingViewOffsetY = 0;

/**
 * 切换账号文字
 */
public String switchText = "切换账号";
/**
 * 切换账号大小
 */
public int switchSize = 14;
/**
 * 切换账号字体颜色
 */
public int switchColor = 0xFF3973FF;
/**
 * 切换账号字体样式
 */
public String switchTypefaceName = "";
/**
 * 切换账号字体是否加粗
 */
public boolean switchTypefaceBold = false;
/**
 * 切换账号字体是否倾斜
 */
public boolean switchTypefaceItalic = false;
/**
 * 切换账号是否隐藏
 */
public boolean switchHidden = false;
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
public String switchImgPath = "";
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
public String privacyTextViewTv1 = "登录即同意";
/**
 * 隐私条款连接字符2
 */
public String privacyTextViewTv2 = "和";
/**
 * 隐私条款连接字符3
 */
public String privacyTextViewTv3 = "、";
/**
 * 隐私条款连接字符4
 */
public String privacyTextViewTv4 = "并使用本机号码登录";

/**
 * 多个开发者隐私条款
 * 按顺序设置，长度为 4 的倍数，配置后优先使用该接口的配置
 */
public String[] privacyClauseTextStrings;
/**
 * 开发者隐私条款1
 */
public String clauseNameOne = "";
/**
 * 开发者隐私URL1
 */
public String clauseUrlOne = "";
/**
 * 开发者隐私条款2
 */
public String clauseNameTwo = "";
/**
 * 开发者隐私URL2
 */
public String clauseUrlTwo = "";
/**
 * 开发者隐私条款3
 */
public String clauseNameThree = "";
/**
 * 开发者隐私URL3
 */
public String clauseUrlThree = "";

/**
 * 基础协议颜色
 */
public int baseClauseColor = 0xFFA8A8A8;
/**
 * 协议颜色
 */
public int clauseColor = 0xFF3973FF;
/**
 * 协议字体大小
 */
public int privacyClauseTextSize = 10;
/**
 * 协议栏基础文字字体样式
 */
public String privacyClauseBaseTypefaceName = "";
/**
 * 协议栏基础文字字体是否加粗
 */
public boolean privacyClauseBaseTypefaceBold = false;
/**
 * 协议栏基础文字字体是否倾斜
 */
public boolean privacyClauseBaseTypefaceItalic = false;
/**
 * 协议栏条款文字字体样式
 */
public String privacyClauseTypefaceName = "";
/**
 * 协议栏条款文字字体是否加粗
 */
public boolean privacyClauseTypefaceBold = false;
/**
 * 协议栏条款文字字体是否倾斜
 */
public boolean privacyClauseTypefaceItalic = false;

/**
 * 复选框选中图片
 */
public String checkedImgPath = "gt_one_login_checked";
/**
 * 复选框未选中图片
 */
public String unCheckedImgPath = "gt_one_login_unchecked";
/**
 * 隐私条款check框默认状态
 */
public boolean privacyState = true;
/**
 * 隐私条款check框未选择时点击一键登录按钮提示文字
 */
public String privacyUnCheckedToastText = "请同意服务条款";
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
public boolean privacyAddFrenchQuotes = false;
/**
 * 隐私协议文字对齐方式
 */
public int privacyTextGravity = Gravity.TOP | Gravity.START;
```

2. 授权页自定义 UI 配置 CustomView

授权页自定义 UI 配置，配置该参数可通过动态添加 View 的形式给授权页添加更多 UI 控件。详细配置说明请参考「授权页 UI 配置」章节。

**代码示例**

```cs
Widget w0 = new Widget();
w0.viewId = "view_line1";
w0.type = "View";
w0.left = 84;
w0.top = 510;
w0.width = 51;
w0.height = 1;
w0.clickable = false;
w0.backgroundColor = 0xFFD8D8D8;

Widget w1 = new Widget();
w1.viewId = "view_tips";
w1.type = "TextView";
w1.left = 145;
w1.top = 500;
w1.clickable = false;
w1.text = "其他方式登录";
w1.textSize = 16;
w1.textColor = 0xFF797894;

Widget w2 = new Widget();
w2.viewId = "view_line2";
w2.type = "View";
w2.left = 258;
w2.top = 510;
w2.width = 51;
w2.height = 1;
w2.clickable = false;
w2.backgroundColor = 0xFFD8D8D8;

Widget w3 = new Widget();
w3.viewId = "weixin_login";
w3.type = "ImageView";
w3.left = 113;
w3.top = 535;
w3.width = 35;
w3.height = 35;
w3.backgroundImgPath = "weixin";

Widget w4 = new Widget();
w4.viewId = "qq_login";
w4.type = "ImageView";
w4.left = 178;
w4.top = 535;
w4.width = 35;
w4.height = 35;
w4.backgroundImgPath = "qq";

Widget w5 = new Widget();
w5.viewId = "weibo_login";
w5.type = "ImageView";
w5.left = 243;
w5.top = 535;
w5.width = 35;
w5.height = 35;
w5.backgroundImgPath = "weibo";

Widget[] widgets = new Widget[6];
widgets[0] = w0;
widgets[1] = w1;
widgets[2] = w2;
widgets[3] = w3;
widgets[4] = w4;
widgets[5] = w5;
return widgets;
```

参数说明：

```cs
string viewId;              //自定义控件 ID
string type;                //自定义控件类型，当前只支持 View, ImageView, TextView
int left;                   //控件局屏幕左边缘偏移量，单位 dp
int top;                    //控件局标题栏下边缘偏移量，单位 dp
int right;                  //控件局屏幕右边缘偏移量，单位 dp
int bottom;                 //控件局屏幕底部偏移量，单位 dp
int width;                  //控件宽度，单位 dp
int height;                 //控件高度，单位 dp
bool clickable;             //控件是否可以点击，默认为可点击
string text;                //type 为 TextView 时控件文本内容
int textSize;               //type 为 TextView 时控件文本字体大小，单位:sp
int textColor;              //type 为 TextView 时控件文本颜色
string backgroundImgPath;   //type 为 TextView 和 ImageView 时控件背景图片
int backgroundColor;        //控件背景颜色
```


## 本机号码认证 API 使用说明

### 初始化


**方法描述**

SDK 初始化

**代码示例**

```cs
olManager.Call("setLogEnabled", true);//打开日志开关
olManager.Call("initWithCustomID", "您在极验后台创建应用时生成的 appid", 8000, new GOLMethodCallback(this));
```

**参数说明**

参数|类型|说明
---|---|---
appid|string|极验后台配置唯一产品 APPID，请在官网申请
timeout|int|超时时间，单位:`ms`，取值范围:`1000~15000`，默认`5000`
initListener|MethodCallback|初始化方法回调, 继承自 AndroidJavaProxy，对应插件 com.geetest.onelogin.callback.MethodCallback 类的回调


### 本机号码认证

在初始化执行之后调用，本机号码认证界面需自行实现，可以在多个需要认证的地方调用。


调用示例：

```cs
string number = inputField.text;
// 建议增加号码格式检查
olManager.Call("verifyPhoneNumber", number, new OnePassPluginCallback(this));
```


**参数说明**

- 接口参数说明：

参数|类型|说明
---|---|---
phone|string|需要校验的手机号


- 回调接口描述：

回调|含义|可用参数
---|---|---
onTokenSuccess | 本机号码认证获取 Token 成功回调| json 格式返回结果，`status` 返回码
onTokenFail | 隐私条款点击 | `name`: 隐私条款名称<br>`url`: 隐私条款地址
onAlgorithm | 是否加密手机号 |
onAlgorithmSelf | 是否自行加密手机号 |
onAlgorithmPhone | 自行加密手机号 | `operator`运营商, `key`加密密钥


- onTokenSuccess 返回参数描述：

字段|类型|含义
---|---|---
process_id|String|流水号
accesscode|String| 本机号码认证返回的 accessCode
phone|String| 手机号

- 返回参数示例:

```json
{"process_id":"7a03ba67e802ce3f6d34567acd45689c","accesscode":"a938d50f81d349dc824ba0515b3a1206","phone":"13888888888"}
```


### 校验手机号

当本机号校验外层 jscode 为200 时，或者返回参数中是否有 accesscode，可以判断 verifyPhoneNumber 接口是否返回成功。返回成功您将获取到返回的参数，请将这些参数传递给后端开发人员，并参考「[服务端](https://docs.geetest.com/onelogin/deploy/server)」文档来实现本机号码认证的
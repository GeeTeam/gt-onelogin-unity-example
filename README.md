# Unity 插件集成文档

# 创建应用

登录极验后台创建应用获取 appId 和 key，具体步骤可参照 [账号创建](https://docs.geetest.com/onelogin/overview/account)

# Android

## 前置条件

- 极验 SDK 支持 Android Studio 2.1.3 及以上版本，Android 5.0及以上版本
- 极验 SDK 支持中国移动 2G/3G/4G/5G，联通 3G/4G/5G，电信 4G/5G （2G/3G 网络下时延相对较高，成功率相对较低） 的取号能力
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
timeout|int|超时时间，单位:`ms`，取值范围:`1000~15000`，默认`8000`
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

返回信息包含一键登录成功、失败、拉起授权页失败、授权页返回、切换账号等回调信息，具体请参考官网一键登录「[返回结果](https://docs.geetest.com/onelogin/deploy/android#2-1%E3%80%81%E7%BB%93%E6%9E%9C%E8%BF%94%E5%9B%9E)」说明文档。当返回码为 200 时，返回信息包含一键登录的 token 信息，请将这些参数传递给后端开发人员，并参考「[服务端](https://docs.geetest.com/onelogin/deploy/server)」文档来实现获取手机号码的步骤。

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
        olb.navText = "一键登录"; olb.navTextColor = 0xFFFFFFFF; olb.navTextSize = 17; olb.navWebTextNormal = true; olb.navWebText = "自定义服务条款标题"; olb.navWebTextColor = 0xFF000000; olb.navWebTextSize = 17;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 40; olb.returnImgHeight = 40; olb.returnImgHidden = false; olb.returnImgOffsetX = 8;
        olb.logoImgPath = "gt_one_login_logo"; olb.logoWidth = 71; olb.logoHeight = 71; olb.logoHidden = false; olb.logoOffsetY = 100; olb.logoOffsetY_B = 0; olb.logoOffsetX = 0;
        olb.sloganColor = 0xFFA8A8A8; olb.sloganSize = 10; olb.sloganOffsetY = 330; olb.sloganOffsetY_B = 0; olb.sloganOffsetX = 0;
        olb.numberColor = 0xFF3D424C; olb.numberSize = 24; olb.numberOffsetY = 160; olb.numberOffsetY_B = 0; olb.numberOffsetX = 0;
        olb.switchText = "切换账号"; olb.switchColor = 0xFF3973FF; olb.switchSize = 14; olb.switchHidden = false; olb.switchOffsetY = 230; olb.switchOffsetY_B = 0; olb.switchOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn"; olb.logBtnWidth = 290; olb.logBtnHeight = 45; olb.logBtnOffsetY = 270; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 18;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.unCheckedImgPath = "gt_one_login_unchecked"; olb.checkedImgPath = "gt_one_login_checked"; olb.privacyState = false; olb.privacyCheckBoxWidth = 12; olb.privacyCheckBoxHeight = 12;
        olb.privacyLayoutWidth = 256; olb.privacyOffsetY = 0; olb.privacyOffsetY_B = 18; olb.privacyOffsetX = 0; olb.isUseNormalWebActivity = true;
        olb.baseClauseColor = 0xFFA8A8A8; olb.clauseColor = 0xFF3973FF; olb.privacyClauseTextSize = 10;
        olb.privacyTextViewTv1 = "登录即同意"; olb.privacyTextViewTv2 = "和"; olb.privacyTextViewTv3 = "、"; olb.privacyTextViewTv4 = "并使用本机号码登录";
        olb.clauseNameOne = "自定义服务条款1"; olb.clauseUrlOne = "https://docs.geetest.com/onelogin/deploy/android";
        olb.clauseNameTwo = "自定义服务条款2"; olb.clauseUrlTwo = "https://docs.geetest.com/onelogin/changelog/android";
        break;
    case 1://弹窗式
        olb.isDialogTheme = true; olb.dialogWidth = popWidth; olb.dialogHeight = popHeight; olb.dialogX = 0; olb.dialogY = 0; olb.isDialogBottom = false; olb.isWebViewDialogTheme = true;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 40; olb.returnImgHeight = 40; olb.returnImgHidden = false; olb.returnImgOffsetX = 8;
        olb.logoImgPath = "gt_one_login_logo"; olb.logoWidth = 71; olb.logoHeight = 71; olb.logoHidden = false; olb.logoOffsetY = 60; olb.logoOffsetY_B = 0; olb.logoOffsetX = 0;
        olb.sloganColor = 0xFFA8A8A8; olb.sloganSize = 10; olb.sloganOffsetY = 270; olb.sloganOffsetY_B = 0; olb.sloganOffsetX = 0;
        olb.numberColor = 0xFF3D424C; olb.numberSize = 24; olb.numberOffsetY = 125; olb.numberOffsetY_B = 0; olb.numberOffsetX = 0;
        olb.switchText = "切换账号"; olb.switchColor = 0xFF3973FF; olb.switchSize = 14; olb.switchHidden = false; olb.switchOffsetY = 165; olb.switchOffsetY_B = 0; olb.switchOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn"; olb.logBtnWidth = 268; olb.logBtnHeight = 45; olb.logBtnOffsetY = 220; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 18;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.unCheckedImgPath = "gt_one_login_unchecked"; olb.checkedImgPath = "gt_one_login_checked"; olb.privacyState = false; olb.privacyCheckBoxWidth = 12; olb.privacyCheckBoxHeight = 12;
        olb.privacyLayoutWidth = -2; olb.privacyOffsetY = 0; olb.privacyOffsetY_B = 5; olb.privacyOffsetX = 0; olb.isUseNormalWebActivity = true; olb.privacyLayoutGravity = 16;
        break;
    case 2://沉浸式
        olb.statusBarColor = 0xFFFFFFFF; olb.navigationBarColor = 0xFFFFFFFF; olb.isLightColor = true;
        olb.returnImgPath = "gt_one_login_ic_chevron_left_black"; olb.returnImgWidth = 40; olb.returnImgHeight = 40; olb.returnImgHidden = false; olb.returnImgOffsetX = 0;
        olb.logBtnImgPath = "gt_one_login_btn"; olb.logBtnWidth = 290; olb.logBtnHeight = 45; olb.logBtnOffsetY = 310; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 18;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.privacyClauseTextStrings = new string[]{
            "登录即同意", "应用自定义服务条款一", "https://docs.geetest.com/onelogin/deploy/android", "",
            "和", "应用自定义服务条款二", "https://docs.geetest.com/onelogin/changelog/android", "",
            "和", "应用自定义服务条款三", "https://docs.geetest.com/onelogin/help/tech", "",
            "", "", "", ""};
        olb.protocolShakeStyle = 1;
        olb.privacyUnCheckedToastText = "亲，还没有同意服务条款哦";
        olb.privacyAddFrenchQuotes = true;
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
        olb.logBtnImgPath = "gt_one_login_btn"; olb.logBtnWidth = 268; olb.logBtnHeight = 36; olb.logBtnOffsetY = 169; olb.logBtnOffsetY_B = 0; olb.logBtnOffsetX = 0;
        olb.logBtnText = "一键登录"; olb.logBtnColor = 0xFFFFFFFF; olb.logBtnTextSize = 15;
        olb.loadingView = "umcsdk_load_dot_white"; olb.loadingViewWidth = 20; olb.loadingViewHeight = 20; olb.loadingViewOffsetRight = 12;
        olb.unCheckedImgPath = "gt_one_login_unchecked"; olb.checkedImgPath = "gt_one_login_checked"; olb.privacyState = false; olb.privacyCheckBoxWidth = 9; olb.privacyCheckBoxHeight = 9;
        olb.privacyLayoutWidth = 512; olb.privacyOffsetY = 0; olb.privacyOffsetY_B = 5; olb.privacyOffsetX = 0; olb.isUseNormalWebActivity = true;
        olb.baseClauseColor = 0xFFA8A8A8; olb.clauseColor = 0xFF3973FF; olb.privacyClauseTextSize = 10;
        olb.privacyTextViewTv1 = "登录即同意"; olb.privacyLayoutWidth = -2; olb.privacyLayoutGravity = 16;
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
public String navText = "";
/**
 * 授权页标题栏文字颜色
 */
public int navTextColor = 0xFF000000; // 亮色时默认为黑色
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
public String navWebText = "";
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
 * 标题栏标题文本的左右边距，默认不碰到返回按钮
 */
public int navTextMargin = 36;

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
 * 返回按钮是否垂直居中显示
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
 * 设置是否屏蔽物理返回键
 */
public boolean blockReturnKey = false;
/**
 * 设置是否屏蔽标题栏返回按钮
 */
public boolean blockReturnBtn = false;

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
 * slogan 是否显示
 */
public boolean sloganVisible = true;
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
 * 运营商品牌文案 空值不生效
 */
public String sloganText = "";

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
public String logBtnImgPath = "gt_one_login_btn";
/**
 * 隐私条款未勾选同意时登录按钮的背景图片
 */
public String logBtnUncheckedImgPath = "gt_one_login_btn_unchecked";
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
 * loading默认居中
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
public int switchWidth = ViewGroup.LayoutParams.WRAP_CONTENT;

/**
 * 切换帐号背景高
 */
public int switchHeight = ViewGroup.LayoutParams.WRAP_CONTENT;

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
 * 隐私协议整体布局子视图的对齐方式，默认顶部对齐，参考 android:gravity="center_horizontal" 属性
 */
public int privacyLayoutGravity = Gravity.CENTER_HORIZONTAL;

/**
 * 隐私条款连接字符1 配置空值会生效，不配置则使用默认值
 */
public String privacyTextViewTv1 = "";
/**
 * 隐私条款连接字符2 配置空值会生效，不配置则使用默认值
 */
public String privacyTextViewTv2 = "";
/**
 * 隐私条款连接字符3 配置空值会生效，不配置则使用默认值
 */
public String privacyTextViewTv3 = "";
/**
 * 隐私条款连接字符4 配置空值会生效，不配置则使用默认值
 */
public String privacyTextViewTv4 = "";

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
public boolean privacyState = false;
/**
 * 隐私条款check框未选择时是否显示提示文字
 */
public boolean enableToast = true;
/**
 * 隐私条款check框未选择时点击一键登录按钮提示文字 空值不生效 默认为“请同意服务条款”
 */
public String privacyUnCheckedToastText = "";
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
public int privacyCheckBoxOffsetY = 0;

/**
 * 复选框与隐私文本之间的间距
 */
public int privacyCheckBoxMarginRight = 5;

/**
 * 隐私条款是否增加书名号显示
 */
public boolean privacyAddFrenchQuotes = false;
/**
 * 隐私协议文字对齐方式
 */
public int privacyTextGravity = Gravity.TOP | Gravity.START;
/**
 * 服务条款未勾选时点击一键登录服务条款执行的动画样式，默认无动画
 * 值代表枚举对应的值
 */
public int protocolShakeStyle = 0;
/**
 * 语言设置，默认中文简体
 * 值代表枚举对应的值
 */
public int languageType = 0;
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

### 一键登录获取手机号

当用户授权登录后，在`OneLoginPluginCallback`的`onResult`回调中可以获取到取号的结果，字段含义可以参考「[返回结果](https://docs.geetest.com/onelogin/deploy/android#2-1%E3%80%81%E7%BB%93%E6%9E%9C%E8%BF%94%E5%9B%9E)」说明。当返回码为 200 时，返回信息包含一键登录的 token 信息，请将这些参数传递给后端开发人员，并参考「[服务端](https://docs.geetest.com/onelogin/deploy/server)」文档来实现获取手机号码的步骤。

### 预取号状态判断

**方法描述**

判断是否预取号结果是否有效，预取号失败、预取号结果超期、预取号结果已使用情况均返回预取号结果失效。

**代码示例**

```cs
bool isPreGetTokenResultValidate = olManager.Call("isPreGetTokenResultValidate");
```

**返回值说明**

类型   |说明
------	|-----
bool |返回`true`表示预取号有效，返回 `false`表示预取号失败或者已失效


### 获取运营商类型

**方法描述**

获取当前SIM卡的运营商类型，双卡手机获取当前流量对应的SIM卡运营商类型

**代码示例**

```cs
string carrier = olManager.Call("getCurrentCarrier");
```

**返回值说明**

类型   |说明
------	|-----
string |CM:中国移动 CU:中国联通 CT:中国电信 unknown:没有SIM卡或不支持的运营商

### 服务条款是否勾选

**方法描述**

判断是否勾选同意服务条款声明。

**代码示例**

```cs
bool isProtocolChecked = olManager.Call("isProtocolChecked");
```

**返回值说明**

类型   |说明
------	|-----
bool |返回`true`表示服务条款勾选框已选中，返回 `false`表示服务条款勾选框未选中

### 改变服务条款勾选状态

**方法描述**

改变服务条款勾选框的选中状态。

**代码示例**

```cs
olManager.Call("setProtocolCheckState", true);
```

**参数说明**

参数 |类型 |说明
-----|-----|-----
isChecked | bool |是否选中

### 删除预取号的缓存

**代码示例**

```cs
olManager.Call("deletePreResultCache");
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
timeout|int|超时时间，单位:`ms`，取值范围:`1000~15000`，默认`8000`
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

当本机号校验外层 jscode 为200 时，或者返回参数中是否有 accesscode，可以判断 verifyPhoneNumber 接口是否返回成功。返回成功您将获取到返回的参数，请将这些参数传递给后端开发人员，并参考「[服务端](https://docs.geetest.com/onelogin/deploy/server)」文档来实现本机号码认证的功能。

# iOS

## 前置条件

- 极验 SDK 支持 Xcode 11+，iOS 8.0+ 版本
- 极验 SDK 支持中国移动 2G/3G/4G/5G，联通 3G/4G/5G，电信 4G/5G 的取号能力
- 极验 SDK 支持网络环境为

1. 纯数据网络
2. 数据网络与 Wifi 网络同时开启

- 对于双卡手机，极验 SDK 取当前流量卡号

## 插件开发

### 创建插件工程

1. 若您已经有 iOS Unity 的 Xcode 工程，请直接在工程中创建插件文件，若您还没有 iOS Unity 的 Xcode 工程，请先创建 Xcode 工程，并新建插件文件，并且将插件的 .m 文件的后缀改为 .mm
2. 将 `Assets/Plugins/iOS` 目录下 SDK 相关文件 `OneLoginSDK.framework`、`account_login_sdk_noui_core.framework`、`EAccountApiSDK.framework`、`TYRZSDK.framework`、`OneLoginResource.bundle` 拷贝到您的 Xcode 工程，`Other Linker Flags` 添加 `-ObjC` 设置，并添加系统依赖库 `libz.1.2.8.tbd`、`libc++.1.tbd`

### 实现插件功能

首先声明需要在 Unity 中使用的 C 方法： 

```c
#if defined(__cplusplus)
extern "C"{
#endif
    // 桥接方法，Unity 中调用
    extern bool isProtocolCheckboxChecked();
    extern char* getCurrentCarrier();
    extern void setProtocolCheckState(bool isChecked);
    extern void deletePreResultCache();
    extern void registerCallback(const char *objName, const char *requestTokenCallbackName, const char *getPhoneCallbackName);
    extern void registerWihtAppID(const char *appId);
    extern void enterAuthController(const char *configs, char **widgets);
    extern void dismissAuthViewController();
    extern void setLogEnabled(bool enabled);
    extern void renewPreGetToken();
    extern bool isPreGetTokenResultValidate();
    extern void setRequestTimeout(double timeout);
    extern char* sdkVersion();
    extern void validateToken(const char *token, const char *appID, const char *processID, const char *authcode);
    extern void showAlertMessage(const char *message);
#if defined(__cplusplus)
}
#endif
```

然后实现声明的 C 方法：

```c
#if defined(__cplusplus)
extern "C"{
#endif
    bool isProtocolCheckboxChecked() {
        return [UnityPlugin isProtocolCheckboxChecked];
    }

    char* getCurrentCarrier() {
        return strdup([[UnityPlugin getCurrentCarrier] UTF8String]);
    }

    void setProtocolCheckState(bool isChecked) {
        [UnityPlugin setProtocolCheckState:isChecked];
    }
    
    void deletePreResultCache() {
        [UnityPlugin deletePreResultCache];
    }
    
    void registerWihtAppID(const char *appId) {
        [UnityPlugin registerWihtAppID:[NSString stringWithUTF8String:appId]];
    }
    
    void registerCallback(const char *objName, const char *requestTokenCallbackName, const char *getPhoneCallbackName) {
        [UnityPlugin registerCallbackWithObjName:[NSString stringWithUTF8String:objName] requestTokenCallbackName:[NSString stringWithUTF8String:requestTokenCallbackName] getPhoneCallbackName:[NSString stringWithUTF8String:getPhoneCallbackName]];
    }
    
    void enterAuthController(const char *configs, char **widgets) {
        NSMutableArray *tempWidgets = [NSMutableArray new];
        if (NULL != widgets && NULL != *widgets) {
            int widgetLen = 0;
            char *temp = widgets[0];
            while (temp) {
                widgetLen++;
                temp = widgets[widgetLen];
            }
            for (int i = 0; i < widgetLen; i++) {
                [tempWidgets addObject:[NSString stringWithUTF8String:widgets[i]]];
            }
        }
        [UnityPlugin enterAuthViewController:[NSString stringWithUTF8String:configs] widgets:tempWidgets.copy];
    }
    
    void dismissAuthViewController() {
        [UnityPlugin dismissAuthViewController];
    }
    
    void setLogEnabled(bool enabled) {
        [UnityPlugin setLogEnabled:enabled];
    }
    
    void renewPreGetToken() {
        [UnityPlugin renewPreGetToken];
    }
    
    bool isPreGetTokenResultValidate() {
        return [UnityPlugin isPreGetTokenResultValidate];
    }
    
    void setRequestTimeout(double timeout) {
        [UnityPlugin setRequestTimeout:timeout];
    }
    
    char* sdkVersion() {
        return strdup([[UnityPlugin sdkVersion] UTF8String]);
    }
    
    void validateToken(const char *token, const char *appID, const char *processID, const char *authcode) {
        [UnityPlugin validateToken:[NSString stringWithUTF8String:token] appID:[NSString stringWithUTF8String:appID] processID:[NSString stringWithUTF8String:processID] authcode:[NSString stringWithUTF8String:authcode]];
    }
    
    void showAlertMessage(const char *message) {
        [UnityPlugin showAlertMessage:[NSString stringWithUTF8String:message]];
    }
#if defined(__cplusplus)
}
#endif
```

C 方法中的具体实现，是通过调用 Objective-C 的方法来完成的：

```objc
// MARK: Init

+ (instancetype)sharedInstance {
    static OneLoginUnityPlugin *up = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        up = [[OneLoginUnityPlugin alloc] init];
    });
    return up;
}

// MARK: OneLogin Methods

- (BOOL) isProtocolCheckboxChecked {
    return [OneLoginPro isProtocolCheckboxChecked];
}

- (NSString *) getCurrentCarrier {
    OLNetworkInfo *networkInfo = [OneLoginPro currentNetworkInfo];
    return networkInfo.carrierName;
}
- (void) setProtocolCheckState:(BOOL)isChecked {
    [OneLoginPro setProtocolCheckState:isChecked];
}

- (void) deletePreResultCache {
    [OneLoginPro deletePreResultCache];
}

- (void)registerCallbackWithObjName:(NSString *)objName requestTokenCallbackName:(NSString *)requestTokenCallbackName getPhoneCallbackName:(NSString *)getPhoneCallbackName {
    NSLog(@"============ register callback ==============");
    NSLog(@"============ objName:%@ ==============",objName);
    NSLog(@"============ requestTokenCallbackName:%@ ==============",requestTokenCallbackName);
    NSLog(@"============ getPhoneCallbackName:%@ ==============",getPhoneCallbackName);
    self.objName = objName;
    self.requestTokenCallbackName = requestTokenCallbackName;
    self.getPhoneCallbackName = getPhoneCallbackName;
}

- (void)registerWihtAppID:(NSString *)appId {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    NSLog(@"============ registerWithAppId ==============");
    [OneLoginPro registerWithAppID:appId];
}

- (void)enterAuthViewController:(NSString *)configs widgets:(NSArray *)widgets {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    NSLog(@"============ enterAuthViewController ==============\r\n");
    NSLog(@"============\r\n configs: %@ \r\n==============", configs);
    
    OLAuthViewModel *viewModel = [[OLAuthViewModel alloc] init];
    NSError *jsonError = nil;
    NSDictionary *viewModelDict = [NSJSONSerialization JSONObjectWithData:[configs dataUsingEncoding:NSUTF8StringEncoding] options:(NSJSONReadingOptions)0 error:&jsonError];
    if (nil == jsonError && [viewModelDict isKindOfClass:[NSDictionary class]] && viewModelDict.count > 0) {
        // *************** languageType *************** //
        if (viewModelDict[@"languageType"]) {
            viewModel.languageType = (OLLanguageType)[viewModelDict[@"languageType"] integerValue];
        }
        
        // *************** statusBarStyle *************** //
        if (viewModelDict[@"statusBarStyle"]) {
            viewModel.statusBarStyle = (UIStatusBarStyle)[viewModelDict[@"statusBarStyle"] integerValue];
        }
        
        // *************** naviTitle *************** //
        if (viewModelDict[@"naviTitle"]) {
            NSString *naviTitleString = [NSString stringWithFormat:@"%@", viewModelDict[@"naviTitle"]];
            NSMutableAttributedString *naviTitle = [[NSMutableAttributedString alloc] initWithString:naviTitleString];
            if (viewModelDict[@"naviTitleColor"]) {
                [naviTitle addAttributes:@{NSForegroundColorAttributeName: [self colorFromHexString:viewModelDict[@"naviTitleColor"]] ?: UIColor.blackColor} range:NSMakeRange(0, naviTitleString.length)];
            }
            if ([self fontFromString:viewModelDict[@"naviTitleFont"]]) {
                [naviTitle addAttributes:@{NSFontAttributeName: [self fontFromString:viewModelDict[@"naviTitleFont"]]} range:NSMakeRange(0, naviTitleString.length)];
            }
            viewModel.naviTitle = naviTitle.copy;
        }
        
        if(viewModelDict[@"navTextMargin"] && [viewModelDict[@"navTextMargin"] doubleValue]) {
            viewModel.navTextMargin = [viewModelDict[@"navTextMargin"] doubleValue];
        }
        
        if ([self colorFromHexString:viewModelDict[@"naviBgColor"]]) {
            viewModel.naviBgColor = [self colorFromHexString:viewModelDict[@"naviBgColor"]];
        }
        
        UIImage *naviBackImage = [self imageWithName:viewModelDict[@"naviBackImage"]];
        if (nil != naviBackImage) {
            viewModel.naviBackImage = naviBackImage;
        }
        
        if (viewModelDict[@"naviHidden"]) {
            viewModel.naviHidden = [viewModelDict[@"naviHidden"] boolValue];
        }
        
        if (viewModelDict[@"backButtonRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"backButtonRect"]]]) {
                viewModel.backButtonRect = [self rectFromString:viewModelDict[@"backButtonRect"]];
            }
        }
        
        if (viewModelDict[@"backButtonHidden"]) {
            viewModel.backButtonHidden = [viewModelDict[@"backButtonHidden"] boolValue];
        }
        
        // *************** appLogo *************** //
        UIImage *appLogo = [self imageWithName:viewModelDict[@"appLogo"]];
        if (nil != appLogo) {
            viewModel.appLogo = appLogo;
        }
        
        if (viewModelDict[@"logoRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"logoRect"]]]) {
                viewModel.logoRect = [self rectFromString:viewModelDict[@"logoRect"]];
            }
        }
        
        if (viewModelDict[@"logoHidden"]) {
            viewModel.logoHidden = [viewModelDict[@"logoHidden"] boolValue];
        }
        
        if (viewModelDict[@"logoCornerRadius"]) {
            viewModel.logoCornerRadius = [viewModelDict[@"logoCornerRadius"] doubleValue];
        }
        
        // *************** phoneNum *************** //
        if ([self colorFromHexString:viewModelDict[@"phoneNumColor"]]) {
            viewModel.phoneNumColor = [self colorFromHexString:viewModelDict[@"phoneNumColor"]];
        }
        
        if ([self fontFromString:viewModelDict[@"phoneNumFont"]]) {
            viewModel.phoneNumFont = [self fontFromString:viewModelDict[@"phoneNumFont"]];
        }
        
        if (viewModelDict[@"phoneNumRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"phoneNumRect"]]]) {
                viewModel.phoneNumRect = [self rectFromString:viewModelDict[@"phoneNumRect"]];
            }
        }
        
        // *************** Switch Button *************** //
        if (viewModelDict[@"switchButtonText"]) {
            NSString *switchButtonText = [NSString stringWithFormat:@"%@", viewModelDict[@"switchButtonText"]];
            if ([self isValidString:switchButtonText]) {
                viewModel.switchButtonText = switchButtonText;
            }
        }
        
        if ([self colorFromHexString:viewModelDict[@"switchButtonColor"]]) {
            viewModel.switchButtonColor = [self colorFromHexString:viewModelDict[@"switchButtonColor"]];
        }
        
        if ([self colorFromHexString:viewModelDict[@"switchButtonBackgroundColor"]]) {
            viewModel.switchButtonBackgroundColor = [self colorFromHexString:viewModelDict[@"switchButtonBackgroundColor"]];
        }
        
        if ([self fontFromString:viewModelDict[@"switchButtonFont"]]) {
            viewModel.switchButtonFont = [self fontFromString:viewModelDict[@"switchButtonFont"]];
        }
        
        if (viewModelDict[@"switchButtonRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"switchButtonRect"]]]) {
                viewModel.switchButtonRect = [self rectFromString:viewModelDict[@"switchButtonRect"]];
            }
        }
        
        if (viewModelDict[@"switchButtonHidden"]) {
            viewModel.switchButtonHidden = [viewModelDict[@"switchButtonHidden"] boolValue];
        }
        
        // *************** Auth Button *************** //
        if (viewModelDict[@"authButtonImages"]) {
            NSArray *imageArray = viewModelDict[@"authButtonImages"];
            if (imageArray.count >= 3) {
                UIImage *image0 = [self imageWithName:imageArray[0]];
                UIImage *image1 = [self imageWithName:imageArray[1]];
                UIImage *image2 = [self imageWithName:imageArray[2]];
                if (image0 && image1 && image2) {
                    viewModel.authButtonImages = @[image0, image1, image2];
                }
            }
        }
        
        if (viewModelDict[@"authButtonTitle"]) {
            NSString *authButtonTitleString = [NSString stringWithFormat:@"%@", viewModelDict[@"authButtonTitle"]];
            if ([self isValidString:authButtonTitleString]) {
                NSMutableAttributedString *authButtonTitle = [[NSMutableAttributedString alloc] initWithString:authButtonTitleString];
                if (viewModelDict[@"authButtonTitleColor"]) {
                    [authButtonTitle addAttributes:@{NSForegroundColorAttributeName: [self colorFromHexString:viewModelDict[@"authButtonTitleColor"]] ?: UIColor.blackColor} range:NSMakeRange(0, authButtonTitleString.length)];
                }
                if ([self fontFromString:viewModelDict[@"authButtonTitleFont"]]) {
                    [authButtonTitle addAttributes:@{NSFontAttributeName: [self fontFromString:viewModelDict[@"authButtonTitleFont"]]} range:NSMakeRange(0, authButtonTitleString.length)];
                }
                viewModel.authButtonTitle = authButtonTitle.copy;
            }
        }
        
        if (viewModelDict[@"authButtonRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"authButtonRect"]]]) {
                viewModel.authButtonRect = [self rectFromString:viewModelDict[@"authButtonRect"]];
            }
        }
        
        if (viewModelDict[@"authButtonCornerRadius"]) {
            viewModel.authButtonCornerRadius = [viewModelDict[@"authButtonCornerRadius"] doubleValue];
        }
        
        // *************** slogan *************** //
        if (viewModelDict[@"sloganRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"sloganRect"]]]) {
                viewModel.sloganRect = [self rectFromString:viewModelDict[@"sloganRect"]];
            }
        }
        
        if (viewModelDict[@"sloganText"]) {
            viewModel.sloganText = viewModelDict[@"sloganText"];
        }
        
        if ([self colorFromHexString:viewModelDict[@"sloganTextColor"]]) {
            viewModel.sloganTextColor = [self colorFromHexString:viewModelDict[@"sloganTextColor"]];
        }
        
        if ([self fontFromString:viewModelDict[@"sloganTextFont"]]) {
            viewModel.sloganTextFont = [self fontFromString:viewModelDict[@"sloganTextFont"]];
        }
        
        // *************** Privacy Terms *************** //
        if (viewModelDict[@"defaultCheckBoxState"]) {
            viewModel.defaultCheckBoxState = [viewModelDict[@"defaultCheckBoxState"] boolValue];
        }
        
        if ([self imageWithName:viewModelDict[@"checkedImage"]]) {
            viewModel.checkedImage = [self imageWithName:viewModelDict[@"checkedImage"]];
        }
        
        if ([self imageWithName:viewModelDict[@"uncheckedImage"]]) {
            viewModel.uncheckedImage = [self imageWithName:viewModelDict[@"uncheckedImage"]];
        }
        
        if (viewModelDict[@"checkBoxRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"checkBoxRect"]]]) {
                viewModel.checkBoxRect = [self rectFromString:viewModelDict[@"checkBoxRect"]];
            }
        }
        
        NSMutableDictionary *privacyTermsAttributes = [NSMutableDictionary dictionary];
        if (viewModelDict[@"privacyTermsColor"]) {
            UIColor *privacyTermsColor = [self colorFromHexString:viewModelDict[@"privacyTermsColor"]];
            if (privacyTermsColor) {
                [privacyTermsAttributes setValue:privacyTermsColor forKey:NSForegroundColorAttributeName];
            }
        }
        if ([self fontFromString:viewModelDict[@"privacyTermsFont"]]) {
            UIFont *privacyTermsFont = [self fontFromString:viewModelDict[@"privacyTermsFont"]];
            if (privacyTermsFont) {
                [privacyTermsAttributes setValue:privacyTermsFont forKey:NSFontAttributeName];
            }
        }
        if (privacyTermsAttributes.count > 0) {
            viewModel.privacyTermsAttributes = privacyTermsAttributes.copy;
        }
        
        if (viewModelDict[@"additionalPrivacyTerms"]) {
            NSArray *additionalPrivacyTerms = viewModelDict[@"additionalPrivacyTerms"];
            if (additionalPrivacyTerms.count > 0) {
                NSMutableArray<OLPrivacyTermItem *> *items = [NSMutableArray arrayWithCapacity:additionalPrivacyTerms.count];
                for (NSInteger i = 0; i + 2 < additionalPrivacyTerms.count; i += 3) {
                    OLPrivacyTermItem *item = [[OLPrivacyTermItem alloc] initWithTitle:additionalPrivacyTerms[i]
                                                                               linkURL:[NSURL URLWithString:additionalPrivacyTerms[i + 1]]
                                                                                 index:[additionalPrivacyTerms[i + 2] integerValue]];
                    [items addObject:item];
                }
                
                if (items.count > 0) {
                    viewModel.additionalPrivacyTerms = items.copy;
                }
            }
        }
        
        if ([self colorFromHexString:viewModelDict[@"termTextColor"]]) {
            viewModel.termTextColor = [self colorFromHexString:viewModelDict[@"termTextColor"]];
        }
        
        if (viewModelDict[@"termsRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"termsRect"]]]) {
                viewModel.termsRect = [self rectFromString:viewModelDict[@"termsRect"]];
            }
        }
        
        if (viewModelDict[@"auxiliaryPrivacyWords"]) {
            NSArray *auxiliaryPrivacyWords = viewModelDict[@"auxiliaryPrivacyWords"];
            if (4 == auxiliaryPrivacyWords.count) {
                viewModel.auxiliaryPrivacyWords = auxiliaryPrivacyWords;
            }
        }
        
        if (viewModelDict[@"termsAlignment"]) {
            viewModel.termsAlignment = (NSTextAlignment)[viewModelDict[@"termsAlignment"] integerValue];
        }
        
        if (viewModelDict[@"privacyCheckBoxMarginRight"]) {
            viewModel.spaceBetweenCheckboxAndTermsText =  [viewModelDict[@"privacyCheckBoxMarginRight"] doubleValue];
        }
        
        if (viewModelDict[@"protocolShakeStyle"]) {
            viewModel.shakeStyle =  [viewModelDict[@"protocolShakeStyle"] integerValue];
        }
        
        // *************** Background *************** //
        if ([self colorFromHexString:viewModelDict[@"backgroundColor"]]) {
            viewModel.backgroundColor = (OLNotCheckProtocolShakeStyle)[self colorFromHexString:viewModelDict[@"backgroundColor"]];
        }
        
        if (viewModelDict[@"backgroundImage"]) {
            viewModel.backgroundImage = [self imageWithName:viewModelDict[@"backgroundImage"]];
        }
        
        if (viewModelDict[@"landscapeBackgroundImage"]) {
            viewModel.landscapeBackgroundImage = [self imageWithName:viewModelDict[@"landscapeBackgroundImage"]];
        }
        
        // *************** Popup *************** //
        if (viewModelDict[@"isPopup"]) {
            viewModel.isPopup = [viewModelDict[@"isPopup"] boolValue];
        }
        
        if (viewModelDict[@"popupRect"]) {
            if (![self isEqualToZeroOLRect:[self rectFromString:viewModelDict[@"popupRect"]]]) {
                viewModel.popupRect = [self rectFromString:viewModelDict[@"popupRect"]];
            }
        }
        
        if (viewModelDict[@"popupCornerRadius"]) {
            viewModel.popupCornerRadius = [viewModelDict[@"popupCornerRadius"] doubleValue];
        }
        
        if (viewModelDict[@"popupRectCorners"]) {
            NSArray *popupRectCorners = viewModelDict[@"popupRectCorners"];
            if (popupRectCorners.count > 0) {
                viewModel.popupRectCorners = popupRectCorners;
            }
        }
        
        if (viewModelDict[@"popupAnimationStyle"]) {
            viewModel.popupAnimationStyle = (OLAuthPopupAnimationStyle)[viewModelDict[@"popupAnimationStyle"] integerValue];
        }
        
        if ([self imageWithName:viewModelDict[@"closePopupImage"]]) {
            viewModel.closePopupImage = [self imageWithName:viewModelDict[@"closePopupImage"]];
        }
        
        if (viewModelDict[@"closePopupTopOffset"]) {
            viewModel.closePopupTopOffset = [viewModelDict[@"closePopupTopOffset"] isKindOfClass:[NSNumber class]] ? viewModelDict[@"closePopupTopOffset"] : [NSNumber numberWithDouble:[viewModelDict[@"closePopupTopOffset"] doubleValue]];
        }
        
        if (viewModelDict[@"closePopupRightOffset"]) {
            viewModel.closePopupRightOffset = [viewModelDict[@"closePopupRightOffset"] isKindOfClass:[NSNumber class]] ? viewModelDict[@"closePopupRightOffset"] : [NSNumber numberWithDouble:[viewModelDict[@"closePopupRightOffset"] doubleValue]];
        }
        
        if (viewModelDict[@"canClosePopupFromTapGesture"]) {
            viewModel.canClosePopupFromTapGesture = [viewModelDict[@"canClosePopupFromTapGesture"] boolValue];
        }
        
        // *************** WebController NavigationBar *************** //
        if (viewModelDict[@"webNaviTitle"]) {
            NSString *webNaviTitleString = [NSString stringWithFormat:@"%@", viewModelDict[@"webNaviTitle"]];
            NSMutableAttributedString *webNaviTitle = [[NSMutableAttributedString alloc] initWithString:webNaviTitleString];
            if (viewModelDict[@"webNaviTitleColor"]) {
                [webNaviTitle addAttributes:@{NSForegroundColorAttributeName: [self colorFromHexString:viewModelDict[@"webNaviTitleColor"]] ?: UIColor.blackColor} range:NSMakeRange(0, webNaviTitleString.length)];
            }
            if ([self fontFromString:viewModelDict[@"webNaviTitleFont"]]) {
                [webNaviTitle addAttributes:@{NSFontAttributeName: [self fontFromString:viewModelDict[@"webNaviTitleFont"]]} range:NSMakeRange(0, webNaviTitleString.length)];
            }
            viewModel.webNaviTitle = webNaviTitle.copy;
        }
        
        if ([self colorFromHexString:viewModelDict[@"webNaviBgColor"]]) {
            viewModel.webNaviBgColor = [self colorFromHexString:viewModelDict[@"webNaviBgColor"]];
        }
        
        // *************** Hint *************** //
        if (viewModelDict[@"notCheckProtocolHint"]) {
            viewModel.notCheckProtocolHint = viewModelDict[@"notCheckProtocolHint"];
        }
        
        // *************** UIModalPresentationStyle *************** //
        if (viewModelDict[@"modalPresentationStyle"]) {
            viewModel.modalPresentationStyle = (UIModalPresentationStyle)[viewModelDict[@"modalPresentationStyle"] integerValue];
        }
        
        // *************** OLPullAuthVCStyle *************** //
        if (viewModelDict[@"pullAuthVCStyle"]) {
            viewModel.pullAuthVCStyle = (OLPullAuthVCStyle)[viewModelDict[@"pullAuthVCStyle"] integerValue];
        }
        
        // *************** UIUserInterfaceStyle *************** //
        if (viewModelDict[@"userInterfaceStyle"]) {
            viewModel.userInterfaceStyle = [viewModelDict[@"userInterfaceStyle"] isKindOfClass:[NSNumber class]] ? viewModelDict[@"userInterfaceStyle"] : [NSNumber numberWithInteger:[viewModelDict[@"userInterfaceStyle"] integerValue]];
        }
        
        // *************** block *************** //
        
        // widgets
        NSArray *tempWidgets = widgets.copy;
        if ((nil == tempWidgets || 0 == tempWidgets.count) && viewModelDict[@"widgets"]) {
            tempWidgets = viewModelDict[@"widgets"];
        }
        
        if ([tempWidgets isKindOfClass:[NSArray class]] && tempWidgets.count > 0) {
            viewModel.customUIHandler = ^(UIView * _Nonnull customAreaView) {
                for (NSInteger i = 0; i < tempWidgets.count; i++) {
                    NSDictionary *widgetDict = nil;
                    if ([tempWidgets[i] isKindOfClass:[NSDictionary class]]) {
                        widgetDict = tempWidgets[i];
                    } else if ([self isValidString:tempWidgets[i]]) {
                        NSError *jsonError = nil;
                        widgetDict = [NSJSONSerialization JSONObjectWithData:[tempWidgets[i] dataUsingEncoding:NSUTF8StringEncoding] options:(NSJSONReadingOptions)0 error:&jsonError];
                    }
                    NSLog(@"widgetDict:%@",widgetDict);
                    UIView *view = [self widgetFromDict:widgetDict];
                    NSLog(@"widgetFromDict:%@",view);
                    if (view && !CGRectEqualToRect(CGRectZero, view.frame)) {
                        [customAreaView addSubview:view];
                    }
                }
            };
        }
        
//        __weak typeof(self) wself = self;
        if (viewModelDict[@"authVCTransitionBlock"]) {
            viewModel.authVCTransitionBlock = ^(CGSize size, id<UIViewControllerTransitionCoordinator>  _Nonnull coordinator, UIView * _Nonnull customAreaView) {
                [self unitySendMessage:self.objName method:viewModelDict[@"authVCTransitionBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"tapAuthBackgroundBlock"]) {
            viewModel.tapAuthBackgroundBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"tapAuthBackgroundBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"viewLifeCycleBlock"]) {
            viewModel.viewLifeCycleBlock = ^(NSString * _Nonnull viewLifeCycle, BOOL animated) {
                [self unitySendMessage:self.objName method:viewModelDict[@"viewLifeCycleBlock"] msgDict:@{@"viewLifeCycle" : viewLifeCycle}];
            };
        }
        
        if (viewModelDict[@"clickBackButtonBlock"]) {
            viewModel.clickBackButtonBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"clickBackButtonBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"clickSwitchButtonBlock"]) {
            viewModel.clickSwitchButtonBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"clickSwitchButtonBlock"] msgDict:nil];
            };
        }
        
        if (viewModelDict[@"clickCheckboxBlock"]) {
            viewModel.clickCheckboxBlock = ^(BOOL isChecked) {
                [self unitySendMessage:self.objName method:viewModelDict[@"clickCheckboxBlock"] msgDict:@{@"isChecked" : (isChecked ? @"true" : @"false")}];
            };
        }
        
        if (viewModelDict[@"hintBlock"]) {
            viewModel.hintBlock = ^{
                [self unitySendMessage:self.objName method:viewModelDict[@"hintBlock"] msgDict:nil];
            };
        }
    }
    
    [OneLoginPro requestTokenWithViewController:[self findCurrentShowingViewController] viewModel:viewModel completion:^(NSDictionary * _Nullable result) {
        [self unitySendMessage:self.objName method:self.requestTokenCallbackName msgDict:result];
    }];
}

- (void)dismissAuthViewController {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    NSLog(@"============ dismissAuthViewController ==============");
    [OneLoginPro dismissAuthViewController:nil];
}

- (void)setLogEnabled:(BOOL)enabled {
    [OneLoginPro setLogEnabled:enabled];
}

- (void)renewPreGetToken {
    [OneLoginPro renewPreGetToken];
}

- (BOOL)isPreGetTokenResultValidate {
    return [OneLoginPro isPreGetTokenResultValidate];
}

- (void)setRequestTimeout:(NSTimeInterval)timeout {
    [OneLoginPro setRequestTimeout:timeout];
}

- (NSString *)sdkVersion {
    return [OneLoginPro sdkVersion];
}

- (void)registerOnepassCallback:(NSString *)objName verifyPhoneCallbackName:(NSString *)verifyPhoneCallbackName validatePhoneCallbackName:(NSString *)validatePhoneCallbackName {
    NSLog(@"============ register onepass callback ==============");
    
    self.objName = objName;
    self.verifyPhoneCallbackName = verifyPhoneCallbackName;
    self.validatePhoneCallbackName = validatePhoneCallbackName;
}

- (void)initWithCustiomId:(NSString * _Nonnull)customID timeout:(NSTimeInterval)timeout {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    self.gopManager = [[GOPManager alloc] initWithCustomID:customID timeout:timeout];
    self.gopManager.delegate = self;
}

- (void)verifyPhoneNumber:(NSString *)phoneNumber {
    // 防抖，防止短时间内多次点击
    NSTimeInterval currentTimeInterval = [[NSDate date] timeIntervalSince1970];
    if (currentTimeInterval - self.timeInterval < OLMinTimeInterval) {
        return;
    }
    self.timeInterval = currentTimeInterval;
    
    [self.gopManager verifyPhoneNumber:phoneNumber];
}

// MARK: GOPManagerDelegate

- (void)gtOnePass:(GOPManager *)manager didReceiveDataToVerify:(NSDictionary *)data {
    [self unitySendMessage:self.objName method:self.verifyPhoneCallbackName msgDict:data];
}

- (void)gtOnePass:(GOPManager *)manager errorHandler:(GOPError *)error {
    [self unitySendMessage:self.objName method:self.verifyPhoneCallbackName msgDict:@{@"errorMsg":error.description ?: @"onepass failed"}];
}

// MARK: Validate Token

- (void)validateToken:(NSString *)token appID:(NSString *)appID processID:(NSString *)processID authcode:(NSString *)authcode {
    NSString *oneloginResult = @"onelogin/result";
    NSURL *url = [NSURL URLWithString:[NSString stringWithFormat:@"%@%@", @"http://onepass.geetest.com/", oneloginResult]];
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url];
    request.HTTPMethod = @"POST";
    NSMutableDictionary *params = @{}.mutableCopy;
    if (token) {
        params[@"token"] = token;
    }
    if (appID) {
        params[@"id_2_sign"] = appID;
    }
    if (processID) {
        params[@"process_id"] = processID;
    }
    if (authcode) {
        params[@"authcode"] = authcode;
    }
    request.HTTPBody = [NSJSONSerialization dataWithJSONObject:params options:NSJSONWritingPrettyPrinted error:nil];
    NSURLSession *session = [NSURLSession sessionWithConfiguration:[NSURLSessionConfiguration defaultSessionConfiguration]];
    NSURLSessionDataTask *task = [session dataTaskWithRequest:request completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        id result = nil;
        if (data && !error) {
            result = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:nil];
        }
        [self finishValidatingToken:result error:error];
        
        if (![result isKindOfClass:[NSDictionary class]] && nil != data) {
            NSLog(@"validateToken result: %@", [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]);
        }
    }];
    [task resume];
}

- (void)finishValidatingToken:(NSDictionary *)result error:(NSError *)error {
    NSLog(@"validateToken result: %@, error: %@", result, error);
    dispatch_async(dispatch_get_main_queue(), ^{
        [self unitySendMessage:self.objName method:self.getPhoneCallbackName msgDict:result];
    });
}

// MARK: Validate Onepass Token

- (void)validateOnePassAccessCode:(NSString *)accessCode customId:(NSString *)customId processId:(NSString *)processId phone:(NSString *)phone operatorType:(NSString *)operatorType {
    NSMutableDictionary *params = [NSMutableDictionary dictionary];
    if ([self isValidString:accessCode]) {
        params[@"accesscode"] = accessCode;
    }
    if ([self isValidString:customId]) {
        params[@"id_2_sign"] = customId;
    }
    if ([self isValidString:processId]) {
        params[@"process_id"] = processId;
    }
    if ([self isValidString:operatorType]) {
        params[@"operatorType"] = operatorType;
    }
    if ([self isValidString:phone]) {
        params[@"phone"] = phone;
    }
    NSURL *url = [NSURL URLWithString:@"http://onepass.geetest.com/v2.0/result"];
    NSMutableURLRequest *req = [NSMutableURLRequest requestWithURL:url];
    req.HTTPMethod = @"POST";
    req.HTTPBody = [NSJSONSerialization dataWithJSONObject:params options:0 error:nil];
    NSURLSessionDataTask *task = [NSURLSession.sharedSession dataTaskWithRequest:req completionHandler:^(NSData * _Nullable data, NSURLResponse * _Nullable response, NSError * _Nullable error) {
        NSLog(@"verify onepass result: %@", [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]);
        dispatch_async(dispatch_get_main_queue(), ^{
            if (nil != data) {
                NSDictionary *result = [NSJSONSerialization JSONObjectWithData:data options:(NSJSONReadingOptions)0 error:nil];
                if (result[@"status"] && [@(200) isEqual:result[@"status"]]) {
                    if (result[@"result"] && [@"0" isEqual:result[@"result"]]) {
                        [self unitySendMessage:self.objName method:self.validatePhoneCallbackName msgDict:result];
                    } else if (result[@"result"] && [@"1" isEqual:result[@"result"]]) {
                        [self unitySendMessage:self.objName method:self.validatePhoneCallbackName msgDict:result];
                    } else {
                        [self verifyOnepassFailed];
                    }
                } else {
                    [self verifyOnepassFailed];
                }
            } else {
                [self verifyOnepassFailed];
            }
        });
    }];
    [task resume];
}

- (void)verifyOnepassFailed {
    [self unitySendMessage:self.objName method:self.validatePhoneCallbackName msgDict:@{@"errorMsg" : @"validate phone failed"}];
}
```

具体实现，请参考`Assets/Plugins/iOS` 目录下的 `OneLoginUnityPlugin.mm` 文件

### Unity 实现 iOS 的回调

要在 Unity 中实现 iOS 的回调，需要借助 Unity 给 iOS 提供的 `void UnitySendMessage(const char* obj, const char* method, const char* msg);` 方法，Unity 在调用插件提供的方法后，插件将获取到的数据通过 `UnitySendMessage` 方法回传给 Unity：

```objc
- (void)unitySendMessage:(NSString *)obj method:(NSString *)method msgDict:(NSDictionary *)msgDict {
    NSString *params = @"";
    if (nil != msgDict && [msgDict isKindOfClass:[NSDictionary class]] && msgDict.count > 0) {
        NSError *jsonError = nil;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:msgDict options:(NSJSONWritingOptions)0 error:&jsonError];
        if (nil == jsonError && [jsonData isKindOfClass:[NSData class]] && jsonData.length > 0) {
            params = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        }
    }
    UnitySendMessage([self isValidString:obj] ? [obj UTF8String] : [@"" UTF8String], [self isValidString:method] ? [method UTF8String] : [@"" UTF8String], [params UTF8String]);
}
```

### Unity 调用插件方法

首先注册插件提供的方法：

```c#
// 获取勾选框状态
[DllImport("__Internal")]
private static extern bool isProtocolCheckboxChecked();

// 获取运营商类型
[DllImport("__Internal")]
private static extern string getCurrentCarrier();

// 设置勾选框勾选状态
[DllImport("__Internal")]
private static extern void setProtocolCheckState(boolisChecked);

// 删除预取号的缓存
[DllImport("__Internal")]
private static extern void deletePreResultCache();

// 注册回调
[DllImport("__Internal")]
private static extern void registerCallback(string objName, string requestTokenCallbackName, string getPhoneCallbackName);

// 注册，将 AppID 传给 SDK
[DllImport("__Internal")]
private static extern void registerWihtAppID(string appId);

// 进入授权页面
[DllImport("__Internal")]
private static extern void enterAuthController(string configs, string[] widgets);

// 关闭授权页面
[DllImport("__Internal")]
private static extern void dismissAuthViewController();

// 设置日志开关
[DllImport("__Internal")]
private static extern void setLogEnabled(bool enabled);

// 重新预取号
[DllImport("__Internal")]
private static extern void renewPreGetToken();

// 判断预取号结果是否有效
[DllImport("__Internal")]
private static extern bool isPreGetTokenResultValidate();

// 设置拉起授权页面时的超时时长
[DllImport("__Internal")]
private static extern void setRequestTimeout(double timeout);

// 获取 SDK 版本
[DllImport("__Internal")]
private static extern string sdkVersion();

// 校验 token 获取手机号，可以直接在 Unity 中实现
[DllImport("__Internal")]
private static extern void validateToken(string token, string appID, string processID, string authcode);

// 弹窗提示，可以直接在 Unity 中实现
[DllImport("__Internal")]
private static extern void showAlertMessage(string message);
```

在需要通过插件完成相应功能的地方，调用插件提供的方法：

```c#
void Start () {
	print("OneLogin is start!\r\n");
    string sv = sdkVersion();
    Console.WriteLine("============ sdk version: {0} ============ ", sv);
    registerCallback("Main Camera", "requestTokenFinished", "getPhoneFinished");
}

public void initClicked() {
    print("Init button is clicked!");
    setLogEnabled(true);
	registerWihtAppID("b41a959b5cac4dd1277183e074630945");
}
```

具体交互实现，请参考 `Assets/iOS` 目录下的 `OneLoginPluginScript.cs` 文件

## Unity 集成插件

1. 将 `Assets/Plugins/iOS` 目录下 SDK 相关文件 `OneLoginSDK.framework`、`account_login_sdk_noui_core.framework`、`EAccountApiSDK.framework`、`TYRZSDK.framework`、`OneLoginResource.bundle` 拷贝到您的 Unity 工程的 `Assets/Plugins/` 目录下
2. 将您上面创建的插件功能实现的文件(.h 和 .mm 文件)，拷贝到您的 Unity 工程的 `Assets/Plugins/` 目录下，您也可以直接使用 `Assets/Plugins/iOS` 目录下的 `OneLoginUnityPlugin.h` 和 `OneLoginUnityPlugin.mm` 文件
3. 创建 `C#` 脚本文件，在文件中根据上文 [Unity 调用插件方法](#Unity 调用插件方法) 章节中的内容实现 Unity 与 iOS 交互的代码，然后将脚本与 Unity 工程进行绑定

## 一键登录具体步骤

**1、初始化**

传入极验 appID，并开始预取号

```c#
public void initClicked() {
    print("Init button is clicked!");
    setLogEnabled(true);
	registerWihtAppID("b41a959b5cac4dd1277183e074630945");
}
```

**2、进入授权页面**

拉起授权页面，用户在授权页面点击一键登录，即可获取 token，拿该 token 即可换取对应的手机号

```c#
public void enterAuthControllerClicked() {
	print("Enter auth controller button is clicked!");
		// 授权页面配置
		OLAuthViewModel viewModel = new OLAuthViewModel();

		// statusBar
		viewModel.statusBarStyle = 0;
        viewModel.languageType = 2;

		// navigation bar
		viewModel.naviTitle = "一键登录Unity";
		viewModel.naviTitleColor = "#FF4900";
		viewModel.naviTitleFont = 17.0;
        viewModel.naviBgColor = "#00FF00";
        viewModel.naviBackImage = "close_black";
        viewModel.naviHidden = false;
        viewModel.backButtonRect = "10, 0, 20, 0, 0, 0, 20, 20";
        viewModel.backButtonHidden = false;

        // logo
        viewModel.appLogo = "logo_icon";
        viewModel.logoRect = "";
        viewModel.logoHidden = false;
        viewModel.logoCornerRadius = 5;

        // phone
        viewModel.phoneNumColor = "#FF00FF";
        viewModel.phoneNumFont = 24;
        viewModel.phoneNumRect = "";

        // switch button
        viewModel.switchButtonText = "换个方式登录";
        viewModel.switchButtonColor = "#6500FF";
        viewModel.switchButtonBackgroundColor = "#FFFFFF";
        viewModel.switchButtonFont = 15;
        viewModel.switchButtonRect = "";
        viewModel.switchButtonHidden = false;

        // auth button
        // viewModel.authButtonImages = {"button_bg", "button_bg", "button_bg"};
        viewModel.authButtonImages = new string[3];
        viewModel.authButtonImages[0] = "authbutton_bg";
        viewModel.authButtonImages[1] = "authbutton_bg";
        viewModel.authButtonImages[2] = "authbutton_bg";
        viewModel.authButtonTitle = "授权登录";
        viewModel.authButtonTitleColor = "#FFFFFF";
        viewModel.authButtonTitleFont = 17;
        viewModel.authButtonRect = "";
        viewModel.authButtonCornerRadius = 5;

        // slogan
        viewModel.sloganRect = "";
        viewModel.sloganTextColor = "#FFFF00";
        viewModel.sloganTextFont = 13;
        viewModel.sloganText = "极验提供一键登录服务";

        // privacy terms
        viewModel.defaultCheckBoxState = false;
        viewModel.checkedImage = "";
        viewModel.uncheckedImage = "";
        viewModel.checkBoxRect = "";
        viewModel.privacyTermsColor = "#00FF00";
        viewModel.privacyTermsFont = 14;
        // additionalPrivacyTerms 为自定义的服务条款，每条服务条款对应三个元素：条款名称、条款链接、条款索引，所以 additionalPrivacyTerms 的元素个数 = 服务条款数 * 3
        viewModel.additionalPrivacyTerms = new string[6];
        // 服务条款1
        viewModel.additionalPrivacyTerms[0] = "自定义服务条款1";
        viewModel.additionalPrivacyTerms[1] = "https://docs.geetest.com/onelogin/deploy/ios";
        viewModel.additionalPrivacyTerms[2] = "0";
        // 服务条款2
        viewModel.additionalPrivacyTerms[3] = "自定义服务条款2";
        viewModel.additionalPrivacyTerms[4] = "https://docs.geetest.com/onelogin/changelog/ios";
        viewModel.additionalPrivacyTerms[5] = "1";
        viewModel.termTextColor = "#0000FF";
        viewModel.termsRect = "";
        viewModel.auxiliaryPrivacyWords = new string[4];
        viewModel.auxiliaryPrivacyWords[0] = "登录表示同意";
        viewModel.auxiliaryPrivacyWords[1] = "与";
        viewModel.auxiliaryPrivacyWords[2] = "&";
        viewModel.auxiliaryPrivacyWords[3] = "并使用本机号码登录";
        viewModel.termsAlignment = 1;
        viewModel.protocolShakeStyle = 1;
        viewModel.privacyCheckBoxMarginRight = 10;

        // background
        viewModel.backgroundColor = "#FFFFFF";
        viewModel.backgroundImage = "background";
        viewModel.landscapeBackgroundImage = "";

        // 服务条款页面导航栏
        viewModel.webNaviTitle = "一键登录Unity服务条款";
        viewModel.webNaviTitleColor = "#1F90FF";
        viewModel.webNaviTitleFont = 20;
        viewModel.webNaviBgColor = "#0F0F00";

        // 未勾选服务条款勾选框时，点击授权按钮的提示
        viewModel.notCheckProtocolHint = "请先阅读服务条款";

        // modal style
        viewModel.modalPresentationStyle = 0;

        // pull auth viewcontroller style
        viewModel.pullAuthVCStyle = 0;

        // user interface style
        viewModel.userInterfaceStyle = 0;

        // authVCTransitionBlock
        viewModel.authVCTransitionBlock = "authVCTransitionBlock";

        // tapAuthBackgroundBlock
        viewModel.tapAuthBackgroundBlock = "tapAuthBackground";

        // viewLifeCycleBlock
        viewModel.viewLifeCycleBlock = "viewLifeCycle";

        // clickBackButtonBlock
        viewModel.clickBackButtonBlock = "clickBackButton";

        // clickSwitchButtonBlock
        viewModel.clickSwitchButtonBlock = "clickSwitchButton";

        // clickCheckboxBlock
        viewModel.clickCheckboxBlock = "clickCheckbox";
        
        // hintBlock
        viewModel.hintBlock = "hintCustom";

        // widgets
        double screenWidth = UnityEngine.Screen.width/3;
        double screenHeight = UnityEngine.Screen.height/3;
        Console.WriteLine("============ screenWidth: {0}, screenHeight: {1} ============", screenWidth, screenHeight);

        // viewModel.widgets = new string[3];
    
        // string widget0 = "{\"type\":\"UIButton\", \"image\":\"qq_icon\", \"action\":\"qqLoginAction\", \"frame\":\"" + (screenWidth/2 - 45 - 10).ToString() + "," + (screenHeight - 200).ToString() + ",45,45\"}";
        // Console.WriteLine("============ widget0: {0} ============ ", widget0);
        // viewModel.widgets[0] = widget0;
        // string widget1 = "{\"type\":\"UIButton\", \"image\":\"weixin_icon\", \"action\":\"weixinLoginAction\", \"frame\":\"" + (screenWidth/2 + 10).ToString() + "," + (screenHeight - 200).ToString() + ",45,45\"}";
        // Console.WriteLine("============ widget1: {0} ============ ", widget1);
        // viewModel.widgets[1] = widget1;
        // string widget2 = "{\"type\":\"UILabel\", \"textColor\":\"#D98866\", \"font\":15, \"textAlignment\":1, \"text\":\"三方登录\", \"frame\":\"" + ((screenWidth - 120)/2).ToString() + "," + (screenHeight - 250).ToString() + ",120,20\"}";
        // viewModel.widgets[2] = widget2;

        // 添加自定义控件
        OLWidget[] widgets = new OLWidget[3];

        // 自定义 UIButton
        OLWidget widget0 = new OLWidget();
        widget0.type = "UIButton";
        widget0.image = "qq_icon";
        widget0.action = "qqLoginAction";
        widget0.frame = new double[4];
        widget0.frame[0] = screenWidth/2 - 45 - 10;
        widget0.frame[1] = screenHeight - 200;
        widget0.frame[2] = 45;
        widget0.frame[3] = 45;
        widgets[0] = widget0;

        // 自定义 UIButton
        OLWidget widget1 = new OLWidget();
        widget1.type = "UIButton";
        widget1.image = "weixin_icon";
        widget1.action = "weixinLoginAction";
        widget1.frame = new double[4];
        widget1.frame[0] = screenWidth/2 + 10;
        widget1.frame[1] = screenHeight - 200;
        widget1.frame[2] = 45;
        widget1.frame[3] = 45;
        widgets[1] = widget1;

        // 自定义 UILabel
        OLWidget widget2 = new OLWidget();
        widget2.type = "UILabel";
        widget2.textColor = "#D98866";
        widget2.text = "三方登录";
        widget2.font = 15;
        widget2.textAlignment = 1;
        widget2.frame = new double[4];
        widget2.frame[0] = (screenWidth - 120)/2;
        widget2.frame[1] = screenHeight - 250;
        widget2.frame[2] = 120;
        widget2.frame[3] = 20;
        widgets[2] = widget2;

        int len = widgets.Length;
        string[] widgetsString = new string[len];
        for (int i = 0; i < len; i++) {
            OLWidget widget = widgets[i];
            string widgetString = JsonUtility.ToJson(widget);
            Console.WriteLine("============ widgetString: {0} ============", widgetString);
            if (null != widgetString) {
                widgetsString[i] = widgetString;
            }
        }
		
		// 进入授权页面
		enterAuthController(serializeModelToJsonString(viewModel), widgetsString);
}
```

**3、手动关闭授权页面**

当开发者设置点击一键登录或者自定义控件不自动销毁授权页时，将需要自行调用此方法主动销毁授权页，建议在置换手机号成功后销毁，请不要使用其他方式关闭授权页面

```c#
void requestTokenFinished(string result) {
    Console.WriteLine("============ reuqest token result: {0} ============ ", result);
    
    dismissAuthViewController();

    requestTokenResult = JsonUtility.FromJson<OLRequestTokenResult>(result);
    if (null != requestTokenResult) {
        Console.WriteLine("============ appID: {0}, authcode: {1}, processID: {2}, token: {3} ============ ", requestTokenResult.appID, requestTokenResult.authcode, requestTokenResult.processID, requestTokenResult.token);
    }

    if (200 == requestTokenResult.status) { // 取号成功
        showAlertMessage("token 获取成功");
    } else {                                // 取号失败
        showAlertMessage("token 获取失败");
    }
}
```

## 本机号码认证具体步骤

**1、初始化**

传入极验 appID

```c#
registerOnepassCallback("Main Camera", "onepassFinished", "validateOnepassFinished");

void onepassInitClicked() {
    initOnePass(OnePassCustomId, 10);
}
```

**2、获取校验是否为本机号码的 accesscode**

获取校验是否为本机号码的 accesscode

```c#
void getOnepassAccessCodeClicked() {
    verifyPhoneNumber(OnePassPhone);
}

void onepassFinished(string result) {
    Console.WriteLine("============ onepass result: {0} ============ ", result);
    
    onepassResult = JsonUtility.FromJson<OLOnePassResult>(result);
    if (null != onepassResult) {
        Console.WriteLine("============ process_id: {0}, accesscode: {1}, operatorType: {2}, phone: {3} ============ ", onepassResult.process_id, onepassResult.accesscode, onepassResult.operatorType, onepassResult.phone);
    }

    if (null == onepassResult.errorMsg) {  // 取 accesscode 成功
        showAlertMessage("onepass accesscode 获取成功");
    } else {                                // 取 accesscode 失败
        showAlertMessage("onepass accesscode 获取失败");
    }
}
```

**3、校验是否为本机号码**

```c#
void validateOnepassAccessCodeClicked() {
    if (null != onepassResult.accesscode) {
        validateOnePassAccessCode(onepassResult.accesscode, OnePassCustomId, onepassResult.process_id, onepassResult.phone, onepassResult.operatorType);
    } else {
         showAlertMessage("accesscode 获取失败，请先重新获取 accesscode");
    }
}

void validateOnepassFinished(string result) {
    Console.WriteLine("============ validate onepass result: {0} ============ ", result);

    OLValidateOnePassResult validateResult = JsonUtility.FromJson<OLValidateOnePassResult>(result);
    if ("200" == validateResult.status) {     
        if ("0" == validateResult.result) {
            showAlertMessage("校验成功");
        } else {
            showAlertMessage("非本机号码");
        }
    } else {                                // 取号失败
        string message = "校验失败: " + validateResult.error_msg;
        showAlertMessage(message);
    }
}
```

具体一键登录流程和本机号码认证流程请参考 [极验官方文档](https://docs.geetest.com/onelogin/deploy/ios)
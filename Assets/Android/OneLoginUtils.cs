using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneLoginUtils : MonoBehaviour {

    private AndroidJavaObject olManager;
    private AndroidJavaClass m_Jc;
    private InputField inputField;

    // Start is called before the first frame update
    void Start() {
        // SDK unity 插件管理类
        m_Jc = new AndroidJavaClass("com.geetest.onelogin.OneLoginManager");
        // SDK unity 插件管理对象
        olManager = m_Jc.CallStatic<AndroidJavaObject>("with", new object[0]);
        inputField = GameObject.Find("Canvas/InputField").GetComponent<InputField>();
    }

    // Update is called once per frame
    public void Update() {

    }

    /// <summary>
    ///  插件 SDK 提供的日志打印方法，输出到 Android logcat，仅供调试使用
    /// </summary>
    /// <param name="msg">要打印的日志</param>
    public void log(string msg) {
        olManager.Call("log", msg);//打印日志
    }

    /// <summary>
    /// 插件 SDK 提供的日志打印方法 Toast 弹出信息提示方法
    /// </summary>
    /// <param name="msg">要弹出的信息</param>
    private void toast(string msg) {
        //AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //if (unityActivity != null) {
        //    AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        //    unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
        //        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
        //        toastObject.Call("show");
        //    }));
        //}
        olManager.Call("toast", msg, true);// true:Toast.LENGTH_LONG false:Toast.LENGTH_SHORT
    }

    // 获取屏幕信息
    public ScreenInfo getScreenInfo() {
        string scrInfo = olManager.Call<string>("getScreenInfo");
        ScreenInfo screenInfo = JsonUtility.FromJson<ScreenInfo>(scrInfo);
        Debug.Log("getScreenInfo screenInfo=" + screenInfo);
        return screenInfo;
    }

    // 一键登录初始化
    public void initOneLogin() {
        olManager.Call("setLogEnabled", true);//打开日志开关
        olManager.Call("register", Constants.APP_ID_OL, 8000, new GOLMethodCallback(this));
    }

    // 本机号码认证初始化
    public void initOnePass() {
        olManager.Call("setLogEnabled", true);//打开日志开关
        olManager.Call("initWithCustomID", Constants.APP_ID_OP, 8000, new GOLMethodCallback(this));
    }

    // 一键登录拉起授权页
    public void requestToken() {
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
    }

    // 一键登录拉起弹框形式授权页
    public void popupOneLogin() {
        OneLoginBean oneLoginBean = getThemeConfig(1);
        string configStr = JsonUtility.ToJson(oneLoginBean, true);
        // 自定义控件参数可选配
        olManager.Call("requestToken", configStr, new OneLoginPluginCallback(this));
    }

    // 本机号码认证
    public void verifyPhoneNumber() {
        string number = inputField.text;
        // 建议增加号码格式检查
        olManager.Call("verifyPhoneNumber", number, new OnePassPluginCallback(this));
    }

    // 获取一键登录授权页样式配置
    private OneLoginBean getThemeConfig(int themeStyle) {
        ScreenInfo screenInfo = getScreenInfo();
        float density = screenInfo.density;
        int width = (int)(screenInfo.screenWidth / density);
        int height = (int)(screenInfo.screenHeight / density);
        int popWidth = (int)(width * 4 / 5);
        int popHeight = (int)(height * 3 / 5);
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
    }

    // 获取一键登录授权页自定义控件配置
    private Widget[] getWidgets() {
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
    }

    // 一键登录取号成功后通过服务端 check_phone 接口置换手机号
    public void checkPhone(OLResult olResult) {
        log("checkPhoneThread start");
        //OLResult olResult = obj as OLResult;
        olResult.id_2_sign = Constants.APP_ID_OL;
        string param = JsonUtility.ToJson(olResult, false);
        olManager.Call("requestPost", Constants.CHECK_PHONE_URL, param, 8000, new RequestPostCallback(this, true));
        //olManager.Call("dismissAuthActivity");
    }

    // 本机号码认证获取 accesscode 成功后通过服务端  check_gateway 接口校验手机号
    public void checkGateWay(OPResult opResult) {
        log("checkGateWayThread start");
        //OPResult opResult = obj as OPResult;
        opResult.id_2_sign = Constants.APP_ID_OP;
        string param = JsonUtility.ToJson(opResult, false);
        olManager.Call("requestPost", Constants.CHECK_GATE_WAY, param, 8000, new RequestPostCallback(this, false));
    }

    // 注册与初始化方法调用回调，用于判断方法是否调用成功
    class GOLMethodCallback : AndroidJavaProxy {
        private OneLoginUtils owner;

        public GOLMethodCallback(OneLoginUtils oneLoginUtils) : base("com.geetest.onelogin.callback.MethodCallback") {
            this.owner = oneLoginUtils;
        }

        public void onSuccess() {
            owner.log("GOLMethodCallback onSuccess");
        }
        public void onFailure(string msg) {
            owner.log("GOLMethodCallback onFailure, msg=" + msg);
        }
    }

    /// <summary>
    ///  一键登录取号回调
    /// </summary>
    class OneLoginPluginCallback : AndroidJavaProxy {
        private OneLoginUtils owner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oneLoginUtils"> 工具类，用来调用外部方法 </param>
        public OneLoginPluginCallback(OneLoginUtils oneLoginUtils) : base("com.geetest.onelogin.callback.OneLoginPluginCallback") {
            this.owner = oneLoginUtils;
        }

        /// <summary>
        /// 拉起授权页、点一键登录取号以及授权页其他用户操作的回调
        /// </summary>
        /// <param name="result"> json 格式的返回参数</param>
        public void onResult(string result) {
            owner.log("OneLoginPluginCallback onResult, result=" + result);
            // 反序列化时注意 operator 关键字
            OLResult olResult = JsonUtility.FromJson<OLResult>(result.Replace("operator", "operator_type"));
            owner.log("OneLoginPluginCallback onResult, olResult=" + olResult);
            if (olResult.status == 200) {
                owner.log("一键登录取号成功: process_id=" + olResult.process_id +
                    ", token=" + olResult.token +
                    ", authcode=" + olResult.authcode +
                    ", id_2_sign=" + olResult.app_id);
                // 取号成功后用以上四个参数请求服务端 check_phone 接口换取真实手机号，当前 demo 略
                owner.checkPhone(olResult);
            } else {
                owner.toast("onResult:" + result); 
                string errorCode = olResult.errorCode;
                if ("-20301".Equals(errorCode) || "-20302".Equals(errorCode)) {
                    owner.log("用户点击返回键关闭了授权页面");
                    return;
                } else if ("-20303".Equals(errorCode)) {
                    owner.log("用户点击切换账号");
                }
                owner.olManager.Call("dismissAuthActivity");
            }
        }

        /// <summary>
        /// 授权页面点击隐私协议条款的回调
        /// 如有需要自定义隐私条款页，可在此回调中跳转到自定义隐私条款页展示隐私条款
        /// 自定义隐私条款页参考 setPrivacyLayout 接口说明
        /// </summary>
        /// <param name="name">隐私条款名字</param>
        /// <param name="url">隐私条款URL</param>
        public void onPrivacyClick(String name, String url) {
            owner.log("OneLoginPluginCallback onPrivacyClick, name=" + name + ", url=" + url);
        }

        /// <summary>
        /// 授权页隐私栏选择框点击事件回调
        /// </summary>
        /// <param name="isChecked">选择框当前的选择状态</param>
        public void onPrivacyCheckBoxClick(bool isChecked) {
            owner.log("OneLoginPluginCallback onPrivacyCheckBoxClick, isChecked=" + isChecked);
        }

        /// <summary>
        /// 授权页点击一键登录按钮的回调
        /// </summary>
        public void onLoginButtonClick() {
            owner.log("OneLoginPluginCallback onLoginButtonClick");
        }

        /// <summary>
        /// 授权页点击切换账号按钮的回调
        /// 2.1.4 版本新增，同时 onResult 回调中仍然会返回 -20303 返回码的回调结果
        /// </summary>
        public void onSwitchButtonClick() {
            owner.log("OneLoginPluginCallback onSwitchButtonClick");
        }

        /// <summary>
        /// 授权页点击标题栏返回按钮或者手机返回键的回调
        /// 2.1.4 版本新增，同时 onResult 回调中仍然会返回 -20301 和 -20302 返回码的回调结果
        /// </summary>
        public void onBackButtonClick() {
            owner.log("OneLoginPluginCallback onBackButtonClick");
        }

        /// <summary>
        /// 授权页点击一键登录按钮后开始启动 loading 的回调
        /// 当用户未集成第三方验证时，onLoginButtonClick 与 onLoginLoading 先后几乎同时发回
        /// 当用户集成了第三方验证时，onLoginButtonClick 在点击一键登录时立即发回调，onLoginLoading 在验证结束后开始取号时发回调
        /// </summary>
        public void onLoginLoading() {
            owner.log("OneLoginPluginCallback onLoginLoading");
        }

        /// <summary>
        /// 拉起授权页成功时授权页 Activity 创建时的回调
        /// </summary>
        /// <param name="activity">授权页 Activity 实例</param>
        public void onAuthActivityCreate(object activity) {
            owner.log("OneLoginPluginCallback onAuthActivityCreate, activity=" + activity);
        }

        /// <summary>
        /// 用户点击授权页隐私条款项跳转到隐私页显示隐私条款内容，隐私页 Activity 创建时的回调
        /// 用户若自定义了隐私页，就不会受到该回调
        /// </summary>
        /// <param name="activity">隐私条款页 Activity 实例</param>
        public void onAuthWebActivityCreate(object activity) {
            owner.log("OneLoginPluginCallback onAuthWebActivityCreate, activity=" + activity);
        }

        /// <summary>
        /// 拉起授权页时返回脱敏手机号的回调
        /// </summary>
        /// <param name="phone">脱敏手机号</param>
        public void onRequestTokenSecurityPhone(string phone) {
            owner.log("OneLoginPluginCallback onRequestTokenSecurityPhone, phone=" + phone);
        }

        /// <summary>
        /// 点一键登录之前用户可增加一些其他额外的校验功能，防止被异常攻击
        /// </summary>
        /// <returns>true 接入其他验证, 验证成功结束后调用 requestTokenDelay 启动正常取号, false 默认验证</returns>
        public bool onRequestOtherVerify() {
            owner.log("OneLoginPluginCallback onRequestOtherVerify");
            return false;
        }

        /// <summary>
        /// 授权页点击自定义控件的回调
        /// </summary>
        /// <param name="viewId">自定义控件的 Id</param>
        public void onCustomViewClick(string viewId) {
            owner.log("OneLoginPluginCallback onCustomViewClick, viewId=" + viewId);
        }
    }

    /// <summary>
    /// 本机号码认证回调
    /// </summary>
    class OnePassPluginCallback : AndroidJavaProxy {
        private OneLoginUtils owner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oneLoginUtils">工具类，用来调用外部方法 </param>
        public OnePassPluginCallback(OneLoginUtils oneLoginUtils) : base("com.geetest.onelogin.callback.OnePassPluginCallback") {
            this.owner = oneLoginUtils;
        }

        /// <summary>
        /// 手机号是否加密传输回调
        /// </summary>
        /// <returns>true: 加密传输 false:不加密</returns>
        public bool onAlgorithm() {
            owner.log("OnePassPluginCallback onAlgorithm");
            return false;
        }

        /// <summary>
        /// 获取token失败回调
        /// </summary>
        /// <param name="result">json 格式错误信息</param>
        public void onTokenFail(string result) {
            owner.log("OnePassPluginCallback onTokenFail, result=" + result);
            owner.toast("本机号认证取号失败");
        }

        /// <summary>
        /// 获取token失败回调
        /// </summary>
        /// <param name="result">json 格式返回结果</param>
        public void onTokenSuccess(string result) {
            owner.log("OnePassPluginCallback onTokenSuccess, result=" + result);
            OPResult opResult = JsonUtility.FromJson<OPResult>(result);
            owner.log("OnePassPluginCallback onTokenSuccess, opResult=" + opResult);
            owner.log("本机号认证取号成功: process_id=" + opResult.process_id +
                    ", accesscode=" + opResult.accesscode +
                    ", phone=" + owner.inputField.text +
                    ", id_2_sign=" + Constants.APP_ID_OP +
                    ", timestamp=" + DateTime.Now.Ticks);
            // 获取 Token 成功后用以上五个参数请求服务端 check_gateway 接口校验手机号，当前 demo 略
            owner.checkGateWay(opResult);
        }

        /// <summary>
        /// 判断手机号是否加密回调
        /// 由自己自行实现手机号加密
        /// </summary>
        /// <returns>true:加密 false:不加密</returns>
        public bool onAlgorithmSelf() {
            owner.log("OnePassPluginCallback onAlgorithmSelf");
            return false;
        }

        /// <summary>
        /// 返回加密的手机号 如果无需加密，该回调不会被调用
        /// </summary>
        /// <param name="op">运营商</param>
        /// <param name="key">加密密钥</param>
        /// <returns>加密的手机号</returns>
        public string onAlgorithmPhone(string op, string key) {
            owner.log("OnePassPluginCallback onAlgorithmPhone, operator=" + op + ", key=" + key);
            return null;
        }
    }

    /// <summary>
    /// 网络请求回调
    /// 演示 Demo 使用，正式应用建议自行实现异步网络请求与结果处理
    /// </summary>
    class RequestPostCallback : AndroidJavaProxy {
        private OneLoginUtils owner;
        private bool isOneLogin;

        public RequestPostCallback(OneLoginUtils oneLoginUtils, bool isOneLogin) : base("com.geetest.onelogin.callback.RequestPostCallback") {
            this.owner = oneLoginUtils;
            this.isOneLogin = isOneLogin;
        }

        /// <summary>
        /// 网络请求结果回调
        /// </summary>
        /// <param name="result">请求结果</param>
        public void onResult(String result) {
            owner.log("RequestPostCallback onResult result=" + result);
            // 结果解析
            PostResult postResult = JsonUtility.FromJson<PostResult>(result);
            if (isOneLogin) {
                if (Constants.SUCCESS_CODE.Equals(postResult.status)) {
                    owner.toast("OneLogin 一键登录成功，手机号:" + postResult.result);
                } else {
                    owner.toast("OneLogin 一键登录失败");
                }
                // 取号结束后关闭授权页
                owner.olManager.Call("dismissAuthActivity");
            } else {
                if (Constants.SUCCESS_CODE.Equals(postResult.status)) {
                    owner.toast("OnePass 本机号码认证成功");
                } else {
                    owner.toast("OnePass 本机号码认证失败");
                }
            }
        }
    }
}

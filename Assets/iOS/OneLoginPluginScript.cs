using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

[Serializable]
public class OLWidget {
    public string type;
    public string image;
    public string backgroundImage;
    public string title;
    public string titleColor;
    public double titleFont;
    public double cornerRadius;
    public string action;
    public double[] frame;
    public double font;
    public string textColor;
    public int textAlignment;
    public string text;
    public string backgroundColor;
    public string placeholder;
}

[Serializable]
public class OLAuthViewModel {
    public int languageType;
    public int statusBarStyle;

    public double navTextMargin;
    public string naviTitle;
    public string naviTitleColor;
    public double naviTitleFont;
    public string naviBgColor;
    public string naviBackImage;
    public bool naviHidden;
    public string backButtonRect;
    public bool backButtonHidden;

    public string appLogo;
    public string logoRect;
    public bool logoHidden;
    public double logoCornerRadius;

    public string phoneNumColor;
    public double phoneNumFont;
    public string phoneNumRect;

    public string switchButtonText;
    public string switchButtonColor;
    public string switchButtonBackgroundColor;
    public double switchButtonFont;
    public string switchButtonRect;
    public bool switchButtonHidden;

    public string[] authButtonImages;
    public string authButtonTitle;
    public string authButtonTitleColor;
    public double authButtonTitleFont;
    public string authButtonRect;
    public double authButtonCornerRadius;

    public string sloganText;
    public string sloganRect;
    public string sloganTextColor;
    public double sloganTextFont;

    public bool defaultCheckBoxState;
    public string checkedImage;
    public string uncheckedImage;
    public string checkBoxRect;
    public string privacyTermsColor;
    public double privacyTermsFont;
    public string[] additionalPrivacyTerms;
    public string termTextColor;
    public string termsRect;
    public string[] auxiliaryPrivacyWords;
    public int termsAlignment;
    public int protocolShakeStyle;
    public double privacyCheckBoxMarginRight;

    public string backgroundColor;
    public string backgroundImage;
    public string landscapeBackgroundImage;

    public bool isPopup;
    public string popupRect;
    public double popupCornerRadius;
    public int[] popupRectCorners;
    public int popupAnimationStyle;
    public string closePopupImage;
    public double closePopupTopOffset;
    public double closePopupRightOffset;
    public bool canClosePopupFromTapGesture;

    public string webNaviTitle;
    public string webNaviTitleColor;
    public double webNaviTitleFont;
    public string webNaviBgColor;

    public string notCheckProtocolHint;

    public int modalPresentationStyle;

    public int pullAuthVCStyle;

    public int userInterfaceStyle;

    public string authVCTransitionBlock;

    public string tapAuthBackgroundBlock;

    public string viewLifeCycleBlock;

    public string clickBackButtonBlock;

    public string clickSwitchButtonBlock;

    public string clickCheckboxBlock;

    public string[] widgets;
    
    public string hintBlock;
}

[Serializable]
public class OLRequestTokenResult {
    public string appID;
    public string authcode;
    public string errorCode;
    public string model;
    public string msg;
    public string number;
    public string operatorType;
    public string processID;
    public string release;
    public int status;
    public string token;
}

[Serializable]
public class OLGetPhoneResult {
    public string result;
    public int status;
}

[Serializable]
public class OLOnePassResult {
    public string process_id;
    public string accesscode;
    public string operatorType;
    public string phone;
    public string errorMsg;
}

[Serializable]
public class OLValidateOnePassResult {
    public string result;
    public string status;
    public string error_msg;
}

public class OneLoginPluginScript : MonoBehaviour {
    // 获取勾选框状态
    [DllImport("__Internal")]
    private static extern bool isProtocolCheckboxChecked();
    
    // 获取运营商类型
    [DllImport("__Internal")]
    private static extern string getCurrentCarrier();

    // 设置勾选框勾选状态
    [DllImport("__Internal")]
    private static extern void setProtocolCheckState(bool isChecked);

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

    // 注册 OnePass 的回调
    [DllImport("__Internal")]
    private static extern void registerOnepassCallback(string objName, string verifyPhoneCallbackName, string validatePhoneCallbackName);

    // OnePass 初始化
    [DllImport("__Internal")]
    private static extern void initOnePass(string customID, double timeout);

    // 获取校验是否为本机号码的 token
    [DllImport("__Internal")]
    private static extern void verifyPhoneNumber(string phoneNumber);

    // 校验是否为本机号码
    [DllImport("__Internal")]
    private static extern void validateOnePassAccessCode(string accessCode, string customId, string processId, string phone, string operatorType);

    private OLRequestTokenResult requestTokenResult = null;
    private OLOnePassResult onepassResult = null;

    private const string OnePassCustomId = "3996159873d7ccc36f25803b88dda97a";
    private const string OnePassPhone = "18627096173";
    // private DateTime currentDT;

	// Use this for initialization
	void Start () {
		print("OneLogin is start!\r\n");
        string sv = sdkVersion();
        Console.WriteLine("============ sdk version: {0} ============ ", sv);
        string carrier = getCurrentCarrier();
        print("current carrier:"+carrier);
        registerCallback("OneLoginHandler", "requestTokenFinished", "getPhoneFinished");
        registerOnepassCallback("OneLoginHandler", "onepassFinished", "validateOnepassFinished");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
            if (OnePointColliderObject() != null) {
                if (OnePointColliderObject().name == "初始化") {
                    initClicked();
                } else if (OnePointColliderObject().name == "进入授权页面") {
                	enterAuthControllerClicked();
                } else if (OnePointColliderObject().name == "获取手机号") {
                	getPhoneClicked();
                } else if (OnePointColliderObject().name == "弹窗模式") {
                	popupButtonClicked();
                } else if (OnePointColliderObject().name == "浮窗模式") {
                	floatWindowButtonClicked();
                } else if (OnePointColliderObject().name == "OnePass初始化") {
                	onepassInitClicked();
                } else if (OnePointColliderObject().name == "验证是否为本机号码") {
                	validateOnepassAccessCodeClicked();
                } else if (OnePointColliderObject().name == "获取校验是否为本机号码的token") {
                    getOnepassAccessCodeClicked();
                }
            }
        }
	}

	public GameObject OnePointColliderObject() {
        // 存有鼠标或者触摸数据的对象
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        // 当前指针位置
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        // 射线命中之后的反馈数据
        List<RaycastResult> results = new List<RaycastResult>();
        // 投射一条光线并返回所有碰撞
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // 返回点击到的物体
        if (results.Count > 0) {
            return results[0].gameObject;
        } else {
            return null;
        }
    }

    public string serializeModelToJsonString(OLAuthViewModel viewModel) {
        if (null == viewModel) {
            return "";
        }

        string jsonStr = JsonUtility.ToJson(viewModel);
        Console.WriteLine("============ serializeDictionaryToJsonString: {0} ============ ", jsonStr);
        return jsonStr;
    }

	public void initClicked() {
        print("Init button is clicked!");
        setLogEnabled(true);
		registerWihtAppID("b41a959b5cac4dd1277183e074630945");
	}

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

    public void popupButtonClicked() {
        // 授权页面配置
		OLAuthViewModel viewModel = new OLAuthViewModel();

        viewModel.backgroundColor = "#FFFFFF";
        viewModel.defaultCheckBoxState = true;
        viewModel.switchButtonHidden = true;

        // popup
        viewModel.isPopup = true;
        viewModel.popupRect = "";
        viewModel.popupCornerRadius = 5;
        viewModel.popupAnimationStyle = 0;
        viewModel.closePopupTopOffset = 5;
        viewModel.closePopupRightOffset = -10;
        viewModel.canClosePopupFromTapGesture = true;
        
        // 进入授权页面
		enterAuthController(serializeModelToJsonString(viewModel), null);
    }

    public void floatWindowButtonClicked() {
        // 授权页面配置
		OLAuthViewModel viewModel = new OLAuthViewModel();

        viewModel.backgroundColor = "#FFFFFF";
        viewModel.defaultCheckBoxState = true;
        viewModel.switchButtonHidden = true;

        // popup
        viewModel.isPopup = true;

        // 设置弹窗的大小位置
        double screenWidth = UnityEngine.Screen.width/3;
        double screenHeight = UnityEngine.Screen.height/3;
        viewModel.popupRect = (screenHeight - 340).ToString() + ",0,0,0,0,0," + screenWidth.ToString() + ",340";

        viewModel.popupCornerRadius = 10;
        viewModel.popupRectCorners = new int[2];
        viewModel.popupRectCorners[0] = 1;
        viewModel.popupRectCorners[1] = 2;
        viewModel.popupAnimationStyle = 0;
        viewModel.closePopupTopOffset = 8;
        viewModel.closePopupRightOffset = -10;
        viewModel.canClosePopupFromTapGesture = true;
        
        // 进入授权页面
		enterAuthController(serializeModelToJsonString(viewModel), null);
    }

	public void getPhoneClicked() {
		print("Get phone button is clicked!");
        if (200 == requestTokenResult.status) {
            validateToken(requestTokenResult.token, requestTokenResult.appID, requestTokenResult.processID, requestTokenResult.authcode);
        } else {
            showAlertMessage("token 获取失败，请先重新获取 token");
        }
	}

    public void requestTokenFinished(string result) {
        Console.WriteLine("============ reuqest token result: {0} ============ ", result);

        requestTokenResult = JsonUtility.FromJson<OLRequestTokenResult>(result);
        if (null != requestTokenResult) {
            Console.WriteLine("============ appID: {0}, authcode: {1}, processID: {2}, token: {3} ============ ", requestTokenResult.appID, requestTokenResult.authcode, requestTokenResult.processID, requestTokenResult.token);
        }

        if (200 == requestTokenResult.status) { // 取号成功
            showAlertMessage("token 获取成功");
        } else {                                // 取号失败
            showAlertMessage("token 获取失败");
        }

        dismissAuthViewController();
    }

    void getPhoneFinished(string result) {
        Console.WriteLine("============ get phone result: {0} ============ ", result);

        OLGetPhoneResult getPhoneResult = JsonUtility.FromJson<OLGetPhoneResult>(result);
        if (200 == getPhoneResult.status) {     // 取号成功
            string message = "获取手机号成功: " + getPhoneResult.result;
            showAlertMessage(message);
            // 重新预取号
            renewPreGetToken();
        } else {                                // 取号失败
            string message = "获取手机号失败: " + getPhoneResult.result;
            showAlertMessage(message);
        }
    }

    void qqLoginAction(string param) {
        dismissAuthViewController();
        showAlertMessage("qq 登录");
    }

    void weixinLoginAction(string param) {
        dismissAuthViewController();
        showAlertMessage("微信登录");
    }

    void authVCTransitionBlock(string param) {
        print("============ authVCTransitionBlock ============");
    }

    void viewLifeCycle(string param) {
        Console.WriteLine("============ viewLifeCycle: {0} ============ ", param);
    }

    void tapAuthBackground(string param) {
        print("============ tapAuthBackground ============");
    }

    void clickBackButton(string param) {
        print("============ clickBackButton ============");
    }

    void clickSwitchButton(string param) {
        print("============ clickSwitchButton ============");
    }

    void clickCheckbox(string param) {
        Console.WriteLine("============ clickCheckbox: {0} ============ ", param);
    }
    
    void hintCustom(string param) {
        bool isChecked = isProtocolCheckboxChecked();
        Console.WriteLine("============ isProtocolCheckboxChecked: {0} ============ ", isChecked);
        Console.WriteLine("============ hintCustom: {0} ============ ", param);
        showAlertMessage("未勾选授权页面隐私协议前勾选框时，点击授权页面登录按钮时提示 block");
    }

    public void onepassInitClicked() {
        initOnePass(OnePassCustomId, 10);
    }

    public void getOnepassAccessCodeClicked() {
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

    public void validateOnepassAccessCodeClicked() {
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
}

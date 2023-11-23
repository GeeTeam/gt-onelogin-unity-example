package com.geetest.onelogin.util;

import android.annotation.SuppressLint;
import android.content.Context;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.geetest.onelogin.bean.OneLoginBean;
import com.geetest.onelogin.OneLoginHelper;
import com.geetest.onelogin.callback.OneLoginPluginCallback;
import com.geetest.onelogin.config.OLLanguageType;
import com.geetest.onelogin.config.ProtocolShakeStyle;
import com.geetest.onelogin.wrapper.RequestTokenListenerWrapper;
import com.geetest.onelogin.bean.ViewRect;
import com.geetest.onelogin.bean.Widget;
import com.geetest.onelogin.config.AuthRegisterViewConfig;
import com.geetest.onelogin.config.OneLoginThemeConfig;

import com.geetest.onelogin.listener.CustomInterface;

import org.json.JSONObject;

import java.lang.reflect.Field;

import static com.geetest.onelogin.constant.Constants.*;

/**
 * Created by 谷闹年 on 2019/4/1.
 * OneLogin工具类
 */
public class OneLoginUtils {
    private Context context;

    public OneLoginUtils() {
    }

    public void init(Context context, String appId) {
        this.context = context;
        OneLoginHelper.with().init(context, appId);
    }

    public void register(int timeout) {
        OneLoginHelper.with().register("", timeout);
    }

    public void requestToken(String themeJsonString, String[] customJsonString, OneLoginPluginCallback callback) {
        OneLoginThemeConfig oneLoginThemeConfig;
        JSONObject configJson = null;
        JSONObject[] customJsons = null;
        try {
            configJson = new JSONObject(themeJsonString);
        } catch (Exception e) {
//            Log.d(TAG, "requestToken->1 error=" + e.toString());
            e.printStackTrace();
        }
        try {
            if (customJsonString != null && customJsonString.length > 0) {
                int len = customJsonString.length;
                customJsons = new JSONObject[len];
                for (int i = 0; i < len; i++) {
                    customJsons[i] = new JSONObject(customJsonString[i]);
                }
            }
        } catch (Exception e) {
//            Log.d(TAG, "requestToken->2 error=" + e.toString());
            e.printStackTrace();
        }
        try {
            oneLoginThemeConfig = initConfig(configJson, customJsons, callback);
        } catch (Exception e) {
            e.printStackTrace();
//            Log.d(TAG, "requestToken->3 error=" + e.toString());
            oneLoginThemeConfig = new OneLoginThemeConfig.Builder().build();
        }
//        printConfig(oneLoginThemeConfig);
//        Log.d(TAG, "***********************************");
//        printConfig(new OneLoginThemeConfig.Builder().build());

        RequestTokenListenerWrapper tokenListener = new RequestTokenListenerWrapper(callback);
        OneLoginHelper.with().requestToken(oneLoginThemeConfig, tokenListener);
    }

    private void printConfig(OneLoginThemeConfig config) {
        StringBuffer buffer = new StringBuffer();
        Field[] fields = OneLoginThemeConfig.class.getDeclaredFields();
        int len = fields.length;
        try {
            for (int i = 0; i < len; i++) {
                Field field = fields[i];
                field.setAccessible(true);
                buffer.append(field.getName() + "=" + field.get(config) + "\n");
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
//        Log.d(TAG, "OneLoginThemeConfig:" + buffer.toString());
    }

    /**
     * 关闭 需在页面关闭时候调用
     */
    public void cancel() {
        OneLoginHelper.with().cancel();
    }

//    private OneLoginThemeConfig getThemeConfig(JSONObject jsonObject) {
//        int statusBarColor = getInt(jsonObject, "statusBarColor");
//        int navigationBarColor = getInt(jsonObject, "navigationBarColor");
//        boolean isLightColor  = getBoolean(jsonObject, "isLightColor");
//
//        int navColor = getInt(jsonObject, "navColor");
//        int authNavHeight = getInt(jsonObject, "authNavHeight");
//        boolean authNavTransparent = getBoolean(jsonObject, "authNavTransparent");
//        boolean authNavGone = getBoolean(jsonObject, "authNavGone");
//
//        String navText = getString(jsonObject, "navText");
//        int navTextColor = getInt(jsonObject, "navTextColor");
//        int navTextSize = getInt(jsonObject, "navTextSize");
//        boolean navTextNormal = getBoolean(jsonObject, "navTextNormal");
//        String navWebViewText = getString(jsonObject, "navWebViewText");
//        int navWebViewTextColor = getInt(jsonObject, "navWebViewTextColor");
//        int navWebViewTextSize = getInt(jsonObject, "navWebViewTextSize");
//
////        String navTextTypeface = getString(jsonObject, "navTextTypeface");
////        String navWebViewTextTypeface = getString(jsonObject, "navWebViewTextTypeface");
//
//        String authBGImgPath = getString(jsonObject, "authBGImgPath");
//        String authBgVideoUri = getString(jsonObject, "authBgVideoUri");
//
//        boolean isDialogTheme = getBoolean(jsonObject, "isDialogTheme");
//        int dialogWidth = getInt(jsonObject, "dialogWidth");
//        int dialogHeight = getInt(jsonObject, "dialogHeight");
//        int dialogX = getInt(jsonObject, "dialogX");
//        int dialogY = getInt(jsonObject, "dialogY");
//        boolean isDialogBottom = getBoolean(jsonObject, "isDialogBottom");
//        boolean isWebViewDialogTheme = getBoolean(jsonObject, "isWebViewDialogTheme");
//
//        String navReturnImgPath = getString(jsonObject, "navReturnImgPath");
//        int returnImgWidth = getInt(jsonObject, "returnImgWidth");
//        int returnImgHeight = getInt(jsonObject, "returnImgHeight");
//        boolean navReturnImgHidden = getBoolean(jsonObject, "navReturnImgHidden");
//        int returnImgOffsetX = getInt(jsonObject, "returnImgOffsetX");
//
//        boolean returnImgCenterInVertical;
//        int returnImgOffsetY;
//        if(jsonObject.containsKey("returnImgOffsetY")) {
//            returnImgCenterInVertical = false;
//            returnImgOffsetY = getInt(jsonObject, "returnImgOffsetY");
//        } else {
//            returnImgCenterInVertical = true;
//            returnImgOffsetY = 0;
//        }
//
//        String logoImgPath = getString(jsonObject, "logoImgPath");
//        int logoWidth = getInt(jsonObject, "logoWidth");
//        int logoHeight = getInt(jsonObject, "logoHeight");
//        boolean logoHidden = getBoolean(jsonObject, "logoHidden");
//        int logoOffsetY = getInt(jsonObject, "logoOffsetY");
//        int logoOffsetY_B = getInt(jsonObject, "logoOffsetY_B");
//        int logoOffsetX = getInt(jsonObject, "logoOffsetX");
//
//        int numberColor = getInt(jsonObject, "numberColor");
//        int numberSize = getInt(jsonObject, "numberSize");
//        int numFieldOffsetY = getInt(jsonObject, "numFieldOffsetY");
//        int numFieldOffsetY_B = getInt(jsonObject, "numFieldOffsetY_B");
//        int numFieldOffsetX = getInt(jsonObject, "numFieldOffsetX");
//
////        String numberViewTypeface = getString(jsonObject, "numberViewTypeface");
//
//        String switchText = getString(jsonObject, "switchText");
//        int switchColor = getInt(jsonObject, "switchColor");
//        int switchSize = getInt(jsonObject, "switchSize");
//        boolean switchAccHidden = getBoolean(jsonObject, "switchAccHidden");
//        int switchAccOffsetY = getInt(jsonObject, "switchAccOffsetY");
//        int switchOffsetY_B = getInt(jsonObject, "switchOffsetY_B");
//        int switchOffsetX = getInt(jsonObject, "switchOffsetX");
//
////        String switchViewTypeface = getString(jsonObject, "switchViewTypeface");
//
//        String switchImgPath = getString(jsonObject, "switchImgPath");
//        int switchWidth = getInt(jsonObject, "switchWidth");
//        int switchHeight = getInt(jsonObject, "switchHeight");
//
//        String loginImgPath = getString(jsonObject, "loginImgPath");
//        int logBtnWidth = getInt(jsonObject, "logBtnWidth");
//        int logBtnHeight = getInt(jsonObject, "logBtnHeight");
//        int logBtnOffsetY = getInt(jsonObject, "logBtnOffsetY");
//        int logBtnOffsetY_B = getInt(jsonObject, "logBtnOffsetY_B");
//        int logBtnOffsetX = getInt(jsonObject, "logBtnOffsetX");
//
//        String loginButtonText = getString(jsonObject, "loginButtonText");
//        int loginButtonColor = getInt(jsonObject, "loginButtonColor");
//        int logBtnTextSize = getInt(jsonObject, "logBtnTextSize");
//
////        String logBtnTextViewTypeface = getString(jsonObject, "logBtnTextViewTypeface");
//
//        boolean disableBtnIfUnChecked = getBoolean(jsonObject, "disableBtnIfUnChecked");
//
//        String loadingView = getString(jsonObject, "loadingView");
//        int loadingViewWidth = getInt(jsonObject, "loadingViewWidth");
//        int loadingViewHeight = getInt(jsonObject, "loadingViewHeight");
//        int loadingViewOffsetRight = getInt(jsonObject, "loadingViewOffsetRight");
//
//        boolean loadingViewCenterInVertical;
//        int loadingViewOffsetY;
//        if(jsonObject.containsKey("loadingViewOffsetY")) {
//            loadingViewCenterInVertical = false;
//            loadingViewOffsetY = getInt(jsonObject, "loadingViewOffsetY");
//        } else {
//            loadingViewCenterInVertical = true;
//            loadingViewOffsetY = 0;
//        }
//
//        int sloganColor = getInt(jsonObject, "sloganColor");
//        int sloganSize = getInt(jsonObject, "sloganSize");
//        int sloganOffsetY = getInt(jsonObject, "sloganOffsetY");
//        int sloganOffsetY_B = getInt(jsonObject, "sloganOffsetY_B");
//        int sloganOffsetX = getInt(jsonObject, "sloganOffsetX");
//
////        String sloganViewTypeface = getString(jsonObject, "sloganViewTypeface");
//
//        int privacyLayoutWidth = getInt(jsonObject, "privacyLayoutWidth");
//        int privacyOffsetY = getInt(jsonObject, "privacyOffsetY");
//        int privacyOffsetY_B = getInt(jsonObject, "privacyOffsetY_B");
//        int privacyOffsetX = getInt(jsonObject, "privacyOffsetX");
//        boolean isUseNormalWebActivity = getBoolean(jsonObject, "isUseNormalWebActivity");
//
//        String unCheckedImgPath = getString(jsonObject, "unCheckedImgPath");
//        String checkedImgPath = getString(jsonObject, "checkedImgPath");
//        boolean privacyState = getBoolean(jsonObject, "privacyState");
//        int privacyCheckBoxWidth = getInt(jsonObject, "privacyCheckBoxWidth");
//        int privacyCheckBoxHeight = getInt(jsonObject, "privacyCheckBoxHeight");
//        int privacyCheckBoxOffsetY = 0;
//        if(jsonObject.containsKey("privacyCheckBoxOffsetY")) {
//            privacyCheckBoxOffsetY = getInt(jsonObject, "privacyCheckBoxOffsetY");
//        }
//
//        String privacyTextViewTv1 = getString(jsonObject, "privacyTextViewTv1");
//        String privacyTextViewTv2 = getString(jsonObject, "privacyTextViewTv2");
//        String privacyTextViewTv3 = getString(jsonObject, "privacyTextViewTv3");
//        String privacyTextViewTv4 = getString(jsonObject, "privacyTextViewTv4");
//
//        String clauseNameOne = getString(jsonObject, "clauseNameOne");
//        String clauseUrlOne = getString(jsonObject, "clauseUrlOne");
//        String clauseNameTwo = getString(jsonObject, "clauseNameTwo");
//        String clauseUrlTwo = getString(jsonObject, "clauseUrlTwo");
//        String clauseNameThree = getString(jsonObject, "clauseNameThree");
//        String clauseUrlThree = getString(jsonObject, "clauseUrlThree");
//
////        String privacyClauseTextStrings = getString(jsonObject, "privacyClauseTextStrings");
//
//        int baseClauseColor = getInt(jsonObject, "baseClauseColor");
//        int clauseColor = getInt(jsonObject, "clauseColor");
//        int privacyClauseTextSize = getInt(jsonObject, "privacyClauseTextSize");
//
////        String privacyClauseBaseTextViewTypeface = getString(jsonObject, "privacyClauseBaseTextViewTypeface");
////        String privacyClauseTextViewTypeface = getString(jsonObject, "privacyClauseTextViewTypeface");
//
//        String privacyUnCheckedToastText = getString(jsonObject, "privacyUnCheckedToastText");
//
//        boolean privacyAddFrenchQuotes = getBoolean(jsonObject, "privacyAddFrenchQuotes");
//
//        int privacyTextGravity = getInt(jsonObject, "privacyTextGravity");
//
//
//        OneLoginThemeConfig.Builder builder = new OneLoginThemeConfig.Builder();
//
//        builder.setStatusBar(statusBarColor, navigationBarColor, isLightColor);
//        builder.setAuthNavLayout(navColor, authNavHeight, authNavTransparent, authNavGone);
//        builder.setAuthNavTextView(navText, navTextColor, navTextSize, navTextNormal, navWebViewText, navWebViewTextColor,navWebViewTextSize);
////        builder.setAuthNavTextViewTypeface(navTextTypeface, navWebViewTextTypeface);
//        builder.setAuthBGImgPath(authBGImgPath);
//        builder.setAuthBgVideoUri(authBgVideoUri);
//        builder.setDialogTheme(isDialogTheme, dialogWidth,dialogHeight, dialogX, dialogY, isDialogBottom, isWebViewDialogTheme);
//        builder.setAuthNavReturnImgView(navReturnImgPath, returnImgWidth, returnImgHeight, navReturnImgHidden, returnImgOffsetX);
//        builder.setAuthNavReturnImgView(navReturnImgPath, returnImgWidth, returnImgHeight, navReturnImgHidden, returnImgOffsetX, returnImgOffsetY);
//        builder.setLogoImgView(logoImgPath, logoWidth, logoHeight, logoHidden, logoOffsetY, logoOffsetY_B, logoOffsetX);
//        builder.setNumberView(numberColor, numberSize,numFieldOffsetY, numFieldOffsetY_B, numFieldOffsetX);
////        builder.setNumberViewTypeface(numberViewTypeface);
//        builder.setSwitchView(switchText, switchColor, switchSize, switchAccHidden, switchAccOffsetY,switchOffsetY_B, switchOffsetX);
////        builder.setSwitchViewTypeface(switchViewTypeface);
//        builder.setSwitchViewLayout(switchImgPath, switchWidth, switchHeight);
//        builder.setLogBtnLayout(loginImgPath, logBtnWidth, logBtnHeight, logBtnOffsetY, logBtnOffsetY_B, logBtnOffsetX);
//        builder.setLogBtnTextView(loginButtonText, loginButtonColor, logBtnTextSize);
////        builder.setLogBtnTextViewTypeface(logBtnTextViewTypeface);
//        builder.setLogBtnDisableIfUnChecked(disableBtnIfUnChecked);
//        builder.setLogBtnLoadingView(loadingView, loadingViewWidth, loadingViewHeight, loadingViewOffsetRight);
//        builder.setLogBtnLoadingView(loadingView, loadingViewWidth, loadingViewHeight, loadingViewOffsetRight, loadingViewOffsetY);
//        builder.setSloganView(sloganColor, sloganSize, sloganOffsetY, sloganOffsetY_B, sloganOffsetX);
////        builder.setSloganViewTypeface(sloganViewTypeface);
//        builder.setPrivacyLayout(privacyLayoutWidth, privacyOffsetY, privacyOffsetY_B, privacyOffsetX, isUseNormalWebActivity);
//        builder.setPrivacyCheckBox(unCheckedImgPath, checkedImgPath, privacyState, privacyCheckBoxWidth, privacyCheckBoxHeight);
//        builder.setPrivacyCheckBox(unCheckedImgPath, checkedImgPath, privacyState, privacyCheckBoxWidth, privacyCheckBoxHeight, privacyCheckBoxOffsetY);
//        builder.setPrivacyTextView(privacyTextViewTv1, privacyTextViewTv2, privacyTextViewTv3, privacyTextViewTv4);
//        builder.setPrivacyClauseText(clauseNameOne, clauseUrlOne, clauseNameTwo, clauseUrlTwo, clauseNameThree, clauseUrlThree);
////        builder.setPrivacyClauseTextStrings(String... privacyClauseTextStrings);
//        builder.setPrivacyClauseView(baseClauseColor, clauseColor, privacyClauseTextSize);
////        builder.setPrivacyClauseViewTypeface(privacyClauseBaseTextViewTypeface, privacyClauseTextViewTypeface);
//        builder.setPrivacyUnCheckedToastText(privacyUnCheckedToastText);
//        builder.setPrivacyAddFrenchQuotes(privacyAddFrenchQuotes);
//        builder.setPrivacyTextGravity(privacyTextGravity);
//
//        return builder.build();
//    }



    private OneLoginThemeConfig getThemeConfig(JSONObject jsonObject) {
        OneLoginBean b = null;
        try {
//            b = com.alibaba.fastjson.JSONObject.parseObject(themeJsonString, OneLoginBean.class);
            b = JsonUtils.parseObject(jsonObject, OneLoginBean.class);
        } catch (Exception e) {
            e.printStackTrace();
            Log.e(TAG, "getThemeConfig error, convert jsonObject to OneLoginBean failed, jsonObject=" + jsonObject);
        }
        if (b == null) {
            b = new OneLoginBean();
        }
        OneLoginThemeConfig.Builder builder = new OneLoginThemeConfig.Builder();

        builder.setStatusBar(b.statusBarColor, b.navigationBarColor, b.isLightColor);
        // 不调用设置含有颜色值的这个方法时就能保持深色模式自动匹配的能力
        if (jsonObject.has("navColor") || jsonObject.has("authNavHeight")
                || jsonObject.has("authNavTransparent") || jsonObject.has("authNavGone")) {
            builder.setAuthNavLayout(b.navColor, b.authNavHeight, b.authNavTransparent, b.authNavGone);
        }
        if (jsonObject.has("navText") || jsonObject.has("navTextColor")
                || jsonObject.has("navTextSize") || jsonObject.has("navWebTextNormal")
                || jsonObject.has("navWebText") || jsonObject.has("navWebTextColor")
                || jsonObject.has("navWebTextSize") || jsonObject.has("navTextMargin")) {
            builder.setAuthNavTextView(b.navText, b.navTextColor, b.navTextSize, b.navWebTextNormal,
                    b.navWebText, b.navWebTextColor, b.navWebTextSize, b.navTextMargin);
        }
        builder.setBlockReturnEvent(b.blockReturnKey, b.blockReturnBtn)
                .setAuthNavTextViewTypeface(getTypeface(b.navTextTypefaceName, b.navTextTypefaceBold, b.navTextTypefaceItalic),
                        getTypeface(b.navWebTextTypefaceName, b.navWebTextTypefaceBold, b.navWebTextTypefaceItalic));
        builder.setAuthBGImgPath(b.authBGImgPath)
                .setAuthBgVideoUri(b.authBgVideoUri)
                .setDialogTheme(b.isDialogTheme, b.dialogWidth, b.dialogHeight, b.dialogX, b.dialogY, b.isDialogBottom, b.isWebViewDialogTheme);
        if (b.returnImgCenterInVertical) {
            builder.setAuthNavReturnImgView(b.returnImgPath, b.returnImgWidth, b.returnImgHeight, b.returnImgHidden, b.returnImgOffsetX);
        } else {
            builder.setAuthNavReturnImgView(b.returnImgPath, b.returnImgWidth, b.returnImgHeight, b.returnImgHidden, b.returnImgOffsetX, b.returnImgOffsetY);
        }

        builder.setLogoImgView(b.logoImgPath, b.logoWidth, b.logoHeight, b.logoHidden, b.logoOffsetY, b.logoOffsetY_B, b.logoOffsetX);
        if (jsonObject.has("numberColor") || jsonObject.has("numberSize")
                || jsonObject.has("numberOffsetY") || jsonObject.has("numberOffsetY_B")
                || jsonObject.has("numberOffsetX")) {
            builder.setNumberView(b.numberColor, b.numberSize, b.numberOffsetY, b.numberOffsetY_B, b.numberOffsetX);
        }

        builder.setNumberViewTypeface(getTypeface(b.numberTypefaceName, b.numberTypefaceBold, b.numberTypefaceItalic));
        if (jsonObject.has("switchText") || jsonObject.has("switchColor")
                || jsonObject.has("switchSize") || jsonObject.has("switchHidden")
                || jsonObject.has("switchOffsetY") || jsonObject.has("switchOffsetY_B")
                || jsonObject.has("switchOffsetX")) {
            builder.setSwitchView(b.switchText, b.switchColor, b.switchSize, b.switchHidden, b.switchOffsetY, b.switchOffsetY_B, b.switchOffsetX);
        }
        builder.setSwitchViewTypeface(getTypeface(b.switchTypefaceName, b.switchTypefaceBold, b.switchTypefaceItalic))
                .setSwitchViewLayout(b.switchImgPath, b.switchWidth, b.switchHeight)
                .setLogBtnLayout(b.logBtnImgPath, b.logBtnUncheckedImgPath, b.logBtnWidth, b.logBtnHeight,
                        b.logBtnOffsetY, b.logBtnOffsetY_B, b.logBtnOffsetX)
                .setLogBtnTextView(b.logBtnText, b.logBtnColor, b.logBtnTextSize)
                .setLogBtnTextViewTypeface(getTypeface(b.logBtnTextTypefaceName, b.logBtnTextTypefaceBold, b.logBtnTextTypefaceItalic))
                .setLogBtnDisableIfUnChecked(b.disableBtnIfUnChecked);
        if (b.loadingViewCenterInVertical) {
            builder.setLogBtnLoadingView(b.loadingView, b.loadingViewWidth, b.loadingViewHeight, b.loadingViewOffsetRight);
        } else {
            builder.setLogBtnLoadingView(b.loadingView, b.loadingViewWidth, b.loadingViewHeight, b.loadingViewOffsetRight, b.loadingViewOffsetY);
        }
        builder.setSlogan(b.sloganVisible)
                .setSloganText(b.sloganText);
        if (jsonObject.has("sloganColor") || jsonObject.has("sloganSize")
                || jsonObject.has("sloganOffsetY") || jsonObject.has("sloganOffsetY_B")
                || jsonObject.has("sloganOffsetX")) {
            builder.setSloganView(b.sloganColor, b.sloganSize, b.sloganOffsetY, b.sloganOffsetY_B, b.sloganOffsetX);
        }
        builder.setSloganViewTypeface(getTypeface(b.sloganTypefaceName, b.sloganTypefaceBold, b.sloganTypefaceItalic))
                .setPrivacyLayout(b.privacyLayoutWidth, b.privacyOffsetY, b.privacyOffsetY_B,
                        b.privacyOffsetX, b.isUseNormalWebActivity, b.privacyLayoutGravity)
                .setPrivacyCheckBox(b.unCheckedImgPath, b.checkedImgPath, b.privacyState, b.privacyCheckBoxWidth,
                        b.privacyCheckBoxHeight, b.privacyCheckBoxOffsetY, b.privacyCheckBoxMarginRight);
        if (jsonObject.has("privacyTextViewTv1") || jsonObject.has("privacyTextViewTv2")
                || jsonObject.has("privacyTextViewTv3") || jsonObject.has("privacyTextViewTv4")) {
            builder.setPrivacyTextView(b.privacyTextViewTv1, b.privacyTextViewTv2, b.privacyTextViewTv3, b.privacyTextViewTv4);
        }
        if (jsonObject.has("clauseNameOne") || jsonObject.has("clauseUrlOne")
                || jsonObject.has("clauseNameTwo") || jsonObject.has("clauseUrlTwo")
                || jsonObject.has("clauseNameThree") || jsonObject.has("clauseUrlThree")) {
            builder.setPrivacyClauseText(b.clauseNameOne, b.clauseUrlOne, b.clauseNameTwo,
                    b.clauseUrlTwo, b.clauseNameThree, b.clauseUrlThree);
        }

        builder.setPrivacyClauseTextStrings(b.privacyClauseTextStrings);
        if (jsonObject.has("baseClauseColor") || jsonObject.has("clauseColor")
                || jsonObject.has("privacyClauseTextSize")) {
            builder.setPrivacyClauseView(b.baseClauseColor, b.clauseColor, b.privacyClauseTextSize);
        }
        builder.setPrivacyClauseViewTypeface(
                getTypeface(b.privacyClauseBaseTypefaceName, b.privacyClauseBaseTypefaceBold, b.privacyClauseBaseTypefaceItalic),
                getTypeface(b.privacyClauseTypefaceName, b.privacyClauseTypefaceBold, b.privacyClauseTypefaceItalic))
                .setPrivacyUnCheckedToastText(b.enableToast, b.privacyUnCheckedToastText)
                .setPrivacyAddFrenchQuotes(b.privacyAddFrenchQuotes)
                .setPrivacyTextGravity(b.privacyTextGravity)
                .setProtocolShakeStyle(getShakeStyleByIndex(b.protocolShakeStyle))
                .setLanguageType(getLanguageTypeByIndex(b.languageType));

        return builder.build();
    }

    private Typeface getTypeface(String fontName, boolean bold, boolean italic) {
        int style = Typeface.NORMAL;
        if (bold) {
            style |= Typeface.BOLD;
        }
        if (italic) {
            style |= Typeface.ITALIC;
        }
        Typeface typeface = Typeface.DEFAULT;
        try {
            typeface = Typeface.create(fontName, style);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return typeface;
    }

    private void addCustomView(Context context, JSONObject[] customList, OneLoginPluginCallback customViewCallback) throws Exception {
        if (customList == null || customList.length == 0) {
            return;
        }
//        Log.d(TAG, "addCustomView length = " + customList.length);
        for (int i = 0; i < customList.length; i++) {
            JSONObject widget = customList[i];
//            Log.d(TAG, "addCustomView i = " + i + ", widget = " + widget);
            addCustomView(widget, customViewCallback);
        }
    }

    private ViewRect getViewRect(Widget widget) {
        int left = widget.left;
        int top = widget.top;
        int right = widget.right;
        int bottom = widget.bottom;
        int width = widget.width;
        int height = widget.height;
        ViewRect rect = new ViewRect(left, top, right, bottom, width, height);
        return rect;
    }

    private RelativeLayout.LayoutParams getLayout(ViewRect rect) {
        return getLayout(rect.left, rect.top, rect.right, rect.bottom, rect.width, rect.height);
    }

    private RelativeLayout.LayoutParams getLayout(int left, int top, int right, int bottom, int width, int height) {
        RelativeLayout.LayoutParams layout = new RelativeLayout.LayoutParams(-2, -2);
        layout.addRule(RelativeLayout.CENTER_HORIZONTAL);
        if (left >= 0) {
            layout.leftMargin = DensityUtils.dip2px(context, left);
            layout.addRule(RelativeLayout.ALIGN_PARENT_LEFT);
        }
        if (top >= 0) {
            layout.topMargin = DensityUtils.dip2px(context, top);
        }
        if (right >= 0) {
            layout.rightMargin = DensityUtils.dip2px(context, right);
            layout.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);
        }
        if (bottom >= 0) {
            layout.bottomMargin = DensityUtils.dip2px(context, bottom);
            layout.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM);
        }
        if (width >= 0) {
            layout.width = DensityUtils.dip2px(context, width);
        }
        if (height >= 0) {
            layout.height = DensityUtils.dip2px(context, height);
        }
        return layout;
    }

    private void addCustomView(JSONObject widgetJson, final OneLoginPluginCallback customViewCallback) throws Exception {
        Widget widget = JsonUtils.parseObject(widgetJson, Widget.class);
        String type = widget.type;
        final String viewId = widget.viewId;
        final View customView;
        if ("TextView".equals(type)) {
            customView = getCustomTextView(context, widget);
        } else if ("ImageView".equals(type)) {
            customView = getCustomImageView(context, widget);
        } else if ("View".equals(type)) {
            customView = getCustomView(context, widget);
        } else {
            Log.e(TAG, "don't support widgetType-->" + type);
            return;
        }
        final boolean isClickable = customView.isClickable();
        AuthRegisterViewConfig authRegisterViewConfig = new AuthRegisterViewConfig.Builder()
                .setView(customView)
                .setRootViewId(AuthRegisterViewConfig.RootViewId.ROOT_VIEW_ID_BODY)
                .setCustomInterface(new CustomInterface() {
                    @Override
                    public void onClick(Context context) {
                        if (!isClickable) {
                            return;
                        }
                        Log.d(TAG, "View:" + viewId + " is clicked");
                        customViewCallback.onCustomViewClick(viewId);
                    }
                }).build();
//        Log.d(TAG, "addCustomView type = " + type + ", viewId = " + viewId);
        OneLoginHelper.with().addOneLoginRegisterViewConfig("custom_view_" + viewId, authRegisterViewConfig);
    }

    @SuppressLint("NewApi")
    private View getCustomView(Context context, Widget widget) {
        ViewRect rect = getViewRect(widget);
//        Log.d(TAG, "rect=" + rect);
        boolean isClickable = widget.clickable;
        View customView = new View(context);
        customView.setClickable(isClickable);
        customView.setBackgroundColor(widget.backgroundColor);
        customView.setLayoutParams(getLayout(rect));

        return customView;
    }

    @SuppressLint("NewApi")
    private View getCustomImageView(Context context, Widget widget) throws Exception {
        ViewRect rect = getViewRect(widget);
//        Log.d(TAG, "rect=" + rect);
        String backgroundImgPath = widget.backgroundImgPath;
        boolean isClickable = widget.clickable;
        ImageView customView = new ImageView(context);
        customView.setClickable(isClickable);
        customView.setBackgroundColor(widget.backgroundColor);
        if (!TextUtils.isEmpty(backgroundImgPath)) {
            int backgroundImgId = IDHelper.getDrawableId(backgroundImgPath, context);
            customView.setImageResource(backgroundImgId);
        }
        customView.setLayoutParams(getLayout(rect));

        return customView;
    }

    @SuppressLint("NewApi")
    private TextView getCustomTextView(Context context, Widget widget) throws Exception {
        ViewRect rect = getViewRect(widget);
//        Log.d(TAG, "rect=" + rect);
        String backgroundImgPath = widget.backgroundImgPath;
        boolean isClickable = widget.clickable;
        TextView customView = new TextView(context);
        customView.setClickable(isClickable);
        customView.setText(widget.text);
        customView.setTextColor(widget.textColor);
        if (widget.textSize > 0) {
            customView.setTextSize(widget.textSize);
        }
        customView.setBackgroundColor(widget.backgroundColor);
        if (!TextUtils.isEmpty(backgroundImgPath)) {
            int backgroundImgId = IDHelper.getDrawableId(backgroundImgPath, context);
            customView.setBackgroundResource(backgroundImgId);
        }
        customView.setGravity(17);
        customView.setLayoutParams(getLayout(rect));

        return customView;
    }

    /**
     * 配置页面布局
     *
     * @return config
     */
    private OneLoginThemeConfig initConfig(JSONObject jsonObject, JSONObject[] customList, OneLoginPluginCallback customViewCallback) throws Exception {
        OneLoginThemeConfig config = getThemeConfig(jsonObject);
//        Log.d(TAG, "initConfig start add custom view");
        addCustomView(context, customList, customViewCallback);
        return config;
    }

    private ProtocolShakeStyle getShakeStyleByIndex(int index) {
        if (index == ProtocolShakeStyle.NONE.ordinal()) {
            return ProtocolShakeStyle.NONE;
        } else if (index == ProtocolShakeStyle.SHAKE_HORIZONTAL.ordinal()) {
            return ProtocolShakeStyle.SHAKE_HORIZONTAL;
        } else {
            return ProtocolShakeStyle.SHAKE_VERTICAL;
        }
    }

    private OLLanguageType getLanguageTypeByIndex(int index) {
        if (index == OLLanguageType.SIMPLIFIED_CHINESE.ordinal()) {
            return OLLanguageType.SIMPLIFIED_CHINESE;
        } else if (index == OLLanguageType.TRADITIONAL_CHINESE.ordinal()) {
            return OLLanguageType.TRADITIONAL_CHINESE;
        } else {
            return OLLanguageType.ENGLISH;
        }
    }
}

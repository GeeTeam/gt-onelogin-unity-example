package com.geetest.onelogin.util;

import android.content.Context;

import com.geetest.onelogin.callback.OnePassPluginCallback;
import com.geetest.onelogin.wrapper.VerifyPhoneListenerWrapper;
import com.geetest.onepassv2.OnePassHelper;

public class OnePassUtils {
    private String appId;

    public OnePassUtils() {
    }

    public void init(Context context, String appId, int timeout) {
        this.appId = appId;
        OnePassHelper.with().init(context);
        OnePassHelper.with().setConnectTimeout(timeout);
    }

    public void getToken(String phone, OnePassPluginCallback verifyListener) {
        VerifyPhoneListenerWrapper verifyPhoneListener = new VerifyPhoneListenerWrapper(verifyListener);
        OnePassHelper.with().getToken(phone, appId, verifyPhoneListener);
    }
}

package com.geetest.onelogin.wrapper;

import com.geetest.onelogin.callback.OnePassPluginCallback;
import com.geetest.onepassv2.listener.OnePassListener;

import org.json.JSONObject;

public class VerifyPhoneListenerWrapper extends OnePassListener {

    private final OnePassPluginCallback verifyListener;

    public VerifyPhoneListenerWrapper(OnePassPluginCallback callback) {
        super();
        this.verifyListener = callback;
    }

    @Override
    public boolean onAlgorithm() {
        return verifyListener.onAlgorithm();
    }

    @Override
    public boolean onAlgorithmSelf() {
        return verifyListener.onAlgorithmSelf();
    }

    @Override
    public String onAlgorithmPhone(String s, String s1) {
        return verifyListener.onAlgorithmPhone(s, s1);
    }

    @Override
    public void onTokenFail(JSONObject jsonObject) {
        verifyListener.onTokenFail(jsonObject.toString());
    }

    @Override
    public void onTokenSuccess(JSONObject jsonObject) {
        verifyListener.onTokenSuccess(jsonObject.toString());
    }
}

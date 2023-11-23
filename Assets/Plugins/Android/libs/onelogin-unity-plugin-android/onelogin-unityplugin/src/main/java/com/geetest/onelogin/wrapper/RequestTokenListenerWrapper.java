package com.geetest.onelogin.wrapper;

import android.app.Activity;

import com.geetest.onelogin.callback.OneLoginPluginCallback;
import com.geetest.onelogin.listener.AbstractOneLoginListener;

import org.json.JSONObject;

public class RequestTokenListenerWrapper extends AbstractOneLoginListener {

    private OneLoginPluginCallback requestTokenListener;

    public RequestTokenListenerWrapper(OneLoginPluginCallback callback) {
        this.requestTokenListener = callback;
    }

    @Override
    public void onResult(JSONObject jsonObject) {
        requestTokenListener.onResult(jsonObject.toString());
    }

    @Override
    public void onPrivacyClick(String name, String url) {
        requestTokenListener.onPrivacyClick(name, url);
    }

    @Override
    public void onPrivacyCheckBoxClick(boolean isChecked) {
        requestTokenListener.onPrivacyCheckBoxClick(isChecked);
    }

    @Override
    public void onLoginButtonClick() {
        requestTokenListener.onLoginButtonClick();
    }

    @Override
    public void onLoginLoading() {
        requestTokenListener.onLoginLoading();
    }

    @Override
    public void onSwitchButtonClick() {
        requestTokenListener.onSwitchButtonClick();
    }

    @Override
    public void onBackButtonClick() {
        requestTokenListener.onBackButtonClick();
    }

    @Override
    public void onAuthActivityCreate(Activity activity) {
        requestTokenListener.onAuthActivityCreate(activity);
    }

    @Override
    public void onAuthWebActivityCreate(Activity activity) {
        requestTokenListener.onAuthWebActivityCreate(activity);
    }

    @Override
    public void onRequestTokenSecurityPhone(String phone) {
        requestTokenListener.onRequestTokenSecurityPhone(phone);
    }
}

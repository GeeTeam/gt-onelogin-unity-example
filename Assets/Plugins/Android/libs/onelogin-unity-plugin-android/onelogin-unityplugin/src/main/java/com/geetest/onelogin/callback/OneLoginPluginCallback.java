package com.geetest.onelogin.callback;

import android.app.Activity;

public interface OneLoginPluginCallback {
    void onResult(String result);

    void onPrivacyClick(String name, String url);

    void onPrivacyCheckBoxClick(boolean isChecked);

    void onLoginButtonClick();

    void onSwitchButtonClick();

    void onBackButtonClick();

    void onLoginLoading();

    void onAuthActivityCreate(Activity activity);

    void onAuthWebActivityCreate(Activity activity);

    void onRequestTokenSecurityPhone(String phone);

    void onCustomViewClick(String viewId);
}

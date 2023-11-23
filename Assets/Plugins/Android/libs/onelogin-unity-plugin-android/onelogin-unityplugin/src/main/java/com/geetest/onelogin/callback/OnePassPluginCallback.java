package com.geetest.onelogin.callback;

public interface OnePassPluginCallback {
    boolean onAlgorithm();

    void onTokenFail(String result);

    void onTokenSuccess(String result);

    boolean onAlgorithmSelf();

    String onAlgorithmPhone(String operator, String key);
}

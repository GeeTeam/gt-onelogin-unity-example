package com.geetest.onelogin.callback;

public interface MethodCallback {
    void onSuccess();
    void onFailure(String msg);
}

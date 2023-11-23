package com.geetest.onelogin;

import android.content.Context;
import android.os.AsyncTask;
import android.text.TextUtils;
import android.util.DisplayMetrics;
import android.util.Log;
import android.widget.Toast;

import com.geetest.onelogin.callback.MethodCallback;
import com.geetest.onelogin.callback.OneLoginPluginCallback;
import com.geetest.onelogin.callback.OnePassPluginCallback;
import com.geetest.onelogin.callback.RequestPostCallback;
import com.geetest.onelogin.util.OneLoginUtils;
import com.geetest.onelogin.util.OnePassUtils;
import com.geetest.onelogin.util.RequestPostTask;
import com.geetest.onelogin.util.ThreadFactoryUtils;
import com.geetest.onepassv2.OnePassHelper;

import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

import static com.geetest.onelogin.constant.Constants.*;

public class OneLoginManager {
    /**
     * 当前对象
     */
    private volatile static OneLoginManager oneLoginManager;

    private OneLoginUtils oneLoginUtils;
    private OnePassUtils onePassUtils;

    private ExecutorService postService;
    private RequestPostTask requestPostTask;

    /**
     * 空构造方法
     */
    private OneLoginManager() {
    }

    /**
     * 初始化
     *
     * @return <>当前的对象</>
     */
    public static OneLoginManager with() {
        if (oneLoginManager == null) {
            synchronized (OneLoginManager.class) {
                if (oneLoginManager == null) {
                    oneLoginManager = new OneLoginManager();
                }
            }
        }
        return oneLoginManager;
    }

    public void log(String msg) {
        Log.d(TAG, "[Unity]" + msg);
    }

    public void toast(String msg) {
        toast(msg, true);
    }

    public void toast(String msg, final boolean longToast) {
        final String toastMsg = TextUtils.isEmpty(msg) ? "$null" : msg;
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // 优化小米手机上 toast 带应用前缀的问题
                Toast toast = Toast.makeText(UnityPlayer.currentActivity.getApplicationContext(),
                        "", longToast ? Toast.LENGTH_LONG : Toast.LENGTH_SHORT);
                toast.setText(toastMsg);
                toast.show();
            }
        });
    }

    public String getScreenInfo() {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        DisplayMetrics dm = context.getResources().getDisplayMetrics();
        int screenWidth = dm.widthPixels;
        int screenHeight = dm.heightPixels;
        float density = dm.density;
        JSONObject jsonObject = new JSONObject();
        try {
            jsonObject.put("screenWidth", screenWidth);
            jsonObject.put("screenHeight", screenHeight);
            jsonObject.put("density", density);
        } catch (Exception ex) {

        }
        Log.d(TAG, "getScreenInfo result=" + jsonObject);
        return jsonObject.toString();
    }

    public void setLogEnabled(boolean openDebug) {
//        Log.i(TAG, "setLogEnabled openDebug=" + openDebug);
        OneLoginHelper.with().setLogEnable(openDebug);
        OnePassHelper.with().openDebug(openDebug);
    }

    public String sdkVersion() {
        Log.i(TAG, "sdkVersion");
        return OneLoginHelper.with().sdkVersion();
    }

    public void register(String appId, int timeout, MethodCallback registerListener) {
//        Log.i(TAG, "register appId=" + appId + ", timeout=" + timeout);
        boolean result = false;
        String msg;
        try {
            Context context = UnityPlayer.currentActivity.getApplicationContext();
//            Log.i(TAG, "register context=" + context + ", pkgName=" + context.getPackageName());
//            Log.i(TAG, "register appId=" + appId + ", timeout=" + timeout);
            oneLoginUtils = new OneLoginUtils();
            oneLoginUtils.init(context, appId);
            oneLoginUtils.register(timeout);
            result = true;
            msg = "register success";
        } catch (Exception e) {
            e.printStackTrace();
            msg = "register error:" + e.toString();
        }

        if (result) {
            registerListener.onSuccess();
        } else {
            registerListener.onFailure(getFailureJson(msg));
        }
    }

    public void requestToken(String themeJsonString, String[] customJsonString, OneLoginPluginCallback requestTokenListener) {
//        Log.i(TAG, "requestToken jsonString=" + themeJsonString);
//        Log.i(TAG, "requestToken customJsonString=" + Arrays.toString(customJsonString));
//        Log.i(TAG, "requestToken oneLoginUtils=" + oneLoginUtils);
        if (oneLoginUtils == null) {
            String msg = "Please call register before calling requestToken";
            requestTokenListener.onResult(getFailureJson(msg));
            return;
        }
        oneLoginUtils.requestToken(themeJsonString, customJsonString, requestTokenListener);
    }

    public void requestToken(String themeJsonString, OneLoginPluginCallback requestTokenListener) {
        requestToken(themeJsonString, null, requestTokenListener);
    }

    public void dismissAuthActivity() {
//        Log.i(TAG, "dismissAuthActivity");
        OneLoginHelper.with().dismissAuthActivity();
    }

    public boolean isPreGetTokenResultValidate() {
        Log.i(TAG, "isPreGetTokenResultValidate");
        return OneLoginHelper.with().isPreGetTokenResultValidate();
    }

    /**
     * 获取运营商类型
     * @return CM CT CU unknown
     */
    public String getCurrentCarrier() {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        return OneLoginHelper.with().getSimOperator(context);
    }

    /**
     * 服务条款是否勾选
     * @return isChecked 是否选中
     */
    public boolean isProtocolChecked() {
        return OneLoginHelper.with().isPrivacyChecked();
    }

    /**
     * 改变服务条款勾选框的状态
     * @param isChecked 是否选中
     */
    public void setProtocolCheckState(boolean isChecked) {
        OneLoginHelper.with().setProtocolCheckState(isChecked);
    }

    /**
     * 删除预取号的缓存
     */
    public void deletePreResultCache() {
        OneLoginHelper.with().deletePreResultCache();
    }


    public void initWithCustomID(String appId, int timeout, MethodCallback initListener) {
        boolean result = false;
        String msg;
        try {
            Context context = UnityPlayer.currentActivity.getApplicationContext();
            Log.i(TAG, "register context=" + context + ", pkgName=" + context.getPackageName());
            onePassUtils = new OnePassUtils();
            onePassUtils.init(context, appId, timeout);
            result = true;
            msg = "init success";
        } catch (Exception e) {
            e.printStackTrace();
            msg = "init error:" + e.toString();
        }

        if (result) {
            initListener.onFailure(msg);
        } else {
            initListener.onSuccess();
        }
    }

    public void verifyPhoneNumber(String phone, OnePassPluginCallback verifyListener) {
        if (onePassUtils == null) {
            String msg = "Please call initWithCustomID before calling verifyPhoneNumber";
            verifyListener.onTokenFail(getFailureJson(msg));
            return;
        }
        onePassUtils.getToken(phone, verifyListener);
    }

    public void destroy() {
        if (oneLoginUtils != null) {
            oneLoginUtils.cancel();
            oneLoginUtils = null;
        }
        onePassUtils = null;
    }

    public void requestPost(String url, String params, int timeout, final RequestPostCallback callback) {
        Log.d(TAG, "requestPost url=" + url + ", params=" + params + ", timeout=" + timeout);
        if (postService == null) {
            postService = Executors.newCachedThreadPool();
        }
        ScheduledExecutorService timeoutService = new ScheduledThreadPoolExecutor(1, ThreadFactoryUtils.getInstance());
        timeoutService.schedule(new Runnable() {
            @Override
            public void run() {
                if (requestPostTask.isFinished() || requestPostTask.isCancelled()) {

                } else {
                    requestPostTask.setFinished(true);
                    callback.onResult(getFailureJson("request time out"));
                    cancelExecutorService(requestPostTask);
                }
            }
        }, timeout, TimeUnit.MILLISECONDS);
        requestPostTask = new RequestPostTask(timeout, callback, timeoutService);
        requestPostTask.executeOnExecutor(postService, url, params);
    }

    private void cancelExecutorService(AsyncTask task) {
        if (task != null && !task.isCancelled()
                && task.getStatus() == AsyncTask.Status.RUNNING) {
            task.cancel(true);
        }
    }

    private String getFailureJson(String msg) {
        JSONObject jsonObject = new JSONObject();
        try {
            jsonObject.put(UNITY_RESULT_STATUS, FAILURE);
            jsonObject.put(UNITY_RESULT_MESSAGE, msg);
        } catch (Exception e) {

        }
        return jsonObject.toString();
    }
}

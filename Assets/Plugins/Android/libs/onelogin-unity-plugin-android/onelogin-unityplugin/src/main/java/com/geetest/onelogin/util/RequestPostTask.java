package com.geetest.onelogin.util;

import android.os.AsyncTask;
import com.geetest.onelogin.callback.RequestPostCallback;
import java.util.concurrent.ScheduledExecutorService;

/**
 * RequestPostTask的请求异步检查任务
 */
public class RequestPostTask extends AsyncTask<String, Void, String> {

    private RequestPostCallback callback;
    private long startTime;
    private long timeout;
    private ScheduledExecutorService timeoutService;

    private volatile boolean isFinished = false;

    public RequestPostTask(long timeout, RequestPostCallback callback, ScheduledExecutorService timeoutService) {
        this.callback = callback;
        this.timeout = timeout;
        this.timeoutService = timeoutService;
    }

    @Override
    protected String doInBackground(String... params) {
        if (isCancelled()) {
            return null;
        }
        this.startTime = System.currentTimeMillis();
        isFinished = false;
        String url = params[0];
        String param = params[1];
        return HttpUtils.requestNetwork(url, param);
    }

    @Override
    protected void onPostExecute(String s) {
        if (callback != null && !isFinished) {
            callback.onResult(s);
        }
        isFinished = true;
        stopTimeoutService();
    }

    @Override
    protected void onCancelled() {
        super.onCancelled();
        isFinished = true;
    }

    public void stopTimeoutService() {
        if (timeoutService != null && !timeoutService.isShutdown()) {
            timeoutService.shutdownNow();
        }
    }

    public boolean isFinished() {
        return isFinished;
    }

    public void setFinished(boolean finished) {
        isFinished = finished;
    }

    private boolean isTimeout() {
        return System.currentTimeMillis() - startTime >= timeout;
    }
}

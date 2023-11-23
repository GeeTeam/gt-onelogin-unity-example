package com.geetest.onelogin.util;

import android.app.Activity;
import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.view.View;

/**
 * 资源获取方法规整
 *
 * @author geetest 谷闹年
 * @date 2019/3/13
 */
public class IDHelper {

    /**
     * 获取布局
     *
     * @param name    具体命名
     * @param context 上下文
     * @return 布局
     * @throws Exception 错误上报
     */
    public static View getLayoutForView(String name, Activity context) throws Exception {
        ApplicationInfo appInfo = context.getApplicationInfo();
        int view = context.getResources().getIdentifier(name, "layout", appInfo.packageName);
        return context.getLayoutInflater().inflate(view, null);
    }

    /**
     * 获取ID
     *
     * @param name    具体命名
     * @param context 上下文
     * @return ID
     * @throws Exception 错误上报
     */
    public static int getLayoutForId(String name, Context context) throws Exception {
        ApplicationInfo appInfo = context.getApplicationInfo();
        return context.getResources().getIdentifier(name, "layout", appInfo.packageName);
    }

    /**
     * 获取ID
     *
     * @param name    具体命名
     * @param context 上下文
     * @return ID
     * @throws Exception 错误上报
     */
    public static int getId(String name, Context context) throws Exception {
        ApplicationInfo appInfo = context.getApplicationInfo();
        return context.getResources().getIdentifier(name, "id", appInfo.packageName);
    }

    /**
     * 获取Drawable
     *
     * @param name    具体命名
     * @param context 上下文
     * @return Drawable
     * @throws Exception 错误上报
     */
    public static int getDrawableId(String name, Context context) throws Exception {
        ApplicationInfo appInfo = context.getApplicationInfo();
        return context.getResources().getIdentifier(name, "drawable", appInfo.packageName);
    }

    /**
     * 获取 Animation ID
     * @param name      具体命名
     * @param context   上下文
     * @return Animation ID
     * @throws Exception 错误上报
     */
    public static int getAnimForId(String name, Context context) throws Exception {
        ApplicationInfo appInfo = context.getApplicationInfo();
        return context.getResources().getIdentifier(name, "anim", appInfo.packageName);
    }
}

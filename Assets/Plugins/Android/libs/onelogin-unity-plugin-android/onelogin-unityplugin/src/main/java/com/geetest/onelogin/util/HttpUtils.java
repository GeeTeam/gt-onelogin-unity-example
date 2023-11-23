package com.geetest.onelogin.util;

import android.util.Log;

import com.geetest.onelogin.constant.Constants;

import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Map;

public class HttpUtils {

    private static final int TIME_OUT = 15000;

    /**
     * 数据流
     *
     * @param inputStream
     * @return
     */
    private static String dealResponseResult(InputStream inputStream) {
        String resultData;
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        byte[] data = new byte[1024];
        int len;
        try {
            while ((len = inputStream.read(data)) != -1) {
                byteArrayOutputStream.write(data, 0, len);
            }
            inputStream.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        resultData = new String(byteArrayOutputStream.toByteArray());
        return resultData;
    }

    /**
     * 拼接方法
     *
     * @param params
     * @return
     */
    private static StringBuffer getRequestData(Map<String, String> params) {
        StringBuffer stringBuffer = new StringBuffer();
        try {
            for (Map.Entry<String, String> entry : params.entrySet()) {
                stringBuffer.append(entry.getKey()).append("=")
                        .append(entry.getValue())
                        .append("&");
            }
            stringBuffer.deleteCharAt(stringBuffer.length() - 1);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return stringBuffer;
    }

    /**
     * url获取
     *
     * @param urlString
     * @return
     */
    private static URL getValidateURL(String urlString) {
        try {
            return new URL(urlString);
        } catch (Exception e) {
        }
        return null;
    }

    public static String requestNetwork(String string, String jsonParams) {
        URL url = getValidateURL(string);

        HttpURLConnection httpURLConnection = null;
        try {
            httpURLConnection = (HttpURLConnection) url.openConnection();

            httpURLConnection.setDoInput(true);
            httpURLConnection.setDoOutput(true);
            httpURLConnection.setRequestMethod("POST");
            httpURLConnection.setUseCaches(false);
            httpURLConnection.connect();
            if (jsonParams != null) {
                OutputStream outputStream = httpURLConnection.getOutputStream();
                outputStream.write(jsonParams.getBytes());
                outputStream.flush();
                outputStream.close();
            }
            int responseCode = httpURLConnection.getResponseCode();
            if (responseCode == HttpURLConnection.HTTP_OK) {
                InputStream intStream = httpURLConnection.getInputStream();
                return dealResponseResult(intStream);
            } else {
                return "url:" + string + ",responseCode:" + responseCode;
            }
        } catch (Exception e) {
            return "url:" + string + ",error:" + e.toString();
        } finally {
            if (httpURLConnection != null) {
                httpURLConnection.disconnect();
            }
        }
    }

    public static String requestNetwork(String string, JSONObject params, Map<String, String> map) {
        URL url = getValidateURL(string);

        HttpURLConnection httpURLConnection = null;
        try {
            httpURLConnection = (HttpURLConnection) url.openConnection();

            httpURLConnection.setConnectTimeout(TIME_OUT);
            httpURLConnection.setReadTimeout(TIME_OUT);
            httpURLConnection.setDoInput(true);
            httpURLConnection.setDoOutput(true);
            httpURLConnection.setRequestMethod("POST");
            httpURLConnection.setUseCaches(false);
            httpURLConnection.connect();
            if (params != null) {
                OutputStream outputStream = httpURLConnection.getOutputStream();
                outputStream.write(params.toString().getBytes());
                outputStream.flush();
                outputStream.close();
            }
            if (map != null) {
                byte[] data = getRequestData(map).toString().getBytes();
                OutputStream outputStream = httpURLConnection.getOutputStream();
                outputStream.write(data);
                outputStream.flush();
                outputStream.close();
            }
            int responseCode = httpURLConnection.getResponseCode();
            if (responseCode == HttpURLConnection.HTTP_OK) {
                InputStream intStream = httpURLConnection.getInputStream();
                return dealResponseResult(intStream);
            } else {
                Log.i(Constants.TAG, "url:" + string + ",responseCode:" + responseCode);
                return "";
            }


        } catch (Exception e) {
            Log.i(Constants.TAG, "url:" + string + ",error:" + e.toString());
            return "";
        } finally {
            if (httpURLConnection != null) {
                httpURLConnection.disconnect();
            }
        }
    }
}

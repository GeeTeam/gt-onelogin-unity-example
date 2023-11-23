package com.geetest.onelogin.util;

import org.json.JSONArray;

import java.lang.reflect.Field;
import java.lang.reflect.Type;
import java.util.Iterator;

public class JsonUtils {
    //    public static com.alibaba.fastjson.JSONObject oJsonToFJson(org.json.JSONObject jsonObject) {
//        com.alibaba.fastjson.JSONObject newJson = com.alibaba.fastjson.JSONObject.parseObject(jsonObject.toString());
//        return newJson;
//    }
//
//    public static <T> T parseObject(String str, Class<T> clazz) {
//        return com.alibaba.fastjson.JSONObject.parseObject(str, clazz);
//    }
//
//    public static <T> T parseObject(org.json.JSONObject jsonObject, Class<T> clazz) {
//        return parseObject(jsonObject.toString(), clazz);
//    }
//
//    public static <T> T parseObject(com.alibaba.fastjson.JSONObject jsonObject, Class<T> clazz) {
//        return com.alibaba.fastjson.JSON.toJavaObject(jsonObject, clazz);
//    }
//
    public static <T> T parseObject(org.json.JSONObject jsonObject, Class<T> clazz) {
        T t = null;
        try {
            t = clazz.newInstance();
            Iterator<String> keys = jsonObject.keys();
            while (keys.hasNext()) {
                String key = keys.next();
                Field field = clazz.getDeclaredField(key);
                if (field != null) {
                    field.setAccessible(true);
                    Type type = field.getType();
                    if (type.equals(String.class)) {
                        field.set(t, jsonObject.getString(key));
                    } else if (type.equals(int.class)) {
                        field.set(t, jsonObject.getInt(key));
                    } else if (type.equals(char.class)) {
                        field.set(t, jsonObject.getString(key).charAt(0));
                    } else if (type.equals(short.class)) {
                        field.set(t, jsonObject.getInt(key));
                    } else if (type.equals(long.class)) {
                        field.set(t, jsonObject.getLong(key));
                    } else if (type.equals(float.class)) {
                        field.set(t, jsonObject.getDouble(key));
                    } else if (type.equals(byte.class)) {
                        field.set(t, jsonObject.getInt(key));
                    } else if (type.equals(boolean.class)) {
                        field.set(t, jsonObject.getBoolean(key));
                    } else if (type.equals(String[].class)) {
                        JSONArray jsonArray = jsonObject.optJSONArray(key);
                        if (jsonArray != null && jsonArray.length() != 0) {
                            String[] strArray = new String[jsonArray.length()];
                            for (int i = 0; i < jsonArray.length(); i++) {
                                strArray[i] = jsonArray.getString(i);
                            }
                            field.set(t, strArray);
                        }
                    }
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return t;
    }
}

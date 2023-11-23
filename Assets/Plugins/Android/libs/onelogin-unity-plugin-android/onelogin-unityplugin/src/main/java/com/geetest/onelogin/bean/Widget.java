package com.geetest.onelogin.bean;

public class Widget {
    public String viewId;
    public String type;
    public int left;
    public int top;
    public int right;
    public int bottom;
    public int width;
    public int height;
    public boolean clickable;
    public String text;
    public int textSize;
    public int textColor;
    public int backgroundColor;
    public String backgroundImgPath;

    @Override
    public String toString() {
        return "Widget{" +
                "viewId='" + viewId + '\'' +
                ", type='" + type + '\'' +
                ", left=" + left +
                ", top=" + top +
                ", right=" + right +
                ", bottom=" + bottom +
                ", width=" + width +
                ", height=" + height +
                ", clickable=" + clickable +
                ", text='" + text + '\'' +
                ", textSize=" + textSize +
                ", textColor=" + textColor +
                ", backgroundColor=" + backgroundColor +
                ", backgroundImgPath='" + backgroundImgPath + '\'' +
                '}';
    }
}

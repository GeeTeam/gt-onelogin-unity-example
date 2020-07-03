using System;

/// <summary>
/// 一键登录取号结果 Model, 参数跟原生 SDK 返回的 JSON 格式数据保持一致
/// operator 字段因与 C# 关键字冲突，此处换成 operator_type，注意解析时需要替换一下
/// </summary>
public class OLResult {
    public int status;
    public string errorCode;
    public string msg;
    public string metadata;
    public string process_id;
    public string app_id;
    public string operator_type;
    public string clienttype;
    public string authcode;
    public string token;
    public string sdk;
    public string release;
    public string model;

    public string id_2_sign;

    public override string ToString() {
        return "{status=" + status +
            ", errorCode=" + errorCode +
            ", msg=" + msg +
            ", metadata=" + metadata +
            ", process_id=" + process_id +
            ", app_id=" + app_id +
            ", id_2_sign=" + id_2_sign +
            ", operator_type=" + operator_type +
            ", clienttype=" + clienttype +
            ", authcode=" + authcode +
            ", token=" + token +
            ", sdk=" + sdk +
            ", release=" + release +
            ", model=" + model + "}";
    }
}
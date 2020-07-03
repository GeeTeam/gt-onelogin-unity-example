using System;

/// <summary>
/// 本机号码认证取号结果 Model, 参数跟原生 SDK 返回的 JSON 格式数据保持一致
/// </summary>
public class OPResult {
    public string code;
    public string process_id;
    public string accesscode;
    public string phone;
    public string custom_id;
    public string metadata;
    public string real_op;
    public string op;
    public string clienttype;
    public string sdk;

    public string id_2_sign;

    public override string ToString() {
        return "{code=" + code +
            ", process_id=" + process_id +
            ", accesscode=" + accesscode +
            ", phone=" + phone +
            ", custom_id=" + custom_id +
            ", id_2_sign=" + id_2_sign +
            ", metadata=" + metadata +
            ", real_op=" + real_op +
            ", op=" + op +
            ", clienttype=" + clienttype +
            ", sdk=" + sdk + "}";
    }
}
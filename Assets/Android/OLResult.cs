using System;

/// <summary>
/// һ����¼ȡ�Ž�� Model, ������ԭ�� SDK ���ص� JSON ��ʽ���ݱ���һ��
/// operator �ֶ����� C# �ؼ��ֳ�ͻ���˴����� operator_type��ע�����ʱ��Ҫ�滻һ��
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
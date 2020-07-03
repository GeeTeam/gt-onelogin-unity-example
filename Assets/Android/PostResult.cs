using System;

/// <summary>
/// 演示 Demo check_phone、check_gateway 接口请求结果解析 model
/// 参数格式仅限 demo，具体请结合后端服务器修改调整
/// </summary>
public class PostResult {
    public string status;
    public bool charge;
    public string result;
    public string msg;
    public string error_msg;
}
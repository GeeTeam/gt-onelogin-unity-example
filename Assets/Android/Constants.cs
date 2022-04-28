using System;

/// <summary>
/// Demo 常量配置
/// </summary>
public class Constants {
    /**
     * 服务器配置的verifyUrl接口地址<>需要用到服务SDK</>
     * 当前验证地址仅供演示Demo使用，实际使用请配置为自定义校验的服务端地址
     */
    public const string BASE_URL = "https://onepass.geetest.com";
    /**
     * *******************************************************************************
     * OneLogin 与 OnePass 属于不同的产品，注意产品 APPID 不可混用，请在后台分别创建对应的应用
     * *******************************************************************************
     */
    /**
     * 后台申请的 OneLogin APP_ID
     * 当前APP_ID仅供演示Demo使用，如果修改了应用包名或者签名信息，请使用申请的APP_ID
     * 谨记：APP_ID需绑定相关的包名和包签名(提供这两项信息从后台申请APP_ID)
     */
    public const string APP_ID_OL = "b41a959b5cac4dd1277183e074630945";
    /**
     * 后台申请的 OnePass APP_ID
     * 当前APP_ID仅供演示Demo使用，如果修改了应用包名或者签名信息，请使用申请的APP_ID
     * 谨记：APP_ID需绑定相关的包名和包签名(提供这两项信息从后台申请APP_ID)
     */
    public const string APP_ID_OP = "3996159873d7ccc36f25803b88dda97a";
    /**
     * 后台配置的服务校验接口，该地址仅限Demo使用，如果修改了Demo包名与APP_ID，请勿使用该地址
     */
    public const string CHECK_PHONE_URL = BASE_URL + "/onelogin/result";

    /**
     * CheckGateWay接口
     * 网关登录校验
     */
    public const string CHECK_GATE_WAY = BASE_URL + "/v2.0/result";

    /**
     * 请求成功的响应值
     */
    public const string SUCCESS_CODE = "200";
}
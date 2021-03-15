using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkUrl 
{
    private const string ServerGameUrl = "https://listeninsoftv.ucl.ac.uk/php/game/";
    private const string ServerDataUrl = "https://listeninsoftv.ucl.ac.uk/php/data/";
    private const string ServerLoginUrl = "https://listeninsoftv.ucl.ac.uk/php/login/";

    public static string UrlSqlRegister = string.Concat(ServerLoginUrl, "register.php");
    public static string UrlSqlLogin = string.Concat(ServerLoginUrl, "login.php");
    public static string UrlSqlSubscriptionCheck = string.Concat(ServerLoginUrl, "check_subscription.php");
    public static string UrlSqlSubscriptionUpdate = string.Concat(ServerLoginUrl, "update_subscription.php");

    public static string ServerUrlDataInput = string.Concat(ServerDataUrl, "data_input_no_header.php");
    public static string ServerUrlFileCheck = string.Concat(ServerDataUrl, "file_check.php");
    public static string ServerUrlFileConsistencyCheck = string.Concat(ServerDataUrl, "file_consistency_check.php");
    public static string ServerUrlSubscriptionCheck = string.Concat(ServerLoginUrl, "check_subscription.php");
    public static string ServerUrlExtendSubscription = string.Concat(ServerLoginUrl, "extend_subscription.php");
    public static string ServerUrlSelectPromoCode = string.Concat(ServerLoginUrl, "select_promo_code.php");

    public static string SqlGetGameUserProfile = string.Concat(ServerGameUrl, "select_user_profile.php");
    public static string SqlSetGameUserProfile = string.Concat(ServerGameUrl, "update_user_profile.php");
    public static string ServerUrlUploadFile = string.Concat(ServerGameUrl, "upload_file_2.php");
    public static string ServerUrlGetFile = string.Concat(ServerGameUrl, "get_file.php");
    public static string ServerUrlGetCurrentBlockFile = string.Concat(ServerGameUrl, "get_current_block.php");
    public static string ServerUrlDeleteFile = string.Concat(ServerGameUrl, "delete_file.php");

    //Andrea
    public static string SqlGetUserBasketTracking = string.Concat(ServerGameUrl, "select_user_basket_tracking.php");
    public static string SqlSetUserBasketTracking = string.Concat(ServerGameUrl, "update_user_basket_tracking.php");
    public static string ServerUrlUploadLogFile = string.Concat(ServerGameUrl, "upload_log_file.php");

}

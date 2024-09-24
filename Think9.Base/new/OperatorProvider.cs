using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Think9.Models;
using Think9.Util.Helper;

namespace Think9.Services.Base
{
    public class OperatorProvider
    {
        public WebHelper WebHelper;

        public OperatorProvider(HttpContext httpContext)
        {
            WebHelper = new WebHelper(httpContext);
        }

        private string LoginUserKey = "Loginkey";
        private string LoginProvider = Configs.GetValue("LoginProvider");

        public CurrentUserEntity GetCurrent()
        {
            CurrentUserEntity CurrentUser = new CurrentUserEntity();
            if (LoginProvider == "Cookie")
            {
                CurrentUser = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginUserKey).ToString()).ToObject<CurrentUserEntity>();
            }
            else
            {
                CurrentUser = DESEncrypt.Decrypt(WebHelper.GetSession(LoginUserKey).ToString()).ToObject<CurrentUserEntity>();
            }
            return CurrentUser;
        }

        public void AddCurrent(CurrentUserEntity operatorModel)
        {
            if (LoginProvider == "Cookie")
            {
                WebHelper.WriteCookie(LoginUserKey, DESEncrypt.Encrypt(operatorModel.ToJson()), 180);
            }
            else
            {
                WebHelper.WriteSession(LoginUserKey, DESEncrypt.Encrypt(operatorModel.ToJson()));
            }
            WebHelper.WriteCookie("Mac", Md5.md5(GetMacByNetworkInterface().ToJson(), 32));
        }

        public void RemoveCurrent()
        {
            if (LoginProvider == "Cookie")
            {
                WebHelper.RemoveCookie(LoginUserKey.Trim());
            }
            else
            {
                WebHelper.RemoveSession(LoginUserKey.Trim());
            }
        }

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }
    }
}
namespace Think9.Models
{
    public class DevEntity
    {
        /// <summary>
        /// 初始化密码
        /// </summary>
        public string InitUserPwd { get; set; }

        /// <summary>
        /// 上传文件大小
        /// </summary>
        public string UploadFileSize { get; set; }

        /// <summary>
        /// 上传文件类型
        /// </summary>
        public string UploadFileType { get; set; }

        /// <summary>
        /// 登录提供者模式：Session、Cookie
        /// </summary>
        public string LoginProvider { get; set; }

        public DevEntity GetDevInfo()
        {
            return new DevEntity
            {
                InitUserPwd = Configs.GetValue("InitUserPwd"),
                UploadFileSize = Configs.GetValue("UploadFileSize"),
                UploadFileType = Configs.GetValue("UploadFileType"),
                LoginProvider = Configs.GetValue("LoginProvider")
            };
        }

        public void SetDevInfo(DevEntity model)
        {
            Configs.SetValue("InitUserPwd", model.InitUserPwd);
            Configs.SetValue("UploadFileSize", model.UploadFileSize);
            Configs.SetValue("UploadFileType", model.UploadFileType);
            Configs.SetValue("LoginProvider", model.LoginProvider);
        }
    }
}
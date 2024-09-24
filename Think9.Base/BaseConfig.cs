using System.IO;

namespace Think9.Services.Base
{
    public static class BaseConfig
    {
        /// <summary>
        /// 字符分割 用于多选项的分割等 多选项格式默认为split+选项1+split+选项2+split...
        /// </summary>
        public const string ComSplit = "ξ";

        /// <summary>
        /// //图片不存在时的替代
        /// </summary>
        /// <returns></returns>
        public static string GetImgNoExistPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\nonexistent.gif");
        }

        /// <summary>
        /// 用户图片所在文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetUserImgPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserImg\\");
        }
    }
}
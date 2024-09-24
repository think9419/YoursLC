using Think9.Util.Global;

namespace Think9.Services.Basic
{
    public class CreatCode
    {
        public static string Creat(string appId, string password, string type, string name)
        {
            string str = "";

            if (type == "英文[空格]")
            {
                str = Think9.TransAPI.BaiduTrans.translation(appId, password, name, true);
            }
            if (type == "英文")
            {
                str = Think9.TransAPI.BaiduTrans.translation(appId, password, name, false);
            }
            if (type == "首拼")
            {
                str = Think9.NPinyin.Pinyin.GetInitials(name);
            }
            if (type == "全拼")
            {
                str = Think9.NPinyin.Pinyin.GetPinyin(name, false);
            }

            return str;
        }

        //生成唯一编号
        public static string NewGuid()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 表示全局唯一标识符 (GUID)。
        /// </summary>
        /// <returns></returns>
        public static string GuId()
        {
            return IDGenerator.NextID().ToString();
        }

        /// <summary>
        /// 生成唯一 ID
        /// </summary>
        /// <param name="idGeneratorOptions"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static object NextID(object idGeneratorOptions)
        {
            return ((IDistributedIDGenerator)GlobalContext.ServiceProvider.GetService(typeof(IDistributedIDGenerator))).Create(idGeneratorOptions);
        }
    }
}
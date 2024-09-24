namespace Think9.Services.Base
{
    public class MenuIdService
    {
        /// <summary>
        /// 将生成的菜单id还原
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetOriginalMenuId(int? id)
        {
            int menuID = 0;
            if (id != null)
            {
                if (id.Value.ToString().Length > 7)
                {
                    string str = id.Value.ToString().Substring(2, id.Value.ToString().Length - 3);
                    if ((int.Parse(str) - 37415) % 13 == 0)
                    {
                        menuID = (int.Parse(str) - 37415) / 13;
                    }
                }
            }
            return menuID;
        }

        /// <summary>
        /// 菜单id生成为int，避免连续id被猜中
        /// </summary>
        /// <param name="menuID"></param>
        /// <returns></returns>
        public static int ChangeMenuId(int menuID)
        {
            System.Random rd = new System.Random();
            int id = menuID * 13 + 37415;
            return int.Parse(rd.Next(1, 9).ToString() + "5" + id.ToString() + rd.Next(0, 9).ToString());
        }
    }
}
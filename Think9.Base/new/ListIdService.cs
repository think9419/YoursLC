namespace Think9.Services.Base
{
    public class ListIdService
    {
        /// <summary>
        /// 将id还原
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static long GetOriginalListId(string id, string isOriginal = "n")
        {
            if (isOriginal == "y")
            {
                return long.Parse(id);
            }
            long listId = 0;
            if (id != null)
            {
                if (id.Length >= 7)
                {
                    string str = id.Substring(2, id.Length - 2);
                    if ((long.Parse(str) - 37415) % 773 == 0)
                    {
                        listId = (long.Parse(str) - 37415) / 773;
                    }
                }
            }
            return listId;
        }

        /// <summary>
        /// 改变listid，连续的id容易被猜，觉得简单的可以自己改
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public static long ChangeListId(long listId)
        {
            System.Random rd = new System.Random();
            long id = listId * 773 + 37415;
            return long.Parse(rd.Next(1, 9).ToString() + "5" + id.ToString());
        }
    }
}
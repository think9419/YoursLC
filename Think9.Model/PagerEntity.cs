using System.Collections.Generic;

namespace Think9.Models
{
    public class PagerEntity
    {
        public static dynamic Paging(IEnumerable<dynamic> list, long total)
        {
            return new { code = 0, msg = "", count = total, data = list };
        }

        public static dynamic Paging_old(IEnumerable<dynamic> list, long total)
        {
            DataListEntity dtlist = new DataListEntity();

            dtlist.code = 0;
            dtlist.msg = "";
            dtlist.count = total;
            dtlist.list = list;

            return dtlist;
        }
    }
}
using System.Collections.Generic;

namespace Think9.Models
{
    public class DataListEntity
    {
        public int code { get; set; }
        public string msg { get; set; }
        public long count { get; set; }
        public IEnumerable<dynamic> list { get; set; }
    }
}
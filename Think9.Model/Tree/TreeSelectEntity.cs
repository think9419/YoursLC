using System.Collections.Generic;

namespace Think9.Models
{
    public class TreeSelectEntity
    {
        public string id { get; set; }
        public string name { get; set; }

        //public string code { get; set; }
        public bool open { get; set; }

        public IEnumerable<TreeSelectEntity> children { get; set; }
    }
}
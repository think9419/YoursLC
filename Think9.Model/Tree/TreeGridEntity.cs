using System.Collections.Generic;

namespace Think9.Models
{
    public class TreeGridEntity
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string title { get; set; }
        public object self { get; set; }
        public object checkArr { get; set; }
        public bool? disabled { get; set; }
        public bool? spread { get; set; }

        //public bool? checked { get; set; }
        public List<TreeGridEntity> children { get; set; }
    }
}
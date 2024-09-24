using System.Collections.Generic;

namespace Think9.Models
{
    public class TreeEntity
    {
        public int id { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string fontFamily { get; set; }
        public string icon { get; set; }
        public IEnumerable<TreeEntity> children { get; set; }
    }
}
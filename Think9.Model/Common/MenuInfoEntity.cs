using System.Collections.Generic;

namespace Think9.Models
{
    public class MenuInfoEntity
    {
        public string title { get; set; }
        public string href { get; set; }
        public string icon { get; set; }
        public string target { get; set; }

        public int id { get; set; }
        public string fontFamily { get; set; }

        public List<MenuInfoEntity> child { get; set; }
    }
}
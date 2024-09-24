using System.Collections.Generic;

namespace Think9.Models
{
    public class InitEntity
    {
        public HomeInfoEntity homeInfo { get; set; }
        public LogoInfoEntity logoInfo { get; set; }
        public List<MenuInfoEntity> menuInfo { get; set; }
    }
}
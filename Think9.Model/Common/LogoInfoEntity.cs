namespace Think9.Models
{
    public class LogoInfoEntity
    {
        public string title { get; set; }
        public string href { get; set; }
        public string image { get; set; }

        public LogoInfoEntity()
        {
            title = "YoursLC";
            href = "";
            image = "../images/logo.png";
        }
    }
}
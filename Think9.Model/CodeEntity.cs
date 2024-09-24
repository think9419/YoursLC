using DapperExtensions;

namespace Think9.Models
{
    public class CodeBuildEntity
    {
        [Computed]
        public string TbId { get; set; }

        [Computed]
        public string Controllers { get; set; }

        [Computed]
        public string Services { get; set; }

        [Computed]
        public string Models { get; set; }

        [Computed]
        public string Views_Index { get; set; }

        [Computed]
        public string Views_Form { get; set; }

        [Computed]
        public string Views_Detail { get; set; }

        [Computed]
        public string Views_Main_Pop { get; set; }

        [Computed]
        public string Views_Grid_Pop { get; set; }

        [Computed]
        public string Self_JS { get; set; }

        [Computed]
        public string Reports_Rdlc { get; set; }

        [Computed]
        public string CreatDataTable { get; set; }
    }
}
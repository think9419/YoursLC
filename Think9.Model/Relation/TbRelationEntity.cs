using DapperExtensions;

namespace Think9.Models
{
    [Table("tbrelation")]
    public class TbRelationEntity
    {
        public int RelationId { get; set; }
        public string TbID { get; set; }
        public string FlowScope { get; set; }
        public string isUse { get; set; }
    }
}
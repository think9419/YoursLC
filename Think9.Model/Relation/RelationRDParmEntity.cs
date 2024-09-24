using DapperExtensions;

namespace Think9.Models
{
    [Table("relationrdparm")]
    public class RelationRDParmEntity
    {
        public int RelationId { get; set; }
        public string ParmId { get; set; }
        public string ParmValue { get; set; }
        public string ParmType { get; set; }
    }
}
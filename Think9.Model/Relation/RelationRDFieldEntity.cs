using DapperExtensions;

namespace Think9.Models
{
    [Table("relationrdfield")]
    public class RelationRDFieldEntity
    {
        [DapperExtensions.Key(true)]
        public int ID { get; set; }

        public int RelationId { get; set; }
        public string FillIndexId { get; set; }
        public string SelectIndexId { get; set; }
        public string isValue { get; set; }
    }
}
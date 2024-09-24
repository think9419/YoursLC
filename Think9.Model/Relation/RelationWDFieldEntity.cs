using DapperExtensions;

namespace Think9.Models
{
    [Table("relationwdfield")]
    public class RelationWDFieldEntity
    {
        [DapperExtensions.Key(true)]
        public int ID { get; set; }

        public int RelationId { get; set; }
        public string WriteField { get; set; }
        public string Expression { get; set; }
        public int EmptyControl { get; set; }
    }
}
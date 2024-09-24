using DapperExtensions;
using System;

namespace Think9.Models
{
    /// <summary>
    ///
    /// </summary>
    [Table("indexparm")]
    public class IndexParmEntity
    {
        public string ParmId { get; set; }

        public string ParmName { get; set; }

        /// <summary>
        /// string int decimal DateTime
        /// </summary>
        public string DataType { get; set; }

        public DateTime UpdateTime { get; set; }

        public string ParmExplain { get; set; }

        [Computed]
        public string Value { get; set; }

        [Computed]
        public string Text { get; set; }

        [Computed]
        public string Used { get; set; }
    }
}
using DapperExtensions;
using System;

namespace Think9.Models
{
    [Table("lockedlist")]
    public class LockedListEntity
    {
        public long ListId { get; set; }
        public string FwId { get; set; }
        public string UserId { get; set; }
        public DateTime LockTime { get; set; }
    }
}
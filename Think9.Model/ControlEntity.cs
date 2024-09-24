using System.Collections.Generic;

namespace Think9.Models
{
    public class ControlEntity
    {
        public string ControlID { get; set; }
        public string ControlValue { get; set; }

        /// <summary>
        /// 1：text文本框 2：select下拉选择 3：checkbox复选框 4：radio单选框 5：img图片 9：其他
        /// </summary>
        public string ControlType { get; set; }

        public IEnumerable<valueTextEntity> ListValue { get; set; }
    }
}
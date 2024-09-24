using System.Collections.Generic;

namespace Think9.QRCodeAPI
{
    public class CodesResultItem
    {
        /// <summary>
        ///
        /// </summary>
        public List<string> text { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string type { get; set; }
    }

    public class CodesResult
    {
        /// <summary>
        ///
        /// </summary>
        public string log_id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int codes_result_num { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<CodesResultItem> codes_result { get; set; }
    }
}
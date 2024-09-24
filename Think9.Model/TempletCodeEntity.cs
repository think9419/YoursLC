using DapperExtensions;

namespace Think9.Models
{
    [Table("TbModel")]
    public class TempletCodeEntity
    {
        /// <summary>
        /// 表编码
        /// </summary>
        public string TbId
        {
            get; set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string FileName
        {
            get; set;
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath
        {
            get; set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreationTime
        {
            get; set;
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastWriteTime
        {
            get; set;
        }

        /// <summary>
        /// 解释说明
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        ///
        /// </summary>
        public double DiffMinutes
        {
            get; set;
        }

        public string SourceFilePath
        {
            get; set;
        }

        public string SourceFileCreatTime
        {
            get; set;
        }

        public string TargetFilePath
        {
            get; set;
        }

        public string TargetFileCreatTime
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }
    }
}
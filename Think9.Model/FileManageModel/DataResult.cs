namespace Think9.Models
{
    /// <summary>
    /// 通用数据结果类
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class DataResult<TModel>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 数据结果
        /// </summary>
        public TModel Data { get; set; }

        public DataResult()
        {
            IsSuccess = true;
        }

        public DataResult(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public DataResult(TModel data)
        {
            IsSuccess = true;
            Data = data;
        }
    }
}
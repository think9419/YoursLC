using System;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public class LogonLogService : BaseService<LogonLogEntity>
    {
        //public dynamic GetListByFilter(LogonLogEntity filter, PageInfoEntity pageInfo)
        //{
        //    string _where = " where 1=1";
        //    if (!string.IsNullOrEmpty(filter.Account))
        //    {
        //        _where += " and Account=@Account";
        //    }
        //    if (!string.IsNullOrEmpty(filter.RealName))
        //    {
        //        _where += " and RealName=@RealName";
        //    }
        //    _where = CreateTimeWhereStr(filter.StartEndDate, _where);
        //    return GetPageByFilter(filter, pageInfo, _where);
        //}

        /// <summary>
        /// 写入登录日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int WriteDbLog(LogonLogEntity model, string ip, string iPAddressName)
        {
            model.IPAddress = ip;
            model.IPAddressName = iPAddressName;
            model.createTime = DateTime.Now;
            return BaseRepository.Insert(model);
        }
    }
}
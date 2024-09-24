using System;
using System.Collections.Generic;

namespace Think9.Services.Base
{
    public class Record
    {
        public static void Delete(string listid, string flowid)
        {
            ComService comService = new ComService();
            comService.ExecuteSql("delete from recordrun  where FlowId ='" + flowid + "' and  ListId ='" + listid + "'");
        }

        public static void AddErr(string listid, string flowid, string flag, Exception ex)
        {
            ComService comService = new ComService();

            List<string> columns = new List<string>();
            columns.Add("OperateTime");
            columns.Add("ListId");
            columns.Add("TbId");
            columns.Add("Info");

            //string err = GetExceptionMessage(ex);
            object param = new { OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ListId = listid, TbId = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_"), Info = flag + ":" + ex.ToString() };

            comService.Insert("recorderr", columns, param);

            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error("\r\n---catch (Exception ex){" + flag + "}\r\n" + ex.ToString());
        }

        public static void Add(string userid, string listid, string flowid, string info)
        {
            ComService comService = new ComService();

            List<string> columns = new List<string>();
            columns.Add("OperateTime");
            columns.Add("OperatePerson");
            columns.Add("ListId");
            columns.Add("FlowId");
            columns.Add("TbId");
            columns.Add("Info");

            object param = new { OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), OperatePerson = userid, ListId = listid, FlowId = flowid, TbId = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_"), Info = info };

            comService.Insert("recordrun", columns, param);
        }

        public static void AddResultList(string userid, string listid, string flowid, List<string> resultList, string exaInfo = "")
        {
            if (resultList == null)
            {
                return;
            }

            ComService comService = new ComService();

            List<string> columns = new List<string>();
            columns.Add("OperateTime");
            columns.Add("OperatePerson");
            columns.Add("ListId");
            columns.Add("FlowId");
            columns.Add("TbId");
            columns.Add("Info");

            foreach (string info in resultList)
            {
                object param = new { OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), OperatePerson = userid, ListId = listid, FlowId = flowid, TbId = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_"), Info = exaInfo + info };
                comService.Insert("recordrun", columns, param);
            }
        }

        public static void AddAttInfo(string userid, string listid, string flowid, string info, string flag)
        {
            ComService comService = new ComService();

            List<string> columns = new List<string>();
            columns.Add("OperateTime");
            columns.Add("OperatePerson");
            columns.Add("ListId");
            columns.Add("FlowId");
            columns.Add("TbId");
            columns.Add("Info");
            columns.Add("RecordFlag");

            object param = new { OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), OperatePerson = userid, ListId = listid, FlowId = flowid, TbId = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_"), Info = info, RecordFlag = flag };

            comService.Insert("recordrun", columns, param);
        }

        public static void AddInfo(string userid, string objectid, string info)
        {
            ComService comService = new ComService();

            List<string> columns = new List<string>();
            columns.Add("OperateTime");
            columns.Add("OperatePerson");
            columns.Add("ObjectId");
            columns.Add("Info");

            object param = new { OperateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), OperatePerson = userid, ObjectId = objectid, Info = info };

            comService.Insert("recordset", columns, param);
        }
    }
}
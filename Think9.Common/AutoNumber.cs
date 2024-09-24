using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class AutoNo
    {
        public static string SetAutoNumber(long listid, string fwid, CurrentUserEntity user)
        {
            //1首先求自动编号 2子表数据初始化
            string err = "";
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string indexid = "";
            string ruleid = "";
            string dataType = "";
            string userId = "";
            string deptNo = "";
            string autoNo = "";
            long itemid = 0;

            ComService comService = new ComService();

            if (user != null)
            {
                userId = user.Account == null ? "!NullEx" : user.Account;
                deptNo = user.DeptNo == null ? "!NullEx" : user.DeptNo;

                DataTable dt = comService.GetDataTable("tbindex", "RuleType,IndexId,RuleId,DataType", "TbId='" + tbid + "'", "");
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["RuleType"].ToString() == "2" && dr["RuleId"].ToString() != "")
                    {
                        indexid = dr["IndexId"].ToString();
                        ruleid = dr["RuleId"].ToString();
                        dataType = dr["DataType"].ToString();

                        autoNo = GetAutoNo(ref err, tbid, indexid, ruleid, userId, deptNo, listid, itemid);

                        if (dataType.StartsWith("3"))//数值型指标
                        {
                            comService.ExecuteSql("update " + tbid + " set " + indexid + "=" + autoNo + " where listid=" + listid + "", null, "编辑{" + indexid + "}自动编号");
                        }
                        else
                        {
                            comService.ExecuteSql("update " + tbid + " set " + indexid + "='" + autoNo + "' where listid=" + listid + "", null, "编辑{" + indexid + "}自动编号");
                        }
                    }
                }
            }
            else
            {
                err = "当前用户为空";
            }
            return err;
        }

        public static long SetAutoNumber(ref string err, string fwid, string tbname, CurrentPrcsEntity mPrcs, CurrentUserEntity user)
        {
            //1首先求自动编号 2子表数据初始化
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string indexid = "";
            string ruleid = "";
            string dataType = "";
            string userId = "";
            string deptNo = "";
            string autoNo = "";
            long listid = 0;
            long itemid = 0;

            ComService comService = new ComService();

            if (user != null)
            {
                userId = user.Account == null ? "!NullEx" : user.Account;
                deptNo = user.DeptNo == null ? "!NullEx" : user.DeptNo;

                DataTable dt = comService.GetDataTable("tbindex", "RuleType,IndexId,RuleId,DataType", "TbId='" + tbid + "'", "");
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["RuleType"].ToString() == "2" && dr["RuleId"].ToString() != "")
                    {
                        indexid = dr["IndexId"].ToString();
                        ruleid = dr["RuleId"].ToString();
                        dataType = dr["DataType"].ToString();

                        if (listid == 0)
                        {
                            listid = PageCom.InsertEmptyReturnID(ref err, fwid, tbname, user, mPrcs);
                        }

                        if (err == "")
                        {
                            autoNo = GetAutoNo(ref err, tbid, indexid, ruleid, userId, deptNo, listid, itemid);

                            if (dataType.StartsWith("3"))//数值型指标
                            {
                                comService.ExecuteSql("update " + tbid + " set " + indexid + "=" + autoNo + " where listid=" + listid + "", null, "编辑{" + indexid + "}自动编号");
                            }
                            else
                            {
                                comService.ExecuteSql("update " + tbid + " set " + indexid + "='" + autoNo + "' where listid=" + listid + "", null, "编辑{" + indexid + "}自动编号");
                            }
                        }
                    }
                }
            }
            return listid;
        }

        /// <summary>
        /// 删除自动编号
        /// </summary>
        /// <param name="listid"></param>
        /// <param name="fwid"></param>
        public static void DelAutoNo(string listid, string fwid)
        {
            ComService ComService = new ComService();
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            foreach (DataRow dr in ComService.GetDataTable("tbindex", "RuleId", "TbId='" + tbid + "' and RuleType='2'", "").Rows)
            {
                string ruleid = dr["RuleId"].ToString();
                if (ComService.IsDataTableExists("ruleauto_" + ruleid))
                {
                    ComService.ExecuteSql("delete from ruleauto_" + ruleid + " where listid=" + listid + " and tbid='" + tbid + "'", null, "删除自动编号");
                }
            }
        }

        /// <summary>
        /// 获取自动编号值
        /// </summary>
        public static string GetAutoNo(ref string err, string tbid, string indexid, string ruleid, string userId, string deptNo, long listid, long itemid)
        {
            ComService comService = new ComService();
            string autoNo = "";

            string timeNo = "";
            string where = "";
            string _no = "";
            string some1 = "";//位数
            string some2 = "";//起始值
            int maxNo = 1;//起始值

            RuleAutoNoEntity model = new RuleAutoNoEntity();
            model.listid = listid;
            model.itemid = itemid;
            model.tbid = tbid;
            model.indexid = indexid;

            where = "where tbid='" + model.tbid + "' and indexid='" + model.indexid + "' ";

            DataTable dt = comService.GetDataTable("ruleauto", "*", "RuleId='" + ruleid + "'", "order by AutoOrder");

            if (ruleid != "")
            {
                //只有存在序号才需要去求序号值
                if (comService.GetTotal("ruleauto", "where RuleId='" + ruleid + "' and AutoType='31'") > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //序号
                        if (dr["AutoType"].ToString() == "31")
                        {
                            //位数!!!
                            some1 = dr["AutoSome1"].ToString().Trim();
                            //起始值!!!
                            some2 = dr["AutoSome2"].ToString().Trim();
                        }

                        //日期（年）
                        if (dr["AutoType"].ToString() == "21")
                        {
                            timeNo = DateTime.Now.Year.ToString();
                            where += " and timeno='" + timeNo + "'";
                            model.timeno = timeNo;
                        }

                        // 日期（年月）
                        if (dr["AutoType"].ToString() == "22")
                        {
                            timeNo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0');
                            where += " and timeno='" + timeNo + "'";
                            model.timeno = timeNo;
                        }

                        //日期（年月日）
                        if (dr["AutoType"].ToString() == "23")
                        {
                            timeNo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                            where += " and timeno='" + timeNo + "'";
                            model.timeno = timeNo;
                        }

                        // 用户登录名
                        if (dr["AutoType"].ToString() == "41")
                        {
                            where += " and userid='" + userId + "'";
                            model.userid = userId;
                        }

                        // 用户所在单位(部门)编码
                        if (dr["AutoType"].ToString() == "42")
                        {
                            where += " and deptno='" + deptNo + "'";
                            model.deptno = deptNo;
                        }
                    }

                    maxNo = 1;
                    if (some2.Trim() != "")
                    {
                        maxNo = int.Parse(some2.Trim());
                    }

                    //判断是否在ruleauto_系列表中添加过了
                    if (comService.GetTotal("ruleauto_" + ruleid, "where listid=" + listid + " and itemid=" + itemid + "  and tbid='" + tbid + "'  and indexid='" + indexid + "' ") > 0)
                    {
                        //sNo = dal.GetSingleStr("autono", "ruleauto_" + tbid, "listid=" + listid + " and itemid=" + itemid + "  and tbid='" + tbid + "'  and indexid='" + mTbIndex.IndexId + "' ");
                    }
                    else
                    {
                        model.autono = comService.GetMaxID("autono", "ruleauto_" + ruleid, where);
                        if (maxNo > model.autono)
                        {
                            model.autono = maxNo;
                        }

                        List<string> columns = new List<string>();
                        columns.Add("listid");
                        columns.Add("itemid");
                        columns.Add("tbid");
                        columns.Add("indexid");
                        columns.Add("deptno");
                        columns.Add("userid");
                        columns.Add("timeno");
                        columns.Add("autono");
                        columns.Add("autoid");

                        object param = new { listid = listid, itemid = 0, tbid = tbid, indexid = indexid, deptno = deptNo, userid = userId, timeno = timeNo, autono = model.autono, autoid = "" };

                        comService.Insert("ruleauto_" + ruleid, columns, param);

                        _no = model.autono.ToString();
                    }

                    //位数不为空
                    if (some1.Trim() != "")
                    {
                        //并且位数小于得出的值，前面补0
                        if (_no.Length < int.Parse(some1.Trim()))
                        {
                            _no = _no.PadLeft(int.Parse(some1.Trim()), '0');
                        }
                    }
                }

                //再循环得值
                foreach (DataRow dr in dt.Rows)
                {
                    //固定字符
                    if (dr["AutoType"].ToString() == "11")
                    {
                        autoNo += dr["AutoSome1"].ToString().Trim();
                    }

                    //序号
                    if (dr["AutoType"].ToString() == "31")
                    {
                        autoNo += _no;
                    }

                    //日期（年）
                    if (dr["AutoType"].ToString() == "21")
                    {
                        timeNo = DateTime.Now.Year.ToString();
                        autoNo += timeNo;
                    }

                    // 日期（年月）
                    if (dr["AutoType"].ToString() == "22")
                    {
                        timeNo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0');
                        autoNo += timeNo;
                    }

                    //日期（年月日）
                    if (dr["AutoType"].ToString() == "23")
                    {
                        timeNo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                        autoNo += timeNo;
                    }

                    // 用户登录名
                    if (dr["AutoType"].ToString() == "41")
                    {
                        autoNo += userId;
                    }

                    // 用户所在单位(部门)编码
                    if (dr["AutoType"].ToString() == "42")
                    {
                        autoNo += deptNo;
                    }
                }
            }

            return autoNo;
        }
    }
}
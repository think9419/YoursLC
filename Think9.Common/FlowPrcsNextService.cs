using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class FlowPrcsNextService : BaseService<FlowPrcsNextEntity>
    {
        private ComService comService = new ComService();
        private ServiceFlowPrcs FlowPrcsService = new ServiceFlowPrcs();

        public IEnumerable<valueTextEntity> GetSelectNextList(string id, string fid)
        {
            DataTable dt = DataTableHelp.NewValueTextDt();

            string sql = "select PrcsId,PrcsName  from flowprcs where FlowId='" + fid + "' order by PrcsOrder";
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                if (dr["PrcsId"].ToString() != id)
                {
                    DataRow row = dt.NewRow();
                    row["ClassID"] = "";
                    row["Value"] = dr["PrcsId"].ToString();
                    row["Text"] = dr["PrcsName"].ToString();
                    dt.Rows.Add(row);
                }
            }

            DataRow row1 = dt.NewRow();
            row1["ClassID"] = "";
            row1["Value"] = "";
            row1["Text"] = "结束";
            dt.Rows.Add(row1);

            return DataTableHelp.ToEnumerable<valueTextEntity>(dt);
        }

        public IEnumerable<FlowPrcsNextEntity> GetNextListById(string id, string fid)
        {
            DataTable dt = comService.GetDataTable("select PrcsId,PrcsName  from flowprcs where FlowId='" + fid + "' order by PrcsOrder");
            IEnumerable<FlowPrcsNextEntity> list = base.GetByWhere("where PrcsId=@id", new { id = id }, null, "order by NextOrder");
            foreach (FlowPrcsNextEntity obj in list)
            {
                string name = "";
                if (obj.NextPrcsId == 0)
                {
                    name = "结束";
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["PrcsId"].ToString() == obj.NextPrcsId.ToString())
                        {
                            name = row["PrcsName"].ToString();
                        }
                    }
                }

                obj.NextPrcsName = name;
            }

            return list;
        }

        /// <summary>
        /// 返回流程步骤下一步
        /// </summary>
        /// <param name="listid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public IEnumerable<valueTextEntity> GetNextPrcsList(string listid, string fid)
        {
            string currentPrcsId = "";
            DataTable dtList = DataTableHelp.NewValueTextDt();

            string sql = "select currentPrcsId  from flowrunlist where listid=" + listid + "";
            DataTable dtRunList = comService.GetDataTable(sql);
            if (dtRunList.Rows.Count > 0)
            {
                currentPrcsId = dtRunList.Rows[0]["currentPrcsId"].ToString();
            }

            if (currentPrcsId != "")
            {
                sql = "select PrcsId,PrcsName  from flowprcs where FlowId='" + fid + "'";
                DataTable dtPrcs = comService.GetDataTable(sql);

                sql = "select PrcsId,NextPrcsId  from flowprcsnext where PrcsId=" + currentPrcsId + " order by NextOrder";
                foreach (DataRow drNext in comService.GetDataTable(sql).Rows)
                {
                    if (drNext["NextPrcsId"].ToString() == "0")
                    {
                        DataRow row = dtList.NewRow();
                        row["ClassID"] = "";
                        row["Value"] = "0";
                        row["Text"] = "结束";
                        dtList.Rows.Add(row);
                    }
                    else
                    {
                        string name = "";
                        foreach (DataRow drPrcs in dtPrcs.Rows)
                        {
                            if (drNext["NextPrcsId"].ToString() == drPrcs["PrcsId"].ToString())
                            {
                                name = drPrcs["PrcsName"].ToString();
                            }
                        }

                        if (name != "")
                        {
                            DataRow row = dtList.NewRow();
                            row["ClassID"] = "";
                            row["Value"] = drNext["NextPrcsId"].ToString();
                            row["Text"] = name;
                            dtList.Rows.Add(row);
                        }
                    }
                }
            }

            return DataTableHelp.ToEnumerable<valueTextEntity>(dtList);
        }

        /// <summary>
        /// 返回流程步骤下一步
        /// </summary>
        /// <param name="listid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public IEnumerable<valueTextEntity> GetAllPrcsList(string fid)
        {
            DataTable dtList = DataTableHelp.NewValueTextDt();

            string sql = "select PrcsId,PrcsName  from flowprcs where FlowId='" + fid + "'";
            DataTable dtPrcs = comService.GetDataTable(sql);

            foreach (DataRow dr in dtPrcs.Rows)
            {
                DataRow row = dtList.NewRow();
                row["ClassID"] = "";
                row["Value"] = dr["PrcsId"].ToString();
                row["Text"] = dr["PrcsName"].ToString();
                dtList.Rows.Add(row);
            }

            DataRow row2 = dtList.NewRow();
            row2["ClassID"] = "";
            row2["Value"] = "0";
            row2["Text"] = "结束";
            dtList.Rows.Add(row2);

            return DataTableHelp.ToEnumerable<valueTextEntity>(dtList);
        }

        /// <summary>
        /// 返回流程步骤下一步 FlowRunPrcsListEntity
        /// </summary>
        /// <param name="listid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public IEnumerable<FlowRunPrcsListEntity> GetBackPrcsList(string listid, string fid)
        {
            string currentPrcsId = "";
            string relatedId = "";
            int number = 0;
            DataTable dtList = DataTableHelp.NewValueTextDt();

            List<FlowRunPrcsListEntity> list = new List<FlowRunPrcsListEntity>();

            string sql = "select currentPrcsId,relatedId  from flowrunlist where listid=" + listid + " ";
            DataTable dtRunList = comService.GetDataTable(sql);
            if (dtRunList.Rows.Count > 0)
            {
                currentPrcsId = dtRunList.Rows[0]["currentPrcsId"].ToString();
                relatedId = dtRunList.Rows[0]["relatedId"].ToString();
            }

            if (currentPrcsId != "")
            {
                FlowPrcsEntity model = FlowPrcsService.GetByWhereFirst("where PrcsId=" + currentPrcsId + "");
                if (model != null)
                {
                    sql = "select *  from flowrunprcslist where listid=" + listid + " and id <> " + relatedId + "  and beginUserId <> '' order by id desc";
                    number = 0;
                    foreach (DataRow dr in comService.GetDataTable(sql).Rows)
                    {
                        if (model.AllowBack == "2")
                        {
                            if (number == 0)
                            {
                                FlowRunPrcsListEntity mList = new FlowRunPrcsListEntity();
                                mList.id = int.Parse(dr["id"].ToString().Trim());
                                mList.PrcsId = dr["PrcsId"].ToString().Trim() == "" ? -1 : int.Parse(dr["PrcsId"].ToString().Trim());
                                mList.beginUserId = dr["beginUserId"].ToString().Trim();
                                mList.createTime = dr["createTime"].ToString().Trim() == "" ? null : DateTime.Parse(dr["createTime"].ToString().Trim());
                                mList.FlowPrcs = dr["FlowPrcs"].ToString().Trim();

                                list.Add(mList);

                                number++;
                            }
                        }

                        if (model.AllowBack == "3")
                        {
                            FlowRunPrcsListEntity mList = new FlowRunPrcsListEntity();
                            mList.id = int.Parse(dr["id"].ToString().Trim());
                            mList.PrcsId = dr["PrcsId"].ToString().Trim() == "" ? -1 : int.Parse(dr["PrcsId"].ToString().Trim());
                            mList.beginUserId = dr["beginUserId"].ToString().Trim();
                            mList.createTime = dr["createTime"].ToString().Trim() == "" ? null : DateTime.Parse(dr["createTime"].ToString().Trim());
                            mList.FlowPrcs = dr["FlowPrcs"].ToString().Trim();

                            list.Add(mList);
                        }
                    }
                }
            }

            return list;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class FlowRunList : BaseService<FlowRunListEntity>
    {
        private ComService comService = new ComService();

        public static long InsertFlowrunListReturnID(ref string err, string maintbid, string flowid, string flowname, CurrentPrcsEntity mCurrentPrcs, CurrentUserEntity user, DataTable dt = null)
        {
            long listid = 0;
            int itemid = 0;
            ComService comService = new ComService();
            FlowRunList flowRunList = new FlowRunList();
            ServiceFlowRunPrcsList flowRunPrcsList = new ServiceFlowRunPrcsList();

            if (user != null)
            {
                if (mCurrentPrcs.ERR == "")
                {
                    FlowRunListEntity mRun = new FlowRunListEntity();
                    mRun.ruNumber = "err_no";
                    mRun.runName = BaseUtil.GetRunName(user.Account, flowid, flowname);
                    mRun.FlowId = flowid;
                    mRun.TbId = maintbid;
                    mRun.createUser = user.Account;
                    mRun.createDeptStr = user.DeptNoStr;
                    mRun.CreateUpDept = user.UpDeptNo;
                    mRun.createDept = user.DeptNo;
                    mRun.createTime = DateTime.Now;
                    mRun.runFlag = "2";
                    mRun.isFinish = "2";
                    mRun.isLock = "0";
                    mRun.isSetUp = "1";
                    mRun.isInspect = "0";
                    mRun.reportMode = 1;

                    mRun.flowType = mCurrentPrcs.flowType;

                    mRun.currentPrcsId = int.Parse(mCurrentPrcs.PrcsId);
                    mRun.currentPrcsName = mCurrentPrcs.PrcsName;
                    mRun.relatedId = 0;

                    mRun.currentPrcsUser = mCurrentPrcs.PrcsUser;
                    mRun.currentPrcsDept = mCurrentPrcs.PrcsDept;
                    mRun.currentPrcsPriv = mCurrentPrcs.PrcsPriv;
                    mRun.currentPrcsUser1 = ";" + user.Account + ";";
                    mRun.currentPrcsUser2 = "";
                    mRun.currentPrcsTransact = "";
                    mRun.transactUser1 = ";" + user.Account + ";";
                    mRun.transactUser2 = "";
                    mRun.attachmentId = "";
                    mRun.EndTime = null;

                    mRun.prevPrcsId = "";
                    mRun.prevUserId = "";
                    mRun.FlowFlag = mCurrentPrcs.flowType; //FlowFlag

                    listid = flowRunList.InsertReturnID(mRun);
                    if (listid != 0)
                    {
                        FlowRunPrcsListEntity mRunItem = new FlowRunPrcsListEntity();

                        mRunItem.listid = listid;
                        mRunItem.FlowId = flowid;
                        mRunItem.PrcsId = int.Parse(mCurrentPrcs.PrcsId);
                        mRunItem.FlowPrcs = mCurrentPrcs.PrcsName;
                        mRunItem.beginUserId = user.Account;
                        mRunItem.createTime = DateTime.Now;
                        mRunItem.runFlag = "2";
                        mRunItem.stepInfo = "{" + user.Account + "于" + DateTime.Now.ToString() + "新建}";
                        mRunItem.deliverType = "0";

                        itemid = flowRunPrcsList.InsertReturnID(mRunItem);
                        if (itemid != 0)
                        {
                            mRun.relatedId = itemid;
                            mRun.ruNumber = BaseUtil.GetRuNumber(flowid, listid);
                            mRun.listid = listid;
                            flowRunList.UpdateByWhere("where listid=" + listid + "", "relatedId,ruNumber", mRun);
                        }
                        else
                        {
                            err = "flowrunprcslist添加数据失败";
                        }
                    }
                    else
                    {
                        err = "flowrunlist添加数据失败";
                    }
                }
                else
                {
                    err = mCurrentPrcs.ERR;
                }
            }
            else
            {
                err = "当前用户为空";
            }

            return listid;
        }

        public List<FlowRunListEntity> GetManageDataList(ref long total, PageInfoEntity pageInfo, CurrentUserEntity user, string fwid, string userid, string islock, string key)
        {
            string where = "";
            List<FlowRunListEntity> list = new List<FlowRunListEntity>();

            where = PageCom.GetManageWhere2(user, fwid, key);
            pageInfo.returnFields = "listid, isLock, runName, createUser, createTime,createDept,createDeptStr,attachmentId";
            total = comService.GetTotal(fwid.Replace("bi_", "tb_"), where);

            DataTable dt = comService.GetPageList(fwid.Replace("bi_", "tb_"), pageInfo.returnFields, where, "order by listid desc", pageInfo.page, pageInfo.limit, null);
            foreach (DataRow dr in dt.Rows)
            {
                FlowRunListEntity mlist = new FlowRunListEntity();
                mlist.listid = long.Parse(dr["listid"].ToString());
                mlist.FlowId = fwid;
                mlist.TbId = fwid.Replace("bi_", "tb_");
                mlist.runName = dr["runName"].ToString();
                mlist.ruNumber = mlist.TbId + "#" + dr["listid"].ToString();
                mlist.isLock = dr["isLock"].ToString();

                mlist.isFinish = "2";
                mlist.relatedId = 0;
                mlist.flowType = "0";

                mlist.createUser = dr["createUser"].ToString();
                if (dr["createTime"].ToString().Trim() != "")
                {
                    mlist.createTime = DateTime.Parse(dr["createTime"].ToString());
                }
                mlist.createDept = dr["createDept"].ToString();
                mlist.createDeptStr = dr["createDeptStr"].ToString();
                mlist.attachmentId = dr["attachmentId"].ToString();
                mlist.isNull = "";

                list.Add(mlist);
            }

            return list;
        }

        public dynamic GetManageDataList(FlowRunListEntity model, PageInfoEntity pageInfo, CurrentUserEntity user, string fwid, string userid, string islock, string key)
        {
            FlowRunList RunListService = new FlowRunList();

            pageInfo.returnFields = "listid, ruNumber,FlowId,TbId,isLock , runName, createUser, isFinish,relatedId,flowType,currentPrcsId,runFlag,currentPrcsName,createTime";
            pageInfo.field = "listid";
            pageInfo.order = "desc";

            string where = PageCom.GetManageWhere(user, fwid, key);

            return RunListService.GetPageByFilter(model, pageInfo, where);
        }
    }
}
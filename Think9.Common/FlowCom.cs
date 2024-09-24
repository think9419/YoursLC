using System;
using System.Data;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class FlowCom
    {
        private static ComService comService = new ComService();

        public static string GetIndexDisabled(string tbid, string prcno, string indexid)
        {
            if (string.IsNullOrEmpty(tbid) || string.IsNullOrEmpty(indexid))
            {
                return "Y";
            }

            string disabled = "Y";
            string _fwid = tbid;
            if (tbid.StartsWith("tb_"))
            {
                _fwid = comService.GetSingleField("select FlowId  FROM tbbasic WHERE TbId='" + tbid + "'");
            }

            if (_fwid.StartsWith("bi_"))
            {
                disabled = "";
            }
            else
            {
                //流程类型？1固定2自由流程 0无流程
                if (comService.GetSingleField("select flowType  FROM flow WHERE FlowId='" + _fwid + "'") == "1")
                {
                    string prcsIndex = comService.GetSingleField("select PrcsIndex  FROM flowprcs WHERE FlowId='" + _fwid + "' and PrcsNo='" + prcno + "' ");//可写字段
                    if (prcsIndex.ToLower().Contains(";" + indexid.ToLower() + ";") || prcsIndex.ToLower() == "#all#")
                    {
                        disabled = "";
                    }
                }
                else
                {
                    disabled = "";
                }
            }

            return disabled;
        }

        //结束工作
        public static string FinishFlowPrcs(string listid, string flowid, string prcno, CurrentUserEntity user)
        {
            string err = "";
            //ComService comService = new ComService();
            if (flowid.StartsWith("fw_"))
            {
                FlowBase flow = new FlowBase();
                FlowEntity mflow = flow.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = flowid });
                if (mflow != null)
                {
                    if (mflow.flowType == "1")
                    {
                        err = FlowCom.SetFlowPrcsNext01(user, listid, "0", "", "", "");
                    }
                    else
                    {
                        err = FlowCom.SetFlowPrcsNext02(user, listid, "0", "", "", "");
                    }
                }
                else
                {
                    err = "流程对象为空";
                }
            }
            else
            {
                comService.ExecuteSql("update " + flowid.Replace("bi_", "tb_") + " set isLock='1'   WHERE listid= " + listid, null, "结束工作");
            }

            comService.ExecuteSql("delete from lockedlist where ListId=" + listid + " and FwId='" + flowid + "'");//删除临时锁定标志

            return err;
        }

        /// <summary>
        /// 得到当前流程第一步 基础信息返回空
        /// </summary>
        /// <param name="fwid">流程编码</param>
        /// <returns>基础信息返回空</returns>
        public static CurrentPrcsEntity GetFistStept(string fwid)
        {
            if (fwid.StartsWith("bi_"))
            {
                return null;
            }
            else
            {
                FlowBase flowService = new FlowBase();

                CurrentPrcsEntity mPrcs = new CurrentPrcsEntity();
                mPrcs.FlowId = fwid;
                mPrcs.flowType = "0";//默认0无流程
                mPrcs.ListId = 0;
                mPrcs.runFlag = "0";//注意默认值0待新建
                mPrcs.relatedId = 0;
                mPrcs.PrcsId = "-1";
                mPrcs.ERR = "";

                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
                if (mflow == null)
                {
                    mPrcs.ERR = "流程FlowEntity对象不存在";
                }
                else
                {
                    mPrcs.FlowId = mflow.FlowId;
                    mPrcs.TbId = mflow.TbId;
                    mPrcs.flowType = mflow.flowType;
                    mPrcs.FlowName = mflow.FlowName;
                    mPrcs.isUse = mflow.isUse;

                    //11查看编辑用户本人新建的数据
                    //22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
                    //21查看编辑用户所属单位(部门)新建的数据
                    //32查看用户所属下级部门新建的数据  编辑本人新建的数据
                    //31查看编辑用户所属下级部门新建的数据
                    //42查看所有数据  编辑本人新建的数据
                    //41查看编辑所有数据
                    mPrcs.SearchMode = string.IsNullOrEmpty(mflow.SearchMode) ? "11" : mflow.SearchMode;

                    mPrcs.ERR = "";
                    if (mPrcs.isUse == "2")
                    {
                        mPrcs.ERR = "已禁用 不能再编辑或办理！";
                    }
                    else
                    {
                        mPrcs.isLock = "0";//添加时一定是未锁定状态

                        //固定流程
                        if (mflow.flowType == "1")
                        {
                            DataTable dt = comService.GetDataTable("flowprcs", "*", "where FlowId=@FlowId and isFirst=@isFirst", "", new { FlowId = fwid, isFirst = 1 });
                            if (dt.Rows.Count == 1)
                            {
                                mPrcs.PrcsId = dt.Rows[0]["PrcsId"].ToString();
                                mPrcs.PrcsNo = dt.Rows[0]["PrcsNo"].ToString();
                                mPrcs.PrcsName = dt.Rows[0]["PrcsName"].ToString();
                                mPrcs.PrcsOrder = dt.Rows[0]["PrcsOrder"].ToString();

                                mPrcs.PrcsIndex = dt.Rows[0]["PrcsIndex"].ToString();
                                mPrcs.HiddenIndex = dt.Rows[0]["HiddenIndex"].ToString();

                                mPrcs.PrcsUser = dt.Rows[0]["PrcsUser"].ToString();
                                mPrcs.PrcsDept = dt.Rows[0]["PrcsDept"].ToString();
                                mPrcs.PrcsPriv = dt.Rows[0]["PrcsPriv"].ToString();
                            }
                            else
                            {
                                if (dt.Rows.Count > 1)
                                {
                                    mPrcs.ERR = "设置了多个流程步骤第1步";
                                }
                                if (dt.Rows.Count < 1)
                                {
                                    mPrcs.ERR = "未设置流程步骤第1步";
                                }
                            }
                        }
                        //自由流程
                        if (mflow.flowType == "2")
                        {
                            mPrcs.PrcsId = "1";
                            mPrcs.PrcsNo = "step_1";
                            mPrcs.PrcsName = "第1步";
                            mPrcs.PrcsOrder = "1";

                            mPrcs.PrcsIndex = "#all#";
                            mPrcs.HiddenIndex = "";

                            mPrcs.PrcsUser = "#all#";
                            mPrcs.PrcsDept = "#all#";
                            mPrcs.PrcsPriv = "#all#";
                        }
                    }
                }

                return mPrcs;
            }
        }

        /// <summary>
        /// 得到当前流程步骤 基础信息返回空
        /// </summary>
        /// <param name="fwid">流程编码</param>
        /// <returns>基础信息返回空</returns>
        public static CurrentPrcsEntity GetCurrentStept(string fwid, string listid)
        {
            if (fwid.StartsWith("bi_"))
            {
                return null;
            }
            else
            {
                FlowBase flowService = new FlowBase();

                CurrentPrcsEntity mPrcs = new CurrentPrcsEntity();
                mPrcs.FlowId = fwid;
                mPrcs.flowType = "0";//默认0无流程
                mPrcs.ListId = 0;
                mPrcs.runFlag = "0";//注意默认值0待新建
                mPrcs.relatedId = 0;
                mPrcs.PrcsId = "-1";
                mPrcs.ERR = "";

                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
                if (mflow == null)
                {
                    mPrcs.ERR = "流程不存在";
                }
                else
                {
                    mPrcs.FlowId = mflow.FlowId;
                    mPrcs.TbId = mflow.TbId;
                    mPrcs.flowType = mflow.flowType;
                    mPrcs.FlowName = mflow.FlowName;
                    mPrcs.isUse = mflow.isUse;

                    //11查看编辑用户本人新建的数据
                    //22查看用户所属单位(部门)新建的数据 编辑本人新建的数据
                    //21查看编辑用户所属单位(部门)新建的数据
                    //32查看用户所属下级部门新建的数据  编辑本人新建的数据
                    //31查看编辑用户所属下级部门新建的数据
                    //42查看所有数据  编辑本人新建的数据
                    //41查看编辑所有数据
                    mPrcs.SearchMode = string.IsNullOrEmpty(mflow.SearchMode) ? "11" : mflow.SearchMode;

                    mPrcs.ERR = "";
                    if (mPrcs.isUse == "2")
                    {
                        mPrcs.ERR = "已禁用 不能再编辑、办理和删除！";
                    }
                    else
                    {
                        DataTable dtlist = comService.GetDataTable("flowrunlist", "*", "where listid=@listid and FlowId=@flowid ", "", new { listid = listid, flowid = fwid });
                        if (dtlist.Rows.Count > 0)
                        {
                            mPrcs.ListId = int.Parse(listid);

                            string sPrcsId = dtlist.Rows[0]["currentPrcsId"].ToString();//当前流程步骤
                            int iRelatedId = int.Parse(dtlist.Rows[0]["relatedId"].ToString());

                            mPrcs.PrcsId = sPrcsId;
                            mPrcs.relatedId = iRelatedId;

                            mPrcs.runFlag = dtlist.Rows[0]["runFlag"].ToString();//0待新建1待接手2办理中3已办结
                            mPrcs.isLock = dtlist.Rows[0]["isLock"].ToString();//锁定状态

                            mPrcs.createUser = dtlist.Rows[0]["createUser"].ToString();
                            mPrcs.createDeptStr = dtlist.Rows[0]["createDeptStr"].ToString();
                            mPrcs.createDept = dtlist.Rows[0]["createDept"].ToString();

                            mPrcs.currentPrcsUser1 = dtlist.Rows[0]["currentPrcsUser1"].ToString();

                            mPrcs.transactUser1 = dtlist.Rows[0]["transactUser1"].ToString();

                            //固定流程
                            if (mflow.flowType == "1")
                            {
                                DataTable dt = comService.GetDataTable("flowprcs", "*", "where PrcsId=@PrcsId", "", new { PrcsId = sPrcsId });
                                if (dt.Rows.Count > 0)
                                {
                                    mPrcs.PrcsNo = dt.Rows[0]["PrcsNo"].ToString();
                                    mPrcs.PrcsName = dt.Rows[0]["PrcsName"].ToString();
                                    mPrcs.PrcsOrder = dt.Rows[0]["PrcsOrder"].ToString();

                                    mPrcs.PrcsIndex = dt.Rows[0]["PrcsIndex"].ToString();
                                    mPrcs.HiddenIndex = dt.Rows[0]["HiddenIndex"].ToString();

                                    mPrcs.PrcsUser = dt.Rows[0]["PrcsUser"].ToString();
                                    mPrcs.PrcsDept = dt.Rows[0]["PrcsDept"].ToString();
                                    mPrcs.PrcsPriv = dt.Rows[0]["PrcsPriv"].ToString();
                                    //0待新建1待接手2办理中3已办结
                                    if (mPrcs.runFlag == "1")
                                    {
                                        mPrcs.PrcsUser = dtlist.Rows[0]["currentPrcsUser"].ToString();
                                        mPrcs.PrcsPriv = dtlist.Rows[0]["currentPrcsPriv"].ToString();
                                        mPrcs.PrcsDept = dtlist.Rows[0]["currentPrcsDept"].ToString();
                                    }
                                }
                                else
                                {
                                    mPrcs.ERR = "数据表flowprcs中id=" + sPrcsId + "的流程步骤对象为空";
                                    if (sPrcsId == "0")
                                    {
                                        mPrcs.ERR = "当前流程步骤已结束";
                                    }
                                }
                            }
                            //自由流程
                            if (mflow.flowType == "2")
                            {
                                mPrcs.PrcsNo = "step_" + sPrcsId;
                                mPrcs.PrcsName = "第" + sPrcsId + "步";
                                mPrcs.PrcsOrder = sPrcsId;

                                mPrcs.PrcsIndex = "#all#";
                                mPrcs.HiddenIndex = "";
                                mPrcs.PrcsUser = dtlist.Rows[0]["currentPrcsUser"].ToString();
                                mPrcs.PrcsDept = "";
                                mPrcs.PrcsPriv = "";
                            }
                        }
                        else
                        {
                            mPrcs.ListId = int.Parse(listid);
                            mPrcs.ERR = "FlowRunList数据表不存在该数据！";
                        }
                    }
                }

                if (mPrcs.isLock == "1")
                {
                    mPrcs.ERR += " 当前数据已锁定";
                }

                return mPrcs;
            }
        }

        /// <summary>
        ///接手办理
        /// </summary>
        /// <param name="user">当前用户信息</param>
        /// <param name="mPrcs">当前流程步骤</param>
        /// <returns></returns>
        public static string TakeOverPrcs(CurrentUserEntity user, CurrentPrcsEntity mPrcs)
        {
            string err = "";
            string userId = ";" + user.Account + ";";
            //本流程所有的主办人，办理过该流程的都算
            string transactUser = mPrcs.transactUser1 == null ? userId : mPrcs.transactUser1;
            if (!transactUser.Contains(userId))
            {
                transactUser += userId;
            }

            //1：修改FlowRunPrcsList中beginUserId,createTime,runFlag状态改为2, stepInfo步骤信息
            string sql = "";
            String stepInfo = "";

            //0待新建1待接手2办理中3已办结
            if (mPrcs.runFlag == "1")
            {
                stepInfo = " {" + user.Account + "于" + DateTime.Now + " 接手办理} ";
                sql = "update flowrunprcslist set beginUserId='" + user.Account + "',createTime='" + DateTime.Now + "',runFlag='2', stepInfo = '" + stepInfo + "'  where Id=" + mPrcs.relatedId + "";
            }
            else
            {
                stepInfo = " {" + user.Account + "于" + DateTime.Now + " 开始办理} ";
                stepInfo = comService.GetSingleField("select stepInfo  FROM flowrunprcslist WHERE Id= " + mPrcs.relatedId) + stepInfo;
                sql = "update flowrunprcslist set runFlag='2', stepInfo = '" + stepInfo + "'  where Id=" + mPrcs.relatedId + "";
            }

            if (comService.ExecuteSql(sql) == 0)
            {
                err = "FlowRunPrcsList修改当前数据失败！";
            }
            else
            {
                if (comService.ExecuteSql("update flowrunlist set currentPrcsUser1='" + userId + "',transactUser1=  '" + transactUser + "',runFlag='2'  where listid=" + mPrcs.ListId + "", null, "") == 0)
                {
                    err = "FlowRunList修改当前数据失败！";
                }
            }

            return err;
        }

        /// <summary>
        /// 固定流程转交
        /// </summary>
        /// <param name="user">当前用户信息</param>
        /// <param name="listid">数据id</param>
        /// <param name="nextprcid">选择的流程下一步骤</param>
        /// <param name="selectuserid">选择的用户</param>
        /// <param name="delivercomments">评论意见</param>
        /// <param name="ispublic">是否公开意见</param>
        /// <returns></returns>
        public static string SetFlowPrcsNext01(CurrentUserEntity user, string listid, string nextprcid, string selectuserid, string delivercomments, string ispublic, string from = null)
        {
            //1：修改FlowRunPrcsList中deliverUserId转交人,deliverTime转交时间,runFlag状态改为3, stepInfo步骤信息
            //2：FlowRunPrcsList添加并返回id listid(与flowrunlist关联) PrcsId(转交的下一步骤id)deliverType(转交方式)deliverComments(转交意见)Ispublic(意见是否公开)
            //3：FlowRunList修改状态信息 包括currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv

            string err;
            string stepInfo;
            int itemId = 0;
            string account = user == null ? "" : user.Account;
            FlowRunListEntity mRun = new FlowRunListEntity();

            ServiceFlowRunList flowRunList = new ServiceFlowRunList();
            FlowRunPrcsList flowRunPrcsList = new FlowRunPrcsList();

            CurrentPrcsEntity mPrcs = new CurrentPrcsEntity();
            mPrcs.FlowId = "";
            mPrcs.ListId = 0;
            mPrcs.runFlag = "2";// runFlag流程步骤标志位--0待新建1待接手2办理中3已办结
            mPrcs.relatedId = 0;
            mPrcs.PrcsId = "-1";

            if (!string.IsNullOrEmpty(account))
            {
                DataTable dtlist = comService.GetDataTable("flowrunlist", "*", "where listid=@listid", "", new { listid = listid });
                if (dtlist.Rows.Count > 0)
                {
                    mPrcs.ListId = int.Parse(listid);

                    mPrcs.FlowId = dtlist.Rows[0]["FlowId"].ToString();//当前流程编码
                    mPrcs.PrcsId = dtlist.Rows[0]["currentPrcsId"].ToString();//当前流程步骤
                    mPrcs.relatedId = int.Parse(dtlist.Rows[0]["relatedId"].ToString());//当前对应的FlowRunPrcsList
                    mPrcs.runFlag = dtlist.Rows[0]["runFlag"].ToString();

                    //选择的下一步 选择了结束
                    if (nextprcid == "0")
                    {
                        mPrcs.isLock = "1";
                        mPrcs.PrcsId = "0";
                        mPrcs.PrcsNo = "_finish";
                        mPrcs.PrcsName = "结束";
                        mPrcs.PrcsOrder = "0";
                        mPrcs.PrcsIndex = "";
                        mPrcs.HiddenIndex = "";
                        mPrcs.PrcsUser = "";
                        mPrcs.PrcsDept = "";
                        mPrcs.PrcsPriv = "";
                    }
                    else
                    {
                        //注意取的是下一步骤的nextprcid
                        DataTable dt = comService.GetDataTable("flowprcs", "*", "where PrcsId=@PrcsId", "", new { PrcsId = nextprcid });
                        if (dt.Rows.Count > 0)
                        {
                            mPrcs.isLock = "0";
                            mPrcs.PrcsNo = dt.Rows[0]["PrcsNo"].ToString();
                            mPrcs.PrcsName = dt.Rows[0]["PrcsName"].ToString();
                            mPrcs.PrcsOrder = dt.Rows[0]["PrcsOrder"].ToString();
                            mPrcs.PrcsIndex = dt.Rows[0]["PrcsIndex"].ToString();
                            mPrcs.HiddenIndex = dt.Rows[0]["HiddenIndex"].ToString();
                            mPrcs.PrcsUser = dt.Rows[0]["PrcsUser"].ToString();
                            mPrcs.PrcsDept = dt.Rows[0]["PrcsDept"].ToString();
                            mPrcs.PrcsPriv = dt.Rows[0]["PrcsPriv"].ToString();
                        }
                        else
                        {
                            mPrcs.ERR = "数据表flowprcs中id=" + nextprcid + "的流程步骤（下一步）为空";
                        }
                    }

                    if (string.IsNullOrEmpty(mPrcs.ERR))
                    {
                        string strAllPrcs = mPrcs.PrcsUser + mPrcs.PrcsDept + mPrcs.PrcsPriv;
                        if (strAllPrcs.Replace(";", "").Trim().Replace("#", "").Trim() == "" && selectuserid.Replace(";", "").Trim() == "" && nextprcid != "0")
                        {
                            mPrcs.ERR = "该流程未设置办理人，请联系管理员或者选择办理人才能继续！";
                        }
                        else
                        {
                            DataTable dtPrcsList = comService.GetDataTable("flowrunprcslist", "*", "where id=@id", "", new { id = mPrcs.relatedId });
                            if (dtPrcsList.Rows.Count > 0)
                            {
                                stepInfo = dtPrcsList.Rows[0]["stepInfo"].ToString();
                                if (nextprcid == "0")
                                {
                                    if (string.IsNullOrEmpty(from))
                                    {
                                        stepInfo += " {" + account + "于" + DateTime.Now + " 结束流程}  ";
                                    }
                                    else
                                    {
                                        stepInfo += " {" + account + "于" + DateTime.Now + " 结束流程 - <通过工作台管理页面操作>}  ";
                                    }
                                }
                                else
                                {
                                    //未选择办理人
                                    if (selectuserid == "")
                                    {
                                        if (string.IsNullOrEmpty(from))
                                        {
                                            stepInfo += " {" + account + "于" + DateTime.Now + " 转交(谁接收由谁办理)}  ";
                                        }
                                        else
                                        {
                                            stepInfo += " {" + account + "于" + DateTime.Now + " 转交(谁接收由谁办理)- <通过工作台管理页面操作>}  ";
                                        }
                                    }
                                    else
                                    {
                                        string sNextUser = " 选择办理人：" + selectuserid;
                                        if (string.IsNullOrEmpty(from))
                                        {
                                            stepInfo += " {" + account + "于" + DateTime.Now + sNextUser + "并转交}  ";
                                        }
                                        else
                                        {
                                            stepInfo += " {" + account + "于" + DateTime.Now + sNextUser + "并转交- <通过工作台管理页面操作>}  ";
                                        }
                                    }
                                }

                                //1：修改FlowRunPrcsList中deliverUserId转交人,deliverTime转交时间,runFlag状态改为3, stepInfo步骤信息
                                if (comService.ExecuteSql("update flowrunprcslist set deliverUserId='" + account + "',deliverTime='" + DateTime.Now + "',runFlag='3', stepInfo = '" + stepInfo + "', deliverComments = '" + delivercomments + "'  where Id=" + mPrcs.relatedId + "", null, "") == 0)
                                {
                                    mPrcs.ERR = "FlowRunPrcsList修改当前数据失败！";
                                }

                                FlowRunPrcsListEntity mRunItem = new FlowRunPrcsListEntity();
                                mRunItem.FlowId = mPrcs.FlowId;
                                mRunItem.listid = int.Parse(listid);
                                mRunItem.PrcsId = int.Parse(nextprcid);
                                mRunItem.deliverType = "1";
                                mRunItem.deliverComments = "";
                                mRunItem.Ispublic = ispublic;
                                if (nextprcid == "0")//选择了结束
                                {
                                    mRunItem.FlowPrcs = "结束";
                                    mRunItem.runFlag = "3";
                                    mRunItem.stepInfo = "{" + account + "于" + DateTime.Now.ToString() + "结束}  ";
                                }
                                else
                                {
                                    mRunItem.FlowPrcs = mPrcs.PrcsName;
                                    mRunItem.runFlag = "1";
                                    mRunItem.stepInfo = "";
                                }

                                //2：FlowRunPrcsList添加并返回id listid(与flowrunlist关联) PrcsId(转交的下一步骤id)deliverType(转交方式)deliverComments(转交意见)Ispublic(意见是否公开)
                                itemId = flowRunPrcsList.InsertReturnID(mRunItem);

                                if (itemId != 0)
                                {
                                    mRun.relatedId = itemId;
                                    mRun.listid = long.Parse(listid);
                                    mRun.currentPrcsUser1 = "";
                                    if (nextprcid == "0")//选择了结束
                                    {
                                        mRun.currentPrcsId = 0;
                                        mRun.currentPrcsName = "结束";
                                        mRun.runFlag = "3";
                                        mRun.isFinish = "1";
                                        mRun.isLock = "1";

                                        mRun.currentPrcsUser = "";
                                        mRun.currentPrcsDept = "";
                                        mRun.currentPrcsPriv = "";
                                    }
                                    else
                                    {
                                        mRun.currentPrcsId = int.Parse(nextprcid);
                                        mRun.currentPrcsName = mPrcs.PrcsName;
                                        mRun.runFlag = "1";
                                        mRun.isFinish = "2";
                                        mRun.isLock = "0";

                                        mRun.currentPrcsUser = selectuserid;
                                        //未选择办理人
                                        if (selectuserid.Replace(";", "").Trim() == "")
                                        {
                                            mRun.currentPrcsUser = mPrcs.PrcsUser;
                                            mRun.currentPrcsDept = mPrcs.PrcsDept;
                                            mRun.currentPrcsPriv = mPrcs.PrcsPriv;
                                        }
                                    }

                                    //3：FlowRunList修改状态信息 包括currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv
                                    if (string.IsNullOrEmpty(mPrcs.ERR))
                                    {
                                        if (flowRunList.UpdateByWhere("where listid=" + listid + "", "currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv,currentPrcsName", mRun) == 0)
                                        {
                                            mPrcs.ERR = "FlowRunList修改当前数据失败";
                                        }
                                    }
                                }
                                else
                                {
                                    mPrcs.ERR = "FlowRunPrcsList新增数据失败";
                                }
                            }
                            else
                            {
                                mPrcs.ERR = "FlowRunPrcsList当前数据不存在";
                            }
                        }
                    }
                }
                else
                {
                    mPrcs.ListId = int.Parse(listid);
                    mPrcs.ERR = "FlowRunList数据表不存在该数据！";
                }

                err = mPrcs.ERR == null ? "" : mPrcs.ERR;
            }
            else
            {
                err = "当前用户对象为空，请重新登录";
            }

            return err;
        }

        /// <summary>
        /// 自由流程转交
        /// </summary>
        /// <param name="user">当前用户信息</param>
        /// <param name="listid">listid</param>
        /// <param name="nextprcid">0表示结束 非0表示转交下一步</param>
        /// <param name="selectuserid">选择交办的用户</param>
        /// <param name="delivercomments">转交意见</param>
        /// <param name="delivercomments">转交意见意见是否公开</param>
        /// <returns></returns>
        public static string SetFlowPrcsNext02(CurrentUserEntity user, string listid, string nextprcid, string selectuserid, string delivercomments, string ispublic, string from = null)
        {
            //1：修改FlowRunPrcsList中deliverUserId转交人,deliverTime转交时间,runFlag状态改为3, stepInfo步骤信息
            //2：FlowRunPrcsList添加并返回id listid(与flowrunlist关联) PrcsId(转交的下一步骤id)deliverType(转交方式)deliverComments(转交意见)Ispublic(意见是否公开)
            //3：FlowRunList修改状态信息 包括currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv

            string err;
            string stepinfo;
            int itemid = 0;
            int addid = 0;
            string account = user == null ? "" : user.Account;
            FlowRunListEntity mRun = new FlowRunListEntity();

            ServiceFlowRunList flowRunList = new ServiceFlowRunList();
            FlowRunPrcsList flowRunPrcsList = new FlowRunPrcsList();

            CurrentPrcsEntity mPrcs = new CurrentPrcsEntity();
            mPrcs.FlowId = "";
            mPrcs.ListId = 0;
            mPrcs.runFlag = "2";
            mPrcs.relatedId = 0;
            mPrcs.PrcsId = "-1";//flowprcs对应的主键id

            if (!string.IsNullOrEmpty(account))
            {
                DataTable dtlist = comService.GetDataTable("flowrunlist", "*", "where listid=@listid", "", new { listid = listid });
                if (dtlist.Rows.Count > 0)
                {
                    mPrcs.ListId = int.Parse(listid);
                    mPrcs.FlowId = dtlist.Rows[0]["FlowId"].ToString();//当前流程编码
                    mPrcs.PrcsId = dtlist.Rows[0]["currentPrcsId"].ToString();//当前流程步骤
                    mPrcs.relatedId = int.Parse(dtlist.Rows[0]["relatedId"].ToString());//当前对应的FlowRunPrcsList
                    mPrcs.runFlag = dtlist.Rows[0]["runFlag"].ToString();

                    addid = int.Parse(mPrcs.PrcsId) + 1;

                    //选择的下一步 选择了结束
                    if (nextprcid == "0")
                    {
                        mPrcs.PrcsId = "0";
                        mPrcs.PrcsNo = "_finish";
                        mPrcs.PrcsName = "结束";
                        mPrcs.PrcsOrder = "0";
                        mPrcs.PrcsIndex = "";
                        mPrcs.HiddenIndex = "";
                        mPrcs.PrcsUser = "";
                        mPrcs.PrcsDept = "";
                        mPrcs.PrcsPriv = "";
                    }
                    else
                    {
                        mPrcs.PrcsId = addid.ToString();
                        mPrcs.PrcsNo = "step_" + addid.ToString();
                        mPrcs.PrcsName = "第" + addid.ToString() + "步";
                        mPrcs.PrcsOrder = addid.ToString();
                        mPrcs.PrcsIndex = "#all#";
                        mPrcs.HiddenIndex = "";
                        mPrcs.PrcsUser = ";" + selectuserid + ";";
                        mPrcs.PrcsDept = "";
                        mPrcs.PrcsPriv = "";
                    }

                    DataTable dtPrcsList = comService.GetDataTable("flowrunprcslist", "*", "where id=@id", "", new { id = mPrcs.relatedId });
                    if (dtPrcsList.Rows.Count > 0)
                    {
                        stepinfo = dtPrcsList.Rows[0]["stepInfo"].ToString();
                        if (nextprcid == "0")
                        {
                            if (string.IsNullOrEmpty(from))
                            {
                                stepinfo += " {" + account + "于" + DateTime.Now + " 结束流程}  ";
                            }
                            else
                            {
                                stepinfo += " {" + account + "于" + DateTime.Now + " 结束流程 - <通过工作台管理页面操作>}  ";
                            }
                        }
                        else
                        {
                            string sNextUser = " 选择办理人：" + selectuserid;

                            if (string.IsNullOrEmpty(from))
                            {
                                stepinfo += " {" + account + "于" + DateTime.Now + sNextUser + "并转交} ";
                            }
                            else
                            {
                                stepinfo += " {" + account + "于" + DateTime.Now + sNextUser + "并转交 - <通过工作台管理页面操作>}  ";
                            }
                        }

                        //1：修改FlowRunPrcsList中deliverUserId转交人,deliverTime转交时间,runFlag状态改为3, stepInfo步骤信息
                        if (comService.ExecuteSql("update flowrunprcslist set deliverUserId='" + account + "',deliverTime='" + DateTime.Now + "',runFlag='3', stepInfo = '" + stepinfo + "', deliverComments = '" + delivercomments + "'  where Id=" + mPrcs.relatedId + "", null, "") == 0)
                        {
                            mPrcs.ERR = "FlowRunPrcsList修改当前数据失败！";
                        }

                        FlowRunPrcsListEntity mRunItem = new FlowRunPrcsListEntity();
                        mRunItem.FlowId = mPrcs.FlowId;
                        mRunItem.listid = int.Parse(listid);
                        mRunItem.deliverType = "1";
                        mRunItem.deliverComments = "";
                        mRunItem.Ispublic = ispublic;
                        if (nextprcid == "0")//选择了结束
                        {
                            mRunItem.PrcsId = 0;
                            mRunItem.FlowPrcs = "结束";
                            mRunItem.runFlag = "3";
                            mRunItem.stepInfo = "{" + account + "于" + DateTime.Now.ToString() + "结束}  ";
                        }
                        else
                        {
                            mRunItem.PrcsId = addid;
                            mRunItem.FlowPrcs = mPrcs.PrcsName;
                            mRunItem.runFlag = "1";
                            mRunItem.stepInfo = "";
                        }

                        //2：FlowRunPrcsList添加并返回id listid(与flowrunlist关联) PrcsId(转交的下一步骤id)deliverType(转交方式)deliverComments(转交意见)Ispublic(意见是否公开)
                        itemid = flowRunPrcsList.InsertReturnID(mRunItem);

                        if (itemid != 0)
                        {
                            mRun.relatedId = itemid;
                            mRun.listid = long.Parse(listid);
                            mRun.currentPrcsUser1 = "";
                            if (nextprcid == "0")//选择了结束
                            {
                                mRun.currentPrcsId = 0;
                                mRun.currentPrcsName = "结束";
                                mRun.runFlag = "3";
                                mRun.isFinish = "1";
                                mRun.isLock = "1";

                                mRun.currentPrcsUser = "";
                                mRun.currentPrcsDept = "";
                                mRun.currentPrcsPriv = "";
                            }
                            else
                            {
                                mRun.currentPrcsId = addid;
                                mRun.currentPrcsName = mPrcs.PrcsName;
                                mRun.runFlag = "1";
                                mRun.isFinish = "2";
                                mRun.isLock = "0";

                                mRun.currentPrcsUser = ";" + selectuserid + ";";
                                mRun.currentPrcsDept = "";
                                mRun.currentPrcsPriv = "";
                            }

                            //3：FlowRunList修改状态信息 包括currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv
                            if (string.IsNullOrEmpty(mPrcs.ERR))
                            {
                                if (flowRunList.UpdateByWhere("where listid=" + listid + "", "currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv,currentPrcsName", mRun) == 0)
                                {
                                    mPrcs.ERR = "FlowRunList修改当前数据失败";
                                }
                            }
                        }
                        else
                        {
                            mPrcs.ERR = "FlowRunPrcsList新增数据失败";
                        }
                    }
                    else
                    {
                        mPrcs.ERR = "FlowRunPrcsList当前数据不存在";
                    }
                }
                else
                {
                    mPrcs.ListId = int.Parse(listid);
                    mPrcs.ERR = "FlowRunList数据表不存在该数据！";
                }

                err = mPrcs.ERR == null ? "" : mPrcs.ERR;
            }
            else
            {
                err = "当前用户对象为空，请重新登录";
            }

            return err;
        }

        /// <summary>
        /// 固定流程回退
        /// </summary>
        /// <param name="user">当前用户信息</param>
        /// <param name="listid">listid</param>
        /// <param name="backprcid"></param>
        /// <param name="selectuserid"></param>
        /// <param name="deliverComments"></param>
        /// <returns></returns>
        public static string SetFlowPrcsBack01(CurrentUserEntity user, string listid, string nextprcid, string selectuserid, string delivercomments, string ispublic)
        {
            //1：修改FlowRunPrcsList中deliverUserId转交人,deliverTime转交时间,runFlag状态改为3, stepInfo步骤信息
            //2：FlowRunPrcsList添加并返回id listid(与flowrunlist关联) PrcsId(转交的下一步骤id)deliverType(转交方式)deliverComments(转交意见)Ispublic(意见是否公开)
            //3：FlowRunList修改状态信息 包括currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv

            string err;
            string stepinfo;
            int itemid = 0;
            FlowRunListEntity mRun = new FlowRunListEntity();

            ServiceFlowRunList flowRunList = new ServiceFlowRunList();
            FlowRunPrcsList flowRunPrcsList = new FlowRunPrcsList();

            CurrentPrcsEntity mPrcs = new CurrentPrcsEntity();
            mPrcs.FlowId = "";
            mPrcs.ListId = 0;
            mPrcs.runFlag = "2";
            mPrcs.relatedId = 0;
            mPrcs.PrcsId = "-1";

            DataTable dtlist = comService.GetDataTable("flowrunlist", "*", "where listid=@listid", "", new { listid = listid });
            if (dtlist.Rows.Count > 0)
            {
                mPrcs.ListId = int.Parse(listid);

                mPrcs.FlowId = dtlist.Rows[0]["FlowId"].ToString();//当前流程编码
                mPrcs.PrcsId = dtlist.Rows[0]["currentPrcsId"].ToString();//当前流程步骤
                mPrcs.relatedId = int.Parse(dtlist.Rows[0]["relatedId"].ToString());//当前对应的FlowRunPrcsList
                mPrcs.runFlag = dtlist.Rows[0]["runFlag"].ToString();

                //选择的下一步 选择了结束
                if (nextprcid == "0")
                {
                    mPrcs.PrcsNo = "0";
                    mPrcs.PrcsName = "结束";
                    mPrcs.PrcsOrder = "0";
                    mPrcs.PrcsIndex = "";
                    mPrcs.HiddenIndex = "";
                    mPrcs.PrcsUser = "";
                    mPrcs.PrcsDept = "";
                    mPrcs.PrcsPriv = "";
                }
                else
                {
                    //注意取的是下一步骤的nextprcid
                    DataTable dt = comService.GetDataTable("flowprcs", "*", "where PrcsId=@PrcsId", "", new { PrcsId = nextprcid });
                    if (dt.Rows.Count > 0)
                    {
                        mPrcs.PrcsNo = dt.Rows[0]["PrcsNo"].ToString();
                        mPrcs.PrcsName = dt.Rows[0]["PrcsName"].ToString();
                        mPrcs.PrcsOrder = dt.Rows[0]["PrcsOrder"].ToString();
                        mPrcs.PrcsIndex = dt.Rows[0]["PrcsIndex"].ToString();
                        mPrcs.HiddenIndex = dt.Rows[0]["HiddenIndex"].ToString();
                        mPrcs.PrcsUser = dt.Rows[0]["PrcsUser"].ToString();
                        mPrcs.PrcsDept = dt.Rows[0]["PrcsDept"].ToString();
                        mPrcs.PrcsPriv = dt.Rows[0]["PrcsPriv"].ToString();
                    }
                    else
                    {
                        mPrcs.ERR = "数据表flowprcs中id=" + nextprcid + "的步骤对象（下一步）为空";
                    }
                }

                if (string.IsNullOrEmpty(mPrcs.ERR))
                {
                    DataTable dtPrcsList = comService.GetDataTable("flowrunprcslist", "*", "where id=@id", "", new { id = mPrcs.relatedId });
                    if (dtPrcsList.Rows.Count > 0)
                    {
                        stepinfo = dtPrcsList.Rows[0]["stepInfo"].ToString();
                        if (nextprcid == "0")
                        {
                            stepinfo += " {" + user.Account + "于" + DateTime.Now + " 回退结束流程}  ";
                        }
                        else
                        {
                            string sNextUser = " 回退：" + selectuserid;
                            stepinfo += " {" + user.Account + "于" + DateTime.Now + sNextUser + "}  ";
                        }

                        //1：修改FlowRunPrcsList中deliverUserId转交人,deliverTime转交时间,runFlag状态改为3, stepInfo步骤信息
                        if (comService.ExecuteSql("update flowrunprcslist set deliverUserId='" + user.Account + "',deliverTime='" + DateTime.Now + "',runFlag='3', stepInfo = '" + stepinfo + "', deliverComments = '" + delivercomments + "'  where Id=" + mPrcs.relatedId + "", null, "") == 0)
                        {
                            mPrcs.ERR = "FlowRunPrcsList修改当前数据失败！";
                        }

                        FlowRunPrcsListEntity mRunItem = new FlowRunPrcsListEntity();

                        mRunItem.FlowId = mPrcs.FlowId;
                        mRunItem.listid = int.Parse(listid);
                        mRunItem.PrcsId = int.Parse(nextprcid);
                        mRunItem.deliverType = "2";
                        mRunItem.deliverComments = "";
                        mRunItem.Ispublic = ispublic;
                        if (nextprcid == "0")//选择了结束
                        {
                            mRunItem.FlowPrcs = "结束";
                            mRunItem.runFlag = "3";
                            mRunItem.stepInfo = "{" + user.Account + "于" + DateTime.Now.ToString() + "回退 - 结束}  ";
                        }
                        else
                        {
                            mRunItem.FlowPrcs = mPrcs.PrcsName;
                            mRunItem.runFlag = "1";
                            mRunItem.stepInfo = "{" + user.Account + "于" + DateTime.Now.ToString() + "回退 - " + mRunItem.FlowPrcs + "}  ";
                        }

                        //2：FlowRunPrcsList添加并返回id listid(与flowrunlist关联) PrcsId(转交的下一步骤id)deliverType(转交方式)deliverComments(转交意见)Ispublic(意见是否公开)
                        itemid = flowRunPrcsList.InsertReturnID(mRunItem);

                        if (itemid != 0)
                        {
                            mRun.relatedId = itemid;
                            mRun.listid = long.Parse(listid);
                            mRun.currentPrcsUser1 = "";
                            if (nextprcid == "0")//选择了结束
                            {
                                mRun.currentPrcsId = 0;
                                mRun.currentPrcsName = "结束";
                                mRun.runFlag = "3";
                                mRun.isFinish = "1";
                                mRun.isLock = "1";

                                mRun.currentPrcsUser = "";
                                mRun.currentPrcsDept = "";
                                mRun.currentPrcsPriv = "";
                            }
                            else
                            {
                                mRun.currentPrcsId = int.Parse(nextprcid);
                                mRun.currentPrcsName = mPrcs.PrcsName;
                                mRun.runFlag = "1";
                                mRun.isFinish = "2";
                                mRun.isLock = "0";

                                mRun.currentPrcsUser = ";" + selectuserid + ";";
                                mRun.currentPrcsDept = "";
                                mRun.currentPrcsPriv = "";
                            }

                            //3：FlowRunList修改状态信息 包括currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv
                            if (string.IsNullOrEmpty(mPrcs.ERR))
                            {
                                if (flowRunList.UpdateByWhere("where listid=" + listid + "", "currentPrcsId,relatedId,currentPrcsUser1,runFlag,isFinish,isLock,currentPrcsUser,currentPrcsDept,currentPrcsPriv,currentPrcsName", mRun) == 0)
                                {
                                    mPrcs.ERR = "FlowRunList修改当前数据失败";
                                }
                            }
                        }
                        else
                        {
                            mPrcs.ERR = "FlowRunPrcsList新增数据失败";
                        }
                    }
                    else
                    {
                        mPrcs.ERR = "FlowRunPrcsList当前数据不存在";
                    }
                }
            }
            else
            {
                mPrcs.ListId = int.Parse(listid);
                mPrcs.ERR = "FlowRunList数据表不存在该数据！";
            }

            err = mPrcs.ERR == null ? "" : mPrcs.ERR;

            return err;
        }

        //获取当前流程步骤编码
        public static string GetFlowNoByID(string flowid, string pid)
        {
            string flno = "";
            if (flowid.StartsWith("fw_"))
            {
                FlowBase flowService = new FlowBase();

                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = flowid });
                if (mflow != null)
                {
                    //固定流程
                    if (mflow.flowType == "1")
                    {
                        if (pid == "0")
                        {
                            flno = "_finish";
                        }
                        else
                        {
                            DataTable dt = comService.GetDataTable("flowprcs", "*", "where PrcsId=@PrcsId", "", new { PrcsId = pid });
                            if (dt.Rows.Count > 0)
                            {
                                flno = dt.Rows[0]["PrcsNo"].ToString();
                            }
                            else
                            {
                                flno = "_nonexistent";
                            }
                        }
                    }
                    else
                    {
                        if (pid == "0")
                        {
                            flno = "_finish";
                        }
                        else
                        {
                            flno = "_step" + pid;
                        }
                    }
                }
            }

            return flno;
        }

        //获取当前流程步骤编码
        public static string GetFlowIDByNo(string flowid, string pno)
        {
            string fwid = "-1";

            if (flowid.StartsWith("fw_"))
            {
                FlowBase flowService = new FlowBase();
                FlowEntity mflow = flowService.GetByWhereFirst("where FlowId=@FlowId  ", new { FlowId = flowid });
                if (mflow != null)
                {
                    //固定流程
                    if (mflow.flowType == "1")
                    {
                        if (pno == "_finish")
                        {
                            fwid = "0";
                        }
                        else
                        {//
                            DataTable dt = comService.GetDataTable("flowprcs", "*", "where FlowId=@FlowId and PrcsNo=@PrcsNo", "", new { PrcsNo = pno, FlowId = flowid });
                            if (dt.Rows.Count > 0)
                            {
                                fwid = dt.Rows[0]["PrcsId"].ToString();
                            }
                        }
                    }
                }
            }

            return fwid;
        }
    }
}
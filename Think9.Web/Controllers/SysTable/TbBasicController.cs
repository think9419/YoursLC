using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Com;
using Think9.Services.Flow;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class TbBasicController : BaseController
    {
        private ComService comService = new ComService();
        private SortService sortService = new SortService();
        private TbBasicService tbBasicService = new TbBasicService();
        private FlowService flowService = new FlowService();
        private IndexDateType indexDtaeTypeService = new IndexDateType();
        private ModuleService moduleService = new ModuleService();
        private TbSearchModeService tbSearchMode = new TbSearchModeService();
        private string split = Think9.Services.Base.BaseConfig.ComSplit;//字符分割 用于多选项的分割等

        public JsonResult GetsearchMode(string flowId)
        {
            DataTable dt = comService.GetDataTable("select id as id, name as name from sys_searchmode");
            string searchMode_Exa = "";
            foreach (DataRow dr in comService.GetDataTable("select * from tbsearchmode where TbId='" + flowId.Replace("bi_", "tb_").Replace("fw_", "tb_") + "'").Rows)
            {
                string typeId = dr["TypeId"].ToString();
                foreach (DataRow drmode in dt.Rows)
                {
                    if (typeId == drmode["id"].ToString())
                    {
                        typeId = drmode["name"].ToString();
                        break;
                    }
                }
                searchMode_Exa += "{" + dr["UserId"].ToString() + "" + typeId + "} ";
            }
            var result = SuccessTip("", searchMode_Exa);

            return Json(result);
        }

        public ActionResult TbSearchMode(string tbid)
        {
            if (string.IsNullOrEmpty(tbid))
            {
                return Json("参数错误！");
            }

            if (tbid.StartsWith("bi_") || tbid.StartsWith("fw_"))
            {
                tbid = tbid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            }

            DataTable dt = DataTableHelp.NewValueTextDt();
            foreach (DataRow dr in comService.GetDataTable("select Account as id, RealName as name from sys_users").Rows)
            {
                DataRow row = dt.NewRow();
                row["ClassID"] = "users";
                row["Value"] = dr["id"].ToString();
                row["Text"] = dr["id"].ToString() + dr["name"].ToString();
                dt.Rows.Add(row);
            }
            foreach (DataRow dr in comService.GetDataTable("select id as id, name as name from sys_searchmode").Rows)
            {
                DataRow row = dt.NewRow();
                row["ClassID"] = "sys_searchmode";
                row["Value"] = dr["id"].ToString();
                row["Text"] = dr["name"].ToString();
                dt.Rows.Add(row);
            }
            ViewBag.UserList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
            ViewBag.SelectList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
            ViewBag.TbId = tbid;

            return View();
        }

        public ActionResult GetTbSearchModeList(string tbid)
        {
            DataTable dt = comService.GetDataTable("select id as id, name as name from sys_searchmode");
            var list = tbSearchMode.GetByWhere("where TbId='" + tbid + "'", null, null, "").ToList();
            foreach (TbSearchModeEntity obj in list)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (obj.TypeId == dr["id"].ToString())
                    {
                        obj.TypeId = dr["name"].ToString();
                        break;
                    }
                }
            }

            var result = new { code = 0, count = 0, data = list };
            return Json(result);
        }

        public JsonResult DelSearchMode(string tbid, string idsStr)
        {
            string id = "";
            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    id = arr[i].ToString().Trim();
                    string where = "where TbId='" + tbid + "' and UserId='" + id + "'";

                    tbSearchMode.DeleteByWhere(where);
                }
            }
            return Json(SuccessTip("删除成功"));
        }

        public ActionResult AddTbSearchMode(string tbid, string userid, string typeid)
        {
            if (comService.GetTotal("tbsearchmode", "where UserId=@UserId and TbId=@TbId", new { TbId = tbid, UserId = userid }) > 0)
            {
                return Json(ErrorTip("重复添加"));
            }
            try
            {
                if (tbSearchMode.Insert(new TbSearchModeEntity { TbId = tbid, UserId = userid, TypeId = typeid }))
                {
                    return Json(SuccessTip("添加成功"));
                }
                else
                {
                    return Json(ErrorTip("添加失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public JsonResult GetModelType(string tbId)
        {
            return Json(tbBasicService.GetTbModelTypeByTbId(tbId));
        }

        [HttpGet]
        public ActionResult TbUtility(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            decimal? Width = 22M;
            decimal? Heigh = 29.7M;
            decimal? Top = 1.5M;
            decimal? Left = 0.5M;
            decimal? Right = 0.5M;
            decimal? Bottom = 1.5M;
            TbPdfSize tbPdfSize = new TbPdfSize();
            TbPdfSizeEntity mPdfSize = tbPdfSize.GetByWhereFirst("where TbId = '" + tbid + "' and  Type='formpdf'");
            if (mPdfSize != null)
            {
                Width = mPdfSize.Width;
                Heigh = mPdfSize.Heigh;
                Top = mPdfSize.Top;
                Left = mPdfSize.Left;
                Right = mPdfSize.Right;
                Bottom = mPdfSize.Bottom;
            }

            ViewBag.Width = Width;
            ViewBag.Heigh = Heigh;
            ViewBag.Top = Top;
            ViewBag.Left = Left;
            ViewBag.Right = Right;
            ViewBag.Bottom = Bottom;

            ViewBag.tbid = tbid;
            return View();
        }

        public ActionResult OpenModule(string tbid, string name)
        {
            string str = "_TB?tbid=" + tbid;

            DataTable dt = DataTableHelp.NewValueTextDt();
            DataTable dtRole = comService.GetDataTable("select * from sys_role");
            foreach (DataRow dr in dtRole.Rows)
            {
                DataRow row = dt.NewRow();
                row["ClassID"] = "sys_role";
                row["Value"] = dr["Id"].ToString();
                row["Text"] = dr["FullName"].ToString();
                dt.Rows.Add(row);
            }
            ViewBag.Split = split;//字符分割 checkbox多选时使用
            ViewBag.RoleList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
            ViewBag.SelectList = moduleService.GetSelectTreeList();
            ViewBag.Frm = "tb";

            ModuleEntity model = moduleService.GetByWhereFirst("where UrlAddress='" + tbid.Replace("tb_", "") + "' OR UrlAddress='/" + tbid.Replace("tb_", "") + "' OR UrlAddress='" + str + "' OR UrlAddress='/" + str + "'");

            if (model != null)
            {
                ViewBag.PId = model.ParentId;

                str = split;
                foreach (DataRow dr in dtRole.Rows)
                {
                    if (comService.GetTotal("sys_roleauthorize", "where RoleId=" + dr["id"].ToString().Trim() + " and ModuleId=" + model.Id + "") > 0)
                    {
                        str += dr["id"].ToString().Trim() + split;
                    }
                }
                model.RoleStr = str;
                return View("~/Areas/SysBasic/Module/Edit.cshtml", model);
            }
            else
            {
                model = new ModuleEntity();
                model.UrlAddress = "_TB?tbid=" + tbid;
                model.FullName = name;
                model.OrderNo = 9;
                return View("~/Areas/SysBasic/Module/Add.cshtml", model);
            }
        }

        public ActionResult OpenModule2(string rpId, string name)
        {
            string str = "_RP?rpid=" + rpId;

            DataTable dt = DataTableHelp.NewValueTextDt();
            DataTable dtRole = comService.GetDataTable("select * from sys_role");
            foreach (DataRow dr in dtRole.Rows)
            {
                DataRow row = dt.NewRow();
                row["ClassID"] = "sys_role";
                row["Value"] = dr["Id"].ToString();
                row["Text"] = dr["FullName"].ToString();
                dt.Rows.Add(row);
            }
            ViewBag.Split = split;//字符分割 checkbox多选时使用
            ViewBag.RoleList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
            ViewBag.SelectList = moduleService.GetSelectTreeList();
            ViewBag.Frm = "rp";

            ModuleEntity model = moduleService.GetByWhereFirst("where UrlAddress='" + rpId.Replace("rp_", "") + "' OR UrlAddress='/" + rpId.Replace("rp_", "") + "' OR UrlAddress='" + str + "' OR UrlAddress='/" + str + "'");

            if (model != null)
            {
                ViewBag.PId = model.ParentId;

                str = split;
                foreach (DataRow dr in dtRole.Rows)
                {
                    if (comService.GetTotal("sys_roleauthorize", "where RoleId=" + dr["id"].ToString().Trim() + " and ModuleId=" + model.Id + "") > 0)
                    {
                        str += dr["id"].ToString().Trim() + split;
                    }
                }
                model.RoleStr = str;
                return View("~/Areas/SysBasic/Module/Edit.cshtml", model);
            }
            else
            {
                model = new ModuleEntity();
                model.UrlAddress = "_RP?rpid=" + rpId;
                model.FullName = name;
                model.OrderNo = 9;
                return View("~/Areas/SysBasic/Module/Add.cshtml", model);
            }
        }

        public ActionResult GetTbButDisableListForOrganize(string tbid, string btnid, string pageType)
        {
            var result = new { code = 0, count = 0, data = tbBasicService.GetButDisableListForOrganize(tbid, btnid, pageType) };
            return Json(result);
        }

        public ActionResult GetTbButDisableListForRole(string tbid, string btnid, string pageType)
        {
            var result = new { code = 0, count = 0, data = tbBasicService.GetButDisableListForRole(tbid, btnid, pageType) };
            return Json(result);
        }

        public ActionResult GetTbButDisableListForUser(string tbid, string btnid, string pageType)
        {
            var result = new { code = 0, count = 0, data = tbBasicService.GetButDisableListForUser(tbid, btnid, pageType) };
            return Json(result);
        }

        [HttpPost]
        public ActionResult EditTbButDisable(string tbid, string btnid, string pageType, List<TbButDisableEntity> list)
        {
            TbButDisableService tbButDisable = new TbButDisableService();
            comService.ExecuteSql("delete from tbbutdisable where tbid = '" + tbid + "' and  btnid = '" + btnid + "' and  pageType = '" + pageType + "'");
            foreach (TbButDisableEntity obj in list)
            {
                obj.TbId = tbid;
                obj.BtnId = btnid;
                obj.PageType = pageType;
                obj.GridId = "";

                tbButDisable.Insert(obj);
            }

            return Json(SuccessTip("编辑成功，属性已更改无需『重新生成』"));
        }

        public ActionResult AddMain()
        {
            ViewBag.TbSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");
            ViewBag.DtaeTypeList = indexDtaeTypeService.GetIndexDtaeTypeList();
            ViewBag.IndexSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_index"), "SortID", "SortName");
            ViewBag.ClassID = "CAT_table";
            return View();
        }

        public ActionResult AddAux()
        {
            ViewBag.TbSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");
            ViewBag.DtaeTypeList = indexDtaeTypeService.GetIndexDtaeTypeList();
            ViewBag.IndexSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_index"), "SortID", "SortName");
            ViewBag.ClassID = "CAT_table";
            return View();
        }

        public ActionResult EditMain(string tbid)
        {
            ViewBag.TbSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");

            var model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model != null)
            {
                var mflow = flowService.GetByWhereFirst("where FlowId=@FlowId", new { FlowId = model.FlowId });
                model.flowType = mflow == null ? "" : mflow.flowType;
                model.EditUser = mflow == null ? "" : mflow.EditUser;
                model.EditUser_Exa = CommonSelectService.GetNameStrByIdStr(model.EditUser, "1");

                model.isLeftNum = string.IsNullOrEmpty(model.isLeftNum) ? "2" : model.isLeftNum;
                model.isSoftDel = string.IsNullOrEmpty(model.isSoftDel) ? "2" : model.isSoftDel;
                model.isInfo = string.IsNullOrEmpty(model.isInfo) ? "1" : model.isInfo;

                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        public ActionResult EditAux(string tbid)
        {
            ViewBag.TbSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");

            var model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model != null)
            {
                var mflow = flowService.GetByWhereFirst("where FlowId=@FlowId", new { FlowId = model.FlowId });
                model.flowType = "0";
                model.EditUser = "";
                model.EditUser_Exa = "";

                model.isLeftNum = string.IsNullOrEmpty(model.isLeftNum) ? "2" : model.isLeftNum;
                model.isSoftDel = string.IsNullOrEmpty(model.isSoftDel) ? "2" : model.isSoftDel;
                model.isInfo = string.IsNullOrEmpty(model.isInfo) ? "1" : model.isInfo;

                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        public ActionResult EditGrid(string tbid)
        {
            var model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model != null)
            {
                TbBasic2Service tbBasic2Service = new TbBasic2Service();
                var mTbBasic2 = tbBasic2Service.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
                model.InType = mTbBasic2 == null ? "0" : mTbBasic2.InType;
                model.isLeftNum = string.IsNullOrEmpty(model.isLeftNum) ? "2" : model.isLeftNum;
                model.isSoftDel = string.IsNullOrEmpty(model.isSoftDel) ? "2" : model.isSoftDel;

                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        public ActionResult TbExplain(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            return View();
        }

        public ActionResult TbLimits(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            if (model.TbType == "1")
            {
                List<ControlEntity> list = new List<ControlEntity>();
                FlowEntity mFlow = flowService.GetByWhereFirst("where FlowId='" + model.FlowId + "'");
                if (mFlow != null)
                {
                    mFlow.EditUser_Exa = CommonSelectService.GetNameStrByIdStr(mFlow.EditUser, "1");
                    mFlow.ManageUser_Exa = CommonSelectService.GetNameStrByIdStr(mFlow.ManageUser, "1");
                    mFlow.ManageUser2_Exa = CommonSelectService.GetNameStrByIdStr(mFlow.ManageUser2, "1");
                    mFlow.QueryUser_Exa = CommonSelectService.GetNameStrByIdStr(mFlow.QueryUser, "1");
                    mFlow.QueryUser2_Exa = CommonSelectService.GetNameStrByIdStr(mFlow.QueryUser2, "1");

                    DataTable dt = comService.GetDataTable("select id as id, name as name from sys_searchmode");
                    string searchMode_Exa = "";
                    foreach (DataRow dr in comService.GetDataTable("select * from tbsearchmode where TbId='" + model.FlowId.Replace("bi_", "tb_").Replace("fw_", "tb_") + "'").Rows)
                    {
                        string typeId = dr["TypeId"].ToString();
                        foreach (DataRow drmode in dt.Rows)
                        {
                            if (typeId == drmode["id"].ToString())
                            {
                                typeId = drmode["name"].ToString();
                                break;
                            }
                        }
                        searchMode_Exa += "{" + dr["UserId"].ToString() + "" + typeId + "} ";
                    }
                    mFlow.SearchMode_Exa = searchMode_Exa;

                    DataTable dtSelect = DataTableHelp.NewValueTextDt();
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow row = dtSelect.NewRow();
                        row["ClassID"] = "sys_searchmode";
                        row["Value"] = dr["id"].ToString();
                        row["Text"] = dr["name"].ToString();
                        dtSelect.Rows.Add(row);
                    }
                    ViewBag.SelectList = DataTableHelp.ToEnumerable<valueTextEntity>(dtSelect);

                    ViewBag.List = list;
                    ViewBag.SearchMode = string.IsNullOrEmpty(mFlow.SearchMode) ? "11" : mFlow.SearchMode;

                    return View(mFlow);
                }
                else
                {
                    return Json("流程对象为空！");
                }
            }
            else
            {
                return Json("请选择主表");
            }
        }

        public ActionResult TbAtt(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            if (model.TbType == "1")
            {
                //List<ControlEntity> list = new List<ControlEntity>();
                FlowEntity mFlow = flowService.GetByWhereFirst("where FlowId='" + model.FlowId + "'");
                if (mFlow != null)
                {
                    return View(mFlow);
                }
                else
                {
                    return Json("流程对象为空！");
                }
            }
            else
            {
                return Json("请选择主表");
            }
        }

        public ActionResult TbBut(string tbid)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            model.TbId = tbid;
            if (model.TbType == "1")
            {
                tbBasicService.GetTbButton(tbid, ref model);
                return View(model);
            }
            else
            {
                return Json("请选择主表");
            }
        }

        public ActionResult TbButDisable(string tbid, string btnid, string pageType)
        {
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            ViewBag.tbid = tbid;
            ViewBag.pageType = pageType;
            ViewBag.btnid = btnid;

            return View();
        }

        public ActionResult EditTbBut(string tbid, TbBasicEntity entity, IEnumerable<valueTextEntity> list)
        {
            try
            {
                tbBasicService.UpTbButton(tbid, entity, list);

                string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                Record.AddInfo(userid, tbid, "编辑录入表页面按钮");

                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult EditTbLimits(FlowEntity model, string fwid)
        {
            string err = "";
            if (fwid.StartsWith("fw_"))
            {
                model.EditUser = "";
            }
            string where = "where FlowId='" + fwid + "'";
            string updateFields = "EditUser,ManageUser,ManageUser2,SearchMode";

            try
            {
                err = flowService.UpdateByWhere(where, updateFields, model) > 0 ? "" : "编辑失败";

                if (string.IsNullOrEmpty(err))
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, fwid.Replace("bi_", "tb_").Replace("fw_", "tb_"), "编辑录入表权限");
                    return Json(SuccessTip("编辑成功，属性已更改无需『重新生成』"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult EditTbAtt(FlowEntity model, string fwid)
        {
            string err = "";
            string where = "where FlowId='" + fwid + "'";
            string updateFields = "FlowAttachment,FlowAttachment2,FlowAttachment3";

            try
            {
                err = flowService.UpdateByWhere(where, updateFields, model) > 0 ? "" : "编辑失败";

                if (string.IsNullOrEmpty(err))
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, fwid.Replace("bi_", "tb_").Replace("fw_", "tb_"), "编辑录入表公共附件设置");
                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult RecordList(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }

        public ActionResult GetRecordList(PageInfoEntity pageInfo, string tbid, string key)
        {
            RecordSetService recordSetService = new RecordSetService();
            RecordSetEntity model = new RecordSetEntity();

            pageInfo.field = "OperateTime";
            pageInfo.order = "desc";

            key = string.IsNullOrEmpty(key) ? "" : key;
            tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;

            string where = "where ObjectId = '" + tbid + "' ";

            if (key != "")
            {
                where += " and (Info like @Info) ";
                model.Info = string.Format("%{0}%", key);
            }

            long total = 0;
            IEnumerable<dynamic> list = recordSetService.GetPageByFilter(ref total, model, pageInfo, where);

            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpPost]
        public ActionResult EditMain(TbBasicEntity entity, string fwid)
        {
            string where = "where TbName=@TbName AND TbId<>@TbId";
            if (comService.GetTotal("tbbasic", where, new { TbName = entity.TbName, TbId = entity.TbId }) > 0)
            {
                return Json(ErrorTip("已存在相同名称"));
            }

            try
            {
                string err = tbBasicService.EditMainTable(entity, fwid);
                if (string.IsNullOrEmpty(err))
                {
                    Record.AddInfo(CurrentUser == null ? "!NullEx" : CurrentUser.Account, entity.TbId, "编辑录入表");
                    return Json(SuccessTip("操作成功，修改软删除属性后，需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult EditAux(TbBasicEntity entity)
        {
            string where = "where TbName=@TbName AND TbId<>@TbId";
            if (comService.GetTotal("tbbasic", where, new { TbName = entity.TbName, TbId = entity.TbId }) > 0)
            {
                return Json(ErrorTip("已存在相同名称"));
            }

            try
            {
                string err = tbBasicService.EditAuxTable(entity);
                if (string.IsNullOrEmpty(err))
                {
                    Record.AddInfo(CurrentUser == null ? "!NullEx" : CurrentUser.Account, entity.TbId, "编辑录入表");

                    return Json(SuccessTip("操作成功，修改软删除属性后，需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult EditGrid(TbBasicEntity entity)
        {
            string where = "where TbName=@TbName AND TbId<>@TbId";
            if (comService.GetTotal("tbbasic", where, new { TbName = entity.TbName, TbId = entity.TbId }) > 0)
            {
                return Json(ErrorTip("已存在相同名称"));
            }

            try
            {
                string err = tbBasicService.EditGridTable(entity);
                if (string.IsNullOrEmpty(err))
                {
                    Record.AddInfo(CurrentUser == null ? "!NullEx" : CurrentUser.Account, entity.TbId, "编辑录入表");

                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult AddGrid()
        {
            ViewBag.TbList = tbBasicService.GetByWhere("where TbType='1' and  isAux <> '1' ", null, "TbId,TbName");
            ViewBag.selectNumber = tbBasicService.GetGridColNumList();
            ViewBag.DtaeTypeList = indexDtaeTypeService.GetIndexDtaeType();
            return View();
        }

        [HttpPost]
        public ActionResult AddTable(TbBasicEntity entity)
        {
            string err = "";
            string str = "#login#home#com#";
            string tbid = entity.TbId.ToLower();
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;

            if (entity.TbId.ToLower().StartsWith("sys"))
            {
                var result = ErrorTip("编码不能sys开头");
                return Json(result);
            }

            if (str.ToLower().Contains("#" + entity.TbId.ToLower() + "#"))
            {
                var result = ErrorTip("编码不能为以下关键字：" + str.Replace("#", " "));
                return Json(result);
            }

            for (int i = 1; i <= Think9.Services.Table.BaseConfig.MaxGridColNum; i++)
            {
                if (entity.TbId.ToLower().EndsWith("v" + i.ToString()))
                {
                    err = "编码不能以：" + "v" + i.ToString() + "结尾";
                    var result = ErrorTip(err);
                    return Json(result);
                }
            }

            if (entity.isInfo == "1" && string.IsNullOrEmpty(entity.EditUser))
            {
                var result = ErrorTip("请选择可编辑用户");
                return Json(result);
            }

            try
            {
                string where = "where TbId=@TbId";
                if (comService.GetTotal("tbbasic", where, new { TbId = "tb_" + entity.TbId }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同编码"));
                }
                where = "where ReportId=@ReportId";
                if (comService.GetTotal("reportbasic", where, new { ReportId = "rp_" + entity.TbId }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同编码的统计表"));
                }

                where = "where TbName=@TbName";
                if (comService.GetTotal("tbbasic", where, new { TbName = entity.TbName }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同名称"));
                }

                err = tbBasicService.AddMainTb(entity, userid);
                if (err == "")
                {
                    if (!entity.TbId.StartsWith("tb_"))
                    {
                        entity.TbId = "tb_" + tbid;
                    }

                    Record.AddInfo(userid, entity.TbId, "新建录入表");
                    return Json(SuccessTip("添加成功,将进入下一步为录入表添加录入指标"));
                }
                else
                {
                    return Json(ErrorTip("添加失败：" + err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip("添加失败：" + ex.Message));
            }
        }

        [HttpPost]
        public ActionResult AddAux(TbBasicEntity entity)
        {
            string err = "";
            string str = "#login#home#com#";
            string tbid = entity.TbId.ToLower();
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;

            if (entity.TbId.ToLower().StartsWith("sys"))
            {
                var result = ErrorTip("编码不能sys开头");
                return Json(result);
            }

            if (str.ToLower().Contains("#" + entity.TbId.ToLower() + "#"))
            {
                var result = ErrorTip("编码不能为以下关键字：" + str.Replace("#", " "));
                return Json(result);
            }

            //for (int i = 1; i <= Think9.Services.Table.BaseConfig.MaxGridColNum; i++)
            //{
            //    if (entity.TbId.ToLower().EndsWith("v" + i.ToString()))
            //    {
            //        err = "编码不能以：" + "v" + i.ToString() + "结尾";
            //        var result = ErrorTip(err);
            //        return Json(result);
            //    }
            //}

            try
            {
                string where = "where TbId=@TbId";
                if (comService.GetTotal("tbbasic", where, new { TbId = "tb_" + entity.TbId }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同编码"));
                }
                where = "where ReportId=@ReportId";
                if (comService.GetTotal("reportbasic", where, new { ReportId = "rp_" + entity.TbId }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同编码的统计表"));
                }
                where = "where TbName=@TbName";
                if (comService.GetTotal("tbbasic", where, new { TbName = entity.TbName }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同名称"));
                }

                err = tbBasicService.AddAuxTb(entity, userid);

                if (err == "")
                {
                    if (!entity.TbId.StartsWith("tb_"))
                    {
                        entity.TbId = "tb_" + tbid;
                    }

                    Record.AddInfo(userid, entity.TbId, "新建辅助表");
                    return Json(SuccessTip("添加成功,将进入下一步为录入表添加录入指标"));
                }
                else
                {
                    return Json(ErrorTip("添加失败：" + err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip("添加失败：" + ex.Message));
            }
        }

        [HttpPost]
        public ActionResult AddGridTable(TbBasicEntity entity, IEnumerable<TbIndexEntity> list)
        {
            string err = "";
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
            try
            {
                string where = "where TbId=@TbId";
                if (comService.GetTotal("tbbasic", where, new { TbId = "tb_" + entity.TbId }) > 0)
                {
                    var result = ErrorTip("添加失败!已存在相同编码");
                    return Json(result);
                }
                where = "where ReportId=@ReportId";
                if (comService.GetTotal("reportbasic", where, new { ReportId = "rp_" + entity.TbId }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同编码的统计表"));
                }
                where = "where TbName=@TbName";
                if (comService.GetTotal("tbbasic", where, new { TbName = entity.TbName }) > 0)
                {
                    return Json(ErrorTip("添加失败!已存在相同名称"));
                }

                err = tbBasicService.AddGridTb(entity, list, userid);
                if (err == "")
                {
                    Record.AddInfo(userid, entity.TbId, "新建录入表");

                    return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip("操作失败" + err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip("操作失败" + ex.Message));
            }
        }

        [HttpPost]
        public ActionResult GetCreatDBStr(string tbid)
        {
            if (!tbid.StartsWith("tb_"))
            {
                tbid = "tb_" + tbid;
            }
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model != null)
            {
                string ParentId = model.ParentId;
                string type = ParentId == "" ? "1" : "2";
                string isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
                string sql = "";
                if (isAux == "1")
                {
                    sql = comService.GetCreatDBStr(tbid);
                }
                else
                {
                    sql = comService.GetCreatDBStr(tbid, type, model.FlowId);
                }

                var result = SuccessTip("操作成功", sql);
                return Json(result);
            }
            else
            {
                return Json(ErrorTip("操作失败，录入表不存在"));
            }
        }

        [HttpPost]
        public ActionResult GetCreatAuxDBStr(string tbid)
        {
            if (!tbid.StartsWith("tb_"))
            {
                tbid = "tb_" + tbid;
            }
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model != null)
            {
                string sql = comService.GetCreatDBStr(tbid);

                var result = SuccessTip("操作成功", sql);
                return Json(result);
            }
            else
            {
                return Json(ErrorTip("操作失败，录入表不存在"));
            }
        }

        [HttpPost]
        public ActionResult CreatTableDB(string tbid)
        {
            try
            {
                if (!comService.IsDataTableExists(tbid))
                {
                    TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
                    if (model != null)
                    {
                        string ParentId = model.ParentId;
                        string type = ParentId == "" ? "1" : "2";

                        string isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
                        string sql = "";
                        if (isAux == "1")
                        {
                            sql = comService.GetCreatDBStr(tbid);
                        }
                        else
                        {
                            sql = comService.GetCreatDBStr(tbid, type, model.FlowId);
                        }

                        comService.ExecuteSql(sql);
                    }

                    if (comService.IsDataTableExists(tbid))
                    {
                        string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                        Record.AddInfo(userid, tbid, "创建数据表");

                        return Json(SuccessTip("操作成功"));
                    }
                    else
                    {
                        return Json(ErrorTip("操作失败"));
                    }
                }
                else
                {
                    return Json(ErrorTip("数据表已存在"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip("操作失败" + ex.Message));
            }
        }

        [HttpPost]
        public ActionResult CreatDB(string sql, string tbid)
        {
            try
            {
                comService.ExecuteSql(sql);

                if (!tbid.StartsWith("tb_"))
                {
                    tbid = "tb_" + tbid;
                }

                if (comService.IsDataTableExists(tbid))
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, tbid, "创建数据表");

                    return Json(SuccessTip("操作成功"));
                }
                else
                {
                    return Json(ErrorTip("操作失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult GetModel(string tbid)
        {
            var model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model == null)
            {
                return Json(ErrorTip("对象为空，请刷新后再操作！"));
            }

            tbBasicService.GetModelExa(tbid, ref model);
            var result = SuccessTip("操作成功", Newtonsoft.Json.JsonConvert.SerializeObject(model));

            return Json(result);
        }

        /// <summary>
        /// 彻底删除录入表
        /// </summary>
        /// <param name="tbid">录入表id</param>
        /// <returns></returns>
        public ActionResult DeletTbAll(string tbid)
        {
            if (Think9.Services.Base.Configs.GetValue("IsDemo") == "true")
            {
                return Json(ErrorTip("演示模式下不能删除录入表！"));
            }
            try
            {
                //if (comService.IsDataTableExists(tbid))
                //{
                //    if (comService.GetTotal(tbid, "") > 0)
                //    {
                //        return Json(ErrorTip("数据表里存在数据，请清空数据后再删除！"));
                //    }
                //}

                string err = tbBasicService.DeletTbAll(tbid);
                if (err == "")
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, tbid, "删除录入表");

                    return Json(SuccessTip("删除成功"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public JsonResult GetGridNumber(int iCount)
        {
            List<TbIndexEntity> list = new List<TbIndexEntity>();

            for (int i = 1; i <= iCount; i++)
            {
                list.Add(new TbIndexEntity { IndexId = "v" + i.ToString(), IndexName = "列" + i.ToString(), IndexNo = i, ColumnWith = 150, DataType = "" });
            }

            var result = new { code = 0, msg = "", count = list.Count, data = list };
            return Json(result);
        }
    }
}
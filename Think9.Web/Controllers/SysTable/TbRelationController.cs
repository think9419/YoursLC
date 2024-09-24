using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Flow;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class TbRelationController : BaseController
    {
        private ComService comService = new ComService();
        private TbBasicService tbServer = new TbBasicService();
        private TbIndexService tbIndex = new TbIndexService();
        private TbRelationService tbRelationService = new TbRelationService();
        private RelationRDService relationRDService = new RelationRDService();
        private RelationRDFieldService relationRDFieldService = new RelationRDFieldService();
        private RelationWDFieldService relationWDFieldService = new RelationWDFieldService();
        private RelationListService relationListService = new RelationListService();
        private RelationWDService relationWDService = new RelationWDService();
        private TbFieldService tbFieldService = new TbFieldService();
        private RuleServiceBasic ruleService = new RuleServiceBasic();

        public ActionResult List(string tbid)
        {
            TbBasicEntity model = tbServer.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        public ActionResult ListRead(string tbid)
        {
            TbBasicEntity model = tbServer.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        public ActionResult ListGridData(string tbid)
        {
            TbBasicEntity model = tbServer.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        public ActionResult ListWrite(string tbid)
        {
            TbBasicEntity model = tbServer.GetByWhereFirst("where TbId='" + tbid + "'");
            if (model == null)
            {
                return Json("录入表对象为空！");
            }

            model.isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
            if (model.isAux == "1")
            {
                return Json("不能对辅助表进行此操作！");
            }

            ViewBag.tbid = tbid;
            return View();
        }

        [HttpGet]
        public JsonResult GetList(RelationListEntity model, PageInfoEntity pageInfo, string tbid)
        {
            long total = 0;
            pageInfo.field = "tbrelation.RelationId";

            var list = relationListService.GetPageList(ref total, model, pageInfo, tbid);
            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetListByType(RelationListEntity model, PageInfoEntity pageInfo, string tbid, string type)
        {
            long total = 0;
            pageInfo.field = "tbrelation.RelationId";

            var list = relationListService.GetPageList(ref total, model, pageInfo, tbid, type).ToList();
            foreach (RelationListEntity obj in list)
            {
                if (obj.DbID != 0)
                {
                    obj.RelationName += " <i class='fa fa-external-link'></i>";
                }
            }
            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        public ActionResult BeforeAddTbRelation(string tbid)
        {
            string err = tbRelationService.BeforeAdd(tbid);

            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        public ActionResult UpIsUse(string id, string isState)
        {
            isState = isState == "true" ? "1" : "2";
            string err = comService.ExecuteSql("update tbrelation set isUse='" + isState + "'   WHERE RelationId= " + id) > 0 ? "" : "操作失败";

            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult AddRead(string tbid)
        {
            ViewBag.Guid = Think9.Services.Basic.CreatCode.NewGuid();
            ViewBag.tbid = tbid;
            ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
            ViewBag.id = 0;
            ViewBag.ParentId = comService.GetSingleField("select ParentId  FROM tbbasic WHERE TbId= '" + tbid + "'");

            ViewBag.SelectSourceList = ruleService.GetSelectSourceList();

            //可选择的数据源绑定
            DataTable dt = tbServer.GetMainAndGridTb(GetTbID.GetMainTbId(tbid));
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
            ViewBag.ViewList = tbFieldService.GetViewsList();

            //目标字段的绑定
            dt = tbIndex.GetSelectFieldListByTbId(tbid);
            ViewBag.FillIndexId = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            return View();
        }

        [HttpPost]
        public ActionResult AddRead(string guid, int dbId, string tbid, string fid, RelationRDEntity model)
        {
            string err = "";
            int rid = 0;

            fid = fid == null ? "" : fid.Trim();
            if (fid.StartsWith("#table#"))
            {
                fid = fid.Substring(7, fid.Length - 7);
            }
            if (dbId != 0)
            {
                model.FromFlowId = fid;
                model.FromTbId = fid;
            }

            try
            {
                if (model.RelationSort == 2)
                {
                    if (comService.GetTotal("sys_temp", "where Guid='" + guid + "'") != 2)
                    {
                        err = "读取列表值时，需添加两条目标字段字段，并且两条字段应当相同，第一条对应value，第二条对应text";
                    }
                }
                else
                {
                    if (comService.GetTotal("sys_temp", "where Guid='" + guid + "'") == 0)
                    {
                        err = "至少添加一条读取字段";
                    }
                }

                if (string.IsNullOrEmpty(err))
                {
                    model.WhereStr1 = model.WhereStr1 == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr1);
                    err = tbRelationService.AddRelationRead(ref rid, dbId, "11", -1, guid, tbid, model);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                if (rid != 0)
                {
                    tbRelationService.Delete(rid.ToString());
                }

                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult AddWrite(string tbid)
        {
            FlowService FlowService = new FlowService();
            DataTable dt;

            ViewBag.Guid = Think9.Services.Basic.CreatCode.NewGuid();
            ViewBag.tbid = tbid;
            ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
            ViewBag.id = 0;

            string maintbid = GetTbID.GetMainTbId(tbid);

            //适用范围
            dt = FlowService.GetFlowListForTbRelation(maintbid);
            ViewBag.FlowList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            //回写数据表
            dt = tbServer.GetMainAndGridTb();
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            return View();
        }

        [HttpPost]
        public ActionResult AddWrite(string guid, string tbid, RelationWriteEntity model)
        {
            string err = "";
            int rid = 0;

            try
            {
                if (comService.GetTotal("sys_temp", "where Guid='" + guid + "'") == 0)
                {
                    err += "至少添加一条回写字段";
                }

                if (string.IsNullOrEmpty(model.FlowPrcs) || model.FlowPrcs.Replace("#", "").Trim() == "")
                {
                    err += "请选择适用范围";
                }

                if (string.IsNullOrEmpty(err))
                {
                    model.WhereStr = model.WhereStr == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr);
                    err = tbRelationService.AddRelationWrite(ref rid, "31", guid, tbid, model);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                if (rid != 0)
                {
                    tbRelationService.Delete(rid.ToString());
                }

                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult EditGridData(string tbid, int id)
        {
            try
            {
                ViewBag.tbid = tbid;
                ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
                ViewBag.id = id;

                string maintbid = GetTbID.GetMainTbId(tbid);
                //可选择的数据源绑定
                DataTable dt = tbServer.GetMainAndGridTb();
                ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

                //目标字段的绑定
                dt = tbIndex.GetSelectFieldListByTbId(tbid);
                ViewBag.FillIndexId = DataTableHelp.ToEnumerable<valueTextEntity>(dt);
                ViewBag.ViewList = tbFieldService.GetViewsList();

                RelationRDEntity model = new RelationRDEntity();
                if (id == 0)
                {
                    ViewBag.Guid = Think9.Services.Basic.CreatCode.NewGuid();
                    model.RelationName = "数据初始化";
                    model.FromTbId = "-1";
                    model.ICount = "3";
                    model.RelationType = "21";
                    model.TbID = tbid;
                    model.OrderType = "2";
                }
                else
                {
                    ViewBag.Guid = "";
                    RelationListEntity mList = relationListService.GetByWhereFirst("where RelationId=" + id + "");
                    if (mList == null)
                    {
                        model = null;
                    }
                    else
                    {
                        if (mList.ICount == -1)
                        {
                            model = relationRDService.GetByWhereFirst("where RelationId=" + id + "");

                            model.ICount = "-1";
                            model.RelationName = mList.RelationName;
                            model.RelationType = "21";
                            model.TbID = tbid;
                        }
                        else
                        {
                            model.ICount = mList.ICount.ToString();
                            model.FromTbId = "-1";
                            model.RelationName = mList.RelationName;
                            model.RelationType = "21";
                            model.TbID = tbid;
                            model.OrderType = "1";
                        }
                    }
                }

                if (model == null)
                {
                    return Json("数据不存在");
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult EditGridData(string guid, int id, string tbid, RelationRDEntity model)
        {
            string err = "";
            int rid = 0;

            if (id == 0)
            {
                if (model.FromTbId == "-1" && string.IsNullOrEmpty(model.ICount))
                {
                    err = "请输入空值行数";
                }

                if (string.IsNullOrEmpty(err))
                {
                    model.WhereStr1 = model.WhereStr1 == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr1);
                    err = tbRelationService.AddRelationRead(ref rid, 0, "21", int.Parse(model.ICount), guid, tbid, model);
                }
            }
            else
            {
                model.WhereStr1 = model.WhereStr1 == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr1);
                err = tbRelationService.EditRelationRead(id, model);
            }

            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("保存成功"));
            }
            else
            {
                if (rid != 0)
                {
                    tbRelationService.Delete(rid.ToString());
                }
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult EditRead(string tbid, int id)
        {
            try
            {
                ViewBag.tbid = tbid;
                ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
                string dbid = comService.GetSingleField("select DbID  FROM  relationlist  WHERE RelationId=" + id + "");
                dbid = string.IsNullOrEmpty(dbid) ? "0" : dbid;
                ViewBag.DbID = dbid;
                ViewBag.DbID_Exa = ExternalDbService.GetName(int.Parse(dbid));
                ViewBag.id = id;
                ViewBag.Guid = "";

                string mainTbId = GetTbID.GetMainTbId(tbid);
                //可选择的数据源绑定
                DataTable dt = tbServer.GetMainAndGridTb(mainTbId);
                ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

                //目标字段的绑定
                dt = tbIndex.GetSelectFieldListByTbId(tbid);
                ViewBag.FillIndexId = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

                RelationRDEntity model = relationRDService.GetByWhereFirst("where RelationId=" + id + "");
                if (model == null)
                {
                    return Json("数据不存在");
                }
                else
                {
                    ViewBag.FId = model.FromTbId;
                    RelationListEntity modelList = relationListService.GetByWhereFirst("where RelationId=" + id + "");

                    model.RelationName = modelList == null ? "" : modelList.RelationName;
                    model.RelationBy = modelList == null ? "" : modelList.RelationBy;
                    model.ICount = modelList == null ? "" : modelList.ICount.ToString();
                    model.RelationType = "11";
                    model.TbID = tbid;
                    model.FromTbName = GetTbID.GetTbName(model.FromTbId);
                    if (model.FromTbName == "")
                    {
                        model.FromTbName = model.FromTbId;
                    }
                    model.UseType = string.IsNullOrEmpty(model.UseType) ? "1" : model.UseType;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult EditRead(int id, string tbid, RelationRDEntity model)
        {
            string err = "";
            try
            {
                model.WhereStr1 = model.WhereStr1 == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr1);
                err = tbRelationService.EditRelationRead(id, model);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }

            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult EditWrite(string tbid, int id)
        {
            try
            {
                ViewBag.tbid = tbid;
                ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
                ViewBag.id = id;
                ViewBag.Guid = "";

                FlowService FlowService = new FlowService();
                string maintbid = GetTbID.GetMainTbId(tbid);

                //适用范围
                DataTable dt = FlowService.GetFlowListForTbRelation(maintbid);
                ViewBag.FlowList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

                //回写数据表
                dt = tbServer.GetMainAndGridTb();
                ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

                RelationWriteEntity model = relationWDService.GetByWhereFirst("where RelationId=" + id + "");

                if (model == null)
                {
                    return Json("数据不存在");
                }
                else
                {
                    model.FlowPrcs_Exa = model.FlowPrcs;
                    ViewBag.FId = model.WriteTbId;
                    RelationListEntity model2 = relationListService.GetByWhereFirst("where RelationId=" + id + "");

                    model.RelationName = model2 == null ? "" : model2.RelationName;
                    model.RelationBy = model2 == null ? "" : model2.RelationBy;
                    model.RelationType = "31";
                    model.TbID = tbid;
                    model.FromTbName = GetTbID.GetTbName(model.WriteTbId);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult EditWrite(int id, string tbid, RelationWriteEntity model)
        {
            string err = "";
            try
            {
                model.WhereStr = model.WhereStr == null ? "" : BaseUtil.ReplaceHtml(model.WhereStr);
                err = tbRelationService.EditRelationWrite(id, model);
                if (string.IsNullOrEmpty(err))
                {
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

        [HttpGet]
        public JsonResult GetWriteRecordList(string id, PageInfoEntity pageInfo)
        {
            long total = 0;
            string where = " where RelationId = " + id + "";
            pageInfo.field = "WriteTime";
            pageInfo.order = "desc";

            WriteBackService writeBackService = new WriteBackService();
            var list = writeBackService.GetPageByFilter(ref total, null, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpGet]
        public ActionResult PUFormula(string tbid, string fid, string some)
        {
            ViewBag.tbid = tbid;
            ViewBag.formid = fid;
            ViewBag.some = some;

            //可选择的数据源绑定
            DataTable dt = tbServer.GetMainAndGridTb();
            ViewBag.FromTbList = DataTableHelp.ToEnumerable<valueTextEntity>(dt);

            return View();
        }

        public ActionResult SelectValue(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }

        public ActionResult SelectValue2(string tbid)
        {
            ViewBag.tbid = tbid;
            return View();
        }

        [HttpGet]
        public ActionResult GetIndexList(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = DataTableHelp.ToEnumerable<valueTextEntity>(tbIndex.GetIndexList(tbid)) };

            return Json(result);
        }

        [HttpGet]
        public ActionResult GetIndexList2(string tbid)
        {
            var result = new { code = 0, msg = "", count = 999999, data = DataTableHelp.ToEnumerable<valueTextEntity>(tbIndex.GetIndexList2(tbid)) };

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetSelectValueFieldByTb(string id, string dbId)
        {
            dbId = string.IsNullOrEmpty(dbId) ? "0" : dbId;
            try
            {
                if (dbId == "0")
                {
                    return Json(tbRelationService.GetSelectValueFieldList(id));
                }
                return Json(ruleService.GetSelectValueFieldListByTbid(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetSelectOrderFieldByTbId(string id, string dbId)
        {
            dbId = string.IsNullOrEmpty(dbId) ? "0" : dbId;
            try
            {
                if (dbId == "0")
                {
                    return Json(tbRelationService.GetOrderFieldList(id == null ? "0" : id));
                }
                return Json(ruleService.GetSelectOrderFieldListByTbid2(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public JsonResult GetConditionFieldList(string id, string dbId)
        {
            dbId = string.IsNullOrEmpty(dbId) ? "0" : dbId;
            try
            {
                if (dbId == "0")
                {
                    return Json(tbRelationService.GetConditionFieldList(id == null ? "" : id));
                }
                return Json(ruleService.GetConditionFieldListByTbid2(id == null ? "0" : id, dbId));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult AddFiledRead(string guid, string id, string value, string value2)
        {
            string err = "";
            value = value == null ? "" : value;
            if (value.Trim().StartsWith("#table#"))
            {
                value = value.Substring(7, value.Length - 7);
            }
            try
            {
                if (string.IsNullOrEmpty(guid))
                {
                    err = tbRelationService.AddListFiledRead(id, value, value2);
                }
                else
                {
                    err = tbRelationService.AddListFiledTempRead(guid, value, value2);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (err == "")
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpPost]
        public ActionResult AddFiledWrite(string guid, string id, string value, string value2)
        {
            string err = "";
            try
            {
                if (string.IsNullOrEmpty(guid))
                {
                    err = tbRelationService.AddListFiledWrite(id, value, value2);
                }
                else
                {
                    err = tbRelationService.AddListFiledTempWrite(guid, value, value2);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (err == "")
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpPost]
        public ActionResult GetFiledRead(string guid, string id, string tbid, string fromtbid)
        {
            fromtbid = fromtbid == null ? "" : fromtbid;
            if (string.IsNullOrEmpty(guid))
            {
                var result = new { code = 0, msg = "", count = 999999, data = tbRelationService.GetListFiledRead(id, tbid, fromtbid) };

                return Json(result);
            }
            else
            {
                var result = new { code = 0, msg = "", count = 999999, data = tbRelationService.GetListFiledTemp(guid, tbid, fromtbid) };

                return Json(result);
            }
        }

        [HttpPost]
        public ActionResult GetFiledWrite(string guid, string id, string writetbid)
        {
            writetbid = writetbid == null ? "" : writetbid;
            if (string.IsNullOrEmpty(guid))
            {
                var result = new { code = 0, msg = "", count = 999999, data = tbRelationService.GetListFiledWrite(id, writetbid) };

                return Json(result);
            }
            else
            {
                var result = new { code = 0, msg = "", count = 999999, data = tbRelationService.GetListFiledTempWrite(guid, writetbid) };

                return Json(result);
            }
        }

        [HttpGet]
        public ActionResult DeleteFiled(string id, string guid)
        {
            string err = "";
            if (string.IsNullOrEmpty(guid))
            {
                err = relationRDFieldService.DeleteByWhere("where id=" + id + "") ? "" : "操作失败";
            }
            else
            {
                err = tbRelationService.DelListFiledTemp(id);
            }

            if (err == "")
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult DeleteReadFiled(string id, string guid)
        {
            string err = "";
            if (string.IsNullOrEmpty(guid))
            {
                err = relationRDFieldService.DeleteByWhere("where id=" + id + "") ? "" : "操作失败";
            }
            else
            {
                err = tbRelationService.DelListFiledTemp(id);
            }

            if (err == "")
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult DeleteWriteFiled(string id, string guid)
        {
            string err = "";
            if (string.IsNullOrEmpty(guid))
            {
                err = relationWDFieldService.DeleteByWhere("where id=" + id + "") ? "" : "操作失败";
            }
            else
            {
                err = tbRelationService.DelListFiledTemp(id);
            }

            if (err == "")
            {
                return Json(SuccessTip(""));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            string err = tbRelationService.Delete(id);

            if (err == "")
            {
                return Json(SuccessTip("删除成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public ActionResult GetSelectIndexList(string tbid)
        {
            tbid = tbid == null ? "" : tbid;
            var result = new { code = 0, msg = "", count = 999999, data = tbIndex.GetIndexByTbID2(GetTbID.GetTbIdByIdStr(tbid)) };

            return Json(result);
        }
    }
}
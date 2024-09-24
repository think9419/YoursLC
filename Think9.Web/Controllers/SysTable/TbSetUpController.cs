using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.CodeBuild;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class TbSetUpController : BaseController
    {
        private TbIndexService tbIndexService = new TbIndexService();
        private TbFieldService tbFieldService = new TbFieldService();
        private ComService comService = new ComService();
        private SortService sortService = new SortService();
        private TbBasicService tbBasicService = new TbBasicService();
        private IndexDateType indexDtaeTypeService = new IndexDateType();
        private ChangeModel changeModel = new ChangeModel();
        private TbEventParaService tbEventParaService = new TbEventParaService();
        private TbButCustomizeService tbButCustomize = new TbButCustomizeService();

        private TbFormService tbFormService = new TbFormService();

        //指标类型 DelIndexLink
        private SelectList IndexDtaeTypeList
        { get { return new SelectList(indexDtaeTypeService.GetIndexDtaeType(), "TypeId", "TypeName"); } }

        public ActionResult TbForm(string tbid, string nColumns)
        {
            nColumns = string.IsNullOrEmpty(nColumns) ? "4" : nColumns;
            ViewBag.NColumns = nColumns;
            ViewBag.tbid = tbid;

            tbFormService.TbFormInit(tbid);
            return View();
        }

        [HttpPost]
        public JsonResult GetJsonStr(string tbid, string nColumns)
        {
            nColumns = string.IsNullOrEmpty(nColumns) ? "4" : nColumns;

            DataTable dtIndex = comService.GetDataTable("select * from tbindex order by TbId,IndexOrderNo");
            DataTable allTB = comService.GetDataTable("select * from tbbasic ");
            TableStyle tbStyle = new TableStyle(allTB, dtIndex);

            DataTable dtTbForm = comService.GetDataTable("select * from tbform where TbId='" + tbid + "' ");
            DataTable dtLabel = tbFormService.GetLabelDataTable(tbid);
            DataTable dtLine = tbFormService.GetLineDataTable(tbid, dtLabel);
            DataTable dtColumn = tbFormService.GetColumnDataTable(tbid, dtLine);

            if (dtTbForm.Select("tbid='" + tbid + "'").Length == 0)
            {
                return Json("ERR:未定义录入指标，不能自动生成");
            }

            try
            {
                string str = tbStyle.GetJsonBySet(tbid, dtLabel, dtLine, dtColumn, dtTbForm, nColumns);
                return Json(str);
            }
            catch (Exception ex)
            {
                return Json("ERR:" + ex.ToString());
            }
        }

        [HttpPost]
        public JsonResult GetTbFormList(PageInfoEntity pageInfo, string tbid, string ini)
        {
            pageInfo.field = "NLabel,NLine,NColumn";
            ini = string.IsNullOrEmpty(ini) ? "n" : ini;
            try
            {
                if (ini == "y")
                {
                    tbFormService.TbFormInit(tbid);
                }
                long total = 0;
                IEnumerable<dynamic> list = tbFormService.GetPageByFilter(ref total, null, pageInfo, " where tbid='" + tbid + "'").ToList();
                //var list = tbFormService.GetByWhere(" where tbid='" + tbid + "'", null, null, "order by NLabel,NLine,NColumn").ToList();
                foreach (TbFormModel obj in list)
                {
                    if (obj.NLabel == null)
                    {
                        obj.NLabel = 99999;
                    }
                    if (obj.NLine == null)
                    {
                        obj.NLine = 99999;
                    }
                    if (obj.NColumn == null)
                    {
                        obj.NColumn = 99999;
                    }
                }
                var result = new { code = 0, msg = "", count = total, data = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult EditTbFormList(string tbid, List<TbFormModel> list)
        {
            string err = "";
            string updateFields = "NLabel,NLine,NColumn";
            try
            {
                foreach (TbFormModel obj in list)
                {
                    //if (!Think9.Services.Base.ValidatorHelper.IsInteger(obj.NLine))
                    //{
                    //	err += _indexid + "列宽不是整数<br> ";
                    //}

                    tbFormService.UpdateById(obj, updateFields);
                }
                return Json(SuccessTip("操作成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult DelLink(string tbid, string indexid)
        {
            string err = tbIndexService.DelIndexLink(tbid, indexid);
            if (err == "")
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        public ActionResult GetIndexLinkStr(string tbid, string indexid)
        {
            return Json(tbIndexService.GetTbIndexLinkStr(tbid, indexid));
        }

        public ActionResult AddIndexLink(string tbid)
        {
            TbIndexService TbIndexService = new TbIndexService();
            ViewBag.Guid = Think9.Services.Basic.CreatCode.NewGuid();
            ViewBag.tbid = tbid;
            ViewBag.IndexList = tbIndexService.GetIndexListByTbId(tbid);
            ViewBag.SelectList = tbButCustomize.GetSelectList(tbid, "list");
            return View();
        }

        [HttpPost]
        public ActionResult AddIndexLink(string guid, string tbid, string indexid, string linkFlag, string linkFlag2)
        {
            string err = tbIndexService.AddIndexLink(guid, tbid, indexid, linkFlag, linkFlag2);
            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("", tbIndexService.GetTbIndexLinkStr(tbid, indexid)));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        public ActionResult EditIndexLink(string tbid, string indexid)
        {
            string where = "where TbId=@TbId and indexid=@indexid ";
            object param = new { TbId = tbid, indexid = indexid };

            TbIndexEntity model = tbIndexService.GetByWhereFirst(where, param);
            if (model != null)
            {
                ViewBag.tbid = tbid;
                ViewBag.indexid = indexid;
                ViewBag.IndexList = tbIndexService.GetIndexListByTbId(tbid);
                ViewBag.SelectList = tbButCustomize.GetSelectList(tbid, "list");
                return View(model);
            }
            else
            {
                return Json("录入表指标不存在！");
            }
        }

        [HttpPost]
        public ActionResult EditIndexLink(string tbid, string indexid, string linkFlag, string linkFlag2)
        {
            string err = tbIndexService.EditIndexLink(tbid, indexid, linkFlag, linkFlag2);
            if (string.IsNullOrEmpty(err))
            {
                return Json(SuccessTip("", tbIndexService.GetTbIndexLinkStr(tbid, indexid)));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public JsonResult GetParaList(string tbid, string indexid, string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                {
                    var result = new { code = 0, msg = "", count = 999999, data = tbEventParaService.GetByWhere(" where tbid='" + tbid + "' and indexid='" + indexid + "'", null, null, "order by id") };
                    return Json(result);
                }
                else
                {
                    var result = new { code = 0, msg = "", count = 999999, data = tbEventParaService.GetByWhere(" where guid='" + guid + "'", null, null, "order by id") };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        public ActionResult AddLinkPara(string tbid, string indexid, string type, string name, string value, string guid)
        {
            string err = "";
            guid = string.IsNullOrEmpty(guid) ? "" : guid;
            try
            {
                err = tbIndexService.AddLinkPara(tbid, indexid, type, name, value, guid);
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }

            if (err == "")
            {
                return Json(SuccessTip("操作成功"));
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        public ActionResult TbIndexList(string tbid, bool isMobile)
        {
            ViewBag.tbid = tbid;
            ViewBag.linkStr = "/SysTable/TbSetUp/TbIndexList2?tbid=" + tbid + "&isMain=y";
            if (isMobile)
            {
                return View("TbIndexList_Mobile");
            }
            else
            {
                return View();
            }
        }

        public ActionResult TbIndexList2(string tbid, string isMain)
        {
            ViewBag.tbid = tbid;
            ViewBag.IsMain = string.IsNullOrEmpty(isMain) ? "n" : isMain;
            ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' "); ;
            return View();
        }

        public ActionResult TbJson(string tbid, string isAux)
        {
            string mobile = string.IsNullOrEmpty(isAux) ? "1" : isAux;
            if (isAux == "1")
            {
                return Json("辅助表无需设置");
            }

            ViewBag.tbid = tbid;
            ViewBag.tbname = comService.GetSingleField("select TbName  FROM  tbbasic  WHERE TbId='" + tbid + "' ");
            return View();
        }

        [HttpPost]
        public ActionResult DelTbJson(string tbid)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\TbJson\\") + tbid + ".json";
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Json(SuccessTip("操作成功"));
        }

        //
        public ActionResult ColumnList(string tbid, bool isMobile)
        {
            ViewBag.tbid = tbid;
            ViewBag.linkStr = "/SysTable/TbSetUp/TbIndexList2?tbid=" + tbid;
            if (isMobile)
            {
                return View("ColumnList_Mobile");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChangeModel(string idsStr, string isRelease)
        {
            DataTable dtRecord = DataTableHelp.NewRecordCodeDt();
            isRelease = string.IsNullOrEmpty(isRelease) ? "" : isRelease;
            try
            {
                changeModel.SetTableModel(dtRecord, isRelease, idsStr);

                RecordCodeService.Add(dtRecord);

                return Json(SuccessTip("保存成功"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        //
        public ActionResult SelectRuleList(string tbid, string indexid, string ismuch)
        {
            ViewBag.ismuch = ismuch;

            String show = "";
            String dataType = "";
            IndexBasicService IndexBasicService = new IndexBasicService();

            if (string.IsNullOrEmpty(tbid) && string.IsNullOrEmpty(indexid))
            {
                var modelTb = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });

                string tbName = modelTb == null ? "" : modelTb.TbName;
                if (tbName.Length > 20)
                {
                    tbName = tbName.Substring(0, 20) + ".. ";
                }

                var modelIndex = IndexBasicService.GetByWhereFirst("where indexid=@indexid", new { indexid = indexid });
                string IndexName = modelIndex == null ? "" : modelIndex.IndexName;
                dataType = modelIndex == null ? "" : modelIndex.IndexDataType;

                show = tbName + "" + IndexName + " - 选择数据规范";
            }

            ViewBag.tbid = tbid;
            ViewBag.indexid = indexid;
            ViewBag.show = show;
            ViewBag.datatype = dataType;

            return View();
        }

        public ActionResult EditMainIndex(string tbid, string indexid, string frm)
        {
            ViewBag.tbid = tbid;
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "list" : frm;
            ViewBag.ValidateList = new SelectList(ValidateList.GetList(), "Value", "Text");

            string where = "where TbId=@TbId and indexid=@indexid ";
            object param = new { TbId = tbid, indexid = indexid };

            TbIndexEntity model = tbIndexService.GetByWhereFirst(where, param);
            if (model != null)
            {
                ViewBag.ControlType = new SelectList(ControlType.GetMainTbControlType(model.DataType), "Value", "Text");
                foreach (IndexDtaeTypeEntity obj in indexDtaeTypeService.GetIndexDtaeType())
                {
                    if (model.DataType == obj.TypeId)
                    {
                        model.DataType = obj.TypeName;
                        ViewBag.DataType = obj.TypeId.Substring(0, 1);//指标类型首字符
                        ViewBag.DataType2 = obj.TypeId;//指标类型
                        break;
                    }
                }

                model.isShow = model.isShow == null ? "1" : model.isShow;
                RuleListService rulelist = new RuleListService();
                model.RuleName = rulelist.GetRuleNameByID(model.RuleId, model.isSelMuch);

                model.LinkStr = HttpUtility.HtmlEncode(tbIndexService.GetTbIndexLinkStr(tbid, indexid));

                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        public ActionResult EditColumn(string tbid, string indexid)
        {
            ViewBag.tbid = tbid;
            ViewBag.ValidateList = new SelectList(ValidateList.GetList(), "Value", "Text");

            string where = "where TbId=@TbId and indexid=@indexid ";
            object param = new { TbId = tbid, indexid = indexid };

            TbIndexEntity model = tbIndexService.GetByWhereFirst(where, param);
            if (model != null)
            {
                ViewBag.ControlType = new SelectList(ControlType.GetGridTbControlType(model.DataType), "Value", "Text");
                foreach (IndexDtaeTypeEntity obj in indexDtaeTypeService.GetIndexDtaeType())
                {
                    if (model.DataType == obj.TypeId)
                    {
                        model.DataType = obj.TypeName;
                        ViewBag.DataType = obj.TypeId.Substring(0, 1);//指标类型首字符
                        ViewBag.DataType2 = obj.TypeId;//指标类型
                        break;
                    }
                }
                model.isShow = model.isShow == null ? "1" : model.isShow;
                RuleListService rulelist = new RuleListService();
                model.RuleName = rulelist.GetRuleNameByID(model.RuleId, model.isSelMuch);
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        public ActionResult RemoveMainIndex(string tbid)
        {
            ViewBag.TbSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_table"), "SortID", "SortName");
            ViewBag.DtaeTypeList = indexDtaeTypeService.GetIndexDtaeTypeList();
            ViewBag.IndexSortList = new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_index"), "SortID", "SortName");
            ViewBag.tbid = HttpContext.Request.Query["tbid"].ToString();
            return View();
        }

        public ActionResult DBTable(string tbid)
        {
            string sql = "";
            TbBasicEntity model = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = tbid });
            if (model != null)
            {
                string ParentId = model.ParentId;
                string type = ParentId == "" ? "1" : "2";
                string isAux = string.IsNullOrEmpty(model.isAux) ? "2" : model.isAux;
                if (isAux == "1")
                {
                    sql = comService.GetCreatDBStr(tbid);
                }
                else
                {
                    sql = comService.GetCreatDBStr(tbid, type, model.FlowId);
                }
            }

            ViewBag.tbid = tbid;
            ViewBag.sql = sql;
            return View();
        }

        public ActionResult DBTable2(string tbid)
        {
            string sql = "";
            ViewBag.tbid = tbid;
            ViewBag.sql = sql;
            return View();
        }

        public ActionResult GetTableFieldsList(string tbid)
        {
            var list = tbFieldService.GetDbField(tbid);
            var result = new { code = 0, msg = "", count = list.Count, data = list };
            return Json(result);
        }

        public ActionResult GetTableFieldsList2(string tbid)
        {
            var list = tbFieldService.GetDbField2(tbid);
            var result = new { code = 0, msg = "", count = list.Count, data = list };
            return Json(result);
        }

        public ActionResult AddColumn(string tbid)
        {
            ViewBag.tbid = tbid;
            ViewBag.DtaeTypeList = IndexDtaeTypeList;
            return View();
        }

        public ActionResult EditColumnType(string tbid, string indexid)
        {
            ViewBag.tbid = tbid;
            ViewBag.indexid = indexid;
            ViewBag.DtaeTypeList = IndexDtaeTypeList;
            return View();
        }

        [HttpPost]
        public ActionResult EditColumnType(string tbid, string indexid, string type)
        {
            string _dataType = comService.GetSingleField("select DataType  FROM tbindex  WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'");
            try
            {
                if (_dataType == type)
                {
                    return Json(ErrorTip("所选类型和原有类型相同"));
                }
                else
                {
                    string sql = comService.GetAlterTbFieldsSql(tbid, indexid, type);
                    int icount = comService.ExecuteSql(sql);

                    sql = "update tbindex set DataType='" + type + "' WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'";
                    comService.ExecuteSql(sql);

                    if (type.Substring(0, 1) == "5")
                    {
                        sql = "update tbindex set ControlType='5' WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'";
                    }
                    else
                    {
                        sql = "update tbindex set ControlType='1' WHERE TbId= '" + tbid + "' and IndexId= '" + indexid + "'";
                    }
                    comService.ExecuteSql(sql);
                }

                return Json(SuccessTip("修改成功！修改后需『重新生成』才能生效"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult AddColumn(string tbid, string name, string type, string isShow)
        {
            try
            {
                string err = tbIndexService.AddGridColumn(tbid, name, type, isShow);

                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("新建成功！修改后需『重新生成』才能生效"));
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
        public ActionResult DelColumn(string tbid)
        {
            try
            {
                string err = tbIndexService.DelGridColumn(tbid);

                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("删除成功！修改后需『重新生成』才能生效"));
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

        //打开校验页面
        [HttpGet]
        public ActionResult PUFormula(string tbid, string fid, string some)
        {
            ViewBag.tbid = tbid;
            ViewBag.formid = fid;
            ViewBag.some = some;
            return View();
        }

        [HttpPost]
        public ActionResult GetTbIndexList(TbIndexEntity model, PageInfoEntity pageInfo, string tbid)
        {
            string _tbid = tbid == null ? "" : tbid;
            if (!_tbid.StartsWith("tb_"))
            {
                _tbid = "tb_" + _tbid;
            }

            long total = 0;
            IEnumerable<dynamic> list = tbIndexService.GetTbIndexList(model, pageInfo, _tbid, ref total);

            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetMainTbIndexList(TbIndexEntity model, PageInfoEntity pageInfo, string tbid, string key)
        {
            string _tbid = tbid == null ? "" : tbid;
            if (!_tbid.StartsWith("tb_"))
            {
                _tbid = "tb_" + _tbid;
            }

            long total = 0;
            IEnumerable<dynamic> list = tbIndexService.GetTbIndexList02(model, pageInfo, _tbid, key, ref total);

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetGridTbIndexList(TbIndexEntity model, PageInfoEntity pageInfo, string tbid)
        {
            string _tbid = tbid == null ? "" : tbid;
            if (!_tbid.StartsWith("tb_"))
            {
                _tbid = "tb_" + _tbid;
            }

            long total = 0;
            IEnumerable<dynamic> list = tbIndexService.GetTbIndexList03(model, pageInfo, _tbid, ref total);

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpGet]
        public ActionResult GetIndexListByTbID(string tbid, string fid)
        {
            tbid = tbid == null ? "" : tbid;
            List<valueTextEntity> list = tbIndexService.GetIndexByTbID(tbid, fid);
            var result = new { code = 0, msg = "", count = list.Count, data = list };

            return Json(result);
        }

        [HttpPost]
        public ActionResult AddTbIndexByStr(string tbid, string si)
        {
            string err = "";

            if (string.IsNullOrWhiteSpace(tbid))
            {
                return Json(ErrorTip("操作失败tbid为空"));
            }
            if (!tbid.StartsWith("tb_"))
            {
                tbid = "tb_" + tbid;
            }

            try
            {
                string sindex = si == null ? "" : si;
                int icount = 0;
                string[] arr = BaseUtil.GetStrArray(sindex, ",");//
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    if (arr[i] != null)
                    {
                        string index = arr[i].ToString().Trim();
                        err = tbIndexService.AddTbIndex(ref icount, tbid, index);
                    }
                }

                string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                Record.AddInfo(userid, tbid, "添加指标" + sindex);

                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("共添加" + icount.ToString() + "指标，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(SuccessTip("共添加" + icount.ToString() + "指标，修改后需『重新生成』才能生效 " + err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveTbIndexByStr(string tbid, string si)
        {
            int icount = 0;
            if (string.IsNullOrWhiteSpace(tbid))
            {
                return Json(ErrorTip("操作失败tbid为空"));
            }

            try
            {
                string strIndex = tbIndexService.RemoveTbIndexByStr(ref icount, tbid, si);

                Record.AddInfo(CurrentUser == null ? "!NullEx" : CurrentUser.Account, tbid, "移除指标" + strIndex);

                return Json(SuccessTip("共移除" + icount.ToString() + "指标，修改后需『重新生成』才能生效"));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult UpTbIndexWidthAndNo(IEnumerable<TbIndexEntity> list)
        {
            var _indexid = "";
            var _tbid = "";
            var _no = "";
            var _width = "";
            var err = "";
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;

            foreach (TbIndexEntity obj in list)
            {
                _indexid = ((TbIndexEntity)obj).IndexId;
                _tbid = ((TbIndexEntity)obj).TbId;
                _no = ((TbIndexEntity)obj).IndexOrderNo.ToString();
                _width = ((TbIndexEntity)obj).ColumnWith.ToString();

                if (string.IsNullOrEmpty(_width))
                {
                    err += _indexid + "列宽不能为空<br> ";
                }
                else
                {
                    if (!Think9.Services.Base.ValidatorHelper.IsInteger(_width))
                    {
                        err += _indexid + "列宽不是整数<br> ";
                    }
                    else
                    {
                        if (int.Parse(_width) < 20)
                        {
                            err += _indexid + "列宽要大于20<br> ";
                        }
                    }
                }

                if (string.IsNullOrEmpty(_no))
                {
                    err += _indexid + "排序不能为空<br> ";
                }
                else
                {
                    if (!Think9.Services.Base.ValidatorHelper.IsInteger(_no))
                    {
                        err += _indexid + "排序不是整数<br> ";
                    }
                    else
                    {
                        if (int.Parse(_no) < 0)
                        {
                            err += _indexid + "排序要大于0<br> ";
                        }
                    }
                }
            }

            try
            {
                if (string.IsNullOrEmpty(err))
                {
                    foreach (TbIndexEntity obj in list)
                    {
                        _indexid = ((TbIndexEntity)obj).IndexId;
                        _tbid = ((TbIndexEntity)obj).TbId;
                        _no = ((TbIndexEntity)obj).IndexOrderNo.ToString();
                        _width = ((TbIndexEntity)obj).ColumnWith.ToString();

                        tbIndexService.UpdateByWhere("where IndexId='" + _indexid + "' and TbId='" + _tbid + "' ", "IndexOrderNo,ColumnWith", (TbIndexEntity)obj);
                        Record.AddInfo(userid, _tbid, "编辑指标属性" + _indexid + "[IndexOrderNo=" + _no + "][width=" + _width + "]");
                    }

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效"));
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
        public ActionResult UpTbIndexWidth(IEnumerable<TbIndexEntity> list)
        {
            var _indexid = "";
            var _tbid = "";
            var _no = "";
            var _width = "";
            var err = "";
            string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;

            DataTable dt = DataTableHelp.IEnumerableToDataTable<TbIndexEntity>(list);//转换DataTable

            foreach (TbIndexEntity obj in list)
            {
                _indexid = ((TbIndexEntity)obj).IndexId;
                _tbid = ((TbIndexEntity)obj).TbId;
                _no = ((TbIndexEntity)obj).IndexOrderNo.ToString();
                _width = ((TbIndexEntity)obj).ColumnWith.ToString();

                if (_indexid.StartsWith("v"))
                {
                    if (string.IsNullOrEmpty(_width))
                    {
                        err += _indexid + "列宽不能为空 ";
                    }
                    else
                    {
                        if (!Think9.Services.Base.ValidatorHelper.IsInteger(_width))
                        {
                            err += _indexid + "列宽不是整数 ";
                        }
                        else
                        {
                            if (int.Parse(_width) < 20)
                            {
                                err += _indexid + "列宽要大于20 ";
                            }
                        }
                    }
                }
            }

            try
            {
                if (string.IsNullOrEmpty(err))
                {
                    foreach (TbIndexEntity obj in list)
                    {
                        _indexid = ((TbIndexEntity)obj).IndexId;
                        _tbid = ((TbIndexEntity)obj).TbId;
                        _no = ((TbIndexEntity)obj).IndexOrderNo.ToString();
                        _width = ((TbIndexEntity)obj).ColumnWith.ToString();

                        if (_indexid.StartsWith("v"))
                        {
                            tbIndexService.UpdateByWhere("where IndexId='" + _indexid + "' and TbId='" + _tbid + "' ", "ColumnWith", (TbIndexEntity)obj);
                            Record.AddInfo(userid, _tbid, "编辑指标属性" + _indexid + "[width=" + _width + "]");
                        }
                    }

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效"));
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
        public ActionResult UpTbIndexSome(string tbid, string indexid, string name, bool isState)
        {
            string err = "";
            string where = "where IndexId='" + indexid + "' and TbId='" + tbid + "' ";
            TbIndexEntity model = new TbIndexEntity();

            if (name == "isEmpty")
            {
                if (isState)
                {
                    model.isEmpty = "2";
                }
                else
                {
                    model = tbIndexService.GetByWhereFirst(where);
                    if (model.isUnique == "1")
                    {
                        err += " 已设置为唯一属性，不能修改必填属性";
                    }
                    if (model.isPK == "1")
                    {
                        err += " 已设置为主键属性，不能修改必填属性";
                    }

                    model.isEmpty = "1";
                }
            }

            if (name == "isOrder")
            {
                model.isOrder = "2";
                if (isState)
                {
                    model.isOrder = "1";
                }
            }

            if (name == "isLock")
            {
                model.isLock = "2";
                if (isState)
                {
                    model.isLock = "1";
                }
            }

            if (name == "isColumnShow2")
            {
                model.isColumnShow2 = "2";
                if (isState)
                {
                    model.isColumnShow2 = "1";
                }
            }

            if (name == "isColumnShow")
            {
                model.isColumnShow = "2";
                if (isState)
                {
                    model.isColumnShow = "1";
                }
            }

            if (name == "isSearch2")
            {
                model.isSearch2 = "2";
                if (isState)
                {
                    model.isSearch2 = "1";
                }
            }

            if (name == "isSearch")
            {
                model.isSearch = "2";
                if (isState)
                {
                    model.isSearch = "1";
                }
            }

            if (name == "isShow")
            {
                model.isShow = "1";
                if (isState)
                {
                    model.isShow = "2";
                }
            }

            if (name == "isSelMuch")
            {
                model.isSelMuch = "2";
                if (isState)
                {
                    model.isSelMuch = "1";
                }
            }

            try
            {
                if (string.IsNullOrEmpty(err))
                {
                    err = tbIndexService.UpdateByWhere(where, name, model) > 0 ? "" : "操作失败";
                }

                if (string.IsNullOrEmpty(err))
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, tbid, "编辑指标属性：" + indexid + "[" + name + "]");

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效"));
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
        public ActionResult UpTbIndexByTbIndexID(TbIndexEntity model, string tbid, string indexid)
        {
            try
            {
                string err = tbIndexService.UpMainTbIndex(model, tbid, indexid);
                if (err == "")
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    string str = "[IndexName=" + model.IndexName + "]" + "[ListStat=" + model.ListStat + "]" + "[ListHeadName=" + model.ListHeadName + "]" + "[ColumnWith=" + model.ColumnWith + "]" + "[isPK=" + model.isPK + "]" + "[isUnique=" + model.isUnique + "]" + "[isEmpty=" + model.isEmpty + "]" + "[isColumnShow=" + model.isColumnShow + "]" + "[isColumnShow2=" + model.isColumnShow2 + "]" + "[isSearch=" + model.isSearch + "]" + "[isOrder=" + model.isOrder + "]" + "[isLock=" + model.isLock + "]" + "[isLock2=" + model.isLock2 + "]" + "[isTime=" + model.isTime + "]" + "[IndexOrderNo=" + model.IndexOrderNo + "]" + "[DefaultV=" + model.DefaultV + "]" + "[RuleName=" + model.RuleName + "]" + "[RuleType=" + model.RuleType + "]" + "[RuleId=" + model.RuleId + "]" + "[isSelMuch=" + model.isSelMuch + "]" + "[ControlType=" + model.ControlType + "]...";
                    Record.AddInfo(userid, tbid, "编辑指标属性:" + indexid + str);

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效"));
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
        public ActionResult UpGridTbIndex(TbIndexEntity model, string tbid, string indexid)
        {
            try
            {
                string err = tbIndexService.UpGridTbIndex(model, tbid, indexid);
                if (err == "")
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    string str = "[IndexName=" + model.IndexName + "]" + "[ListStat=" + model.ListStat + "]" + "[ListHeadName=" + model.ListHeadName + "]" + "[ColumnWith=" + model.ColumnWith + "]" + "[isPK=" + model.isPK + "]" + "[isUnique=" + model.isUnique + "]" + "[isEmpty=" + model.isEmpty + "]" + "[isColumnShow=" + model.isColumnShow + "]" + "[isColumnShow2=" + model.isColumnShow2 + "]" + "[isSearch=" + model.isSearch + "]" + "[isOrder=" + model.isOrder + "]" + "[isLock=" + model.isLock + "]" + "[isLock2=" + model.isLock2 + "]" + "[isTime=" + model.isTime + "]" + "[IndexOrderNo=" + model.IndexOrderNo + "]" + "[DefaultV=" + model.DefaultV + "]" + "[RuleName=" + model.RuleName + "]" + "[RuleType=" + model.RuleType + "]" + "[RuleId=" + model.RuleId + "]" + "[isSelMuch=" + model.isSelMuch + "]" + "[ControlType=" + model.ControlType + "]...";
                    Record.AddInfo(userid, tbid, "编辑指标属性:" + indexid + str);

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效"));
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
        public ActionResult UpTbIndexRule(string ruleid, string tbid, string indexid)
        {
            string name = "";
            string err = "";

            string _ruleid = ruleid == null ? "" : ruleid;

            try
            {
                if (_ruleid.Trim() == "")
                {
                    return this.RemoveTbIndexRule(tbid, indexid);
                }
                else
                {
                    err = tbBasicService.UpTbIndexByTbIndexIDAndRuleID(ref name, _ruleid, tbid, indexid);
                }

                if (err == "")
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, tbid, "编辑指标数据规范:" + indexid + "[" + _ruleid + "]");

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效", name));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex.ToString()));
            }
        }

        [HttpPost]
        public ActionResult UpTbIndexRule2(string ruleid, string tbid, string indexid)
        {
            string name = "";
            string err = "";

            string _ruleid = ruleid == null ? "" : ruleid;

            try
            {
                if (_ruleid.Trim() == "")
                {
                    return this.RemoveTbIndexRule(tbid, indexid);
                }
                else
                {
                    err = tbBasicService.UpTbIndexByTbIndexIDAndRuleID(ref name, _ruleid, tbid, indexid);
                }

                if (err == "")
                {
                    //name = "<a target=\"_blank\"  class='layui-btn layui-btn-primary layui-btn-xs' href='/SysTable/RuleLook/RuleDataList?id=" + ruleid + "'>" + name + "<i class='fa fa-file-text-o'></i></a>";
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, tbid, "编辑指标数据规范:" + indexid + "[" + _ruleid + "]");

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效", name));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex.ToString()));
            }
        }

        [HttpPost]
        public ActionResult RemoveTbIndexRule(string tbid, string indexid)
        {
            try
            {
                string Err = tbBasicService.RemoveTbIndexRule(tbid, indexid);

                if (Err == "")
                {
                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效", ""));
                }
                else
                {
                    return Json(ErrorTip(Err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult UpTbIndexControlType(string typeid, string tbid, string indexid)
        {
            try
            {
                string err = tbBasicService.UpTbIndexControlType(typeid, tbid, indexid);

                if (err == "")
                {
                    string userid = CurrentUser == null ? "!NullEx" : CurrentUser.Account;
                    Record.AddInfo(userid, tbid, "编辑指标控件类型:" + indexid + "[" + typeid + "]");

                    return Json(SuccessTip("保存成功，修改后需『重新生成』才能生效"));
                }
                else
                {
                    return Json(ErrorTip(err));
                }
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex.ToString()));
            }
        }
    }
}
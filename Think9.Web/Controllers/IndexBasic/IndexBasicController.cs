using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class IndexBasicController : BaseController
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private ComService comService = new ComService();
        private IndexBasicService indexBasicService = new IndexBasicService();
        private SortService sortService = new SortService();
        private IndexDateType dateTypeService = new IndexDateType();
        private TbIndex tbIndexService = new TbIndex();

        //指标分类
        private SelectList SortList
        { get { return new SelectList(sortService.GetAll("ClassID,SortID,SortName", "ORDER BY SortOrder").Where(x => x.ClassID == "CAT_index"), "SortID", "SortName"); } }

        //指标类型
        private SelectList IndexDtaeTypeList
        {
            get { return new SelectList(dateTypeService.GetIndexDtaeType(), "TypeId", "TypeName"); }
        }

        //批量新增
        public ActionResult BatchAdd()
        {
            ViewBag.SortList = SortList;
            ViewBag.DtaeTypeList = dateTypeService.GetIndexDtaeType();
            ViewBag.Guid = Think9.Services.Basic.CreatCode.NewGuid();

            return View();
        }

        public ActionResult BatchAdd2()
        {
            ViewBag.SortList = SortList;
            ViewBag.DtaeTypeList = dateTypeService.GetIndexDtaeType();

            return View();
        }

        [HttpPost]
        public ActionResult BatchAdd2(IndexBasicEntity model, int num)
        {
            string err = "";
            string strErr = "";
            try
            {
                for (int i = 1; i <= num; i++)
                {
                    IndexBasicEntity entity = new IndexBasicEntity();
                    entity.IndexSort = model.IndexSort;
                    entity.IndexDataType = model.IndexDataType;
                    entity.IndexId = model.IndexId + i.ToString().PadLeft(2, '0');
                    entity.IndexName = model.IndexName + i.ToString().PadLeft(2, '0');

                    err = indexBasicService.AddIndex(entity);
                    if (!string.IsNullOrEmpty(err))
                    {
                        strErr += entity.IndexId + err + "<br>";
                    }
                }

                return Json(string.IsNullOrEmpty(strErr) ? SuccessTip("操作成功") : ErrorTip(strErr));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult BatchAdd(string guid, string sort, List<IndexBasicEntity> list)
        {
            try
            {
                string err = indexBasicService.AddIndex(sort, list);
                if (string.IsNullOrEmpty(err) && !string.IsNullOrEmpty(guid))
                {
                    comService.ExecuteSql("delete from sys_temp where guid='" + guid + "'");
                }

                var result = string.IsNullOrEmpty(err) ? SuccessTip("操作成功") : ErrorTip(err);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult BatchCheck(string sort, List<IndexBasicEntity> list)
        {
            try
            {
                string err = indexBasicService.CheckIndex(sort, list);

                var result = string.IsNullOrEmpty(err) ? SuccessTip("未检测错误") : ErrorTip(err);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public ActionResult GetIndexListByGuid(string guid, string idsStr, string type)
        {
            var result = new { code = 0, msg = "", count = 999999, data = indexBasicService.GetListByidsStr(guid, idsStr, type) };

            return Json(result);
        }

        [HttpGet]
        public override ActionResult Index(int? id)
        {
            ViewBag.DtaeTypeList = dateTypeService.GetIndexDtaeTypeList();
            ViewBag.IndexSortList = SortList;

            return base.Index(id);
        }

        [HttpGet]
        public ActionResult Add(string frm)
        {
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.SortList = SortList;
            ViewBag.DtaeTypeList = dateTypeService.GetIndexDtaeType();

            return View();
        }

        [HttpPost]
        public ActionResult Add(IndexBasicEntity model)
        {
            try
            {
                string err = indexBasicService.AddIndex(model);
                return Json(string.IsNullOrEmpty(err) ? SuccessTip("操作成功") : ErrorTip(err));
            }
            catch (Exception ex)
            {
                return Json(ErrorTip(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.SortList = SortList;
            ViewBag.DtaeTypeList = dateTypeService.GetIndexDtaeType();

            var model = indexBasicService.GetByWhereFirst("where IndexId=@IndexId", new { IndexId = id });
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Json("数据不存在！");
            }
        }

        [HttpPost]
        public ActionResult Edit(IndexBasicEntity model)
        {
            model.UpdateTime = DateTime.Now;
            model.IndexName = TableHelp.CleanInvalidHtmlChars(model.IndexName);

            string where = "where IndexId='" + model.IndexId + "'";
            string updateFields = "IndexSort,IndexName,IndexExplain,UpdateTime";

            tbIndexService.UpdateByWhere(where, "IndexName", model);
            var result = indexBasicService.UpdateByWhere(where, updateFields, model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetIndexList(IndexBasicEntity model, PageInfoEntity pageInfo, string key, string sort, string type)
        {
            string _tbname;
            string _sort = sort == null ? "" : sort; ;
            string _datetype = type == null ? "" : type; ;
            string _keywords = key == null ? "" : key;

            pageInfo.returnFields = "IndexSort,IndexDataType,IndexId,IndexName,IndexExplain";
            pageInfo.field = " UpdateTime desc";

            string where = "where 1=1 ";
            if (_sort != "")
            {
                where += " and IndexSort=@IndexSort ";
                model.IndexSort = _sort;
            }

            if (_datetype != "")
            {
                if (_datetype.Length == 1)
                {
                    where += " and left(IndexDataType, 1)=@IndexDataType ";
                }
                else
                {
                    where += " and IndexDataType=@IndexDataType ";
                }
                model.IndexDataType = _datetype;
            }

            if (_keywords != "")
            {
                where += " and (IndexId like @IndexId OR IndexName like @IndexName) ";
                model.IndexId = string.Format("%{0}%", _keywords);
                model.IndexName = string.Format("%{0}%", _keywords);
            }

            long total = 0;
            IEnumerable<dynamic> list = indexBasicService.GetPageByFilter(ref total, model, pageInfo, where);

            string sql = "select * from sys_datatype";
            DataTable dt = comService.GetDataTable(sql);

            sql = "select * from sys_sort where ClassID='CAT_index' ORDER BY SortOrder";
            DataTable dt2 = comService.GetDataTable(sql);

            sql = @"SELECT a.TbId,a.IndexId,a.IndexName,b.TbName FROM tbindex a
                           INNER JOIN tbbasic b ON a.TbId=b.TbId ORDER BY TbId ASC";
            DataTable dt3 = comService.GetDataTable(sql);

            foreach (Object obj in list)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["TypeId"].ToString() == ((IndexBasicEntity)obj).IndexDataType)
                    {
                        ((IndexBasicEntity)obj).DataTypeName = dr["TypeName"].ToString();
                        break;
                    }
                }

                foreach (DataRow dr in dt2.Rows)
                {
                    if (dr["SortID"].ToString() == ((IndexBasicEntity)obj).IndexSort)
                    {
                        ((IndexBasicEntity)obj).IndexSort = dr["SortName"].ToString();
                        break;
                    }
                }

                _tbname = "";
                foreach (DataRow dr in dt3.Select("IndexId='" + ((IndexBasicEntity)obj).IndexId + "'"))
                {
                    if (dr["IndexId"].ToString() == ((IndexBasicEntity)obj).IndexId)
                    {
                        _tbname += "{" + dr["TbName"].ToString() + "}  ";
                    }
                }
                ((IndexBasicEntity)obj).TableName = _tbname;
            }

            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetIndexListBySearch(IndexBasicEntity model, PageInfoEntity pageInfo, string key, string sort, string type, string show, string tbid)
        {
            string _sort = sort == null ? "" : sort;
            string _datetype = type == null ? "" : type;
            string _keywords = key == null ? "" : key;
            string _show = show == null ? "" : show;
            string _tbid = tbid == null ? "" : tbid;
            if (!_tbid.StartsWith("tb_"))
            {
                _tbid = "tb_" + _tbid;
            }

            pageInfo.returnFields = "IndexSort,IndexDataType,IndexId,IndexName,IndexExplain,UpdateTime";
            pageInfo.field = "UpdateTime";
            pageInfo.order = "desc";

            string where = "where 1=1 ";
            if (_sort != "")
            {
                where += " and IndexSort=@IndexSort ";
                model.IndexSort = _sort;
            }

            if (_datetype != "")
            {
                if (_datetype.Length == 1)
                {
                    where += " and left(IndexDataType, 1)=@IndexDataType ";
                }
                else
                {
                    where += " and IndexDataType=@IndexDataType ";
                }
                model.IndexDataType = _datetype;
            }

            if (_keywords != "")
            {
                where += " and (IndexId like @IndexId OR IndexName like @IndexName) ";
                model.IndexId = string.Format("%{0}%", _keywords);
                model.IndexName = string.Format("%{0}%", _keywords);
            }

            long total = 0;
            var list = indexBasicService.GetPageByFilter(ref total, model, pageInfo, where);
            var result = new { code = 0, msg = "", count = total, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            string where = "where IndexId='" + id + "'";

            if (comService.GetTotal("tbindex", where) > 0)
            {
                return Json(ErrorTip("不能删除！有录入表与指标关联"));
            }

            where = "where IndexId='" + id + "'";
            var result = indexBasicService.DeleteByWhere(where) ? SuccessTip("删除成功") : ErrorTip("操作失败");
            return Json(result);
        }

        [HttpGet]
        public JsonResult BatchDel(string idsStr)
        {
            string str = "";
            string id = "";
            int success = 0;
            int num = 0;

            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null)
                {
                    if (arr[i].ToString().Trim() != "")
                    {
                        id = arr[i].ToString().Trim();
                        string where = "where IndexId='" + id + "'";

                        if (comService.GetTotal("tbindex", where) > 0)
                        {
                            str += arr[i].ToString().Trim() + "不能删除！有录入表与指标关联";
                            num++;
                        }
                        else
                        {
                            where = "where IndexId='" + id + "'";
                            if (indexBasicService.DeleteByWhere(where))
                            {
                                success++;
                            }
                            else
                            {
                                num++;
                            }
                        }
                    }
                }
            }
            if (num == 0)
            {
                return Json(SuccessTip("删除成功"));
            }
            else
            {
                string show = "删除成功" + success.ToString() + "数据 " + " 失败" + num.ToString() + "数据";
                return Json(ErrorTip(show));
            }
        }
    }
}
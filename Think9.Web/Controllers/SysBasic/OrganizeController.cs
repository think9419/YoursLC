using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;

namespace Think9.Controllers.Basic
{
    [Area("SysBasic")]
    public class OrganizeController : BaseController
    {
        private OrganizeService organizeService = new OrganizeService();
        private ComService comService = new ComService();

        public override ActionResult Index(int? id)
        {
            return base.Index(id);
        }

        [HttpGet]
        public JsonResult GetList(PageInfoEntity pageInfo, string key)
        {
            IEnumerable<valueTextEntity> list = null;
            if (string.IsNullOrEmpty(key))
            {
                list = organizeService.GetSelectTreeList();
            }
            else
            {
                list = organizeService.GetSelectTreeList().Where(x => x.Value.Contains(key, StringComparison.OrdinalIgnoreCase) || x.Text.Contains(key, StringComparison.OrdinalIgnoreCase));
            }

            var result = new { code = 0, count = list.Count(), data = list };
            return Json(result);

            //todo 分页先放下
            //var pageList = DataTableHelp.TakePage(list, pageInfo.limit, pageInfo.page);
            //var result = new { code = 0, count = list.Count(), data = pageList };
            //return Json(result);
        }

        [HttpGet]
        public JsonResult GetOrganizeTreeSelect()
        {
            var result = organizeService.GetOrganizeTreeSelect();
            return Json(result);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(OrganizeEntity model)
        {
            model.UpdateTime = DateTime.Now;

            string where = "where EnCode=@EnCode";
            object param = new { EnCode = model.EnCode };
            if (organizeService.GetTotal(where, param) > 0)
            {
                var result = ErrorTip("添加失败!已存在相同编码");
                return Json(result);
            }
            else
            {
                var result = organizeService.Insert(model) ? SuccessTip("操作成功") : ErrorTip("操作失败");
                return Json(result);
            }
        }

        public ActionResult Edit(string id)
        {
            var model = organizeService.GetByWhereFirst("where EnCode=@EnCode", new { EnCode = id });
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
        public ActionResult Edit(OrganizeEntity model)
        {
            string err = "";
            if (model.ParentId == model.EnCode)
            {
                err = "不能选择本级做上级！";
            }
            else
            {
                string strAllUpNO = ".";
                List<OrganizeEntity> list = organizeService.GetPreLevelList(model.ParentId);
                foreach (var item in list)
                {
                    strAllUpNO += item.EnCode + ".";
                }

                if (strAllUpNO.Contains("." + model.EnCode + "."))
                {
                    err = "上级选择错误！";
                }
            }

            if (string.IsNullOrEmpty(err))
            {
                model.UpdateTime = DateTime.Now;
                if (model.EnCode == "top")
                {
                    model.ParentId = "";
                }

                var result = organizeService.UpdateByWhere(" where EnCode='" + model.EnCode + "'", "ParentId,FullName,OrderNo,UpdateTime", model) > 0 ? SuccessTip("操作成功") : ErrorTip("编辑失败");
                return Json(result);
            }
            else
            {
                return Json(ErrorTip(err));
            }
        }

        [HttpGet]
        public JsonResult Delete(string id)
        {
            string err = "";
            var model = organizeService.GetByWhereFirst("where EnCode=@EnCode", new { EnCode = id });

            if (model != null)
            {
                string EnCode = model.EnCode;
                string where = "where ParentId=@EnCode";
                object param = new { EnCode = EnCode };
                if (comService.GetTotal("sys_organize", where, param) > 0)
                {
                    err = "该单位(部门)包含下级，不能删除！";
                }

                if (EnCode == "top")
                {
                    err = "不能删除顶级机构！";
                }

                if (err == "")
                {
                    where = "where DeptNo=@EnCode";
                    if (comService.GetTotal("sys_users", where, param) > 0)
                    {
                        err = "该单位(部门)包含用户，不能删除！";
                    }
                }
            }
            else
            {
                err = "单位(部门)不存在！";
            }

            if (err != "")
            {
                return Json(ErrorTip(err));
            }
            else
            {
                var result = organizeService.DeleteByWhere(" where EnCode='" + id + "'") ? SuccessTip("删除成功") : ErrorTip("操作失败");
                return Json(result);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Com;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbUtilityController : BaseController
    {
        private ComService comService = new ComService();
        private TbTagService tbTagService = new TbTagService();
        private TbIndexService tbIndex = new TbIndexService();
        private TbPdfSize tbPdfSize = new TbPdfSize();
        private string str;

        [HttpPost]
        public JsonResult SetPdfSize(string tbid, IEnumerable<ControlEntity> list)
        {
            TbPdfSizeEntity model = new TbPdfSizeEntity();
            model.TbId = tbid;
            model.Type = "formpdf";
            str = BasicHelp.GetValueFrmList(list, "Width");
            if (ValidatorHelper.IsNumberic(str))
            {
                model.Width = ExtConvert.ToDecimalOrNull(str);
            }
            str = BasicHelp.GetValueFrmList(list, "Heigh");
            if (ValidatorHelper.IsNumberic(str))
            {
                model.Heigh = ExtConvert.ToDecimalOrNull(str);
            }
            str = BasicHelp.GetValueFrmList(list, "Top");
            if (ValidatorHelper.IsNumberic(str))
            {
                model.Top = ExtConvert.ToDecimalOrNull(str);
            }
            str = BasicHelp.GetValueFrmList(list, "Left");
            if (ValidatorHelper.IsNumberic(str))
            {
                model.Left = ExtConvert.ToDecimalOrNull(str);
            }
            str = BasicHelp.GetValueFrmList(list, "Right");
            if (ValidatorHelper.IsNumberic(str))
            {
                model.Right = ExtConvert.ToDecimalOrNull(str);
            }
            str = BasicHelp.GetValueFrmList(list, "Bottom");
            if (ValidatorHelper.IsNumberic(str))
            {
                model.Bottom = ExtConvert.ToDecimalOrNull(str);
            }

            tbPdfSize.DeleteByWhere("where TbId = '" + tbid + "' and  Type='formpdf'");
            if (tbPdfSize.Insert(model))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("操作失败"));
            }
        }

        [HttpPost]
        public ActionResult AddTbTag(IEnumerable<ControlEntity> list)
        {
            TbTagEntity model = new TbTagEntity();
            model.Type = "list";
            model.TbId = BasicHelp.GetValueFrmList(list, "TbId");
            model.TagName = BasicHelp.GetValueFrmList(list, "TagName");
            model.FilterStr = BasicHelp.GetValueFrmList(list, "FilterStr");
            string str = BasicHelp.GetValueFrmList(list, "OrderNo");
            if (ValidatorHelper.IsInteger(str))
            {
                model.OrderNo = ExtConvert.ToIntOrNull(str);
            }

            if (comService.GetTotal("tbtag", "where TbId=@TbId and OrderNo=@OrderNo and Type=@Type", model) > 0)
            {
                return Json(ErrorTip("存在相同序号"));
            }

            if (tbTagService.Insert(model))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("添加失败"));
            }
        }

        [HttpGet]
        public ActionResult DeleteTag(int id)
        {
            if (tbTagService.DeleteById(id))
            {
                return Json(SuccessTip("操作成功，修改后需『重新生成』才能生效"));
            }
            else
            {
                return Json(ErrorTip("删除失败"));
            }
        }

        [HttpGet]
        public JsonResult GetTagList(string tbid)
        {
            string where = "where TbId='" + tbid + "' ";
            var result = new { code = 0, msg = "", count = 999999, data = tbTagService.GetByWhere(where) };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetIndexLinkList(string tbid)
        {
            string where = "where TbId='" + tbid + "' and LinkFlag != ''";
            var result = new { code = 0, msg = "", count = 999999, data = tbIndex.GetByWhere(where) };
            return Json(result);
        }
    }
}
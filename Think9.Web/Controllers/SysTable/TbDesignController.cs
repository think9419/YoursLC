using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.CodeBuild;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public class TbDesignController : BaseController
    {
        private TbJson tbJson = new TbJson();
        private TbIndexService tbIndexService = new TbIndexService();
        private ComService comService = new ComService();
        private TbBasicService tbBasicService = new TbBasicService();
        private IndexDateType indexDtaeTypeService = new IndexDateType();

        private readonly IWebHostEnvironment _webHostEnvironment;

        public TbDesignController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [HttpPost]
        public JsonResult GetIndexListByTbIDAndTag(string tbid, string tag)
        {
            string _tbid = tbid == null ? "" : tbid;

            IEnumerable<dynamic> list = tbIndexService.GetIndexByTbIDAndTag(tbid, tag);

            return Json(list);
        }

        [HttpPost]
        public JsonResult GetGridList(string tbid)
        {
            tbid = tbid == null ? "" : tbid;

            IEnumerable<TbBasicEntity> list = tbBasicService.GetByWhere("where ParentId=@tbid", new { tbid = tbid }, "TbId,TbName");

            return Json(list);
        }

        [HttpPost]
        public JsonResult GetIndexByTbIDAndIndexID(string tbid, string indexid)
        {
            string _tbid = tbid == null ? "" : tbid;
            IEnumerable<dynamic> list = tbIndexService.GetIndexByTbIDAndIndexID(tbid, indexid);
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetJsonStr(string tbid, string type, string nColumns)
        {
            nColumns = string.IsNullOrEmpty(nColumns) ? "4" : nColumns;

            DataTable dtIndex = comService.GetDataTable("select * from tbindex order by TbId,IndexOrderNo");
            DataTable allTB = comService.GetDataTable("select * from tbbasic ");
            TableStyle tbStyle = new TableStyle(allTB, dtIndex);

            TbFormService tbFormService = new TbFormService();
            DataTable dtTbForm = comService.GetDataTable("select * from tbform where TbId='" + tbid + "' ");
            DataTable dtLabel = tbFormService.GetLabelDataTable(tbid);
            DataTable dtLine = tbFormService.GetLineDataTable(tbid, dtLabel);
            DataTable dtColumn = tbFormService.GetColumnDataTable(tbid, dtLine);

            string str = "";

            if (dtIndex.Select("tbid='" + tbid + "'").Length == 0)
            {
                return Json("ERR:未定义录入指标，不能自动生成");
            }

            try
            {
                if (type == "99")
                {
                    if (dtTbForm.Rows.Count == 0)
                    {
                        return Json("ERR:快速排列无数据，请点击『快速排列』按钮进入『快速排列页面』进行设置");
                    }
                    str = tbStyle.GetJsonBySet(tbid, dtLabel, dtLine, dtColumn, dtTbForm, nColumns);
                }
                else
                {
                    str = tbStyle.GetJsonStrByTbID(tbid, type);
                }

                return Json(str);
            }
            catch (Exception ex)
            {
                str = ex.ToString();
                return Json("ERR:" + ex.ToString());
            }
        }

        [HttpPost]
        public JsonResult GetGridTableHtml(string tbid)
        {
            return Json(tbBasicService.GetGridTableHtmlByTbID(tbid));
        }

        [HttpPost]
        public JsonResult CreatJsonFile(string tbid, string json)
        {
            json = json.Replace("@", "＠");
            FormControlEntity model = new FormControlEntity();

            try
            {
                string strPath = $"{this._webHostEnvironment.WebRootPath}\\TbJson\\";
                Think9.Util.Helper.FileHelper.CreateFile(strPath + "" + tbid + ".json", json);

                DataTable _dtindex = comService.GetDataTable("select * from tbindex order by TbId,IndexOrderNo");
                DataTable allTB = comService.GetDataTable("select * from tbbasic ");
                TableStyle TBStyle = new TableStyle(allTB, _dtindex);
                List<FormControlEntity> list = Conversion(TBStyle.GetFormControlList(tbid, json));

                string err = tbJson.CheckControlFromObj(list, tbid);

                if (string.IsNullOrEmpty(err))
                {
                    err = tbJson.UpTbIndexByControlFromObj(list, tbid);//再修改tbindexid表
                }

                if (string.IsNullOrEmpty(err))
                {
                    return Json(SuccessTip("操作成功"));
                }
                else
                {
                    return Json(ErrorTip("验证失败！" + err));
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult SaveJsonFile(string tbid, string json)
        {
            try
            {
                string strPath = $"{this._webHostEnvironment.WebRootPath}\\TbJson\\";

                Think9.Util.Helper.FileHelper.CreateFile(strPath + "" + tbid + ".json", json);

                return Json(SuccessTip(""));
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return Json(ErrorTip(ex));
            }
        }

        [HttpPost]
        public JsonResult GetJsonFromApi(string tbid)
        {
            string json = "load";
            try
            {
                string strPath = $"{this._webHostEnvironment.WebRootPath}\\TbJson\\" + tbid + ".json";

                if (Directory.Exists(Path.GetDirectoryName(strPath)))
                {
                    json = Think9.CreatCode.FileHelper.FileToString(strPath);
                }

                return Json(json);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return Json("err");
            }
        }

        private List<FormControlEntity> Conversion(List<TableFormControlModel> list)
        {
            List<FormControlEntity> _list = new List<FormControlEntity>();
            foreach (TableFormControlModel obj in list)
            {
                FormControlEntity _obj = new FormControlEntity();

                _obj._default = obj._default;
                _obj._readonly = obj._readonly;
                _obj.index = obj.index;
                _obj.tag = obj.tag;
                _obj.label = obj.label;
                _obj.labelhide = obj.labelhide;
                _obj.name = obj.name;
                _obj.type = obj.type;
                _obj.placeholder = obj.placeholder;
                _obj.min = obj.min;
                _obj.max = obj.max;
                _obj.maxlength = obj.maxlength;
                _obj.verify = obj.verify;
                _obj.width = obj.width;
                _obj.height = obj.height;
                _obj.lay_skin = obj.lay_skin;
                _obj.labelwidth = obj.labelwidth;
                _obj.uploadtype = obj.uploadtype;
                _obj.disabled = obj.disabled;
                _obj.required = obj.required;
                _obj.lay_search = obj.lay_search;
                _obj.data_datetype = obj.data_datetype;
                _obj.data_maxvalue = obj.data_maxvalue;
                _obj.data_minvalue = obj.data_minvalue;
                _obj.data_dateformat = obj.data_dateformat;
                _obj.data_half = obj.data_half;
                _obj.theme = obj.theme;
                _obj.data_theme = obj.data_theme;
                _obj.data_color = obj.data_color;
                _obj.data_default = obj.data_default;
                _obj.data_value = obj.data_value;
                _obj.msg = obj.msg;
                _obj.textarea = obj.textarea;
                _obj.column = obj.column;

                _list.Add(_obj);
            }

            return _list;
        }
    }
}
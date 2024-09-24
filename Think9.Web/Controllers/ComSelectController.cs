using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Basic;
using Think9.Services.Table;

namespace Think9.Controllers.Basic
{
    [Area("Com")]
    public class ComSelectController : BaseController
    {
        private UserService userService = new UserService();
        private ComService comService = new ComService();
        private OrganizeService organizeService = new OrganizeService();
        private RoleService roleService = new RoleService();
        private TbIndexService tbIndexService = new TbIndexService();

        [HttpGet]
        public ActionResult SelectParm()
        {
            return View();
        }

        public ActionResult SelectParm2(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        public ActionResult SelectFileType()
        {
            return View();
        }

        public ActionResult SelectImgType()
        {
            return View();
        }

        public ActionResult SelectIndexStatsParm()
        {
            return View();
        }

        public ActionResult SelectUser(string fId, string sId)
        {
            ViewBag.fromId = fId;
            ViewBag.strId = sId == null ? "" : sId;
            ViewBag.depList = new SelectList(organizeService.GetSelectTreeList(), "Value", "Text");

            return View();
        }

        public ActionResult SelectIndexStats(string type, string frm)
        {
            ViewBag.type = type == null ? "" : type;
            ViewBag.frm = frm == null ? "" : frm;

            return View();
        }

        public ActionResult SelectStatsField(string tbid, string frm)
        {
            ViewBag.frm = string.IsNullOrEmpty(frm) ? "" : frm;
            ViewBag.tbid = string.IsNullOrEmpty(tbid) ? "" : tbid;

            return View();
        }

        public ActionResult SelectDept(string fId, string sId)
        {
            ViewBag.fromId = fId;
            ViewBag.strId = sId == null ? "" : sId;

            return View();
        }

        public ActionResult SelectRole(string fId, string sId)
        {
            ViewBag.fromId = fId;
            ViewBag.strId = sId == null ? "" : sId;

            return View();
        }

        public ActionResult SelectIndexAndGridTb(string fId, string sId, string tbid)
        {
            ViewBag.fromId = fId;
            ViewBag.tbid = tbid == null ? "" : tbid;
            ViewBag.strId = sId == null ? "" : sId;

            return View();
        }

        [HttpPost]
        public JsonResult GetUserListBySearch(UserEntity model, PageInfoEntity pageInfo, string key, string dep)
        {
            string _key = key == null ? "" : key;
            string _dep = dep == null ? "" : dep;

            string where = "where 1=1 ";
            if (_key != "")
            {
                where += " and (Account like @Account OR RealName like @Account) ";
                model.Account = string.Format("%{0}%", _key);
            }
            if (_dep != "")
            {
                where += " and (DeptNo = @DeptNo) ";
                model.DeptNo = _dep;
            }

            long total = 0;
            IEnumerable<dynamic> list = userService.GetPageByFilter(ref total, model, pageInfo, where);

            var result = new { code = 0, msg = "", count = total, data = list };

            return Json(result);
        }

        [HttpPost]
        public ActionResult GetUserListByStr(string strID)
        {
            string _id = strID == null ? "" : strID;
            List<valueTextEntity> list = new List<valueTextEntity>();
            string sql = "Select Account AS id ,RealName AS name  from sys_users";
            DataTable dt = comService.GetDataTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                if (_id.Contains(";" + dr["id"].ToString() + ";"))
                {
                    list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
                }
            }

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetDeptTreeList()
        {
            var result = new { code = 0, count = 999999, data = organizeService.GetSelectTreeList() };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetDeptListByStr(string strID)
        {
            string _id = strID == null ? "" : strID;
            List<valueTextEntity> list = new List<valueTextEntity>();
            string sql = "Select EnCode AS id ,FullName AS name  from sys_organize";
            DataTable dt = comService.GetDataTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                if (_id.Contains(";" + dr["id"].ToString() + ";"))
                {
                    list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
                }
            }

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetRoleList()
        {
            var result = new { code = 0, count = 999999, data = roleService.GetAll() };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetRoleListByStr(string strID)
        {
            string _id = strID == null ? "" : strID;
            List<valueTextEntity> list = new List<valueTextEntity>();
            string sql = "Select EnCode AS id ,FullName AS name  from sys_role";
            DataTable dt = comService.GetDataTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                if (_id.Contains(";" + dr["id"].ToString() + ";"))
                {
                    list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
                }
            }

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetIndexAndSonTbList(string tbid)
        {
            string _tbid = tbid == null ? "" : tbid;

            List<valueTextEntity> list = new List<valueTextEntity>();

            string sql = "Select IndexId AS id ,IndexName AS name  from tbindex where TbId='" + _tbid + "' order by IndexNo";
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
            }

            sql = "Select TbId AS id ,TbName AS name  from tbbasic where ParentId='" + _tbid + "'  and TbType = '2'";
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
            }

            var result = new { code = 0, count = 999999, data = list };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetIndexAndSonTbListByStr(string tbid, string strID)
        {
            string _id = strID == null ? "" : strID;
            string _tbid = tbid == null ? "" : tbid;

            List<valueTextEntity> list = new List<valueTextEntity>();
            string sql = "Select IndexId AS id ,IndexName AS name  from tbindex where TbId='" + _tbid + "' order by IndexNo";

            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                if (_id.Contains(";" + dr["id"].ToString() + ";"))
                {
                    list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
                }
            }

            sql = "Select TbId AS id ,TbName AS name  from tbbasic where ParentId='" + _tbid + "' and TbType = '2'";
            foreach (DataRow dr in comService.GetDataTable(sql).Rows)
            {
                if (_id.Contains(";" + dr["id"].ToString() + ";"))
                {
                    list.Add(new valueTextEntity { ClassID = "", Value = dr["id"].ToString(), Text = dr["name"].ToString() });
                }
            }

            var result = new { code = 0, msg = "", count = 999999, data = list };
            return Json(result);
        }
    }
}
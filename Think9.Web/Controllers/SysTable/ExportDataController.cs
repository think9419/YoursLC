using Microsoft.AspNetCore.Mvc;
using System.IO;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Com;

namespace Think9.Controllers.Basic
{
    [Area("SysTable")]
    public partial class ExportDataController : BaseController
    {
        private ComService comService = new ComService();

        public ActionResult ExportData(string tbid)
        {
            string err = "";
            string guid = System.Guid.NewGuid().ToString("N");
            string directory = Directory.GetCurrentDirectory();
            CreatTableRdlc creatRdlc = new CreatTableRdlc();
            string newFileName = guid + ".xlsx";
            string sourcePath = Path.Combine(directory, "wwwroot\\TempFile\\" + newFileName);

            string str = creatRdlc.CreatListRdlc(tbid, comService.GetDataTable("select * from tbindex where TbId = '" + tbid + "'  ORDER BY IndexOrderNo"));
            FileHelper.CreateFile(directory + "\\wwwroot\\TempFile\\" + guid + ".rdlc", str);

            RdlcDeviceEntity device = new RdlcDeviceEntity();
            device.GridDt = comService.GetDataTable("select * from " + tbid + "    ORDER BY listid  ");//列表数据
            device.PathRdlc = Path.Combine(directory, "wwwroot\\TempFile\\" + guid + ".rdlc");//rdlc默认模板

            byte[] _byte = RDLCReport.ExportExcelList(ref err, device);
            if (!string.IsNullOrEmpty(err))
            {
                return Json(ErrorTip(err));
            }
            Think9.Util.Helper.FileHelper.CreateFile(sourcePath, _byte);
            string url = "<br><br><div class=\"layui-form-item\" style=\"text-align: center;width: 200px;\"><a href='../TempFile/" + newFileName + "' target ='_blank' class='layui-btn'>点击下载</a></div>";
            return Json(SuccessTip("", url));
        }
    }
}
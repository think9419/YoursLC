using System.Data;
using System.IO;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Com;

namespace Think9.Controllers.Basic
{
    public class _TBService
    {
        private ComService comService = new ComService();

        /// <summary>
        /// 获取rdlc报表的相关数据
        /// </summary>
        /// <param name="listid">数据主键</param>
        /// <param name="flowid">流程编码</param>
        /// <param name="tbid">主表编码</param>
        /// <param name="tbname">主表名称</param>
        /// <returns></returns>
        public RdlcDeviceEntity GetRdlcDevice(CurrentUserEntity user, string listid, string flowid, string tbname, string hostUrl)
        {
            string tbid = flowid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string err = "";
            string directory = Directory.GetCurrentDirectory();

            RdlcDeviceEntity device = PdfService.GetPdfDevice(tbid);
            device.FlowId = flowid;
            device.TbId = tbid;
            device.TbName = tbname;
            device.ListId = listid;
            device.PathRdlc = Path.Combine(directory, "wwwroot\\Reports\\" + flowid.Replace("bi_", "").Replace("fw_", "") + ".rdlc");//rdlc模板文件
            if (!File.Exists(device.PathRdlc))
            {
                device.Err = device.PathRdlc + " --rdlc模板文件不存在";
                return device;
            }
            device.MainDt = PageCom.GetMainTbDt(ref err, flowid, listid, hostUrl, device.PathUserImg, device.ImgNoExist, user);//主表数据
            device.GridDt = PageCom.GetGridTbDt(ref err, flowid, listid, hostUrl, device.PathUserImg, device.ImgNoExist, user);//子表数据

            if (!string.IsNullOrEmpty(err))
            {
                device.Err = err;
            }

            return device;
        }

        public RdlcDeviceEntity GetRdlcDevice(string flowid, DataTable gridDt)
        {
            string directory = Directory.GetCurrentDirectory();
            string tbid = flowid.Replace("bi_", "").Replace("fw_", "");

            RdlcDeviceEntity device = PdfService.GetPdfDevice();
            device.FlowId = flowid;
            device.PathRdlc = Path.Combine(directory, "wwwroot\\Reports\\list_" + tbid + ".rdlc");//rdlc模板文件
            device.GridDt = gridDt;//子表数据

            return device;
        }

        public string GetPrcnoOrEdit(string fwid, string prcno)
        {
            string prcnoOrEdit = prcno;
            if (fwid.StartsWith("fw_"))
            {
                string flowType = comService.GetSingleField("select flowType  FROM  flow  WHERE flowid='" + fwid + "' ");
                //流程类型？1固定2自由流程 0无流程
                if (flowType == "1")
                {
                    prcnoOrEdit = prcno;
                }
                else
                {
                    prcnoOrEdit = "edit";
                }
            }
            else
            {
                prcnoOrEdit = "edit";
            }

            return prcnoOrEdit;
        }

        public string GetPrcnoOrFinish(string fwid)
        {
            string prcnoOrFinish = "";
            if (fwid.StartsWith("fw_"))
            {
                string flowType = comService.GetSingleField("select flowType  FROM  flow  WHERE flowid='" + fwid + "' ");
                //流程类型？1固定2自由流程 0无流程
                if (flowType == "1")
                {
                    prcnoOrFinish = "_finish";
                }
                else
                {
                    prcnoOrFinish = "finish";
                }
            }
            else
            {
                prcnoOrFinish = "finish";
            }

            return prcnoOrFinish;
        }
    }
}
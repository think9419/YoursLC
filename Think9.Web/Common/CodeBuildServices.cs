using System.Data;
using System.IO;
using System.IO.Compression;
using Think9.CreatCode;
using Think9.Models;
using Think9.Services.Base;
using Think9.Services.Table;

namespace Think9.Services.Basic
{
    public class CodeBuildServices
    {
        private TbBasicService tbBasicService = new TbBasicService();
        private ComService comService = new ComService();
        private Build build = new Build();
        private string _dbtype = Think9.Services.Base.Configs.GetDBProvider("DBProvider");//数据库类型

        public string GetCountInfo(string idsStr)
        {
            string msg = "";

            int num = 0;
            if (!string.IsNullOrEmpty(idsStr))
            {
                string[] arr = HelpCode.GetStrArray(idsStr, ",");//
                num = arr.GetLength(0);
            }

            string url = Think9.Services.Base.Configs.GetValue("YoursLCSA");
            string appId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");
            string pwd = Think9.Services.Base.Configs.GetValue("YoursLCPassWord");

            msg = build.GetCountByServer(url, appId, pwd, num.ToString());

            return msg;
        }

        /// <summary>
        /// 生成单独得录入表js文件，在wwwroot文件夹中生成js文件时会引起页面刷新，所以需单独提出来处理
        /// </summary>
        /// <param name="idsStr"></param>
        /// <param name="siteRoot"></param>
        /// <param name="_dbtype"></param>
        /// <returns></returns>
        public string GetTableJSCode(DataTable dtRecord, string idsStr, string siteRoot, string _dbtype)
        {
            Creat _creat = new Creat();
            string err = "";

            TableDtModel dtModel = AppSet.GetTableDtModel();
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype);

            string[] arr = HelpCode.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null && arr[i].ToString().Trim() != "")
                {
                    string currentTbId = arr[i].ToString().Trim();
                    dtModel.sonList = DataTableCom.GetSonDt(currentTbId, dtModel.allTB);
                    err += _creat.CreatJSCodeForRelease(siteRoot, currentTbId, dtModel, modelSet, dtRecord);
                }
            }

            return err;
        }

        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="idsStr"></param>
        /// <param name="siteRoot"></param>
        /// <param name="guid"></param>
        /// <param name="user"></param>
        /// <param name="type"></param>
        public string GetReportCode(DataTable dtRecord, string idsStr, string siteRoot, string guid, CurrentUserEntity user, string type, int? timesCount = null)
        {
            HelpCode.CreatDESEncryptFile(siteRoot);

            Think9.CreatCode.AppGetTempletStr.GetDESEncryptStr(AppSet.GetSetModel(Path.Combine(Directory.GetCurrentDirectory(), ""), HelpCode.GetDBProvider("DBProvider")));

            string err = "";
            BaseConfigModel.Creator = user == null ? "!NullEx" : user.Account;

            ReportDtModel dtModel = AppSet.GetReportDtModel(_dbtype);
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype);

            err = build.BuildReportCode(dtRecord, idsStr, siteRoot, guid, type, dtModel, modelSet, timesCount, user.LoginIPAddress);

            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null && !string.IsNullOrEmpty(arr[i].ToString().Trim()))
                {
                    if (type == "release")
                    {
                        Record.AddInfo(user == null ? ";!NullEx;" : user.Account, arr[i].ToString().Trim(), "重新生成");
                    }
                    else
                    {
                        Record.AddInfo(user == null ? ";!NullEx;" : user.Account, arr[i].ToString().Trim(), "代码下载");
                    }
                }
            }

            return err;
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="idsStr"></param>
        /// <param name="siteRoot"></param>
        /// <param name="guid"></param>
        /// <param name="user"></param>
        /// <param name="isAutoCreat"></param>
        /// <param name="isTimesCount">是否计次次数</param>
        /// <returns></returns>
        public string GetTableCode(DataTable dtRecord, string idsStr, string siteRoot, string guid, CurrentUserEntity user, string isAutoCreat, int? timesCount = null, string directoryType = "1")
        {
            HelpCode.CreatDESEncryptFile(siteRoot);//要删除的

            Think9.CreatCode.AppGetTempletStr.GetDESEncryptStr(AppSet.GetSetModel(Path.Combine(Directory.GetCurrentDirectory(), ""), HelpCode.GetDBProvider("DBProvider")));

            string err = "";
            BaseConfigModel.Creator = user == null ? "!NullEx" : user.Account;

            TableDtModel dtModel = AppSet.GetTableDtModel();
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype, directoryType);

            err = build.BuildTableCode(dtRecord, idsStr, siteRoot, guid, isAutoCreat, dtModel, modelSet, _dbtype, timesCount, user.LoginIPAddress);

            string[] arr = BaseUtil.GetStrArray(idsStr, ",");//
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (arr[i] != null && !string.IsNullOrEmpty(arr[i].ToString().Trim()))
                {
                    if (isAutoCreat == "release")
                    {
                        Record.AddInfo(user == null ? ";!NullEx;" : user.Account, arr[i].ToString().Trim(), "重新生成");
                    }
                    else
                    {
                        Record.AddInfo(user == null ? ";!NullEx;" : user.Account, arr[i].ToString().Trim(), "代码下载");
                    }
                }
            }

            return err;
        }

        /// <summary>
        /// 代码查看
        /// </summary>
        /// <param name="siteRoot"></param>
        /// <param name="maintbid"></param>
        /// <returns></returns>
        public CodeBuildEntity LookOverTableCode(ref string err, string siteRoot, string maintbid, CurrentUserEntity user)
        {
            Think9.Models.CodeBuildEntity _model = new CodeBuildEntity();
            _model.TbId = maintbid;

            TableDtModel dtModel = AppSet.GetTableDtModel(maintbid);
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype);

            var obj = build.BuildTableCodeForLookOver(ref err, siteRoot, maintbid, _dbtype, dtModel, modelSet, user.LoginIPAddress);
            if (string.IsNullOrEmpty(err))
            {
                _model.Controllers = obj.Controllers;
                _model.Services = obj.Services;
                _model.Models = obj.Models;
                _model.Views_Index = obj.Views_Index;
                _model.Views_Form = obj.Views_Form;
                _model.Views_Detail = obj.Views_Detail;
                _model.Views_Main_Pop = obj.Views_Main_Pop;
                _model.Views_Grid_Pop = obj.Views_Grid_Pop;
                _model.Self_JS = obj.Self_JS;
                _model.Reports_Rdlc = obj.Reports_Rdlc;
                _model.CreatDataTable = obj.CreatDataTable;

                StringPlus creatDataTable = new StringPlus();
                TbBasicEntity mTb = tbBasicService.GetByWhereFirst("where TbId=@TbId", new { TbId = maintbid });
                if (mTb != null)
                {
                    string type = string.IsNullOrEmpty(mTb.ParentId) ? "1" : "2";
                    string str = comService.GetCreatDBStr(maintbid, type, mTb.FlowId);
                    creatDataTable.AppendLine(str);

                    foreach (DataRow dr in comService.GetDataTable("Select TbId AS id  from tbbasic where ParentId='" + maintbid + "' ").Rows)
                    {
                        str = comService.GetCreatDBStr(dr["id"].ToString(), "2", mTb.FlowId);
                        creatDataTable.AppendLine(str);
                    }
                }
                _model.CreatDataTable = creatDataTable.Value;
            }

            return _model;
        }

        /// <summary>
        /// 代码查看
        /// </summary>
        /// <param name="siteRoot"></param>
        /// <param name="rpId"></param>
        /// <returns></returns>
        public CodeBuildEntity LookOverReportCode(ref string err, string siteRoot, string rpId, CurrentUserEntity user)
        {
            ReportDtModel dtModel = AppSet.GetReportDtModel(_dbtype);
            AppSetEntity modelSet = AppSet.GetSetModel(siteRoot, _dbtype);

            Think9.Models.CodeBuildEntity _model = new CodeBuildEntity();
            _model.TbId = rpId;

            var obj = build.BuildReportCodeForLookOver(ref err, siteRoot, rpId, dtModel, modelSet, user.LoginIPAddress);
            if (string.IsNullOrEmpty(err))
            {
                _model.Controllers = obj.Controllers;
                _model.Services = obj.Services;
                _model.Models = obj.Models;
                _model.Views_Index = obj.Views_Index;
                _model.Views_Form = obj.Views_Form;
                _model.Views_Detail = obj.Views_Detail;
                _model.Views_Main_Pop = obj.Views_Main_Pop;
                _model.Views_Grid_Pop = obj.Views_Grid_Pop;
                _model.Self_JS = obj.Self_JS;
                _model.Reports_Rdlc = obj.Reports_Rdlc;
                _model.CreatDataTable = obj.CreatDataTable;
            }

            return _model;
        }

        /// <summary>
        /// 文件压缩
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public string CompressFile(string prefix)
        {
            string siteRoot = Path.Combine(Directory.GetCurrentDirectory(), "");
            string filePath = siteRoot + "\\AppCode_Temp\\" + prefix + "\\";//
            string zipPath = siteRoot + "\\wwwroot\\CreatCode\\" + prefix + ".zip";

            ZipFile.CreateFromDirectory(filePath, zipPath);

            Think9.Util.Helper.FileHelper.DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory(), "AppCode_Temp\\" + prefix + "\\"));

            string url = "";

            url += "<br><div class=\"layui-form-item\" style=\"text-align: center;\"><a href='../CreatCode/" + prefix + ".zip' target ='_blank'  class='layui-btn'>点击下载</a></div>";
            url += "<div class=\"message-item-text\"><span style=\"margin-left:10px; margin-right:10px;color: #999;\">下载后，参看说明将文件按要求放置，并启动调试模式，【启动调试模式】可使用代码调式功能</span></div>";
            url += "<div class=\"message-item-text\"><span style=\"margin-left:10px; margin-right:10px;color: #999;\">备注：生成后再运行时需考虑清理浏览器缓存(尤其当有子表时)</span></div>";

            return url;
        }
    }
}
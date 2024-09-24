using System.Data;
using Think9.CreatCode;
using Think9.Services.Base;

namespace Think9.Services.Basic
{
    public partial class AppSet
    {
        public static TableDtModel GetTableDtModel(string maintbid)
        {
            TableDtModel model = new TableDtModel();
            ComService comService = new ComService();

            string sql = "select * from tbtag order by TbId,OrderNo";
            DataTable dtTbTag = comService.GetDataTable(sql);
            model.dtTbTag = dtTbTag;

            sql = "select * from tbbutcustomize order by TbId,GridId";
            DataTable dtTbButCustomize = comService.GetDataTable(sql);
            model.dtTbButCustomize = dtTbButCustomize;

            sql = "select * from tbbutcustomizeevent order by BtnId,OrderNo";
            DataTable dtTbButCustomizeEvent = comService.GetDataTable(sql);
            model.dtTbButCustomizeEvent = dtTbButCustomizeEvent;

            sql = "select * from tbeventcustomize order by TbId,OrderNo";
            DataTable dtTbEventCustomize = comService.GetDataTable(sql);
            model.dtTbEventCustomize = dtTbEventCustomize;

            sql = "select * from tbeventpara order by TbId,Id";
            DataTable dtTbEventPara = comService.GetDataTable(sql);
            model.dtTbEventPara = dtTbEventPara;

            sql = "select * from tbindex order by TbId,IndexOrderNo";
            DataTable _dtindex = comService.GetDataTable(sql);
            model.dtIndex = _dtindex;

            sql = "select * from tbbasic where ParentId='" + maintbid + "'";
            DataTable sonlist = comService.GetDataTable(sql);
            model.sonList = sonlist;

            sql = "select * from tbbasic ";
            DataTable allTB = comService.GetDataTable(sql);
            model.allTB = allTB;

            sql = "select * from tbbasic2 ";
            DataTable allTB2 = comService.GetDataTable(sql);
            model.allTB2 = allTB2;

            sql = "select * from rulesingle";
            DataTable dtrulesingle = comService.GetDataTable(sql);
            model.dtRuleSingle = dtrulesingle;

            sql = "select * from rulemultiple";
            DataTable dtrulemultiple = comService.GetDataTable(sql);
            model.dtRuleMultiple = dtrulemultiple;

            sql = "select * from rulemultiplefiled order by RuleId,FiledOrder";
            DataTable dtrulemultiplefiled = comService.GetDataTable(sql);
            model.dtRuleMultipleFiled = dtrulemultiplefiled;

            sql = "select * from ruletree";
            DataTable dtruletree = comService.GetDataTable(sql);
            model.dtRuleTree = dtruletree;

            sql = "select * from flow";
            DataTable dtflow = comService.GetDataTable(sql);
            model.dtFlow = dtflow;

            sql = "select * from flowprcs order by FlowId,PrcsNo";
            DataTable dtflowprcs = comService.GetDataTable(sql);
            model.dtFlowPrcs = dtflowprcs;

            sql = "select * from tbbut order by TbId";
            DataTable dtTbBut = comService.GetDataTable(sql);
            model.dtTbBut = dtTbBut;

            sql = "SELECT tbrelation.*,relationlist.* FROM  tbrelation INNER JOIN relationlist ON tbrelation.RelationId = relationlist.RelationId  ";
            DataTable dtVIEW_TbRelationRD = comService.GetDataTable(sql);
            model.dtVIEW_TbRelationRD = dtVIEW_TbRelationRD;

            sql = "select * from relationrd order by RelationId";
            DataTable dtRelationRD = comService.GetDataTable(sql);
            model.dtRelationRD = dtRelationRD;

            sql = "select * from relationrdfield order by ID, FillIndexId";//order by ID很重要--对数据联动
            DataTable dtRelationRDField = comService.GetDataTable(sql);
            model.dtRelationRDField = dtRelationRDField;

            //sql = @"SELECT RelationList.RelationId, RelationList.RelationType, RelationList.RelationName, RelationList.RelationBy, RelationList.RelationFlag, RelationList.FlowStr, RelationList.ICount,  RelationWD.SourceFlowId, RelationWD.SourceTbId, RelationWD.FlowPrcs, RelationWD.WriteTbId,RelationWD.RelationWDFlag, RelationWD.OrderNo, RelationWD.RelationWDBy, TbRelation.TbID,TbRelation.FlowScope, RelationWD.WhereStr, RelationWD.NumberType, RelationWD.RelationSort,RelationList.DbID FROM  RelationList INNER JOIN RelationWD ON RelationList.RelationId = RelationWD.RelationId INNER JOIN TbRelation ON RelationList.RelationId = TbRelation.RelationId  where TbRelation.isUse='1' order by RelationId";
            sql = @"SELECT relationlist.RelationId, relationlist.RelationType, relationlist.RelationName, relationlist.RelationBy, relationlist.RelationFlag, relationlist.FlowStr, relationlist.ICount,  relationwd.SourceFlowId, relationwd.SourceTbId, relationwd.FlowPrcs, relationwd.WriteTbId,relationwd.RelationWDFlag, relationwd.OrderNo, relationwd.RelationWDBy, tbrelation.TbID,tbrelation.FlowScope, relationwd.WhereStr, relationwd.NumberType, relationwd.RelationSort,relationlist.DbID FROM  relationlist INNER JOIN relationwd ON relationlist.RelationId = relationwd.RelationId INNER JOIN tbrelation ON relationlist.RelationId = tbrelation.RelationId  where tbrelation.isUse='1' order by RelationId";

            DataTable dtVIEW_TbRelationWD = comService.GetDataTable(sql);
            model.dtVIEW_TbRelationWD = dtVIEW_TbRelationWD;

            sql = @"select * from relationwdfield";
            DataTable dtRelationWDField = comService.GetDataTable(sql);
            model.dtRelationWDField = dtRelationWDField;

            sql = @"select * from externaldb";
            DataTable dtExternalDb = comService.GetDataTable(sql);
            model.dtExternalDb = dtExternalDb;

            return model;
        }

        public static TableDtModel GetTableDtModel()
        {
            TableDtModel model = new TableDtModel();

            ComService comService = new ComService();

            string sql = "select * from tbtag order by TbId,OrderNo";
            DataTable dtTbTag = comService.GetDataTable(sql);
            model.dtTbTag = dtTbTag;

            sql = "select * from tbbutcustomize order by TbId,GridId";
            DataTable dtTbButCustomize = comService.GetDataTable(sql);
            model.dtTbButCustomize = dtTbButCustomize;

            sql = "select * from tbbutcustomizeevent order by BtnId,OrderNo";
            DataTable dtTbButCustomizeEvent = comService.GetDataTable(sql);
            model.dtTbButCustomizeEvent = dtTbButCustomizeEvent;

            sql = "select * from tbeventcustomize order by TbId,OrderNo";
            DataTable dtTbEventCustomize = comService.GetDataTable(sql);
            model.dtTbEventCustomize = dtTbEventCustomize;

            sql = "select * from tbeventpara order by TbId,Id";
            DataTable dtTbEventPara = comService.GetDataTable(sql);
            model.dtTbEventPara = dtTbEventPara;

            sql = "select * from tbindex order by TbId,IndexOrderNo";
            DataTable _dtindex = comService.GetDataTable(sql);
            model.dtIndex = _dtindex;

            sql = "select * from rulelist";
            model.dtRuleList = comService.GetDataTable(sql);

            sql = "select * from tbbasic ";
            DataTable allTB = comService.GetDataTable(sql);
            model.allTB = allTB;

            sql = "select * from tbbasic2 ";
            DataTable allTB2 = comService.GetDataTable(sql);
            model.allTB2 = allTB2;

            sql = "select * from rulesingle";
            DataTable dtrulesingle = comService.GetDataTable(sql);
            model.dtRuleSingle = dtrulesingle;

            sql = "select * from rulemultiple";
            DataTable dtrulemultiple = comService.GetDataTable(sql);
            model.dtRuleMultiple = dtrulemultiple;

            sql = "select * from rulemultiplefiled order by RuleId,FiledOrder";
            DataTable dtrulemultiplefiled = comService.GetDataTable(sql);
            model.dtRuleMultipleFiled = dtrulemultiplefiled;

            sql = "select * from ruletree";
            DataTable dtruletree = comService.GetDataTable(sql);
            model.dtRuleTree = dtruletree;

            sql = "select * from flow";
            DataTable dtflow = comService.GetDataTable(sql);
            model.dtFlow = dtflow;

            sql = "select * from flowprcs order by FlowId,PrcsNo";
            DataTable dtflowprcs = comService.GetDataTable(sql);
            model.dtFlowPrcs = dtflowprcs;

            sql = "select * from tbbut order by TbId";
            DataTable dtTbBut = comService.GetDataTable(sql);
            model.dtTbBut = dtTbBut;

            sql = "SELECT tbrelation.*,relationlist.* FROM  tbrelation INNER JOIN relationlist ON tbrelation.RelationId = relationlist.RelationId  ";
            DataTable dtVIEW_TbRelationRD = comService.GetDataTable(sql);
            model.dtVIEW_TbRelationRD = dtVIEW_TbRelationRD;

            sql = "select * from relationrd order by RelationId";
            DataTable dtRelationRD = comService.GetDataTable(sql);
            model.dtRelationRD = dtRelationRD;

            sql = "select * from relationrdfield order by ID, FillIndexId";//order by ID很重要--对数据联动
            DataTable dtRelationRDField = comService.GetDataTable(sql);
            model.dtRelationRDField = dtRelationRDField;

            sql = @"SELECT relationlist.RelationId, relationlist.RelationType, relationlist.RelationName, relationlist.RelationBy, relationlist.RelationFlag, relationlist.FlowStr, relationlist.ICount,  relationwd.SourceFlowId, relationwd.SourceTbId, relationwd.FlowPrcs, relationwd.WriteTbId,relationwd.RelationWDFlag, relationwd.OrderNo, relationwd.RelationWDBy, tbrelation.TbID,tbrelation.FlowScope, relationwd.WhereStr, relationwd.NumberType, relationwd.RelationSort,relationlist.DbID FROM  relationlist INNER JOIN relationwd ON relationlist.RelationId = relationwd.RelationId INNER JOIN tbrelation ON relationlist.RelationId = tbrelation.RelationId  where tbrelation.isUse='1' order by RelationId";
            DataTable dtVIEW_TbRelationWD = comService.GetDataTable(sql);
            model.dtVIEW_TbRelationWD = dtVIEW_TbRelationWD;

            sql = @"select * from relationwdfield";
            DataTable dtRelationWDField = comService.GetDataTable(sql);
            model.dtRelationWDField = dtRelationWDField;

            sql = @"select * from externaldb";
            DataTable dtExternalDb = comService.GetDataTable(sql);
            model.dtExternalDb = dtExternalDb;

            return model;
        }

        public static AppSetEntity GetSetModel(string siteRoot, string dbType, string directoryType = "1")
        {
            string templetFilePath = "";

            AppSetEntity model = new AppSetEntity();

            model.CopyCodeStr = Think9.Services.Base.Configs.GetValue("CopyCodeStr");
            model.URL = Think9.Services.Base.Configs.GetValue("YoursLCSA");
            model.AppId = Think9.Services.Base.Configs.GetValue("YoursLCAppId");
            model.PassWord = Think9.Services.Base.Configs.GetValue("YoursLCPassWord");

            model.DbType = dbType;
            model.DirectoryType = directoryType;//目录类别--下载代码时才起作用 1表示Controllers、Services作为根目录 2表示以录入表编码作为根目录

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\BuildCode.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.BuildCode = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ControllerCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ControllerCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ControllerGridCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ControllerGridCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ServiceGridCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ServiceGridCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ControllerPUCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ControllerPUCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\GridServiceCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.GridServiceCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ServiceCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ServiceCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            //Layui
            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\DetailCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.DetailCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\MobileDetailCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.MobileDetailCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\FormCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.FormCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\MobileFormCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.MobileFormCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\GridColumnCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.GridColumnCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\FormGrid2Cshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.FormGrid2Cshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\FormGridCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.FormGridCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\MobileFormGridCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.MobileFormGridCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\FormNoGridCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.FormNoGridCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\ListCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ListCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\MobileListCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.MobileListCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\PuCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.PuCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\PuCshtmlDict.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.PuCshtmlDict = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\PuCshtmlTree.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.PuCshtmlTree = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\SelfJS.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.SelfJS = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Rdlc\\TableRdlc.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.TableRdlc = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Rdlc\\ListRdlc.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ListRdlc = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Rdlc\\ReportRdlc.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportRdlc = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Rdlc\\ReportRdlc2.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportRdlc2 = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ReportControllerCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportControllerCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ReportServiceCs.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportServiceCs = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\ReportCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\ReportShowCshtml.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportShowCshtml = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ReportControllerCs2.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportControllerCs2 = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Dapper\\ReportServiceCs2.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportServiceCs2 = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\ReportCshtml2.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportCshtml2 = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            templetFilePath = siteRoot + "\\wwwroot\\AppTempletFile\\Layui\\ReportShowCshtml2.txt";
            if (System.IO.File.Exists(templetFilePath))
            {
                model.ReportShowCshtml2 = Think9.Util.Helper.FileHelper.FileToString(templetFilePath);
            }

            return model;
        }
    }
}
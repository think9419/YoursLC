using System.Collections.Generic;
using System.Data;
using System.IO;
using Think9.Models;
using Think9.Services.Base;

namespace Think9.Services.Com
{
    public class AttachmentService : BaseService<AttachmentEntity>
    {
        private static string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\");
        private static ComService comService = new ComService();

        public static void DealWithAttachments(long listid, string fwid, string attachmentId)
        {
            UpdateAttachmentId(listid, fwid, attachmentId);//listid=0时添加了附件
            MoveFile(listid, fwid);//将listid=0时添加的附件转移
        }

        private static void UpdateAttachmentId(long listid, string fwid, string attachmentId)
        {
            if (!string.IsNullOrEmpty(attachmentId))
            {
                //ComService comService = new ComService();
                if (fwid.StartsWith("bi_"))
                {
                    comService.ExecuteSql("update " + fwid.Replace("bi_", "tb_") + " set attachmentId='" + attachmentId + "'   WHERE listid= " + listid);
                }
                else
                {
                    comService.ExecuteSql("update flowrunlist set attachmentId='" + attachmentId + "'   WHERE listid= " + listid);
                }

                //修改ListId=0时上传附件的记录
                comService.ExecuteSql("update recordrun set ListId=" + listid + "  WHERE listid = 0 and RecordFlag = '" + attachmentId + "' and FlowId = '" + fwid + "' ");
            }

            MoveFile(listid, fwid);//将listid=0时添加的附件转移
        }

        //将listid=0时添加的附件转移
        private static void MoveFile(long listid, string fwid)
        {
            List<string> list = new List<string>();

            string oldDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "_Files\\0\\");
            string newDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "_Files\\" + listid + "\\");
            string str = "";

            DataTable dtIndex = comService.GetDataTable("select IndexId as id from tbindex where TbId='" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' and DataType='4100' ");
            if (dtIndex.Rows.Count > 0)
            {
                DataTable dt = comService.GetDataTable("select * from " + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + " where listid=" + listid + " ");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtIndex.Rows)
                    {
                        str = dt.Rows[0][dr["id"].ToString()].ToString().Trim();
                        if (str != "" && Think9.Util.Helper.FileHelper.IsExistFile(oldDirectory + str))
                        {
                            list.Add(str);
                        }
                    }
                }
            }

            foreach (DataRow drGrid in comService.GetDataTable("select TbId as id from tbbasic where ParentId='" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' ").Rows)
            {
                string grid = drGrid["id"].ToString();

                dtIndex = comService.GetDataTable("select IndexId as id from tbindex where TbId='" + grid + "' and DataType='4100' ");
                if (dtIndex.Rows.Count > 0)
                {
                    DataTable dt = comService.GetDataTable("select * from " + grid + " where listid=" + listid + " ");
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow drIndex in dtIndex.Rows)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                str = dr[drIndex["id"].ToString()].ToString().Trim();
                                if (str != "" && Think9.Util.Helper.FileHelper.IsExistFile(oldDirectory + str))
                                {
                                    list.Add(str);
                                }
                            }
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                Think9.Util.Helper.FileHelper.CreateSuffic(newDirectory);
                foreach (string item in list)
                {
                    System.IO.File.Move(oldDirectory + item, newDirectory + item);
                }
            }
        }

        public static void DelAttachment(long listid, string fwid)
        {
            //数据删除时不删除附件
            string isDel = "|" + Think9.Services.Base.Configs.GetValue("DelNoFile").Trim() + "|";
            if (isDel.Contains("|" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "|"))
            {
                return;
            }

            string attId = "";
            if (fwid.StartsWith("bi_"))
            {
                attId = comService.GetSingleField("select attachmentId  FROM " + fwid.Replace("bi_", "tb_") + " WHERE listid= " + listid);
            }
            else
            {
                attId = comService.GetSingleField("select attachmentId  FROM flowrunlist WHERE listid= " + listid);
            }

            if (!string.IsNullOrEmpty(attId))
            {
                comService.ExecuteSql("delete from flowattachment where attachmentId = '" + attId + "'");

                Think9.Util.Helper.FileHelper.DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + attId));
            }

            //删除附件指标上传的附件
            Think9.Util.Helper.FileHelper.DeleteDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "_Files\\" + listid + "\\"));
        }

        public static void DelGridAttachment(long listid, long id, string fwid, string grid)
        {
            //数据删除时不删除附件
            string isDel = "|" + Think9.Services.Base.Configs.GetValue("DelNoFile").Trim() + "|";
            if (isDel.Contains("|" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "|") || isDel.Contains("|" + grid + "|"))
            {
                return;
            }

            DataTable dtIndex = comService.GetDataTable("select IndexId as id from tbindex where TbId='" + grid + "' and DataType='4100' ");
            if (dtIndex.Rows.Count > 0)
            {
                DataTable dt = comService.GetDataTable("select * from " + grid + " where id=" + id + " ");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtIndex.Rows)
                    {
                        string str = dt.Rows[0][dr["id"].ToString()].ToString().Trim();
                        if (str != "")
                        {
                            //删除附件指标上传的附件
                            Think9.Util.Helper.FileHelper.DeleteFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserFile\\" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "_Files\\" + listid + "\\" + str));
                        }
                    }
                }
            }
        }

        public static void DelImg(long listid, string fwid)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserImg\\");
            string tbid = fwid.Replace("bi_", "tb_").Replace("fw_", "tb_");
            string str = "";

            //数据删除时不删除图片
            string isDel = "|" + Think9.Services.Base.Configs.GetValue("DelNoImg").Trim() + "|";
            if (isDel.Contains("|" + tbid + "|"))
            {
                return;
            }

            DataTable dtIndex = comService.GetDataTable("select IndexId as id from tbindex where TbId='" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' and DataType='5200' ");
            if (dtIndex.Rows.Count > 0)
            {
                DataTable dt = comService.GetDataTable("select * from " + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + " where listid=" + listid + " ");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtIndex.Rows)
                    {
                        str = dt.Rows[0][dr["id"].ToString()].ToString().Trim();
                        if (str != "")
                        {
                            Think9.Util.Helper.FileHelper.DeleteFile(directory + str);
                            Think9.Util.Helper.FileHelper.DeleteFile(directory + "_" + str);
                        }
                    }
                }
            }

            foreach (DataRow drGrid in comService.GetDataTable("select TbId as id from tbbasic where ParentId='" + fwid.Replace("bi_", "tb_").Replace("fw_", "tb_") + "' ").Rows)
            {
                string grid = drGrid["id"].ToString();
                dtIndex = comService.GetDataTable("select IndexId as id from tbindex where TbId='" + grid + "' and DataType='5200' ");
                if (dtIndex.Rows.Count > 0)
                {
                    DataTable dt = comService.GetDataTable("select * from " + grid + " where listid=" + listid + " ");
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow drIndex in dtIndex.Rows)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                str = dr[drIndex["id"].ToString()].ToString().Trim();
                                if (str != "")
                                {
                                    Think9.Util.Helper.FileHelper.DeleteFile(directory + str);
                                    Think9.Util.Helper.FileHelper.DeleteFile(directory + "_" + str);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DelGridImg(long id, string grid)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\UserImg\\");
            string str = "";

            //数据删除时不删除图片
            string isDel = "|" + Think9.Services.Base.Configs.GetValue("DelNoImg").Trim() + "|";
            if (isDel.Contains("|" + grid + "|"))
            {
                return;
            }

            DataTable dtIndex = comService.GetDataTable("select IndexId as id from tbindex where TbId='" + grid + "' and DataType='5200' ");
            if (dtIndex.Rows.Count > 0)
            {
                DataTable dt = comService.GetDataTable("select * from " + grid + " where id=" + id + " ");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtIndex.Rows)
                    {
                        str = dt.Rows[0][dr["id"].ToString()].ToString().Trim();
                        if (str != "")
                        {
                            Think9.Util.Helper.FileHelper.DeleteFile(directory + str);
                            Think9.Util.Helper.FileHelper.DeleteFile(directory + "_" + str);
                        }
                    }
                }
            }
        }

        public static void GetAttachmentId(string listid, string fwid)
        {
            string attid = "";
            if (fwid.StartsWith("bi_"))
            {
                if (listid != "0")
                {
                    attid = comService.GetSingleField("select attachmentId  FROM " + fwid.Replace("bi_", "tb_") + " WHERE listid= " + listid);
                }
            }
            else
            {
                if (listid != "0")
                {
                    attid = comService.GetSingleField("select attachmentId  FROM flowrunlist WHERE listid= " + listid);
                }
            }
        }

        /// <summary>
        ///流程步骤对附件的操作权限
        /// </summary>
        /// <param name="fwid"></param>
        /// <param name="prcid"></param>
        /// <param name="A1">新建 1有权限2无</param>
        /// <param name="A2">下载 1有权限2无</param>
        /// <param name="A3">删除 1有权限2无</param>
        public void GetFileAuthority(string fwid, string prcid, ref string A1, ref string A2, ref string A3)
        {
            if (fwid.StartsWith("bi_") || fwid == "_notify")
            {
                A1 = "1";
                A2 = "1";
                A3 = "1";
            }
            else
            {
                ServiceFlow flow = new ServiceFlow();
                FlowEntity mflow = flow.GetByWhereFirst("where FlowId=@FlowId ", new { FlowId = fwid });
                if (mflow == null)
                {
                    A1 = "2";
                    A2 = "2";
                    A3 = "2";
                }
                else
                {
                    if (mflow.flowType == "2")
                    {
                        A1 = "1";
                        A2 = "1";
                        A3 = "1";
                    }
                    else
                    {
                        ServiceFlowPrcs FlowPrcsService = new ServiceFlowPrcs();
                        FlowPrcsEntity model = FlowPrcsService.GetByWhereFirst("where PrcsId=@id", new { id = prcid });
                        if (model == null)
                        {
                            A1 = "2";
                            A2 = "2";
                            A3 = "2";
                        }
                        else
                        {
                            if (model.BAttachment.Length >= 3)
                            {
                                A1 = model.BAttachment.Substring(0, 1);
                                A2 = model.BAttachment.Substring(1, 1);
                                A3 = model.BAttachment.Substring(2, 1);
                            }
                            else
                            {
                                A1 = "2";
                                A2 = "2";
                                A3 = "2";
                            }
                        }
                    }
                }
            }
        }
    }
}
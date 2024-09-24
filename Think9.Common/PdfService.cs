using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using Think9.Models;

namespace Think9.Services.Com
{
    public class PdfService
    {
        public void MergeExport(string idsStr)
        {
        }

        public static RdlcDeviceEntity GetPdfDevice(string tbid)
        {
            RdlcDeviceEntity device = new RdlcDeviceEntity();

            TbPdfSize tbPdfSize = new TbPdfSize();
            TbPdfSizeEntity model = tbPdfSize.GetByWhereFirst("where TbId = '" + tbid + "' and  Type='formpdf'");
            if (model != null)
            {
                device.Width = model.Width;
                device.Heigh = model.Heigh;
                device.Top = model.Top;
                device.Left = model.Left;
                device.Right = model.Right;
                device.Bottom = model.Bottom;
            }

            device.PathUserImg = Think9.Services.Base.BaseConfig.GetUserImgPath();//用户图片所在文件夹
            device.ImgNoExist = Think9.Services.Base.BaseConfig.GetImgNoExistPath();//图片不存在时的替代

            return device;
        }

        public static RdlcDeviceEntity GetPdfDevice()
        {
            RdlcDeviceEntity device = new RdlcDeviceEntity();

            device.PathUserImg = Think9.Services.Base.BaseConfig.GetUserImgPath();//用户图片所在文件夹
            device.ImgNoExist = Think9.Services.Base.BaseConfig.GetImgNoExistPath();//图片不存在时的替代

            return device;
        }

        public static void MergePDF2(string sourcePath, string targetPath, string newFileName)
        {
            //todo 文档尺寸问题还没解决，总按A4大小输出
            //需要合并的pdf集合
            string[] fileList = Directory.GetFiles(sourcePath, "*.pdf", SearchOption.AllDirectories);
            //合并到的总PDF
            string outMergeFile = targetPath + "\\" + newFileName;
            iTextSharp.text.pdf.PdfReader reader;
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;

            for (int i = 0; i < fileList.Length; i++)
            {
                reader = new PdfReader(fileList[i]);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
        }

        public static void MergePDF1(string sourcePath, string targetPath, string newFileName)
        {
            //todo 文档尺寸问题还没解决，总按A4大小输出
            //需要合并的pdf集合
            string[] fileList = Directory.GetFiles(sourcePath, "*.pdf", SearchOption.AllDirectories);

            //合并到的总PDF
            string outMergeFile = targetPath + "\\" + newFileName;

            iTextSharp.text.pdf.PdfReader reader;

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            document.Open();

            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;

            for (int i = 0; i < fileList.Length; i++)
            {
                reader = new PdfReader(fileList[i]);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
        }

        public static void MergePDF(string sourcePath, string targetPath, string newFileName)
        {
            //todo 文档尺寸问题还没解决，总按A4大小输出
            //需要合并的pdf集合
            string[] fileList = Directory.GetFiles(sourcePath, "*.pdf", SearchOption.AllDirectories);

            //合并到的总PDF
            string outMergeFile = targetPath + "\\" + newFileName;

            MergePdfFiles(fileList, outMergeFile);
        }

        /// <summary>
        /// 合成pdf文件
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="outMergeFile"></param>
        public static void MergePdfFiles(string[] fileList, string outMergeFile)
        {
            PdfReader reader;

            //此处将内容从文本提取至文件流中的目的是避免文件被占用,无法删除
            FileStream fsFist = new FileStream(fileList[0], FileMode.Open);
            byte[] bytes1 = new byte[(int)fsFist.Length];
            fsFist.Read(bytes1, 0, bytes1.Length);
            fsFist.Close();
            reader = new PdfReader(bytes1);
            reader.GetPageSize(1);
            // iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(1000,800);//设置样式
            iTextSharp.text.Rectangle rec = reader.GetPageSize(1);
            float width = rec.Width;
            float height = rec.Height;
            float marginLeft = 50;
            float marginRight = 50;
            float marginTop = 50;
            float marginBottom = 50;
            //创建一个文档变量
            iTextSharp.text.Document document = new iTextSharp.text.Document(rec, marginLeft, marginRight, marginTop, marginBottom);
            //创建该文档
            PdfWriter pdfWrite = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            //打开文档
            document.Open();
            //添加内容
            PdfContentByte contentByte = pdfWrite.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Length; i++)
            {
                //File.Delete(fileList[i]);
                FileStream fs = new FileStream(fileList[i], FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                reader = new PdfReader(bytes);
                int pageNum = reader.NumberOfPages;//获取文档页数
                for (int j = 1; j <= pageNum; j++)
                {
                    document.NewPage();
                    newPage = pdfWrite.GetImportedPage(reader, j);
                    contentByte.AddTemplate(newPage, 0, 0);
                }
                File.Delete(fileList[i]);
            }
            document.Close();
        }

        /// <summary>
        /// 合成pdf文件
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="outMergeFile"></param>
        public static void MergePdfFiles(List<string> fileList, string outMergeFile)
        {
            PdfReader reader;

            //此处将内容从文本提取至文件流中的目的是避免文件被占用,无法删除
            FileStream fsFist = new FileStream(fileList[0], FileMode.Open);
            byte[] bytes1 = new byte[(int)fsFist.Length];
            fsFist.Read(bytes1, 0, bytes1.Length);
            fsFist.Close();
            reader = new PdfReader(bytes1);
            reader.GetPageSize(1);
            // iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(1000,800);//设置样式
            iTextSharp.text.Rectangle rec = reader.GetPageSize(1);
            float width = rec.Width;
            float height = rec.Height;
            //创建一个文档变量
            iTextSharp.text.Document document = new iTextSharp.text.Document(rec, 50, 50, 50, 50);
            //创建该文档
            PdfWriter pdfWrite = PdfWriter.GetInstance(document, new FileStream(outMergeFile, FileMode.Create));
            //打开文档
            document.Open();
            //添加内容
            PdfContentByte contentByte = pdfWrite.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Count; i++)
            {
                //File.Delete(fileList[i]);
                FileStream fs = new FileStream(fileList[i], FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                reader = new PdfReader(bytes);
                int pageNum = reader.NumberOfPages;//获取文档页数
                for (int j = 1; j <= pageNum; j++)
                {
                    document.NewPage();
                    newPage = pdfWrite.GetImportedPage(reader, j);
                    contentByte.AddTemplate(newPage, 0, 0);
                }
                File.Delete(fileList[i]);
            }
            document.Close();
        }

        /// <summary>
        /// 删除一个文件夹里的所有文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteAllPdf(string DirectoryPath)
        {
            System.IO.DirectoryInfo dic = new System.IO.DirectoryInfo(DirectoryPath);
            if (dic.Exists)
            {
                FileInfo[] fInfo = dic.GetFiles("*.pdf");
                foreach (FileInfo temp in fInfo)
                {
                    File.Delete(DirectoryPath + "\\" + temp.Name);
                }
            }
        }
    }
}
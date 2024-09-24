using Microsoft.AspNetCore.Mvc;

namespace Think9.Controllers.Basic
{
    [Area("Com")]
    public class RDLCPrintController
    {
        //private Size rdlcPageSize = new Size();

        //public LocalReport LoadFile(string rdlcfile, string sourceName, DataTable sourceTable)
        //{
        //    LocalReport report = new LocalReport();
        //    //设置需要打印的报表的文件名称。
        //    report.ReportPath = rdlcfile;
        //    //创建要打印的数据源
        //    ReportDataSource source = new ReportDataSource(sourceName, sourceTable);
        //    //report.SetParameters(
        //    //    new ReportParameter[] {
        //    //        new ReportParameter("TimeNow", DateTime.Now.ToString()),
        //    //        new ReportParameter("ReportTitle", PubConstant.REPORTTITLE)
        //    //    }
        //    //);
        //    m_streams = new List<Stream>();

        //    report.DataSources.Add(source);

        //    //刷新报表中的需要呈现的数据
        //    report.Refresh();
        //    rdlcPageSize.Width = report.GetDefaultPageSettings().PaperSize.Width;
        //    rdlcPageSize.Height = report.GetDefaultPageSettings().PaperSize.Height;
        //    //OutputFormat 如果字体太大，可以用PDF格式，首选EMF格式
        //    //一等分一整张28cm,宽度是固定的:21.4cm;二等分高度是:13.9cm,三等分的话高度是9.3cm。
        //    //纸宽指标反映出打印机最大打印宽度,通用打印机的该项指标一般为9英寸和13.6英寸;
        //    //纸厚度则反映出打印头的击打能力,这项指标对于需要复写拷贝的用途很重要。
        //    string deviceInfo =
        //                      "<DeviceInfo>" +
        //                      "<OutputFormat>EMF</OutputFormat>" +
        //                      "<PageWidth>21cm</PageWidth>" +
        //                      "<PageHeight>13.9cm</PageHeight>" +
        //                      "<MarginTop>0cm</MarginTop>" +
        //                      "<MarginLeft>0cm</MarginLeft>" +
        //                      "<MarginRight>0cm</MarginRight>" +
        //                      "<MarginBottom>0cm</MarginBottom>" +
        //                      "</DeviceInfo>";
        //    Warning[] warnings;
        //    //将报表的内容按照deviceInfo指定的格式输出到CreateStream函数提供的Stream中。
        //    report.Render("Image", deviceInfo, CreateStream, out warnings);

        //    return report;
        //}

        ////声明一个Stream对象的列表用来保存报表的输出数据
        ////LocalReport对象的Render方法会将报表按页输出为多个Stream对象。
        //private List<Stream> m_streams = null;

        ////用来提供Stream对象的函数，用于LocalReport对象的Render方法的第三个参数。
        //private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        //{
        //    //如果需要将报表输出的数据保存为文件，请使用FileStream对象。
        //    Stream stream = new MemoryStream();
        //    m_streams.Add(stream);
        //    return stream;
        //}

        ////用来记录当前打印到第几页了
        //private int m_currentPageIndex = 0;

        //public void Print(string printername, Size size)
        //{
        //    //size = rdlcPageSize;

        //    m_currentPageIndex = 0;
        //    if (m_streams == null || m_streams.Count == 0) return;
        //    //声明PrintDocument对象用于数据的打印
        //    PrintDocument printDoc = new PrintDocument();
        //    //指定需要使用的打印机的名称，使用空字符串""来指定默认打印机
        //    printDoc.PrinterSettings.PrinterName = printername;

        //    //printDoc.PrinterSettings.PrinterName = "Microsoft XPS Document Writer";
        //    //判断指定的打印机是否可用
        //    if (!printDoc.PrinterSettings.IsValid)
        //    {
        //        //MessageBox.Show("未发现打印机 " + printDoc.PrinterSettings.PrinterName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    //声明PrintDocument对象的PrintPage事件，具体的打印操作需要在这个事件中处理。
        //    //指定是否横向打印
        //    printDoc.DefaultPageSettings.Landscape = false;
        //    //827 552
        //    printDoc.DefaultPageSettings.PaperSize = new PaperSize("Custom Size 1", size.Width, size.Height);
        //    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
        //    //执行打印操作，Print方法将触发PrintPage事件。

        //    //将写好的格式给打印预览控件以便预览

        //    //PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
        //    //printPreviewDialog1.Document = printDoc;
        //    //显示打印预览
        //    //DialogResult result = printPreviewDialog1.ShowDialog();

        //    printDoc.Print();

        //    //释放资源
        //    foreach (Stream stream in m_streams)
        //    {
        //        stream.Dispose();
        //        stream.Close();
        //    }
        //    m_streams = null;
        //}

        //private void PrintPage(object sender, PrintPageEventArgs ev)
        //{
        //    //Metafile对象用来保存EMF或WMF格式的图形，
        //    //我们在前面将报表的内容输出为EMF图形格式的数据流。
        //    m_streams[0].Position = 0;
        //    Metafile pageImage = new Metafile(m_streams[0]);

        //    //这里的Graphics对象实际指向了打印机
        //    //这几个参数很重要，否则会出现打印很大的现象
        //    int w = Convert.ToInt32(ev.PageBounds.Width / 1.8);
        //    int h = Convert.ToInt32(ev.PageBounds.Height / 1.8);
        //    ev.Graphics.DrawImage(pageImage, ev.PageBounds, 0, 0, w, h, System.Drawing.GraphicsUnit.Millimeter);

        //    m_streams[m_currentPageIndex].Close();
        //    m_currentPageIndex++;
        //    //设置是否需要继续打印
        //    ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        //}
    }
}
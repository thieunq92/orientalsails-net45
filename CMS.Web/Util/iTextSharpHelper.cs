using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CMS.Web.Util
{
    public class iTextSharpHelper
    {
        public delegate void PdfWriteContentHandler(Document document);

        public static void ExportToPdf(PdfWriteContentHandler writerHandler, HttpResponse response)
        {
            response.Clear();

            // Mở một memory stream để khắc phục với IE (không truyền trực tiếp được)
            MemoryStream m = new MemoryStream();
            Document document = new Document();

            // Kiểu response sẽ là kiểu pdf
            response.ContentType = "application/pdf";
            PdfWriter writer = PdfWriter.GetInstance(document, m);
            writer.CloseStream = false;

            // Việc mở file để bắt đầu ghi nội dung phải bắt đầu trong writerHandler
            //document.Open();

            // Nội dung file pdf
            writerHandler(document);

            // Kết thúc việc ghi nội dung ra file pdf
            document.Close();

            // Ghi nội dung file pdf vào stream gửi về client
            response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            response.OutputStream.Flush();
            response.OutputStream.Close();
            m.Close();
            response.End();
        }

        public static PdfPCell NewPdfCell(string str, Font font, PdfPCell style)
        {
            PdfPCell cell = new PdfPCell(new Phrase(str, font));
            cell.HorizontalAlignment = style.HorizontalAlignment;
            cell.Border = style.Border;
            cell.PaddingLeft = style.PaddingLeft;
            cell.PaddingRight = style.PaddingRight;
            cell.PaddingTop = style.PaddingTop;
            cell.PaddingBottom = style.PaddingBottom;
            return cell;
        }
    }
}

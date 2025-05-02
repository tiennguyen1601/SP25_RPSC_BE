using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP25_RPSC.Data.Models.LContractModel.Request;
using SixLabors.ImageSharp.Processing;

namespace SP25_RPSC.Services.Utils.PdfGenerator
{
    public class PdfGenerator
    {
        public static async Task<PdfDocument?> GenerateContractPdf(LContractRequestDTO request, HttpClient client)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(gfx);
            var fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
            var fontContent = new XFont("Arial", 12, XFontStyle.Regular);
            var fontBold = new XFont("Arial", 12, XFontStyle.Bold);
            var pageHeight = page.Height;
            var lineHeight = 5;
            double marginLeft = 50;
            double marginRight = page.Width - 50;
            double currentY = 40; // Starting point

            void AddNewPage()
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                tf = new XTextFormatter(gfx);
                currentY = 40;
            }

            void DrawCenteredText(string text, XFont font, double y)
            {
                XSize textSize = gfx.MeasureString(text, font);
                double xCenter = (page.Width - textSize.Width) / 2;
                tf.DrawString(text, font, XBrushes.Black, new XRect(xCenter, y, textSize.Width, textSize.Height), XStringFormats.TopLeft);
            }

            void DrawParagraph(string text, XFont font, double lineSpacing)
            {
                string[] lines = text.Split('\n');
                foreach (string line in lines)
                {
                    XSize textSize = gfx.MeasureString(line, font);
                    tf.DrawString(line, font, XBrushes.Black, new XRect(marginLeft, currentY, marginRight - marginLeft, textSize.Height + 10), XStringFormats.TopLeft);
                    currentY += textSize.Height + lineSpacing;
                }
            }

            // Title
            DrawCenteredText("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM", fontTitle, currentY);
            currentY += 25;
            DrawCenteredText("Độc lập - Tự do - Hạnh phúc", fontTitle, currentY);
            currentY += 25;
            DrawCenteredText("----------o0o-----------", fontTitle, currentY);
            currentY += 40;

            // Contract title
            DrawCenteredText("HỢP ĐỒNG CUNG CẤP DỊCH VỤ", new XFont("Arial", 14, XFontStyle.Bold), currentY);
            currentY += 30;

            // Date
            tf.DrawString($"Hôm nay, ngày {request.SignedDate:dd/MM/yyyy}, chúng tôi gồm:", fontContent, XBrushes.Black, new XRect(marginLeft, currentY, marginRight - marginLeft, 20), XStringFormats.TopLeft);
            currentY += 25;

            // Party A (Bên Cung Cấp Dịch Vụ)
            string partyAInfo = @$"BÊN A (BÊN CUNG CẤP DỊCH VỤ):
Công ty: EasyRoomie
Địa chỉ: Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Hồ Chí Minh
Đại diện: Nguyễn Trần Vĩ Đức
Chức vụ: Giám đốc Công ty
Số điện thoại: 0385928575
Email: easyroomie.RPSC@gmail.com";
            DrawParagraph(partyAInfo, fontContent, lineHeight);
            currentY += 10;

            // Party B (Bên Sử Dụng Dịch Vụ)
            string partyBInfo = @$"BÊN B (BÊN SỬ DỤNG DỊCH VỤ - LANDLORD):
Công ty: {request.CompanyName}
Họ và tên: {request.LandlordName}
Địa chỉ: {request.LandlordAddress}
Số điện thoại: {request.LandlordPhone}
Chức vụ: Chủ trọ";
            DrawParagraph(partyBInfo, fontContent, lineHeight);
            currentY += 15;

            // Contract Details
            DrawCenteredText("Điều 1: Nội dung hợp đồng", fontBold, currentY);
            currentY += 20;
            string section1 = @$"Bên A đồng ý cung cấp cho Bên B gói dịch vụ {request.PackageName} sử dụng trong {request.ServiceName}.
Gói dịch vụ bao gồm:
- Quyền đăng tải {request.MaxPost} bài đăng
- Hiển thị nhãn ưu tiên: {request.Label} 
- Bài đăng được ưu tiên hiển thị đầu trang trong {request.PriorityTime} giờ.";
            DrawParagraph(section1, fontContent, lineHeight);
            currentY += 15;

            // Duration
            DrawCenteredText("Điều 2: Thời hạn hợp đồng", fontBold, currentY);
            currentY += 20;
            string section2 = @$"Hợp đồng có hiệu lực từ ngày {request.StartDate:dd/MM/yyyy} đến ngày {request.StartDate.AddDays(request.Duration):dd/MM/yyyy}.";
            DrawParagraph(section2, fontContent, lineHeight);
            currentY += 15;

            // next page
            AddNewPage();

            // Payment and Terms
            DrawCenteredText("Điều 3: Phí dịch vụ và phương thức thanh toán", fontBold, currentY);
            currentY += 20;
            string section3 = @$"Phí dịch vụ: {request.Price.ToString("N0", new CultureInfo("vi-VN"))} VNĐ/tháng.
Phương thức thanh toán: CHUYỂN KHOẢN.";
            DrawParagraph(section3, fontContent, lineHeight);
            currentY += 15;

            // Rights and Responsibilities
            DrawCenteredText("Điều 4: Quyền và nghĩa vụ của các bên", fontBold, currentY);
            currentY += 20;
            string section4 = @$"Bên A:
- Cung cấp dịch vụ đúng nội dung đã cam kết.
- Đảm bảo hệ thống hoạt động ổn định.
- Hỗ trợ kỹ thuật trong quá trình sử dụng dịch vụ.";
            DrawParagraph(section4, fontContent, lineHeight);
            currentY += 15;
            string section5 = @$"Bên B:
- Thanh toán đầy đủ và đúng hạn.
- Sử dụng dịch vụ đúng mục đích, không vi phạm pháp luật.
- Chịu trách nhiệm về nội dung đăng tải trên hệ thống.";
            DrawParagraph(section5, fontContent, lineHeight);
            currentY += 15;

            // Termination Clause
            DrawCenteredText("Điều 5: Chấm dứt hợp đồng", fontBold, currentY);
            currentY += 20;
            string section6 = @$"Hợp đồng có thể chấm dứt trong các trường hợp:
- Hết thời hạn hợp đồng mà không gia hạn.
- Một trong hai bên vi phạm nghiêm trọng nghĩa vụ hợp đồng.";
            DrawParagraph(section6, fontContent, lineHeight);
            currentY += 15;

            // Dispute Resolution
            DrawCenteredText("Điều 6: Giải quyết tranh chấp", fontBold, currentY);
            currentY += 20;
            string section7 = @$"Mọi tranh chấp phát sinh sẽ được giải quyết thông qua thương lượng. 
Nếu không thành, sẽ đưa ra tòa án có thẩm quyền.";
            DrawParagraph(section7, fontContent, lineHeight);
            currentY += 15;

            // Execution Clause
            DrawCenteredText("Điều 7: Điều khoản thi hành", fontBold, currentY);
            currentY += 20;
            string section8 = @$"Hợp đồng có hiệu lực từ ngày ký và được lập thành hai (02) bản, 
mỗi bên giữ một (01) bản có giá trị pháp lý như nhau.";
            DrawParagraph(section8, fontContent, lineHeight);
            currentY += 20;

            // Signing Section
            var signTitlePos = currentY;
            var signImgPos = signTitlePos + 30;
            var signFullNamePos = signImgPos + 75;

            // Upload signatures
            try
            {
                var signatureCusBytes = await client.GetByteArrayAsync("https://res.cloudinary.com/dzoxs1sd7/image/upload/v1745523686/easyroomie-sign.png");

                // read image
                using (var ms = new MemoryStream(signatureCusBytes))
                {
                    // ensure Load method does not occur error
                    using (var image = Image.Load<Rgba32>(ms))
                    {
                        // Resize image
                        image.Mutate(x => x.Resize(150, 50));

                        // store image into MemoryStream with PNG type
                        using (var outputMs = new MemoryStream())
                        {
                            image.Save(outputMs, new PngEncoder());
                            outputMs.Seek(0, SeekOrigin.Begin); // read at first

                            // init XImage
                            var xImage = XImage.FromStream(() => outputMs);

                            // draw image
                            var cusSign = @"ĐẠI DIỆN BÊN BÁN
(Ký, ghi rõ họ tên)";
                            tf.DrawString(cusSign, fontBold, XBrushes.Black, new XRect(80, signTitlePos, 150, 50), XStringFormats.TopLeft);
                            gfx.DrawImage(xImage, 55, signImgPos, 150, 80);

                            // customer name
                            tf.DrawString("EasyRoomie System", fontContent, XBrushes.Black, new XRect(85, signFullNamePos, 150, 50), XStringFormats.TopLeft);
                        }
                    }
                }

                var signatureBytes = await client.GetByteArrayAsync(request.LandlordSignatureUrl);

                using (var ms = new MemoryStream(signatureBytes))
                {
                    using (var image = Image.Load<Rgba32>(ms))
                    {
                        image.Mutate(x => x.Resize(150, 50));

                        using (var outputMs = new MemoryStream())
                        {
                            image.Save(outputMs, new PngEncoder());
                            outputMs.Seek(0, SeekOrigin.Begin);

                            var xImage = XImage.FromStream(() => outputMs);

                            tf.DrawString("ĐẠI DIỆN BÊN B\n(Ký, ghi rõ họ tên)", fontBold, XBrushes.Black, new XRect(page.Width - 200, signTitlePos, 150, 50), XStringFormats.TopLeft);
                            gfx.DrawImage(xImage, page.Width - 220, signImgPos, 150, 80);

                            tf.DrawString(request.LandlordName, fontContent, XBrushes.Black, new XRect(page.Width - 180, signFullNamePos, 150, 50), XStringFormats.TopLeft);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return document;
        }
    }
}

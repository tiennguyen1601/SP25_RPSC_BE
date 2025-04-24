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
using SixLabors.ImageSharp.Processing;
using SP25_RPSC.Data.Models.ContractCustomerModel.ContractCustomerRequest;

namespace SP25_RPSC.Services.Utils.PdfGenerator
{
    public class CustomerContractPdfGenerator
    {
        public static async Task<PdfDocument?> GenerateCustomerContractPdf(CustomerContractRequestDTO request, HttpClient client)
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
            DrawCenteredText("HỢP ĐỒNG THUÊ NHÀ TRỌ", new XFont("Arial", 14, XFontStyle.Bold), currentY);
            currentY += 10;
            DrawCenteredText($"(Số: {request.ContractId}/HĐTNO)", new XFont("Arial", 12, XFontStyle.Regular), currentY);
            currentY += 30;

            // Date
            tf.DrawString($"Hôm nay, ngày {request.StartDate:dd} tháng {request.StartDate:MM} năm {request.StartDate:yyyy}, Tại {request.LandlordAddress}", fontContent, XBrushes.Black, new XRect(marginLeft, currentY, marginRight - marginLeft, 20), XStringFormats.TopLeft);
            currentY += 25;
            tf.DrawString("Chúng tôi gồm có:", fontContent, XBrushes.Black, new XRect(marginLeft, currentY, marginRight - marginLeft, 20), XStringFormats.TopLeft);
            currentY += 25;

            // Party A (Bên Cho Thuê)
            string partyAInfo = @$"BÊN CHO THUÊ (BÊN A):
Ông/bà: {request.LandlordName}
Địa chỉ: {request.LandlordAddress}
Điện thoại: {request.LandlordPhone}
Là chủ sở hữu nhà ở: {request.RoomAddress}";
            DrawParagraph(partyAInfo, fontContent, lineHeight);
            currentY += 10;

            // Party B (Bên Thuê)
            string partyBInfo = @$"BÊN THUÊ (BÊN B):
Ông/bà: {request.CustomerName}
Địa chỉ: {request.CustomerAddress}
Điện thoại: {request.CustomerPhone}";
            DrawParagraph(partyBInfo, fontContent, lineHeight);
            currentY += 15;

            tf.DrawString("Hai bên cùng thỏa thuận ký hợp đồng với những nội dung sau:", fontContent, XBrushes.Black, new XRect(marginLeft, currentY, marginRight - marginLeft, 20), XStringFormats.TopLeft);
            currentY += 25;

            // Article 1: Contract Object
            DrawCenteredText("ĐIỀU 1. ĐỐI TƯỢNG CỦA HỢP ĐỒNG", fontBold, currentY);
            currentY += 20;
            string section1 = @$"Bên A đồng ý cho Bên B thuê căn hộ (căn nhà) tại địa chỉ {request.RoomAddress} thuộc sở hữu hợp pháp của Bên A.
Chi tiết căn hộ như sau:
{request.RoomDescription}";
            DrawParagraph(section1, fontContent, lineHeight);
            currentY += 15;

            // Article 2: Rental Price
            DrawCenteredText("ĐIỀU 2. GIÁ CHO THUÊ NHÀ Ở VÀ PHƯƠNG THỨC THANH TOÁN", fontBold, currentY);
            currentY += 20;
            string section2 = @$"2.1. Giá cho thuê nhà ở là {request.Price.ToString("N0", new CultureInfo("vi-VN"))} đồng/tháng (Bằng chữ: {request.PriceInWords})
Giá cho thuê này đã bao gồm các chi phí về quản lý, bảo trì và vận hành nhà ở.
2.2. Các chi phí sử dụng điện, nước, điện thoại và các dịch vụ khác do bên B thanh toán cho bên cung cấp điện, nước, điện thoại và các cơ quan quản lý dịch vụ.
2.3. Phương thức thanh toán: bằng {request.PaymentMethod}, trả vào ngày {request.PaymentDate} hàng tháng.";
            DrawParagraph(section2, fontContent, lineHeight);
            currentY += 15;

            // Article 3: Duration
            DrawCenteredText("ĐIỀU 3. THỜI HẠN THUÊ VÀ THỜI ĐIỂM GIAO NHẬN NHÀ Ở", fontBold, currentY);
            currentY += 20;
            string section3 = @$"3.1. Thời hạn thuê ngôi nhà nêu trên là {request.Duration} tháng kể từ ngày {request.StartDate:dd} tháng {request.StartDate:MM} năm {request.StartDate:yyyy}.
3.2. Thời điểm giao nhận nhà ở là ngày {request.StartDate:dd} tháng {request.StartDate:MM} năm {request.StartDate:yyyy}.";
            DrawParagraph(section3, fontContent, lineHeight);
            currentY += 15;

            // Add new page if needed
            if (currentY > pageHeight - 100)
            {
                AddNewPage();
            }

            // Article 4: Rights and Obligations of Party A
            DrawCenteredText("ĐIỀU 4. NGHĨA VỤ VÀ QUYỀN CỦA BÊN A", fontBold, currentY);
            currentY += 20;
            string section4 = @"4.1. Nghĩa vụ của bên A:
a) Giao nhà ở và trang thiết bị gắn liền với nhà ở (nếu có) cho bên B theo đúng hợp đồng;
b) Phổ biến cho bên B quy định về quản lý sử dụng nhà ở;
c) Bảo đảm cho bên B sử dụng ổn định nhà trong thời hạn thuê;
d) Bảo dưỡng, sửa chữa nhà theo định kỳ hoặc theo thỏa thuận; nếu bên A không bảo dưỡng, sửa chữa nhà mà gây thiệt hại cho bên B, thì phải bồi thường;";
            DrawParagraph(section4, fontContent, lineHeight);

            // Check if we need a new page
            if (currentY > pageHeight - 100)
            {
                AddNewPage();
            }

            string section4b = @"4.2. Quyền của bên A:
a) Yêu cầu bên B trả đủ tiền thuê nhà đúng kỳ hạn như đã thỏa thuận;
b) Yêu cầu bên B có trách nhiệm trong việc sửa chữa phần hư hỏng, bồi thường thiệt hại do lỗi của bên B gây ra;
c) Cải tạo, nâng cấp nhà cho thuê khi được bên B đồng ý, nhưng không được gây phiền hà cho bên B sử dụng chỗ ở;
d) Được lấy lại nhà cho thuê khi hết hạn hợp đồng;";
            DrawParagraph(section4b, fontContent, lineHeight);
            currentY += 15;

            // Check if we need a new page
            if (currentY > pageHeight - 100)
            {
                AddNewPage();
            }

            // Article 5: Rights and Obligations of Party B
            DrawCenteredText("ĐIỀU 5. NGHĨA VỤ VÀ QUYỀN CỦA BÊN B", fontBold, currentY);
            currentY += 20;
            string section5 = @"5.1. Nghĩa vụ của bên B:
a) Sử dụng nhà đúng mục đích đã thỏa thuận, giữ gìn nhà ở và có trách nhiệm trong việc sửa chữa những hư hỏng do mình gây ra;
b) Trả đủ tiền thuê nhà đúng kỳ hạn đã thỏa thuận;
c) Trả tiền điện, nước, điện thoại, vệ sinh và các chi phí phát sinh khác trong thời gian thuê nhà;
d) Trả nhà cho bên A theo đúng thỏa thuận.
e) Chấp hành đầy đủ những quy định về quản lý sử dụng nhà ở;";
            DrawParagraph(section5, fontContent, lineHeight);

            // Check if we need a new page
            if (currentY > pageHeight - 100)
            {
                AddNewPage();
            }

            string section5b = @"5.2. Quyền của bên B:
a) Nhận nhà ở và trang thiết bị gắn liền (nếu có) theo đúng thoả thuận;
b) Yêu cầu bên A sửa chữa nhà đang cho thuê trong trường hợp nhà bị hư hỏng nặng;
c) Được ưu tiên ký hợp đồng thuê tiếp, nếu đã hết hạn thuê mà nhà vẫn dùng để cho thuê;";
            DrawParagraph(section5b, fontContent, lineHeight);
            currentY += 15;

            // Check if we need a new page
            if (currentY > pageHeight - 100)
            {
                AddNewPage();
            }

            // Article 8: Other Agreements
            DrawCenteredText("ĐIỀU 6. CÁC THỎA THUẬN KHÁC", fontBold, currentY);
            currentY += 20;
            string section8 = @"6.1. Việc sửa đổi, bổ sung hoặc hủy bỏ hợp đồng này phải lập thành văn bản mới có giá trị để thực hiện.
6.2. Việc chấm dứt hợp đồng thuê nhà ở được thực hiện khi có một trong các trường hợp sau đây:
a) Hợp đồng thuê nhà ở hết hạn;
b) Nhà ở cho thuê không còn;
c) Nhà ở cho thuê bị hư hỏng nặng, có nguy cơ sập đổ;
d) Hai bên thoả thuận chấm dứt hợp đồng trước thời hạn;";
            DrawParagraph(section8, fontContent, lineHeight);
            currentY += 15;

            // Check if we need a new page
            if (currentY > pageHeight - 100)
            {
                AddNewPage();
            }

            // Article 9: Commitments
            DrawCenteredText("ĐIỀU 7. CAM KẾT CỦA CÁC BÊN", fontBold, currentY);
            currentY += 20;
            string section9 = @"Bên A và bên B chịu trách nhiệm trước pháp luật về những lời cùng cam kết sau đây:
7.1. Đã khai đúng sự thật và tự chịu trách nhiệm về tính chính xác của những thông tin về nhân thân đã ghi trong hợp đồng này.
7.2. Thực hiện đúng và đầy đủ tất cả những thỏa thuận đã ghi trong hợp đồng này; nếu bên nào vi phạm mà gây thiệt hại, thì phải bồi thường cho bên kia hoặc cho người thứ ba (nếu có).
7.3. Hợp đồng này có giá trị kể từ ngày hai bên ký kết.
Hợp đồng được lập thành hai (02) bản, mỗi bên giữ một bản và có giá trị như nhau.";
            DrawParagraph(section9, fontContent, lineHeight);
            currentY += 30;

            // Signing Section
            var signTitlePos = currentY;
            var signImgPos = signTitlePos + 30;
            var signFullNamePos = signImgPos + 75;

            // Upload signatures
            try
            {
                var landlordSignatureBytes = await client.GetByteArrayAsync(request.LandlordSignatureUrl);

                // read image
                using (var ms = new MemoryStream(landlordSignatureBytes))
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
                            tf.DrawString("BÊN CHO THUÊ\n(Ký, ghi rõ họ tên)", fontBold, XBrushes.Black, new XRect(80, signTitlePos, 150, 50), XStringFormats.TopLeft);
                            gfx.DrawImage(xImage, 55, signImgPos, 150, 80);

                            // landlord name
                            tf.DrawString(request.LandlordName, fontContent, XBrushes.Black, new XRect(85, signFullNamePos, 150, 50), XStringFormats.TopLeft);
                        }
                    }
                }

                var customerSignatureBytes = await client.GetByteArrayAsync(request.CustomerSignatureUrl);

                using (var ms = new MemoryStream(customerSignatureBytes))
                {
                    using (var image = Image.Load<Rgba32>(ms))
                    {
                        image.Mutate(x => x.Resize(150, 50));

                        using (var outputMs = new MemoryStream())
                        {
                            image.Save(outputMs, new PngEncoder());
                            outputMs.Seek(0, SeekOrigin.Begin);

                            var xImage = XImage.FromStream(() => outputMs);

                            tf.DrawString("BÊN THUÊ\n(Ký, ghi rõ họ tên)", fontBold, XBrushes.Black, new XRect(page.Width - 200, signTitlePos, 150, 50), XStringFormats.TopLeft);
                            gfx.DrawImage(xImage, page.Width - 220, signImgPos, 150, 80);

                            tf.DrawString(request.CustomerName, fontContent, XBrushes.Black, new XRect(page.Width - 180, signFullNamePos, 150, 50), XStringFormats.TopLeft);
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
using System.Text.Json;

using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

using PinballApronCard.Models;

namespace PinballApronCard.Logic;
public class ApronPdfGenerator
{
    private const float Cm = 28.35f;
    private Point margin = new Point((int)(1f * Cm), (int)(1f * Cm));
    private List<KeyValuePair<string, Image>> BackgroundImages = new List<KeyValuePair<string, Image>>();
    private PdfFont FontBold = PdfFontFactory.CreateFont("./fonts/Futura Round Medium.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
    private PdfFont FontLight = PdfFontFactory.CreateFont("./fonts/Futura Round Light.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

    public void Process()
    {
        var aprons = LoadJson<Apron>("./Input/example.json");
        var apronTypes = LoadJson<ApronType>("./Input/aprontypes.json");
        BackgroundImages = LoadBackgroundImages();
        Print(aprons!, apronTypes);
    }

    private static List<T> LoadJson<T>(string path)
        => JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true, }) ?? [];

    private static List<KeyValuePair<string, Image>> LoadBackgroundImages()
        => (from image in Directory.GetFiles("./images", "*.png")
            let name = System.IO.Path.GetFileName(image)
            let imageData = ImageDataFactory.Create(image)
            let img = new Image(imageData)
            select new KeyValuePair<string, Image>(name, img)).ToList();

    private void Print(List<Apron> aprons, List<ApronType> apronTypes)
    {
        using var pdf = new PdfDocument(new PdfWriter($"./output/aprons.pdf"));
        using Document document = new Document(pdf, PageSize.A4);
        float pageHeight = document.GetPdfDocument().GetDefaultPageSize().GetHeight();
        var cursor = new Point(margin.x, (int)(pageHeight - margin.y));

        foreach (var apron in aprons)
        {
            var apronType = apronTypes.FirstOrDefault(a => a.Id == apron.ApronCardSize);
            if (apronType == null)
            {
                Console.WriteLine($"Apron type {apron.ApronCardSize} not found for apron {apron.Name}.");
                continue;
            }

            if ((float)(cursor.y - (int)(apronType.Height * Cm)) < margin.y)
            {
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                cursor = new Point(margin.x, (int)(pageHeight - margin.y));
            }

            cursor = RenderApron(document, cursor, apron, apronType);
        }

        document.Close();
    }

    private Point RenderApron(Document document, Point cursor, Apron apron, ApronType apronType)
    {
        var image = BackgroundImages.FirstOrDefault(i => i.Key == apronType.Image).Value;
        if (image == null)
        {
            Console.WriteLine($"Image {apronType.Image} not found (id = {apronType.Id}).");
            return cursor;
        }
        float yPos = (float)(cursor.y - (int)(apronType.Height * Cm));

        // Add image to document
        image.ScaleToFit(apronType.Width * Cm, apronType.Height * Cm);
        image.SetFixedPosition((float)cursor.x, yPos);
        document.Add(image);

        AddHeaderAndContentText(document, new Point(margin.x, yPos), apron, apronType);

        // Add QR code to document
        AddQrImage(document, cursor, apron, apronType, yPos);

        // add apron + margin to cursor
        cursor.y = yPos - 0.2 * Cm;
        return cursor;
    }

    private static void AddQrImage(Document document, Point cursor, Apron apron, ApronType apronType, float yPos)
    {
        apron.QrCode = apron.QrCode.Substring(apron.QrCode.IndexOf("base64,") + 7);
        byte[] imageBytes = Convert.FromBase64String(apron.QrCode);
        var imageData = ImageDataFactory.Create(imageBytes);
        var qrImage = new Image(imageData);

        qrImage.ScaleToFit(apronType.Qr.Width * Cm, apronType.Height * Cm);
        qrImage.SetFixedPosition((float)cursor.x + apronType.Qr.Xpos * Cm, yPos + apronType.Qr.Ypos * Cm);
        document.Add(qrImage);
    }

    private void AddHeaderAndContentText(Document document, Point cursor, Apron apron, ApronType apronType)
    {
        float xPos = (float)(cursor.x + (apronType.Text.Xpos * Cm));
        float yPos = (float)(cursor.y + (apronType.Text.Ypos * Cm));
        var width = apronType.Text.Width * Cm;

        var pHeader = new Paragraph(apron.Name)
            .SetFont(FontBold)
            .SetFontSize(apronType.Text.FontSize);
        var headerHeight = CalculateHeightOfElement(pHeader, width, document.GetRenderer());
        pHeader.SetFixedPosition(xPos, yPos - headerHeight, width);
        document.Add(pHeader);

        var content = apronType.Text.Template
            .Replace("{manufacturer}", apron.Manufacturer)
            .Replace("{year}", apron.Year.ToString())
            .Replace("{tech}", apron.MachineTech);
        var pContent = new Paragraph(content)
            .SetFont(FontLight)
            .SetFontSize(apronType.Text.FontSize);
        var contentHeight = CalculateHeightOfElement(pContent, width, document.GetRenderer());
        pContent.SetFixedPosition(xPos, yPos - contentHeight - headerHeight, width);
        document.Add(pContent);

        var dev = new Paragraph($"{apron.Id}\n{apron.ApronCardSize}")
            .SetFont(FontBold)
            .SetFontSize(apronType.Text.FontSize);
        var devHeight = CalculateHeightOfElement(pHeader, width, document.GetRenderer());
        dev.SetFixedPosition(xPos + apronType.Width * Cm + 10, yPos - 2 * Cm, width);
        document.Add(dev);


    }

    public float CalculateHeightOfElement(IElement element, float width, IRenderer rootRender)
    {
        IRenderer eRenderer = element.CreateRendererSubTree();
        LayoutResult result = eRenderer.SetParent(rootRender).Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, 1000))));
        float height = result.GetOccupiedArea().GetBBox().GetHeight();
        return height;
    }
}

namespace PinballApronCard.Logic;

using System.Text.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using PinballApronCard.Helpers;
using PinballApronCard.Models;

public class ApronPdfGenerator
{
    private XFont NameFont;
    private XFont NumberFont;
    private XFont SmallFont;
    public ApronPdfGenerator()
    {
        GlobalFontSettings.FontResolver = new FontResolver();
        NameFont = new XFont("Consolas", 18, XFontStyle.Bold);
        NumberFont = new XFont("Consolas", 18, XFontStyle.Bold);
        SmallFont = new XFont("Consolas", 10, XFontStyle.Bold);
    }

    public void Process()
    {
        var json = File.ReadAllText($"./Input/example.json");
        var aprons = JsonSerializer.Deserialize<List<Apron>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new ApronSizeConverter() }
        });

        aprons!
            .OrderBy(a => a.Size)
            // .Where(a => a.DpoTShirt != null)
            .ToList().ForEach(a =>
            {
                // Console.WriteLine($"{a.Name}, {a.DpoTShirt!.Size}");
                // Console.WriteLine($"{a.Name}, {a.Number.First()}, {string.Join("/", a.Days)}, {a.Opmerkingen}");
            });
        Print(aprons!);
    }

    private void Print(List<Apron> aprons)
    {
        // 3x8
        var document = new PdfDocument();

        while (aprons.Count() > 0)
        {
            var page = document.AddPage();
            var e = aprons.TakeRange(12);
            DrawPage(page, e.ToList());
        }

        document.Save($"./output/test.pdf");
    }

    private void DrawPage(PdfPage page, List<Apron> envelopes)
    {
        //       10              10
        // 15 [ 272 ] 10 | 10 [ 272 ] 15 (height:115)
        //       10              10
        // Width = 595
        // Height =842
        var gfx = XGraphics.FromPdfPage(page);
        var textColor = XBrushes.Black;

        var i = 0;
        foreach (var env in envelopes)
        {
            // 0 1  -> 0 0
            // 2 3  -> 1 1
            // 4 5  -> 2 2

            var x = 570D / 2 * (envelopes.IndexOf(env) % 2D);
            var y = 10 + (135.2 * i);

            var x1 = 18 + x;
            var y1 = 10 + y;
            // Draw box
            // gfx.DrawRectangle(XBrushes.LightGray, x1, y1, 272, 131);
            // gfx.DrawRectangle(XBrushes.White, x1 + 1, y1 + 1, 272 - 2, 131 - 2);

            // Margin
            y1 = y1 + 5;
            x1 = x1 + 5;

            // gfx.DrawRectangle(XBrushes.White, x1, y1, 200, 287);
            // gfx.DrawRectangle(XBrushes.Black, x1, y1, 200, 287);
            DrawName(gfx, env.Name, x1, y1);
            Opmerkingen(gfx, x1, y1, env.Name);

            // Prep next
            i = i + (envelopes.IndexOf(env) % 2);
        }
    }


    private void DrawName(XGraphics gfx, string name, Double x, Double y)
    {
        var size = gfx.MeasureString(name, NameFont);
        if (size.Width > 160)
        {
            name = name.Substring(0, Math.Min(18, name.Length)) + "...";
        }
        gfx.DrawString(name, NameFont, XBrushes.Black, x, y + 23);
    }


    private void Opmerkingen(XGraphics gfx, Double x, Double y, string opmerkingen)
    {
        if (string.IsNullOrEmpty(opmerkingen))
            return;
        var rect = new XRect(x, y + 73, 270, 52);
        var tf = new XTextFormatter(gfx);
        opmerkingen = opmerkingen.Replace("\\n", " ");
        opmerkingen = opmerkingen.Replace("\\r", " ");
        tf.DrawString($"{opmerkingen}", SmallFont, XBrushes.Black, rect, XStringFormats.TopLeft);
    }

}
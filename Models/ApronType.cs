using System.Text.Json.Serialization;

namespace PinballApronCard.Models
{
    public class ApronType
    {
        public string Id { get; set; } = default!;
        public float Height { get; set; }
        public float Width { get; set; }
        public string Image { get; set; } = default!;
        public Element Qr { get; set; } = default!;
        public TextElement Text { get; set; } = default!;
        public List<ManufacturerElement> Manufacturer { get; set; } = default!;

        public class Element
        {
            public float Xpos { get; set; }
            public float Ypos { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }

        }
        public class TextElement : Element
        {
            public int FontSize { get; set; }
            public string Template { get; set; } = default!;
        }

        public class ManufacturerElement : Element
        {
            public string Name { get; set; } = default!;
            public string Logo { get; set; } = default!;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace App2
{
    public class Stand
    {
        public string Text { get; set; }
        public string Pass { get; private set; }
        public ImageSource source;
        public bool Visited;
        public int Points { get; set; }

        public Stand(string text, string pass, int points = 1)
        {
            Text = text;
            Pass = pass;
            Visited = false;
            Points = points;
        }

        public Stand(XmlNode stand)
        {
            Text = stand.Attributes["Text"].Value;
            Pass = stand.Attributes["Pass"].Value;
            source = ImageSource.FromStream(()=>Settings.DownloadImage(Settings.FtpServer + "/QRApp/" + stand.Attributes["Img"].Value));
            Visited = false;
            Points = int.Parse(stand.Attributes["Points"].Value);
        }

        public StackLayout GetStackLayout()
        {
            Color color = Color.Default;
            double fontSize = 20;
            if (Visited)
                color = Color.Green;

            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal,HorizontalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(new Label { Text = "\u2713", FontSize=fontSize, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, TextColor = color, Opacity = Visited ? 1 : 0 });
            stack.Children.Add(new Image() { Source = source });
            stack.Children.Add(new Label { Text = this.Text, FontSize = fontSize, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start, TextColor = color });
            stack.Children.Add(new Label { Text = Points.ToString(), FontSize = fontSize, HorizontalOptions = LayoutOptions.End,HorizontalTextAlignment= TextAlignment.Start, TextColor = color });
            return stack;
        }

        public bool Visit(string pass)
        {
            if (pass.ToLower() == Pass)
            {
                this.Visited = true;
                return true;
            }
            return false;
        }
    }
}

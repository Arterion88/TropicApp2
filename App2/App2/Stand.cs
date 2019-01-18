using System;
using System.Collections.Generic;
using System.IO;
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
            string url = Settings.Server + Settings.imgFolder + stand.Attributes["Img"].Value;
            Stream stream = Settings.DownloadImage(url);
            source = ImageSource.FromUri(new Uri(url));

            Visited = false;
            Points = int.Parse(stand.Attributes["Points"].Value);
        }

        public List<View> AddRow()
        {
            Color color = Visited? Color.Green:Color.Default;
            double fontSize = 20;

            //StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
            //stack.Children.Add(new Label { Text = "\u2713", FontSize = fontSize, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, TextColor = color, Opacity = Visited ? 1 : 0 });
            //stack.Children.Add(new Image() { Source = source,BackgroundColor=Color.Black,Aspect=Aspect.AspectFit, VerticalOptions = LayoutOptions.Start });
            //stack.Children.Add(new Label { Text = this.Text, FontSize = fontSize, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start, TextColor = color });
            //stack.Children.Add(new Label { Text = Points.ToString(), FontSize = fontSize, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.Start, TextColor = color });

            List<View> list = new List<View>
            {
                new Label { Text = "\u2713", FontSize = fontSize, HorizontalOptions = LayoutOptions.Start,VerticalOptions=LayoutOptions.Center, VerticalTextAlignment=TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, TextColor = color, Opacity = Visited ? 1 : 0 },
                new Image { Source = source, BackgroundColor = Color.Black, Aspect = Aspect.AspectFit, VerticalOptions = LayoutOptions.Center ,HeightRequest=22},
                new Label { Text = this.Text, FontSize = fontSize, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions=LayoutOptions.Center, VerticalTextAlignment=TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Start, TextColor = color },
                new Label { Text = Points.ToString(), FontSize = fontSize, HorizontalOptions = LayoutOptions.End, VerticalOptions=LayoutOptions.Center, VerticalTextAlignment=TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Start, TextColor = color }
            };

            return list;
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

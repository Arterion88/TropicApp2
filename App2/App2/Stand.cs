using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace App2
{
    public class Stand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Text { get; set; }
        public Color TextColor => Visited ? Color.Green : Color.Default;
        public double Opacity => Visited ? 1 : 0;
        public string Pass { get; private set; }


        #region ImageSource
        private string ImageSourceUrl { get; set; }
        private ImageSource source;
        public ImageSource Source
        {
            get { return ImageSource.FromUri(new Uri(ImageSourceUrl)); }
            set
            {
                //if (source == null)
                    source = ImageSource.FromUri(new Uri(ImageSourceUrl));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Source"));
            }
        }
        #endregion

        #region Visited
        private bool visited;
        private int _points;

        public bool Visited
        {
            get { return visited; }
            set
            {
                visited = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visited"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextColor"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Opacity"));
            }
        }
        #endregion


        public int Points { get => _points; set => _points = value; }
        //public string PointsString { get { return Points.ToString(); } }

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
            Pass = stand.Attributes["Pass"].Value.ToLower();
            ImageSourceUrl = Settings.imgFolder + stand.Attributes["Img"].Value;

            Visited = false;
            Points = int.Parse(stand.Attributes["Points"].Value);
        }

        public void AddRow(Grid grid, int row)
        {
            Color color = Visited ? Color.Green : Color.Default;
            double fontSize = 22;

            grid.Children.Add(new Label
            {
                Text = "\u2713",
                FontSize = fontSize,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = color,
                Opacity = Visited ? 1 : 0
            }, 0, row);
            grid.Children.Add(new Label
            {
                Text = this.Text,
                FontSize = fontSize,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = color
            }, 1, row);
            grid.Children.Add(new Image
            {
                Source = Source,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 30,

            }, 2, row);
            grid.Children.Add(new Label
            {
                Text = Points.ToString(),
                FontSize = fontSize,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = color
            }, 3, row);

        }

        public static void AddEmptyRow(Grid grid, int row)
        {
            double fontSize = 22;

            grid.Children.Add(new Label
            {
                Text = "\u2713",
                FontSize = fontSize,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                Opacity = 0

            }, 0, row);
            grid.Children.Add(new Label
            {
                Text = "dffefrfgdreg",
                FontSize = fontSize,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                Opacity = 0

            }, 1, row);
            grid.Children.Add(new Image
            {
                HeightRequest = 30,
                Opacity = 0

            }, 2, row);
            grid.Children.Add(new Label
            {
                Text = "5",
                FontSize = fontSize,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                Opacity = 0

            }, 3, row);

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

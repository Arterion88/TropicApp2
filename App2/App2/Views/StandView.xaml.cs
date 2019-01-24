using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StandView : ContentView
	{
        public int ContentSource { get; set; }
        public StandView ()
		{
			InitializeComponent ();
		}

        #region Text (Bindable string)
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                                  "Text", //Public name to use
                                                                  typeof(string), //this type
                                                                  typeof(StandView), //parent type (tihs control)
                                                                  string.Empty); //default value
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion Text (Bindable string)
    }
}
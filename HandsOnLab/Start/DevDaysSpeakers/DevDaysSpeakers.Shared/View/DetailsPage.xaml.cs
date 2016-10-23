using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using DevDaysSpeakers.Model;
using Plugin.TextToSpeech;

using DevDaysSpeakers.ViewModel;

namespace DevDaysSpeakers.View
{
    public partial class DetailsPage : ContentPage
    {
		readonly Speaker _speaker;
		public DetailsPage(Speaker speaker)
        {
            InitializeComponent();
            
            //Set local instance of speaker and set BindingContext
            _speaker = speaker;
            BindingContext = _speaker;

			ButtonSpeak.Clicked += ButtonSpeakClicked;
			ButtonGo.Clicked += ButtonGoClicked;
        }

		void ButtonSpeakClicked(object sender, EventArgs e)
		{
			CrossTextToSpeech.Current.Speak(_speaker.Description);
		}

		void ButtonGoClicked(object sender, EventArgs e)
		{
			if (_speaker.Website.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
			{
				Device.OpenUri(new Uri(_speaker.Website));
			}
		}
        
    }
}

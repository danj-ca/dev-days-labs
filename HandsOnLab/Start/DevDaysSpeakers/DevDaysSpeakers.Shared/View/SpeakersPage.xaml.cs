﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using DevDaysSpeakers.Model;
using DevDaysSpeakers.ViewModel;


namespace DevDaysSpeakers.View
{
    public partial class SpeakersPage : ContentPage
    {
		readonly SpeakersViewModel vm;
		public SpeakersPage()
        {
            InitializeComponent();

            //Create the view model and set as binding context
            vm = new SpeakersViewModel();
            BindingContext = vm;

			ListViewSpeakers.ItemSelected += ListViewSpeakersItemSelected;
        }

		private async void ListViewSpeakersItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var speaker = e.SelectedItem as Speaker;
			if (speaker == null)
				return;

			await Navigation.PushAsync(new DetailsPage(speaker));

			ListViewSpeakers.SelectedItem = null;
		}

       
    }
}

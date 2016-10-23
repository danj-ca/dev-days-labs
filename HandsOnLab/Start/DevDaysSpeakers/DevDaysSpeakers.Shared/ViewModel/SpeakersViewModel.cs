using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DevDaysSpeakers.ViewModel
{
	public class SpeakersViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string name = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		bool _busy;
		public bool IsBusy
		{
			get { return _busy; }
			set
			{
				_busy = value;
				OnPropertyChanged();

				GetSpeakersCommand.ChangeCanExecute(); // guessing this emits the OnExecuteChanged event, or whatever; it's a Xam.Forms method
			}
		}

		public ObservableCollection<Speaker> Speakers
		{
			get;
			set;
		}

		public Command GetSpeakersCommand { get; private set; }

		public SpeakersViewModel()
		{
			Speakers = new ObservableCollection<Speaker>();
			GetSpeakersCommand = new Command(async () => await GetSpeakers(), () => !IsBusy);
		}

		async Task GetSpeakers()
		{
			if (IsBusy) return;

			Exception error = null;

			try
			{
				IsBusy = true;
				// Let's use Azure instead!

				//using (var client = new HttpClient())
				//{
				//	var json = await client.GetStringAsync("http://demo4404797.mockable.io/speakers");
				//	var items = JsonConvert.DeserializeObject<List<Speaker>>(json); // could use async version if list were massive
				//}


				// If we needed platform-specific versions of our AzureService,
				// using the DependencyService here resolves the correct one
				// for whichever platform we're running on
				//
				// Since AzureService is part of our shared code, doing it
				// this way is a bit trivial, but is basically like using
				// a built-in DI container...
				var service = DependencyService.Get<AzureService>();
				var items = await service.GetSpeakers();

				Speakers.Clear();
				foreach (var item in items)
				{
					Speakers.Add(item);
				}
			}
			catch (Exception ex)
			{
				error = ex;
			}
			finally
			{
				IsBusy = false;
			}

			if (error != null)
			{
				// This is admittedly a goofball technique for displaying an error dialog...
				await Application.Current.MainPage.DisplayAlert("OH NOES!", error.Message, "Alas");
			}
		}
	}
}

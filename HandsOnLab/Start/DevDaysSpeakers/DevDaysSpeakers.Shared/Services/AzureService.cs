using DevDaysSpeakers.Services;
using DevDaysSpeakers.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;

// This attribute registers this type with the Xamarin.Forms DependencyService
[assembly: Dependency(typeof(AzureService))]
namespace DevDaysSpeakers.Services
{
    public class AzureService
    {
        public MobileServiceClient Client { get; set; } = null;
		IMobileServiceSyncTable<Speaker> _table;

		/// <summary>
		/// Initialize our local storage and hook up to our remote.
		/// </summary>
        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            var appUrl = "https://danjtest-speakers.azurewebsites.net";

            //Create our client
            Client = new MobileServiceClient(appUrl);

            //InitializeDatabase for path
            var path = "syncstore.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);


            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<Speaker>();

            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            _table = Client.GetSyncTable<Speaker>();
        }


        public async Task<IEnumerable<Speaker>> GetSpeakers()
        {
			await Initialize();
			await SyncSpeakers();

			return await _table.OrderBy(s => s.Name).ToEnumerableAsync();
        }

      
        public async Task SyncSpeakers()
        {
            try
            {
				await Client.SyncContext.PushAsync(); // sync local changes up to Azure
				await _table.PullAsync("allSpeakers", _table.CreateQuery()); // pull down any new changes from the cloud
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync speakers, that is alright as we have offline capabilities: " + ex);
            }

        }
    }
}
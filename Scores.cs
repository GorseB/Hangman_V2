using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using System.Data;

namespace Hangman_V2
{
    [Activity(Label = "Scores")]
    public class Scores : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            var DB = new Azure_Connect();
            // Create Database Class to Fetch Scores
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Scores);
            Button Return = FindViewById<Button>(Resource.Id.Scores_ButtonBack);
            Return.Click += delegate { StartActivity(typeof(Home)); };
            var LV = FindViewById<ListView>(Resource.Id.Scores_Listview);
            DataTable DT = new DataTable();
            DT = DB.Fill();
            // Fetch Scores
            List<string> MyItems = new List<string>();
            foreach (DataRow row in DT.Rows)
            {
                string Word = (row.ItemArray[2].ToString() + " Finished their run with " + row.ItemArray[3].ToString() + " Points. Last Word = " + row.ItemArray[1].ToString());
                MyItems.Add(Word);
                // Add them to a list
            }
            ArrayAdapter<string> Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, MyItems);
            LV.Adapter = Adapter;
            // The Listview on screen = the list i made
        }
    }
}
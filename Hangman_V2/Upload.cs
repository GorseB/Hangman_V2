using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using System.Data;

namespace Hangman_V2
{
    [Activity(Label = "Upload")]
    public class Upload : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            var DB = new Azure_Connect();
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Upload);
            string text = Intent.GetStringExtra("Data") ?? "Data not available";
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvqxyz".ToCharArray();
            int lastletter = text.LastIndexOfAny(alpha);
            string WordName = text.Substring(0, lastletter+1);
            int score = System.Convert.ToInt32(text.Substring(lastletter+1, text.Length-(lastletter+1)));
            Button Upload = FindViewById<Button>(Resource.Id.Upload);
            EditText Name = FindViewById<EditText>(Resource.Id.editText1);
            Upload.Click += delegate
            {
                if (Name.Text != "")
                {
                    DB.InsertData(WordName, Name.Text, score);
                    StartActivity(typeof(Home));
                }
            };
           
        }
    }
}
using Android.App;
using Android.OS;
using Android.Widget;

namespace Hangman_V2
{
    [Activity(Label = "Upload")]
    public class Upload : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            var DB = new Azure_Connect();
            // Create database to upload data
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Upload);
            string text = Intent.GetStringExtra("Data") ?? "Data not available";
            // Get Info from Game Activity
            // //
            //
            //
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvqxyz".ToCharArray();
            int lastletter = text.LastIndexOfAny(alpha);
            string WordName = text.Substring(0, lastletter + 1);
            int score = System.Convert.ToInt32(text.Substring(lastletter + 1, text.Length - (lastletter + 1)));
            //
            //
            //
            // // They only let you pass in one string so i had to use some trickery to get both the word and score out of it
            Button Upload = FindViewById<Button>(Resource.Id.Upload);
            EditText Name = FindViewById<EditText>(Resource.Id.editText1);
            Upload.Click += delegate
            {
                if (Name.Text != "")
                {
                    DB.InsertData(WordName, Name.Text, score);
                    StartActivity(typeof(Home));
                    // If there is text for a name, upload the data and take us home.
                }
            };
        }
    }
}
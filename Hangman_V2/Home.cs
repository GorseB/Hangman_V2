using Android.App;
using Android.OS;
using Android.Widget;

namespace Hangman_V2
{
    [Activity(Label = "Hangman", MainLauncher = true, Icon = "@drawable/icon")]
    public class Home : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Button btn_Scores = FindViewById<Button>(Resource.Id.Main_ToScores);
            Button btn_Exit = FindViewById<Button>(Resource.Id.Main_Exit);
            Button btn_Play = FindViewById<Button>(Resource.Id.Main_play);
            btn_Exit.Click += delegate { this.FinishAffinity(); };
            btn_Scores.Click += delegate { StartActivity(typeof(Scores)); };
            btn_Play.Click += delegate { StartActivity(typeof(Game)); };
        }
    }
}
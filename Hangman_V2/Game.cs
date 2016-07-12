using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hangman_V2
{
    [Activity(Label = "Game")]
    public class Game : Activity
    {
        static int score = 0;
        static int attempts = 3;
        static int lives = 3;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Game);
            string dbpath = Stuff();
            UserDialogs.Init(this);
            //
            //
            EditText Textbox = FindViewById<EditText>(Resource.Id.Game_PasswordTest);
            EditText Scorebox = FindViewById<EditText>(Resource.Id.Game_Info);
            string Word = Worker(dbpath);
            string HiddenWord = new string('*', Word.Length);
            Textbox.Text = HiddenWord;
            Scorebox.Text = "Tries : " + attempts + "  Lives : " + lives + "  ---Score : " + score;
            //
            GridView gridview = FindViewById<GridView>(Resource.Id.Game_GV);
            String[] letters = new String[] {
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T",
            "U", "V", "W", "X", "Y", "Z"};
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSelectableListItem, letters);
            gridview.Adapter = adapter;
            //
            gridview.ChildViewAdded += delegate (object sender, GridView.ChildViewAddedEventArgs args)
            {
                TextView child = (TextView)args.Child;
                child.Gravity = GravityFlags.Center;
                adapter.NotifyDataSetChanged();
            };
            //
            //
            gridview.ItemClick += delegate (object sender, GridView.ItemClickEventArgs args)
            {
                TextView View = (TextView)args.View;
                if (View.Text == "---")
                {
                }
                else
                {
                    string Guess = View.Text.ToLower();
                    Char GuessC = Guess[0];
                    if (Word.ToLower().Contains(Guess))
                    {
                        int count = Word.Length - Word.ToLower().Replace(Guess, "").Length;
                        int index;
                        string dummyword = Word;
                        for (int i = 0; i < count; i++)
                        {
                            index = dummyword.IndexOf(GuessC);
                            dummyword = ReplaceAtIndex(index, ' ', dummyword);
                            HiddenWord = ReplaceAtIndex(index, GuessC, HiddenWord);
                        }
                        Textbox.Text = HiddenWord;
                        if (!HiddenWord.Contains("*"))
                        {
                            Toast.MakeText(Application, "Good Work!", ToastLength.Short).Show(); ;
                            score += 1;
                            attempts = 3;
                            Scorebox.Text = "Tries : " + attempts + "  Lives : " + lives + "  ---Score : " + score;
                            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSelectableListItem, letters);
                            gridview.Adapter.Dispose();
                            gridview.Adapter = adapter;
                            Word = Worker(dbpath);
                            HiddenWord = new string('*', Word.Length);
                            Textbox.Text = HiddenWord;
                        }
                    }
                    else
                    {
                        if (attempts == 0)
                        {
                            if (lives == 0)
                            {
                                var Upload = new Intent(this, typeof(Upload));
                                Upload.PutExtra("Data", Word + score);
                                StartActivity(Upload);
                            }
                            Toast.MakeText(Application, "Word Was : " + Word, ToastLength.Short).Show();
                            lives -= 1;
                            attempts = 3;
                            Scorebox.Text = "Tries : " + attempts + "  Lives : " + lives + "  ---Score : " + score;
                            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSelectableListItem, letters);
                            gridview.Adapter.Dispose();
                            gridview.Adapter = adapter;
                            Word = Worker(dbpath);
                            HiddenWord = new string('*', Word.Length);
                            Textbox.Text = HiddenWord;
                        }
                        else
                        {
                            Toast.MakeText(Application, "Wrong!", ToastLength.Short).Show();
                            attempts -= 1;
                            Scorebox.Text = "Tries : " + attempts + "  Lives : " + lives + "  ---Score : " + score;
                        }
                    }
                    View.Text = "---";
                };
            };
            //
            //
            //
            //
        }

        private string Stuff()
        {
            string dbName = "HangmanWords.sql";
            string dbPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString(), dbName);
            // Check if your DB has already been extracted.
            if (!File.Exists(dbPath))
            {
                using (BinaryReader br = new BinaryReader(Assets.Open(dbName)))
                {
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int len = 0;
                        while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, len);
                        }
                    }
                }
            }
            return dbPath;
        }

        private string Worker(string dbPath)
        {
            Random Rand = new Random();
            int Random_Number = Rand.Next(0, 178691);
            using (var conn = new SQLite.SQLiteConnection(dbPath))
            {
                var cmd = new SQLite.SQLiteCommand(conn);
                cmd.CommandText = "SELECT Word FROM Words WHERE _Id = " + Random_Number;
                string Word = cmd.ExecuteScalar<string>();
                return Word;
            }
        }

        private static string ReplaceAtIndex(int i, char value, string word)
        {
            char[] letters = word.ToCharArray();
            letters[i] = value;
            return string.Join("", letters);
        }
    }
}
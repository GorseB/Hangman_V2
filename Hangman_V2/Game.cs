using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.IO;

namespace Hangman_V2
{
    [Activity(Label = "Game")]
    public class Game : Activity
    {
        int score = 0;
        int attempts = 3;
        int lives = 3;
        // Init our variables for use by the program

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Game);
            string dbpath = Stuff();
            // ^ SQLite Stuff
            UserDialogs.Init(this);
            // Dialog Stuff ^
            //
            EditText Textbox = FindViewById<EditText>(Resource.Id.Game_PasswordTest);
            EditText Scorebox = FindViewById<EditText>(Resource.Id.Game_Info);
            string Word = Worker(dbpath);
            // Get our Random Word
            string HiddenWord = new string('*', Word.Length);
            // Mask it
            Textbox.Text = HiddenWord;
            // The word it masked on screen
            Scorebox.Text = "Tries : " + attempts + "  Lives : " + lives + "  ---Score : " + score;
            // Setup our scoreboard
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
                // Formatting our gridview
            };
            //
            //
            gridview.ItemClick += delegate (object sender, GridView.ItemClickEventArgs args)
            {
                TextView View = (TextView)args.View;
                // When you click on a view in the gridview (a letter) this code pulls it down so it can be processed
                if (View.Text == "---")
                // if its not a letter do nothing
                {
                }
                else
                {
                    string Guess = View.Text.ToLower();
                    // turn it into a lower for comparision
                    Char GuessC = Guess[0];
                    // a char value is also needed below
                    if (Word.ToLower().Contains(Guess))
                    // if you guessed correctly
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
                        // ^^ This block of code turns the ****** into **A**A for the game
                        if (!HiddenWord.Contains("*"))
                        // if you have won, reset game and you get a point!
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
                        { // Will Lose a Life
                            if (lives == 0)
                            { // Dead
                                var Upload = new Intent(this, typeof(Upload));
                                Upload.PutExtra("Data", Word + score);
                                StartActivity(Upload);
                            }
                            else
                            {
                                // Resetting the board V
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
                        }
                        else
                        {
                            // Will lose an Attempt
                            Toast.MakeText(Application, "Wrong!", ToastLength.Short).Show();
                            attempts -= 1;
                            Scorebox.Text = "Tries : " + attempts + "  Lives : " + lives + "  ---Score : " + score;
                        }
                    }
                    View.Text = "---";
                    // Finally sets the view to the null identifier so that the program knows that value has been used
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
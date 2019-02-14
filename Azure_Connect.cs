using System.Data;
using System.Data.SqlClient;

namespace Hangman_V2
{
    internal class Azure_Connect
    {
        private SqlConnection SCNT = new SqlConnection();
        private SqlCommand SCMD = new SqlCommand();
        private SqlDataAdapter DA = new SqlDataAdapter();
        public string ConnectionString = null;

        public Azure_Connect()
        {
            ConnectionString = "Data Source=jk029.database.windows.net;Initial Catalog=Hangman;Integrated Security=False;User ID=readwrite;Password=BW4WmnDW8K;Connect Timeout=60;";
            SCNT.ConnectionString = ConnectionString;
            SCMD.Connection = SCNT;
        }

        public DataTable Fill()
        {
            SCNT.ConnectionString = ConnectionString;
            SCMD.Connection = SCNT;
            DataTable DT = new DataTable();
            using (DA = new SqlDataAdapter("select * from Top_10 ", SCNT))
            {
                DA.Fill(DT);
                SCNT.Close();
            }
            return DT;
        }

        public void InsertData(string Word, string Player, int Score)
        {
            using (SqlCommand cmd = new SqlCommand("dbo.Procedure", SCNT))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WordName", Word);
                cmd.Parameters.AddWithValue("@PlayerName", Player);
                cmd.Parameters.AddWithValue("@Score", Score);
                SCNT.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
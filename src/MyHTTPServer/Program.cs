//using System;
//using System.Data.SqlClient;
namespace CSharpMicroService
{
    public class Program
    {
        public void Main(string[] args)
        {
            HTTPServer server = new HTTPServer();
            server.Start();
            /*
            SqlConnection cnn;

            //string connectionString = (@"Data Source=localhost; Initial Catalog=ContosoUniversity1; Integrated Security=True");
            string connectionString = "Data Source=(LocalDb)\\v11.0;AttachDBFilename=C:\\Users\\v-honguy\\Documents\\Visual Studio 2015\\Projects\\ContosoUniversity\\ContosoUniversity\\App_Data\\ContosoUniversity1.mdf;Initial Catalog=ContosoUniversity1;Integrated Security=SSPI;";

            cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();
                Console.WriteLine("Connection Open ! ");
                cnn.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not open connection ! ");
                Console.ReadLine();
            }
            */

        }
    }
}

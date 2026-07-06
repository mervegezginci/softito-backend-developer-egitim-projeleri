using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace OfisTasarim_RazorPagesProje.Pages.Categories
{
    public class EditModel : PageModel
    {

        public Category categorybilgi = new Category();


        public void OnGet()
        {


            string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;" +
            "Integrated Security=true;TrustServerCertificate=true;";


            string ID = Request.Query["ID"];


            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();


                string sql =
                "select * from Categories where ID=@ID";


                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);


                    SqlDataReader reader = command.ExecuteReader();


                    if (reader.Read())
                    {

                        categorybilgi.ID =
                        reader.GetInt32(0).ToString();


                        categorybilgi.Name =
                        reader.GetString(1);

                    }

                }

            }


        }



        public void OnPost()
        {


            categorybilgi.ID = Request.Form["ID"];

            categorybilgi.Name = Request.Form["Name"];



            string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;" +
            "Integrated Security=true;TrustServerCertificate=true;";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();


                string sql =
                "update Categories set Name=@Name where ID=@ID";


                using (SqlCommand command = new SqlCommand(sql, connection))
                {


                    command.Parameters.AddWithValue("@Name",
                    categorybilgi.Name);


                    command.Parameters.AddWithValue("@ID",
                    categorybilgi.ID);


                    command.ExecuteNonQuery();

                }

            }


            Response.Redirect("/Categories/Index");


        }


    }
}
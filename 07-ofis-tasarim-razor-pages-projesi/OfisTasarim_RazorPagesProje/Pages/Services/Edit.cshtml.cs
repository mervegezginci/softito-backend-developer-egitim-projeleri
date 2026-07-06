using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfisTasarim_RazorPagesProje.Pages.Services
{

    public class EditModel : PageModel
    {


        public Service servicebilgi = new Service();


        public string errorMessage = "";

        public string successMessage = "";



        public void OnGet()
        {

            string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;" +
            "Integrated Security=true;TrustServerCertificate=true;";


            string ID = Request.Query["ID"];


            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();


                    string sql = "select * from Services where ID=@ID";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {


                        command.Parameters.AddWithValue("@ID", ID);


                        SqlDataReader reader = command.ExecuteReader();


                        if (reader.Read())
                        {

                            servicebilgi.ID = "" + reader.GetInt32(0);

                            servicebilgi.Title = reader.GetString(1);

                            servicebilgi.Description = reader.GetString(2);

                            servicebilgi.IconUrl = reader.GetString(3);


                        }

                    }

                }

            }

            catch
            {

            }


        }




        public void OnPost()
        {


            servicebilgi.ID = Request.Form["ID"];

            servicebilgi.Title = Request.Form["Title"];

            servicebilgi.Description = Request.Form["Description"];

            servicebilgi.IconUrl = Request.Form["IconUrl"];



            if (servicebilgi.Title.Length == 0 ||
            servicebilgi.Description.Length == 0 ||
            servicebilgi.IconUrl.Length == 0)
            {

                errorMessage = "Tüm alanlar zorunlu";

                return;

            }



            try
            {

                string connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;" +
                "Integrated Security=true;TrustServerCertificate=true;";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();


                    string sql =
                    "update Services set Title=@Title,Description=@Description," +
                    "IconUrl=@IconUrl where ID=@ID";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {


                        command.Parameters.AddWithValue("@Title", servicebilgi.Title);

                        command.Parameters.AddWithValue("@Description", servicebilgi.Description);

                        command.Parameters.AddWithValue("@IconUrl", servicebilgi.IconUrl);

                        command.Parameters.AddWithValue("@ID", servicebilgi.ID);


                        command.ExecuteNonQuery();


                    }

                }

            }

            catch (Exception ex)
            {

                errorMessage = ex.Message;

                return;

            }


            successMessage = "Kayýt baþarýlý";


            Response.Redirect("/Services/Index");


        }


    }

}
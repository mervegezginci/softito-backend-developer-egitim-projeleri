using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfisTasarim_RazorPagesProje.Pages.Services
{
    public class DeleteModel : PageModel
    {

        public Service servicebilgi = new Service();


        public void OnGet()
        {

            string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;" +
            "Integrated Security=true;TrustServerCertificate=true;";


            string ID = Request.Query["ID"];


            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();


                string sql = "delete from Services where ID=@ID";


                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);

                    command.ExecuteNonQuery();

                }

            }


            Response.Redirect("/Services/Index");

        }
    }
}
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace OfisTasarim_RazorPagesProje.Pages.Categories
{
    public class DeleteModel : PageModel
    {

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
                "delete from Categories where ID=@ID";


                using (SqlCommand command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@ID", ID);

                    command.ExecuteNonQuery();

                }

            }


            Response.Redirect("/Categories/Index");

        }

    }
}
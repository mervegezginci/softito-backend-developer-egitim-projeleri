using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Projects
{
    public class DeleteModel : PageModel
    {
        private readonly string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

        public void OnGet()
        {
            string id = Request.Query["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Projects WHERE ID = @ID", connection))
                    {
                        command.Parameters.AddWithValue("@ID", Convert.ToInt32(id));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // delete error ignored
            }

            Response.Redirect("/Projects/Index");
        }
    }
}

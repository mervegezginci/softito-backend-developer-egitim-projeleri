using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OfisTasarim_RazorPagesProje.Pages.Services
{
    public class CreateModel : PageModel
    {
        public Service servicebilgi = new Service();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {

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
                errorMessage = "Tüm alanlar zorunludur";
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

                    string Sql =
                    "insert into Services(Title,Description,IconUrl)" +
                    "values(@Title,@Description,@IconUrl)";

                    using (SqlCommand command = new SqlCommand(Sql, connection))
                    {
                        command.Parameters.AddWithValue("@Title",
                            servicebilgi.Title);

                        command.Parameters.AddWithValue("@Description",
                            servicebilgi.Description);

                        command.Parameters.AddWithValue("@IconUrl",
                            servicebilgi.IconUrl);

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
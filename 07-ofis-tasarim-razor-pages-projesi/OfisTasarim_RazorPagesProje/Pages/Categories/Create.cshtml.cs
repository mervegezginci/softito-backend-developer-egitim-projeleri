using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace OfisTasarim_RazorPagesProje.Pages.Categories
{
    public class CreateModel : PageModel
    {

        public Category categorybilgi = new Category();

        public string errorMessage = "";

        public string successMessage = "";


        public void OnGet()
        {

        }


        public void OnPost()
        {

            categorybilgi.Name = Request.Form["Name"];


            if (categorybilgi.Name.Length == 0)
            {
                errorMessage = "Alan zorunlu";
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
                    "insert into Categories(Name) values(@Name)";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@Name",
                        categorybilgi.Name);


                        command.ExecuteNonQuery();

                    }

                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            Response.Redirect("/Categories/Index");

        }

    }
}
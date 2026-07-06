using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace OfisTasarim_RazorPagesProje.Pages.Contacts
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
                "delete from Contacts where ID=@ID";



                using (SqlCommand command = new SqlCommand(sql, connection))
                {


                    command.Parameters.AddWithValue("@ID", ID);


                    command.ExecuteNonQuery();


                }


            }


            Response.Redirect("/Contacts/Index");


        }

    }
}
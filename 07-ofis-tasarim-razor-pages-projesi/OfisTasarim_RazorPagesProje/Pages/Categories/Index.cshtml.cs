using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Categories
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Category> listele { get; set; } = new List<Category>();

        public string SearchTerm { get; set; } = "";

        public void OnGet(string search)
        {
            SearchTerm = search ?? "";
            string connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Categories";
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    sql += " WHERE Name LIKE @Search";
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (!string.IsNullOrEmpty(SearchTerm))
                    {
                        command.Parameters.AddWithValue("@Search", "%" + SearchTerm + "%");
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category kategori = new Category
                            {
                                ID = reader.GetInt32(0).ToString(),
                                Name = reader.GetString(1)
                            };
                            listele.Add(kategori);
                        }
                    }
                }
            }
        }
    }
}

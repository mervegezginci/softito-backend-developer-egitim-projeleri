using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Contacts
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Contact> listele { get; set; } = new List<Contact>();

        public string SearchTerm { get; set; } = "";

        public void OnGet(string search)
        {
            SearchTerm = search ?? "";
            string connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Contacts";
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    sql += " WHERE Name LIKE @Search OR Email LIKE @Search OR Message LIKE @Search";
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
                            Contact mesaj = new Contact
                            {
                                ID = reader.GetInt32(0).ToString(),
                                Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Message = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Date = reader.IsDBNull(4) ? "" : reader.GetString(4)
                            };

                            listele.Add(mesaj);
                        }
                    }
                }
            }
        }
    }
}

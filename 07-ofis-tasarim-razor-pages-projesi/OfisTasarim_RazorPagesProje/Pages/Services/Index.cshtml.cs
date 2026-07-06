using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Services
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Service> listele { get; set; } = new List<Service>();

        public string SearchTerm { get; set; } = "";

        public void OnGet(string search)
        {
            SearchTerm = search ?? "";
            string connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Services";
                    if (!string.IsNullOrEmpty(SearchTerm))
                    {
                        sql += " WHERE Title LIKE @Search OR Description LIKE @Search";
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
                                Service service = new Service
                                {
                                    ID = reader.GetInt32(0).ToString(),
                                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    IconUrl = reader.IsDBNull(3) ? "" : reader.GetString(3)
                                };
                                listele.Add(service);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Error handled gracefully
            }
        }
    }
}

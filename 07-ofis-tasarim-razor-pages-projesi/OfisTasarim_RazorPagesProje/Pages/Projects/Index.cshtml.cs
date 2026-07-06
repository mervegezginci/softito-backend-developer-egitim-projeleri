using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Projects
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Project> Listele { get; set; } = new List<Project>();

        public string SearchTerm { get; set; } = "";

        private readonly string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

        public void OnGet(string search)
        {
            SearchTerm = search ?? "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT p.ID, p.Title, p.Description, p.ImageUrl, p.CategoryId, c.Name AS CategoryName
                        FROM Projects p
                        LEFT JOIN Categories c ON p.CategoryId = c.ID";

                    if (!string.IsNullOrEmpty(SearchTerm))
                    {
                        sql += " WHERE p.Title LIKE @Search OR p.Description LIKE @Search OR c.Name LIKE @Search";
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
                                Project project = new Project
                                {
                                    ID = reader.GetInt32(0).ToString(),
                                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    ImageUrl = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    CategoryId = reader.IsDBNull(4) ? "" : reader.GetInt32(4).ToString(),
                                    CategoryName = reader.IsDBNull(5) ? "Kategorisiz" : reader.GetString(5)
                                };

                                Listele.Add(project);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // load fail ignored
            }
        }
    }
}

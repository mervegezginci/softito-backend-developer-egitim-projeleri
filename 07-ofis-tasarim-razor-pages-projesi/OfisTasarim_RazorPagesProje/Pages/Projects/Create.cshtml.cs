using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfisTasarim_RazorPagesProje.Pages.Categories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Projects
{
    public class CreateModel : PageModel
    {
        public Project ProjectBilgi = new Project();
        public List<Category> Categories { get; set; } = new List<Category>();
        public string ErrorMessage = "";

        private readonly string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

        public void OnGet()
        {
            LoadCategories();
        }

        public void OnPost()
        {
            ProjectBilgi.Title = Request.Form["Title"];
            ProjectBilgi.Description = Request.Form["Description"];
            ProjectBilgi.ImageUrl = Request.Form["ImageUrl"];
            ProjectBilgi.CategoryId = Request.Form["CategoryId"];

            LoadCategories();

            if (string.IsNullOrEmpty(ProjectBilgi.Title) ||
                string.IsNullOrEmpty(ProjectBilgi.Description) ||
                string.IsNullOrEmpty(ProjectBilgi.ImageUrl) ||
                string.IsNullOrEmpty(ProjectBilgi.CategoryId))
            {
                ErrorMessage = "Tüm alanlar zorunludur.";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO Projects(Title, Description, ImageUrl, CategoryId) VALUES(@Title, @Description, @ImageUrl, @CategoryId)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Title", ProjectBilgi.Title);
                        command.Parameters.AddWithValue("@Description", ProjectBilgi.Description);
                        command.Parameters.AddWithValue("@ImageUrl", ProjectBilgi.ImageUrl);
                        command.Parameters.AddWithValue("@CategoryId", Convert.ToInt32(ProjectBilgi.CategoryId));

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Projects/Index");
        }

        private void LoadCategories()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT ID, Name FROM Categories", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Categories.Add(new Category
                                {
                                    ID = reader.GetInt32(0).ToString(),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Categories load fail is ignored
            }
        }
    }
}

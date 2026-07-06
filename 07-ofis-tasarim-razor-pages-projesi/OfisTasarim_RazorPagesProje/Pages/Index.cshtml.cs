using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfisTasarim_RazorPagesProje.Pages.Services;
using OfisTasarim_RazorPagesProje.Pages.Projects;
using OfisTasarim_RazorPagesProje.Pages.Contacts;
using OfisTasarim_RazorPagesProje.Pages.Categories;

namespace OfisTasarim_RazorPagesProje.Pages
{
    public class IndexModel : PageModel
    {
        public List<Service> Services = new();
        public List<Project> Projects = new();
        public List<Contact> Contacts = new();
        public List<Category> Categories = new();

        string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;" +
            "Integrated Security=true;TrustServerCertificate=true;";

        public void OnGet()
        {
            GetServices();
            GetProjects();
            GetCategories();
        }

        private void GetServices()
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            connection.Open();

            string sql = "SELECT * FROM Services";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                Service service = new Service();

                service.ID = reader["ID"].ToString();
                service.Title = reader["Title"].ToString();
                service.Description = reader["Description"].ToString();
                service.IconUrl = reader["IconUrl"].ToString();

                Services.Add(service);
            }
        }

        private void GetProjects()
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            connection.Open();

            string sql = "SELECT * FROM Projects";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                Project project = new Project();

                project.ID = reader["ID"].ToString();
                project.Title = reader["Title"].ToString();
                project.Description = reader["Description"].ToString();
                project.ImageUrl = reader["ImageUrl"].ToString();
                project.CategoryId = reader["CategoryId"].ToString();

                Projects.Add(project);
            }
        }

        private void GetCategories()
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            connection.Open();

            string sql = "SELECT * FROM Categories";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                Category category = new Category();

                category.ID = reader["ID"].ToString();
                category.Name = reader["Name"].ToString();

                Categories.Add(category);
            }
        }
    }
}
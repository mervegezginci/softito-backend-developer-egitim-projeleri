using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfisTasarim_RazorPagesProje.Pages.Contacts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace OfisTasarim_RazorPagesProje.Pages.Admin
{
    public class ReportItem
    {
        public string Bilgi { get; set; } = "";
        public string Deger { get; set; } = "";
    }

    public class IndexModel : PageModel
    {
        public int TotalCategories { get; set; }
        public int TotalServices { get; set; }
        public int TotalProjects { get; set; }
        public int TotalContacts { get; set; }

        public class CategoryStat
        {
            public string CategoryName { get; set; } = "";
            public int ProjectCount { get; set; }
        }

        public List<CategoryStat> CategoryProjectStats { get; set; } = new List<CategoryStat>();
        public List<Contact> RecentContacts { get; set; } = new List<Contact>();

        private readonly string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=OfisDB;Integrated Security=true;TrustServerCertificate=true;";

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Categories Count
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Categories", connection))
                    {
                        TotalCategories = (int)cmd.ExecuteScalar();
                    }

                    // Services Count
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Services", connection))
                    {
                        TotalServices = (int)cmd.ExecuteScalar();
                    }

                    // Projects Count
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Projects", connection))
                    {
                        TotalProjects = (int)cmd.ExecuteScalar();
                    }

                    // Contacts Count
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Contacts", connection))
                    {
                        TotalContacts = (int)cmd.ExecuteScalar();
                    }

                    // Kategori Proje İstatistikleri (Grafik için)
                    string statsSql = @"
                        SELECT c.Name, COUNT(p.ID) AS ProjectCount
                        FROM Categories c
                        LEFT JOIN Projects p ON c.ID = p.CategoryId
                        GROUP BY c.Name";

                    using (SqlCommand cmd = new SqlCommand(statsSql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CategoryProjectStats.Add(new CategoryStat
                                {
                                    CategoryName = reader.GetString(0),
                                    ProjectCount = reader.GetInt32(1)
                                });
                            }
                        }
                    }

                    // Son 5 İletişim Mesajı
                    string contactsSql = "SELECT TOP 5 ID, Name, Email, Message, Date FROM Contacts ORDER BY ID DESC";
                    using (SqlCommand cmd = new SqlCommand(contactsSql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RecentContacts.Add(new Contact
                                {
                                    ID = reader.GetInt32(0).ToString(),
                                    Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Message = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Date = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // varsayılan boş kalır
            }
        }

        public JsonResult OnGetReportData(string reportType)
        {
            var selectedReportType = reportType ?? "proje_sayilari_kategori";
            var ajaxReportData = new List<ReportItem>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string reportSql = "";

                    switch (selectedReportType)
                    {
                        case "proje_sayilari_kategori":
                            reportSql = @"
                                SELECT c.Name, COUNT(p.ID) 
                                FROM Categories c 
                                LEFT JOIN Projects p ON c.ID = p.CategoryId 
                                GROUP BY c.Name";
                            using (SqlCommand cmd = new SqlCommand(reportSql, connection))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        ajaxReportData.Add(new ReportItem
                                        {
                                            Bilgi = reader.GetString(0),
                                            Deger = reader.GetInt32(1) + " Adet Proje"
                                        });
                                    }
                                }
                            }
                            break;

                        case "hizmet_aciklama_uzunluk":
                            reportSql = "SELECT Title, LEN(Description) FROM Services";
                            using (SqlCommand cmd = new SqlCommand(reportSql, connection))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        ajaxReportData.Add(new ReportItem
                                        {
                                            Bilgi = reader.GetString(0),
                                            Deger = (reader.IsDBNull(1) ? 0 : reader.GetInt32(1)) + " Karakter"
                                        });
                                    }
                                }
                            }
                            break;

                        case "son_mesajlar_karakter":
                            reportSql = "SELECT Name + ' (' + Email + ')', LEN(Message) FROM Contacts";
                            using (SqlCommand cmd = new SqlCommand(reportSql, connection))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        ajaxReportData.Add(new ReportItem
                                        {
                                            Bilgi = reader.IsDBNull(0) ? "İsimsiz" : reader.GetString(0),
                                            Deger = (reader.IsDBNull(1) ? 0 : reader.GetInt32(1)) + " Karakter"
                                        });
                                    }
                                }
                            }
                            break;

                        case "ofis_gecen_projeler":
                            reportSql = "SELECT Title, CategoryId FROM Projects WHERE Title LIKE '%Ofis%'";
                            using (SqlCommand cmd = new SqlCommand(reportSql, connection))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        ajaxReportData.Add(new ReportItem
                                        {
                                            Bilgi = reader.GetString(0),
                                            Deger = "Kategori ID: " + (reader.IsDBNull(1) ? "Yok" : reader.GetInt32(1).ToString())
                                        });
                                    }
                                }
                            }
                            break;

                        case "unsplash_gorselli_projeler":
                            reportSql = "SELECT Title, ImageUrl FROM Projects WHERE ImageUrl LIKE '%unsplash%'";
                            using (SqlCommand cmd = new SqlCommand(reportSql, connection))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string img = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                        if (img.Length > 45) img = img.Substring(0, 45) + "...";
                                        ajaxReportData.Add(new ReportItem
                                        {
                                            Bilgi = reader.GetString(0),
                                            Deger = img
                                        });
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // varsayılan olarak boş döner
            }

            return new JsonResult(ajaxReportData);
        }
    }
}

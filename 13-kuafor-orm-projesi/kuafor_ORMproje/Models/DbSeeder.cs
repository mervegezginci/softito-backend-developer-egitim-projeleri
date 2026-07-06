using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace kuafor_ORMproje.Models
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // 1. Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 2. Seed Admin User
            var adminEmail = "merve@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    Name = "Merve",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "123456Zz.");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 3. Seed Services if empty
            if (!await context.Services.AnyAsync())
            {
                var services = new List<Service>
                {
                    new Service { ServiceName = "Saç Kesimi & Yıkama", Price = 350, Duration = 45, Description = "Modern model kesim ve yıkama hizmeti.", ImageUrl = "/img/default-service.jpg" },
                    new Service { ServiceName = "Sakal Tasarımı", Price = 150, Duration = 20, Description = "Sakal şekillendirme ve bakım yağları uygulaması.", ImageUrl = "/img/default-service.jpg" },
                    new Service { ServiceName = "Saç Boyama (Renklendirme)", Price = 800, Duration = 90, Description = "Profesyonel renklendirme ve dip boyası.", ImageUrl = "/img/default-service.jpg" },
                    new Service { ServiceName = "Cilt Bakımı & Maske", Price = 400, Duration = 30, Description = "Siyah nokta temizleme ve canlandırıcı maske.", ImageUrl = "/img/default-service.jpg" },
                    new Service { ServiceName = "Fön & Şekillendirme", Price = 120, Duration = 15, Description = "Günlük ve özel günler için fön hizmeti.", ImageUrl = "/img/default-service.jpg" }
                };
                await context.Services.AddRangeAsync(services);
                await context.SaveChangesAsync();
            }

            // 4. Seed Employees if empty
            if (!await context.Employees.AnyAsync())
            {
                var employees = new List<Employee>
                {
                    new Employee { FullName = "Ahmet Yılmaz", Speciality = "Erkek Saç Tasarım & Sakal", Phone = "0532 111 2233", IsActive = true, ImageUrl = "/img/default-employee.jpg" },
                    new Employee { FullName = "Elif Demir", Speciality = "Bayan Saç Kesim & Boyama", Phone = "0533 222 3344", IsActive = true, ImageUrl = "/img/default-employee.jpg" },
                    new Employee { FullName = "Can Kaya", Speciality = "Cilt Bakımı & Masaj", Phone = "0535 333 4455", IsActive = true, ImageUrl = "/img/default-employee.jpg" }
                };
                await context.Employees.AddRangeAsync(employees);
                await context.SaveChangesAsync();
            }

            // 5. Seed Customers if empty
            if (!await context.Customers.AnyAsync())
            {
                var customers = new List<Customer>
                {
                    new Customer { FullName = "Mustafa Şen", Phone = "0542 444 5566", Email = "mustafa@gmail.com", Gender = "Erkek", CreatedDate = DateTime.Now.AddDays(-15) },
                    new Customer { FullName = "Ayşe Kaya", Phone = "0543 555 6677", Email = "ayse@gmail.com", Gender = "Kadın", CreatedDate = DateTime.Now.AddDays(-10) },
                    new Customer { FullName = "Mehmet Öztürk", Phone = "0544 666 7788", Email = "mehmet@gmail.com", Gender = "Erkek", CreatedDate = DateTime.Now.AddDays(-7) },
                    new Customer { FullName = "Zeynep Çelik", Phone = "0545 777 8899", Email = "zeynep@gmail.com", Gender = "Kadın", CreatedDate = DateTime.Now.AddDays(-5) }
                };
                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync();
            }

            // 6. Seed Appointments & Payments if empty
            if (!await context.Appointments.AnyAsync())
            {
                var customers = await context.Customers.ToListAsync();
                var employees = await context.Employees.ToListAsync();
                var services = await context.Services.ToListAsync();

                var appointments = new List<Appointment>();
                var payments = new List<Payment>();

                var random = new Random();
                var today = DateTime.Today;

                // Let's seed for the last 7 days to populate the charts beautifully!
                for (int i = 0; i < 7; i++)
                {
                    var date = today.AddDays(-i);
                    // Add 1 or 2 appointments for each day
                    int appCount = random.Next(1, 3);
                    for (int j = 0; j < appCount; j++)
                    {
                        var customer = customers[random.Next(customers.Count)];
                        var employee = employees[random.Next(employees.Count)];
                        var service = services[random.Next(services.Count)];

                        var appointment = new Appointment
                        {
                            CustomerId = customer.Id,
                            EmployeeId = employee.Id,
                            ServiceId = service.Id,
                            AppointmentDate = date,
                            AppointmentTime = new TimeSpan(random.Next(9, 19), 0, 0),
                            Status = "Tamamlandı",
                            Note = "Örnek randevu kaydı."
                        };

                        appointments.Add(appointment);
                    }
                }

                // Add appointments to DB first to generate IDs
                await context.Appointments.AddRangeAsync(appointments);
                await context.SaveChangesAsync();

                // Create matching payments
                foreach (var app in appointments)
                {
                    var service = services.First(s => s.Id == app.ServiceId);
                    var payment = new Payment
                    {
                        AppointmentId = app.Id,
                        Amount = service.Price,
                        PaymentMethod = random.Next(2) == 0 ? "Kredi Kartı" : "Nakit",
                        PaymentDate = app.AppointmentDate.AddHours(1),
                        PaymentStatus = true
                    };
                    payments.Add(payment);
                }

                await context.Payments.AddRangeAsync(payments);
                await context.SaveChangesAsync();
            }
        }
    }
}

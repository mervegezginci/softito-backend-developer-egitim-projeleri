using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ogrenciyonetimi_mvc.Models;

namespace ogrenciyonetimi_mvc.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient client, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _client.BaseAddress = new Uri(config["ApiBaseUrl"] ?? "https://localhost:7101/");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthHeader()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var token = user?.FindFirst("JWToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<(bool Success, string Message, LoginResponse? Data)> LoginAsync(LoginViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/auth/login", content);
            if (!response.IsSuccessStatusCode)
            {
                return (false, "E-posta veya şifre hatalı.", null);
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return (true, "Giriş başarılı.", data);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/auth/register", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrEmpty(errorMsg) ? "Kayıt işlemi başarısız." : errorMsg);
            }
            return (true, "Kayıt başarılı.");
        }

        public async Task<IEnumerable<StudentViewModel>> GetStudentsAsync(string? query = null, int? deptId = null)
        {
            AddAuthHeader();
            string url = "api/students";
            if (!string.IsNullOrEmpty(query) || deptId.HasValue)
            {
                url += $"/search?q={Uri.EscapeDataString(query ?? "")}&deptId={deptId}";
            }

            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<StudentViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<StudentViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Enumerable.Empty<StudentViewModel>();
        }

        public async Task<StudentViewModel?> GetStudentByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/students/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StudentViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateStudentAsync(StudentViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/students", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStudentAsync(int id, StudentViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/students/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/students/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<DepartmentViewModel>> GetDepartmentsAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/departments");
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<DepartmentViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DepartmentViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Enumerable.Empty<DepartmentViewModel>();
        }

        public async Task<DepartmentViewModel?> GetDepartmentByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/departments/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DepartmentViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateDepartmentAsync(DepartmentViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/departments", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDepartmentAsync(int id, DepartmentViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/departments/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/departments/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<GradeViewModel>> GetGradesAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/grades");
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<GradeViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<GradeViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Enumerable.Empty<GradeViewModel>();
        }

        public async Task<GradeViewModel?> GetGradeByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/grades/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GradeViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<IEnumerable<GradeViewModel>> GetGradesByStudentIdAsync(int studentId)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/grades/student/{studentId}");
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<GradeViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<GradeViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Enumerable.Empty<GradeViewModel>();
        }

        public async Task<bool> CreateGradeAsync(GradeViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/grades", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateGradeAsync(int id, GradeViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/grades/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteGradeAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/grades/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]?> DownloadPdfAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/students/export/pdf");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]?> DownloadExcelAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/students/export/excel");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}

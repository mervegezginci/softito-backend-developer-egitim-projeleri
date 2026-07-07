using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sporkulubu_mvc.Models;

namespace sporkulubu_mvc.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient client, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _client.BaseAddress = new Uri(config["ApiBaseUrl"] ?? "https://localhost:7102/"); // Port offset
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

        // --- Auth ---

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

        // --- Branches (Spor Branşları) ---

        public async Task<IEnumerable<BranchViewModel>> GetBranchesAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/branches");
            if (!response.IsSuccessStatusCode) return Array.Empty<BranchViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<BranchViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Array.Empty<BranchViewModel>();
        }

        public async Task<BranchViewModel?> GetBranchByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/branches/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BranchViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateBranchAsync(BranchViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/branches", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBranchAsync(int id, BranchViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/branches/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/branches/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Coaches (Antrenörler) ---

        public async Task<IEnumerable<CoachViewModel>> GetCoachesAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/coaches");
            if (!response.IsSuccessStatusCode) return Array.Empty<CoachViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<CoachViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Array.Empty<CoachViewModel>();
        }

        public async Task<CoachViewModel?> GetCoachByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/coaches/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CoachViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateCoachAsync(CoachViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/coaches", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCoachAsync(int id, CoachViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/coaches/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCoachAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/coaches/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Members (Üyeler) ---

        public async Task<IEnumerable<MemberViewModel>> GetMembersAsync(string? query = null, bool? isActive = null)
        {
            AddAuthHeader();
            string url = "api/members";
            if (!string.IsNullOrEmpty(query) || isActive.HasValue)
            {
                url += $"/search?q={Uri.EscapeDataString(query ?? "")}";
                if (isActive.HasValue) url += $"&isActive={isActive.Value.ToString().ToLower()}";
            }

            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Array.Empty<MemberViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<MemberViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Array.Empty<MemberViewModel>();
        }

        public async Task<MemberViewModel?> GetMemberByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/members/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MemberViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateMemberAsync(MemberViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/members", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMemberAsync(int id, MemberViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/members/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMemberAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/members/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Trainings (Antrenman Kayıtları) ---

        public async Task<IEnumerable<TrainingViewModel>> GetTrainingsAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/trainings");
            if (!response.IsSuccessStatusCode) return Array.Empty<TrainingViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<TrainingViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? Array.Empty<TrainingViewModel>();
        }

        public async Task<TrainingViewModel?> GetTrainingByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/trainings/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TrainingViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateTrainingAsync(TrainingViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/trainings", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTrainingAsync(int id, TrainingViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/trainings/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTrainingAsync(int id)
        {
            AddAuthHeader();
            var response = await _client.DeleteAsync($"api/trainings/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- Exports ---

        public async Task<byte[]?> DownloadPdfAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/members/export/pdf");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]?> DownloadExcelAsync()
        {
            AddAuthHeader();
            var response = await _client.GetAsync("api/members/export/excel");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }

        // --- Dynamic Reports ---

        public async Task<string> GetReportJsonAsync(string reportType)
        {
            AddAuthHeader();
            var response = await _client.GetAsync($"api/reports/{reportType}");
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }
    }
}

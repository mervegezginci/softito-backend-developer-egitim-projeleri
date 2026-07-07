using System;

namespace sporkulubu_proje.Models
{
    // --- Database Entities ---

    public class SportsBranch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class Coach
    {
        public int CoachId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int BranchId { get; set; }
        public string? BranchName { get; set; } // Join result
    }

    public class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class Training
    {
        public int TrainingId { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; } // Join result
        public int CoachId { get; set; }
        public string? CoachName { get; set; } // Join result
        public DateTime TrainingDate { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Fee { get; set; }
        public string? BranchName { get; set; } // Join result
    }

    // --- DTOs (Data Transfer Objects) ---

    public class CoachDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int BranchId { get; set; }
    }

    public class MemberDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TrainingDto
    {
        public int MemberId { get; set; }
        public int CoachId { get; set; }
        public DateTime TrainingDate { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Fee { get; set; }
    }

    // --- Auth DTOs ---

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

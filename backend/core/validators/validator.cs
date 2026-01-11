using System.Text.RegularExpressions;

namespace GymManagement.Core.Validators
{
    public static class EmailValidator
    {
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Simple regex for email validation
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }

    public static class PasswordValidator
    {
        public static bool IsStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            // Optional: check for special characters
            // if (!password.Any(ch => !char.IsLetterOrDigit(ch))) return false;

            return true;
        }
    }
}


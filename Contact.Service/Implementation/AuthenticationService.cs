using Contact.Domain.Entities;
using Contact.Service.Abstracts;
using Contact.Shared.Bases;
using Microsoft.AspNetCore.Identity;

namespace Contact.Service.Implementation
{
    internal class AuthenticationService : ReturnBase, IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        public AuthenticationService(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<ReturnBase<bool>> RegisterUserAsync(User user, string password)
        {
            try
            {
                var validateUserResult = await ValidateUserAsync(user, password);

                if (!validateUserResult.Succeeded)
                    return Failed<bool>(validateUserResult.Message);

                var createUser = await _userManager.CreateAsync(user, password);

                if (createUser.Succeeded)
                {
                    if (user.Email == "mazenabdelgawad700@gmail.com")
                        await _userManager.AddToRoleAsync(user, "Admin");
                    else
                        await _userManager.AddToRoleAsync(user, "User");

                    return Success<bool>(true);
                }

                return Failed<bool>("Failed to register user, please try again");
            }
            catch (Exception ex)
            {
                return Failed<bool>(ex.InnerException.Message);
            }
        }
        private async Task<ReturnBase<bool>> ValidateUserAsync(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
                return Failed<bool>("Username is required");

            if (user.UserName.Length < 3 || user.UserName.Length > 50)
                return Failed<bool>("Username must be between 3 and 50 characters");

            if (!System.Text.RegularExpressions.Regex.IsMatch(user.UserName, @"^[a-zA-Z0-9_-]+$"))
                return Failed<bool>("Username can only contain letters, numbers, underscores, and hyphens");

            if (string.IsNullOrWhiteSpace(user.Email))
                return Failed<bool>("Email address is required");

            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(user.Email, emailPattern))
                return Failed<bool>("Please enter a valid email address");

            if (string.IsNullOrWhiteSpace(password))
                return Failed<bool>("Password is required");

            if (password.Length < 8)
                return Failed<bool>("Password must be at least 8 characters long");

            if (password.Length > 100)
                return Failed<bool>("Password cannot exceed 100 characters");

            var passwordValidation = ValidatePasswordStrength(password);
            if (!passwordValidation.Succeeded)
                return Failed<bool>(passwordValidation.Message);

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                return Failed<bool>("Phone number is required");

            var phoneValidation = ValidatePhoneNumber(user.PhoneNumber);
            if (!phoneValidation.Succeeded)
                return Failed<bool>(phoneValidation.Message);

            var existingUserByEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingUserByEmail != null)
                return Failed<bool>("Email address is already registered");


            return Success(true);
        }
        private ReturnBase<bool> ValidatePasswordStrength(string password)
        {
            var hasUpperCase = System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]");
            var hasLowerCase = System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]");
            var hasDigit = System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]");
            var hasSpecialChar = System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

            if (!hasUpperCase)
                return Failed<bool>("Password must contain at least one uppercase letter");

            if (!hasLowerCase)
                return Failed<bool>("Password must contain at least one lowercase letter");

            if (!hasDigit)
                return Failed<bool>("Password must contain at least one number");

            if (!hasSpecialChar)
                return Failed<bool>("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;':\"\\,.<>?/)");

            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"(.)\1{2,}"))
                return Failed<bool>("Password cannot contain the same character repeated more than twice");

            if (ContainsSequentialCharacters(password))
                return Failed<bool>("Password cannot contain sequential characters (like 123, abc, etc.)");

            return Success(true);
        }
        private ReturnBase<bool> ValidatePhoneNumber(string phoneNumber)
        {
            var digitsOnly = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

            if (string.IsNullOrEmpty(digitsOnly))
                return Failed<bool>("Phone number must contain digits");

            if (digitsOnly.Length < 10)
                return Failed<bool>("Phone number is too short. Please include country code");

            if (digitsOnly.Length > 15)
                return Failed<bool>("Phone number is too long");

            var phonePattern = @"^\+?[1-9]\d{1,14}$";

            if (phoneNumber.StartsWith("+"))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(digitsOnly, phonePattern.Replace("^\\+?", "^")))
                    return Failed<bool>("Invalid phone number format");
            }
            else
            {
                if (digitsOnly.Length < 11)
                    return Failed<bool>("Phone number must include country code (e.g., +1 for US, +44 for UK)");
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(digitsOnly, @"^(\d)\1{9,}$")) // All same digits
                return Failed<bool>("Phone number cannot consist of the same digit repeated");

            return Success(true);
        }
        private bool ContainsSequentialCharacters(string input)
        {
            for (int i = 0; i < input.Length - 2; i++)
            {
                if (input[i] + 1 == input[i + 1] && input[i + 1] + 1 == input[i + 2])
                    return true;

                if (input[i] - 1 == input[i + 1] && input[i + 1] - 1 == input[i + 2])
                    return true;
            }
            return false;
        }
    }
}

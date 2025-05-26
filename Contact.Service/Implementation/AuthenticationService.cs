using Contact.Domain.Entities;
using Contact.Domain.Helpers;
using Contact.Infrastructure.Context;
using Contact.Service.Abstracts;
using Contact.Shared.Bases;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Contact.Service.Implementation
{
    internal class AuthenticationService : ReturnBase, IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfirmEmailService _confirmEmailService;
        private readonly JwtSettings _jwtSettings;
        private readonly AppDbContext _dbContext;


        public AuthenticationService(UserManager<User> userManager, IConfirmEmailService confirmEmailService, SignInManager<User> signInManager, JwtSettings jwtSettings, AppDbContext dbContext)
        {
            this._userManager = userManager;
            this._confirmEmailService = confirmEmailService;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            _dbContext = dbContext;
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

                    var sendConfirmationEmailResult = await _confirmEmailService.SendConfirmationEmailAsync(user);

                    while (!sendConfirmationEmailResult.Succeeded)
                        sendConfirmationEmailResult = await _confirmEmailService.SendConfirmationEmailAsync(user);

                    return Success<bool>(true, "User registerd successfully, please confirm your email address");
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
        public async Task<ReturnBase<string>> LoginAsync(string email, string password, bool rememberMe)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                    return Failed<string>("Please, enter email and password");

                var user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                    return Failed<string>("Invalid email or password");


                var ifUserLockedOut = await _userManager.IsLockedOutAsync(user);

                if (ifUserLockedOut)
                    return Failed<string>("Your account is locked due to multiple unsuccessful login attempts. Please try again later or contact support.");

                var passwordCheck = await _userManager.CheckPasswordAsync(user, password);

                if (!passwordCheck)
                {
                    await _userManager.AccessFailedAsync(user);

                    var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                    var maxFailedAccessAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                    var attemptsLeft = maxFailedAccessAttempts - accessFailedCount;

                    if (accessFailedCount != 0)
                        return Failed<string>($"Invalid login attempt. You have {attemptsLeft} more {(attemptsLeft == 1 ? "attempt" : "attempts")} before your account gets locked.");
                    else
                        return Failed<string>("Your account is locked due to multiple unsuccessful login attempts. Please try again later or contact support.");
                }


                if (!user.EmailConfirmed)
                {
                    var sendConfirmationEmailResult = await _confirmEmailService.SendConfirmationEmailAsync(user);

                    while (!sendConfirmationEmailResult.Succeeded)
                        sendConfirmationEmailResult = await _confirmEmailService.SendConfirmationEmailAsync(user);

                    return Failed<string>("Please, confirm your email address to login");
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

                if (!result.Succeeded)
                    return Failed<string>("Can not Logged in. Please, try again later");

                string jwtId = Guid.NewGuid().ToString();
                string token = await GenerateJwtToken(user, jwtId);
                await BuildRefreshToken(user, jwtId);

                return Success(token, "Logged in successfully");

            }
            catch (Exception ex)
            {
                return Failed<string>(ex.InnerException.Message);
            }
        }
        private async Task<string> GenerateJwtToken(User user, string jwtId)
        {
            List<Claim> claims = await GetClaimsAsync(user, jwtId);

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwtSettings.AccessTokenExpireDate),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<List<Claim>> GetClaimsAsync(User user, string jwtId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            List<Claim> claims =
            [
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            ];
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            return claims;
        }
        private async Task BuildRefreshToken(User user, string jwtId)
        {
            RefreshToken newRefreshToken = new()
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                JwtId = jwtId,
                IsUsed = false,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(_jwtSettings.RefreshTokenExpireDate)
            };

            RefreshToken? existingRefreshTokenRecord = await _dbContext.RefreshToken
                .FirstOrDefaultAsync(rt => rt.UserId == user.Id);

            if (existingRefreshTokenRecord is null)
            {
                await _dbContext.RefreshToken.AddAsync(newRefreshToken);
            }
            else
            {
                existingRefreshTokenRecord.Token = GenerateRefreshToken();
                existingRefreshTokenRecord.CreatedAt = DateTime.UtcNow;
                existingRefreshTokenRecord.ExpiresAt = DateTime.UtcNow.AddMonths(_jwtSettings.RefreshTokenExpireDate);

                _dbContext.RefreshToken.Update(existingRefreshTokenRecord);
            }

            await _dbContext.SaveChangesAsync();
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private bool IsAccessTokenExpired(string accessToken)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new();
                if (tokenHandler.ReadToken(accessToken) is not JwtSecurityToken token)
                    return true;

                DateTimeOffset expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Exp).Value));

                return expirationTime.UtcDateTime <= DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
        public async Task<ReturnBase<string>> RefreshTokenAsync(string accessToken)
        {
            try
            {
                if (!IsAccessTokenExpired(accessToken))
                    return Success("", "Access Token Is Valid");

                string? userId = GetUserIdFromToken(accessToken);
                string? jwtId = GetJwtIdFromToken(accessToken);

                if (jwtId is null || userId is null)
                    return Failed<string>("Invalid Access Token");

                RefreshToken? storedRefreshToken = await _dbContext.RefreshToken
                    .FirstOrDefaultAsync(rt => rt.UserId.ToString() == userId && rt.JwtId == jwtId);

                if (storedRefreshToken is null || storedRefreshToken.IsRevoked)
                    return Failed<string>("Your session has expired. please log in again.");

                if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    storedRefreshToken.IsRevoked = true;
                    _dbContext.RefreshToken.Update(storedRefreshToken);
                    await _dbContext.SaveChangesAsync();
                    return Failed<string>("Your session has expired. please log in again.");
                }

                if (!storedRefreshToken.IsUsed)
                {
                    storedRefreshToken.IsUsed = true;
                    _dbContext.RefreshToken.Update(storedRefreshToken);
                }

                User? user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                    return Failed<string>("Invalid Access Token");

                string newJwtId = Guid.NewGuid().ToString();
                string newAccessToken = await GenerateJwtToken(user, newJwtId);

                storedRefreshToken.JwtId = newJwtId;

                await _dbContext.SaveChangesAsync();

                if (newAccessToken is null)
                    return Failed<string>("Failed To Generate New Access Token");

                return Success(newAccessToken, "New Access Token Created");
            }
            catch (Exception ex)
            {
                return Failed<string>(ex.InnerException.Message);
            }
        }
        private string? GetJwtIdFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
        private string? GetUserIdFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value.ToString();
        }
    }
}

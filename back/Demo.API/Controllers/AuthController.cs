using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace Demo.API.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Demo.API.Extensions;
    using Demo.API.Interfaces;
    using Demo.API.Models;
    using Demo.API.Utility;

    using FluentValidation.Results;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ICommonService _commonService;

        private readonly IEmailSender _emailSender;

        private readonly IHostingEnvironment _env;

        private readonly IConfiguration _iconfiguration;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IUserRepository _userRepository;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ICommonService commonService,
            IConfiguration iconfiguration,
            IUserRepository userRepository,
            IHostingEnvironment env)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
            this._commonService = commonService;
            this._iconfiguration = iconfiguration;
            this._userRepository = userRepository;
            this._env = env;
        }

        [HttpPost("activeuser")]
        //[Authorize(Roles = RolesNames.User)]
        public async Task<ServiceResponse<bool>> ActiveUser(string userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                if (userId == null)
                {
                    response.ErrorMessages.Add(new ValidationFailure(string.Empty, "User id not provided."));
                    return response;
                }

                var user = await this._userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, $"Unable to load user with ID '{userId}'."));
                    return response;
                }

                response.Data = await this._userRepository.UpdateUserStatus(userId, false);
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }

        [HttpGet("confirmemail")]
        [AllowAnonymous]
        public async Task<ServiceResponse<bool>> ConfirmEmail(string userId, string code)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                if (userId == null || code == null)
                {
                    response.ErrorMessages.Add(new ValidationFailure(string.Empty, "User id not provided."));
                    return response;
                }

                var user = await this._userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, $"Unable to load user with ID '{userId}'."));
                    return response;
                }

                var result = await this._userManager.ConfirmEmailAsync(user, code);
                response.Data = result.Succeeded;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }

        [HttpPost("deleteuser")]
        //[Authorize(Roles = RolesNames.SuperAdmin)]
        public async Task<ServiceResponse<bool>> DeleteUser(string userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                if (userId == null)
                {
                    response.ErrorMessages.Add(new ValidationFailure(string.Empty, "User id not provided."));
                    return response;
                }

                var user = await this._userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, $"Unable to load user with ID '{userId}'."));
                    return response;
                }

                var result = await this._userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, "Something went wrrong! please try again"));
                    return response;
                }

                response.Data = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }

        [HttpPost("disableuser")]
        //[Authorize(Roles = RolesNames.User)]
        public async Task<ServiceResponse<bool>> DisableUser(string userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                if (userId == null)
                {
                    response.ErrorMessages.Add(new ValidationFailure(string.Empty, "User id not provided."));
                    return response;
                }

                var user = await this._userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, $"Unable to load user with ID '{userId}'."));
                    return response;
                }

                response.Data = await this._userRepository.UpdateUserStatus(userId, true);
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }

        [HttpPost("forgotpassword")]
        public async Task<ServiceResponse> ForgotPassword(ForgotPasswordViewModel model)
        {
            var response = new ServiceResponse();
            if (this.ModelState.IsValid)
            {
                var validation = new ForgotPasswordViewModelValidator(model);
                var results = validation.Validate(model);

                response.ErrorMessages = results.Errors.ToList();

                if (!response.Successful) return response;

                var user = await this._userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, "Please check your email to reset your password."));
                    return response;
                }

                var code = await this._userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl =
                    $"{new Uri(this._iconfiguration["Cors:AllowedOrigin"])}/resetpassword?userid={user.Id}&code={code}";

                var forgotpasswordFile =
                    $"{this._env.WebRootPath}{Path.DirectorySeparatorChar}EmailTemplate{Path.DirectorySeparatorChar}ForgotPassword.html";
                var replaceEmailText = new
                {
                    model.Email,
                    ForgotPasswordLink = callbackUrl,
                    Logo = Convert.ToString(
                                                   $"{new Uri(this._iconfiguration["Cors:AllowedOrigin"])}{Path.DirectorySeparatorChar}assets{Path.DirectorySeparatorChar}img{Path.DirectorySeparatorChar}logo.png"),
                    Url = Convert.ToString(
                                                   new Uri(this._iconfiguration["Cors:AllowedOrigin"]))
                };
                var forgotPasswordBody = replaceEmailText.SetEmailTemplates(forgotpasswordFile.ReadFile());
                await this._emailSender.SendEmailAsync(model.Email, "Reset your Password", forgotPasswordBody);
            }
            else
            {
                response.ErrorMessages.Add(
                    new ValidationFailure(string.Empty, "Please check your email to reset your password."));
            }

            return response;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ServiceResponse<ProfileViewModel>> Login([FromBody] LoginModel model)
        {
            var response = new ServiceResponse<ProfileViewModel>();
            try
            {
                var validation = new LoginModelValidator();
                var results = validation.Validate(model);

                response.ErrorMessages = results.Errors.ToList();

                if (!response.Successful) return response;

                await this._signInManager.SignOutAsync();

                var result = await this._signInManager.PasswordSignInAsync(
                                 model.Email,
                                 model.Password,
                                 model.RememberMe,
                                 false);
                if (!result.Succeeded)
                {
                    response.ErrorMessages.Add(
                        result.IsLockedOut
                            ? new ValidationFailure(model.Email, $"User {model.Email} account locked out.")
                            : new ValidationFailure(model.Email, "Invalid login attempt."));
                }
                else
                {
                    var user = await this._userManager.FindByEmailAsync(model.Email);
                    if (!user.EmailConfirmed)
                        response.ErrorMessages.Add(
                            new ValidationFailure(model.Email, "your login account is not verified yet."));
                    else if (user.Disabled)
                        response.ErrorMessages.Add(
                            new ValidationFailure(
                                model.Email,
                                "Your profile has been disabled please contact administration."));
                    else
                        response = new ServiceResponse<ProfileViewModel>
                        {
                            Data = new ProfileViewModel
                            {
                                Email = model.Email,
                                Id = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Token = await this._commonService.GenerateToken(
                                                                      model.Email,
                                                                      model.Password)
                            }
                        };
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(model.Email, "Invalid login attempt."));
            }

            return response;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ServiceResponse<ProfileViewModel>> Register([FromBody] RegisterModel model)
        {
            var response = new ServiceResponse<ProfileViewModel>();

            try
            {
                var validation = new RegisterModelValidator(model);
                var results = validation.Validate(model);

                response.ErrorMessages = results.Errors.ToList();

                if (!response.Successful) return response;

                var isUserExist = await this._userManager.FindByEmailAsync(model.Email);
                if (isUserExist != null)
                {
                    response.ErrorMessages.Add(
                        new ValidationFailure(string.Empty, "That email is taken. Try another."));
                    return response;
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Disabled = false
                };

                var result = await this._userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    user.EmailConfirmed = false;
                    await this._userManager.UpdateAsync(user);
                    await this._userManager.AddToRoleAsync(user, RolesNames.User);

                    var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = HttpUtility.UrlEncode(code);
                    var callbackUrl =
                        $"{new Uri(this._iconfiguration["Cors:AllowedOrigin"])}/register?userid={user.Id}&code={code}";

                    var registerText =
                        $"{this._env.WebRootPath}{Path.DirectorySeparatorChar}EmailTemplate{Path.DirectorySeparatorChar}Register.html";
                    var replaceEmailText = new
                    {
                        model.Email,
                        RegisterLink = callbackUrl,
                        Logo = Convert.ToString(
                                                       $"{new Uri(this._iconfiguration["Cors:AllowedOrigin"])}{Path.DirectorySeparatorChar}assets{Path.DirectorySeparatorChar}img{Path.DirectorySeparatorChar}logo.png"),
                        Url = Convert.ToString(
                                                       new Uri(this._iconfiguration["Cors:AllowedOrigin"]))
                    };
                    var registerBody = replaceEmailText.SetEmailTemplates(registerText.ReadFile());
                    await this._emailSender.SendEmailAsync(model.Email, "Confirm your Account", registerBody);
                }
                else
                {
                    foreach (var error in result.Errors)
                        response.ErrorMessages.Add(new ValidationFailure(error.Code, error.Description));
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(model.Email, ex.Message));
            }

            return response;
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<ServiceResponse<bool>> ResetPassword(ResetPasswordViewModel model)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var validation = new ResetPasswordViewModelValidator(model);
                var results = validation.Validate(model);

                response.ErrorMessages = results.Errors.ToList();

                if (response.Successful)
                {
                    var user = await this._userManager.FindByIdAsync(model.UserId);
                    if (user != null)
                    {
                        model.Code = model.Code.Replace(" ", "+");
                        var result = await this._userManager.ResetPasswordAsync(user, model.Code, model.Password);
                        response.Data = result.Succeeded;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }

        [HttpGet("getallusers")]
        [Authorize(Roles = RolesNames.User)]
        public async Task<ServiceResponse<List<ApplicationUser>>> GetAllUsers()
        {
            var response = new ServiceResponse<List<ApplicationUser>>();
            try
            {
                var users = await this._userManager.GetUsersInRoleAsync(RolesNames.User);
                //response.Data = users.Select(x => new ApplicationUser
                //{
                //    AccountId = x.AccountId,
                //    FirstName = x.FirstName,
                //    LastName = x.LastName,
                //    Disabled = x.Disabled
                //}).ToList();
                response.Data = users.ToList();
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }

        [HttpPost("changepassword")]
        [Authorize(Roles = RolesNames.User)]
        public async Task<ServiceResponse<bool>> ChangePassword(ChangePasswordModel model)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var validation = new ChangePasswordModelValidator(model);
                var results = validation.Validate(model);

                response.ErrorMessages = results.Errors.ToList();

                if (response.Successful)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId);
                    var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    response.Data = result.Succeeded;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(new ValidationFailure(string.Empty, ex.Message));
            }

            return response;
        }
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Usersapp.models;
using Usersapp.ViewModels;
using UsersApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UsersApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> Login(loginViewModel model)
        {
            if (ModelState.IsValid)
            {
                 var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                    return View(model);
                }
            }        
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };

                var result = await userManager.CreateAsync(users, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            return View(model);
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if(user == null)
                {
                    ModelState.AddModelError("", "Something is wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword","Account", new {username = user.UserName});
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModels model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if(user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong. try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

[HttpPost]
public async Task<IActionResult> Profile(ProfileViewModel model)
{
    var user = await userManager.GetUserAsync(User);
    if (user == null)
        return RedirectToAction("Login", "Account");
    user.DOB = model.DOB;
    user.Address = model.Address;
    user.Xth_Marks = model.Xth_Marks;
    user.XIIth_Marks = model.XIIth_Marks;
    user.UG_Marks = model.UG_Marks;
    user.PG_Marks = model.PG_Marks;
    user.PhoneNumber= model.PhoneNumber;

    await userManager.UpdateAsync(user); // Save to DB

    return RedirectToAction("Profile");
}

        [HttpGet]
public async Task<IActionResult> Profile()
{
    var users = await userManager.GetUserAsync(User); // Gets currently logged-in user

    if (users == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var model = new ProfileViewModel
    {
        Name = users.FullName,
        Id = users.Id,
        Email = users.Email,
        DOB = users.DOB ?? DateTime.MinValue,
        Address = users.Address,
        Xth_Marks = users.Xth_Marks,
        XIIth_Marks = users.XIIth_Marks,
        UG_Marks = users.UG_Marks,
        PG_Marks = users.PG_Marks,
        PhoneNumber= users.PhoneNumber
    };
    return View(model);
}
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            var allUsers = userManager.Users.ToList();

            var usersViewModel = allUsers.Select(u => new ProfileViewModel
            {
                Name = u.FullName,
                Id = u.Id,
                Email = u.Email,
                DOB = u.DOB ?? DateTime.MinValue,
                Address = u.Address,
                Xth_Marks = u.Xth_Marks,
                XIIth_Marks = u.XIIth_Marks,
                UG_Marks = u.UG_Marks,
                PG_Marks = u.PG_Marks,
                PhoneNumber = u.PhoneNumber,
                Role =u.Role
            }).ToList();

            return View(usersViewModel);
        }

        [HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UpdateUserFromAdmin(ProfileViewModel model)
{
    if (model == null || string.IsNullOrEmpty(model.Id))
    {
        TempData["ErrorMessage"] = "Invalid data submitted.";
        return RedirectToAction("Adminpanel");
    }

    var user = await userManager.FindByIdAsync(model.Id);
    if (user == null)
    {
        TempData["ErrorMessage"] = "User not found.";
        return RedirectToAction("Adminpanel");
    }

    // Update user details safely
    user.FullName = model.Name ?? user.FullName;
    user.Email = model.Email ?? user.Email;
    user.UserName = model.Email ?? user.UserName;
    user.DOB = model.DOB;
    user.Address = model.Address ?? user.Address;
    user.Xth_Marks = model.Xth_Marks;
    user.XIIth_Marks = model.XIIth_Marks;
    user.UG_Marks = model.UG_Marks;
    user.PG_Marks = model.PG_Marks;
    user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

    var result = await userManager.UpdateAsync(user);
    TempData["SuccessMessage"] = result.Succeeded ? "User updated." : "Update failed.";
    return RedirectToAction("Adminpanel");
}
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteUserByEmail(string email)
{
    if (string.IsNullOrEmpty(email))
    {
        return BadRequest("Email is required.");
    }

    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        return NotFound();
    }

    var result = await userManager.DeleteAsync(user);
    if (result.Succeeded)
    {
        TempData["SuccessMessage"] = "User deleted successfully.";
        return RedirectToAction("AdminPanel");
    }

    TempData["ErrorMessage"] = "User deletion failed.";
    return RedirectToAction("AdminPanel");
}
public IActionResult AddUser()
        {
            return View();
        }
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> AddUser(ProfileViewModel model)
{
    var newUser = new Users
    {
        FullName = model.Name,
        UserName = model.Email,
        Email = model.Email,
        PhoneNumber = model.PhoneNumber,
        DOB = model.DOB,
        Address = model.Address,
        Xth_Marks = model.Xth_Marks,
        XIIth_Marks = model.XIIth_Marks,
        UG_Marks = model.UG_Marks,
        PG_Marks = model.PG_Marks
    };

    var result = await userManager.CreateAsync(newUser, "123456789"); // Change password logic if needed

    if (result.Succeeded)
    {
        TempData["SuccessMessage"] = "User created successfully.";
        return RedirectToAction("AdminPanel");
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError("", error.Description);
    }

    return View("AddUser", model);
}
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> ChangeUserRole([FromForm] string Id, [FromForm] string Role, 
    [FromServices] UserManager<Users> userManager, 
    [FromServices] RoleManager<IdentityRole> roleManager)
{
    var user = await userManager.FindByIdAsync(Id);
    if (user == null)
    {
        TempData["ErrorMessage"] = "User not found.";
        return RedirectToAction("AdminPanel");
    }

    // Get current roles
    var currentRoles = await userManager.GetRolesAsync(user);

    // Remove all current roles
    await userManager.RemoveFromRolesAsync(user, currentRoles);

    // Check if the role exists, if not, create it
    if (!await roleManager.RoleExistsAsync(Role))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(Role));
        if (!roleResult.Succeeded)
        {
            TempData["ErrorMessage"] = "Failed to create role.";
            return RedirectToAction("AdminPanel");
        }
    }

    // Add the new role to the user
    await userManager.AddToRoleAsync(user, Role);

    // Optional: Update custom 'Role' field in database, if needed (assuming you have a 'Role' field)
    user.Role = Role;
    await userManager.UpdateAsync(user);

    TempData["SuccessMessage"] = "User role updated successfully.";
    return RedirectToAction("AdminPanel");
}


}
}


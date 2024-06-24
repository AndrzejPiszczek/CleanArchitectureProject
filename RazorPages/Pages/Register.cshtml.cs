using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public string Username { get; set; }
    [BindProperty]
    public string Password { get; set; }
    public string ErrorMessage { get; set; }

    public RegisterModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = Username };
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                return RedirectToPage("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ErrorMessage = "Invalid registration attempt.";
        return Page();
    }
}

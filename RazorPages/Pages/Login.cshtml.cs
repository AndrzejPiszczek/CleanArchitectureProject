using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    [BindProperty]
    public string Username { get; set; }
    [BindProperty]
    public string Password { get; set; }

    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Username, Password, false, false);
        if (result.Succeeded)
        {
            return RedirectToPage("Index");
        }
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}

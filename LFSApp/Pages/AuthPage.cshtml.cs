using LFSApp.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Dbcontext;

namespace LFSApp.Pages
{
    public class AuthPageModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MainDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthPageModel(
            SignInManager<ApplicationUser> signInManager,
            MainDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public User Users { get; set; }
        [BindProperty]
        public Login Login { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }
        public ActionResult OnGet(string returnUrl = null)
        {
            // preserve incoming returnUrl so forms can submit it back
            // ReturnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : Url.Content("~/");

         
            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // preserve returnUrl
            ReturnUrl = returnUrl ?? Url.Content("~/");

            // validate model
            if (!ModelState.IsValid || Users == null)
            {
                return Page();
            }

            // check if email exists
            var emailExists = await _userManager.FindByEmailAsync(Users.Email);

            if (emailExists != null)
            {
                ModelState.AddModelError("Users.Email", "Email already exists.");
                return Page();
            }

            // create user object
            var user = new ApplicationUser
            {
                FirstName = Users.Firstname,
                LastName = Users.Lastname,
                Email = Users.Email,
                UserName = Users.Email
            };

            var result = await _userManager.CreateAsync(user, Users.Password);

            // if account was created successfully
            if (result.Succeeded)
            {
                // redirect directly to login page
                return RedirectToPage("/AuthPage", new { ReturnUrl = ReturnUrl });
            }

            // show errors from identity
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }


        public async Task<IActionResult> OnPostLogIn(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/CartPage");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Login.Email);
            if (user == null)
            {
                ModelState.AddModelError("Login.Email", "Email Does Not Exist.");
                return Page();
            }

            //var userExist = await _signInManager.CheckPasswordSignInAsync(user, Login.Password, false);
            var userExist = await _signInManager.PasswordSignInAsync(user, Login.Password, false, false);
            if (userExist.Succeeded)
            {

                await _signInManager.SignInAsync(user, isPersistent: false);
                // ensure returnUrl is local
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return LocalRedirect(Url.Content("~/CartPage"));

            }
            else
            {
                ModelState.AddModelError("Login.Password", "Password Is Wrong Try Again.");
            }

            return Page();
        }
    }
}

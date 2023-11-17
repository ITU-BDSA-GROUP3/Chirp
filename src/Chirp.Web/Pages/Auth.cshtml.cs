using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Chirp.Web.Pages;

[Authorize]
public class AuthModel : PageModel
{
    IAuthorRepository _authorRepository;

    public AuthModel(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public ActionResult OnGet(bool? signOut)
    {
        if (User.Identity.IsAuthenticated && signOut != null)
        {
            if (signOut.Value)
                Response.Cookies.Delete(".AspNetCore.Cookies");
        }
    
        _authorRepository.CreateAuthor(new Author { AuthorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), Name = User.Identity.Name, Email = User.FindFirst(ClaimTypes.Email)?.Value});
        return RedirectToPage("Public");
    }
}

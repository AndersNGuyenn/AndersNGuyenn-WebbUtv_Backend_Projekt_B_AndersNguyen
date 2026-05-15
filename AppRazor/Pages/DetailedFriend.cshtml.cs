using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages;

public class DetailedFriendModel : PageModel
{
    private readonly IFriendsService _friendsService;
    readonly ILogger<DetailedFriendModel> _logger = null;
    public DetailedFriendModel(IFriendsService friendsService, ILogger<DetailedFriendModel> logger)
    {
        _friendsService = friendsService;
        _logger = logger;
    }

    public IFriend? Friend { get; set; }

    public async Task<IActionResult> OnGet()
    {
        Guid _friendId = Guid.Parse(Request.Query["id"]);
        Friend = (await _friendsService.ReadFriendAsync(_friendId,false)).Item;

        return Page();
    }

}
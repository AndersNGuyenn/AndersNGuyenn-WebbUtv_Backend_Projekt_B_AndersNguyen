using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages;

public class OverviewModel : PageModel
{
    private readonly IFriendsService _friendsService;

    public OverviewModel(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public List<IFriend>? Friends { get; set; }

    public List<IFriend>? SelfCreateFriends { get; set; }

    [BindProperty]
    public string? Filter { get; set; }

    public int PageSize { get; set; } = 10;
    public int ThisPageNr { get; set; }
    public int PrevPageNr { get; set; }
    public int NextPageNr { get; set; }
    public int NrVisiblePages { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (int.TryParse(Request.Query["pagenr"], out int pageNr))
        {
            ThisPageNr = pageNr;
        }

        Filter = Request.Query["search"];

        var seededFriends = await _friendsService.ReadFriendsAsync(
            seeded: true,
            flat: false,
            filter: Filter,
            pageNumber: ThisPageNr,
            pageSize: PageSize);

        var unSeededFriends = await _friendsService.ReadFriendsAsync(
            seeded: false,
            flat: false,
            filter: Filter,
            pageNumber: ThisPageNr,
            pageSize: PageSize);

        Friends = seededFriends.PageItems;
        Friends.AddRange(unSeededFriends.PageItems);

        UpdatePagination(seededFriends.DbItemsCount);

        return Page();
    }

    public async Task<IActionResult> OnPostSearch()
    {
        var seededFriends = await _friendsService.ReadFriendsAsync(true, false, Filter, ThisPageNr, PageSize);
        var unSeededFriends = await _friendsService.ReadFriendsAsync(false, false, Filter, ThisPageNr, PageSize);

        Friends = seededFriends.PageItems;
        SelfCreateFriends = unSeededFriends.PageItems;

        Friends.AddRange(SelfCreateFriends);


        UpdatePagination(seededFriends.DbItemsCount);

        return Page();
    }

    public async Task<IActionResult> OnPostSortName()
    {
        var seededFriends = await _friendsService.ReadFriendsAsync(
            true,
            false,
            Filter,
            ThisPageNr,
            PageSize);

        var unSeededFriends = await _friendsService.ReadFriendsAsync(false, false, Filter, ThisPageNr, PageSize);
        SelfCreateFriends = unSeededFriends.PageItems;

        Friends = seededFriends.PageItems;
        Friends.AddRange(SelfCreateFriends);

        Friends = Friends.OrderByDescending(f => f.FirstName).ToList();

        UpdatePagination(seededFriends.DbItemsCount);

        return Page();
    }

    public async Task<IActionResult> OnPostSortCity()
    {
        var seededFriends = await _friendsService.ReadFriendsAsync(
            true,
            false,
            Filter,
            ThisPageNr,
            PageSize);

        var unSeededFriends = await _friendsService.ReadFriendsAsync(false, false, Filter, ThisPageNr, PageSize);
        SelfCreateFriends = unSeededFriends.PageItems;

        Friends = seededFriends.PageItems;
        Friends.AddRange(SelfCreateFriends);

        Friends = Friends.OrderBy(f => f.Address?.City == null).ThenBy(f => f.Address!.City).ToList();

        UpdatePagination(seededFriends.DbItemsCount);

        return Page();
    }

    public async Task<IActionResult> OnPostSortPets()
    {
        var seededFriends = await _friendsService.ReadFriendsAsync(
            true,
            false,
            Filter,
            ThisPageNr,
            PageSize);

        var unSeededFriends = await _friendsService.ReadFriendsAsync(false, false, Filter, ThisPageNr, PageSize);
        SelfCreateFriends = unSeededFriends.PageItems;

        Friends = seededFriends.PageItems;
        Friends.AddRange(SelfCreateFriends);

        Friends = Friends.OrderByDescending(f => f.Pets.Count()).ToList();

        UpdatePagination(seededFriends.DbItemsCount);

        return Page();
    }


    private void UpdatePagination(int nrOfItems)
    {
        var nrOfPages = (int)Math.Ceiling((double)nrOfItems / PageSize);

        PrevPageNr = Math.Max(0, ThisPageNr - 1);
        NextPageNr = Math.Min(nrOfPages - 1, ThisPageNr + 1);
        NrVisiblePages = Math.Min(10, nrOfPages);
    }
}
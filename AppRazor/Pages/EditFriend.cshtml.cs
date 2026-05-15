using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages;

public class EditFriendModel : PageModel
{
    private readonly IFriendsService _friendsService;
    private readonly IAddressesService _addressesService;
    private readonly IPetsService _petsService;
    private readonly IQuotesService _quotesService;

    public EditFriendModel(
        IFriendsService friendsService,
        IAddressesService addressesService,
        IPetsService petsService,
        IQuotesService quotesService)
    {
        _friendsService = friendsService;
        _addressesService = addressesService;
        _petsService = petsService;
        _quotesService = quotesService;
    }

    [BindProperty]
    public FriendInputModel FriendInput { get; set; } = new();

    [BindProperty]
    public Guid SelectedPetId { get; set; }

    [BindProperty]
    public Guid SelectedQuoteId { get; set; }

    public string PageHeader { get; set; } = "Edit Friend";

    public async Task<IActionResult> OnGet(Guid id)
    {
        var friendData = await _friendsService.ReadFriendAsync(id, false);
        var friend = friendData.Item;

        FriendInput = new FriendInputModel(friend);
        PageHeader = $"Edit details of {FriendInput.FirstName} {FriendInput.LastName}";

        return Page();
    }

    public async Task<IActionResult> OnPostSave()
    {
        if (!ModelState.IsValid)
        {
            PageHeader = $"Edit details of {FriendInput.FirstName} {FriendInput.LastName}";
            return Page();
        }

        var addressDto = new AddressCuDto
        {
            AddressId = FriendInput.Address.AddressId,
            StreetAddress = FriendInput.Address.StreetAddress,
            City = FriendInput.Address.City,
            Country = FriendInput.Address.Country
        };

        await _addressesService.UpdateAddressAsync(addressDto);

        var friendDto = new FriendCuDto
        {
            FriendId = FriendInput.FriendId,
            FirstName = FriendInput.FirstName,
            LastName = FriendInput.LastName,
            Birthday = FriendInput.Birthday,
            AddressId = FriendInput.Address.AddressId,
            PetsId = FriendInput.Pets.Select(p => p.PetId).ToList(),
            QuotesId = FriendInput.Quotes.Select(q => q.QuoteId).ToList()
        };

        await _friendsService.UpdateFriendAsync(friendDto);

        return Redirect($"~/DetailedFriend?id={FriendInput.FriendId}");
    }

    public async Task<IActionResult> OnPostDeletePet()
    {
        await _petsService.DeletePetAsync(SelectedPetId);
        return Redirect($"~/EditFriend?id={FriendInput.FriendId}");
    }

    public async Task<IActionResult> OnPostDeleteQuote()
    {
        await _quotesService.DeleteQuoteAsync(SelectedQuoteId);
        return Redirect($"~/EditFriend?id={FriendInput.FriendId}");
    }

    public class FriendInputModel
    {
        public Guid FriendId { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        [Required]
        public DateTime? Birthday { get; set; }

        public AddressInputModel Address { get; set; } = new();

        public List<PetInputModel> Pets { get; set; } = new();

        public List<QuoteInputModel> Quotes { get; set; } = new();

        public FriendInputModel() { }

        public FriendInputModel(IFriend friend)
        {
            FriendId = friend.FriendId;
            FirstName = friend.FirstName;
            LastName = friend.LastName;
            Email = friend.Email;
            Birthday = friend.Birthday;
            Address = new AddressInputModel(friend.Address);
            Pets = friend.Pets?.Select(p => new PetInputModel(p)).ToList() ?? new();
            Quotes = friend.Quotes?.Select(q => new QuoteInputModel(q)).ToList() ?? new();
        }
    }

    public class AddressInputModel
    {
        public Guid AddressId { get; set; }

        [Required]
        public string StreetAddress { get; set; } = "";

        [Required]
        [Range(1, int.MaxValue)]
        public int ZipCode { get; set; }

        [Required]
        public string City { get; set; } = "";

        [Required]
        public string Country { get; set; } = "";

        public AddressInputModel() { }

        public AddressInputModel(IAddress address)
        {
            if (address == null) return;

            AddressId = address.AddressId;
            StreetAddress = address.StreetAddress;
            ZipCode = address.ZipCode;
            City = address.City;
            Country = address.Country;
        }
    }

    public class PetInputModel
    {
        public Guid PetId { get; set; }
        public string Name { get; set; } = "";
        public string Kind { get; set; } = "";

        public PetInputModel() { }

        public PetInputModel(IPet pet)
        {
            PetId = pet.PetId;
            Name = pet.Name;
            Kind = pet.Kind.ToString();
        }
    }

    public class QuoteInputModel
    {
        public Guid QuoteId { get; set; }
        public string QuoteText { get; set; } = "";
        public string Author { get; set; } = "";

        public QuoteInputModel() { }

        public QuoteInputModel(IQuote quote)
        {
            QuoteId = quote.QuoteId;
            QuoteText = quote.QuoteText;
            Author = quote.Author;
        }
    }
}
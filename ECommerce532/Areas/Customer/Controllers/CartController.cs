using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce532.Areas.Customer.Controllers;

[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize]
public class CartController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Promotion> _promotionRepository;

    public CartController(UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Product> productRepository,
        IRepository<Promotion> promotionRepository)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task<IActionResult> AddToCart(int productId, int count, CancellationToken ct = default)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

        var product = _productRepository.GetOne(e => e.Id == productId);
        if (product is null) return NotFound();

        if (cartInDB is not null)
        {
            cartInDB.Count += count;
        }
        else
        {
            await _cartRepository.CreateAsync(new Cart
            {
                ApplicationUserId = user.Id,
                Count = count,
                ProductId = productId,
                CurrentPrice = product.Price
            }, ct);
        }
        
        await _cartRepository.CommitAsync(ct);

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Add Product Successfully";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(string? promo = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var carts = _cartRepository.Get(e => e.ApplicationUserId == user.Id, 
            includes: [e => e.Product]).ToList(); 

        if(promo is not null)
        {
            var promotion = _promotionRepository.GetOne(e => e.Code == promo && e.Status && e.ValidTo >= DateTime.Now && e.MaxUsage >= 1);

            bool isValidPromo = false;

            if(promotion is not null)
            {
                foreach (var item in carts)
                {
                    if(item.ProductId == promotion.ProductId)
                    {
                        var priceAfterDiscount = item.Product.Price - (item.Product.Price * (promotion.Discount / 100m));
                        item.CurrentPrice = priceAfterDiscount;
                        _cartRepository.Update(item);
                        await _cartRepository.CommitAsync();
                        isValidPromo = true;
                        break;
                    }
                }

            }

            if(isValidPromo)
                TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Apply promotion successfully";
            else
                TempData[NotificationConstants.ERROR_NOTIFICATION] = "Invalid promo code, invalid product or expired";
        }

        return View(carts);
    }

    public async Task<IActionResult> IncrementQuantity(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.Id == id);
        if (cartInDB is null) return NotFound();

        cartInDB.Count += 1;
        await _cartRepository.CommitAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DecrementQuantity(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.Id == id);
        if (cartInDB is null) return NotFound();

        if (cartInDB.Count > 1)
        {
            cartInDB.Count -= 1;
            await _cartRepository.CommitAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteProduct(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var cartInDB = _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.Id == id);
        if (cartInDB is null) return NotFound();

        _cartRepository.Delete(cartInDB);
        await _cartRepository.CommitAsync();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Pay()
    {
        return View();
    }

    // TODO
    public IActionResult SaveToWishlist()
    {
        return View();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce532.Areas.Customer.Controllers;

[Area(AreaConstants.CUSTOMER_AREA)]
[Authorize]
public class OrderController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;

    public OrderController(UserManager<ApplicationUser> userManager, IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository)
    {
        _userManager = userManager;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<IActionResult> Index(int? query, int page = 1, int size = 4)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var orders = _orderRepository.Get();

        if (query is not null)
            orders = orders.Where(e => e.Id == query);

        var totalPages = Math.Ceiling(orders.Count() / (double)size);
        orders = orders.Skip((page - 1) * size).Take(size);

        return View(new OrderWithFilterVM
        {
            Orders = orders,
            Query = query,
            TotalPages = totalPages,
            CurrentPage = page,
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var order = _orderRepository.GetOne(e => e.Id == id && e.ApplicationUserId == user.Id);
        if (order is null) return NotFound();

        var orderItems = _orderItemRepository.Get(e => e.OrderId == id, includes: [e => e.Product, e=>e.Order]);

        return View(orderItems);
    }

    public async Task<IActionResult> Refund(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var order = _orderRepository.GetOne(e => e.Id == id && e.ApplicationUserId == user.Id);
        if (order is null) return NotFound();

        if (order.OrderStatus >= OrderStatus.Shipped) return BadRequest();

        var options = new RefundCreateOptions()
        {
            Reason = RefundReasons.Unknown,
            Amount = (long)order.TotalPrice,
            PaymentIntent = order.TransactionId,
        };

        var service = new RefundService();
        var session = service.Create(options);

        return View();
    }
}

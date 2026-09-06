using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
public class ProductController : Controller
{
    //private readonly ApplicationDbContext _db = new();
    IFileUpload fileUpload = new FileUpload();

    private readonly IRepository<Product> _productRepository;
    private readonly IBulkRepository<ProductSubImg> _productSubImgRepository;// = new BulkRepository<ProductSubImg>();
    private readonly IRepository<Category> _categoryRepository;// = new Repository<Category>();
    private readonly IRepository<Brand> _brandRepository;// = new Repository<Brand>();

    public ProductController(IRepository<Product> productRepository, 
        IBulkRepository<ProductSubImg> productSubImgRepository,
        IRepository<Category> categoryRepository,
        IRepository<Brand> brandRepository)
    {
        _productRepository = productRepository;
        _productSubImgRepository = productSubImgRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
    }

    public IActionResult Index(ProductFilterVM productFilterVM, int page = 1, int size = 5)
    {
        //var products = _db.Products
        //    .Include(e => e.Category)
        //    .Include(e => e.Brand)
        //    .AsQueryable();

        var products = _productRepository.Get(includes: [e => e.Category, e => e.Brand]);

        // Filter

        if (productFilterVM.name is not null)
            products = products.Where(e => e.Name.ToLower().Contains(productFilterVM.name.ToLower()));

        if (productFilterVM.minPrice is not null)
            products = products.Where(e => e.Price >= productFilterVM.minPrice);

        if (productFilterVM.maxPrice is not null)
            products = products.Where(e => e.Price < productFilterVM.maxPrice);

        if (productFilterVM.categoryId is not null)
            products = products.Where(e => e.CategoryId == productFilterVM.categoryId);

        if (productFilterVM.brandId is not null)
            products = products.Where(e => e.BrandId == productFilterVM.brandId);

        if (productFilterVM.lessQuantity is not null)
            products = products.OrderBy(e => e.Quantity);

        // Pagination

        var totalPages = Math.Ceiling(products.Count() / (double)size);
        products = products.Skip((page - 1) * size).Take(size);

        // Extra data

        var categories = _categoryRepository.Get();
        var brands = _brandRepository.Get();

        return View(new ProductWithFilterVM()
        {
            Products = products,
            Categories = categories,
            Brands = brands,
            TotalPages = totalPages,
            CurrentPage = page,
            Name = productFilterVM.name ?? "",
            MinPrice = productFilterVM.minPrice,
            MaxPrice = productFilterVM.maxPrice,
            CategoryId = productFilterVM.categoryId,
            BrandId = productFilterVM.brandId,
            LessQuantity = productFilterVM.lessQuantity,
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        var categories = _categoryRepository.Get();
        var brands = _brandRepository.Get();

        return View(new ProductWithFilterVM()
        {
            Categories = categories,
            Brands = brands
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, IFormFile mainImg, List<IFormFile> subImgs/*, List<string> colors*/, CancellationToken ct = default)
    {
        if(mainImg is not null && mainImg.Length > 0)
        {
            var fileName = fileUpload.GenerateFileName(mainImg.FileName);

            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "products", fileName);

            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, mainImg);

            product.MainImg = fileName;
        }

        await _productRepository.CreateAsync(product, ct);
        await _productRepository.CommitAsync(ct);

        if (subImgs.Any())
        {
            foreach (var item in subImgs)
            {
                var fileName = fileUpload.GenerateFileName(item.FileName);
                if (fileName is null) return BadRequest();

                var filePath = fileUpload.GenerateFullPath(FileType.Img, "products\\sub-imgs", fileName);
                if (filePath is null) return BadRequest();

                fileUpload.UploadFileLocally(filePath, item);

                //_db.ProductSubImgs.Add(new()
                //{
                //    SubImg = fileName,
                //    ProductId = product.Id
                //});

                await _productSubImgRepository.CreateAsync(new()
                {
                    SubImg = fileName,
                    ProductId = product.Id
                });
            }

            await _productRepository.CommitAsync(ct);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var product = _productRepository.GetOne(e => e.Id == id, tracked: false);

        if (product is null) return NotFound();

        var categories = _categoryRepository.Get();
        var brands = _brandRepository.Get();

        //var productSubImgs = _db.ProductSubImgs.Where(e => e.ProductId == product.Id);
        var productSubImgs = _productSubImgRepository.Get(e => e.ProductId == product.Id);

        return View(new ProductWithDetailsVM()
        {
            Product = product ?? new(),
            ProductSubImgs = productSubImgs,
            Categories = categories.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(),
            }),
            Brands = brands.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Id.ToString(),
            }),
        });
    }

    [HttpPost]
    public async Task<IActionResult> Update(Product product, IFormFile mainImg, List<IFormFile> subImgs/*, List<string> colors*/, CancellationToken ct = default)
    {
        //var productInDB = _db.Products.AsNoTracking().FirstOrDefault(e => e.Id == product.Id);
        var productInDB = _productRepository.GetOne(e => e.Id == product.Id, tracked: false);
        if(productInDB is null) return NotFound();

        if (mainImg is not null && mainImg.Length > 0)
        {
            // create new img
            var fileName = fileUpload.GenerateFileName(mainImg.FileName);
            if (fileName is null) return BadRequest();

            var filePath = fileUpload.GenerateFullPath(FileType.Img, "products", fileName);
            if (filePath is null) return BadRequest();

            fileUpload.UploadFileLocally(filePath, mainImg);

            // delete old img from wwwroot
            var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "products", productInDB.MainImg);
            if (oldFilePath is null) return BadRequest();

            fileUpload.DeleteFileLocally(oldFilePath);

            // update
            product.MainImg = fileName;
        }
        else
            product.MainImg = productInDB.MainImg;

        _productRepository.Update(product);
        await _productRepository.CommitAsync(ct);

        if (subImgs.Any())
        {
            // delete old img from wwwroot & db
            //var oldImgs = _db.ProductSubImgs.Where(e => e.ProductId == product.Id);
            var oldImgs = _productSubImgRepository.Get(e => e.ProductId == product.Id);

            foreach (var item in oldImgs)
            {
                var oldFilePath = fileUpload.GenerateFullPath(FileType.Img, "products\\sub-imgs", item.SubImg);
                if (oldFilePath is null) return BadRequest();

                fileUpload.DeleteFileLocally(oldFilePath);
            }

            _productSubImgRepository.DeleteRange(oldImgs);

            // create new img in wwwroot & db
            foreach (var item in subImgs)
            {
                var fileName = fileUpload.GenerateFileName(item.FileName);
                if (fileName is null) return BadRequest();

                var filePath = fileUpload.GenerateFullPath(FileType.Img, "products\\sub-imgs", fileName);
                if (filePath is null) return BadRequest();

                fileUpload.UploadFileLocally(filePath, item);

                await _productSubImgRepository.CreateAsync(new()
                {
                    SubImg = fileName,
                    ProductId = product.Id
                });
            }

            await _productRepository.CommitAsync(ct);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id, CancellationToken ct = default)
    {
        // TODO

        // 1. retrieve product with specified id

        // 2. delete img from wwwroot related this product

        // 3. delete product from db

        return RedirectToAction(nameof(Index));
    }
}

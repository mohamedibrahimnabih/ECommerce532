namespace ECommerce532.Repositories;

public class ProductSubImgRepository : Repository<ProductSubImg>
{
    public bool DeleteRange(IEnumerable<ProductSubImg> productSubImgs)
    {
        try
        {
            _db.RemoveRange(productSubImgs);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}

using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var connStr = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using (var connection = new NpgsqlConnection(connStr))
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                    (
                        "SELECT * From Coupon WHERE ProductName = @productName",
                        new { productName }
                    );
                if (coupon == null)
                {
                    coupon = new Coupon
                    {
                        ProductName = "No Discount",
                        Amount = 0,
                        Description = "No Discount Desc"
                    };
                }

                return coupon;
            }
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var connStr = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using (var connection = new NpgsqlConnection(connStr))
            {
                string query = @"
                    INSERT INTO Coupon (ProductName, Description, Amount) 
                    VALUES (@productName, @description, @amount)
                ";
                var affected = await connection.ExecuteAsync(
                    query,
                    new {
                        productName = coupon.ProductName,
                        description = coupon.Description,
                        amount = coupon.Amount,
                    });

                return affected > 0;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var connStr = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using (var connection = new NpgsqlConnection(connStr))
            {
                string query = @"
                    UPDATE Coupon
                    SET
                        ProductName = @productName,
                        Description = @description, 
                        Amount = @amount 
                    WHERE Id = @id
                ";
                var affected = await connection.ExecuteAsync(
                    query,
                    new
                    {
                        productName = coupon.ProductName,
                        description = coupon.Description,
                        amount = coupon.Amount,
                        id = coupon.Id
                    });

                return affected > 0;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var connStr = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
            using (var connection = new NpgsqlConnection(connStr))
            {
                string query = @"
                    DELETE FROM Coupon
                    WHERE ProductName = @productName
                ";
                var affected = await connection.ExecuteAsync(
                    query,
                    new
                    {
                        productName
                    });

                return affected > 0;
            }
        }
    }
}

using Catalog.Application.Repositories;
using DeliveryHub.Catalog.Domain.Appliaction.Repositories;
using DeliveryHub.Catalog.Domain.Entities;
using DeliveryHub.CatalogService.Domain.Entities;
using MongoDB.Driver.Linq;

namespace Catalog.API.Seed
{
    public static class CatalogDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var categoryRepo = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var imageRepo = scope.ServiceProvider.GetRequiredService<IProductImageRepository>();
            var stockRepo = scope.ServiceProvider.GetRequiredService<IStockRepository>();

            var categories = await SeedCategoriesAsync(categoryRepo);
            var productIds = await SeedProductsAsync(productRepo, categories);

            await SeedStockAsync(stockRepo, productIds);
            await SeedImagesAsync(imageRepo);
        }

        private static async Task SeedImagesAsync(IProductImageRepository imageRepository)
        {
            // Use the fixed GUIDs from your product seeder
            var imagesPerProduct = new Dictionary<Guid, (string main, string thumb)>
            {
                [new Guid("10000000-0000-0000-0000-000000000001")] = ("/images/10000000-0000-0000-0000-000000000001/main.webp", "/images/10000000-0000-0000-0000-000000000001/thumb.webp"),
                [new Guid("10000000-0000-0000-0000-000000000002")] = ("/images/10000000-0000-0000-0000-000000000002/main.webp", "/images/10000000-0000-0000-0000-000000000002/thumb.webp"),
                [new Guid("10000000-0000-0000-0000-000000000003")] = ("/images/10000000-0000-0000-0000-000000000003/main.webp", "/images/10000000-0000-0000-0000-000000000003/thumb.webp"),

                [new Guid("10000000-0000-0000-0000-000000000101")] = ("/images/10000000-0000-0000-0000-000000000101/main.webp", "/images/10000000-0000-0000-0000-000000000101/thumb.webp"),
                [new Guid("10000000-0000-0000-0000-000000000102")] = ("/images/10000000-0000-0000-0000-000000000102/main.webp", "/images/10000000-0000-0000-0000-000000000102/thumb.webp"),
                [new Guid("10000000-0000-0000-0000-000000000103")] = ("/images/10000000-0000-0000-0000-000000000103/main.webp", "/images/10000000-0000-0000-0000-000000000103/thumb.webp"),

                [new Guid("20000000-0000-0000-0000-000000000001")] = ("/images/20000000-0000-0000-0000-000000000001/main.webp", "/images/20000000-0000-0000-0000-000000000001/thumb.webp"),
                [new Guid("20000000-0000-0000-0000-000000000002")] = ("/images/20000000-0000-0000-0000-000000000002/main.webp", "/images/20000000-0000-0000-0000-000000000002/thumb.webp"),
                [new Guid("20000000-0000-0000-0000-000000000003")] = ("/images/20000000-0000-0000-0000-000000000003/main.webp", "/images/20000000-0000-0000-0000-000000000003/thumb.webp"),

                [new Guid("20000000-0000-0000-0000-000000000101")] = ("/images/20000000-0000-0000-0000-000000000101/main.webp", "/images/20000000-0000-0000-0000-000000000101/thumb.webp"),
                [new Guid("20000000-0000-0000-0000-000000000102")] = ("/images/20000000-0000-0000-0000-000000000102/main.webp", "/images/20000000-0000-0000-0000-000000000102/thumb.webp"),
                [new Guid("20000000-0000-0000-0000-000000000103")] = ("/images/20000000-0000-0000-0000-000000000103/main.webp", "/images/20000000-0000-0000-0000-000000000103/thumb.webp"),
            };

            foreach (var kv in imagesPerProduct)
            {
                var productId = kv.Key;
                var (mainUrl, thumbUrl) = kv.Value;

                // check and create Main
                var hasMain = imageRepository.GetAll().Any(i => i.ProductId == productId && i.Type == ProductImageType.Main);
                if (!hasMain)
                {
                    var img = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = mainUrl,
                        Type = ProductImageType.Main,
                        Order = 0
                    };
                    await imageRepository.CreateAsync(img, default);
                }

                // thumbnail
                var hasThumb = imageRepository.GetAll().Any(i => i.ProductId == productId && i.Type == ProductImageType.Thumbnail);
                if (!hasThumb)
                {
                    var img = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        Url = thumbUrl,
                        Type = ProductImageType.Thumbnail,
                        Order = 0
                    };
                    await imageRepository.CreateAsync(img, default);
                }
            }
        }

        // returns mapping <categoryName, categoryId>
        private static async Task<Dictionary<string, Guid>> SeedCategoriesAsync(ICategoryRepository categoryRepository)
        {
            var existing = (await categoryRepository.GetAllAsync(default)).ToList();

            Guid EnsureOrCreate(string name, Guid? parent = null, Guid? fixedId = null)
            {
                var found = existing.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)
                                                        && c.ParentId == parent);
                if (found != null)
                    return found.Id;

                var id = fixedId ?? Guid.NewGuid();
                var cat = new Category
                {
                    Id = id,
                    Name = name,
                    ParentId = parent
                };

                categoryRepository.CreateAsync(cat, default).GetAwaiter().GetResult();

                existing.Add(cat);
                return id;
            }

            // Root categories
            var elekId = EnsureOrCreate("Электротехника", null, new Guid("a1111111-1111-1111-1111-111111111111"));
            var shoesId = EnsureOrCreate("Обувь", null, new Guid("b2222222-2222-2222-2222-222222222222"));

            // Subcategories
            var mobilesId = EnsureOrCreate("Мобильные устройства", elekId, new Guid("a1111111-1111-1111-1111-111111111112"));
            var tvsId = EnsureOrCreate("Телевизоры", elekId, new Guid("a1111111-1111-1111-1111-111111111113"));

            var womenShoesId = EnsureOrCreate("Женская обувь", shoesId, new Guid("b2222222-2222-2222-2222-222222222223"));
            var menShoesId = EnsureOrCreate("Мужская обувь", shoesId, new Guid("b2222222-2222-2222-2222-222222222224"));

            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
            {
                ["Электротехника"] = elekId,
                ["Мобильные устройства"] = mobilesId,
                ["Телевизоры"] = tvsId,
                ["Обувь"] = shoesId,
                ["Женская обувь"] = womenShoesId,
                ["Мужская обувь"] = menShoesId
            };
        }

        // returns created or existing product ids
        private static async Task<List<Guid>> SeedProductsAsync(IProductRepository productRepository, Dictionary<string, Guid> categories)
        {
            var existing = (await productRepository.GetAllAsync(default)).ToList();

            Guid EnsureOrCreateProduct(string name, string description, decimal price, Guid categoryId, Dictionary<string, string>? attributes = null, Guid? fixedId = null)
            {
                var found = existing.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)
                                                        && p.CategoryId == categoryId);
                if (found != null)
                    return found.Id;

                var id = fixedId ?? Guid.NewGuid();
                var product = new Product
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Price = price,
                    CategoryId = categoryId,
                    Attributes = attributes != null ? new Dictionary<string, string>(attributes) : new Dictionary<string, string>()
                };

                productRepository.CreateAsync(product, default).GetAwaiter().GetResult();
                existing.Add(product);
                return id;
            }

            var createdIds = new List<Guid>();

            // Mobile devices (3)
            createdIds.Add(EnsureOrCreateProduct("iPhone 15", "Apple iPhone 15, 128GB, Pink", 62600.00m, categories["Мобильные устройства"],
                new() { ["Color"] = "Pink", ["Storage"] = "128GB" }, new Guid("10000000-0000-0000-0000-000000000001")) );

            createdIds.Add(EnsureOrCreateProduct("Samsung Galaxy S21", "Samsung Galaxy S21, 128GB, Phantom Gray", 32000.00m, categories["Мобильные устройства"],
                new() { ["Color"] = "Phantom Gray", ["Storage"] = "128GB" }, new Guid("10000000-0000-0000-0000-000000000002")) );

            createdIds.Add(EnsureOrCreateProduct("Xiaomi Redmi Note 14", "Xiaomi Redmi Note 14, 64GB", 30000.00m, categories["Мобильные устройства"],
                new() { ["Color"] = "Blue", ["Storage"] = "64GB" }, new Guid("10000000-0000-0000-0000-000000000003")) );

            // TVs (3)
            createdIds.Add(EnsureOrCreateProduct("LG OLED55", "LG OLED 55\" 4K", 55000.00m, categories["Телевизоры"],
                new() { ["Size"] = "55\"", ["Resolution"] = "4K" }, new Guid("10000000-0000-0000-0000-000000000101")) );

            createdIds.Add(EnsureOrCreateProduct("Samsung QLED 50", "Samsung QLED 50\" 4K", 50999.00m, categories["Телевизоры"],
                new() { ["Size"] = "50\"", ["Resolution"] = "4K" }, new Guid("10000000-0000-0000-0000-000000000102")) );

            createdIds.Add(EnsureOrCreateProduct("Sony Bravia 43", "Sony Bravia 43\" Full HD", 40199.00m, categories["Телевизоры"],
                new() { ["Size"] = "43\"", ["Resolution"] = "Full HD" }, new Guid("10000000-0000-0000-0000-000000000103")) );

            // Women's shoes (3)
            createdIds.Add(EnsureOrCreateProduct("Женские туфли-лодочки", "Женские туфли-лодочки, кожа", 2100.00m, categories["Женская обувь"],
                new() { ["Color"] = "Black", ["Material"] = "Leather" }, new Guid("20000000-0000-0000-0000-000000000001")) );

            createdIds.Add(EnsureOrCreateProduct("Кроссовки женские", "Кроссовки женские, текстиль", 3500.00m, categories["Женская обувь"],
                new() { ["Color"] = "White", ["SizeRange"] = "36-41" }, new Guid("20000000-0000-0000-0000-000000000002")) );

            createdIds.Add(EnsureOrCreateProduct("Ботинки женские", "Ботинки женские, замша", 3119.00m, categories["Женская обувь"],
                new() { ["Color"] = "Brown", ["Material"] = "Suede" }, new Guid("20000000-0000-0000-0000-000000000003")) );

            // Men's shoes (3)
            createdIds.Add(EnsureOrCreateProduct("Мужские ботинки", "Мужские кожаные ботинки", 3120.00m, categories["Мужская обувь"],
                new() { ["Color"] = "Brown", ["Material"] = "Leather" }, new Guid("20000000-0000-0000-0000-000000000101")) );

            createdIds.Add(EnsureOrCreateProduct("Кроссовки мужские", "Кроссовки мужские, сетка", 5900.00m, categories["Мужская обувь"],
                new() { ["Color"] = "Gray", ["SizeRange"] = "40-46" }, new Guid("20000000-0000-0000-0000-000000000102")) );

            createdIds.Add(EnsureOrCreateProduct("Лоферы мужские", "Лоферы мужские, кожа", 890.00m, categories["Мужская обувь"],
                new() { ["Color"] = "Black", ["Material"] = "Leather" }, new Guid("20000000-0000-0000-0000-000000000103")) );

            return createdIds;
        }

        private static async Task SeedStockAsync(IStockRepository stockRepository, IEnumerable<Guid> productIds)
        {
            var rnd = new Random(42);
            var stocks = productIds.Select(id => new { ProductId = id, Quantity = rnd.Next(5, 51) }).ToList();

            foreach (var s in stocks)
            {
                var foundStock = await stockRepository.GetAll().FirstOrDefaultAsync(x => x.ProductId == s.ProductId);

                if (foundStock == null)
                {
                    await stockRepository.CreateAsync(new Stock { ProductId = s.ProductId, TotalQty = s.Quantity }, default);
                }
            }
        }
    }
}

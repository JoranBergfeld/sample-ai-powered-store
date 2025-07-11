using Microsoft.EntityFrameworkCore;
using app.Models;

namespace app.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure many-to-many relationship between Product and Tag
        modelBuilder.Entity<ProductTag>()
            .HasKey(pt => new { pt.ProductId, pt.TagId });

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Product)
            .WithMany(p => p.ProductTags)
            .HasForeignKey(pt => pt.ProductId);

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.ProductTags)
            .HasForeignKey(pt => pt.TagId);

        // Seed data for Tags
        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "Electronics" },
            new Tag { Id = 2, Name = "Clothing" },
            new Tag { Id = 3, Name = "Books" },
            new Tag { Id = 4, Name = "Home & Kitchen" },
            new Tag { Id = 5, Name = "Sports" },
            new Tag { Id = 6, Name = "Automotive" },
            new Tag { Id = 7, Name = "Beauty" },
            new Tag { Id = 8, Name = "Health" },
            new Tag { Id = 9, Name = "Toys & Games" },
            new Tag { Id = 10, Name = "Office Supplies" }
        );

        // Seed data for Products
        SeedProducts(modelBuilder);

        // Seed data for ProductTags
        SeedProductTags(modelBuilder);
    }

    private void SeedProducts(ModelBuilder modelBuilder)
    {
        var products = new List<Product>();
        
        // Original products
        products.Add(new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop with 16GB RAM and 512GB SSD" });
        products.Add(new Product { Id = 2, Name = "T-Shirt", Description = "Cotton t-shirt, available in multiple colors" });
        products.Add(new Product { Id = 3, Name = "Programming Book", Description = "Learn C# programming with practical examples" });

        // Electronics (Tag 1)
        products.Add(new Product { Id = 4, Name = "Smartphone", Description = "Latest smartphone with 6.7-inch display and 5G connectivity" });
        products.Add(new Product { Id = 5, Name = "Tablet", Description = "10-inch tablet with high-resolution display and long battery life" });
        products.Add(new Product { Id = 6, Name = "Wireless Earbuds", Description = "Bluetooth earbuds with noise cancellation and 24-hour battery life" });
        products.Add(new Product { Id = 7, Name = "Smart Watch", Description = "Fitness tracking watch with heart rate monitor and GPS" });
        products.Add(new Product { Id = 8, Name = "Gaming Console", Description = "Next-gen gaming console with 4K gaming capabilities" });
        products.Add(new Product { Id = 9, Name = "Bluetooth Speaker", Description = "Portable speaker with 360-degree sound and waterproof design" });
        products.Add(new Product { Id = 10, Name = "Digital Camera", Description = "DSLR camera with 24MP sensor and 4K video recording" });
        products.Add(new Product { Id = 11, Name = "Monitor", Description = "27-inch 4K monitor with HDR support" });
        products.Add(new Product { Id = 12, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse with customizable buttons" });
        products.Add(new Product { Id = 13, Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard with customizable switches" });

        // Clothing (Tag 2)
        products.Add(new Product { Id = 14, Name = "Jeans", Description = "Classic denim jeans available in various fits and washes" });
        products.Add(new Product { Id = 15, Name = "Sweater", Description = "Warm knit sweater perfect for colder weather" });
        products.Add(new Product { Id = 16, Name = "Jacket", Description = "Waterproof jacket with breathable fabric" });
        products.Add(new Product { Id = 17, Name = "Dress", Description = "Elegant dress suitable for formal occasions" });
        products.Add(new Product { Id = 18, Name = "Shorts", Description = "Comfortable shorts for warm weather activities" });
        products.Add(new Product { Id = 19, Name = "Socks", Description = "Pack of 6 cotton socks in assorted colors" });
        products.Add(new Product { Id = 20, Name = "Hoodie", Description = "Casual pullover hoodie with front pocket" });
        products.Add(new Product { Id = 21, Name = "Swimwear", Description = "Quick-dry swimwear for beach and pool activities" });
        products.Add(new Product { Id = 22, Name = "Scarf", Description = "Soft woven scarf for added warmth and style" });
        products.Add(new Product { Id = 23, Name = "Gloves", Description = "Touchscreen compatible gloves for winter use" });

        // Books (Tag 3)
        products.Add(new Product { Id = 24, Name = "Mystery Novel", Description = "Bestselling mystery novel from acclaimed author" });
        products.Add(new Product { Id = 25, Name = "Science Fiction", Description = "Award-winning sci-fi book set in a distant future" });
        products.Add(new Product { Id = 26, Name = "Biography", Description = "Inspiring biography of a historical figure" });
        products.Add(new Product { Id = 27, Name = "Cookbook", Description = "Collection of easy and delicious recipes for beginners" });
        products.Add(new Product { Id = 28, Name = "Self-Help Book", Description = "Guide to personal growth and development" });
        products.Add(new Product { Id = 29, Name = "Business Book", Description = "Strategies for success in the modern business world" });
        products.Add(new Product { Id = 30, Name = "Travel Guide", Description = "Comprehensive guide to popular travel destinations" });
        products.Add(new Product { Id = 31, Name = "Art Book", Description = "Collection of artwork from famous artists throughout history" });
        products.Add(new Product { Id = 32, Name = "Children's Book", Description = "Illustrated storybook for young readers" });
        products.Add(new Product { Id = 33, Name = "History Book", Description = "Detailed account of important historical events" });

        // Home & Kitchen (Tag 4)
        products.Add(new Product { Id = 34, Name = "Coffee Maker", Description = "Programmable coffee maker with thermal carafe" });
        products.Add(new Product { Id = 35, Name = "Blender", Description = "High-powered blender for smoothies and food processing" });
        products.Add(new Product { Id = 36, Name = "Toaster", Description = "4-slice toaster with multiple browning settings" });
        products.Add(new Product { Id = 37, Name = "Microwave Oven", Description = "Countertop microwave with multiple cooking functions" });
        products.Add(new Product { Id = 38, Name = "Cutlery Set", Description = "24-piece stainless steel cutlery set" });
        products.Add(new Product { Id = 39, Name = "Cooking Pot Set", Description = "Non-stick cooking pot set with glass lids" });
        products.Add(new Product { Id = 40, Name = "Throw Pillow", Description = "Decorative throw pillows for couch and bed" });
        products.Add(new Product { Id = 41, Name = "Bedding Set", Description = "Soft cotton bedding set with duvet cover and pillowcases" });
        products.Add(new Product { Id = 42, Name = "Bathroom Towel Set", Description = "Set of 6 absorbent cotton bath towels" });
        products.Add(new Product { Id = 43, Name = "Area Rug", Description = "Soft area rug to enhance your living space" });

        // Sports (Tag 5)
        products.Add(new Product { Id = 44, Name = "Running Shoes", Description = "Lightweight running shoes with cushioned sole" });
        products.Add(new Product { Id = 45, Name = "Yoga Mat", Description = "Non-slip yoga mat with carrying strap" });
        products.Add(new Product { Id = 46, Name = "Dumbbells", Description = "Set of adjustable dumbbells for home workouts" });
        products.Add(new Product { Id = 47, Name = "Tennis Racket", Description = "Professional tennis racket with carrying case" });
        products.Add(new Product { Id = 48, Name = "Basketball", Description = "Official size and weight basketball" });
        products.Add(new Product { Id = 49, Name = "Cycling Helmet", Description = "Lightweight cycling helmet with adjustable fit" });
        products.Add(new Product { Id = 50, Name = "Fitness Tracker", Description = "Activity tracker with heart rate monitoring" });
        products.Add(new Product { Id = 51, Name = "Golf Clubs", Description = "Complete set of golf clubs for beginners" });
        products.Add(new Product { Id = 52, Name = "Snowboard", Description = "All-mountain snowboard for intermediate riders" });
        products.Add(new Product { Id = 53, Name = "Swimming Goggles", Description = "Anti-fog swimming goggles with UV protection" });

        // Automotive (Tag 6)
        products.Add(new Product { Id = 54, Name = "Car Vacuum", Description = "Portable vacuum cleaner for car interiors" });
        products.Add(new Product { Id = 55, Name = "Dashboard Camera", Description = "HD dashboard camera with night vision capability" });
        products.Add(new Product { Id = 56, Name = "Car Phone Mount", Description = "Adjustable phone mount for car dashboard or windshield" });
        products.Add(new Product { Id = 57, Name = "Jump Starter", Description = "Portable jump starter with built-in flashlight" });
        products.Add(new Product { Id = 58, Name = "Tire Pressure Gauge", Description = "Digital tire pressure gauge with backlit display" });
        products.Add(new Product { Id = 59, Name = "Car Wax", Description = "Premium car wax for long-lasting shine and protection" });
        products.Add(new Product { Id = 60, Name = "Car Cover", Description = "Waterproof car cover for all weather protection" });
        products.Add(new Product { Id = 61, Name = "Seat Covers", Description = "Universal fit seat covers with stain resistance" });
        products.Add(new Product { Id = 62, Name = "Floor Mats", Description = "All-weather floor mats with anti-slip backing" });
        products.Add(new Product { Id = 63, Name = "GPS Navigator", Description = "GPS navigation system with real-time traffic updates" });

        // Beauty (Tag 7)
        products.Add(new Product { Id = 64, Name = "Facial Cleanser", Description = "Gentle facial cleanser for all skin types" });
        products.Add(new Product { Id = 65, Name = "Moisturizer", Description = "Hydrating face moisturizer with SPF protection" });
        products.Add(new Product { Id = 66, Name = "Hair Dryer", Description = "Professional-grade hair dryer with multiple heat settings" });
        products.Add(new Product { Id = 67, Name = "Makeup Set", Description = "Complete makeup set with brushes and case" });
        products.Add(new Product { Id = 68, Name = "Perfume", Description = "Luxurious fragrance with long-lasting scent" });
        products.Add(new Product { Id = 69, Name = "Hair Straightener", Description = "Ceramic hair straightener with adjustable temperature" });
        products.Add(new Product { Id = 70, Name = "Face Mask", Description = "Hydrating sheet masks for weekly skin treatment" });
        products.Add(new Product { Id = 71, Name = "Nail Polish Set", Description = "Set of 10 nail polishes in trendy colors" });
        products.Add(new Product { Id = 72, Name = "Eyeshadow Palette", Description = "Versatile eyeshadow palette with matte and shimmer finishes" });
        products.Add(new Product { Id = 73, Name = "Beard Trimmer", Description = "Precision beard trimmer with multiple length settings" });

        // Health (Tag 8)
        products.Add(new Product { Id = 74, Name = "Vitamin Supplements", Description = "Daily multivitamin supplements for overall health" });
        products.Add(new Product { Id = 75, Name = "Digital Scale", Description = "Digital bathroom scale with body composition analysis" });
        products.Add(new Product { Id = 76, Name = "Blood Pressure Monitor", Description = "Home blood pressure monitor with irregular heartbeat detection" });
        products.Add(new Product { Id = 77, Name = "First Aid Kit", Description = "Comprehensive first aid kit for home and travel" });
        products.Add(new Product { Id = 78, Name = "Massage Gun", Description = "Deep tissue massage gun with multiple attachments" });
        products.Add(new Product { Id = 79, Name = "Heating Pad", Description = "Electric heating pad with multiple heat settings" });
        products.Add(new Product { Id = 80, Name = "Resistance Bands", Description = "Set of resistance bands for strength training" });
        products.Add(new Product { Id = 81, Name = "Protein Powder", Description = "Whey protein powder for post-workout recovery" });
        products.Add(new Product { Id = 82, Name = "Sleep Mask", Description = "Contoured sleep mask for better rest" });
        products.Add(new Product { Id = 83, Name = "Foam Roller", Description = "Textured foam roller for muscle recovery" });

        // Toys & Games (Tag 9)
        products.Add(new Product { Id = 84, Name = "Board Game", Description = "Family board game for ages 8 and up" });
        products.Add(new Product { Id = 85, Name = "Building Blocks", Description = "Creative building blocks set for children" });
        products.Add(new Product { Id = 86, Name = "Puzzle", Description = "1000-piece jigsaw puzzle with scenic design" });
        products.Add(new Product { Id = 87, Name = "Remote Control Car", Description = "High-speed remote control car for off-road racing" });
        products.Add(new Product { Id = 88, Name = "Stuffed Animal", Description = "Soft plush stuffed animal for children" });
        products.Add(new Product { Id = 89, Name = "Action Figure", Description = "Collectible action figure with accessories" });
        products.Add(new Product { Id = 90, Name = "Card Game", Description = "Fun card game for parties and gatherings" });
        products.Add(new Product { Id = 91, Name = "Drone", Description = "Mini drone with camera for beginners" });
        products.Add(new Product { Id = 92, Name = "Educational Toy", Description = "STEM learning toy for developing minds" });
        products.Add(new Product { Id = 93, Name = "Doll House", Description = "Detailed dollhouse with furniture and accessories" });

        // Office Supplies (Tag 10)
        products.Add(new Product { Id = 94, Name = "Notebook", Description = "Premium ruled notebook with hardcover" });
        products.Add(new Product { Id = 95, Name = "Pen Set", Description = "Set of 10 gel pens in assorted colors" });
        products.Add(new Product { Id = 96, Name = "Desk Organizer", Description = "Multi-compartment desk organizer for office supplies" });
        products.Add(new Product { Id = 97, Name = "Stapler", Description = "Heavy-duty stapler with staple remover" });
        products.Add(new Product { Id = 98, Name = "File Cabinet", Description = "Two-drawer file cabinet for document storage" });
        products.Add(new Product { Id = 99, Name = "Whiteboard", Description = "Magnetic whiteboard with marker tray" });
        products.Add(new Product { Id = 100, Name = "Paper Shredder", Description = "Cross-cut paper shredder for secure document disposal" });
        products.Add(new Product { Id = 101, Name = "Desk Lamp", Description = "LED desk lamp with adjustable brightness" });
        products.Add(new Product { Id = 102, Name = "Printer Paper", Description = "500 sheets of premium printer paper" });
        products.Add(new Product { Id = 103, Name = "Calendar", Description = "Monthly desk calendar with notes section" });

        modelBuilder.Entity<Product>().HasData(products);
    }

    private void SeedProductTags(ModelBuilder modelBuilder)
    {
        var productTags = new List<ProductTag>();

        // Original product tags
        productTags.Add(new ProductTag { ProductId = 1, TagId = 1 }); // Laptop - Electronics
        productTags.Add(new ProductTag { ProductId = 2, TagId = 2 }); // T-Shirt - Clothing
        productTags.Add(new ProductTag { ProductId = 3, TagId = 3 }); // Programming Book - Books

        // Add tags for Electronics products (4-13)
        for (int i = 4; i <= 13; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 1 }); // Electronics
        }

        // Add tags for Clothing products (14-23)
        for (int i = 14; i <= 23; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 2 }); // Clothing
        }

        // Add tags for Books products (24-33)
        for (int i = 24; i <= 33; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 3 }); // Books
        }

        // Add tags for Home & Kitchen products (34-43)
        for (int i = 34; i <= 43; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 4 }); // Home & Kitchen
        }

        // Add tags for Sports products (44-53)
        for (int i = 44; i <= 53; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 5 }); // Sports
        }

        // Add tags for Automotive products (54-63)
        for (int i = 54; i <= 63; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 6 }); // Automotive
        }

        // Add tags for Beauty products (64-73)
        for (int i = 64; i <= 73; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 7 }); // Beauty
        }

        // Add tags for Health products (74-83)
        for (int i = 74; i <= 83; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 8 }); // Health
        }

        // Add tags for Toys & Games products (84-93)
        for (int i = 84; i <= 93; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 9 }); // Toys & Games
        }

        // Add tags for Office Supplies products (94-103)
        for (int i = 94; i <= 103; i++)
        {
            productTags.Add(new ProductTag { ProductId = i, TagId = 10 }); // Office Supplies
        }

        // Add some cross-category tags to make the data more interesting
        // Some Electronics are also Home & Kitchen
        productTags.Add(new ProductTag { ProductId = 6, TagId = 4 });  // Wireless Earbuds - Home & Kitchen
        productTags.Add(new ProductTag { ProductId = 9, TagId = 4 });  // Bluetooth Speaker - Home & Kitchen
        productTags.Add(new ProductTag { ProductId = 34, TagId = 1 }); // Coffee Maker - Electronics
        productTags.Add(new ProductTag { ProductId = 35, TagId = 1 }); // Blender - Electronics

        // Some Sports items are also Health related
        productTags.Add(new ProductTag { ProductId = 45, TagId = 8 }); // Yoga Mat - Health
        productTags.Add(new ProductTag { ProductId = 46, TagId = 8 }); // Dumbbells - Health
        productTags.Add(new ProductTag { ProductId = 50, TagId = 8 }); // Fitness Tracker - Health
        productTags.Add(new ProductTag { ProductId = 80, TagId = 5 }); // Resistance Bands - Sports

        // Some items are both Electronics and Toys
        productTags.Add(new ProductTag { ProductId = 8, TagId = 9 });  // Gaming Console - Toys & Games
        productTags.Add(new ProductTag { ProductId = 91, TagId = 1 }); // Drone - Electronics

        // Some items are related to multiple categories
        productTags.Add(new ProductTag { ProductId = 27, TagId = 4 }); // Cookbook - Home & Kitchen
        productTags.Add(new ProductTag { ProductId = 50, TagId = 1 }); // Fitness Tracker - Electronics
        productTags.Add(new ProductTag { ProductId = 73, TagId = 8 }); // Beard Trimmer - Health
        productTags.Add(new ProductTag { ProductId = 96, TagId = 3 }); // Desk Organizer - Books

        modelBuilder.Entity<ProductTag>().HasData(productTags);
    }
}

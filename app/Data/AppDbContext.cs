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

        // Electronics (Tag 1) - International products
        products.Add(new Product { Id = 4, Name = "Smartphone", Description = "Latest smartphone with 6.7-inch display and 5G connectivity" });
        products.Add(new Product { Id = 5, Name = "Tablette", Description = "Tablette 10 pouces avec écran haute résolution et autonomie longue durée" });
        products.Add(new Product { Id = 6, Name = "Auriculares Inalámbricos", Description = "Auriculares Bluetooth con cancelación de ruido y 24 horas de batería" });
        products.Add(new Product { Id = 7, Name = "Smartwatch", Description = "Fitness tracking horloge met hartslagmeter en GPS" });
        products.Add(new Product { Id = 8, Name = "Console de Jeu", Description = "Console de jeu nouvelle génération avec capacités gaming 4K" });
        products.Add(new Product { Id = 9, Name = "Altavoz Bluetooth", Description = "Altavoz portátil con sonido 360 grados y diseño resistente al agua" });
        products.Add(new Product { Id = 10, Name = "Digitale Camera", Description = "DSLR camera met 24MP sensor en 4K video opname" });
        products.Add(new Product { Id = 11, Name = "Moniteur", Description = "Moniteur 4K 27 pouces avec support HDR" });
        products.Add(new Product { Id = 12, Name = "Ratón Inalámbrico", Description = "Ratón inalámbrico ergonómico con botones personalizables" });
        products.Add(new Product { Id = 13, Name = "Mechanisch Toetsenbord", Description = "RGB mechanisch toetsenbord met aanpasbare schakelaars" });

        // Clothing (Tag 2) - International products
        products.Add(new Product { Id = 14, Name = "Vaqueros", Description = "Vaqueros clásicos de mezclilla disponibles en varios cortes y lavados" });
        products.Add(new Product { Id = 15, Name = "Pull-over", Description = "Pull-over en maille chaude parfait pour les temps froids" });
        products.Add(new Product { Id = 16, Name = "Waterproof Jas", Description = "Waterdichte jas met ademende stof" });
        products.Add(new Product { Id = 17, Name = "Vestido", Description = "Vestido elegante adecuado para ocasiones formales" });
        products.Add(new Product { Id = 18, Name = "Short", Description = "Short confortable pour les activités par temps chaud" });
        products.Add(new Product { Id = 19, Name = "Sokken", Description = "Pak van 6 katoenen sokken in verschillende kleuren" });
        products.Add(new Product { Id = 20, Name = "Sudadera con Capucha", Description = "Sudadera casual con bolsillo frontal" });
        products.Add(new Product { Id = 21, Name = "Maillot de Bain", Description = "Maillot de bain à séchage rapide pour la plage et la piscine" });
        products.Add(new Product { Id = 22, Name = "Sjaal", Description = "Zachte geweven sjaal voor extra warmte en stijl" });
        products.Add(new Product { Id = 23, Name = "Guantes", Description = "Guantes compatibles con pantalla táctil para uso en invierno" });

        // Books (Tag 3) - Programming books in different languages
        products.Add(new Product { Id = 24, Name = "Programmeren in Python", Description = "Leer Python programmeren met praktische voorbeelden en oefeningen" });
        products.Add(new Product { Id = 25, Name = "JavaScript para Principiantes", Description = "Guía completa de JavaScript para desarrollo web moderno" });
        products.Add(new Product { Id = 26, Name = "Programmation Java", Description = "Maîtrisez la programmation Java avec des exemples concrets" });
        products.Add(new Product { Id = 27, Name = "C# Programming Guide", Description = "Comprehensive guide to C# programming with practical examples" });
        products.Add(new Product { Id = 28, Name = "React Development", Description = "Ontwikkel moderne web applicaties met React en Redux" });
        products.Add(new Product { Id = 29, Name = "Desarrollo con Angular", Description = "Construye aplicaciones web robustas con Angular y TypeScript" });
        products.Add(new Product { Id = 30, Name = "Développement Web avec Vue.js", Description = "Créez des applications web interactives avec Vue.js et Vuex" });
        products.Add(new Product { Id = 31, Name = "Database Design", Description = "Leer database ontwerp en SQL optimalisatie technieken" });
        products.Add(new Product { Id = 32, Name = "Algoritmos y Estructuras de Datos", Description = "Fundamentos de algoritmos y estructuras de datos en programación" });
        products.Add(new Product { Id = 33, Name = "Machine Learning en Python", Description = "Introduction au machine learning avec Python et scikit-learn" });

        // Home & Kitchen (Tag 4) - International products
        products.Add(new Product { Id = 34, Name = "Cafetera", Description = "Cafetera programable con jarra térmica" });
        products.Add(new Product { Id = 35, Name = "Mixeur", Description = "Mixeur haute puissance pour smoothies et transformation alimentaire" });
        products.Add(new Product { Id = 36, Name = "Broodrooster", Description = "4-slice broodrooster met meerdere bruiningsinstellingen" });
        products.Add(new Product { Id = 37, Name = "Horno Microondas", Description = "Microondas de encimera con múltiples funciones de cocción" });
        products.Add(new Product { Id = 38, Name = "Service de Couverts", Description = "Service de 24 couverts en acier inoxydable" });
        products.Add(new Product { Id = 39, Name = "Kookpot Set", Description = "Anti-aanbak kookpot set met glazen deksels" });
        products.Add(new Product { Id = 40, Name = "Cojín Decorativo", Description = "Cojines decorativos para sofá y cama" });
        products.Add(new Product { Id = 41, Name = "Parure de Lit", Description = "Parure de lit en coton doux avec housse de couette et taies d'oreiller" });
        products.Add(new Product { Id = 42, Name = "Handdoeken Set", Description = "Set van 6 absorberende katoenen badhanddoeken" });
        products.Add(new Product { Id = 43, Name = "Alfombra", Description = "Alfombra suave para mejorar tu espacio de vida" });

        // Sports (Tag 5) - International products
        products.Add(new Product { Id = 44, Name = "Zapatillas de Running", Description = "Zapatillas ligeras para correr con suela acolchada" });
        products.Add(new Product { Id = 45, Name = "Tapis de Yoga", Description = "Tapis de yoga antidérapant avec sangle de transport" });
        products.Add(new Product { Id = 46, Name = "Halters", Description = "Set van verstelbare halters voor thuis workouts" });
        products.Add(new Product { Id = 47, Name = "Raqueta de Tenis", Description = "Raqueta de tenis profesional con funda de transporte" });
        products.Add(new Product { Id = 48, Name = "Ballon de Basketball", Description = "Ballon de basketball taille et poids officiels" });
        products.Add(new Product { Id = 49, Name = "Fietshelm", Description = "Lichtgewicht fietshelm met verstelbare pasvorm" });
        products.Add(new Product { Id = 50, Name = "Monitor de Actividad", Description = "Rastreador de actividad con monitoreo de frecuencia cardíaca" });
        products.Add(new Product { Id = 51, Name = "Clubs de Golf", Description = "Set complet de clubs de golf pour débutants" });
        products.Add(new Product { Id = 52, Name = "Snowboard", Description = "All-mountain snowboard voor gevorderde rijders" });
        products.Add(new Product { Id = 53, Name = "Gafas de Natación", Description = "Gafas de natación anti-empañamiento con protección UV" });

        // Automotive (Tag 6) - International products
        products.Add(new Product { Id = 54, Name = "Aspiradora para Coche", Description = "Aspiradora portátil para interiores de automóviles" });
        products.Add(new Product { Id = 55, Name = "Caméra de Tableau de Bord", Description = "Caméra de tableau de bord HD avec capacité de vision nocturne" });
        products.Add(new Product { Id = 56, Name = "Auto Telefoonhouder", Description = "Verstelbare telefoonhouder voor autodashboard of voorruit" });
        products.Add(new Product { Id = 57, Name = "Arrancador Portátil", Description = "Arrancador portátil con linterna integrada" });
        products.Add(new Product { Id = 58, Name = "Manomètre de Pneu", Description = "Manomètre de pression des pneus numérique avec écran rétroéclairé" });
        products.Add(new Product { Id = 59, Name = "Autowas", Description = "Premium autowas voor langdurige glans en bescherming" });
        products.Add(new Product { Id = 60, Name = "Funda para Coche", Description = "Funda impermeable para protección en todo tipo de clima" });
        products.Add(new Product { Id = 61, Name = "Housses de Siège", Description = "Housses de siège universelles avec résistance aux taches" });
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

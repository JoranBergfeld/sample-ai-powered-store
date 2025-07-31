using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTag",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTag", x => new { x.ProductId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ProductTag_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "High-performance laptop with 16GB RAM and 512GB SSD", "Laptop" },
                    { 2, "Cotton t-shirt, available in multiple colors", "T-Shirt" },
                    { 3, "Learn C# programming with practical examples", "Programming Book" },
                    { 4, "Latest smartphone with 6.7-inch display and 5G connectivity", "Smartphone" },
                    { 5, "Tablette 10 pouces avec écran haute résolution et autonomie longue durée", "Tablette" },
                    { 6, "Auriculares Bluetooth con cancelación de ruido y 24 horas de batería", "Auriculares Inalámbricos" },
                    { 7, "Fitness tracking horloge met hartslagmeter en GPS", "Smartwatch" },
                    { 8, "Console de jeu nouvelle génération avec capacités gaming 4K", "Console de Jeu" },
                    { 9, "Altavoz portátil con sonido 360 grados y diseño resistente al agua", "Altavoz Bluetooth" },
                    { 10, "DSLR camera met 24MP sensor en 4K video opname", "Digitale Camera" },
                    { 11, "Moniteur 4K 27 pouces avec support HDR", "Moniteur" },
                    { 12, "Ratón inalámbrico ergonómico con botones personalizables", "Ratón Inalámbrico" },
                    { 13, "RGB mechanisch toetsenbord met aanpasbare schakelaars", "Mechanisch Toetsenbord" },
                    { 14, "Vaqueros clásicos de mezclilla disponibles en varios cortes y lavados", "Vaqueros" },
                    { 15, "Pull-over en maille chaude parfait pour les temps froids", "Pull-over" },
                    { 16, "Waterdichte jas met ademende stof", "Waterproof Jas" },
                    { 17, "Vestido elegante adecuado para ocasiones formales", "Vestido" },
                    { 18, "Short confortable pour les activités par temps chaud", "Short" },
                    { 19, "Pak van 6 katoenen sokken in verschillende kleuren", "Sokken" },
                    { 20, "Sudadera casual con bolsillo frontal", "Sudadera con Capucha" },
                    { 21, "Maillot de bain à séchage rapide pour la plage et la piscine", "Maillot de Bain" },
                    { 22, "Zachte geweven sjaal voor extra warmte en stijl", "Sjaal" },
                    { 23, "Guantes compatibles con pantalla táctil para uso en invierno", "Guantes" },
                    { 24, "Leer Python programmeren met praktische voorbeelden en oefeningen", "Programmeren in Python" },
                    { 25, "Guía completa de JavaScript para desarrollo web moderno", "JavaScript para Principiantes" },
                    { 26, "Maîtrisez la programmation Java avec des exemples concrets", "Programmation Java" },
                    { 27, "Comprehensive guide to C# programming with practical examples", "C# Programming Guide" },
                    { 28, "Ontwikkel moderne web applicaties met React en Redux", "React Development" },
                    { 29, "Construye aplicaciones web robustas con Angular y TypeScript", "Desarrollo con Angular" },
                    { 30, "Créez des applications web interactives avec Vue.js et Vuex", "Développement Web avec Vue.js" },
                    { 31, "Leer database ontwerp en SQL optimalisatie technieken", "Database Design" },
                    { 32, "Fundamentos de algoritmos y estructuras de datos en programación", "Algoritmos y Estructuras de Datos" },
                    { 33, "Introduction au machine learning avec Python et scikit-learn", "Machine Learning en Python" },
                    { 34, "Cafetera programable con jarra térmica", "Cafetera" },
                    { 35, "Mixeur haute puissance pour smoothies et transformation alimentaire", "Mixeur" },
                    { 36, "4-slice broodrooster met meerdere bruiningsinstellingen", "Broodrooster" },
                    { 37, "Microondas de encimera con múltiples funciones de cocción", "Horno Microondas" },
                    { 38, "Service de 24 couverts en acier inoxydable", "Service de Couverts" },
                    { 39, "Anti-aanbak kookpot set met glazen deksels", "Kookpot Set" },
                    { 40, "Cojines decorativos para sofá y cama", "Cojín Decorativo" },
                    { 41, "Parure de lit en coton doux avec housse de couette et taies d'oreiller", "Parure de Lit" },
                    { 42, "Set van 6 absorberende katoenen badhanddoeken", "Handdoeken Set" },
                    { 43, "Alfombra suave para mejorar tu espacio de vida", "Alfombra" },
                    { 44, "Zapatillas ligeras para correr con suela acolchada", "Zapatillas de Running" },
                    { 45, "Tapis de yoga antidérapant avec sangle de transport", "Tapis de Yoga" },
                    { 46, "Set van verstelbare halters voor thuis workouts", "Halters" },
                    { 47, "Raqueta de tenis profesional con funda de transporte", "Raqueta de Tenis" },
                    { 48, "Ballon de basketball taille et poids officiels", "Ballon de Basketball" },
                    { 49, "Lichtgewicht fietshelm met verstelbare pasvorm", "Fietshelm" },
                    { 50, "Rastreador de actividad con monitoreo de frecuencia cardíaca", "Monitor de Actividad" },
                    { 51, "Set complet de clubs de golf pour débutants", "Clubs de Golf" },
                    { 52, "All-mountain snowboard voor gevorderde rijders", "Snowboard" },
                    { 53, "Gafas de natación anti-empañamiento con protección UV", "Gafas de Natación" },
                    { 54, "Aspiradora portátil para interiores de automóviles", "Aspiradora para Coche" },
                    { 55, "Caméra de tableau de bord HD avec capacité de vision nocturne", "Caméra de Tableau de Bord" },
                    { 56, "Verstelbare telefoonhouder voor autodashboard of voorruit", "Auto Telefoonhouder" },
                    { 57, "Arrancador portátil con linterna integrada", "Arrancador Portátil" },
                    { 58, "Manomètre de pression des pneus numérique avec écran rétroéclairé", "Manomètre de Pneu" },
                    { 59, "Premium autowas voor langdurige glans en bescherming", "Autowas" },
                    { 60, "Funda impermeable para protección en todo tipo de clima", "Funda para Coche" },
                    { 61, "Housses de siège universelles avec résistance aux taches", "Housses de Siège" },
                    { 62, "All-weather floor mats with anti-slip backing", "Floor Mats" },
                    { 63, "GPS navigation system with real-time traffic updates", "GPS Navigator" },
                    { 64, "Gentle facial cleanser for all skin types", "Facial Cleanser" },
                    { 65, "Hydrating face moisturizer with SPF protection", "Moisturizer" },
                    { 66, "Professional-grade hair dryer with multiple heat settings", "Hair Dryer" },
                    { 67, "Complete makeup set with brushes and case", "Makeup Set" },
                    { 68, "Luxurious fragrance with long-lasting scent", "Perfume" },
                    { 69, "Ceramic hair straightener with adjustable temperature", "Hair Straightener" },
                    { 70, "Hydrating sheet masks for weekly skin treatment", "Face Mask" },
                    { 71, "Set of 10 nail polishes in trendy colors", "Nail Polish Set" },
                    { 72, "Versatile eyeshadow palette with matte and shimmer finishes", "Eyeshadow Palette" },
                    { 73, "Precision beard trimmer with multiple length settings", "Beard Trimmer" },
                    { 74, "Daily multivitamin supplements for overall health", "Vitamin Supplements" },
                    { 75, "Digital bathroom scale with body composition analysis", "Digital Scale" },
                    { 76, "Home blood pressure monitor with irregular heartbeat detection", "Blood Pressure Monitor" },
                    { 77, "Comprehensive first aid kit for home and travel", "First Aid Kit" },
                    { 78, "Deep tissue massage gun with multiple attachments", "Massage Gun" },
                    { 79, "Electric heating pad with multiple heat settings", "Heating Pad" },
                    { 80, "Set of resistance bands for strength training", "Resistance Bands" },
                    { 81, "Whey protein powder for post-workout recovery", "Protein Powder" },
                    { 82, "Contoured sleep mask for better rest", "Sleep Mask" },
                    { 83, "Textured foam roller for muscle recovery", "Foam Roller" },
                    { 84, "Family board game for ages 8 and up", "Board Game" },
                    { 85, "Creative building blocks set for children", "Building Blocks" },
                    { 86, "1000-piece jigsaw puzzle with scenic design", "Puzzle" },
                    { 87, "High-speed remote control car for off-road racing", "Remote Control Car" },
                    { 88, "Soft plush stuffed animal for children", "Stuffed Animal" },
                    { 89, "Collectible action figure with accessories", "Action Figure" },
                    { 90, "Fun card game for parties and gatherings", "Card Game" },
                    { 91, "Mini drone with camera for beginners", "Drone" },
                    { 92, "STEM learning toy for developing minds", "Educational Toy" },
                    { 93, "Detailed dollhouse with furniture and accessories", "Doll House" },
                    { 94, "Premium ruled notebook with hardcover", "Notebook" },
                    { 95, "Set of 10 gel pens in assorted colors", "Pen Set" },
                    { 96, "Multi-compartment desk organizer for office supplies", "Desk Organizer" },
                    { 97, "Heavy-duty stapler with staple remover", "Stapler" },
                    { 98, "Two-drawer file cabinet for document storage", "File Cabinet" },
                    { 99, "Magnetic whiteboard with marker tray", "Whiteboard" },
                    { 100, "Cross-cut paper shredder for secure document disposal", "Paper Shredder" },
                    { 101, "LED desk lamp with adjustable brightness", "Desk Lamp" },
                    { 102, "500 sheets of premium printer paper", "Printer Paper" },
                    { 103, "Monthly desk calendar with notes section", "Calendar" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Clothing" },
                    { 3, "Books" },
                    { 4, "Home & Kitchen" },
                    { 5, "Sports" },
                    { 6, "Automotive" },
                    { 7, "Beauty" },
                    { 8, "Health" },
                    { 9, "Toys & Games" },
                    { 10, "Office Supplies" }
                });

            migrationBuilder.InsertData(
                table: "ProductTag",
                columns: new[] { "ProductId", "TagId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 6, 4 },
                    { 7, 1 },
                    { 8, 1 },
                    { 8, 9 },
                    { 9, 1 },
                    { 9, 4 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 2 },
                    { 15, 2 },
                    { 16, 2 },
                    { 17, 2 },
                    { 18, 2 },
                    { 19, 2 },
                    { 20, 2 },
                    { 21, 2 },
                    { 22, 2 },
                    { 23, 2 },
                    { 24, 3 },
                    { 25, 3 },
                    { 26, 3 },
                    { 27, 3 },
                    { 27, 4 },
                    { 28, 3 },
                    { 29, 3 },
                    { 30, 3 },
                    { 31, 3 },
                    { 32, 3 },
                    { 33, 3 },
                    { 34, 1 },
                    { 34, 4 },
                    { 35, 1 },
                    { 35, 4 },
                    { 36, 4 },
                    { 37, 4 },
                    { 38, 4 },
                    { 39, 4 },
                    { 40, 4 },
                    { 41, 4 },
                    { 42, 4 },
                    { 43, 4 },
                    { 44, 5 },
                    { 45, 5 },
                    { 45, 8 },
                    { 46, 5 },
                    { 46, 8 },
                    { 47, 5 },
                    { 48, 5 },
                    { 49, 5 },
                    { 50, 1 },
                    { 50, 5 },
                    { 50, 8 },
                    { 51, 5 },
                    { 52, 5 },
                    { 53, 5 },
                    { 54, 6 },
                    { 55, 6 },
                    { 56, 6 },
                    { 57, 6 },
                    { 58, 6 },
                    { 59, 6 },
                    { 60, 6 },
                    { 61, 6 },
                    { 62, 6 },
                    { 63, 6 },
                    { 64, 7 },
                    { 65, 7 },
                    { 66, 7 },
                    { 67, 7 },
                    { 68, 7 },
                    { 69, 7 },
                    { 70, 7 },
                    { 71, 7 },
                    { 72, 7 },
                    { 73, 7 },
                    { 73, 8 },
                    { 74, 8 },
                    { 75, 8 },
                    { 76, 8 },
                    { 77, 8 },
                    { 78, 8 },
                    { 79, 8 },
                    { 80, 5 },
                    { 80, 8 },
                    { 81, 8 },
                    { 82, 8 },
                    { 83, 8 },
                    { 84, 9 },
                    { 85, 9 },
                    { 86, 9 },
                    { 87, 9 },
                    { 88, 9 },
                    { 89, 9 },
                    { 90, 9 },
                    { 91, 1 },
                    { 91, 9 },
                    { 92, 9 },
                    { 93, 9 },
                    { 94, 10 },
                    { 95, 10 },
                    { 96, 3 },
                    { 96, 10 },
                    { 97, 10 },
                    { 98, 10 },
                    { 99, 10 },
                    { 100, 10 },
                    { 101, 10 },
                    { 102, 10 },
                    { 103, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTag_TagId",
                table: "ProductTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTag");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}

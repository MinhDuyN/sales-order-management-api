using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveDefaultsMinimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON c.default_object_id = dc.object_id
    JOIN sys.tables t ON t.object_id = c.object_id
    WHERE t.name = 'Users' AND c.name = 'IsActive'
)
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [DF_Users_IsActive] DEFAULT ((1)) FOR [IsActive];
END;
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c ON c.default_object_id = dc.object_id
    JOIN sys.tables t ON t.object_id = c.object_id
    WHERE t.name = 'Products' AND c.name = 'IsActive'
)
BEGIN
    ALTER TABLE [Products] ADD CONSTRAINT [DF_Products_IsActive] DEFAULT ((1)) FOR [IsActive];
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @dfUsers NVARCHAR(128);
SELECT @dfUsers = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON c.default_object_id = dc.object_id
JOIN sys.tables t ON t.object_id = c.object_id
WHERE t.name = 'Users' AND c.name = 'IsActive';

IF @dfUsers IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @dfUsers + ']');
END;
");

            migrationBuilder.Sql(@"
DECLARE @dfProducts NVARCHAR(128);
SELECT @dfProducts = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON c.default_object_id = dc.object_id
JOIN sys.tables t ON t.object_id = c.object_id
WHERE t.name = 'Products' AND c.name = 'IsActive';

IF @dfProducts IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [Products] DROP CONSTRAINT [' + @dfProducts + ']');
END;
");
        }
    }
}

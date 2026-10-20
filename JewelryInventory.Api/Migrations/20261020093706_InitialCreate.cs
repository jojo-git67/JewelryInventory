using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JewelryInventory.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    UnitOfMeasurement = table.Column<string>(type: "TEXT", nullable: false),
                    UnitCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    LowStockThreshold = table.Column<decimal>(type: "TEXT", nullable: false),
                    StorageLocation = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiveOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiveOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualAdjustments_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceiveOrderRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReceiveOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "TEXT", nullable: false),
                    UnitPurchasePrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiveOrderRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiveOrderRows_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiveOrderRows_ReceiveOrders_ReceiveOrderId",
                        column: x => x.ReceiveOrderId,
                        principalTable: "ReceiveOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComponentInventoryMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComponentId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceiveOrderRowId = table.Column<int>(type: "INTEGER", nullable: true),
                    ManualAdjustmentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentInventoryMovements", x => x.Id);
                    table.CheckConstraint("CK_InventoryMovement_ExactlyOneSource", "(ReceiveOrderRowId IS NOT NULL AND ManualAdjustmentId IS NULL)\nOR\n(ReceiveOrderRowId IS NULL AND ManualAdjustmentId IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ComponentInventoryMovements_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComponentInventoryMovements_ManualAdjustments_ManualAdjustmentId",
                        column: x => x.ManualAdjustmentId,
                        principalTable: "ManualAdjustments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComponentInventoryMovements_ReceiveOrderRows_ReceiveOrderRowId",
                        column: x => x.ReceiveOrderRowId,
                        principalTable: "ReceiveOrderRows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentInventoryMovements_ComponentId",
                table: "ComponentInventoryMovements",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentInventoryMovements_ManualAdjustmentId",
                table: "ComponentInventoryMovements",
                column: "ManualAdjustmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentInventoryMovements_ReceiveOrderRowId",
                table: "ComponentInventoryMovements",
                column: "ReceiveOrderRowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Components_Barcode",
                table: "Components",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManualAdjustments_ComponentId",
                table: "ManualAdjustments",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveOrderRows_ComponentId",
                table: "ReceiveOrderRows",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveOrderRows_ReceiveOrderId",
                table: "ReceiveOrderRows",
                column: "ReceiveOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentInventoryMovements");

            migrationBuilder.DropTable(
                name: "ManualAdjustments");

            migrationBuilder.DropTable(
                name: "ReceiveOrderRows");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "ReceiveOrders");
        }
    }
}

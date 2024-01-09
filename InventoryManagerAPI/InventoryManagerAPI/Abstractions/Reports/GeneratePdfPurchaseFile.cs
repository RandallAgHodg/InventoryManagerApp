using InventoryManagerAPI.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace InventoryManagerAPI.Abstractions.Reports;

public class PurchaseReportCreator
{
    public static byte[]? PurchasePdfReport(Purchase purchase)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header()
                    .AlignCenter()
                    .Row(row =>
                    {
                        row
                        .RelativeItem()
                        .Element(container => container.AlignLeft())
                        .Text($"Purchase - {purchase.Id}")
                        .SemiBold().FontSize(10);

                        row
                        .RelativeItem()
                        .Element(container => container.AlignRight())
                        .Column(column => {
                            column.Spacing(5);
                            column.Item().Text($"{purchase.User.FirstName} {purchase.User.LastName}").ExtraLight().FontSize(9);
                            column.Item().Text($"{purchase.CreatedAt.ToString("dddd d 'of' MMMM 'of' yyyy 'at' h:mm:ss tt", new CultureInfo("en-US"))}").FontSize(8);
                        });
                    });

                    

                
                page.Content()             
                 .Table(table =>
                 {
                     table.ColumnsDefinition(columns =>
                     {
                         columns.RelativeColumn();
                         columns.RelativeColumn();
                         columns.RelativeColumn();
                         columns.RelativeColumn();
                     });

                     table.Header(header =>
                     {
                         header.Cell().Element(CellStyle).Text("Product");
                         header.Cell().Element(CellStyle).AlignRight().Text("Unit Cost");
                         header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                         header.Cell().Element(CellStyle).AlignRight().Text("Subtotal");
                     });

                     foreach (var detail in purchase.PurchaseDetails)
                     {
                         table.Cell().Element(CellStyle).AlignLeft().Text(detail.Product.Name).FontSize(14);
                         table.Cell().Element(CellStyle).AlignRight().Text($"{detail.Product.Cost} $").FontSize(14);
                         table.Cell().Element(CellStyle).AlignRight().Text($"{detail.Amount}").FontSize(14);
                         table.Cell().Element(CellStyle).AlignRight().Text($"{detail.Subtotal} $").FontSize(14);

                     }

                     table.Footer(footer =>
                     {
                         footer
                         .Cell()
                         .Element(container => container.Padding(10))
                         .AlignRight()
                         .Text($"Total - {purchase.Total} $")
                         .ExtraBold()
                         .FontSize(12);
                     });
                 });
            });
        }).GeneratePdf();

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }
    }
}

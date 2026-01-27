using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DayNote.Models;

namespace DayNote.Services;

public class PdfExportService
{
    public byte[] BuildPdf(List<JournalEntry> entries, DateTime from, DateTime to)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                page.Header().Text($"Journal Export ({from:yyyy-MM-dd} → {to:yyyy-MM-dd})")
                             .FontSize(18).Bold();

                page.Content().Column(col =>
                {
                    foreach (var e in entries)
                    {
                        col.Item().PaddingVertical(8).BorderBottom(1).Column(c =>
                        {
                            c.Item().Text(e.EntryDate.ToString("yyyy-MM-dd")).Bold();
                            c.Item().Text(e.Content ?? "");
                        });
                    }
                });

                page.Footer().AlignCenter()
                    .Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
            });
        }).GeneratePdf();
    }
}

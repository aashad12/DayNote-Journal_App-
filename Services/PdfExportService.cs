// QuestPDF namespaces for building PDF documents
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
// Import JournalEntry model
using DayNote.Models;

namespace DayNote.Services;

// Service class responsible for exporting journal entries as PDF
public class PdfExportService
{
    // Builds and returns a PDF file as a byte array
    public byte[] BuildPdf(List<JournalEntry> entries, DateTime from, DateTime to)
    {
        // Set QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(doc =>
        {
            // Define PDF page layout
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                // PDF header showing selected date range
                page.Header().Text($"Journal Export ({from:yyyy-MM-dd} → {to:yyyy-MM-dd})")
                             .FontSize(18).Bold();

                // Main content: loop through journal entries and display them
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
                // Footer section of the PDF
                page.Footer().AlignCenter()
                    .Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
            });
            // Generate the PDF and return it as byte array
        }).GeneratePdf();
    }
}

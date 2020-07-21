using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using VeraCustomTriage.Logic.Models;
using VeraCustomTriage.Shared.Helpers;

namespace VeraCustomTriage.Logic
{
    public interface IOutputWriter
    {
        MemoryStream Write(Report results);
    }
    public class ExcelWriter : IOutputWriter
    {
        private string[] alpha = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
        private string[] severity = new[] { "Informational", "Informational", "Low", "Medium", "High", "Very High" };
        private string[] _fields
        {
            get
            {
                var tempfields = typeof(LineItem).GetProperties().Select(x => x.Name).ToList();
                tempfields.Add("DaysSinceFound");
                return tempfields.ToArray();
            }
        }
        private ExcelConfiguration _config { get; set; }

        public ExcelWriter(IOptions<ExcelConfiguration> config)
        {
            _config = config.Value;
        }
        public MemoryStream Write(Report report)
        {
            var lineItems = report.FlawsAndResponses.Select(x => new LineItem
            {
                Severity = severity[Int32.Parse(x.Key.severity)],
                FileName = x.Key.sourcefile,
                FilePath = x.Key.sourcefilepath,
                FlawId = x.Key.issueid,
                CategoryDescription = x.Key.categoryname,
                ModuleName = x.Key.module,
                ActionToTake = string.Join("\n\n", x.Value.Select(x => x.ActionToTake).Distinct().ToArray()),
                SecurityTeamComments = string.Join("\n\n", x.Value.Select(x => x.Response).ToArray()),
                LineNumber = x.Key.line,
                DevelopmentTeamComments = "[Please enter your remediation plan here.]",
                EstimatedRemediationDate = "[Please enter your estimated date to remediate this flaw.]",
                DateFound = DateHelper.GetDate(x.Key.date_first_occurrence),
            }).ToArray();

            var mem = new MemoryStream();

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "RemediationPlan"
            };

            SheetData sheetData = new SheetData();
            sheetData.Append(GenerateHeaderRow());
            for (var i = 0; i < lineItems.Count(); i++)
                sheetData.Append(GenerateFlawRow(i + 2, lineItems[i]));

            sheets.Append(sheet);

            worksheetPart.Worksheet = new Worksheet(sheetData);

            DefineTable(worksheetPart, lineItems);

            workbookpart.Workbook.Save();
            spreadsheetDocument.Close();
            return mem;

        }

        private Row GenerateHeaderRow()
        {
            Row row = new Row() { RowIndex = 1 };
            for (var i = 0; i < _fields.Length; i++)
            {
                Cell cell = new Cell()
                {
                    CellReference = alpha[i] + $"{1}",
                    CellValue = new CellValue(_fields[i]),
                    DataType = CellValues.String
                };
                row.Append(cell);
            }
            return row;
        }

        private Row GenerateFlawRow(int index, LineItem lineItem)
        {
            uint rowIndex = (uint)index;
            Row row = new Row() { RowIndex = rowIndex };
            var fields = typeof(LineItem).GetProperties();
            for (var i = 0; i < fields.Length; i++)
            {
                Cell cell = new Cell()
                {
                    CellReference = alpha[i] + $"{index}",
                    CellValue = new CellValue(fields[i].GetValue(lineItem).ToString()),
                    DataType = CellValues.String
                };
                row.Append(cell);
            }

            Cell cell2 = new Cell()
            {
                CellReference = alpha[fields.Length] + $"{index}",
                DataType = new EnumValue<CellValues>(CellValues.Number),
                CellFormula = new CellFormula($"=DATEDIF(D{index},TODAY(),\"D\")")
            };
            row.Append(cell2);
            return row;
        }

        private void SetupStyleSheet(WorkbookPart workbookpart)
        {
            var stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();

            stylesPart.Stylesheet = new Stylesheet();
            stylesPart.Stylesheet.Fonts = new Fonts();
            stylesPart.Stylesheet.Fonts.AppendChild(new Font());
            stylesPart.Stylesheet.Fills = new Fills();

            var veryhigh = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.VeryHighRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            var high = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.HighRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            var medium = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.MediumRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            var low = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.LowRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            var informational = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.InformationalRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            var highlight = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.HighlightRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            var header = new PatternFill()
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString(_config.HeaderRgbHex) },
                BackgroundColor = new BackgroundColor { Indexed = 64 }
            };

            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = veryhigh });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = high });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = medium });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = low });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = informational });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = highlight });
            stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = header });
            stylesPart.Stylesheet.Borders = new Borders();
            stylesPart.Stylesheet.Borders.AppendChild(new Border());
            stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
            stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());
            stylesPart.Stylesheet.CellFormats = new CellFormats();
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 2, ApplyFill = true });
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 3, ApplyFill = true });
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 4, ApplyFill = true });
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 5, ApplyFill = true });
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 6, ApplyFill = true });
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 7, ApplyFill = true });
            stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 8, ApplyFill = true });
            stylesPart.Stylesheet.Save();
        }
        private uint GetStyleIndex(LineItem item, string field)
        {
            if (field.Equals("DevelopmentTeamComments")
                || field.Equals("EstimatedRemediationDate"))
                return 6;

            if (item.Severity.Equals("Very High"))
                return 1;

            if (item.Severity.Equals("High"))
                return 2;

            if (item.Severity.Equals("Medium"))
                return 3;

            if (item.Severity.Equals("Low"))
                return 4;

            if (item.Severity.Equals("Informational"))
                return 5;

            return 1;
        }

        private void DefineTable(WorksheetPart worksheetPart, LineItem[] lineItems)
        {
            TableDefinitionPart tableDefinitionPart = worksheetPart.AddNewPart<TableDefinitionPart>("rId" + (worksheetPart.TableDefinitionParts.Count() + 1));
            int tableNo = worksheetPart.TableDefinitionParts.Count();

            string reference = $"A1:{((char)(64 + _fields.Length))}{lineItems.Length}";

            Table table = new Table() { Id = (UInt32)tableNo, Name = "Table" + tableNo, DisplayName = "Table" + tableNo, Reference = reference, TotalsRowShown = false };
            AutoFilter autoFilter = new AutoFilter() { Reference = reference };


            TableColumns tableColumns = new TableColumns() { Count = (UInt32)_fields.Length };
            for (int i = 0; i < _fields.Length; i++)
            {
                tableColumns.Append(new TableColumn() { Id = (UInt32)(i + 1), Name = _fields[i] });
            }

            table.Append(autoFilter);
            table.Append(tableColumns);
            tableDefinitionPart.Table = table;

            TableParts tableParts = new TableParts() { Count = (UInt32)1 };
            TablePart tablePart = new TablePart() { Id = "rId" + tableNo };

            tableParts.Append(tablePart);

            worksheetPart.Worksheet.Append(tableParts);
        }
    }
}

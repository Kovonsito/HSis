using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using HSis.Logic.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HSis.Logic.Services
{
    public class ReportExportService : IReportExportService
    {
        static ReportExportService()
        {
            // Establecer licencia para QuestPDF antes de cualquier invocación
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // Generar archivo Excel con ClosedXML
        public byte[] GenerarExcel(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
        {
            using var workbook = new XLWorkbook();

            // 1. Pestaña de Resumen y KPIs
            var wsKpis = workbook.Worksheets.Add("Resumen KPIs");
            wsKpis.ShowGridLines = false;

            // Título
            var cellTitle = wsKpis.Cell("B2");
            cellTitle.Value = "HSis - Reporte de Incidencias y KPIs";
            cellTitle.Style.Font.FontSize = 16;
            cellTitle.Style.Font.Bold = true;
            cellTitle.Style.Font.FontColor = XLColor.FromHtml("#1F2937");

            var cellSub = wsKpis.Cell("B3");
            cellSub.Value = $"Periodo del {inicio:dd/MM/yyyy} al {fin:dd/MM/yyyy} | Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";
            cellSub.Style.Font.Italic = true;
            cellSub.Style.Font.FontColor = XLColor.FromHtml("#4B5563");

            // KPI Cards Layout (B5:E6)
            string[] kpiTitles = ["TICKETS CREADOS", "TICKETS RESUELTOS", "TASA DE CIERRE", "TIEMPO PROM. ATENCIÓN"];
            string[] kpiValues = [kpis.TotalCreados.ToString(), kpis.TotalResueltos.ToString(), $"{kpis.TasaCierre:F1}%", $"{kpis.TiempoPromedioAtencionHoras:F1} Hrs"];
            string[] kpiColors = ["#3B82F6", "#10B981", "#F59E0B", "#8B5CF6"];

            for (int i = 0; i < 4; i++)
            {
                var colLetter = (char)('B' + i);

                var cellVal = wsKpis.Cell($"{colLetter}5");
                cellVal.Value = kpiValues[i];
                cellVal.Style.Font.FontSize = 18;
                cellVal.Style.Font.Bold = true;
                cellVal.Style.Font.FontColor = XLColor.FromHtml(kpiColors[i]);
                cellVal.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                var cellLbl = wsKpis.Cell($"{colLetter}6");
                cellLbl.Value = kpiTitles[i];
                cellLbl.Style.Font.FontSize = 9;
                cellLbl.Style.Font.Bold = true;
                cellLbl.Style.Font.FontColor = XLColor.FromHtml("#9CA3AF");
                cellLbl.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Bordes para la tarjeta
                var range = wsKpis.Range($"{colLetter}5:{colLetter}6");
                range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.FromHtml("#E5E7EB"));
                range.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F9FAFB"));
            }

            // Tablas de Productividad Técnica
            var cellProdTitle = wsKpis.Cell("B9");
            cellProdTitle.Value = "Productividad del Personal Técnico";
            cellProdTitle.Style.Font.FontSize = 12;
            cellProdTitle.Style.Font.Bold = true;
            cellProdTitle.Style.Font.FontColor = XLColor.FromHtml("#111827");

            var rowIdx = 11;
            wsKpis.Cell("B10").Value = "Técnico";
            wsKpis.Cell("C10").Value = "Asignados";
            wsKpis.Cell("D10").Value = "Resueltos";
            wsKpis.Cell("E10").Value = "Tasa de Cierre";

            var headerRange = wsKpis.Range("B10:E10");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1F2937"));
            headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            foreach (var tech in kpis.ProductividadTecnica)
            {
                wsKpis.Cell($"B{rowIdx}").Value = tech.Tecnico;
                wsKpis.Cell($"C{rowIdx}").Value = tech.TicketsAsignados;
                wsKpis.Cell($"D{rowIdx}").Value = tech.TicketsResueltos;
                wsKpis.Cell($"E{rowIdx}").Value = tech.TasaCierre / 100.0;
                wsKpis.Cell($"E{rowIdx}").Style.NumberFormat.Format = "0.0%";

                wsKpis.Range($"B{rowIdx}:E{rowIdx}").Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.FromHtml("#F3F4F6"));
                wsKpis.Cell($"C{rowIdx}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                wsKpis.Cell($"D{rowIdx}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                wsKpis.Cell($"E{rowIdx}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                rowIdx++;
            }

            // Tablas de Demanda por Departamento
            var cellDptoTitle = wsKpis.Cell("G9");
            cellDptoTitle.Value = "Incidencias por Departamento";
            cellDptoTitle.Style.Font.FontSize = 12;
            cellDptoTitle.Style.Font.Bold = true;
            cellDptoTitle.Style.Font.FontColor = XLColor.FromHtml("#111827");

            wsKpis.Cell("G10").Value = "Departamento";
            wsKpis.Cell("H10").Value = "Tickets";
            wsKpis.Cell("I10").Value = "% Del Total";

            var dptoHeader = wsKpis.Range("G10:I10");
            dptoHeader.Style.Font.Bold = true;
            dptoHeader.Style.Font.FontColor = XLColor.White;
            dptoHeader.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#3B82F6"));
            dptoHeader.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var dptoRow = 11;
            foreach (var dpto in kpis.DemandaDepartamentos)
            {
                wsKpis.Cell($"G{dptoRow}").Value = dpto.Departamento;
                wsKpis.Cell($"H{dptoRow}").Value = dpto.TotalTickets;
                wsKpis.Cell($"I{dptoRow}").Value = dpto.PorcentajeDelTotal / 100.0;
                wsKpis.Cell($"I{dptoRow}").Style.NumberFormat.Format = "0.0%";

                wsKpis.Range($"G{dptoRow}:I{dptoRow}").Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.FromHtml("#F3F4F6"));
                wsKpis.Cell($"H{dptoRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                wsKpis.Cell($"I{dptoRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                dptoRow++;
            }

            wsKpis.Columns("B:J").AdjustToContents();

            // 2. Pestaña de Detalle de Tickets
            var wsTickets = workbook.Worksheets.Add("Listado de Tickets");
            wsTickets.ShowGridLines = true;

            // Cabeceras
            string[] headers = ["Folio", "Usuario Emisor", "Departamento", "Estatus", "Prioridad", "Fecha de Alta", "Fecha de Atención", "Fecha de Cierre", "Personal de Seguimiento", "Descripción", "Solución"];
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = wsTickets.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1F2937"));
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }

            var tRowIdx = 2;
            foreach (var t in tickets)
            {
                wsTickets.Cell(tRowIdx, 1).Value = t.IdTicket;
                wsTickets.Cell(tRowIdx, 2).Value = t.NombreUsuario;
                wsTickets.Cell(tRowIdx, 3).Value = t.DepartamentoUsuario;

                var cellStatus = wsTickets.Cell(tRowIdx, 4);
                cellStatus.Value = t.Status ?? "N/A";
                // Aplicar estilo condicional básico según el estatus
                if (t.Status == "Cerrado")
                {
                    cellStatus.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#D1FAE5"));
                    cellStatus.Style.Font.FontColor = XLColor.FromHtml("#065F46");
                }
                else if (t.Status == "Abierto" || t.Status == "Reabierto")
                {
                    cellStatus.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#FEE2E2"));
                    cellStatus.Style.Font.FontColor = XLColor.FromHtml("#991B1B");
                }
                else
                {
                    cellStatus.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#FEF3C7"));
                    cellStatus.Style.Font.FontColor = XLColor.FromHtml("#92400E");
                }
                cellStatus.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                wsTickets.Cell(tRowIdx, 5).Value = t.Prioridad ?? "N/A";
                wsTickets.Cell(tRowIdx, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Fechas
                if (t.Alta.HasValue)
                {
                    var cell = wsTickets.Cell(tRowIdx, 6);
                    cell.Value = t.Alta.Value;
                    cell.Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                }
                if (t.Atencion.HasValue)
                {
                    var cell = wsTickets.Cell(tRowIdx, 7);
                    cell.Value = t.Atencion.Value;
                    cell.Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                }
                if (t.Cierre.HasValue)
                {
                    var cell = wsTickets.Cell(tRowIdx, 8);
                    cell.Value = t.Cierre.Value;
                    cell.Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                }

                wsTickets.Cell(tRowIdx, 9).Value = string.IsNullOrWhiteSpace(t.NombreTecnico) ? "Sin Asignar" : t.NombreTecnico;
                wsTickets.Cell(tRowIdx, 10).Value = t.Descripcion ?? "N/A";
                wsTickets.Cell(tRowIdx, 11).Value = t.Solucion ?? "N/A";

                // Alineaciones y bordes
                wsTickets.Range(tRowIdx, 1, tRowIdx, 11).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetBottomBorderColor(XLColor.FromHtml("#E5E7EB"));
                tRowIdx++;
            }

            wsTickets.Range(1, 1, tRowIdx - 1, 11).SetAutoFilter();
            wsTickets.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        // Generar archivo PDF con QuestPDF
        public byte[] GenerarPdf(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Segoe UI").FontColor(Colors.Grey.Darken3));

                    // CABECERA
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("HSis - MÓDULO DE TICKETS").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Reporte Ejecutivo de Incidencias").FontSize(10).Italic();
                        });

                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().AlignRight().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                            col.Item().AlignRight().Text($"Rango: {inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    // CONTENIDO PRINCIPAL
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(15);

                        // 1. CARDS DE INDICADORES (KPIs)
                        col.Item().Row(row =>
                        {
                            row.Spacing(10);

                            // Total Creados
                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                            {
                                c.Item().AlignCenter().Text("TICKETS CREADOS").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                                c.Item().AlignCenter().Text(kpis.TotalCreados.ToString()).FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                            });

                            // Total Resueltos
                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                            {
                                c.Item().AlignCenter().Text("TICKETS RESUELTOS").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                                c.Item().AlignCenter().Text(kpis.TotalResueltos.ToString()).FontSize(16).Bold().FontColor(Colors.Green.Medium);
                            });

                            // Tasa de Cierre
                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                            {
                                c.Item().AlignCenter().Text("TASA DE CIERRE").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                                c.Item().AlignCenter().Text($"{kpis.TasaCierre:F1}%").FontSize(16).Bold().FontColor(Colors.Orange.Medium);
                            });

                            // Tiempo Promedio
                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(8).Column(c =>
                            {
                                c.Item().AlignCenter().Text("TIEMPO PROM. ATN").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                                c.Item().AlignCenter().Text($"{kpis.TiempoPromedioAtencionHoras:F1} Hrs").FontSize(16).Bold().FontColor(Colors.Purple.Medium);
                            });
                        });

                        // Línea divisoria
                        col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                        // 2. TABLA DE PRODUCTIVIDAD TÉCNICA
                        col.Item().Text("Productividad del Personal Técnico").FontSize(11).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Cabecera Tabla
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).Text("Nombre Técnico").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).AlignCenter().Text("Asignados").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).AlignCenter().Text("Resueltos").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).AlignCenter().Text("Tasa de Cierre").Bold().FontColor(Colors.White);
                            });

                            // Contenido Tabla
                            bool alternado = false;
                            foreach (var tech in kpis.ProductividadTecnica)
                            {
                                var bgColor = alternado ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bgColor).Padding(4).Text(tech.Tecnico);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(tech.TicketsAsignados.ToString());
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(tech.TicketsResueltos.ToString());
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text($"{tech.TasaCierre:F1}%");
                                alternado = !alternado;
                            }
                        });

                        // 3. TABLA DE MAYOR DEMANDA POR DEPARTAMENTO
                        col.Item().PaddingTop(10).Text("Demanda y Distribución por Departamento").FontSize(11).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten1).Padding(5).Text("Departamento").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten1).Padding(5).AlignCenter().Text("Tickets").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten1).Padding(5).AlignCenter().Text("Prioridad Alta").Bold().FontColor(Colors.White);
                                header.Cell().Background(Colors.Blue.Lighten1).Padding(5).AlignCenter().Text("% Del Total").Bold().FontColor(Colors.White);
                            });

                            bool alternado = false;
                            foreach (var dpto in kpis.DemandaDepartamentos)
                            {
                                var bgColor = alternado ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bgColor).Padding(4).Text(dpto.Departamento);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(dpto.TotalTickets.ToString());
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(dpto.AltaPrioridad.ToString());
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text($"{dpto.PorcentajeDelTotal:F1}%");
                                alternado = !alternado;
                            }
                        });

                        // 4. TABLA DE MAYORES DEMANDANTES (TOP USUARIOS)
                        col.Item().PaddingTop(10).Text("Mayores Demandantes (Top 10 Usuarios)").FontSize(11).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Usuario").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("Creados").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("Resueltos").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text("Pendientes").Bold();
                            });

                            bool alternado = false;
                            foreach (var usr in kpis.DemandaUsuarios)
                            {
                                var bgColor = alternado ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bgColor).Padding(4).Text(usr.Usuario);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(usr.TicketsCreados.ToString());
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(usr.TicketsResueltos.ToString());
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(usr.TicketsPendientes.ToString());
                                alternado = !alternado;
                            }
                        });
                    });

                    // PIE DE PÁGINA
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text("HSis - Sistema Integrado de Gestión Operativa").FontSize(8).FontColor(Colors.Grey.Medium);
                        row.ConstantItem(50).AlignRight().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}

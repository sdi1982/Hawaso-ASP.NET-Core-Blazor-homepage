﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using VisualAcademy.Models.Replys;

namespace Hawaso.Apis
{
    public class ReplyDownloadController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IReplyRepository _repository;
        private readonly IFileStorageManager _fileStorageManager;

        public ReplyDownloadController(
            IWebHostEnvironment environment,
            IReplyRepository repository,
            IFileStorageManager fileStorageManager)
        {
            _environment = environment;
            _repository = repository;
            _fileStorageManager = fileStorageManager;
        }

        /// <summary>
        /// 게시판 파일 강제 다운로드 기능(/BoardDown/:Id)
        /// </summary>
        public async Task<IActionResult> FileDown(int id)
        {
            var model = await _repository.GetByIdAsync(id);

            if (model == null)
            {
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FileName))
                {
                    byte[] fileBytes = await _fileStorageManager.DownloadAsync(model.FileName, "");
                    if (fileBytes != null)
                    {
                        // DownCount
                        model.DownCount = model.DownCount + 1;
                        await _repository.EditAsync(model);

                        return File(fileBytes, "application/octet-stream", model.FileName);
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }

                return Redirect("/");
            }
        }

        /// <summary>
        /// 엑셀 파일 강제 다운로드 기능(/ExcelDown)
        /// </summary>
        public async Task<IActionResult> ExcelDown()
        {
            var results = await _repository.GetAllAsync(0, 100);

            var models = results.Records.ToList();

            if (models != null)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Replys");

                    var tableBody = worksheet.Cells["B2:B2"].LoadFromCollection(
                        from m in models select new { m.Created, m.Name, m.Title, m.DownCount, m.FileName }
                        , true);

                    var uploadCol = tableBody.Offset(1, 1, models.Count, 1);

                    // 그라데이션 효과 부여 
                    var rule = uploadCol.ConditionalFormatting.AddThreeColorScale();
                    rule.LowValue.Color = Color.SkyBlue;
                    rule.MiddleValue.Color = Color.White;
                    rule.HighValue.Color = Color.Red;

                    var header = worksheet.Cells["B2:F2"];
                    worksheet.DefaultColWidth = 25;
                    worksheet.Cells[3, 2, models.Count + 2, 2].Style.Numberformat.Format = "yyyy MMM d DDD";
                    tableBody.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    tableBody.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tableBody.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
                    tableBody.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    header.Style.Font.Bold = true;
                    header.Style.Font.Color.SetColor(Color.White);
                    header.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);

                    return File(package.GetAsByteArray(), "application/octet-stream", $"{DateTime.Now.ToString("yyyyMMddhhmmss")}_Replys.xlsx");
                }

            }
            return Redirect("/");
        }
    }
}

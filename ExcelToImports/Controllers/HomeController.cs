using ExcelToImports.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToImports.Controllers
{
    public class HomeController : Controller
    {

        /*
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        */
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        
        public IActionResult Index()
        {
            FileUploadViewModel fileUplodedVM = new FileUploadViewModel();
            return View(fileUplodedVM);
        }
        
        [HttpPost]
        [Obsolete]
        public ActionResult Index(FileUploadViewModel fileUplodedVM)
        {
            //FileUploadViewModel fileUplodedErroVM = new FileUploadViewModel();
            if (fileUplodedVM.ExcelFile != null)
            {
                /* save file on server */
                string rootFolder = _hostingEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + fileUplodedVM.ExcelFile.FileName;
                FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));

                using (var stream = new MemoryStream())
                {
                    fileUplodedVM.ExcelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        package.SaveAs(file);
                    }
                }

                /* reading and validating file */
                using (ExcelPackage package = new ExcelPackage(file))
                {                    
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null && worksheet.Dimension != null)
                    {
                        var rowCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {                            
                            Boolean LineError = false;                        
                            for (int column = 1; column <= 4; column++) 
                            {                                
                                var cell = (worksheet.Cells[row, column].Value ?? string.Empty).ToString().Trim();
                                if (cell != "")
                                {                                    
                                    switch (column)
                                    {
                                        case 1://DeliveryDate                                            
                                            Boolean IsDate = DateTime.TryParse(worksheet.Cells[row, 1].Value.ToString().Trim(), out DateTime date);
                                            if (IsDate)
                                            {
                                                var DeliverDate = DateTime.Parse((worksheet.Cells[row, 1].Value).ToString().Trim());
                                                if (DeliverDate <= DateTime.Today)
                                                {
                                                    LineError = true;
                                                    fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                    {
                                                        Line = row,
                                                        Field = "Data de Entrega",
                                                        InfoError = "Não pode ser menor ou igual que o dia atual",
                                                    });

                                                }
                                                break;
                                            }
                                            else
                                            {
                                                LineError = true;
                                                fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                {
                                                    Line = row,
                                                    Field = "Data de Entrega",
                                                    InfoError = "Valor deve ser uma data valida ex.: DD/MM/AAAA",
                                                });
                                                break;
                                            }
                                            
                                        case 2://ProductName
                                            string ProductName = worksheet.Cells[row, 2].Value.ToString().Trim();
                                            if (ProductName.Length > 50)
                                            {
                                                LineError = true;
                                                fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                {
                                                    Line = row,
                                                    Field = "Nome do Produto",
                                                    InfoError = "precisa ter o tamanho máximo de 50 caracteres",
                                                });
                                            }
                                            break;
                                        case 3://Amount                                            
                                            Boolean IsNumber = int.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out int number);
                                            if (IsNumber)
                                            {
                                                int Amount = int.Parse(worksheet.Cells[row, 3].Value.ToString().Trim());
                                                if (Amount <= 0)
                                                {
                                                    LineError = true;
                                                    fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                    {
                                                        Line = row,
                                                        Field = "Quantidade",
                                                        InfoError = "tem que ser maior do que zero",
                                                    });
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                LineError = true;
                                                fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                {
                                                    Line = row,
                                                    Field = "Quantidade",
                                                    InfoError = "Valor deve ser um numero valido",
                                                });
                                                break;
                                            }
                                        case 4://UnitaryValue                                            
                                            Boolean IsNumberFloat = double.TryParse(worksheet.Cells[row, 4].Value.ToString().Trim(),out double numberFloat);
                                            if (IsNumberFloat)
                                            {
                                                double UnitaryValue = double.Parse(worksheet.Cells[row, 4].Value.ToString().Trim());
                                                if (UnitaryValue <= 0)
                                                {
                                                    LineError = true;
                                                    fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                    {
                                                        Line = row,
                                                        Field = "Valor Unitário",
                                                        InfoError = "tem que ser maior do que zero",
                                                    });
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                LineError = true;
                                                fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                                {
                                                    Line = row,
                                                    Field = "Valor Unitário",
                                                    InfoError = "Valor deve ser um numero valido",
                                                });
                                                break;
                                            }
                                        default:                                                                                        
                                            break;
                                    }
                                }
                                else
                                {
                                    string field = string.Empty;
                                    switch (column)
                                    {
                                        case 1: field = "Data de Entrega"; break;
                                        case 2: field = "Nome do Produto"; break;
                                        case 3: field = "Quantidade"; break;
                                        case 4: field = "Valor Unitário"; break;
                                        default:
                                            break;
                                    }
                                        LineError = true;
                                        fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                                        {
                                            Line = row,
                                            Field = field,
                                            InfoError = "Valor invalido, está vazia",
                                        });
                                    
                                }
                            }

                            if (LineError == false)
                            {
                                fileUplodedVM.ExcelInfoViewModel.ItemsList.Add(new ExcelInfoViewModel
                                {
                                    DeliveryDate = DateTime.Parse(worksheet.Cells[row, 1].Value.ToString().Trim()),
                                    ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                    Amount = int.Parse(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                    UnitaryValue = double.Parse(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                });
                            }
                        }
                        
                    }
                    else
                    {
                        fileUplodedVM.ErrorInfoViewModel.ErrorList.Add(new ErrorInfoViewModel
                        {
                            Line = 0,                            
                            Field = "Todos",
                            InfoError = "Documento vazio",
                        });
                    }
                }
            }
            
            return View(fileUplodedVM);
        }
        /*
            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        */
    }
}

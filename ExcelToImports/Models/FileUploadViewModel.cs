using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToImports.Models
{
    public class FileUploadViewModel
    {
        public IFormFile ExcelFile { get; set; }
        /*create InfoViewModel  object because we need to add read
         excel data and mapping in InfoViewModel*/
        public ExcelInfoViewModel ExcelInfoViewModel { get; set; }
        public ErrorInfoViewModel ErrorInfoViewModel { get; set; }
        public FileUploadViewModel()//Create contractor
        {
            //call InfoViewModel  this object in contractor
            ExcelInfoViewModel = new ExcelInfoViewModel();
            ErrorInfoViewModel = new ErrorInfoViewModel();
        }
    }
}

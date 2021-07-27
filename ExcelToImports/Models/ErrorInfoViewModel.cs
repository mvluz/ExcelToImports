using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToImports.Models
{
    public class ErrorInfoViewModel
    {
        public int Line { get; set; }
        public string Field { get; set; }
        public string InfoError { get; set; }
        public List<ErrorInfoViewModel> ErrorList { get; set; }
        public ErrorInfoViewModel()
        {
            ErrorList = new List<ErrorInfoViewModel>();
        }
    }
}

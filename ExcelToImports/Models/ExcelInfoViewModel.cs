using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToImports.Models
{
    public class ExcelInfoViewModel
    {
        public DateTime DeliveryDate { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitaryValue { get; set; }
        public List<ExcelInfoViewModel> ItemsList { get; set; }
        public ExcelInfoViewModel()
        {
            ItemsList = new List<ExcelInfoViewModel>();
        }
    }
}

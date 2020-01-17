using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace DecisionTreeRegression
{
    public class EmployeeSalary
    {
        [LoadColumn(0)]
        public string PositionName { get; set; }
        [LoadColumn(1)]
        public float PositionLevel { get; set; }
        [LoadColumn(2)]
        public float Salary { get; set; }
    }
}

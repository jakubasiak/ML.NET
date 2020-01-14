using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace PolynomialRegression
{
    public class EmployeeSalaryPrediction
    {
        [ColumnName("Score")]
        public float Salary { get; set; }
    }
}

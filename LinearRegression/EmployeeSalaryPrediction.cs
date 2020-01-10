using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace LinearRegression
{
    public class EmployeeSalaryPrediction
    {
        [ColumnName("Score")]
        public float Salary { get; set; }
    }
}

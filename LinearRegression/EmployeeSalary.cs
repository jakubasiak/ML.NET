using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace LinearRegression
{
    public class EmployeeSalary
    {
        [LoadColumn(0)]
        public float YearsExperience { get; set; }
        [LoadColumn(1)]
        public float Salary { get; set; }
    }
}

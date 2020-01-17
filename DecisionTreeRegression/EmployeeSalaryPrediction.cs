using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace DecisionTreeRegression
{
    public class EmployeeSalaryPrediction
    {
        [ColumnName("Score")]
        public float Salary { get; set; }
    }
}

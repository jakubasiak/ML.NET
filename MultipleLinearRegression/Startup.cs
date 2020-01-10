using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace MultipleLinearRegression
{
    public class Startup
    {
        [LoadColumn(0)]
        public float RAndD { get; set; }
        [LoadColumn(1)]
        public float Administration { get; set; }
        [LoadColumn(2)]
        public float Marketing { get; set; }
        [LoadColumn(3)]
        public string State { get; set; }
        [LoadColumn(4)]
        public float Profit { get; set; }

    }
}

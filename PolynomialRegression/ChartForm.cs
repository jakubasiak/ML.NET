﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Brushes = System.Drawing.Brushes;

namespace PolynomialRegression
{
    public partial class ChartForm : Form
    {
        public ChartForm(Axis xAxis, Axis yAxis, SeriesCollection series)
        {
            this.InitializeComponent();
            this.cartesianChart.Series = series;
            this.cartesianChart.AxisX.Add(xAxis);
            this.cartesianChart.AxisY.Add(yAxis);
            cartesianChart.LegendLocation = LegendLocation.Right;
        }

    }
}

using System;
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
            //cartesianChart.Series = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Series 1",
            //        Values = new ChartValues<double> {4, 6, 5, 2, 7}
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 2",
            //        Values = new ChartValues<double> {6, 7, 3, 4, 6},
            //        PointGeometry = null
            //    },
            //    new LineSeries
            //    {
            //        Title = "Series 2",
            //        Values = new ChartValues<double> {5, 2, 8, 3},
            //        PointGeometry = DefaultGeometries.Square,
            //        PointGeometrySize = 15
            //    }
            //};

            //cartesianChart.AxisX.Add(new Axis
            //{
            //    Title = "",
            //    Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
            //});

            //cartesianChart.AxisY.Add(new Axis
            //{
            //    Title = "Sales",
            //    LabelFormatter = value => value.ToString("C")
            //});

            cartesianChart.LegendLocation = LegendLocation.Right;

            //modifying the series collection will animate and update the chart
            //cartesianChart.Series.Add(new LineSeries
            //{
            //    Values = new ChartValues<double> { 5, 3, 2, 4, 5 },
            //    LineSmoothness = 0, //straight lines, 1 really smooth lines
            //    PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
            //    PointGeometrySize = 50,
            //    PointForeground = System.Windows.Media.Brushes.Gray
            //});

            //modifying any series values will also animate and update the chart
            //cartesianChart.Series[2].Values.Add(5d);


            cartesianChart.DataClick += cartesianChartOnDataClick;
        }

        private void cartesianChartOnDataClick(object sender, ChartPoint chartPoint)
        {
            MessageBox.Show("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")");
        }
    }
}

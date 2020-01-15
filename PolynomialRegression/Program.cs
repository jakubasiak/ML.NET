using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace PolynomialRegression
{
    static class Program
    {
        static readonly string dataPath = Path.Combine(Environment.CurrentDirectory, "data", "Position_Salaries.csv");
        static readonly string modelPath = Path.Combine(Environment.CurrentDirectory, "data", "Model.zip");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mlContext = new MLContext();

            IDataView dataView = mlContext.Data.LoadFromTextFile<EmployeeSalary>(dataPath, hasHeader: true, separatorChar: ',');

            Action<EmployeeSalary, EmployeeSalary> squared = (input, output) =>
            {
                output.PositionLevel = (float) Math.Pow(input.PositionLevel, 2.0);
            };
            Action<EmployeeSalary, EmployeeSalary> fourth = (input, output) =>
            {
                output.PositionLevel = (float)Math.Pow(input.PositionLevel, 4.0);
            };

            var dpp1 = mlContext.Transforms.CopyColumns("Squared", nameof(EmployeeSalary.PositionLevel))
                .Append(mlContext.Transforms.CustomMapping(squared, null));

            var dpp2 = mlContext.Transforms.CopyColumns("Third", nameof(EmployeeSalary.PositionLevel))
                .Append(mlContext.Transforms.CustomMapping(fourth, null));

            var dataProcessingPipeline = mlContext.Transforms.CopyColumns("Label", nameof(EmployeeSalary.Salary))
                //.Append(dpp1)
                .Append(dpp2)
                .Append(mlContext.Transforms.Concatenate("Features", nameof(EmployeeSalary.PositionLevel), "Third"));

            var trainer = mlContext.Regression.Trainers.LbfgsPoissonRegression(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessingPipeline.Append(trainer);

            var trainedModel = trainingPipeline.Fit(dataView);

            // Use only during debugging
            var previewTrainSet = DebuggerExtensions.Preview(trainedModel.Transform(dataView), 100);

            var predEngine = mlContext.Model.CreatePredictionEngine<EmployeeSalary, EmployeeSalaryPrediction>(trainedModel);



            // Model data
            var positionLevel = dataView.GetColumn<float>("PositionLevel").ToArray();
            var salary = dataView.GetColumn<float>("Salary").ToArray();
            var modelObservablePoints = new List<ObservablePoint>();
            for (int i = 0; i < salary.Length; i++)
            {
                modelObservablePoints.Add(new ObservablePoint(positionLevel[i], salary[i]));
            }

            // Predictions data
            var predictionsObservablePoints = new List<ObservablePoint>();
            for (float i = 0f; i <= 10; i = i + 0.5f)
            {
                var prdiction = predEngine.Predict(new EmployeeSalary()
                {
                    PositionLevel = i
                });

                predictionsObservablePoints.Add(new ObservablePoint(i, prdiction.Salary));
            }

            var series = new SeriesCollection()
            {
                new ScatterSeries()
                {
                    Values = new ChartValues<ObservablePoint>(modelObservablePoints),
                    Title = "Training set"
                },
                new LineSeries()
                {
                    Title = "Predictions",
                    Values = new ChartValues<ObservablePoint>(predictionsObservablePoints),
                }

            };

            var xAxis = new Axis()
            {
                Title = "Level",
                Separator = new LiveCharts.Wpf.Separator()
                {
                    Step = 1.0,
                    IsEnabled = false
                }

            };

            var yAxis = new Axis()
            {
                Title = "Salary",
                LabelFormatter = value => value.ToString("C")
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChartForm(xAxis, yAxis, series));
        }
    }
}

using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace MultipleLinearRegression
{
    class Program
    {
        static readonly string dataPath = Path.Combine(Environment.CurrentDirectory, "data", "50_Startups.csv");
        static readonly string modelPath = Path.Combine(Environment.CurrentDirectory, "data", "Model.zip");

        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();

            IDataView dataView = mlContext.Data.LoadFromTextFile<Startup>(dataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(dataView, 0.2, seed: 0);

            var trainSetCollection = mlContext.Data.CreateEnumerable<Startup>(split.TrainSet, false);
            IDataView trainSet = mlContext.Data.LoadFromEnumerable(trainSetCollection);

            var testSetCollection = mlContext.Data.CreateEnumerable<Startup>(split.TestSet, false);
            IDataView testSet = mlContext.Data.LoadFromEnumerable(testSetCollection);

            // Use only during debugging
            //var previewTrainSet = DebuggerExtensions.Preview(trainSet, 100);
            //var previewTestSet = DebuggerExtensions.Preview(testSet, 100);

            var dataProcessingPipeline = mlContext.Transforms.CopyColumns("Label", nameof(Startup.Profit))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "StateEncoded",
                    inputColumnName: nameof(Startup.State)))
                .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(Startup.RAndD)))
                .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(Startup.Administration)))
                .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(Startup.Marketing)))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(Startup.RAndD),
                    nameof(Startup.Administration), nameof(Startup.Marketing), "StateEncoded"));

            var trainer = mlContext.Regression.Trainers.LbfgsPoissonRegression(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessingPipeline.Append(trainer);

            Console.WriteLine("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainSet);

            // STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");
            IDataView predictions = trainedModel.Transform(testSet);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");
            PrintRegressionMetrics(trainer.ToString(), metrics);
        }

        public static void PrintRegressionMetrics(string name, RegressionMetrics metrics)
        {
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Metrics for {name} regression model      ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       LossFn:        {metrics.LossFunction:0.##}");
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}");
            Console.WriteLine($"*       Squared loss:  {metrics.MeanSquaredError:#.##}");
            Console.WriteLine($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine($"*************************************************");
        }

    }
}

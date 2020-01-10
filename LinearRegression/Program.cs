using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace LinearRegression
{
    class Program
    {
        static readonly string trainDataPath = Path.Combine(Environment.CurrentDirectory, "data", "Salary_Data_Train.csv");
        static readonly string testDataPath = Path.Combine(Environment.CurrentDirectory, "data", "Salary_Data_Test.csv");
        static readonly string modelPath = Path.Combine(Environment.CurrentDirectory, "data", "Model.zip");

        static void Main(string[] args)
        {

            MLContext mlContext = new MLContext();

            // STEP 1: Common data loading configuration
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<EmployeeSalary>(trainDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<EmployeeSalary>(testDataPath, hasHeader: true, separatorChar: ',');

            // STEP 2: Common data process configuration with pipeline data transformations
            var dataProcessPipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(EmployeeSalary.Salary))
                .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(EmployeeSalary.YearsExperience)))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(EmployeeSalary.YearsExperience)));

            // STEP 3: Set the training algorithm, then create and config the modelBuilder - Selected Trainer (SDCA Regression algorithm)                            
            var trainer = mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // STEP 4: Train the model fitting to the DataSet
            //The pipeline is trained on the dataset that has been loaded and transformed.
            Console.WriteLine("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            // STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");

            IDataView predictions = trainedModel.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

            PrintRegressionMetrics(trainer.ToString(), metrics);

            // STEP 6: Save/persist the trained model to a .ZIP file
            mlContext.Model.Save(trainedModel, trainingDataView.Schema, modelPath);

            Console.WriteLine("The model is saved to {0}", modelPath);

            // TEST
            //TestSinglePrediction(mlContext, 4.5f);
            TestMultiplePredictions(mlContext);
        }

        private static void TestSinglePrediction(MLContext mlContext, float yearsExperience)
        {

            var taxiTripSample = new EmployeeSalary()
            {
                YearsExperience = yearsExperience,
                Salary = 0
            };

            ITransformer trainedModel = mlContext.Model.Load(modelPath, out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<EmployeeSalary, EmployeeSalaryPrediction>(trainedModel);

            //Score
            var resultprediction = predEngine.Predict(taxiTripSample);
            ///

            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted salary: {resultprediction.Salary:0.####}");
            Console.WriteLine($"**********************************************************************");
        }

        private static void TestMultiplePredictions(MLContext mlContext)
        {
            ITransformer trainedModel = mlContext.Model.Load(modelPath, out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<EmployeeSalary, EmployeeSalaryPrediction>(trainedModel);

            for (float i = 0; i < 15; i = i + 0.5f)
            {
                var taxiTripSample = new EmployeeSalary()
                {
                    YearsExperience = i,
                    Salary = 0
                };

                var resultprediction = predEngine.Predict(taxiTripSample);
                Console.WriteLine(resultprediction.Salary);
            }
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

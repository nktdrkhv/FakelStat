// using ScottPlot;

// ScottPlot.Plot myPlot = new();

// // create an array of DateTimes one hour apart
// int numberOfHours = 24;
// DateTime[] dateTimes = new DateTime[numberOfHours];
// DateTime startDateTime = new(2024, 1, 1);
// TimeSpan deltaTimeSpan = TimeSpan.FromHours(1);
// for (int i = 0; i < numberOfHours; i++)
// {
//     dateTimes[i] = startDateTime + i * deltaTimeSpan;
// }

// // create an array of doubles representing the same DateTimes one hour apart
// double[] dateDoubles = new double[numberOfHours];
// double startDouble = startDateTime.ToOADate(); // days since 1900
// double deltaDouble = 1.0 / 24.0; // an hour is 1/24 of a day
// for (int i = 0; i < numberOfHours; i++)
// {
//     dateDoubles[i] = startDouble + i * deltaDouble;
// }

// // now both arrays represent the same dates
// myPlot.Add.Scatter(dateTimes, Generate.Sin(numberOfHours));
// myPlot.Add.Scatter(dateDoubles, Generate.Cos(numberOfHours));
// myPlot.Axes.DateTimeTicksBottom();

// myPlot.SavePng("demo.png", 1200, 300);

// -------------------------------------

// var f1 = Task.Run(async () => await Foo("f1", 3000));


// var s1 = f1.ContinueWith(async (_) => await Foo("s1", 2000), TaskContinuationOptions.OnlyOnRanToCompletion);
// var q1 = s1.ContinueWith(async (_) => await Foo("q1", 3000), TaskContinuationOptions.OnlyOnRanToCompletion);

// async Task Foo(string s, int delay)
// {
//     Console.WriteLine($"{s} start");
//     await Task.Delay(delay);
//     Console.WriteLine($"{s} con");
//     await Task.Delay(delay);
//     Console.WriteLine($"{s} end");
// }

// Console.ReadKey();

Console.WriteLine("hello world");

using System;
using System.IO;
using System.Diagnostics;

class TestRunner
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: tester.exe <test_cases_file>");
            return;
        }

        string testCasesFile = args[0];
        string resultsFile = "test_results.txt";

        try
        {
            string[] testCases = File.ReadAllLines(testCasesFile);
            using (StreamWriter resultsWriter = new StreamWriter(resultsFile))
            {
                int totalTests = 0;
                int passedTests = 0;

                foreach (string testCase in testCases)
                {
                    if (string.IsNullOrWhiteSpace(testCase))
                        continue;

                    string[] parts = testCase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 4)
                    {
                        resultsWriter.WriteLine($"Invalid test case: {testCase}");
                        continue;
                    }

                    string sideA = parts[0];
                    string sideB = parts[1];
                    string sideC = parts[2];
                    string expectedResult = string.Join(" ", parts, 3, parts.Length - 3).ToLower();

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "triangle.exe",
                        Arguments = $"{sideA} {sideB} {sideC}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    string actualResult = "";
                    try
                    {
                        using (Process process = Process.Start(startInfo))
                        {
                            actualResult = process.StandardOutput.ReadToEnd().Trim().ToLower();
                            process.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        actualResult = "неизвестная ошибка";
                    }

                    bool isPassed = actualResult == expectedResult;
                    if (isPassed) passedTests++;
                    totalTests++;

                    resultsWriter.WriteLine($"Test case: {sideA} {sideB} {sideC}");
                    resultsWriter.WriteLine($"Expected: {expectedResult}");
                    resultsWriter.WriteLine($"Actual: {actualResult}");
                    resultsWriter.WriteLine($"Result: {(isPassed ? "PASSED" : "FAILED")}");
                    resultsWriter.WriteLine();
                }

                resultsWriter.WriteLine($"SUMMARY: {passedTests}/{totalTests} tests passed");
                resultsWriter.WriteLine($"Success rate: {((double)passedTests / totalTests) * 100:0.00}%");
            }

            Console.WriteLine($"Testing completed. Results saved to {resultsFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
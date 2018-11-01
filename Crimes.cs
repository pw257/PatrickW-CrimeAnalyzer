using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimeAnalyzer
{
    class Crimes
    {
        private static List<CrimeData> CrimeDataList = new List<CrimeData>();

        public Crimes(string[] args)
        {
            string csvFile = string.Empty;
            string reportFile = string.Empty;
            string startPath = Directory.GetCurrentDirectory();

            if (Debugger.IsAttached)
            {
                csvFile = Path.Combine(startPath, "CrimeData.csv");

                reportFile = Path.Combine(startPath, "CrimeReport.txt");
            }
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid call.\n Valid call example : CrimeAnalyzer <crime_csv_file_path> <report_file_path>");
                    Console.ReadLine();

                    return;
                }
                else
                {
                    csvFile = args[0];

                    reportFile = args[1];

                    if (!csvFile.Contains("\\"))
                    {
                        csvFile = Path.Combine(startPath, csvFile);
                    }
                    if (!reportFile.Contains("\\"))
                    {
                        reportFile = Path.Combine(startPath, reportFile);
                    }
                }
            }
            if (File.Exists(csvFile))
            {
                if (ReadData(csvFile))
                {
                    try
                    {
                        var file = File.Create(reportFile);

                        file.Close();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Unable to create report file at : {reportFile}");
                    }
                    WriteReport(reportFile);
                }
            }
            else
            {
                Console.Write($"Crime data file does not exist at path: {csvFile}");
            }

            Console.ReadLine();
        }

        private static bool ReadData(string filePath)
        {
            try
            {
                int column = 0;
                string[] crimeDataLines = File.ReadAllLines(filePath);

                for (int index = 0; index < crimeDataLines.Length; index++)
                {
                    string crimeDataLine = crimeDataLines[index];
                    string[] data = crimeDataLine.Split(',');

                    if (index == 0)
                    {
                        column = data.Length;
                    }
                    else
                    {
                        if (column != data.Length)
                        {
                            Console.WriteLine($"Row {index} contains {data.Length} values. It should contain {column}.");
                            return false;
                        }
                        else
                        {
                            try
                            {
                                CrimeData crimeData = new CrimeData();

                                crimeData.Year = Convert.ToInt32(data[0]);

                                crimeData.Population = Convert.ToInt32(data[1]);

                                crimeData.ViolentCrimes = Convert.ToInt32(data[2]);

                                crimeData.Murder = Convert.ToInt32(data[3]);

                                crimeData.Rape = Convert.ToInt32(data[4]);

                                crimeData.Robbery = Convert.ToInt32(data[5]);

                                crimeData.Theft = Convert.ToInt32(data[9]);

                                crimeData.MotorVehicleTheft = Convert.ToInt32(data[10]);

                                CrimeDataList.Add(crimeData);
                            }

                            catch (InvalidCastException)
                            {
                                Console.WriteLine($"Row {index} contains invalid value.");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in reading data from csv file.");
                throw ex;
            }
        }

        private static void WriteReport(string filePath)
        {
            try
            {
                if (CrimeDataList != null && CrimeDataList.Any())
                {
                    StringBuilder newString = new StringBuilder();

                    newString.Append("Crime Analyzer Report");
                    newString.Append(Environment.NewLine);
                    newString.Append(Environment.NewLine);

                    // 1 & 2
                    int minYear = CrimeDataList.Min(x => x.Year);
                    int maxYear = CrimeDataList.Max(x => x.Year);

                    int years = maxYear - minYear + 1;

                    newString.Append($"Period: {minYear}-{maxYear} ({years} years)");
                    newString.Append(Environment.NewLine);
                    newString.Append(Environment.NewLine);

                    // 3
                    var totalYears = from crimeData in CrimeDataList
                                     where crimeData.Murder < 15000
                                     select crimeData.Year;

                    string totalYearsStr = string.Empty;

                    for (int i = 0; i < totalYears.Count(); i++)
                    {
                        totalYearsStr += totalYears.ElementAt(i).ToString();

                        if (i < totalYears.Count() - 1) totalYearsStr += ", ";
                    }

                    newString.Append($"Years murders per year < 15000: {totalYearsStr}");
                    newString.Append(Environment.NewLine);

                    // 4
                    var robYears = from crimeData in CrimeDataList
                                   where crimeData.Robbery > 500000
                                   select crimeData;

                    string robYearsStr = string.Empty;

                    for (int i = 0; i < robYears.Count(); i++)
                    {
                        CrimeData crimeData = robYears.ElementAt(i);

                        robYearsStr += $"{crimeData.Year} = {crimeData.Robbery}";

                        if (i < robYears.Count() - 1) robYearsStr += ", ";
                    }

                    newString.Append($"Robberies per year > 500000: {robYearsStr}");
                    newString.Append(Environment.NewLine);

                    // 5
                    var violentCrime = from crimeData in CrimeDataList
                                       where crimeData.Year == 2010
                                       select crimeData;

                    CrimeData violentCrimeData = violentCrime.First();

                    double violentCrimePerCapita = (double)violentCrimeData.ViolentCrimes / (double)violentCrimeData.Population;

                    newString.Append($"Violent crime per capita rate (2010): {violentCrimePerCapita}");
                    newString.Append(Environment.NewLine);

                    // 6
                    double avgMurders = (float)CrimeDataList.Sum(x => x.Murder) / (float)CrimeDataList.Count;

                    newString.Append($"Average murder per year (all years): {avgMurders}");
                    newString.Append(Environment.NewLine);

                    // 7
                    int murder = CrimeDataList.Where(x => x.Year >= 1994 && x.Year <= 1997).Sum(y => y.Murder);

                    double avgMurder = (float)murder / 4;

                    newString.Append($"Average murder per year (1994-1997): {avgMurder}");
                    newString.Append(Environment.NewLine);

                    // 8
                    int murd = CrimeDataList.Where(x => x.Year >= 2010 && x.Year <= 2014).Sum(y => y.Murder);

                    double avgMurd = (float)murd / 5;

                    newString.Append($"Average murder per year (2010-2014): {avgMurd}");
                    newString.Append(Environment.NewLine);

                    // 9
                    int minTheft = CrimeDataList.Where(x => x.Year >= 1999 && x.Year <= 2004).Min(x => x.Theft);

                    newString.Append($"Minimum thefts per year (1999-2004): {minTheft}");
                    newString.Append(Environment.NewLine);

                    // 10
                    int maxTheft = CrimeDataList.Where(x => x.Year >= 1999 && x.Year <= 2004).Max(x => x.Theft);

                    newString.Append($"Maximum thefts per year (1999-2004): {maxTheft}");
                    newString.Append(Environment.NewLine);

                    // 11
                    int maxVehicleTheft = CrimeDataList.OrderByDescending(x => x.MotorVehicleTheft).First().Year;

                    newString.Append($"Year of highest number of motor vehicle thefts: {maxVehicleTheft}");
                    newString.Append(Environment.NewLine);

                    using (var stream = new StreamWriter(filePath))
                    {
                        stream.Write(newString.ToString());
                    }

                    Console.WriteLine();
                    Console.WriteLine(newString.ToString());
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"No data to write.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in writing report file.");
                throw ex;
            }
        }
    }
}

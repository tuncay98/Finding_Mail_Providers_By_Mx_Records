using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MxRecords.Models
{
    public class CsvOperations
    {
        public static List<string> ReadCsv(string path)
        {
            List<string> mails = new List<string>();
            sbyte index = 0;

            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,50})+)$");
             

            string[] fileStream = System.IO.File.ReadAllLines(path);

            //int indexOfProvider = 0;



            /*foreach (var line in fileStream)
            {
                string[] columns = line.Split(',');

                for (int i = 0; i < columns.Length; i++)
                {
                    if (regex.IsMatch(columns[i]))
                    {
                        indexOfProvider = i;
                        break;
                    }
                }
            }

            foreach (string line in fileStream)
            {
                if (index == 0)
                {
                    index = 1;
                    continue;
                }
                List<string> splitLine = line.Split(',').ToList();

                mails.Add(splitLine[indexOfProvider]);
            }*/

            foreach (string line in fileStream)
            {
                string newLine = line.Replace(", ", "~~<");
                if (index == 0)
                {
                    index = 1;
                    continue;
                }
                List<string> splitLine = newLine.Split(',').ToList();

                foreach (var item in splitLine)
                {
                    if (regex.IsMatch(item))
                    {
                        mails.Add(item);
                        break;
                    }
                }
            }

            return mails;
        }

        public static void WriteCsv(string path, List<string> mails)
        {
            string[] fileStreamArray = System.IO.File.ReadAllLines(path);
            List<string> fileStream = fileStreamArray.ToList();
            int indexOfFile = 0;

            if (!fileStream[0].Contains("Mail Providers"))
            {
                fileStream[0] += ",Mail Providers";

                foreach (string mail in mails)
                {
                    indexOfFile++;
                    fileStream[indexOfFile] += "," + mail;
                }
            }
            else
            {
                string[] columns = fileStream[0].Split(',');
                int indexOfProvider = 0;

                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].Contains("Mail Providers"))
                    {
                        indexOfProvider = i;
                        break;
                    }
                }

                foreach (string mail in mails)
                {
                    indexOfFile++;

                    List<string> lineString = fileStream[indexOfFile].Split(',').ToList();

                    lineString[indexOfProvider] = mail;

                    fileStream[indexOfFile] = "";

                    foreach (string words in lineString)
                    {
                        if (lineString[lineString.Count - 1] != words)
                            fileStream[indexOfFile] += words + ",";
                        else
                            fileStream[indexOfFile] += words;
                    }
                }
            }
            System.IO.File.WriteAllLines(path, fileStream);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NZOR.Matching.Batch.Helpers
{
    class InputDataParser
    {
        public string IDColumnName { get; set; }
        public string FullNameColumnName { get; set; }

        public InputDataParser()
        {
            IDColumnName = "ID";
            FullNameColumnName = "ScientificName";
        }

        /// <summary>
        /// Parses the input data in csv format to return a list of submitted names.
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public List<SubmittedName> ParseInputData(string inputData)
        {
            List<SubmittedName> submittedNames = new List<SubmittedName>();

            using (StringReader reader = new StringReader(inputData))
            {
                using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                    parser.SetDelimiters(",");
                    
                    string[] currentRecord;

                    if (!parser.EndOfData)
                    {
                        // Read the first line to establish column headings.
                        currentRecord = parser.ReadFields();

                        int idIndex = Array.FindIndex(currentRecord, o => o.Equals(IDColumnName, StringComparison.OrdinalIgnoreCase));
                        int scientificNameIndex = Array.FindIndex(currentRecord, o => o.Equals(FullNameColumnName, StringComparison.OrdinalIgnoreCase));

                        if (idIndex == -1 || scientificNameIndex == -1)
                        {
                            throw new Exception("The input data does not contain the headers specified in the template");
                        }

                        if (!parser.EndOfData)
                        {
                            do
                            {
                                try
                                {
                                    currentRecord = parser.ReadFields();
                                    SubmittedName submittedName = new SubmittedName();

                                    submittedName.Id = currentRecord[idIndex];
                                    submittedName.ScientificName = currentRecord[scientificNameIndex];

                                    submittedNames.Add(submittedName);
                                }
                                catch (Exception)
                                {
                                    throw new Exception("The number of values does not match the number of headers (" + String.Join(", ", currentRecord) + ")");
                                }
                            } while (!parser.EndOfData);
                        }
                    }
                }
            }

            return submittedNames;
        }
    }
}

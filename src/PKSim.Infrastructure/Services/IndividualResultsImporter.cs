using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using LumenWorks.Framework.IO.Csv;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class IndividualResultsImporter : IIndividualResultsImporter
   {
      private const int INDIVIDUAL_ID = 0;
      private const int TIME = 1;
      private const int FIRST_QUANTITY = TIME + 1;
      private readonly Regex _regex;

      public IndividualResultsImporter()
      {
         _regex = new Regex(createRegexPattern());
      }


      public IEnumerable<IndividualResults> ImportFrom(string fileFullPath, Simulation simulation,IImportLogger logger)
      {
         //do not define this variables as member variable to be thread safe
         var cacheQuantitiesValues = new Cache<QuantityValues, List<float>>();
         var cacheTimeValues = new Cache<IndividualResults, List<float>>();

         try
         {
            var individualResults = new Cache<int, IndividualResults>(x => x.IndividualId);
            using (var reader = new CsvReaderDisposer(fileFullPath))
            {
               var csv = reader.Csv;
               var headers = csv.GetFieldHeaders();
               validateHeaders(headers);

               //skip the first two indexes that are individual id and time
               var allQuantityPaths = retrieveQuantityPathsFromHeader(headers, simulation);

               while (csv.ReadNextRecord())
               {
                  int individualId = retrieveParameterId(csv);
                  if (!individualResults.Contains(individualId))
                     individualResults.Add(createIndividualResults(individualId, cacheTimeValues));

                  addRecordToIndividualResults(individualResults[individualId], allQuantityPaths, csv, cacheTimeValues, cacheQuantitiesValues);
               }
            }
            updateResults(cacheTimeValues, cacheQuantitiesValues);
            return individualResults;
         }
         catch (Exception e)
         {
            logger.AddError(e.FullMessage());
            return Enumerable.Empty<IndividualResults>();
         }
         finally
         {
            cacheQuantitiesValues.Clear();
            cacheTimeValues.Clear();
         }
      }

      private static string[] retrieveQuantityPathsFromHeader(IEnumerable<string> headers, Simulation simulation)
      {
         var allPaths =  headers.Skip(FIRST_QUANTITY).ToArray();
         for (int i = 0; i < allPaths.Length; i++)
         {
            var objectPath = new ObjectPath(allPaths[i].ToPathArray());
            if(string.Equals(objectPath.FirstOrDefault(),simulation.Name))
               objectPath.Remove(simulation.Name);

            allPaths[i] = objectPath;
         }
         return allPaths;
      }

      private string createRegexPattern()
      {
         return "^" +                     //beginning of phrase
                "(?<header>.+)" +         //a series of at least one letters and numbers only that we call header
                "\\s*\\[.*\\]\\s*" +      // [..] with padding before and after (not mandatory)
                "$";                      //end of phrase      
      }

      private void validateHeaders(string[] headers)
      {
         var exception = new PKSimException(PKSimConstants.Error.SimulationResultsFileDoesNotHaveTheExpectedFormat);
         if (headers.Length <= FIRST_QUANTITY)
            throw exception;

         //check if headers are actually real strings
         double result;
         if (double.TryParse(headers[0], out result))
            throw exception;

         for (int i = 0; i < headers.Length; i++)
         {
            if (_regex.IsMatch(headers[i]))
               headers[i] =  _regex.Match(headers[i]).Groups["header"].Value.Trim();
         }
      }

      private void updateResults(Cache<IndividualResults, List<float>> cacheTimeValues, Cache<QuantityValues, List<float>> cacheQuantitiesValues)
      {
         //Set the time in each individual results
         cacheTimeValues.Keys.Each(indRes => { indRes.Time.Values = cacheTimeValues[indRes].ToArray(); });

         //set the results in each individual results
         cacheQuantitiesValues.Keys.Each(quantityValue => { quantityValue.Values = cacheQuantitiesValues[quantityValue].ToArray(); });
      }

      private static int retrieveParameterId(CsvReader csv)
      {
         return csv.IntAt(INDIVIDUAL_ID);
      }

      private void addRecordToIndividualResults(IndividualResults individualResult, string[] allPaths, CsvReader csv, Cache<IndividualResults, List<float>> cacheTimeValues, Cache<QuantityValues, List<float>> cacheQuantitiesValues)
      {
         var time = csv.FloatAt(TIME);
         for (int i = 0; i < allPaths.Length; i++)
         {
            var path = allPaths[i];
            var quantity = quantityPathFor(individualResult, path, cacheQuantitiesValues);
            cacheQuantitiesValues[quantity].Add(csv.FloatAt(FIRST_QUANTITY + i));
         }
         cacheTimeValues[individualResult].Add(time);
      }

      private QuantityValues quantityPathFor(IndividualResults individualResult, string path, Cache<QuantityValues, List<float>> cacheQuantitiesValues)
      {
         var quantity = individualResult.ValuesFor(path);
         if (quantity == null)
         {
            quantity = new QuantityValues {QuantityPath = path};
            cacheQuantitiesValues.Add(quantity, new List<float>());
            individualResult.Add(quantity);
         }
         return quantity;
      }

      private IndividualResults createIndividualResults(int individualId, Cache<IndividualResults, List<float>> cacheTimeValues)
      {
         var individualResults = new IndividualResults
         {
            IndividualId = individualId,
            Time = new QuantityValues(),
         };

         cacheTimeValues.Add(individualResults, new List<float>());
         return individualResults;
      }
   }
}
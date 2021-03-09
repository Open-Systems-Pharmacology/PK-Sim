using System.Collections.Generic;
using System.Data;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class AgingData
   {
      private readonly ICache<string, ParameterAgingData> _data;

      public AgingData()
      {
         _data = new Cache<string, ParameterAgingData>(x => x.ParameterPath);
      }

      public virtual IReadOnlyCollection<ParameterAgingData> AllParameterData => _data;

      /// <summary>
      ///    Returns or set a <seealso cref="DataTable " /> containing 4 columns. First one is Individual index, second one is parameter ParameterPath, third one is X value (time) in base unit
      ///    and fourth one is value in base unit
      /// </summary>
      public virtual DataTable ToDataTable()
      {
         var dataTable = new DataTable(CoreConstants.Population.AGING_DATA_TABLE_NAME);

         //Create one column for the parameter path
         dataTable.AddColumn<int>(Constants.Population.INDIVIDUAL_ID_COLUMN);
         dataTable.AddColumn(Constants.Population.PARAMETER_PATH_COLUMN);
         dataTable.AddColumn(Constants.Population.TIME_COLUMN);
         dataTable.AddColumn(Constants.Population.VALUE_COLUMN);

         foreach (var parameterAgingData in _data)
         {
            for (int i = 0; i < parameterAgingData.IndividualIndexes.Count; i++)
            {
               var row = dataTable.NewRow();
               row[Constants.Population.INDIVIDUAL_ID_COLUMN] = parameterAgingData.IndividualIndexes[i];
               row[Constants.Population.PARAMETER_PATH_COLUMN] = inQuote(parameterAgingData.ParameterPath);

               //use string converter to ensure value are generated with "."
               row[Constants.Population.TIME_COLUMN] = parameterAgingData.Times[i].ConvertedTo<string>();
               row[Constants.Population.VALUE_COLUMN] = parameterAgingData.Values[i].ConvertedTo<string>();
               dataTable.Rows.Add(row);
            }
         }
         return dataTable;
      }

      private string inQuote(string text) => $"\"{text}\"";

      public virtual void Add(int individualIndex, string parameterPath, double time, double value)
      {
         if (!_data.Contains(parameterPath))
            Add(new ParameterAgingData { ParameterPath = parameterPath });

         _data[parameterPath].Add(individualIndex, time, value);
      }

      public virtual void Add(ParameterAgingData parameterAgingData)
      {
         _data.Add(parameterAgingData);
      }

      public virtual AgingData Clone()
      {
         var clone = new AgingData();
         _data.Each(x => clone.Add(x.Clone()));
         return clone;
      }
   }

   public class ParameterAgingData
   {
      public string ParameterPath { get; set; }
      public List<int> IndividualIndexes { get; set; }
      public List<double> Values { get; set; }
      public List<double> Times { get; set; }

      public ParameterAgingData()
      {
         IndividualIndexes = new List<int>();
         Values = new List<double>();
         Times = new List<double>();
      }

      public virtual void Add(int individualIndex, double time, double value)
      {
         IndividualIndexes.Add(individualIndex);
         Times.Add(time);
         Values.Add(value);
      }

      public virtual ParameterAgingData Clone()
      {
         return new ParameterAgingData
         {
            ParameterPath = ParameterPath,
            IndividualIndexes = new List<int>(IndividualIndexes),
            Values = new List<double>(Values),
            Times = new List<double>(Times),
         };
      }
   }
}
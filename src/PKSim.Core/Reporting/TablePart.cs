using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Reporting
{
   public class TablePart : ReportPart
   {
      public string KeyName { get; private set; }

      public IReadOnlyList<string> ValueNames => _valueNames;

      private readonly ICache<string, IReadOnlyList<string>> _allRows = new Cache<string, IReadOnlyList<string>>();
      private readonly List<string> _valueNames;
      public string Caption { get; set; }
      public ICache<string, Type> Types { get; } = new Cache<string, Type>(s => typeof (string));

      public TablePart(string keyName, params string[] valueName)
      {
         KeyName = keyName;
         Caption = string.Empty;
         _valueNames = valueName.ToList();
         if (!_valueNames.Any())
            _valueNames.AddRange(new[] {PKSimConstants.UI.Value, PKSimConstants.UI.Unit});
      }

      public void Merge(TablePart tablePartToMerge)
      {
         tablePartToMerge.Rows.Each(row => AddIs(row.Key, row.Value));
      }

      public void AddIs(IParameter parameter, IRepresentationInfoRepository representationInfoRepository)
      {
         AddIs(representationInfoRepository.DisplayNameFor(parameter), ParameterMessages.DisplayValueFor(parameter, numericalDisplayOnly: true), ParameterMessages.DisplayUnitFor(parameter));
      }

      public void AddIs(string name, params string[] values)
      {
         AddIs(name, values.ToList());
      }

      public void AddIs(string name, IReadOnlyList<string> values)
      {
         _allRows.Add(name, values);
      }

      public IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> Rows => _allRows.KeyValues;

      public override string Content
      {
         get
         {
            var sb = new StringBuilder();

            foreach (var allRow in _allRows.KeyValues)
            {
               sb.AppendLine(PKSimConstants.UI.ReportIs(allRow.Key, splatValuesFor(allRow.Value)));
            }

            return sb.ToString();
         }
      }

      private static string splatValuesFor(IEnumerable<string> values)
      {
         return values.Where(x => !x.IsNullOrEmpty()).ToString(" ");
      }
   }
}
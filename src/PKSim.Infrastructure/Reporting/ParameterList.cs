using System.Collections;
using System.Collections.Generic;
using PKSim.Assets;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting
{
   public class ParameterList : IEnumerable<IParameter>
   {
      private readonly IList<IParameter> _parameters;
      public string Caption { get; }

      public ParameterList(string caption, params IParameter[] parameters)
         : this(caption, new List<IParameter>(parameters))
      {
      }

      public ParameterList(string caption, IEnumerable<IParameter> parameters)
      {
         _parameters = new List<IParameter>(parameters);
         Caption = caption;
      }

      public IEnumerator<IParameter> GetEnumerator()
      {
         return _parameters.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public TablePart ToTable(IRepresentationInfoRepository representationInfoRepository)
      {
         var table = new TablePart(PKSimConstants.UI.Parameter, PKSimConstants.UI.Value, PKSimConstants.UI.Unit) {Caption = Caption};
         table.Types[PKSimConstants.UI.Value] = typeof (double);
         foreach (var parameter in _parameters)
         {
            table.AddIs(parameter, representationInfoRepository);
         }
         return table;
      }
   }
}
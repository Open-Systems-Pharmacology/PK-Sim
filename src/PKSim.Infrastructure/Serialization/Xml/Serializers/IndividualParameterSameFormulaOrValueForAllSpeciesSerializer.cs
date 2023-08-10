using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IndividualParameterSameFormulaOrValueForAllSpeciesSerializer : BaseXmlSerializer<IndividualParameterSameFormulaOrValueForAllSpecies>
   {
      public override void PerformMapping()
      {
         Map(x => x.ContainerPath);
         Map(x => x.ParameterName);
         Map(x => x.ContainerId);
         Map(x => x.IsSameFormula);
      }
   }
}

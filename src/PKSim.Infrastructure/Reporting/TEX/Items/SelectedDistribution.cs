using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Items
{
   public class SelectedDistribution
   {
      public IVectorialParametersContainer ParameterContainer { get; private set; }
      public int StructureElementOffset { get; private set; }

      public SelectedDistribution(IVectorialParametersContainer parameterContainer, int structureElementOffset = 0)
      {
         ParameterContainer = parameterContainer;
         StructureElementOffset = structureElementOffset;
      }
   }
}
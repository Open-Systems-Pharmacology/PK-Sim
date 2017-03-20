using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.Nodes
{
   public class CompoundParameterNodeType : RootNodeType
   {
      public static readonly CompoundParameterNodeType SpecificIntestinalPermeability = new CompoundParameterNodeType(PKSimConstants.UI.SpecificIntestinalPermeability, ApplicationIcons.Permeability);
      public static readonly CompoundParameterNodeType DistributionCalculation = new CompoundParameterNodeType(PKSimConstants.UI.DistributionCalculation, ApplicationIcons.DistributionCalculation);

      private CompoundParameterNodeType(string displayName, ApplicationIcon applicationIcon) : base(displayName, applicationIcon)
      {
      }
   }
}
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public class ExportIndividualToPKMLCommand : ExportBuildingBlockToPKMLCommand<Individual, IndividualBuildingBlock>
   {
      public ExportIndividualToPKMLCommand(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, IIndividualToIndividualBuildingBlockMapper mapper) : base(dialogCreator, pkmlPersistor, mapper, PKSimConstants.UI.ExportIndividual)
      {
      }
   }
}
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Mappers;

namespace PKSim.Presentation.UICommands
{
   public class ExportIndividualToPkmlCommand : PkmlExportCommandForBuildingBlock<Individual, IndividualBuildingBlock>
   {
      public ExportIndividualToPkmlCommand(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, IIndividualToIndividualBuildingBlockMapper mapper) : base(dialogCreator, pkmlPersistor, mapper)
      {
      }

      protected override string DialogCaption()
      {
         return PKSimConstants.UI.ExportIndividual;
      }
   }
}
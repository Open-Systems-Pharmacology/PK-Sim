using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public class ExportExpressionProfileToPkmlCommand : PkmlExportCommandForBuildingBlock<ExpressionProfile, ExpressionProfileBuildingBlock>
   {
      public ExportExpressionProfileToPkmlCommand(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, IExpressionProfileToExpressionProfileBuildingBlockMapper mapper) :
         base(dialogCreator, pkmlPersistor, mapper)
      {

      }

      protected override string DialogCaption()
      {
         return PKSimConstants.UI.ExportExpressionProfileToPkml;
      }
   }
}
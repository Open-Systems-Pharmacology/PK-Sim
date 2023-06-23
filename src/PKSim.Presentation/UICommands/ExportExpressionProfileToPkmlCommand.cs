using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public class ExportExpressionProfileToPKMLCommand : ExportBuildingBlockToPKMLCommand<ExpressionProfile, ExpressionProfileBuildingBlock>
   {
      public ExportExpressionProfileToPKMLCommand(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, IExpressionProfileToExpressionProfileBuildingBlockMapper mapper) :
         base(dialogCreator, pkmlPersistor, mapper, PKSimConstants.UI.ExportExpressionProfile)
      {
      }
   }
}
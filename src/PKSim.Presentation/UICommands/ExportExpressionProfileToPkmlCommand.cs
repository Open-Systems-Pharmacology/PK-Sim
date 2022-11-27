using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public class ExportExpressionProfileToPkmlCommand : ObjectUICommand<ExpressionProfile>
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IPKMLPersistor _pkmlPersistor;

      private readonly IExpressionProfileToExpressionProfileBuildingBlockMapper _mapper;
      //possibly we need to pack all this in a task
      public ExportExpressionProfileToPkmlCommand(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, 
         IExpressionProfileToExpressionProfileBuildingBlockMapper mapper)
      {
         _dialogCreator = dialogCreator;
         _pkmlPersistor = pkmlPersistor;
         _mapper = mapper;
      }

      protected override void PerformExecute()
      {
         var file = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportExpressionProfileToPkml, Constants.Filter.PKML_FILE_FILTER, Subject.Name);
         if (string.IsNullOrEmpty(file)) return;

         //first map to expression profile Core
         var expressionProfileBuildingBlock = _mapper.MapFrom(Subject);
         _pkmlPersistor.SaveToPKML(expressionProfileBuildingBlock, file);
      }
   }
}
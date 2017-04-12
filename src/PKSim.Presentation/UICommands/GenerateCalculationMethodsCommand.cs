using System.Linq;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using ICoreCalculationMethodRepository = PKSim.Core.Repositories.ICoreCalculationMethodRepository;

namespace PKSim.Presentation.UICommands
{
   public class GenerateCalculationMethodsCommand : IUICommand
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly ICoreCalculationMethodRepository _calculationMethodRepository;
      private readonly IPKMLPersistor _pkmlPersistor;

      public GenerateCalculationMethodsCommand(IDialogCreator dialogCreator, ICoreCalculationMethodRepository calculationMethodRepository, IPKMLPersistor pkmlPersistor)
      {
         _dialogCreator = dialogCreator;
         _calculationMethodRepository = calculationMethodRepository;
         _pkmlPersistor = pkmlPersistor;
      }

      public void Execute()
      {
         var file = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportCalculationMethodsToMoBiTitle, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART, CoreConstants.DefaultCalculationMethodsFileNameForMoBi);
         if (string.IsNullOrEmpty(file)) return;

         var mobiRepository = new CoreCalculationMethodRepository();
         _calculationMethodRepository.All().Each(mobiRepository.AddCalculationMethod);
         _pkmlPersistor.SaveToPKML(mobiRepository, file);
      }
   }
}
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class GenerateGroupsTemplateCommand : IUICommand
   {
      private readonly IGroupRepository _groupRepository;
      private readonly IGroupRepositoryPersistor _groupRepositoryPersistor;
      private readonly IDialogCreator _dialogCreator;

      public GenerateGroupsTemplateCommand(IGroupRepository groupRepository, IGroupRepositoryPersistor groupRepositoryPersistor, IDialogCreator dialogCreator)
      {
         _groupRepository = groupRepository;
         _groupRepositoryPersistor = groupRepositoryPersistor;
         _dialogCreator = dialogCreator;
      }

      public void Execute()
      {
         var exportFile = _dialogCreator.AskForFileToSave("Groups Template File", Constants.Filter.XML_FILE_FILTER,  Constants.DirectoryKey.MODEL_PART,"GroupRepository");
         if (string.IsNullOrEmpty(exportFile))
            return;

         _groupRepositoryPersistor.Save(_groupRepository, exportFile);
      }
   }
}
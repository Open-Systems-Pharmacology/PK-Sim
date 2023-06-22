using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public abstract class ExportBuildingBlockToPKMLCommand<T, TBuildingBlock> : ObjectUICommand<T> where T : PKSimBuildingBlock
   {
      protected IDialogCreator _dialogCreator;
      protected IPKMLPersistor _pkmlPersistor;
      protected IPathAndValueBuildingBlockMapper<T,TBuildingBlock> _mapper;
      private readonly string _caption;

      protected ExportBuildingBlockToPKMLCommand(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, IPathAndValueBuildingBlockMapper<T, TBuildingBlock> mapper, string caption)
      {
         _dialogCreator = dialogCreator;
         _pkmlPersistor = pkmlPersistor;
         _mapper = mapper;
         _caption = caption;
      }

      protected override void PerformExecute()
      {
         var file = _dialogCreator.AskForFileToSave(_caption, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART, Subject.Name);
         if (string.IsNullOrEmpty(file)) 
            return;

         //first map to core building block
         var buildingBlock = _mapper.MapFrom(Subject);
         _pkmlPersistor.SaveToPKML(buildingBlock, file);
      }

   }
}
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public abstract class PkmlExportCommandForBuildingBlock<T, TBuildingBlock> : ObjectUICommand<T> where T : PKSimBuildingBlock
   {
      protected IDialogCreator _dialogCreator;
      protected IPKMLPersistor _pkmlPersistor;
      protected IPathAndValueBuildingBlockMapper<T,TBuildingBlock> _mapper;

      protected PkmlExportCommandForBuildingBlock(IDialogCreator dialogCreator, IPKMLPersistor pkmlPersistor, IPathAndValueBuildingBlockMapper<T, TBuildingBlock> mapper)
      {
         _dialogCreator = dialogCreator;
         _pkmlPersistor = pkmlPersistor;
         _mapper = mapper;
      }

      protected override void PerformExecute()
      {
         var file = _dialogCreator.AskForFileToSave(DialogCaption(), Constants.Filter.PKML_FILE_FILTER, Subject.Name);
         if (string.IsNullOrEmpty(file)) return;

         //first map to core building block
         var expressionProfileBuildingBlock = _mapper.MapFrom(Subject);
         _pkmlPersistor.SaveToPKML(expressionProfileBuildingBlock, file);
      }

      protected abstract string DialogCaption();
   }
}
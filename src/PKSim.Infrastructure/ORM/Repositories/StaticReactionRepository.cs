using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class StaticReactionRepository : StartableRepository<PKSimReaction>, IStaticReactionRepository
   {
      private readonly IFlatModelRepository _flatModelRepository;
      private readonly IFlatProcessRepository _flatProcessesRepository;
      private readonly IFlatModelProcessRepository _modelProcessRepository;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatProcessToActiveProcessMapper _activeProcessMapper;

      //all reactions (cached by name)
      private readonly ICache<string, PKSimReaction> _allReactions;

      //reactions cached by model name
      private readonly ICache<string, IList<PKSimReaction>> _reactionsByModel;

      public StaticReactionRepository(IFlatModelRepository flatModelRepository,
         IFlatProcessRepository flatProcessesRepository,
         IFlatModelProcessRepository modelProcessRepository,
         IParameterContainerTask parameterContainerTask,
         IFlatProcessToActiveProcessMapper activeProcessMapper)
      {
         _flatModelRepository = flatModelRepository;
         _flatProcessesRepository = flatProcessesRepository;
         _modelProcessRepository = modelProcessRepository;
         _parameterContainerTask = parameterContainerTask;
         _activeProcessMapper = activeProcessMapper;
         _reactionsByModel = new Cache<string, IList<PKSimReaction>>();
         _allReactions = new Cache<string, PKSimReaction>(r => r.Name);
      }

      protected override void DoStart()
      {
         fillReactions();

         foreach (var flatModel in _flatModelRepository.All())
         {
            var reactionsForModel = reactionsFor(flatModel.Id);
            _reactionsByModel.Add(flatModel.Id, reactionsForModel.ToList());
         }
      }

      private IEnumerable<PKSimReaction> reactionsFor(string modelName)
      {
         var processNamesForModel = _modelProcessRepository.AllFor(modelName).Select(p => p.Process).ToList();
         return _allReactions.Where(r => processNamesForModel.Contains(r.Name));
      }

      private void fillReactions()
      {
         foreach (var flatProc in _flatProcessesRepository.All().Where(processIsNonTemplateReaction))
         {
            _allReactions.Add(_activeProcessMapper.MapFrom(flatProc).DowncastTo<PKSimReaction>());
         }

         _allReactions.Each(process => _parameterContainerTask.AddProcessBuilderParametersTo(process));
      }

      private bool processIsNonTemplateReaction(FlatProcess flatProcess)
      {
         return !flatProcess.IsTemplate && flatProcess.ActionType == ProcessActionType.Reaction;
      }

      public override IEnumerable<PKSimReaction> All()
      {
         Start();
         return _allReactions;
      }

      public IEnumerable<PKSimReaction> AllFor(string modelName)
      {
         Start();
         return _reactionsByModel[modelName];
      }
   }
}
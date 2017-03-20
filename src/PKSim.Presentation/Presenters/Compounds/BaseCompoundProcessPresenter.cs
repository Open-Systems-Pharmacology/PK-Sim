using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IProcessPresenter<in TCompoundProcess> : ICommandCollectorPresenter
   {
      void Edit(TCompoundProcess process);
      void DescriptionChanged(string description);
      IEnumerable<Species> AllSpecies();
      void SpeciesChanged(Species species);
   }

   public abstract class BaseCompoundProcessPresenter<TCompoundProcess, TView, TPresenter, TProcessDTO> :
      AbstractCommandCollectorPresenter<TView, TPresenter>, IProcessPresenter<TCompoundProcess>
      where TPresenter : IPresenter
      where TView : IView<TPresenter>, IProcessView<TProcessDTO>
      where TCompoundProcess : CompoundProcess, IValidatable, INotifier, IContainer
      where TProcessDTO : ProcessDTO<TCompoundProcess>
   {
      private readonly IParametersByGroupPresenter _parametersPresenter;
      private readonly IEntityTask _entityTask;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IMapper<TCompoundProcess, TProcessDTO> _compoundProcessDTOMapper;
      protected ICompoundProcessTask _processTask;
      private TCompoundProcess _compoundProcess;

      protected BaseCompoundProcessPresenter(TView view,
         IParametersByGroupPresenter parametersPresenter,
         IEntityTask entityTask, ISpeciesRepository speciesRepository,
         IRepresentationInfoRepository representationInfoRepository,
         IMapper<TCompoundProcess, TProcessDTO> compoundProcessDTOMapper,
         ICompoundProcessTask compoundProcessTask)
         : base(view)
      {
         _parametersPresenter = parametersPresenter;
         _entityTask = entityTask;
         _speciesRepository = speciesRepository;
         _representationInfoRepository = representationInfoRepository;
         _compoundProcessDTOMapper = compoundProcessDTOMapper;
         _processTask = compoundProcessTask;
         _view.SetParametersView(_parametersPresenter.View);
         AddSubPresenters(_parametersPresenter);
      }

      public virtual void Edit(TCompoundProcess process)
      {
         _compoundProcess = process;
         rebind(process);
         using (new BatchUpdate(_parametersPresenter))
         {
            _parametersPresenter.EditParameters(process.AllParameters());
            _view.AdjustParametersHeight(_parametersPresenter.OptimalHeight);
         }
      }

      private void rebind(TCompoundProcess process)
      {
         var dto = _compoundProcessDTOMapper.MapFrom(process);
         _view.SpeciesVisible = process.IsAnImplementationOf<ISpeciesDependentCompoundProcess>();

         _view.BindTo(dto);
      }

      public void DescriptionChanged(string description)
      {
         AddCommand(_entityTask.UpdateDescription(_compoundProcess, description));
      }

      public string DisplayNameFor(Species species)
      {
         return _representationInfoRepository.DisplayNameFor(species);
      }

      public IEnumerable<Species> AllSpecies()
      {
         return _speciesRepository.All();
      }

      public void SpeciesChanged(Species species)
      {
         AddCommand(_processTask.SetSpeciesForProcess(_compoundProcess, species));
         _parametersPresenter.RefreshParameters();
      }
   }
}
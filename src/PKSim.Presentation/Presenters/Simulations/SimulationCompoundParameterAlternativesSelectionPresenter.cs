using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundParameterAlternativesSelectionPresenter : IEditSimulationCompoundPresenter, IPresenter<ISimulationCompoundParameterAlternativesSelectionView>
   {
      IEnumerable<ParameterAlternative> AllAlternativesFor(CompoundParameterSelectionDTO compoundParameterSelectionDTO);
   }

   public class SimulationCompoundParameterAlternativesSelectionPresenter : AbstractSubPresenter<ISimulationCompoundParameterAlternativesSelectionView, ISimulationCompoundParameterAlternativesSelectionPresenter>, ISimulationCompoundParameterAlternativesSelectionPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private SimulationCompoundParameterMappingDTO _parameterMappingDTO;
      private Simulation _simulation;
      private readonly ISimulationToSimulationCompoundParameterMappingDTOMapper _simulationCompoundParameterMappingDTOMapper;
      private CompoundProperties _compoundProperties;
      private Compound _compound;

      public SimulationCompoundParameterAlternativesSelectionPresenter(ISimulationCompoundParameterAlternativesSelectionView view, ILazyLoadTask lazyLoadTask, ISimulationToSimulationCompoundParameterMappingDTOMapper simulationCompoundParameterMappingDTOMapper) : base(view)
      {
         _lazyLoadTask = lazyLoadTask;
         _simulationCompoundParameterMappingDTOMapper = simulationCompoundParameterMappingDTOMapper;
      }

      public IEnumerable<ParameterAlternative> AllAlternativesFor(CompoundParameterSelectionDTO compoundParameterSelectionDTO)
      {
         return compoundParameterSelectionDTO.CompoundParameterGroup.AllAlternatives;
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         _simulation = simulation;
         _compound = compound;
         _compoundProperties = _simulation.CompoundPropertiesFor(_compound);
         updateActiveCompound();
      }

      private void updateActiveCompound()
      {
         _lazyLoadTask.Load(_compound);
         _parameterMappingDTO = _simulationCompoundParameterMappingDTOMapper.MapFrom(_simulation, _compound);
         _view.BindTo(_parameterMappingDTO.AllParameterGroups());
      }

      public void SaveConfiguration()
      {
         _compoundProperties.ClearGroupMapping();
         _parameterMappingDTO.SelectedAlternatives().Each(
            alternative => _compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection
            {
               GroupName = alternative.GroupName,
               AlternativeName = alternative.Name
            }));
      }
   }
}
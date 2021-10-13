using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundProtocolFormulationPresenter : IPresenter<ISimulationCompoundProtocolFormulationView>, IEditSimulationCompoundPresenter
   {
      IEnumerable<FormulationSelectionDTO> AllFormulationsFor(FormulationMappingDTO formulationMappingDTO);
      void CreateFormulationFor(FormulationMappingDTO formulationMappingDTO);
      Task LoadFormulationForAsync(FormulationMappingDTO formulationMappingDTO);
      bool FormulationVisible { get; }
      void UpdateSelectedFormulation(Formulation templateFormulation);
   }

   public class SimulationCompoundProtocolFormulationPresenter : AbstractSubPresenter<ISimulationCompoundProtocolFormulationView, ISimulationCompoundProtocolFormulationPresenter>,
      ISimulationCompoundProtocolFormulationPresenter
   {
      private readonly IFormulationTask _formulationTask;
      private readonly IFormulationMappingDTOToFormulationMappingMapper _formulationMappingMapper;
      private readonly IFormulationFromMappingRetriever _formulationFromMappingRetriever;
      private readonly IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      private Protocol _protocol;
      private ProtocolProperties _protocolProperties;
      private IEnumerable<FormulationMappingDTO> _allFormulationMappingDTO;
      private Simulation _simulation;

      public SimulationCompoundProtocolFormulationPresenter(ISimulationCompoundProtocolFormulationView view,
         IFormulationTask formulationTask,
         IFormulationMappingDTOToFormulationMappingMapper formulationMappingMapper,
         IFormulationFromMappingRetriever formulationFromMappingRetriever, IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer)
         : base(view)
      {
         _formulationTask = formulationTask;
         _formulationMappingMapper = formulationMappingMapper;
         _formulationFromMappingRetriever = formulationFromMappingRetriever;
         _buildingBlockSelectionDisplayer = buildingBlockSelectionDisplayer;
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         var compoundProperties = simulation.CompoundPropertiesFor(compound);
         _protocolProperties = compoundProperties.ProtocolProperties;
         _protocol = _protocolProperties.Protocol;
         _simulation = simulation;
         _allFormulationMappingDTO = createFormulationMapping().ToList();
         _view.FormulationVisible = _allFormulationMappingDTO.Any();
         _view.BindTo(_allFormulationMappingDTO);
         _view.FormulationKeyVisible = (_allFormulationMappingDTO.Count() > 1);
      }

      private IEnumerable<FormulationMappingDTO> createFormulationMapping()
      {
         if (_protocol == null)
            return Enumerable.Empty<FormulationMappingDTO>();

         return (from usedFormulationKey in _protocol.UsedFormulationKeys
            where !usedFormulationKey.IsNullOrEmpty()
            let applicationType = _protocol.ApplicationTypeUsing(usedFormulationKey)
            let formulation = formulationUsedInSimulationFor(applicationType, usedFormulationKey) ?? defaultFormulationFor(applicationType, usedFormulationKey)
            select new FormulationMappingDTO
            {
               Selection = selectionFrom(formulation),
               ApplicationType = applicationType,
               FormulationKey = usedFormulationKey
            }).ToList();
      }

      private Formulation defaultFormulationFor(ApplicationType applicationType, string usedFormulationKey)
      {
         var allApplicationForType = _formulationTask.All().Where(f => f.HasRoute(applicationType.Route)).ToList();
         return allApplicationForType.FindByName(usedFormulationKey) ?? allApplicationForType.FirstOrDefault();
      }

      private Formulation formulationUsedInSimulationFor(ApplicationType applicationType, string usedFormulationKey)
      {
         var formulation = _formulationFromMappingRetriever.TemplateFormulationUsedBy(_simulation, _protocolProperties.MappingWith(usedFormulationKey));
         return formulation != null && formulation.HasRoute(applicationType.Route) ? formulation : null;
      }

      public IEnumerable<FormulationSelectionDTO> AllFormulationsFor(FormulationMappingDTO formulationMappingDTO)
      {
         var formulationSelections = _formulationTask.All().Where(f => f.HasRoute(formulationMappingDTO.Route)).Select(selectionFrom);
         var hashSet = new HashSet<FormulationSelectionDTO>(formulationSelections);
         if (formulationMappingDTO.Selection != null)
            hashSet.Add(formulationMappingDTO.Selection);

         return hashSet;
      }

      private FormulationSelectionDTO selectionFrom(Formulation formulation)
      {
         return new FormulationSelectionDTO
         {
            BuildingBlock = formulation,
            DisplayName = _buildingBlockSelectionDisplayer.DisplayNameFor(formulation)
         };
      }

      public void CreateFormulationFor(FormulationMappingDTO formulationMappingDTO)
      {
         var formulation = _formulationTask.CreateFormulationForRoute(formulationMappingDTO.Route);
         if (formulation == null) return;
         updateFormulationInMapping(formulationMappingDTO, formulation);
      }

      public async Task LoadFormulationForAsync(FormulationMappingDTO formulationMappingDTO)
      {
         var formulation = await _formulationTask.LoadFormulationForRoute(formulationMappingDTO.Route);
         updateFormulationInMapping(formulationMappingDTO, formulation);
      }

      public bool FormulationVisible => _view.FormulationVisible;

      private void updateFormulationInMapping(FormulationMappingDTO formulationMappingDTO, Formulation formulation)
      {
         formulationMappingDTO.Selection = selectionFrom(formulation);
         _view.RefreshData();
         OnStatusChanged();
      }

      public void UpdateSelectedFormulation(Formulation templateFormulation)
      {
         _allFormulationMappingDTO.Where(x => x.Formulation.IsNamed(templateFormulation.Name)).ToList()
            .Each(x => updateFormulationInMapping(x, templateFormulation));
      }

      public void SaveConfiguration()
      {
         _protocolProperties.ClearFormulationMapping();

         _allFormulationMappingDTO.Each(formulationMappingDTO =>
         {
            _formulationTask.Load(formulationMappingDTO.Formulation);
            _protocolProperties.AddFormulationMapping(_formulationMappingMapper.MapFrom(formulationMappingDTO, _simulation));
         });
      }
   }
}
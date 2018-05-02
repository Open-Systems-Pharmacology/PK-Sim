using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Formulations;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface IFormulationSettingsPresenter : IFormulationItemPresenter
   {
      IEnumerable<FormulationTypeDTO> AllFormulationTypes();
      event Action<string> FormulationTypeChanged;
      void OnFormulationTypeChanged();
      Formulation Formulation { get; }
      void EditFormulationFor(string applicationRoute);
      string ApplicationRoute { get; set; }
      bool CanEditFormulationType { set; }

      /// <summary>
      ///    Specifies whether the formulation should be saved as soon as a command is raised or only whne triggered explictely.
      ///    Default is false
      /// </summary>
      bool AutoSave { get; set; }

      void SaveFormulation();
   }

   public class FormulationSettingsPresenter : AbstractSubPresenter<IFormulationSettingsView, IFormulationSettingsPresenter>,
      IFormulationSettingsPresenter
   {
      private readonly IFormulationToFormulationDTOMapper _formulationDTOMapper;
      private readonly IMultiParameterEditPresenter _formulationParametersPresenter;
      private readonly IFormulationRepository _formulationRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ITableFormulationPresenter _tableFormulationPresenter;
      private readonly ICloner _cloner;
      private readonly ISimpleChartPresenter _simpleChartPresenter;
      private readonly IFormulationValuesRetriever _formulationValuesRetriever;
      private FormulationDTO _formulationDTO;
      public string ApplicationRoute { get; set; }
      public event Action<string> FormulationTypeChanged = delegate { };
      public Formulation Formulation { get; private set; }
      public bool AutoSave { get; set; }

      public FormulationSettingsPresenter(IFormulationSettingsView view, IFormulationToFormulationDTOMapper formulationDTOMapper,
         IMultiParameterEditPresenter formulationParametersPresenter, IFormulationRepository formulationRepository,
         IRepresentationInfoRepository representationInfoRepository, ITableFormulationPresenter tableFormulationPresenter,
         ICloner cloner, ISimpleChartPresenter simpleChartPresenter,
         IFormulationValuesRetriever formulationValuesRetriever) : base(view)
      {
         _formulationDTOMapper = formulationDTOMapper;
         _formulationParametersPresenter = formulationParametersPresenter;
         _formulationRepository = formulationRepository;
         _representationInfoRepository = representationInfoRepository;
         _tableFormulationPresenter = tableFormulationPresenter;
         _cloner = cloner;
         _simpleChartPresenter = simpleChartPresenter;
         _formulationValuesRetriever = formulationValuesRetriever;
         _formulationParametersPresenter.IsSimpleEditor = true;
         _formulationParametersPresenter.ParameterChanged += parameterChanged;
         _view.AddChartView(_simpleChartPresenter.View);
         _tableFormulationPresenter.TableFormulaChanged += tableFormulaChanged;
         AddSubPresenters(_formulationParametersPresenter, _tableFormulationPresenter);
         AutoSave = false;
      }

      public bool CanEditFormulationType
      {
         set => _view.FormulationTypeVisible = value;
      }

      public void EditFormulationFor(string applicationRoute)
      {
         ApplicationRoute = applicationRoute;
         EditFormulation(_cloner.Clone(_formulationRepository.DefaultFormulationFor(applicationRoute)));
      }

      public void SaveFormulation()
      {
         //nothing to do when the formulation is not table
         if (!Formulation.IsTable)
            return;

         _tableFormulationPresenter.Save();
      }

      public void EditFormulation(Formulation formulation)
      {
         Formulation = formulation;
         _formulationDTO = _formulationDTOMapper.MapFrom(formulation);
         adjustParameterVisibility();
         if (Formulation.IsTable)
         {
            _view.AddParameterView(_tableFormulationPresenter.BaseView);
            _tableFormulationPresenter.Edit(formulation);
         }
         else
         {
            _formulationParametersPresenter.Edit(_formulationDTO.Parameters);
            _view.AddParameterView(_formulationParametersPresenter.View);
         }
         updatePlot();
         _view.BindTo(_formulationDTO);
      }

      private void tableFormulaChanged(object sender, EventArgs eventArgs)
      {
         updatePlot();
         saveFormulationIfRequired();
      }

      private void saveFormulationIfRequired()
      {
         if (!AutoSave) return;
         SaveFormulation();
      }

      private void updatePlot()
      {
         if (noPlotForFormulation())
         {
            View.ChartVisible = false;
            return;
         }

         if (Formulation.IsTable)
            _simpleChartPresenter.Plot(_tableFormulationPresenter.EditedFormula);
         else
            _simpleChartPresenter.Plot(_formulationValuesRetriever.TableValueFor(Formulation));

         View.ChartVisible = true;
      }

      private bool noPlotForFormulation()
      {
         return Formulation.IsParticleDissolution || Formulation.IsDissolved;
      }

      private void adjustParameterVisibility()
      {
         if (!Formulation.IsParticleDissolution)
            return;

         var cache = new Cache<string, IParameter>(p => p.Name);
         cache.AddRange(_formulationDTO.Parameters);
         cache.Each(p => p.Visible = true);

         var particleDisperseSystem = cache[CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM];
         var particleDistribution = cache[CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION];
         var parameterToHide = new List<string>();

         if (particleDisperseSystem.Value == CoreConstants.Parameters.MONODISPERSE)
            parameterToHide.AddRange(CoreConstants.Parameters.HiddenParameterForMonodisperse);

         else if (particleDistribution.Value == CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL)
            parameterToHide.AddRange(CoreConstants.Parameters.HiddenParameterForPolydisperseNormal);

         else
            parameterToHide.AddRange(CoreConstants.Parameters.HiddenParameterForPolydisperseLogNormal);

         parameterToHide.Each(p => cache[p].Visible = false);
      }

      private void parameterChanged(IParameter parameter)
      {
         updatePlot();

         if (!parameter.NameIsOneOf(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM, CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION))
            return;

         adjustParameterVisibility();
         _formulationParametersPresenter.Edit(_formulationDTO.Parameters);
         saveFormulationIfRequired();
      }

      public IEnumerable<FormulationTypeDTO> AllFormulationTypes()
      {
         return from formulation in _formulationRepository.AllFor(ApplicationRoute)
            where formulation.Name != CoreConstants.Formulation.EmptyFormulation
            let formulationInfo = _representationInfoRepository.InfoFor(formulation)
            select new FormulationTypeDTO {Id = formulation.FormulationType, DisplayName = formulationInfo.DisplayName};
      }

      public void OnFormulationTypeChanged()
      {
         var formulationType = _formulationDTO.Type.Id;
         FormulationTypeChanged(formulationType);
         var templateFormulation = _formulationRepository.FormulationBy(formulationType);
         EditFormulation(_cloner.Clone(templateFormulation).WithName(string.Empty));
      }
   }
}
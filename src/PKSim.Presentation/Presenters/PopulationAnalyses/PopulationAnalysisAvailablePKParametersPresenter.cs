using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;

using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisAvailablePKParametersPresenter : IPresenter<IPopulationAnalysisAvailablePKParametersView>, IInitializablePresenter<IPopulationDataCollector>
   {
      /// <summary>
      ///    Returns the selected pk parameters
      /// </summary>
      IEnumerable<QuantityPKParameter> SelectedPKParameters { get; }

      void QuantityPKParameterDTODoubleClicked(QuantityPKParameterDTO quantityPKParameterDTO);
      void QuantityPKParameterSelected(QuantityPKParameterDTO quantityPKParameterDTO);

      event EventHandler<PKParameterDoubleClickedEventArgs> QuantityPKParameterDoubleClicked;
      event EventHandler<PKParameterSelectedEventArgs> PKParameterSelected;

      string QuantityPathDisplayFor(QuantityPKParameter quantityPKParameter);
   }

   public class PopulationAnalysisAvailablePKParametersPresenter : AbstractPresenter<IPopulationAnalysisAvailablePKParametersView, IPopulationAnalysisAvailablePKParametersPresenter>, IPopulationAnalysisAvailablePKParametersPresenter
   {
      private readonly IQuantityToQuantitySelectionDTOMapper _quantitySelectionDTOMapper;
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private readonly IPKParameterRepository _pkParameterRepository;
      private readonly List<QuantityPKParameterDTO> _allPKParameters;
      private IPopulationDataCollector _populationDataCollector;
      public event EventHandler<PKParameterDoubleClickedEventArgs> QuantityPKParameterDoubleClicked = delegate { };
      public event EventHandler<PKParameterSelectedEventArgs> PKParameterSelected = delegate { };

      public PopulationAnalysisAvailablePKParametersPresenter(IPopulationAnalysisAvailablePKParametersView view,
         IQuantityToQuantitySelectionDTOMapper quantitySelectionDTOMapper, IEntitiesInContainerRetriever entitiesInContainerRetriever, IPKParameterRepository pkParameterRepository)
         : base(view)
      {
         _quantitySelectionDTOMapper = quantitySelectionDTOMapper;
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         _pkParameterRepository = pkParameterRepository;
         _allPKParameters = new List<QuantityPKParameterDTO>();
      }

      public void InitializeWith(IPopulationDataCollector populationDataCollector)
      {
         _populationDataCollector = populationDataCollector;
         createAvailableQuantitiesListBasedOn(populationDataCollector);
         _view.BindTo(_allPKParameters);
      }

      private void createAvailableQuantitiesListBasedOn(IPopulationDataCollector populationDataCollector)
      {
         _allPKParameters.Clear();
         _allPKParameters.AddRange(_entitiesInContainerRetriever.OutputsFrom(populationDataCollector).SelectMany(availablePKParametersFor));
      }

      public IEnumerable<QuantityPKParameter> SelectedPKParameters
      {
         get { return View.SelectedPKParameters.Select(x => x.PKParameter); }
      }

      public void QuantityPKParameterDTODoubleClicked(QuantityPKParameterDTO quantityPKParameterDTO)
      {
         if (quantityPKParameterDTO == null) return;
         QuantityPKParameterDoubleClicked(this, new PKParameterDoubleClickedEventArgs(quantityPKParameterDTO.PKParameter));
      }

      public void QuantityPKParameterSelected(QuantityPKParameterDTO quantityPKParameterDTO)
      {
         if (quantityPKParameterDTO == null) return;
         PKParameterSelected(this, new PKParameterSelectedEventArgs(quantityPKParameterDTO.PKParameter));
      }

      private IReadOnlyCollection<QuantityPKParameterDTO> availablePKParametersFor(IQuantity quantity)
      {
         var quantityDTO = _quantitySelectionDTOMapper.MapFrom(quantity);
         return _populationDataCollector.AllPKParametersFor(quantityDTO.QuantityPath)
            .Select(x => mapFrom(x, quantityDTO.DisplayPathAsString)).ToList();
      }

      public string QuantityPathDisplayFor(QuantityPKParameter quantityPKParameter)
      {
         return _allPKParameters.First(x => Equals(x.PKParameter, quantityPKParameter)).QuantityDisplayPath;
      }

      private QuantityPKParameterDTO mapFrom(QuantityPKParameter quantityPKParameter, string quantityDisplayPath)
      {
         return new QuantityPKParameterDTO
         {
            DisplayName = _pkParameterRepository.DisplayNameFor(quantityPKParameter.Name),
            PKParameter = quantityPKParameter,
            QuantityDisplayPath = quantityDisplayPath,
            Description = _pkParameterRepository.DescriptionFor(quantityPKParameter.Name)
         };
      }
   }
}
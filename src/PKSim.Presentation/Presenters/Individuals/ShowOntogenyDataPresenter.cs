using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IShowOntogenyDataPresenter : IDisposablePresenter
   {
      void Show(Ontogeny ontogeny);
      IEnumerable<IGroup> AllContainers();
      IEnumerable<Ontogeny> AllOntogenies();
      void OntogenyChanged();
      void ContainerChanged();
      string GroupDescriptionFor(int index);
   }

   public class ShowOntogenyDataPresenter : AbstractDisposablePresenter<IShowOntogenyDataView, IShowOntogenyDataPresenter>, IShowOntogenyDataPresenter
   {
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly ISimpleChartPresenter _simpleChartPresenter;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;
      private string _speciesName;
      private ShowOntogenyDataDTO _dto;
      private Ontogeny _ontogeny;

      public ShowOntogenyDataPresenter(IShowOntogenyDataView view, IOntogenyRepository ontogenyRepository,
         ISimpleChartPresenter simpleChartPresenter, IDimensionRepository dimensionRepository,
         IGroupRepository groupRepository, IDisplayUnitRetriever displayUnitRetriever) : base(view)
      {
         _ontogenyRepository = ontogenyRepository;
         _simpleChartPresenter = simpleChartPresenter;
         _dimensionRepository = dimensionRepository;
         _groupRepository = groupRepository;
         _displayUnitRetriever = displayUnitRetriever;
         _view.AddChart(_simpleChartPresenter.View);
      }

      public void Show(Ontogeny ontogeny)
      {
         _speciesName = ontogeny.SpeciesName;
         _ontogeny = ontogeny;
         _dto = new ShowOntogenyDataDTO {SelectedOntogeny = ontogeny};
         updateAvailableContainerForOntogeny();
         _view.BindTo(_dto);
         updateChart();
         _view.Display();
      }

      public IEnumerable<IGroup> AllContainers()
      {
         return _ontogenyRepository.AllValuesFor(_dto.SelectedOntogeny)
            .Select(x => _groupRepository.GroupByName(x.GroupName)).Distinct();
      }

      public IEnumerable<Ontogeny> AllOntogenies()
      {
         return _ontogenyRepository.AllFor(_speciesName).Union(new[] {_ontogeny}).OrderBy(x => x.DisplayName);
      }

      public void OntogenyChanged()
      {
         updateAvailableContainerForOntogeny();
         _view.UpdateContainers();
         updateChart();
      }

      public void ContainerChanged()
      {
         updateChart();
      }

      public string GroupDescriptionFor(int index)
      {
         var allContainers = AllContainers().ToList();
         return index < allContainers.Count ? allContainers[index].Description : string.Empty;
      }

      private void updateChart()
      {
         var xUnit = _displayUnitRetriever.PreferredUnitFor(_dimensionRepository.AgeInYears);
         var yUnit = _displayUnitRetriever.PreferredUnitFor(_dimensionRepository.Fraction);
         var chart = _simpleChartPresenter.Plot(dataForSelectedOntogeny(xUnit, yUnit), Scalings.Linear);
         chart.AxisBy(AxisTypes.X).Caption = PKSimConstants.UI.PostMenstrualAge;
         chart.AxisBy(AxisTypes.X).GridLines = true;
         chart.AxisBy(AxisTypes.Y).Caption = PKSimConstants.UI.OntogenyFor(_dto.SelectedOntogeny.Name);
         chart.AxisBy(AxisTypes.Y).GridLines = true;  
      }

      private DataRepository dataForSelectedOntogeny(Unit xUnit, Unit yUnit)
      {
         var dataRepository = new DataRepository {Name = PKSimConstants.UI.OntogenyFor(_dto.SelectedOntogeny.DisplayName)};
         var pma = new BaseGrid(PKSimConstants.UI.PostMenstrualAge, _dimensionRepository.AgeInYears) {DisplayUnit = xUnit};
         var mean = new DataColumn(dataRepository.Name, _dimensionRepository.Fraction, pma) {DisplayUnit = yUnit};
         var std = new DataColumn(PKSimConstants.UI.StandardDeviation, _dimensionRepository.Fraction, pma) { DisplayUnit = yUnit };
         mean.DataInfo.AuxiliaryType = AuxiliaryType.GeometricMeanPop;
         std.AddRelatedColumn(mean);
         dataRepository.Add(mean);
         dataRepository.Add(std);

         var allOntogenies = _ontogenyRepository.AllValuesFor(_dto.SelectedOntogeny, _dto.SelectedContainer.Name).OrderBy(x => x.PostmenstrualAge).ToList();
         pma.Values = values( allOntogenies, x => x.PostmenstrualAge);
         mean.Values = values(allOntogenies, x => x.OntogenyFactor);
         std.Values = values(allOntogenies, x => x.Deviation);
         return dataRepository;
      }

      private void updateAvailableContainerForOntogeny()
      {
         _dto.SelectedContainer = AllContainers().FirstOrDefault();
      }

      private float[] values(IEnumerable<OntogenyMetaData> allOntogenies, Func<OntogenyMetaData, double> valueFunc)
      {
         return allOntogenies.Select(valueFunc).ToFloatArray();
      }
   }
}
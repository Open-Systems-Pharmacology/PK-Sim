using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IShowOntogenyDataPresenter : IDisposablePresenter
   {
      void Show(Ontogeny ontogeny);
      IEnumerable<IGroup> AllGroups();
      IEnumerable<Ontogeny> AllOntogenies();
      void OntogenyChanged();
      void GroupChanged();
      string GroupDescriptionFor(int index);
   }

   public class ShowOntogenyDataPresenter : AbstractDisposablePresenter<IShowOntogenyDataView, IShowOntogenyDataPresenter>, IShowOntogenyDataPresenter
   {
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly ISimpleChartPresenter _simpleChartPresenter;
      private readonly IGroupRepository _groupRepository;
      private string _speciesName;
      private ShowOntogenyDataDTO _dto;
      private Ontogeny _ontogeny;

      public ShowOntogenyDataPresenter(
         IShowOntogenyDataView view,
         IOntogenyRepository ontogenyRepository,
         ISimpleChartPresenter simpleChartPresenter,
         IGroupRepository groupRepository) : base(view)
      {
         _ontogenyRepository = ontogenyRepository;
         _simpleChartPresenter = simpleChartPresenter;
         _simpleChartPresenter.PreExportHook = orderOntogenyColumns;
         _groupRepository = groupRepository;
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

      public IEnumerable<IGroup> AllGroups()
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

      public void GroupChanged()
      {
         updateChart();
      }

      public string GroupDescriptionFor(int index)
      {
         var allGroups = AllGroups().ToList();
         return index < allGroups.Count ? allGroups[index].Description : string.Empty;
      }

      private void updateChart()
      {
         var data = _ontogenyRepository.OntogenyToRepository(_dto.SelectedOntogeny, _dto.SelectedGroup.Name);
         var chart = _simpleChartPresenter.Plot(data, Scalings.Linear);
         chart.AxisBy(AxisTypes.X).Caption = PKSimConstants.UI.PostMenstrualAge;
         chart.AxisBy(AxisTypes.X).GridLines = true;
         chart.AxisBy(AxisTypes.Y).Caption = PKSimConstants.UI.OntogenyFor(_dto.SelectedOntogeny.Name);
         chart.AxisBy(AxisTypes.Y).GridLines = true;
         _simpleChartPresenter.Refresh();
      }


      private IEnumerable<DataColumn> orderOntogenyColumns(IEnumerable<DataColumn> dataColumns)
      {
         //we need to inverse the order of columns so that standard deviation is last and ontogeny is first 
         //this is the other way by constructions as the main curve is in fact the deviation and the related column is the mean
         return dataColumns.Reverse();
      }

      private void updateAvailableContainerForOntogeny()
      {
         _dto.SelectedGroup = AllGroups().FirstOrDefault();
      }
   }
}
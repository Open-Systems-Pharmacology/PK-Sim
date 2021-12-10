using System;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IRandomPopulationSettingsPresenter : IPopulationSettingsPresenter<RandomPopulation>
   {
   }

   public class RandomPopulationSettingsPresenter : AbstractSubPresenter<IRandomPopulationSettingsView, IRandomPopulationSettingsPresenter>, IRandomPopulationSettingsPresenter
   {
      private readonly IRandomPopulationFactory _randomPopulationFactory;
      private readonly IPopulationSettingsDTOMapper _populationSettingsMapper;
      private readonly ILazyLoadTask _lazyLoadTask;
      private PopulationSettingsDTO _populationSettingsDTO;
      private CancellationTokenSource _cancellationTokenSource;
      public bool PopulationCreated { get; private set; }
      public RandomPopulation Population { get; private set; }
      public event EventHandler<PopulationCreationEventArgs> PopulationCreationFinished = delegate { };
      public bool IsLatched { get; set; }

      public RandomPopulationSettingsPresenter(
         IRandomPopulationSettingsView view,
         IRandomPopulationFactory randomPopulationFactory,
         IPopulationSettingsDTOMapper populationSettingsMapper,
         ILazyLoadTask lazyLoadTask)
         : base(view)
      {
         _randomPopulationFactory = randomPopulationFactory;
         _populationSettingsMapper = populationSettingsMapper;
         _lazyLoadTask = lazyLoadTask;
      }

      public void PrepareForCreating(Individual basedIndividual)
      {
         _lazyLoadTask.Load(basedIndividual);
         _populationSettingsDTO = _populationSettingsMapper.MapFrom(basedIndividual);
         this.DoWithinLatch(updateView);
      }

      public void IndividualSelectionChanged(Individual newIndividual)
      {
         if (IsLatched) return;
         PrepareForCreating(newIndividual);
      }

      public async Task CreatePopulation()
      {
         _view.CreatingPopulation = true;
         _cancellationTokenSource = new CancellationTokenSource();
         try
         {
            Population = await _randomPopulationFactory.CreateFor(_populationSettingsMapper.MapFrom(_populationSettingsDTO), _cancellationTokenSource.Token);
            PopulationCreated = true;
            raisePopulationCreationFinish(success: true);
         }
         catch (Exception e)
         {
            raisePopulationCreationFinish(success: false);
            if (!(e is OperationCanceledException))
               throw;
         }
         finally
         {
            _view.CreatingPopulation = false;
         }
      }

      private void raisePopulationCreationFinish(bool success)
      {
         //never error or warning
         PopulationCreationFinished(this, new PopulationCreationEventArgs(success, hasWarningOrError: false));
      }

      public void Cancel()
      {
         _cancellationTokenSource?.Cancel();
      }

      public override void ViewChanged()
      {
         PopulationCreated = false;
         OnStatusChanged();
      }

      public void LoadPopulation(RandomPopulation population)
      {
         _view.UpdateLayoutForEditing();
         Population = population;
         _populationSettingsDTO = _populationSettingsMapper.MapFrom(Population.Settings);
         //do in latch since no update should be perform
         this.DoWithinLatch(updateView);
         PopulationCreated = true;
      }

      private void updateView()
      {
         _view.BindTo(_populationSettingsDTO);
      }
   }
}
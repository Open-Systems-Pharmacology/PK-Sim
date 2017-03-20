using System;
using System.Threading;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Presentation.Presenters;

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
      private CancellationTokenSource _cancelationTokenSource;
      public bool PopulationCreated { get; private set; }
      public RandomPopulation Population { get; private set; }
      public event EventHandler<PopulationCreationEventArgs> PopulationCreationFinished = delegate { };
      public bool IsLatched { get; set; }

      public RandomPopulationSettingsPresenter(IRandomPopulationSettingsView view, IRandomPopulationFactory randomPopulationFactory, IPopulationSettingsDTOMapper populationSettingsMapper,
                                               ILazyLoadTask lazyLoadTask)
         : base(view)
      {
         _randomPopulationFactory = randomPopulationFactory;
         _populationSettingsMapper = populationSettingsMapper;
         _lazyLoadTask = lazyLoadTask;
      }

      public void PrepareForCreating(PKSim.Core.Model.Individual basedIndividual)
      {
         _lazyLoadTask.Load(basedIndividual);
         _populationSettingsDTO = _populationSettingsMapper.MapFrom(basedIndividual);
         this.DoWithinLatch(updateView);
      }

      public void IndividualSelectionChanged(PKSim.Core.Model.Individual newIndividual)
      {
         if (IsLatched) return;
         PrepareForCreating(newIndividual);
      }

      public async void CreatePopulation()
      {
         _view.CreatingPopulation = true;
         _cancelationTokenSource = new CancellationTokenSource();
         try
         {
            Population = await _randomPopulationFactory.CreateFor(_populationSettingsMapper.MapFrom(_populationSettingsDTO), _cancelationTokenSource.Token);
            PopulationCreated = true;
            raisePopulationCreationFinish(success: true);
         }
         catch (OperationCanceledException)
         {
            raisePopulationCreationFinish(success: false);
         }
         finally
         {
            _view.CreatingPopulation = false;
         }
      }

      private void raisePopulationCreationFinish(bool success)
      {
         //never error or warning
         PopulationCreationFinished(this, new PopulationCreationEventArgs(success, hasWarningOrError:false));
      }

      public void Cancel()
      {
         if (_cancelationTokenSource == null) return;
         _cancelationTokenSource.Cancel();
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
         _view.GenderSelectionVisible = _populationSettingsDTO.HasMultipleGenders;
         _view.Population = _populationSettingsDTO.Population;
      }
   }
}
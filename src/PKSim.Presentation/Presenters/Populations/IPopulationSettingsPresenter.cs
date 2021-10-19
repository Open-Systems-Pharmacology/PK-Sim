using System;
using System.Threading.Tasks;
using OSPSuite.Utility;

namespace PKSim.Presentation.Presenters.Populations
{
   public class PopulationCreationEventArgs:EventArgs
   {
      public bool Success { get; private set; }
      public bool HasWarningOrError { get; private set; }

      public PopulationCreationEventArgs(bool success, bool hasWarningOrError)
      {
         Success = success;
         HasWarningOrError = hasWarningOrError;
      }
   }

   public interface IPopulationSettingsPresenter : IPopulationItemPresenter, ILatchable
   {
      /// <summary>
      ///    Initialize presenter in order to create a new population
      /// </summary>
      void PrepareForCreating(PKSim.Core.Model.Individual basedIndividual);

      /// <summary>
      ///    Function will be called whenever the based individual was changed (which will also make the additional parameter invalid)
      /// </summary>
      void IndividualSelectionChanged(PKSim.Core.Model.Individual newIndividual);

      /// <summary>
      ///    Create the population based on the settings defined by the user
      /// </summary>
      Task CreatePopulation();

      /// <summary>
      ///    Returns true if the population with the defined settings was created, otherwise false
      /// </summary>
      bool PopulationCreated { get; }

      /// <summary>
      /// Event is fired whenever a population has been created. The boolean indicates if the creation was successful or not
      /// The second arguments indicates if error or warnings were found: Set to true, errors and warnings found. Set to false, no error and no warnings
      /// </summary>
      event EventHandler<PopulationCreationEventArgs> PopulationCreationFinished;

      /// <summary>
      ///    This action should be call to cancel the population creation
      /// </summary>
      void Cancel();
   }


   public interface IPopulationSettingsPresenter<TPopulation> : IPopulationSettingsPresenter where TPopulation : PKSim.Core.Model.Population
   {
      /// <summary>
      ///    Return the created population
      /// </summary>
      TPopulation Population { get; }

      /// <summary>
      ///    Initialize the presenter with the population settings
      /// </summary>
      /// <param name="population">Population to edit</param>
      void LoadPopulation(TPopulation population);

   }
}
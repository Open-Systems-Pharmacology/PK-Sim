using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IMoleculeParameterVariabilityCreator
   {
      /// <summary>
      ///    Automatically adds variability to the <paramref name="population" />
      ///    but only for molecule parameters defined in the database.
      /// </summary>
      void AddVariabilityTo(Population population);

      /// <summary>
      ///    Automatically adds user defined variability to the <paramref name="population" /> for
      ///    the <paramref name="molecule" /> only if the information is available in the database and returns the
      ///    executed command. If<paramref name="usePredefinedMeanVariability" /> is <c>true</c>, the mean defined in the
      ///    database will be used. Otherwise the value defined in the molecule will used.
      /// </summary>
      ICommand AddMoleculeVariability(IndividualMolecule molecule, Population population, bool usePredefinedMeanVariability);
   }

   public class MoleculeParameterVariabilityCreator : IMoleculeParameterVariabilityCreator
   {
      private readonly IAdvancedParameterFactory _advancedParameterFactory;
      private readonly IMoleculeParameterRepository _moleculeParameterRepository;
      private readonly IAdvancedParametersTask _advancedParametersTask;
      private readonly IExecutionContext _executionContext;

      public MoleculeParameterVariabilityCreator(IAdvancedParameterFactory advancedParameterFactory, IMoleculeParameterRepository moleculeParameterRepository,
         IAdvancedParametersTask advancedParametersTask, IExecutionContext executionContext)
      {
         _advancedParameterFactory = advancedParameterFactory;
         _moleculeParameterRepository = moleculeParameterRepository;
         _advancedParametersTask = advancedParametersTask;
         _executionContext = executionContext;
      }

      public void AddVariabilityTo(Population population)
      {
         var individual = population.FirstIndividual;
         //use false because we want to use the default values defined in the individual
         individual.AllMolecules().Each(m => AddMoleculeVariability(m, population, usePredefinedMeanVariability: false));
      }

      public ICommand AddMoleculeVariability(IndividualMolecule molecule, Population population, bool usePredefinedMeanVariability)
      {
         var macroCommand = new PKSimMacroCommand(new[]
         {
            addParameterVariability(molecule, molecule.ReferenceConcentration, population, usePredefinedMeanVariability),
            addParameterVariability(molecule, molecule.HalfLifeLiver, population, usePredefinedMeanVariability),
            addParameterVariability(molecule, molecule.HalfLifeIntestine, population, usePredefinedMeanVariability)
         })
         {
            CommandType = PKSimConstants.Command.CommandTypeAdd,
            ObjectType = PKSimConstants.ObjectTypes.Population,
            Description = PKSimConstants.Command.AddDefaultVariabilityToPopulation(population.Name)
         };
         _executionContext.UpdateBuildinBlockProperties(macroCommand, population);
         return macroCommand;
      }

      private ICommand addParameterVariability(IndividualMolecule individualMolecule, IParameter parameter, Population population, bool usePredefinedMeanVariability)
      {
         var predefinedVariability = _moleculeParameterRepository.ParameterFor(individualMolecule.Name, parameter.Name);
         if (predefinedVariability == null)
            return new PKSimEmptyCommand();

         var advancedParameter = _advancedParameterFactory.Create(parameter, predefinedVariability.Formula.DistributionType());
         advancedParameter.DistributedParameter.MeanParameter.Value = usePredefinedMeanVariability ? predefinedVariability.MeanParameter.Value : parameter.Value;
         advancedParameter.DistributedParameter.DeviationParameter.Value = predefinedVariability.DeviationParameter.Value;

         return _advancedParametersTask.AddAdvancedParameter(advancedParameter, population);
      }
   }
}
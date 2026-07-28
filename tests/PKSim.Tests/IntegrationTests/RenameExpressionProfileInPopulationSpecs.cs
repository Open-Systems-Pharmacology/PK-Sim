using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_renaming_the_molecule_of_an_expression_profile_used_by_a_population : ContextForIntegration<IRenameBuildingBlockTask>
   {
      private Population _population;
      private ExpressionProfile _expressionProfile;
      private PKSimProject _project;
      private IEntityPathResolver _entityPathResolver;
      private string _referenceConcentrationPath;
      private double[] _valuesBeforeRename;
      private double _meanBeforeRename;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IRenameBuildingBlockTask>();
         _entityPathResolver = IoC.Resolve<IEntityPathResolver>();

         var individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(individual, "CYP3A4");
         _population = DomainFactoryForSpecs.CreateDefaultPopulation(individual);
         IoC.Resolve<IMoleculeParameterVariabilityCreator>().AddVariabilityTo(_population);

         _project = new PKSimProject();
         _project.AddBuildingBlock(_population);
         _project.AddBuildingBlock(_expressionProfile);
         IoC.Resolve<ICoreWorkspace>().Project = _project;
         _project.All<IPKSimBuildingBlock>().Each(IoC.Resolve<IRegistrationTask>().Register);

         var referenceConcentration = _population.MoleculeByName("CYP3A4").ReferenceConcentration;
         _referenceConcentrationPath = _entityPathResolver.PathFor(referenceConcentration);
         _valuesBeforeRename = _population.IndividualValuesCache.GetValues(_referenceConcentrationPath);
         _meanBeforeRename = _population.AdvancedParameterFor(_entityPathResolver, referenceConcentration)
            .DistributedParameter.MeanParameter.Value;
      }

      protected override void Because()
      {
         sut.RenameBuildingBlock(_expressionProfile, "CYP2D6|Human|Standard");
      }

      [Observation]
      public void should_have_renamed_the_molecule_in_the_advanced_parameter_paths()
      {
         pathsContaining(_population.AdvancedParameters.Select(x => x.ParameterPath), "CYP3A4").ShouldBeEmpty();
         pathsContaining(_population.AdvancedParameters.Select(x => x.ParameterPath), "CYP2D6").ShouldNotBeEmpty();
      }

      [Observation]
      public void should_have_renamed_the_molecule_in_the_generated_individual_values()
      {
         pathsContaining(_population.IndividualValuesCache.AllParameterPaths(), "CYP3A4").ShouldBeEmpty();
         pathsContaining(_population.IndividualValuesCache.AllParameterPaths(), "CYP2D6").ShouldNotBeEmpty();
      }

      [Observation]
      public void should_still_resolve_the_variability_of_the_renamed_molecule()
      {
         var molecule = _population.MoleculeByName("CYP2D6");
         _population.AdvancedParameterFor(_entityPathResolver, molecule.ReferenceConcentration).ShouldNotBeNull();
      }

      [Observation]
      public void should_still_export_the_varied_parameters_of_the_renamed_molecule()
      {
         var exported = _population.AllVectorialParameters(_entityPathResolver).Select(_entityPathResolver.PathFor);
         pathsContaining(exported, "CYP2D6").ShouldNotBeEmpty();
      }

      [Observation]
      public void should_move_the_existing_variability_instead_of_regenerating_it()
      {
         var referenceConcentration = _population.MoleculeByName("CYP2D6").ReferenceConcentration;
         var newPath = _entityPathResolver.PathFor(referenceConcentration);

         _population.IndividualValuesCache.GetValues(newPath).ShouldBeEqualTo(_valuesBeforeRename);

         _population.AdvancedParameterFor(_entityPathResolver, referenceConcentration)
            .DistributedParameter.MeanParameter.Value.ShouldBeEqualTo(_meanBeforeRename);
      }

      [Observation]
      public void should_keep_the_renamed_paths_when_the_population_values_are_cloned()
      {
         var clone = _population.IndividualValuesCache.Clone();
         pathsContaining(clone.AllParameterPaths(), "CYP3A4").ShouldBeEmpty();
      }

      private static string[] pathsContaining(System.Collections.Generic.IEnumerable<string> paths, string moleculeName) =>
         paths.Where(x => x.Contains(moleculeName)).ToArray();

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _project.All<IPKSimBuildingBlock>().Each(Unregister);
      }
   }
}

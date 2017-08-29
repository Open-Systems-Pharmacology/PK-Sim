using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeParameterVariabilityCreator : ContextSpecification<IMoleculeParameterVariabilityCreator>
   {
      protected IMoleculeParameterRepository _moleculeParameterFactory;
      protected IAdvancedParameterFactory _advancedParmeterFactory;
      protected RandomPopulation _randomPopulation;
      protected IndividualMolecule _molecule1;
      protected IndividualMolecule _molecule2;
      protected IAdvancedParametersTask _advancedParametersTask;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _moleculeParameterFactory = A.Fake<IMoleculeParameterRepository>();
         _advancedParmeterFactory = A.Fake<IAdvancedParameterFactory>();
         _advancedParametersTask = A.Fake<IAdvancedParametersTask>();
         _executionContext = A.Fake<IExecutionContext>();
         sut = new MoleculeParameterVariabilityCreator(_advancedParmeterFactory, _moleculeParameterFactory, _advancedParametersTask, _executionContext);

         _randomPopulation = new RandomPopulation {Settings = new RandomPopulationSettings()};
         _randomPopulation.SetAdvancedParameters(new AdvancedParameterCollection());
         var individual = A.Fake<Individual>();
         _molecule1 = DomainHelperForSpecs.DefaultIndividualMolecule().WithName("CYP1");
         _molecule2 = DomainHelperForSpecs.DefaultIndividualMolecule().WithName("CYP2");
         A.CallTo(() => individual.AllMolecules()).Returns(new[] {_molecule1, _molecule2});
         _randomPopulation.Settings.BaseIndividual = individual;

         A.CallTo(() => _advancedParametersTask.AddAdvancedParameter(A<AdvancedParameter>._, _randomPopulation))
            .Invokes(x =>
            {
               var advancedParameter = x.GetArgument<AdvancedParameter>(0);
               _randomPopulation.AddAdvancedParameter(advancedParameter);
            })
            .Returns(new AddAdvancedParameterToContainerCommand(new AdvancedParameter(), _randomPopulation, _executionContext));
      }
   }

   public class When_creating_the_variability_for_a_random_population : concern_for_MoleculeParameterVariabilityCreator
   {
      private IDistributedParameter _molecule1RefConcDistribution;
      private IDistributedParameter _molecule2HalfLifeIntestineDistribution;
      private AdvancedParameter _advancedParameterMolecule1RefConc;
      private AdvancedParameter _advancedParameterMolecule2HalfLifeIntestine;

      protected override void Context()
      {
         base.Context();

         _molecule1RefConcDistribution = DomainHelperForSpecs.NormalDistributedParameter(defaultMean: 4, defaultDeviation: 2).WithId("Molecule1RefConf");
         _molecule2HalfLifeIntestineDistribution = DomainHelperForSpecs.NormalDistributedParameter(defaultMean: 6, defaultDeviation: 3).WithId("Molecule2HalfLifeIntestine");

         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule1.Name, _molecule1.ReferenceConcentration.Name)).Returns(_molecule1RefConcDistribution);
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule1.Name, _molecule1.HalfLifeLiver.Name)).Returns(null);
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule1.Name, _molecule1.HalfLifeIntestine.Name)).Returns(null);

         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule2.Name, _molecule2.ReferenceConcentration.Name)).Returns(null);
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule2.Name, _molecule2.HalfLifeLiver.Name)).Returns(null);
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule2.Name, _molecule2.HalfLifeIntestine.Name)).Returns(_molecule2HalfLifeIntestineDistribution);


         _advancedParameterMolecule1RefConc = A.Fake<AdvancedParameter>().WithName("Molecule1RefConf");
         _advancedParameterMolecule1RefConc.ParameterPath = "Path1";

         _advancedParameterMolecule2HalfLifeIntestine = A.Fake<AdvancedParameter>().WithName("Molecule2HalfLifeIntestine");
         _advancedParameterMolecule2HalfLifeIntestine.ParameterPath = "Path2";

         A.CallTo(() => _advancedParmeterFactory.Create(_molecule1.ReferenceConcentration, DistributionTypes.Normal)).Returns(_advancedParameterMolecule1RefConc);
         A.CallTo(() => _advancedParmeterFactory.Create(_molecule2.HalfLifeIntestine, DistributionTypes.Normal)).Returns(_advancedParameterMolecule2HalfLifeIntestine);
      }

      protected override void Because()
      {
         sut.AddVariabilityTo(_randomPopulation);
      }

      [Observation]
      public void should_add_the_user_defined_variability_for_each_known_parameter_of_predefined_molecules()
      {
         _randomPopulation.AdvancedParameters.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_used_the_default_value_of_the_molecule_parameter_as_mean()
      {
         _advancedParameterMolecule1RefConc.DistributedParameter.MeanParameter.Value.ShouldBeEqualTo(_molecule1.ReferenceConcentration.Value);
         _advancedParameterMolecule2HalfLifeIntestine.DistributedParameter.MeanParameter.Value.ShouldBeEqualTo(_molecule2.HalfLifeIntestine.Value);
      }

      [Observation]
      public void should_have_used_the_predefined_deviation_in_the_database_as_deviation()
      {
         _advancedParameterMolecule1RefConc.DistributedParameter.DeviationParameter.Value.ShouldBeEqualTo(_molecule1RefConcDistribution.DeviationParameter.Value);
         _advancedParameterMolecule2HalfLifeIntestine.DistributedParameter.DeviationParameter.Value.ShouldBeEqualTo(_molecule2HalfLifeIntestineDistribution.DeviationParameter.Value);
      }
   }

   public class When_creating_the_variability_for_a_given_molecule_and_population : concern_for_MoleculeParameterVariabilityCreator
   {
      private PKSimMacroCommand _result;
      private IDistributedParameter _molecule1RefConcDistribution;
      private AdvancedParameter _advancedParameterMolecule1RefConc;

      protected override void Context()
      {
         base.Context();
         _molecule1RefConcDistribution = DomainHelperForSpecs.NormalDistributedParameter(defaultMean: 4, defaultDeviation: 2).WithId("Molecule1RefConf");
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule1.Name, _molecule1.ReferenceConcentration.Name)).Returns(_molecule1RefConcDistribution);
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule1.Name, _molecule1.HalfLifeLiver.Name)).Returns(null);
         A.CallTo(() => _moleculeParameterFactory.ParameterFor(_molecule1.Name, _molecule1.HalfLifeIntestine.Name)).Returns(null);

         _advancedParameterMolecule1RefConc = A.Fake<AdvancedParameter>().WithName("Molecule1RefConf");
         _advancedParameterMolecule1RefConc.ParameterPath = "Path1";
         A.CallTo(() => _advancedParmeterFactory.Create(_molecule1.ReferenceConcentration, DistributionTypes.Normal)).Returns(_advancedParameterMolecule1RefConc);
      }

      protected override void Because()
      {
         _result = sut.AddMoleculeVariability(_molecule1, _randomPopulation, true) as PKSimMacroCommand;
      }

      [Observation]
      public void should_return_a_macro_command_containing_a_sub_command_for_each_advanced_parameter_added_to_the_population()
      {
         _result.ShouldNotBeNull();
         _result.All().First().ShouldBeAnInstanceOf<AddAdvancedParameterToContainerCommand>();
      }
   }
}
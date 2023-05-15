using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_MoleculeParameterTask : ContextSpecification<IMoleculeParameterTask>
   {
      protected IMoleculeParameterRepository _moleculeParameterRepository;
      private ITransportContainerUpdater _transportContainerUpdater;
      protected IOntogenyRepository _ontogenyRepository;
      protected IOntogenyTask _ontogenyTask;
      protected IDiseaseStateImplementationRepository _diseaseStateImplementationRepository;

      protected override void Context()
      {
         _transportContainerUpdater = A.Fake<ITransportContainerUpdater>();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _ontogenyTask = A.Fake<IOntogenyTask>();
         _moleculeParameterRepository = A.Fake<IMoleculeParameterRepository>();
         _diseaseStateImplementationRepository = A.Fake<IDiseaseStateImplementationRepository>();

         sut = new MoleculeParameterTask(_moleculeParameterRepository, _transportContainerUpdater, _ontogenyRepository, _ontogenyTask, _diseaseStateImplementationRepository);
      }
   }

   public class When_setting_the_default_parameters_for_a_molecule : concern_for_MoleculeParameterTask
   {
      private readonly string _moleculeName = "MOLECULE";
      protected IndividualEnzyme _molecule;

      protected override void Context()
      {
         base.Context();
         _molecule = new IndividualEnzyme {Name = "CYP3A4"};
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(REFERENCE_CONCENTRATION));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(20).WithName(HALF_LIFE_LIVER));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(30).WithName(HALF_LIFE_INTESTINE));

         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, REFERENCE_CONCENTRATION, A<double?>._, null)).Returns(10);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, HALF_LIFE_LIVER, A<double?>._, null)).Returns(20);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, HALF_LIFE_INTESTINE, A<double?>._, null)).Returns(40);
      }

      protected override void Because()
      {
         sut.SetDefaultMoleculeParameters(_molecule, _moleculeName);
      }

      [Observation]
      public void should_update_the_default_value_for_the_reference_concentration_parameter()
      {
         _molecule.ReferenceConcentration.Value.ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_update_the_default_value_for_the_half_life_parameters()
      {
         _molecule.HalfLifeLiver.Value.ShouldBeEqualTo(20);
         _molecule.HalfLifeIntestine.Value.ShouldBeEqualTo(40);
      }
   }

   public class When_setting_the_default_for_an_expression_profile_for_an_enzyme : concern_for_MoleculeParameterTask
   {
      private ExpressionProfile _expressionProfile;
      private const string _moleculeName = "CYP2D6";
      private Ontogeny _ontogeny;
      private IDiseaseStateImplementation _diseaseStateImplementation;
      protected IndividualMolecule _molecule;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _molecule = _expressionProfile.Molecule;
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(REFERENCE_CONCENTRATION));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(20).WithName(HALF_LIFE_LIVER));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(30).WithName(HALF_LIFE_INTESTINE));

         _ontogeny = new DatabaseOntogeny().WithName(_moleculeName);
         _diseaseStateImplementation = A.Fake<IDiseaseStateImplementation>();
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, REFERENCE_CONCENTRATION, A<double?>._, null)).Returns(10);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, HALF_LIFE_LIVER, A<double?>._, null)).Returns(20);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, HALF_LIFE_INTESTINE, A<double?>._, null)).Returns(40);
         A.CallTo(() => _ontogenyRepository.AllFor(_expressionProfile.Species.Name)).Returns(new[] {_ontogeny});

         A.CallTo(() => _diseaseStateImplementationRepository.FindFor(_expressionProfile.Individual)).Returns(_diseaseStateImplementation);
      }

      protected override void Because()
      {
         sut.SetDefaultFor(_expressionProfile, _moleculeName);
      }

      [Observation]
      public void should_set_the_ontogeny_values()
      {
         _ontogenyTask.SetOntogenyForMolecule(_molecule, _ontogeny, _expressionProfile.Individual);
      }

      [Observation]
      public void should_update_the_default_molecule_parameters()
      {
         _molecule.ReferenceConcentration.Value.ShouldBeEqualTo(10);
         _molecule.HalfLifeLiver.Value.ShouldBeEqualTo(20);
         _molecule.HalfLifeIntestine.Value.ShouldBeEqualTo(40);
      }

      [Observation]
      public void should_apply_the_disease_state_if_one_is_defined()
      {
         A.CallTo(() => _diseaseStateImplementation.ApplyTo(_expressionProfile, _moleculeName)).MustHaveHappened();
      }
   }
}
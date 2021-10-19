using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_MoleculeParameterTask : ContextSpecification<IMoleculeParameterTask>
   {
      protected IMoleculeParameterRepository _moleculeParameterRepository;
      protected IndividualEnzyme _molecule;

      protected override void Context()
      {
         _moleculeParameterRepository = A.Fake<IMoleculeParameterRepository>();
         sut = new MoleculeParameterTask(_moleculeParameterRepository);
         _molecule = new IndividualEnzyme {Name = "CYP3A4"};
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.REFERENCE_CONCENTRATION));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(20).WithName(CoreConstants.Parameters.HALF_LIFE_LIVER));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(30).WithName(CoreConstants.Parameters.HALF_LIFE_INTESTINE));
      }
   }

   public class When_setting_the_default_parameters_for_a_molecule : concern_for_MoleculeParameterTask
   {
      private readonly string _moleculeName = "MOLECULE";

      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, CoreConstants.Parameters.REFERENCE_CONCENTRATION, A<double?>._, null)).Returns(10);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, CoreConstants.Parameters.HALF_LIFE_LIVER, A<double?>._, null)).Returns(20);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor(_moleculeName, CoreConstants.Parameters.HALF_LIFE_INTESTINE, A<double?>._, null)).Returns(40);
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
}
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_6;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_6
{
   public class When_converting_the_Individual_561_project : ContextWithLoadedProject<Converter561To562>
   {
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Individual_561");
         _individual = First<Individual>();
      }

      [Observation]
      public void should_have_added_the_new_half_life_parameter_to_the_enzyme_of_the_individual()
      {
         validateHalfLifeParameterExistenceAndValue<IndividualEnzyme>("ENZ");
      }

      [Observation]
      public void should_have_added_the_new_half_life_parameter_to_the_transporter_of_the_individual()
      {
         validateHalfLifeParameterExistenceAndValue<IndividualTransporter>("TRANS");
      }

      [Observation]
      public void should_have_added_the_new_half_life_parameter_to_the_protein_of_the_individual()
      {
         validateHalfLifeParameterExistenceAndValue<IndividualOtherProtein>("PROT");
      }

      private void validateHalfLifeParameterExistenceAndValue<T>(string moleculeName) where T : IndividualMolecule
      {
         var halfLifeLiver = _individual.MoleculeByName<T>(moleculeName).HalfLifeLiver;
         halfLifeLiver.ShouldNotBeNull();
         halfLifeLiver.Value.ShouldBeEqualTo(CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN);

         var halfLifeIntestine= _individual.MoleculeByName<T>(moleculeName).HalfLifeIntestine;
         halfLifeIntestine.ShouldNotBeNull();
         halfLifeIntestine.Value.ShouldBeEqualTo(CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN);
      }
   }
}
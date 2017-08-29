using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterAlternativeFactory : ContextSpecification<IParameterAlternativeFactory>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected ICloner _cloner;
      protected ISpeciesRepository _speciesRepository;
      protected Compound _compound;
      protected ParameterAlternativeGroup _compoundParameterGroup;
      protected ICoreUserSettings _userSettings;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _cloner = A.Fake<ICloner>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         //necessary to create compound since default parameter willl be added to the group based on compound parameters
         _compound = new Compound();
         _compoundParameterGroup = new ParameterAlternativeGroup();
         _compound.AddParameterAlternativeGroup(_compoundParameterGroup);
         _userSettings = A.Fake<ICoreUserSettings>();
         sut = new ParameterAlternativeFactory(_objectBaseFactory, _cloner, _speciesRepository, _userSettings);
      }
   }

   public class When_retrieving_the_default_alternative_for_a_group_that_should_contain_a_default_calculated_alternative : concern_for_ParameterAlternativeFactory
   {
      private ParameterAlternative _result;

      protected override void Context()
      {
         base.Context();
         var alternative = new ParameterAlternative();
         A.CallTo(() => _objectBaseFactory.Create<ParameterAlternative>()).Returns(alternative);
         _compoundParameterGroup.Name = CoreConstants.Groups.COMPOUND_PERMEABILITY;
      }

      protected override void Because()
      {
         _result = sut.CreateDefaultAlternativeFor(_compoundParameterGroup);
      }

      [Observation]
      public void should_return_an_alternative_with_the_name_set_to_the_calculated()
      {
         _result.Name.ShouldBeEqualTo(PKSimConstants.UI.CalculatedAlernative);
      }

      [Observation]
      public void should_set_the_alternative_as_the_default_alternative()
      {
         _result.IsDefault.ShouldBeTrue();
      }
   }

   public class When_retrieving_the_default_alternative_for_a_standard_group : concern_for_ParameterAlternativeFactory
   {
      private ParameterAlternative _result;

      protected override void Context()
      {
         base.Context();
         var alternative = new ParameterAlternative();
         A.CallTo(() => _userSettings.DefaultLipophilicityName).Returns("MyLipo");
         A.CallTo(() => _objectBaseFactory.Create<ParameterAlternative>()).Returns(alternative);
         _compoundParameterGroup.Name = CoreConstants.Groups.COMPOUND_LIPOPHILICITY;
      }

      protected override void Because()
      {
         _result = sut.CreateDefaultAlternativeFor(_compoundParameterGroup);
      }

      [Observation]
      public void should_return_an_alternative_with_the_name_set_to_the_default_name_defined_in_the_user_settings()
      {
         _result.Name.ShouldBeEqualTo("MyLipo");
      }

      [Observation]
      public void should_set_the_alternative_as_the_default_alternative()
      {
         _result.IsDefault.ShouldBeTrue();
      }
   }

   public class When_creating_an_alternative_for_a_parameter_group_containing_already_a_default_calculated_alternative : concern_for_ParameterAlternativeFactory
   {
      private ParameterAlternative _result;

      protected override void Context()
      {
         base.Context();
         var alternative = new ParameterAlternative();
         alternative.Add(new PKSimParameter().WithName("toto").WithFormula(new ExplicitFormula("2*3")));
         A.CallTo(() => _objectBaseFactory.Create<ParameterAlternative>()).Returns(alternative);
         A.CallTo(() => _objectBaseFactory.Create<ConstantFormula>()).Returns(new ConstantFormula());
         _compoundParameterGroup.Name = CoreConstants.Groups.COMPOUND_PERMEABILITY;
      }

      protected override void Because()
      {
         _result = sut.CreateAlternativeFor(_compoundParameterGroup);
      }

      [Observation]
      public void should_reset_all_the_parameter_of_the_alternative_to_a_constant_value()
      {
         _result.Parameter("toto").Formula.ShouldBeAnInstanceOf<ConstantFormula>();
      }
   }

   public class When_retrieving_the_default_alternative_for_a_group_requiring_a_species_information : concern_for_ParameterAlternativeFactory
   {
      private ParameterAlternative _result;
      private Species _species;

      protected override void Context()
      {
         base.Context();
         _species = new Species();
         var alternative = new ParameterAlternativeWithSpecies();
         A.CallTo(() => _objectBaseFactory.Create<ParameterAlternativeWithSpecies>()).Returns(alternative);
         _compoundParameterGroup.Name = CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND;
         A.CallTo(() => _speciesRepository.DefaultSpecies).Returns(_species);
      }

      protected override void Because()
      {
         _result = sut.CreateAlternativeFor(_compoundParameterGroup);
      }

      [Observation]
      public void should_return_a_parameter_alternative_with_a_default_species_set_to_the_default_species_for_the_current_user()
      {
         _result.ShouldBeAnInstanceOf<ParameterAlternativeWithSpecies>();
         _result.DowncastTo<ParameterAlternativeWithSpecies>().Species.ShouldBeEqualTo(_species);
      }
   }
}
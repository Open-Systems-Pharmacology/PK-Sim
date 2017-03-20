using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundToCompoundPropertiesMapper : ContextSpecification<ICompoundToCompoundPropertiesMapper>
   {
      private ICompoundCalculationMethodCategoryRepository _compoundCalculationMethodCategoryRepository;
      protected CalculationMethodCategory _category2;
      protected CalculationMethodCategory _category1;
      protected CalculationMethod _default1;
      protected CalculationMethod _default2;

      protected override void Context()
      {
         _default1 = new CalculationMethod {Name = "Default1",Category = "C1"};
         _default2 = new CalculationMethod { Name = "Default2", Category = "C2"};
         _category1 = new CalculationMethodCategory {Name = "C1",DefaultItem = _default1};
         _category2 = new CalculationMethodCategory { Name = "C2", DefaultItem = _default2 };
         _compoundCalculationMethodCategoryRepository= A.Fake<ICompoundCalculationMethodCategoryRepository>();
         A.CallTo(() => _compoundCalculationMethodCategoryRepository.All()).Returns(new []{_category1,_category2});
         sut = new CompoundToCompoundPropertiesMapper(_compoundCalculationMethodCategoryRepository);
      }
   }

   public class When_mapping_a_compound_to_compound_properties : concern_for_CompoundToCompoundPropertiesMapper
   {
      private Compound _compound;
      private CompoundProperties _compoundProperties;
      private CalculationMethod _compoundSpecificCalculationMethod;

      protected override void Context()
      {
         base.Context();
         _compound= new Compound();
         _compoundSpecificCalculationMethod = new CalculationMethod {Category = "C1", Name = "Compound Specific"};
         _compound.AddCalculationMethod(_compoundSpecificCalculationMethod);
      }
      protected override void Because()
      {
         _compoundProperties = sut.MapFrom(_compound);
      }

      [Observation]
      public void should_return_a_compound_properties_referencing_the_compound()
      {
         _compoundProperties.Compound.ShouldBeEqualTo(_compound);
      }

      [Observation]
      public void should_add_the_default_calculation_methods_for_molecules()
      {
         _compoundProperties.CalculationMethodFor("C2").ShouldBeEqualTo(_default2);
      }

      [Observation]
      public void should_use_the_compound_specific_calculation_methods_when_available()
      {
         _compoundProperties.CalculationMethodFor("C1").ShouldBeEqualTo(_compoundSpecificCalculationMethod);
      }
   }
}	
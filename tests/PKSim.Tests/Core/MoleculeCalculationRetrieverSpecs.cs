using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using ICoreCalculationMethodRepository = PKSim.Core.Repositories.ICoreCalculationMethodRepository;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeCalculationRetriever : ContextSpecification<IMoleculeCalculationRetriever>
   {
      protected ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      protected ICoreCalculationMethodRepository _coreCalculationMethodRepository;

      protected override void Context()
      {
         _calculationMethodCategoryRepository= A.Fake<ICalculationMethodCategoryRepository>();
         _coreCalculationMethodRepository= A.Fake<ICoreCalculationMethodRepository>();
         sut = new MoleculeCalculationRetriever(_calculationMethodCategoryRepository, _coreCalculationMethodRepository);
      }
   }

   public class When_retrieving_the_molecule_calculation_methods_used_in_a_given_simulation : concern_for_MoleculeCalculationRetriever
   {
      private Simulation _simulation;
      private IEnumerable<ICoreCalculationMethod> _result;
      private List<ICoreCalculationMethod> _allCoreCalculationMethods;
      private ICoreCalculationMethod _coreCM1;
      private ICoreCalculationMethod _coreCM2;
      private ICoreCalculationMethod _coreCM3;
      private ICoreCalculationMethod _coreCM4;
      private CoreCalculationMethod _coreCM5;

      protected override void Context()
      {
         base.Context();
         _allCoreCalculationMethods=new List<ICoreCalculationMethod>();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         var compound1 = new Compound().WithName("C1");
         var compound2 = new Compound().WithName("C2");
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("C1",PKSimBuildingBlockType.Compound){BuildingBlock = compound1});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("C2",PKSimBuildingBlockType.Compound){BuildingBlock = compound2});

         var cm1 = new CalculationMethod { Category = "MOLECULE" }.WithName("CM1");
         var cm2 = new CalculationMethod { Category = "MOLECULE" }.WithName("CM2");
         var cm3 = new CalculationMethod{Category = "MOLECULE" }.WithName("CM3");
         var cm5 = new CalculationMethod{Category = "NOT MOLECULE" }.WithName("CM5");

         A.CallTo(() => _calculationMethodCategoryRepository.FindBy("MOLECULE")).Returns(new CalculationMethodCategory{CategoryType = CategoryType.Molecule});
         A.CallTo(() => _calculationMethodCategoryRepository.FindBy("NOT MOLECULE")).Returns(new CalculationMethodCategory { CategoryType = CategoryType.Individual });

         var cp1 = new CompoundProperties {Compound = compound1};
         var cp2 = new CompoundProperties {Compound = compound2};
         _simulation.Properties.AddCompoundProperties(cp1);
         _simulation.Properties.AddCompoundProperties(cp2);

         cp1.AddCalculationMethod(cm1);
         cp1.AddCalculationMethod(cm2);

         cp2.AddCalculationMethod(cm2);
         cp2.AddCalculationMethod(cm3);
         cp2.AddCalculationMethod(cm5);
         
         _coreCM1=new CoreCalculationMethod{Name = cm1.Name};
         _coreCM2 = new CoreCalculationMethod { Name = cm2.Name };
         _coreCM3 = new CoreCalculationMethod { Name = cm3.Name };
         _coreCM4 = new CoreCalculationMethod { Name = "TOTO" };
         _coreCM5 = new CoreCalculationMethod {Name = cm5.Name};

         _allCoreCalculationMethods.Add(_coreCM1);
         _allCoreCalculationMethods.Add(_coreCM2);
         _allCoreCalculationMethods.Add(_coreCM3);
         _allCoreCalculationMethods.Add(_coreCM4);
         _allCoreCalculationMethods.Add(_coreCM5);
         A.CallTo(() => _coreCalculationMethodRepository.All()).Returns(_allCoreCalculationMethods);

      }

      protected override void Because()
      {
         _result = sut.AllMoleculeCalculationMethodsUsedBy(_simulation);
      }

      [Observation]
      public void should_iterate_over_all_the_used_compounds_and_return_the_calculation_methods_used_in_their_configuration()
      {
         _result.ShouldOnlyContain(_coreCM1, _coreCM2, _coreCM3);   
      }
   }
}	
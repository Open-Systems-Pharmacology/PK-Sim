using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IPKCalculationOptionsFactory = PKSim.Core.Services.IPKCalculationOptionsFactory;
using PKCalculationOptionsFactory = PKSim.Core.Services.PKCalculationOptionsFactory;

namespace PKSim.Core
{
   public abstract class concern_for_PKCalculationOptionsFactory : ContextSpecification<IPKCalculationOptionsFactory>
   {
      private ILazyLoadTask _lazyLoadTask;
      protected Simulation _simulation1;
      protected IContainer _applications;
      protected IContainer _applications2;
      protected IContainer _rootContainer;

      protected override void Context()
      {
         _simulation1 = A.Fake<Simulation>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _rootContainer = new Container();
         _applications = new EventGroup {Name = Constants.APPLICATIONS, ContainerType = ContainerType.EventGroup};
         _applications2 = new EventGroup {Name = "APPLICATIONS_2", ContainerType = ContainerType.EventGroup};
         _simulation1.Model.Root = _rootContainer;
         _rootContainer.Add(_applications);
         A.CallTo(() => _simulation1.BodyWeight).Returns(DomainHelperForSpecs.ConstantParameterWithValue(50));
         sut = new PKCalculationOptionsFactory(_lazyLoadTask);
      }
   }

   public abstract class When_creating_pk_calculation_options_in_multiple_dosing_base_context : concern_for_PKCalculationOptionsFactory
   {
      protected IParameter _startTime1;
      protected IParameter _startTime2;
      protected IParameter _drugMass1;
      protected IParameter _drugMass2;

      protected void CreateMultipleAdministrationForMolecule(string moleculeName)
      {
         var container1 = new Container {Name = $"App1-{moleculeName}", ContainerType = ContainerType.Application};
         var container2 = new Container {Name = $"App2-{moleculeName}", ContainerType = ContainerType.Application};

         _startTime1 = A.Fake<IParameter>().WithName(Constants.Parameters.START_TIME);
         _startTime1.Value = 300;
         _drugMass1 = A.Fake<IParameter>().WithName(Constants.Parameters.DRUG_MASS);
         _drugMass1.Value = 10;

         _startTime2 = A.Fake<IParameter>().WithName(Constants.Parameters.START_TIME);
         _startTime2.Value = 500;
         _drugMass2 = A.Fake<IParameter>().WithName(Constants.Parameters.DRUG_MASS);
         _drugMass2.Value = 20;

         _applications.Add(container1);
         _applications.Add(container2);

         container1.Add(new MoleculeAmount {Name = moleculeName});
         container1.Add(_startTime1);
         container1.Add(_drugMass1);

         container2.Add(new MoleculeAmount {Name = moleculeName});
         container2.Add(_startTime2);
         container2.Add(_drugMass2);
      }

      protected IReactionBuilder CreateReactionForCompoundAndMetabolite(string[] products, params string[] educts)
      {
         var reaction = new ReactionBuilder();
         educts.Each(educt => reaction.AddEduct(new ReactionPartnerBuilder(educt, 1)));

         products.Each(product => reaction.AddProduct(new ReactionPartnerBuilder(product, 1)));
         return reaction;
      }
   }

   public class When_creating_the_pk_analyses_options_for_a_population_simulation : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private PopulationSimulation _populationSimulation;
      private string _appliedCompound;
      private PKCalculationOptions _result;

      protected override void Context()
      {
         base.Context();
         _appliedCompound = "compound";
         _populationSimulation = A.Fake<PopulationSimulation>();
         _populationSimulation.Model.Root = _rootContainer;

         CreateMultipleAdministrationForMolecule(_appliedCompound);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_populationSimulation, _appliedCompound);
      }

      [Observation]
      public void should_return_options_with_a_dose_set_to_null_as_it_needs_to_be_calculated_for_all_individual_separately()
      {
         _result.DrugMassPerBodyWeight.ShouldBeNull();
      }
   }

   public class when_creating_analysis_options_for_multiple_dosing_and_reactions_have_multiple_educts : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private string _metabolite1;
      private string _metabolite2;
      private string _secondMetabolite;
      private string _appliedCompound;
      private ReactionBuildingBlock _reactionBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _metabolite1 = "metabolite1";
         _metabolite2 = "metabolite2";
         _secondMetabolite = "secondMetabolite";
         _appliedCompound = "compound";
         _reactionBuildingBlock = new ReactionBuildingBlock();
         A.CallTo(() => _simulation1.Reactions).Returns(_reactionBuildingBlock);

         _reactionBuildingBlock.Add(CreateReactionForCompoundAndMetabolite(new[] {_metabolite1}, _appliedCompound));
         _reactionBuildingBlock.Add(CreateReactionForCompoundAndMetabolite(new[] {_secondMetabolite}, _metabolite1, _metabolite2));

         CreateMultipleAdministrationForMolecule(_appliedCompound);

         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      [Observation]
      public void pk_analysis_for_first_metabolite_should_be_multiple_dosing()
      {
         var pkOptions = sut.CreateFor(_simulation1, _metabolite1);
         pkOptions.PKParameterMode.ShouldBeEqualTo(PKParameterMode.Multi);
      }

      [Observation]
      public void pk_analysis_for_second_metabolite_should_be_single_dosing()
      {
         sut.CreateFor(_simulation1, _secondMetabolite).PKParameterMode.ShouldBeEqualTo(PKParameterMode.Single);
      }
   }

   public class When_creating_pk_analyses_for_observed_data_and_simulations_for_which_only_one_dose_is_available : concern_for_PKCalculationOptionsFactory
   {
      private double? _dose;
      private string _moleculeName;

      protected override void Context()
      {
         base.Context();
         _moleculeName = "DRUG";
         _dose = 10d;
         _simulation1 = A.Fake<Simulation>();
         A.CallTo(() => _simulation1.TotalDrugMassPerBodyWeightFor(_moleculeName)).Returns(_dose);
      }

      [Observation]
      public void should_return_option_with_the_dose_set()
      {
         sut.CreateForObservedData(new[] {_simulation1}, _moleculeName).DrugMassPerBodyWeight.ShouldBeEqualTo(_dose);
      }
   }

   public class When_creating_pk_analyses_for_observed_data_and_simulations_for_which_multiple_doses_are_available : concern_for_PKCalculationOptionsFactory
   {
      private string _moleculeName;
      private Simulation _simulation2;

      protected override void Context()
      {
         base.Context();
         _moleculeName = "DRUG";
         _simulation1 = A.Fake<Simulation>();
         A.CallTo(() => _simulation1.TotalDrugMassPerBodyWeightFor(_moleculeName)).Returns(10);

         _simulation2 = A.Fake<Simulation>();
         A.CallTo(() => _simulation1.TotalDrugMassPerBodyWeightFor(_moleculeName)).Returns(20);
      }

      [Observation]
      public void should_return_option_with_the_dose_null()
      {
         sut.CreateForObservedData(new[] {_simulation1, _simulation2,}, _moleculeName).DrugMassPerBodyWeight.ShouldBeNull();
      }
   }

   public class When_creating_analysis_for_a_simulation_that_has_no_reactions : concern_for_PKCalculationOptionsFactory
   {
      protected override void Because()
      {
         A.CallTo(() => _simulation1.Reactions).Returns(null);
      }

      [Observation]
      public void should_return_a_pk_analysis_single_mode()
      {
         sut.CreateFor(_simulation1, "Drug").PKParameterMode.ShouldBeEqualTo(PKParameterMode.Single);
      }
   }

   public class when_creating_analysis_options_for_multiple_dosing_and_first_reaction_has_multiple_products : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private string _metabolite1;
      private string _secondMetabolite;
      private string _appliedCompound;
      private ReactionBuildingBlock _reactionBuildingBlock;
      private string _metabolite2;

      protected override void Context()
      {
         base.Context();
         _metabolite1 = "metabolite1";
         _secondMetabolite = "secondMetabolite";
         _metabolite2 = "metabolite2";
         _appliedCompound = "compound";
         _reactionBuildingBlock = new ReactionBuildingBlock();
         A.CallTo(() => _simulation1.Reactions).Returns(_reactionBuildingBlock);

         _reactionBuildingBlock.Add(CreateReactionForCompoundAndMetabolite(new[] {_metabolite1, _metabolite2}, _appliedCompound));
         _reactionBuildingBlock.Add(CreateReactionForCompoundAndMetabolite(new[] {_secondMetabolite}, _metabolite1));

         CreateMultipleAdministrationForMolecule(_appliedCompound);

         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      [Observation]
      public void pk_analysis_for_first_metabolite_should_be_multiple_dosing()
      {
         sut.CreateFor(_simulation1, _metabolite1).PKParameterMode.ShouldBeEqualTo(PKParameterMode.Single);
      }

      [Observation]
      public void pk_analysis_for_second_metabolite_should_be_single_dosing()
      {
         sut.CreateFor(_simulation1, _secondMetabolite).PKParameterMode.ShouldBeEqualTo(PKParameterMode.Single);
      }
   }

   public class when_creating_analysis_options_for_multiple_dosing_and_two_reactions_with_different_educts_have_the_same_product : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private ReactionBuildingBlock _reactionBuildingBlock;
      private string _educt2;
      private string _educt1;
      private string _product;

      protected override void Context()
      {
         base.Context();
         _educt1 = "educt1";
         _educt2 = "educt2";
         _product = "product";
         _reactionBuildingBlock = new ReactionBuildingBlock
         {
            CreateReactionForCompoundAndMetabolite(new[] {_product}, _educt1),
            CreateReactionForCompoundAndMetabolite(new[] {_product}, _educt2)
         };

         A.CallTo(() => _simulation1.Reactions).Returns(_reactionBuildingBlock);
         CreateMultipleAdministrationForMolecule(_educt1);
         CreateMultipleAdministrationForMolecule(_educt2);
         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      [Observation]
      public void multiple_adiministration_times_should_not_be_used_to_calculate_pk_parameter_options()
      {
         sut.CreateFor(_simulation1, _product).PKParameterMode.ShouldBeEqualTo(PKParameterMode.Single);
      }
   }

   public class when_creating_analysis_options_for_multiple_dosing_and_last_reaction_has_multiple_products : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private string _metabolite1;
      private string _secondMetabolite;
      private string _appliedCompound;
      private ReactionBuildingBlock _reactionBuildingBlock;
      private string _secondMetabolite2;

      protected override void Context()
      {
         base.Context();
         _metabolite1 = "metabolite1";
         _secondMetabolite = "secondMetabolite";
         _secondMetabolite2 = "secondMetabolite2";
         _appliedCompound = "compound";
         _reactionBuildingBlock = new ReactionBuildingBlock();
         A.CallTo(() => _simulation1.Reactions).Returns(_reactionBuildingBlock);

         _reactionBuildingBlock.Add(CreateReactionForCompoundAndMetabolite(new[] {_metabolite1}, _appliedCompound));
         _reactionBuildingBlock.Add(CreateReactionForCompoundAndMetabolite(new[] {_secondMetabolite, _secondMetabolite2}, _metabolite1));

         CreateMultipleAdministrationForMolecule(_appliedCompound);

         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      [Observation]
      public void pk_analysis_for_first_metabolite_should_be_multiple_dosing()
      {
         sut.CreateFor(_simulation1, _metabolite1).PKParameterMode.ShouldBeEqualTo(PKParameterMode.Multi);
      }

      [Observation]
      public void pk_analysis_for_second_metabolite_should_be_single_dosing()
      {
         sut.CreateFor(_simulation1, _secondMetabolite).PKParameterMode.ShouldBeEqualTo(PKParameterMode.Single);
      }
   }

   public class when_creating_the_analysis_options_for_a_metabolite_in_a_multiple_dose_protocol : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private string _metaboliteName;
      private string _compoundName;
      private IReactionBuildingBlock _reactionBuildingBlock;
      private string _secondMetaboliteName;
      private PKCalculationOptions _result;

      protected override void Context()
      {
         base.Context();
         _metaboliteName = "metabolite";
         _compoundName = "compound";
         _secondMetaboliteName = "secondMetabolite";
         _reactionBuildingBlock = new ReactionBuildingBlock();
         A.CallTo(() => _simulation1.Reactions).Returns(_reactionBuildingBlock);

         var reaction = CreateReactionForCompoundAndMetabolite(new[] {_metaboliteName}, _compoundName);
         _simulation1.Reactions.Add(reaction);

         reaction = CreateReactionForCompoundAndMetabolite(new[] {_secondMetaboliteName}, _metaboliteName);
         _simulation1.Reactions.Add(reaction);

         CreateMultipleAdministrationForMolecule(_compoundName);
         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_simulation1, _secondMetaboliteName);
      }

      [Observation]
      public void dosing_mode_detection_for_options_should_be_recursive_until_applied_compound_is_found()
      {
         _result.PKParameterMode.ShouldBeEqualTo(PKParameterMode.Multi);
      }
   }

   public class When_creating_the_pk_analyses_options_for_a_single_dosing_simulation : concern_for_PKCalculationOptionsFactory
   {
      private PKCalculationOptions _options;
      private IParameter _startTime;
      private string _moleculeName;

      protected override void Context()
      {
         base.Context();
         _moleculeName = "TOTO";
         var container1 = new Container {Name = "App1", ContainerType = ContainerType.Application};
         _startTime = A.Fake<IParameter>().WithName(Constants.Parameters.START_TIME);
         _startTime.Value = 300;

         _applications.Add(container1);
         container1.Add(new MoleculeAmount {Name = _moleculeName});
         container1.Add(_startTime);

         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      protected override void Because()
      {
         _options = sut.CreateFor(_simulation1, _moleculeName);
      }

      [Observation]
      public void should_return_options_containing_dosing_intervals_for_the_first_and_only_application()
      {
         _options.SingleDosing.ShouldBeTrue();
         _options.FirstInterval.DrugMassPerBodyWeight.ShouldBeEqualTo(_startTime.Value.ToFloat());
      }
   }

   public class When_creating_the_pk_analyses_options_for_a_multiple_dosing_simulation : When_creating_pk_calculation_options_in_multiple_dosing_base_context
   {
      private PKCalculationOptions _options;
      private string _moleculeName;

      protected override void Context()
      {
         base.Context();
         _moleculeName = "TOTO";
         CreateMultipleAdministrationForMolecule(_moleculeName);

         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      protected override void Because()
      {
         _options = sut.CreateFor(_simulation1, _moleculeName);
      }

      [Observation]
      public void should_return_options_containing_dosing_intervals_for_the_multiple_applications()
      {
         _options.SingleDosing.ShouldBeFalse();
         _options.FirstInterval.StartValue.ShouldBeEqualTo(_startTime1.Value.ToFloat());
         _options.LastInterval.StartValue.ShouldBeEqualTo(_startTime2.Value.ToFloat());
         _options.LastMinusOneInterval.StartValue.ShouldBeEqualTo(_startTime1.Value.ToFloat());
      }


      [Observation]
      public void should_return_the_expected_dose_for_all_intervals()
      {
         _options.FirstInterval.DrugMassPerBodyWeight.ShouldBeEqualTo(_drugMass1.Value/_simulation1.BodyWeight.Value);
         _options.LastMinusOneInterval.DrugMassPerBodyWeight.ShouldBeEqualTo(_drugMass1.Value / _simulation1.BodyWeight.Value);
         _options.LastInterval.DrugMassPerBodyWeight.ShouldBeEqualTo(_drugMass2.Value / _simulation1.BodyWeight.Value);
      }
   }

   public class When_creating_the_pk_analyses_options_for_a_multiple_dosing_simulation_containing_multiple_molecules : concern_for_PKCalculationOptionsFactory
   {
      private IParameter _startTime1;
      private IParameter _startTime2;
      private string _multipleAppMolecule;
      private string _singleAppMolecule;

      protected override void Context()
      {
         base.Context();
         _multipleAppMolecule = "MULTI";
         _singleAppMolecule = "SINGLE";
         var container1 = new Container {Name = "App1", ContainerType = ContainerType.Application};
         var container2 = new Container {Name = "App2", ContainerType = ContainerType.Application};
         var container3 = new Container {Name = "App3", ContainerType = ContainerType.Application};

         _startTime1 = A.Fake<IParameter>().WithName(Constants.Parameters.START_TIME);
         _startTime1.Value = 300;

         _startTime2 = A.Fake<IParameter>().WithName(Constants.Parameters.START_TIME);
         _startTime2.Value = 500;

         _applications.Add(container1);
         _applications.Add(container2);

         _rootContainer.Add(_applications2);
         _applications2.Add(container3);

         container1.Add(new MoleculeAmount {Name = _multipleAppMolecule});
         container1.Add(_startTime1);

         container2.Add(new MoleculeAmount {Name = _multipleAppMolecule});
         container2.Add(_startTime2);

         container3.Add(new MoleculeAmount {Name = _singleAppMolecule});
         container3.Add(_startTime1);

         A.CallTo(() => _simulation1.EndTime).Returns(1000);
      }

      [Observation]
      public void should_return_options_containing_dosing_intervals_for_the_multiple_applications_defined_in_event_group_containers()
      {
         var options = sut.CreateFor(_simulation1, _multipleAppMolecule);
         options.SingleDosing.ShouldBeFalse();
         options.FirstInterval.StartValue.ShouldBeEqualTo(_startTime1.Value.ToFloat());
         options.LastInterval.StartValue.ShouldBeEqualTo(_startTime2.Value.ToFloat());
         options.LastMinusOneInterval.StartValue.ShouldBeEqualTo(_startTime1.Value.ToFloat());
      }

      [Observation]
      public void should_return_options_containing_dosing_intervals_for_the_single_application()
      {
         var options = sut.CreateFor(_simulation1, _singleAppMolecule);
         options.SingleDosing.ShouldBeTrue();
         options.FirstInterval.StartValue.ShouldBeEqualTo(_startTime1.Value.ToFloat());
      }
   }

   public class When_creating_the_pk_analyses_for_a_simulation_that_does_not_have_an_application : concern_for_PKCalculationOptionsFactory
   {
      private PKCalculationOptions _options;
      private string _moleculeName;

      protected override void Context()
      {
         base.Context();
         _moleculeName = "TOTO";
         _simulation1 = A.Fake<Simulation>();
      }

      protected override void Because()
      {
         _options = sut.CreateFor(_simulation1, _moleculeName);
      }

      [Observation]
      public void should_return_a_valid_option()
      {
         _options.SingleDosing.ShouldBeTrue();
      }
   }
}
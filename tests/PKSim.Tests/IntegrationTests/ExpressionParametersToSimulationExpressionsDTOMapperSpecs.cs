using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.IntegrationTests
{
   public abstract class
      concern_for_ExpressionParametersToSimulationExpressionsDTOMapper : ContextForIntegration<IExpressionParametersToSimulationExpressionsDTOMapper>
   {
      private IWithIdRepository _withIdRepository;
      protected Simulation _simulation;
      protected IndividualMolecule _transporter;
      protected IndividualEnzyme _enzyme;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _withIdRepository = IoC.Resolve<IWithIdRepository>();
         _simulation = new IndividualSimulation().WithId("Sim");
         _simulation.IsLoaded = true;
         _withIdRepository.Register(_simulation);

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Ind", PKSimBuildingBlockType.Individual)
            {BuildingBlock = DomainHelperForSpecs.CreateIndividual()});
         _transporter = new IndividualTransporter().WithName("Trans");
         _enzyme = new IndividualEnzyme().WithName("Enz");

         _simulation.Individual.AddMolecule(_transporter);
         _simulation.Individual.AddMolecule(_enzyme);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _withIdRepository.Unregister(_simulation.Id);
      }
   }

   public class When_mapping_a_list_of_parameters_to_a_simulation_expression_dto : concern_for_ExpressionParametersToSimulationExpressionsDTOMapper
   {
      private List<IParameter> _parameters;
      private SimulationExpressionsDTO _result;
      private IParameter _referenceConcentrationParam;
      private IParameter _halfLifeLiverParameter;
      private IParameter _halfLifeLiverIntestineParameter;

      protected override void Context()
      {
         base.Context();
         _parameters = new List<IParameter>();
         var brain = new Organ().WithName(CoreConstants.Organ.Brain);
         var brain_pls = new Compartment().WithName(CoreConstants.Compartment.Plasma).WithParentContainer(brain);
         var brain_pls_trans = new Container().WithName(_transporter.Name).WithParentContainer(brain_pls);
         var relExp2Param = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REL_EXP)
            .WithParentContainer(brain_pls_trans);
         relExp2Param.Origin.SimulationId = "Sim";

         var liver = new Organ().WithName(CoreConstants.Organ.Liver);
         var liver_cell = new Container().WithName(CoreConstants.Compartment.Intracellular).WithParentContainer(liver);
         var liver_enz = new Container().WithName(_enzyme.Name).WithParentContainer(liver_cell);
         var relExp1Param = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REL_EXP)
            .WithParentContainer(liver_enz);
         relExp1Param.Origin.SimulationId = "Sim";

         _referenceConcentrationParam = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REFERENCE_CONCENTRATION);
         _halfLifeLiverParameter = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.HALF_LIFE_LIVER);
         _halfLifeLiverIntestineParameter = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.HALF_LIFE_INTESTINE);

         _parameters.AddRange(new[]
            {_halfLifeLiverParameter, _halfLifeLiverIntestineParameter, _referenceConcentrationParam, relExp1Param, relExp2Param});
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_parameters);
      }

      [Observation]
      public void should_retrieve_the_reference_concentration()
      {
         _result.ReferenceConcentration.Parameter.ShouldBeEqualTo(_referenceConcentrationParam);
      }

      [Observation]
      public void should_retrieve_the_half_life()
      {
         _result.HalfLifeLiver.Parameter.ShouldBeEqualTo(_halfLifeLiverParameter);
         _result.HalfLifeIntestine.Parameter.ShouldBeEqualTo(_halfLifeLiverIntestineParameter);
      }

      [Observation]
      public void should_map_one_parameter_for_each_relative_expression_parameter_defined_in_the_list()
      {
         _result.ExpressionParameters.Count.ShouldBeEqualTo(2);
      }
   }

   public class
      When_mapping_a_list_of_parameters_to_a_simulation_expression_dto_that_was_imported_from_pkml_and_does_not_have_an_individual :
         concern_for_ExpressionParametersToSimulationExpressionsDTOMapper
   {
      private List<IParameter> _parameters;
      private SimulationExpressionsDTO _result;
      private IParameter _referenceConcentrationParam;
      private IParameter _halfLifeLiverParameter;
      private IParameter _halfLifeLiverIntestineParameter;

      protected override void Context()
      {
         base.Context();
         _simulation.RemoveAllBuildingBlockOfType(PKSimBuildingBlockType.Individual);
         _parameters = new List<IParameter>();
         var kidney = new Organ().WithName(CoreConstants.Organ.Kidney);
         var kidney_cell = new Compartment().WithName(CoreConstants.Compartment.Intracellular).WithParentContainer(kidney);
         var kid_cell_trans = new Container().WithName(_transporter.Name).WithParentContainer(kidney_cell);
         var relExp2Param = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REL_EXP)
            .WithParentContainer(kid_cell_trans);
         relExp2Param.Origin.SimulationId = "Sim";

         var liver = new Organ().WithName(CoreConstants.Organ.Liver);
         var liver_cell = new Container().WithName(CoreConstants.Compartment.Intracellular).WithParentContainer(liver);
         var liver_enz = new Container().WithName(_enzyme.Name).WithParentContainer(liver_cell);
         var relExp1Param = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REL_EXP)
            .WithParentContainer(liver_enz);
         relExp1Param.Origin.SimulationId = "Sim";

         _referenceConcentrationParam = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REFERENCE_CONCENTRATION);
         _halfLifeLiverParameter = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.HALF_LIFE_LIVER);
         _halfLifeLiverIntestineParameter = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.HALF_LIFE_INTESTINE);

         _parameters.AddRange(new[]
            {_halfLifeLiverParameter, _halfLifeLiverIntestineParameter, _referenceConcentrationParam, relExp1Param, relExp2Param});
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_parameters);
      }

      [Observation]
      public void should_retrieve_the_reference_concentration()
      {
         _result.ReferenceConcentration.Parameter.ShouldBeEqualTo(_referenceConcentrationParam);
      }

      [Observation]
      public void should_retrieve_the_half_life()
      {
         _result.HalfLifeLiver.Parameter.ShouldBeEqualTo(_halfLifeLiverParameter);
         _result.HalfLifeIntestine.Parameter.ShouldBeEqualTo(_halfLifeLiverIntestineParameter);
      }

      [Observation]
      public void should_map_one_parameter_for_each_relative_expression_parameter_defined_in_the_list()
      {
         _result.ExpressionParameters.Count().ShouldBeEqualTo(2);
      }
   }
}
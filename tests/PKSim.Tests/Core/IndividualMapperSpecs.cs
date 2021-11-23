using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ExpressionProfile = PKSim.Core.Snapshots.ExpressionProfile;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;
using OriginData = PKSim.Core.Snapshots.OriginData;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualMapper : ContextSpecificationAsync<IndividualMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected ModelIndividual _individual;
      protected SnapshotIndividual _snapshot;
      protected IParameter _parameterLiver;
      protected IParameter _parameterKidney;
      protected string _parameterKidneyPath = "ParameterKidneyPath";
      protected IDimensionRepository _dimensionRepository;
      protected ExpressionProfileMapper _expressionProfileMapper;
      protected LocalizedParameter _localizedParameterKidney;
      protected IIndividualFactory _individualFactory;
      protected OriginDataMapper _originDataMapper;
      protected OriginData _originDataSnapshot;
      protected IMoleculeExpressionTask<ModelIndividual> _moleculeExpressionTask;
      public Model.ExpressionProfile _expressionProfile1;
      public Model.ExpressionProfile _expressionProfile2;
      protected IParameter _parameterKidneyRelExp;
      protected List<IParameter> _mappedParameters;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _expressionProfileMapper = A.Fake<ExpressionProfileMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _originDataMapper = A.Fake<OriginDataMapper>();
         _moleculeExpressionTask= A.Fake<IMoleculeExpressionTask<ModelIndividual>>();  

         sut = new IndividualMapper(_parameterMapper, _expressionProfileMapper, _originDataMapper, _individualFactory,_moleculeExpressionTask);

         _individual = DomainHelperForSpecs.CreateIndividual();
         _individual.Name = "Ind";
         _individual.Description = "Model Description";
         var kidney = _individual.EntityAt<IContainer>(Constants.ORGANISM, CoreConstants.Organ.KIDNEY);
         _parameterLiver = _individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.LIVER, "PLiver");
         _parameterKidney = _individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.KIDNEY, "PKidney");
         _parameterKidneyRelExp = DomainHelperForSpecs.ConstantParameterWithValue().WithName(CoreConstants.Parameters.REL_EXP);
         _parameterKidneyRelExp.DefaultValue = 10;
         kidney.Add(_parameterKidneyRelExp);

         _parameterLiver.ValueDiffersFromDefault().ShouldBeFalse();
         _parameterKidney.ValueDiffersFromDefault().ShouldBeFalse();

         _parameterKidney.Value = 40;
         _parameterKidney.ValueDiffersFromDefault().ShouldBeTrue();
         _parameterKidneyRelExp.Value = 50;
         _parameterKidneyRelExp.ValueDiffersFromDefault().ShouldBeTrue();

         _expressionProfile1 = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(moleculeName: "Enz");
         _expressionProfile2 = DomainHelperForSpecs.CreateExpressionProfile<IndividualTransporter>(moleculeName: "Trans");
         _individual.AddExpressionProfile(_expressionProfile1);
         _individual.AddExpressionProfile(_expressionProfile2);
         

         _originDataSnapshot = new OriginData();
         A.CallTo(() => _originDataMapper.MapToSnapshot(_individual.OriginData)).Returns(_originDataSnapshot);

         _localizedParameterKidney = new LocalizedParameter {Path = "Organism|Kidney|PKidney"};
         A.CallTo(() => _parameterMapper.LocalizedParametersFrom(A<IEnumerable<IParameter>>._))
            .Invokes(x=> _mappedParameters = x.GetArgument<IEnumerable<IParameter>>(0).ToList())
            .Returns(new[] {_localizedParameterKidney});

         return _completed;
      }
   }

   public class When_mapping_an_individual_to_snapshot : concern_for_IndividualMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_individual);
      }

      [Observation]
      public void should_save_the_individual_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_individual.Name);
         _snapshot.Description.ShouldBeEqualTo(_individual.Description);
      }

      [Observation]
      public void should_only_saved_parameter_that_have_changed_and_that_are_not_relative_expression_parameters()
      {
         _mappedParameters.ShouldContain(_parameterKidney);
         _mappedParameters.ShouldNotContain(_parameterKidneyRelExp);
      }

      [Observation]
      public void should_save_the_origin_data_properties()
      {
         _snapshot.OriginData.ShouldBeEqualTo(_originDataSnapshot);
      }

      [Observation]
      public void should_save_all_individual_parameters_that_have_been_changed_by_the_user()
      {
         _snapshot.Parameters.ShouldOnlyContain(_localizedParameterKidney);
      }

      [Observation]
      public void should_save_the_individual_molecules()
      {
         _snapshot.ExpressionProfiles.ShouldOnlyContain(_expressionProfile1.Name, _expressionProfile2.Name);
      }
   }

   public class When_mapping_a_valid_individual_snapshot_to_an_individual : concern_for_IndividualMapper
   {
      private ModelIndividual _newIndividual;
      private Model.OriginData _newOriginData;
      private PKSimProject _project;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_individual);
         _project = new PKSimProject();


         _snapshot.Name = "New individual";
         _snapshot.Description = "The description that will be deserialized";

         //clear enzyme before mapping them again
         _individual.RemoveExpressionProfile(_expressionProfile1);
         _individual.RemoveExpressionProfile(_expressionProfile2);

         //reset parameter
         _parameterKidney.ResetToDefault();

         _project.AddBuildingBlock(_expressionProfile1);
         _project.AddBuildingBlock(_expressionProfile2);

         _newOriginData = new Model.OriginData();
         A.CallTo(() => _originDataMapper.MapToModel(_snapshot.OriginData)).Returns(_newOriginData);

         A.CallTo(() => _individualFactory.CreateAndOptimizeFor(_newOriginData, _snapshot.Seed))
            .Returns(_individual);

      }

      protected override async Task Because()
      {
         _newIndividual = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_use_the_expected_individual_origin_data_to_create_the_individual()
      {
         _newIndividual.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_an_event_with_the_expected_properties()
      {
         _newIndividual.Name.ShouldBeEqualTo(_snapshot.Name);
         _newIndividual.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_created_the_expected_molecules()
      {
         A.CallTo(() => _moleculeExpressionTask.AddExpressionProfile(_newIndividual, _expressionProfile1)).MustHaveHappened();
         A.CallTo(() => _moleculeExpressionTask.AddExpressionProfile(_newIndividual, _expressionProfile2)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_parameter_previously_set_by_the_user()
      {
         A.CallTo(() => _parameterMapper.MapLocalizedParameters(_snapshot.Parameters, _individual, true)).MustHaveHappened();
      }
   }
}
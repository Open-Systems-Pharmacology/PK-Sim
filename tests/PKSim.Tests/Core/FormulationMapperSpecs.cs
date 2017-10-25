using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_FormulationMapper : ContextSpecificationAsync<FormulationMapper>
   {
      protected Formulation _formulation;
      protected Snapshots.Formulation _snapshot;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IParameter _hiddenParameter;
      protected ParameterMapper _parameterMapper;
      protected IFormulationRepository _formulationRepository;
      protected ICloner _cloner;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _formulationRepository = A.Fake<IFormulationRepository>();
         _cloner = A.Fake<ICloner>();

         sut = new FormulationMapper(_parameterMapper, _formulationRepository, _cloner);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param1");

         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param2");

         _hiddenParameter = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param3");
         _hiddenParameter.Visible = false;

         _formulation = new Formulation
         {
            Name = "Form",
            FormulationType = "Weibul",
            Description = "Amazing formulation"
         };

         _formulation.Add(_parameter1);
         _formulation.Add(_parameter2);
         _formulation.Add(_hiddenParameter);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).Returns(new Snapshots.Parameter().WithName(_parameter1.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter2)).Returns(new Snapshots.Parameter().WithName(_parameter2.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_hiddenParameter)).Returns(new Snapshots.Parameter().WithName(_hiddenParameter.Name));

         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_model_formulation_to_a_snapshot_formulation : concern_for_FormulationMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_formulation);
      }

      [Observation]
      public void should_save_the_formulation_properties()
      {
         _snapshot.FormulationType.ShouldBeEqualTo(_formulation.FormulationType);
         _snapshot.Name.ShouldBeEqualTo(_formulation.Name);
         _snapshot.Description.ShouldBeEqualTo(_formulation.Description);
      }

      [Observation]
      public void should_save_the_visible_formulation_parameters_only()
      {
         _snapshot.Parameters.Length.ShouldBeEqualTo(_formulation.AllVisibleParameters().Count());
         _snapshot.Parameters.ExistsByName(_parameter1.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_parameter2.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_hiddenParameter.Name).ShouldBeFalse();
      }
   }

   public class When_mapping_a_valid_formulation_snapshot_to_a_formulation : concern_for_FormulationMapper
   {
      private Formulation _newFormulation;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_formulation);
         A.CallTo(() => _formulationRepository.FormulationBy(_snapshot.FormulationType)).Returns(_formulation);
         A.CallTo(() => _cloner.Clone(_formulation)).Returns(_formulation);

         _snapshot.Name = "New Formulation";
         _snapshot.Description = "The description that will be deserialized";
      }

      protected override async Task Because()
      {
         _newFormulation = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_have_created_a_formulation_with_the_expected_properties()
      {
         _newFormulation.Name.ShouldBeEqualTo(_snapshot.Name);
         _newFormulation.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_updated_all_visible_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newFormulation, _newFormulation.Name)).MustHaveHappened();
      }
   }
}
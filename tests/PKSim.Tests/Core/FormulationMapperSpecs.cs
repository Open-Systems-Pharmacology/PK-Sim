using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_FormulationMapper : ContextSpecification<FormulationMapper>
   {
      protected Formulation _formulation;
      protected Snapshots.Formulation _snapshot;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IParameter _hiddenParameter;
      private ParameterMapper _parameterMapper;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         sut = new FormulationMapper(_parameterMapper);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param1");

         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param2");

         _hiddenParameter = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param3");
         _hiddenParameter.Visible = false;

         _formulation = new Formulation
         {
            FormulationType = "Weibul",
            Description = "Amazing formulation"
         };

         _formulation.Add(_parameter1);
         _formulation.Add(_parameter2);
         _formulation.Add(_hiddenParameter);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).Returns(new Parameter().WithName(_parameter1.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter2)).Returns(new Parameter().WithName(_parameter2.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_hiddenParameter)).Returns(new Parameter().WithName(_hiddenParameter.Name));
      }
   }

   public class When_mapping_a_model_formulationt_to_a_snapshot_formulation : concern_for_FormulationMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_formulation);
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
         _snapshot.Parameters.Count.ShouldBeEqualTo(_formulation.AllVisibleParameters().Count());
         _snapshot.Parameters.ExistsByName(_parameter1.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_parameter2.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_hiddenParameter.Name).ShouldBeFalse();
      }
   }
}
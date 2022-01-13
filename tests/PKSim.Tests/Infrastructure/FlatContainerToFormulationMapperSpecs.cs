using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_FlatContainerToFormulationMapper : ContextSpecification<IFlatContainerToFormulationMapper>
   {
      protected IParameterContainerTask _parameterContainerTask;
      protected IFlatContainerIdToFormulationMapper _containerIdToFormulationMapper;

      protected override void Context()
      {
         _parameterContainerTask = A.Fake<IParameterContainerTask>();
         _containerIdToFormulationMapper = A.Fake<IFlatContainerIdToFormulationMapper>();

         sut = new FlatContainerToFormulationMapper(_parameterContainerTask, _containerIdToFormulationMapper);
      }
   }

   
   public class When_mapping_a_flat_container_to_a_formulation_container : concern_for_FlatContainerToFormulationMapper
   {
      private FlatContainer _formulationFlatContainer;
      private  PKSim.Core.Model.Formulation _formulation;

      protected override void Context()
      {
         base.Context();
         _formulation = A.Fake< PKSim.Core.Model.Formulation>();
         _formulationFlatContainer = new FlatContainer();
         _formulationFlatContainer.Type = CoreConstants.ContainerType.FORMULATION;
         _formulationFlatContainer.Name = "tralal";
         A.CallTo(() => _containerIdToFormulationMapper.MapFrom(_formulationFlatContainer)).Returns(_formulation);

      }
      protected override void Because()
      {
         _formulation = sut.MapFrom(_formulationFlatContainer);
      }


      [Observation]
      public void should_have_added_the_parameter_to_the_formulation()
      {
         A.CallTo(() => _parameterContainerTask.AddFormulationParametersTo(_formulation)).MustHaveHappened();
      }

      [Observation]
      public void should_have_marked_the_formulation_as_loaded()
      {
         _formulation.IsLoaded.ShouldBeTrue();
      }
   }
}	
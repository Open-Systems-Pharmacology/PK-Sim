using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_ObjectBaseDTOFactory : ContextSpecification<IObjectBaseDTOFactory>
   {
      private ICoreWorkspace _workspace;
      private IForbiddenCompoundNamesRetriever _forbiddenCompoundNamesRetriever;
      protected PKSimProject _project;

      protected override void Context()
      {
         _workspace = A.Fake<ICoreWorkspace>();
         _forbiddenCompoundNamesRetriever = A.Fake<IForbiddenCompoundNamesRetriever>();
         _project = new PKSimProject();
         A.CallTo(() => _workspace.Project).Returns(_project);
         A.CallTo(() => _forbiddenCompoundNamesRetriever.All()).Returns(new[] {"Volume"});

         _project.AddBuildingBlock(new Compound().WithName("Caffeine"));
         _project.AddBuildingBlock(new Individual().WithName("Ind1"));

         sut = new ObjectBaseDTOFactory(_workspace, _forbiddenCompoundNamesRetriever);
      }
   }

   public class When_creating_a_dto_for_a_new_compound : concern_for_ObjectBaseDTOFactory
   {
      private ObjectBaseDTO _dto;

      protected override void Because()
      {
         _dto = sut.CreateFor<Compound>();
      }

      [Observation]
      public void should_return_a_compound_dto()
      {
         _dto.ShouldBeAnInstanceOf<CompoundDTO>();
      }

      [Observation]
      public void should_use_the_project_as_container_type()
      {
         _dto.ContainerType.ShouldBeEqualTo(PKSimConstants.ObjectTypes.Project);
      }

      [Observation]
      public void should_add_the_names_of_the_existing_compounds_as_used_names()
      {
         //names are added in lower case
         _dto.UsedNames.ShouldOnlyContain("caffeine");
      }

      [Observation]
      public void should_not_allow_the_name_of_a_model_element()
      {
         _dto.Name = "Theophylline";
         _dto.IsValid().ShouldBeTrue();
         _dto.Name = "Volume";
         _dto.IsValid().ShouldBeFalse();
      }
   }

   public class When_creating_a_dto_for_a_new_building_block_that_is_not_a_compound : concern_for_ObjectBaseDTOFactory
   {
      private ObjectBaseDTO _dto;

      protected override void Because()
      {
         _dto = sut.CreateFor<Individual>();
      }

      [Observation]
      public void should_return_a_standard_dto()
      {
         _dto.GetType().ShouldBeEqualTo(typeof(ObjectBaseDTO));
      }

      [Observation]
      public void should_add_the_names_of_the_existing_building_blocks_of_the_same_type_as_used_names()
      {
         _dto.UsedNames.ShouldOnlyContain("ind1");
      }

      [Observation]
      public void should_allow_the_name_of_a_model_element()
      {
         _dto.Name = "Volume";
         _dto.IsValid().ShouldBeTrue();
      }
   }
}

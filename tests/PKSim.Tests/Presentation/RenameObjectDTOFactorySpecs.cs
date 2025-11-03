using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Core.Services;
using RenameObjectDTOFactory = PKSim.Presentation.DTO.Core.RenameObjectDTOFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameObjectDTOFactory : ContextSpecification<IRenameObjectDTOFactory>
   {
      private IPKSimProjectRetriever _projectRetriever;
      private IObjectTypeResolver _objectTypeResolver;
      protected PKSimProject _project;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         sut = new RenameObjectDTOFactory(_projectRetriever, _objectTypeResolver);

         _project = new PKSimProject();
         A.CallTo(() => _projectRetriever.Current).Returns(_project);
      }
   }

   public class When_creating_a_rename_DTO_for_a_building_block : concern_for_RenameObjectDTOFactory
   {
      private Individual _individual2;
      private Individual _individual1;
      private PKSimEvent _event1;
      private PKSimEvent _event2;
      private SimpleProtocol _protocol1;
      private SimpleProtocol _protocol2;

      protected override void Context()
      {
         base.Context();
         _individual1 = new Individual().WithName("Ind1");
         _individual2 = new Individual().WithName("Ind2");

         _event1 = new PKSimEvent().WithName("Event1");
         _event2 = new PKSimEvent().WithName("Event2");

         _protocol1 = new SimpleProtocol().WithName("Prot1");
         _protocol2 = new SimpleProtocol().WithName("Prot2");

         _project.AddBuildingBlock(_individual1);
         _project.AddBuildingBlock(_individual2);
         _project.AddBuildingBlock(_event1);
         _project.AddBuildingBlock(_event2);
         _project.AddBuildingBlock(_protocol1);
         _project.AddBuildingBlock(_protocol2);
      }

      [Observation]
      public void should_return_the_names_of_all_building_block_of_the_same_type_as_used_names_for_a_standard_building_block()
      {
         var dto = sut.CreateFor(_individual2);
         //names are added in lower case
         dto.UsedNames.ShouldOnlyContain("ind1", "ind2");
      }

      [Observation]
      public void should_prevent_protocol_and_event_from_having_the_same_name()
      {
         var dto = sut.CreateFor(_protocol1);
         //names are added in lower case
         dto.UsedNames.ShouldOnlyContain("prot1", "prot2", "event1", "event2");
      }
   }
}
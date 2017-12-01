using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_FormulationKeyRepository : ContextSpecification<IFormulationKeyRepository>
   {
      protected IPKSimProjectRetriever _projectRetriever;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         sut = new FormulationKeyRepository(_projectRetriever);
      }
   }

   public class When_retrieving_the_availble_formulation_keys : concern_for_FormulationKeyRepository
   {
      private string _key1;
      private string _key2;
      private string _key3;
      private string _key4;

      protected override void Context()
      {
         base.Context();
         _key1 = "key1";
         _key2 = "key2";
         _key3 = "key3";
         _key4 = "key4";

         var loadedProtocol1 = A.Fake<Protocol>().WithName("P1");
         loadedProtocol1.IsLoaded = true;
         A.CallTo(() => loadedProtocol1.UsedFormulationKeys).Returns(new[] {_key1, _key2});
         var notLoadedProtocol = A.Fake<Protocol>().WithName("P2");
         A.CallTo(() => notLoadedProtocol.UsedFormulationKeys).Returns(new[] {"tralala"});

         var loadedProtocol2 = A.Fake<Protocol>().WithName("P3");
         loadedProtocol2.IsLoaded = true;
         A.CallTo(() => loadedProtocol2.UsedFormulationKeys).Returns(new[] {_key2, _key3});

         var project = new PKSimProject();
         project.AddBuildingBlock(loadedProtocol1);
         project.AddBuildingBlock(loadedProtocol2);
         project.AddBuildingBlock(notLoadedProtocol);

         var formulation = new Formulation().WithName(_key4);
         project.AddBuildingBlock(formulation);
         A.CallTo(() => _projectRetriever.CurrentProject).Returns(project);
      }

      [Observation]
      public void should_return_the_keys_used_in_all_loaded_protocols_of_the_active_project_and_not_the_name_of_all_available_formulation()
      {
         sut.All().ShouldOnlyContain(_key1, _key2, _key3);
      }
   }
}
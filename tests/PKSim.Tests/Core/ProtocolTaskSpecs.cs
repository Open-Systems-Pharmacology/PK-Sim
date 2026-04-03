using System.Collections.Generic;
using PKSim.Core.Commands;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolTask : ContextSpecification<IProtocolTask>
   {
      protected IFormulationKeyRepository _formulationKeyRepository;
      protected IEventKeyRepository _eventKeyRepository;
      private ISchemaTask _schemaTask;
      private ISchemaItemParameterRetriever _schemaItemParameterRetriver;
      private IExecutionContext _context;

      protected override void Context()
      {
         _formulationKeyRepository = A.Fake<IFormulationKeyRepository>();
         _eventKeyRepository = A.Fake<IEventKeyRepository>();
         _schemaItemParameterRetriver = A.Fake<ISchemaItemParameterRetriever>();
         _schemaTask = A.Fake<ISchemaTask>();
         _context= A.Fake<IExecutionContext>();
         sut = new ProtocolTask( _formulationKeyRepository, _eventKeyRepository, _schemaTask,_schemaItemParameterRetriver,_context);
      }
   }

   
   public class When_retrieving_the_formulation_keys_avalaible : concern_for_ProtocolTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _formulationKeyRepository.All()).Returns(A.Fake<IEnumerable<string>>());
      }

      [Observation]
      public void should_return_the_formulation_key_defined_in_the_project()
      {
         sut.AllFormulationKey().ShouldBeEqualTo(_formulationKeyRepository.All());
      }
   }

   public class When_retrieving_the_event_keys_available : concern_for_ProtocolTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _eventKeyRepository.All()).Returns(A.Fake<IEnumerable<string>>());
      }

      [Observation]
      public void should_return_the_event_keys_defined_in_the_project()
      {
         sut.AllEventKeys().ShouldBeEqualTo(_eventKeyRepository.All());
      }
   }
}
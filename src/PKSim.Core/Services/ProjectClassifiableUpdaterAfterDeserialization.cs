using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization;

namespace PKSim.Core.Services
{
   public class ProjectClassifiableUpdaterAfterDeserialization : ProjectClassifiableUpdaterAfterDeserializationBase
   {
      private readonly IWithIdRepository _withIdRepository;

      public ProjectClassifiableUpdaterAfterDeserialization(IWithIdRepository withIdRepository)
      {
         _withIdRepository = withIdRepository;
      }

      protected override IWithId RetrieveSubjectFor(IClassifiableWrapper classifiableWrapper, IProject project)
      {
         return _withIdRepository.Get(classifiableWrapper.Id);
      }
   }
}
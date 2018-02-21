using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Repositories
{
   public interface IValueOriginRepository : IStartableRepository<ValueOrigin>
   {
      /// <summary>
      /// Returns the <see cref="ValueOrigin"/> defined for the <paramref name="parameter"/> or a default <see cref="ValueOrigin"/> if not found
      /// </summary>
      ValueOrigin ValueOriginFor(IParameter parameter);

      /// <summary>
      /// Returns the <see cref="ValueOrigin"/> defined in the database with the given <paramref name="id"/> or a default <see cref="ValueOrigin"/> if not found
      /// </summary>
      ValueOrigin FindBy(int? id);
   }
}
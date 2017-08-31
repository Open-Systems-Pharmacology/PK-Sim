using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Snapshots.Mappers
{
   public interface ISnapshotMapper
   {
      /// <summary>
      ///    Given a <paramref name="model" /> object, returns the corresponding snapshot.
      /// </summary>
      /// <exception cref="SnapshotNotFoundException">
      ///    is thrown if a snapshot could not be found for the given
      ///    <paramref name="model" />
      /// </exception>
      object MapToSnapshot(object model);

      /// <summary>
      ///    Given a <paramref name="snapshot" /> object, returns the corresponding model.
      /// </summary>
      /// <exception cref="SnapshotNotFoundException">
      ///    is thrown if a snapshot could not be found for the given
      ///    <paramref name="snapshot" />
      /// </exception>
      object MapToModel(object snapshot);
   }

   public class SnapshotMapper : ISnapshotMapper
   {
      private readonly List<ISnapshotMapperSpecification> _allMappers;

      public SnapshotMapper(IRepository<ISnapshotMapperSpecification> snapshotMapperRepository)
      {
         _allMappers = snapshotMapperRepository.All().ToList();
      }

      public object MapToSnapshot(object model)
      {
         var modelType = model.GetType();
         foreach (var mapper in _allMappers)
         {
            if (mapper.IsSatisfiedBy(modelType))
               return mapper.MapToSnapshot(model);
         }

         throw new SnapshotNotFoundException(modelType);
      }

      public object MapToModel(object snapshot)
      {
         throw new NotImplementedException();
      }
   }
}
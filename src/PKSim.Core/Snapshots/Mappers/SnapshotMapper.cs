using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
      Task<object> MapToSnapshot(object model);

      /// <summary>
      ///    Given a <paramref name="snapshot" /> object, returns the corresponding model.
      /// </summary>
      /// <exception cref="SnapshotNotFoundException">
      ///    is thrown if a snapshot could not be found for the given
      ///    <paramref name="snapshot" />
      /// </exception>
      Task<object> MapToModel(object snapshot);

      /// <summary>
      /// Returns the snapshot type for the model type <typeparamref name="T"/>
      /// </summary>
      /// <typeparam name="T">Model type for which the snapshot type should be found</typeparam>
      /// <exception cref="SnapshotNotFoundException">
      ///    is thrown if a snapshot could not be found for the given
      ///    model type <typeparamref name="T"/>
      /// </exception>
      Type SnapshotTypeFor<T>();
   }

   public class SnapshotMapper : ISnapshotMapper
   {
      private readonly List<ISnapshotMapperSpecification> _allMappers;

      public SnapshotMapper(IRepository<ISnapshotMapperSpecification> snapshotMapperRepository)
      {
         _allMappers = snapshotMapperRepository.All().ToList();
      }

      public Task<object> MapToSnapshot(object model)
      {
         var modelType = model.GetType();
         return mapperFor(modelType).MapToSnapshot(model);
      }

      public Task<object> MapToModel(object snapshot)
      {
         var snapshotType = snapshot.GetType();
         return mapperFor(snapshotType).MapToModel(snapshot);
      }

      public Type SnapshotTypeFor<T>()
      {
         var modelType = typeof(T);
         return mapperFor(modelType).SnapshotTypeFor<T>();
      }

      private ISnapshotMapper mapperFor(Type modelOrSnapshotType)
      {
         var mapper =  _allMappers.FirstOrDefault(x => x.IsSatisfiedBy(modelOrSnapshotType));
         if (mapper != null)
            return mapper;

         throw new SnapshotNotFoundException(modelOrSnapshotType);
      }
   }
}
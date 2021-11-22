﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

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

      /// <summary>
      /// Returns the mapper associated with a given <paramref name="modelOrSnapshotType"/>
      /// <exception cref="SnapshotNotFoundException">Is thrown if no mapper could be found for the given type</exception>
      /// </summary>
      ISnapshotMapper MapperFor(object modelOrSnapshotType);
      /// <summary>
      /// Returns the mapper associated with a given <paramref name="modelOrSnapshotType"/>
      /// <exception cref="SnapshotNotFoundException">Is thrown if no mapper could be found for the given type</exception>
      /// </summary>
      ISnapshotMapper MapperFor(Type modelOrSnapshotType);
   }

   public interface ISnapshotMapperWithProjectAsContext<TModel, TSnapshot>
   {
      /// <summary>
      ///    Given a <paramref name="snapshot" /> object, returns the corresponding model using the <paramref name="project"/> as context
      /// </summary>
      /// <exception cref="SnapshotNotFoundException">
      ///    is thrown if a snapshot could not be found for the given
      ///    <paramref name="snapshot" />
      /// </exception>
      Task<TModel> MapToModel(TSnapshot snapshot, PKSimProject project );
   }

   public class SnapshotMapper : ISnapshotMapper
   {
      private readonly List<ISnapshotMapperSpecification> _allMappers;

      public SnapshotMapper(IRepository<ISnapshotMapperSpecification> snapshotMapperRepository)
      {
         _allMappers = snapshotMapperRepository.All().ToList();
      }

      public Task<object> MapToSnapshot(object model) => MapperFor(model).MapToSnapshot(model);

      public Task<object> MapToModel(object snapshot) => MapperFor(snapshot).MapToModel(snapshot);

      public Type SnapshotTypeFor<T>()
      {
         var modelType = typeof(T);
         return MapperFor(modelType).SnapshotTypeFor<T>();
      }

      public ISnapshotMapper MapperFor(object modelOrSnapshotObject) => MapperFor(modelOrSnapshotObject.GetType());

      public ISnapshotMapper MapperFor(Type modelOrSnapshotType)
      {
         var mapper =  _allMappers.FirstOrDefault(x => x.IsSatisfiedBy(modelOrSnapshotType));
         if (mapper != null)
            return mapper;

         throw new SnapshotNotFoundException(modelOrSnapshotType);
      }
   }
}
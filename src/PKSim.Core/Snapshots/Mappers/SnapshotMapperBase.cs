﻿using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Snapshots.Mappers
{
   public interface ISnapshotMapperSpecification : ISnapshotMapper, ISpecification<Type>
   {

   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot> : ISnapshotMapperSpecification 
      where TSnapshot: ISnapshot
      where TModel: IObjectBase
   {
      public virtual object MapToSnapshot(object model)
      {
         return MapToSnapshot(model.DowncastTo<TModel>());
      }

      public virtual object MapToModel(object snapshot)
      {
         return MapToModel(snapshot.DowncastTo<TSnapshot>());
      }

      public Type SnapshotTypeFor<T>()
      {
         return typeof(TSnapshot);
      }

      public abstract TSnapshot MapToSnapshot(TModel model);

      public abstract TModel MapToModel(TSnapshot snapshot);

      public virtual bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<TModel>() || item.IsAnImplementationOf<TSnapshot>();
      }

      protected void MapModelPropertiesIntoSnapshot(TModel model, TSnapshot snapshot)
      {
         snapshot.Name = model.Name;
         snapshot.Description = SnapshotValueFor(model.Description);
      }

      protected void MapSnapshotPropertiesIntoModel(TSnapshot snapshot, TModel model)
      {
         model.Name = snapshot.Name;
         model.Description = snapshot.Description;
      }

      protected string SnapshotValueFor(string value) => !string.IsNullOrEmpty(value) ? value : null;

      protected string UnitValueFor(string unit) => unit ?? " ";
   }

   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot> 
      where TModel : IObjectBase 
      where TSnapshot : ParameterContainerSnapshotBase
   {
      protected readonly ParameterMapper _parameterMapper;

      protected ParameterContainerSnapshotMapperBase(ParameterMapper parameterMapper)
      {
         _parameterMapper = parameterMapper;
      }

      protected Parameter ParameterSnapshotFor(IParameter parameter) => _parameterMapper.MapToSnapshot(parameter);

      protected void MapParameters(IEnumerable<IParameter> parameters, TSnapshot snapshot)
      {
         parameters.Each(p => snapshot.AddParameters(ParameterSnapshotFor(p)));
      }
   }
}
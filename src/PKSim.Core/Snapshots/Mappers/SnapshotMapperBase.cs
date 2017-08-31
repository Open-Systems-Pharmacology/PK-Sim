using System;
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
         throw new NotImplementedException();
      }

      public abstract TSnapshot MapToSnapshot(TModel model);

      public virtual bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<TModel>() || item.IsAnImplementationOf<TSnapshot>();
      }

      protected void MapBaseProperties(TModel model, TSnapshot snapshot)
      {
         snapshot.Name = model.Name;
         snapshot.Description = SnapshotValueFor(model.Description);
      }

      protected string SnapshotValueFor(string value) => !string.IsNullOrEmpty(value) ? value : null;
   }

   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot> 
      where TModel : IObjectBase 
      where TSnapshot : ParameterContainerSnapshotBase
   {
      private readonly ParameterMapper _parameterMapper;

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
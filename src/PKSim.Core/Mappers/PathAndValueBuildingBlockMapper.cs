using System.Collections.Generic;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IPathAndValueBuildingBlockMapper<in T, out TBuildingBlock> : IMapper<T, TBuildingBlock>
   {
   }

   public abstract class PathAndValueBuildingBlockMapper<T, TBuildingBlock, TBuilder> : IPathAndValueBuildingBlockMapper<T, TBuildingBlock> where T : PKSimBuildingBlock where TBuildingBlock : PathAndValueEntityBuildingBlockFromPKSim<TBuilder> where TBuilder : PathAndValueEntity
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IEntityPathResolver _entityPathResolver;
      protected IApplicationConfiguration _applicationConfiguration;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly Cache<string, IFormula> _formulaCache = new Cache<string, IFormula>(x => x.Name);

      protected PathAndValueBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration, ILazyLoadTask lazyLoadTask)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _applicationConfiguration = applicationConfiguration;
         _lazyLoadTask = lazyLoadTask;
      }

      protected TBuildingBlock CreateBaseObject(T pkSimBuildingBlock)
      {
         var buildingBlock = _objectBaseFactory.Create<TBuildingBlock>();

         buildingBlock.Name = pkSimBuildingBlock.Name;
         buildingBlock.PKSimVersion = _applicationConfiguration.Version;
         buildingBlock.Description = pkSimBuildingBlock.Description;
         return buildingBlock;
      }

      private TBuilder mapBuilderParameter(IParameter parameter)
      {
         var builderParameter = _objectBaseFactory.Create<TBuilder>();

         // Add the formula to the building block formula cache if the formula can be cached
         if (isFormulaCachable(parameter))
         {
            if (!_formulaCache.Contains(parameter.Formula.Name))
               _formulaCache.Add(parameter.Formula);

            // If the parameter value is different from the default value, set the value only and not the formula
            // If the parameter value is not different from the default, set the formula only and not the value
            // Even if the formula is not be used by the builder parameter, the cache will have the formula available
            if (parameter.ValueDiffersFromDefault())
               builderParameter.Value = getParameterValue(parameter);
            else
               builderParameter.Formula = _formulaCache[parameter.Formula.Name];
         }
         else
         {
            builderParameter.Value = getParameterValue(parameter);
         }

         builderParameter.Name = parameter.Name;

         builderParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
         builderParameter.Dimension = parameter.Dimension;
         builderParameter.DisplayUnit = parameter.DisplayUnit;
         return builderParameter;
      }

      private static bool isFormulaCachable(IParameter parameter)
      {
         return parameter.Formula != null && parameter.Formula.IsCachable();
      }

      private static double getParameterValue(IParameter parameter)
      {
         return parameter.TryGetValue().value;
      }

      protected void MapAllParameters(T sourcePKSimBuildingBlock, TBuildingBlock buildingBlock)
      {
         var allParameters = AllParametersFor(sourcePKSimBuildingBlock);

         foreach (var parameter in allParameters)
         {
            var builderParameter = mapBuilderParameter(parameter);
            buildingBlock.Add(builderParameter);
         }

         foreach (var formula in _formulaCache)
         {
            buildingBlock.FormulaCache.Add(formula);
         }
      }

      protected abstract IReadOnlyList<IParameter> AllParametersFor(T sourcePKSimBuildingBlock);

      public virtual TBuildingBlock MapFrom(T input)
      {
         _lazyLoadTask.Load(input);

         var buildingBlock = CreateBaseObject(input);
         MapAllParameters(input, buildingBlock);
         return buildingBlock;
      }
   }
}
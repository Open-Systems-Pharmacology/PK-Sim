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
      private readonly Cache<string, IFormula> _formulaCache = new Cache<string, IFormula>(x => x.Name);

      protected PathAndValueBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _applicationConfiguration = applicationConfiguration;
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

         if (parameter.Formula != null && parameter.Formula.IsCachable())
         {
            if (!_formulaCache.Contains(parameter.Formula.Name))
               _formulaCache.Add(parameter.Formula);

            builderParameter.Formula = _formulaCache[parameter.Formula.Name];
         }
         else
         {
            (builderParameter.Value, _) = parameter.TryGetValue();
         }

         builderParameter.Name = parameter.Name;

         builderParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
         builderParameter.Dimension = parameter.Dimension;
         builderParameter.DisplayUnit = parameter.DisplayUnit;
         return builderParameter;
      }

      protected void MapAllParameters(T sourcePKSimBuildingBlock, TBuildingBlock buildingBlock)
      {
         var allParameters = AllParametersFor(sourcePKSimBuildingBlock);

         foreach (var parameter in allParameters)
         {
            var expressionParameter = mapBuilderParameter(parameter);
            buildingBlock.Add(expressionParameter);
         }
         foreach (var formula in _formulaCache)
         {
            buildingBlock.FormulaCache.Add(formula);
         }
      }

      protected abstract IReadOnlyList<IParameter> AllParametersFor(T sourcePKSimBuildingBlock);

      public virtual TBuildingBlock MapFrom(T input)
      {
         var buildingBlock = CreateBaseObject(input);

         MapAllParameters(input, buildingBlock);
         return buildingBlock;
      }
   }
}
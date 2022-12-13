using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using PKSim.Core.Model;
using FormulaExtensions = OSPSuite.Core.Domain.Formulas.FormulaExtensions;

namespace PKSim.Core.Mappers
{
   public abstract class PathAndValueBuildingBlockMapper<T, TBuildingBlock, TBuilder> : IMapper<T, TBuildingBlock> where T : PKSimBuildingBlock where TBuildingBlock : PathAndValueEntityBuildingBlockSourcedFromPKSimBuildingBlock<TBuilder> where TBuilder : PathAndValueEntity
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IEntityPathResolver _entityPathResolver;
      protected IFormulaFactory _formulaFactory;
      protected IApplicationConfiguration _applicationConfiguration;

      protected PathAndValueBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IFormulaFactory formulaFactory, IApplicationConfiguration applicationConfiguration)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _formulaFactory = formulaFactory;
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

      protected TBuilder MapBuilderParameter(IParameter parameter,
         TBuildingBlock buildingBlock)
      {
         var builderParameter = _objectBaseFactory.Create<TBuilder>();

         if (parameter.Formula != null && FormulaExtensions.IsCachable(parameter.Formula))
         {
            var formula = _formulaFactory.RateFor(CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS, parameter.Formula.Name,
               buildingBlock.FormulaCache);
            builderParameter.Formula = formula;
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
         var allParameters = sourcePKSimBuildingBlock.GetAllChildren<IParameter>();

         foreach (var parameter in allParameters)
         {
            var expressionParameter = MapBuilderParameter(parameter, buildingBlock);
            buildingBlock.Add(expressionParameter);
         }
      }

      public virtual TBuildingBlock MapFrom(T input)
      {
         var buildingBlock = CreateBaseObject(input);

         MapAllParameters(input, buildingBlock);
         return buildingBlock;
      }
   }
}
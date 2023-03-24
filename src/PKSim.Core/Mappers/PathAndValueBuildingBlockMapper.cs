using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Mappers
{
   public interface IPathAndValueBuildingBlockMapper<in TPKSimBuildingBlock, out TBuildingBlock> : IMapper<TPKSimBuildingBlock, TBuildingBlock>
   {
   }

   public interface IPathAndValueBuildingBlockMapper<in TPKSimBuildingBlock, out TBuildingBlock, out TBuilder> : IPathAndValueBuildingBlockMapper<TPKSimBuildingBlock, TBuildingBlock>
   {
      /// <summary>
      ///    Map the parameter to the underlying builder parameter.
      ///    Note that formula or value will not be set. Only common parameter properties
      /// </summary>
      TBuilder MapParameter(IParameter parameter);
   }

   public abstract class PathAndValueBuildingBlockMapper<TPKSimBuildingBlock, TBuildingBlock, TBuilder> : IPathAndValueBuildingBlockMapper<TPKSimBuildingBlock, TBuildingBlock, TBuilder> where TPKSimBuildingBlock : PKSimBuildingBlock
      where TBuildingBlock : PathAndValueEntityBuildingBlockFromPKSim<TBuilder>
      where TBuilder : PathAndValueEntity
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IEntityPathResolver _entityPathResolver;
      protected IApplicationConfiguration _applicationConfiguration;
      private readonly ILazyLoadTask _lazyLoadTask;

      protected readonly IFormulaFactory _formulaFactory;

      protected PathAndValueBuildingBlockMapper(
         IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver,
         IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask,
         IFormulaFactory formulaFactory)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _applicationConfiguration = applicationConfiguration;
         _lazyLoadTask = lazyLoadTask;
         _formulaFactory = formulaFactory;
      }

      protected TBuildingBlock CreateBaseObject(TPKSimBuildingBlock pkSimBuildingBlock)
      {
         var buildingBlock = _objectBaseFactory.Create<TBuildingBlock>();
         buildingBlock.Name = pkSimBuildingBlock.Name;
         buildingBlock.PKSimVersion = _applicationConfiguration.Version;
         buildingBlock.Description = pkSimBuildingBlock.Description;
         return buildingBlock;
      }

      public virtual TBuilder MapParameter(IParameter parameter)
      {
         var builderParameter = _objectBaseFactory.Create<TBuilder>();
         builderParameter.Name = parameter.Name;
         builderParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
         builderParameter.Dimension = parameter.Dimension;
         builderParameter.DisplayUnit = parameter.DisplayUnit;
         return builderParameter;
      }

      private TBuilder mapBuilderParameter(IParameter parameter, TPKSimBuildingBlock pkSimBuildingBlock, BuildingBlockFormulaCache formulaCache)
      {
         var builderParameter = MapParameter(parameter);

         // Add the formula to the building block formula cache if the formula can be cached
         var parameterValue = getParameterValue(parameter);
         var valueChanged = parameter.ValueDiffersFromDefault();
         var formula = parameter.Formula;

         switch (formula)
         {
            case ConstantFormula _:
               builderParameter.Value = parameterValue;
               return builderParameter;

            case DistributionFormula _:

               //formula and did not change. Do not return
               if (!valueChanged)
                  return null;

               builderParameter.Value = parameterValue;
               return builderParameter;

            default:
               if (formula.IsCachable())
               {
                  var templateFormula = retrieveTemplateFormulaFromCache(parameter, pkSimBuildingBlock, formulaCache);
                  builderParameter.Formula = templateFormula;
               }

               // Only set the value of the parameter using a formula if it was indeed set
               if (valueChanged)
                  builderParameter.Value = parameterValue;

               return builderParameter;
         }
      }

      private IFormula retrieveTemplateFormulaFromCache(IParameter parameter, TPKSimBuildingBlock pkSimBuildingBlock, BuildingBlockFormulaCache formulaCache)
      {
         var formulaName = parameter.Formula.Name;
         if (formulaCache.Contains(formulaName))
            return formulaCache[formulaName];

         //This will add the formula top the cache 
         var templateFormula = TemplateFormulaFor(parameter, formulaCache, pkSimBuildingBlock);
         //We need to remove the ROOT keyword when exporting to PKML structure 
         templateFormula?.ObjectPaths.Each(x => x.Remove(Constants.ROOT));
         return templateFormula;
      }

      protected abstract IFormula TemplateFormulaFor(IParameter parameter, IFormulaCache formulaCache, TPKSimBuildingBlock pkSimBuildingBlock);

      private static double getParameterValue(IParameter parameter)
      {
         return parameter.TryGetValue().value;
      }

      protected void MapAllParameters(TPKSimBuildingBlock sourcePKSimBuildingBlock, TBuildingBlock buildingBlock)
      {
         //Cache used to store all formula that can be cached. This is required to avoid having the same formula defined multiple times in the building block
         //note that a clone of the original formula is added to the cache so that it can be modified if required
         var formulaCache = new BuildingBlockFormulaCache();
         var allBuilderParameters = AllParametersFor(sourcePKSimBuildingBlock).Select(x=>mapBuilderParameter(x, sourcePKSimBuildingBlock, formulaCache));
         allBuilderParameters.Where(x => x != null).Each(buildingBlock.Add);

         //Formula cache already contains a clone of all formula. We can add as is
         formulaCache.Each(buildingBlock.FormulaCache.Add);
      }

      protected abstract IReadOnlyList<IParameter> AllParametersFor(TPKSimBuildingBlock sourcePKSimBuildingBlock);

      public virtual TBuildingBlock MapFrom(TPKSimBuildingBlock pkSimBuildingBlock)
      {
         _lazyLoadTask.Load(pkSimBuildingBlock);

         var buildingBlock = CreateBaseObject(pkSimBuildingBlock);
         MapAllParameters(pkSimBuildingBlock, buildingBlock);
         return buildingBlock;
      }
   }
}
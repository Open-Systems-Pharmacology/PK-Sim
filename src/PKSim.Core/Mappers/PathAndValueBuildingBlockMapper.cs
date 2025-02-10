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
using PKSim.Core.Services;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using ILazyLoadTask = OSPSuite.Core.Domain.Services.ILazyLoadTask;

namespace PKSim.Core.Mappers
{
   public interface IPathAndValueBuildingBlockMapper<in TPKSimBuildingBlock, out TBuildingBlock> : IMapper<TPKSimBuildingBlock, TBuildingBlock>
   {
   }

   public interface IPathAndValueBuildingBlockMapper<in TPKSimBuildingBlock, out TBuildingBlock, out TBuilder> : IPathAndValueBuildingBlockMapper<TPKSimBuildingBlock, TBuildingBlock>
   {

      /// <summary>
      ///    Map the parameter to the underlying builder parameter.
      /// </summary>
      TBuilder MapParameter(IParameter parameter, TPKSimBuildingBlock pkSimBuildingBlock, IFormulaCache formulaCache);
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
      private readonly ICloner _cloner;

      protected PathAndValueBuildingBlockMapper(
         IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver,
         IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask,
         IFormulaFactory formulaFactory,
         ICloner cloner)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _applicationConfiguration = applicationConfiguration;
         _lazyLoadTask = lazyLoadTask;
         _formulaFactory = formulaFactory;
         _cloner = cloner;
      }

      protected TBuildingBlock CreateBaseObject(TPKSimBuildingBlock pkSimBuildingBlock)
      {
         var buildingBlock = _objectBaseFactory.Create<TBuildingBlock>();
         buildingBlock.Name = pkSimBuildingBlock.Name;
         buildingBlock.PKSimVersion = _applicationConfiguration.Version;
         buildingBlock.Description = pkSimBuildingBlock.Description;
         return buildingBlock;
      }

      protected virtual TBuilder CreateParameter(IParameter parameter, TPKSimBuildingBlock pkSimBuildingBlock)
      {
         var builderParameter = _objectBaseFactory.Create<TBuilder>();
         builderParameter.Name = parameter.Name;
         builderParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
         builderParameter.Dimension = parameter.Dimension;
         builderParameter.DisplayUnit = parameter.DisplayUnit;
         builderParameter.ValueOrigin.UpdateFrom(parameter.ValueOrigin);
         return builderParameter;
      }

      protected virtual void MapFormulaOrValue(IParameter parameter, TBuilder builderParameter, TPKSimBuildingBlock pkSimBuildingBlock, IFormulaCache formulaCache)
      {
         var parameterValue = getParameterValue(parameter);
         var valueChanged = parameter.ValueDiffersFromDefault();
         var formula = parameter.Formula;

         switch (formula)
         {
            case ConstantFormula _:
               builderParameter.Value = parameterValue;
               break;
            case DistributionFormula _:
            {
               //formula and did not change. Do not return
               if (!valueChanged)
                  break;

               builderParameter.Value = parameterValue;
               break;
            }
            default:
               if (formula.IsCachable())
               {
                  var templateFormula = retrieveTemplateFormulaFromCache(parameter, pkSimBuildingBlock, formulaCache);
                  builderParameter.Formula = templateFormula;
               }

               // Only set the value of the parameter using a formula if it was indeed set
               if (valueChanged)
                  builderParameter.Value = parameterValue;
               break;
         }
      }

      public TBuilder MapParameter(IParameter parameter, TPKSimBuildingBlock pkSimBuildingBlock, IFormulaCache formulaCache)
      {
         var builderParameter = CreateParameter(parameter, pkSimBuildingBlock);
         MapFormulaOrValue(parameter, builderParameter, pkSimBuildingBlock, formulaCache);
         return builderParameter;
      }

      private IFormula retrieveTemplateFormulaFromCache(IParameter parameter, TPKSimBuildingBlock pkSimBuildingBlock, IFormulaCache formulaCache)
      {
         var formula = parameter.Formula;
         var formulaName = formula.Name;
         if (formulaCache.Contains(formulaName))
            return formulaCache[formulaName];

         IFormula templateFormula;
         switch (formula)
         {
            case TableFormula tableFormula:
               templateFormula = _cloner.Clone(tableFormula);
               formulaCache.Add(templateFormula);
               break;
            default:

               //This will add the formula to the cache 
               templateFormula = TemplateFormulaFor(parameter, formulaCache, pkSimBuildingBlock);
               //We need to remove the ROOT keyword when exporting to PKML structure 
               templateFormula?.ObjectPaths.Each(x => x.Remove(Constants.ROOT));
               break;
         }

         return templateFormula;
      }

      protected abstract IFormula TemplateFormulaFor(IParameter parameter, IFormulaCache formulaCache, TPKSimBuildingBlock pkSimBuildingBlock);

      private static double getParameterValue(IParameter parameter) => parameter.TryGetValue().value;

      protected void MapAllParameters(TPKSimBuildingBlock sourcePKSimBuildingBlock, TBuildingBlock buildingBlock)
      {
         //Cache used to store all formula that can be cached. This is required to avoid having the same formula defined multiple times in the building block
         //note that a clone of the original formula is added to the cache so that it can be modified if required
         var formulaCache = new BuildingBlockFormulaCache();
         var allBuilderParameters = AllParametersFor(sourcePKSimBuildingBlock).Select(x => MapParameter(x, sourcePKSimBuildingBlock, formulaCache));
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
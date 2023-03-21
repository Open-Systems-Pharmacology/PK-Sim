﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ILazyLoadTask = OSPSuite.Core.Domain.Services.ILazyLoadTask;

namespace PKSim.Core.Mappers
{
   public interface IPathAndValueBuildingBlockMapper<in TPKSimBuildingBlock, out TBuildingBlock> : IMapper<TPKSimBuildingBlock, TBuildingBlock>
   {
   }

   public abstract class PathAndValueBuildingBlockMapper<TPKSimBuildingBlock, TBuildingBlock, TBuilder> : IPathAndValueBuildingBlockMapper<TPKSimBuildingBlock, TBuildingBlock> where TPKSimBuildingBlock : PKSimBuildingBlock where TBuildingBlock : PathAndValueEntityBuildingBlockFromPKSim<TBuilder>
      where TBuilder : PathAndValueEntity
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IEntityPathResolver _entityPathResolver;
      protected IApplicationConfiguration _applicationConfiguration;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly ICloner _cloner;

      //Cache used to store all formula that can be cached. This is required to avoid having the same formula defined multiple times in the building block
      //note that a clone of the original formula is added to the cache so that it can be modified if required
      private readonly Cache<string, IFormula> _formulaCache = new Cache<string, IFormula>(x => x.Name);

      protected PathAndValueBuildingBlockMapper(
         IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver,
         IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask,
         ICloner cloner)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _applicationConfiguration = applicationConfiguration;
         _lazyLoadTask = lazyLoadTask;
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

      private TBuilder mapBuilderParameter(IParameter parameter)
      {
         var builderParameter = _objectBaseFactory.Create<TBuilder>();
         builderParameter.Name = parameter.Name;
         builderParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
         builderParameter.Dimension = parameter.Dimension;
         builderParameter.DisplayUnit = parameter.DisplayUnit;

         // Add the formula to the building block formula cache if the formula can be cached
         if (isFormulaCachable(parameter))
         {
            if (!_formulaCache.Contains(parameter.Formula.Name))
               _formulaCache.Add(cloneFormulaForExport(parameter));

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

         return builderParameter;
      }

      private IFormula cloneFormulaForExport(IParameter parameter)
      {
         var cloneFormula = _cloner.Clone(parameter.Formula);
         cloneFormula.ObjectPaths.Each(x => x.Remove(Constants.ROOT));
         return cloneFormula;
      }

      private static bool isFormulaCachable(IParameter parameter)
      {
         return parameter.Formula != null && parameter.Formula.IsCachable();
      }

      private static double getParameterValue(IParameter parameter)
      {
         return parameter.TryGetValue().value;
      }

      protected void MapAllParameters(TPKSimBuildingBlock sourcePKSimBuildingBlock, TBuildingBlock buildingBlock)
      {
         var allBuilderParameters = AllParametersFor(sourcePKSimBuildingBlock).Select(mapBuilderParameter);
         allBuilderParameters.Each(buildingBlock.Add);

         //Formula cache already contains a clone of all formula. We can add as is
         _formulaCache.Each(buildingBlock.FormulaCache.Add);
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
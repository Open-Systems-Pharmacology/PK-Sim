using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public interface IAdvancedParameterFactory
   {
      AdvancedParameter CreateDefaultFor(IParameter parameter);
      AdvancedParameter Create(IParameter parameter, DistributionType distributionType);
   }

   public class AdvancedParameterFactory : IAdvancedParameterFactory
   {
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterFactory _parameterFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private const int _defaultUniformFactor = 1000;

      public AdvancedParameterFactory(IEntityPathResolver entityPathResolver, IParameterFactory parameterFactory, IObjectBaseFactory objectBaseFactory)
      {
         _entityPathResolver = entityPathResolver;
         _parameterFactory = parameterFactory;
         _objectBaseFactory = objectBaseFactory;
      }

      public AdvancedParameter CreateDefaultFor(IParameter parameter)
      {
         return Create(parameter, DistributionTypes.Normal);
      }

      public AdvancedParameter Create(IParameter parameter, DistributionType distributionType)
      {
         var advancedParameter = _objectBaseFactory.Create<AdvancedParameter>();

         //we don't care about the name, it should only be unique in the hiearchy
         advancedParameter.Name = _entityPathResolver.PathFor(parameter);
         advancedParameter.ParameterPath = advancedParameter.Name;

         var distributionMetaData = new ParameterDistributionMetaData
         {
            DistributionType = distributionType.Id,
            BuildingBlockType = buildingBlockTypeFrom(parameter),
            Mean = defaultMeanValueBasedOn(parameter.Value, distributionType),
            Deviation = defaultStdValueBasedOn(distributionType),
            Dimension = parameter.Dimension.Name,
            DefaultUnit = parameter.DisplayUnit.Name,
            ParameterName = "distribution"
         };

         if (distributionType == DistributionTypes.Uniform)
         {
            distributionMetaData.MinValue = parameter.MinValue ?? parameter.Value / _defaultUniformFactor;
            //+ _defaultUniformFactor in case the default value is 0
            distributionMetaData.MaxValue = parameter.MaxValue ?? (parameter.Value * _defaultUniformFactor + _defaultUniformFactor);
         }
         else
         {
            distributionMetaData.MinValue = parameter.MinValue;
            distributionMetaData.MinIsAllowed = parameter.MinIsAllowed;
            distributionMetaData.MaxValue = parameter.MaxValue;
            distributionMetaData.MaxIsAllowed = parameter.MaxIsAllowed;
         }

         advancedParameter.DistributedParameter = _parameterFactory.CreateFor(distributionMetaData);

         //should set the distribution parameter as visible since they will need to be edited
         advancedParameter.DistributedParameter.AllParameters().Each(x =>
         {
            x.Visible = true;
            x.IsDefault = false;
            x.CanBeVaried = true;
            x.CanBeVariedInPopulation = false;
         });

         return advancedParameter;
      }

      private PKSimBuildingBlockType buildingBlockTypeFrom(IParameter parameter)
      {
         if (parameter.BuildingBlockType == PKSimBuildingBlockType.Simulation)
            return PKSimBuildingBlockType.Simulation;
         return PKSimBuildingBlockType.Population;
      }

      private double defaultStdValueBasedOn(DistributionType distributionType)
      {
         return distributionType == DistributionTypes.LogNormal ? 1 : 0;
      }

      private double defaultMeanValueBasedOn(double value, DistributionType distributionType)
      {
         return value;
      }
   }
}
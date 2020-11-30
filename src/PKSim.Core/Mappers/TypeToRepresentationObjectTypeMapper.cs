using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using CalculationMethod = OSPSuite.Core.Domain.CalculationMethod;

namespace PKSim.Core.Mappers
{
   public interface ITypeToRepresentationObjectTypeMapper : IMapper<Type, RepresentationObjectType>
   {
   }

   public class TypeToRepresentationObjectTypeMapper : ITypeToRepresentationObjectTypeMapper
   {
      public RepresentationObjectType MapFrom(Type objectType)
      {
         if (objectType.IsAnImplementationOf<IParameter>())
            return RepresentationObjectType.PARAMETER;

         if (objectType.IsAnImplementationOf<Species>())
            return RepresentationObjectType.SPECIES;

         if (objectType.IsAnImplementationOf<Gender>())
            return RepresentationObjectType.GENDER;

         if (objectType.IsAnImplementationOf<SpeciesPopulation>())
            return RepresentationObjectType.POPULATION;

         if (objectType.IsAnImplementationOf<ModelConfiguration>())
            return RepresentationObjectType.MODEL;

         if (objectType.IsAnImplementationOf<ParameterValueVersion>())
            return RepresentationObjectType.PARAMETER_VALUE_VERSION;

         if (objectType.IsAnImplementationOf<CalculationMethod>())
            return RepresentationObjectType.CALCULATION_METHOD;

         if (objectType.IsAnImplementationOf<CalculationMethodCategory>())
            return RepresentationObjectType.CATEGORY;

         if (objectType.IsAnImplementationOf<Category<CalculationMethod>>())
            return RepresentationObjectType.CATEGORY;

         if (objectType.IsAnImplementationOf<ParameterValueVersionCategory>())
            return RepresentationObjectType.CATEGORY;

         if (objectType.IsAnImplementationOf<IGroup>())
            return RepresentationObjectType.GROUP;

         if (objectType.IsAnImplementationOf<ParameterAlternativeGroup>())
            return RepresentationObjectType.GROUP;

         if (objectType.IsAnImplementationOf<IDimension>())
            return RepresentationObjectType.DIMENSION;

         if (objectType.IsAnImplementationOf<IObserver>())
            return RepresentationObjectType.OBSERVER;

         if (objectType.IsAnImplementationOf<IContainer>())
            return RepresentationObjectType.CONTAINER;

         if (objectType.IsAnImplementationOf<Ontogeny>())
            return RepresentationObjectType.ONTOGENY;

         if (objectType.IsAnImplementationOf<IEventAssignment>())
            return RepresentationObjectType.EVENT;

         if(objectType.IsAnImplementationOf<TransportDirection>())
            return RepresentationObjectType.TRANSPORT_DIRECTION;

         return RepresentationObjectType.UNKNOWN;
      }
   }
}
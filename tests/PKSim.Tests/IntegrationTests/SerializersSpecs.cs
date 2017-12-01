using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using OSPSuite.BDDHelper;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using NUnit.Framework;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using EntityRules = PKSim.Core.Model.EntityRules;

namespace PKSim.IntegrationTests
{
   public class pksim_domain_model_conventions : StaticContextSpecification
   {
      [Observation]
      public void each_concrete_domain_model_object_should_have_a_defined_serializer_or_attribute_mapper()
      {
         var domainModelAssembly = typeof (PKSimProject).Assembly;

         //in namespace domain model and not a test!
         var allModelType = from type in ReflectionHelper.GetConcreteTypesFromAssembly(domainModelAssembly, true)
            where type.Namespace.StartsWith(typeof (PKSimProject).Namespace)
            where type.IsAnImplementationOf<StaticContextSpecification>() == false
            select type;

         var serializerAssembly = typeof (BaseXmlSerializer<>).Assembly;
         var allSerializerTypes = ReflectionHelper.GetConcreteTypesFromAssemblyImplementing<IXmlSerializer>(serializerAssembly, true);
         var allImplementationType = new Collection<Type>();

         //cache all serializer
         foreach (var type in allSerializerTypes)
         {
            foreach (var typeForGenericType in type.GetDeclaredTypesForGeneric(typeof (IXmlSerializer<,>)))
            {
               if (allImplementationType.Contains(typeForGenericType.DeclaredType)) continue;
               allImplementationType.Add(typeForGenericType.DeclaredType);
            }
         }

         var allPossibleTypeDefinedAsAttribute = getAllPossibleTypeDefinedAsAttributeIn(serializerAssembly);

         var errorList = new List<string>();
         foreach (var type in allModelType)
         {
            var modelType = type;

            if (typeDoesNotNeedSerializer(modelType)) continue;
            if (allImplementationType.Contains(modelType)) continue;

            //can we find an attribute?
            if (allPossibleTypeDefinedAsAttribute.Any(attributeType => modelType.IsAnImplementationOf(attributeType))) continue;

            errorList.Add($"No serializer found for {type}");
         }

         Assert.IsTrue(errorList.Count == 0, errorList.ToString("\n"));
      }

      private bool typeDoesNotNeedSerializer(Type type)
      {
         if (type.IsNested) return true;
         if (type.IsAnImplementationOf<Exception>()) return true;
         if (type.Name.Contains("Exception")) return true;
         if (type.Name.Contains("Factory")) return true;
         if (type.Equals(typeof (PKSimContainerType))) return true;
         if (type.Equals(typeof (PlasmaProteinBindingPartner))) return true;
         if (type.Equals(typeof (PivotArea))) return true;
         if (type.Equals(typeof (RandomValue))) return true;
         if (type.Equals(typeof (PopulationAgeSettings))) return true;
         if (type.Equals(typeof (CompoundType))) return true;
         if (type.Equals(typeof (RateKey))) return true;
         if (type.Equals(typeof (CalculationMethodCategory))) return true;
         if (type.Equals(typeof (Template))) return true;
         if (type.Equals(typeof (TransporterContainerTemplate))) return true;
         if (type.Equals(typeof (ParameterValueVersionCategory))) return true;
         if (type.Equals(typeof (ImportLogger))) return true;
         if (type.Equals(typeof (LabelGenerationStrategy))) return true;
         if (type.Equals(typeof (DosingIntervalId))) return true;
         if (type.Equals(typeof (LabelGenerationStrategyId))) return true;
         if (type.Equals(typeof (SystemicProcessTypeId))) return true;
         if (type.Equals(typeof (CategoryType))) return true;
         if (type.Equals(typeof (ProcessActionType))) return true;
         if (type.Equals(typeof(GlobalPKAnalysis))) return true;
         if (type.Equals(typeof (NullIndividualMolecule))) return true;
         if (type.Equals(typeof (NoInteractionProcess))) return true;
         if (type.Equals(typeof (EntityRules))) return true;
         if (type.Equals(typeof(NullPopulationSimulationPKAnalyses))) return true;
         if (type.Equals(typeof (NotSelectedSystemicProcess))) return true;
         if (type.Equals(typeof (NotAvailableSystemicProcess))) return true;
         if (type.Equals(typeof (NullSystemicProcess))) return true;
         if (type.Equals(typeof (SimulationPartialProcessStatus))) return true;
         if (type.Equals(typeof (CompoundProcessParameterMapping))) return true;
         if (type.Equals(typeof (MoleculeStartFormula))) return true;
         if (type.Equals(typeof (MoleculeParameter))) return true;
         if (type.Equals(typeof (SimulationPartialProcess))) return true;
         if (type.Equals(typeof (FormulaType))) return true;
         if (type.Equals(typeof (ExpressionResult))) return true;
         if (type.Equals(typeof (ExpressionContainerInfo))) return true;
         if (type.Equals(typeof (QueryExpressionResults))) return true;
         if (type.Equals(typeof (QueryExpressionSettings))) return true;
         if (type.Equals(typeof (OntogenyMetaData))) return true;
         if (type.Equals(typeof (DistributedParameterValue))) return true;
         if (type.Equals(typeof (QuantityValues))) return true;
         if (type.Equals(typeof (SimulationResultsFile))) return true;
         if (type.Equals(typeof (SimulationResultsImport))) return true;
         if (type.Equals(typeof (SimulationPKParametersImport))) return true;
         if (type.Equals(typeof(PopulationSimulationImport))) return true;
         if (type.Equals(typeof(GroupingItem))) return true;
         if (type.Equals(typeof(NullOntogeny))) return true;
         if (type.Equals(typeof(NullNumericField))) return true;
         if (type.Equals(typeof(NullOutputField))) return true;
         if (type.Equals(typeof(NullSimulation))) return true;
         if (type.Equals(typeof(NullParameter))) return true;
     
         //this type are always generated on the fly in PKSim and do not need to be serialied
         if (type.Equals(typeof (PKSimTransport))) return true;
         if (type.Equals(typeof (PKSimSpatialStructure))) return true;
         if (type.Equals(typeof (PKSimReaction))) return true;
         if (type.Equals(typeof (PKSimObserverBuilder))) return true;
         if (type.Equals(typeof (PKSimTransport))) return true;

         //help classes that are not stored in our domain objects
         if (type.Equals(typeof(NumericFieldContext))) return true;
         if (type.Equals(typeof(ParameterValue))) return true;
         if (type.Equals(typeof(PKAnalysesFile))) return true;
         if (type.Equals(typeof (IndividualProperties))) return true;
         if (type.Equals(typeof (ParameterValueMetaData))) return true;
         if (type.Equals(typeof (ParameterMetaData))) return true;
         if (type.Equals(typeof (TemplateType))) return true;
         if (type.Equals(typeof (RateObjectPaths))) return true;
         if (type.Equals(typeof (NullObjectPaths))) return true;
         if (type.Equals(typeof (FloatMatrix))) return true;
         if (type.Equals(typeof (PivotResult))) return true;
         if (type.Equals(typeof (DynamicGroup))) return true;
         if (type.Equals(typeof (PKAnalysis))) return true;
         if (type.Equals(typeof (ParameterRateMetaData))) return true;
         if (type.Equals(typeof (StatisticalAggregationType))) return true;
         if (type.Equals(typeof (RepresentationObjectType))) return true;
         if (type.Equals(typeof (RepresentationInfo))) return true;
         if (type.Equals(typeof (EmptyRepresentationInfo))) return true;
         if (type.Equals(typeof (NullDataRepository))) return true;
         if (type.Equals(typeof (ParameterDistributionMetaData))) return true;
         if (type.Equals(typeof (BinInterval))) return true;
         if (type.Equals(typeof (ContinuousDistributionData))) return true;
         if (type.Equals(typeof (DiscreteDistributionData))) return true;

         //enum are created on the fly
         if (type.Equals(typeof (OrganType))) return true;
         if (type.Equals(typeof(InteractionType))) return true;
         if (type.Equals(typeof (ProtocolMode))) return true;
         if (type.Equals(typeof (BuildingBlockStatus))) return true;
         if (type.Equals(typeof (TemplateDatabaseType))) return true;
         if (type.Equals(typeof (TissueLocation))) return true;
         if (type.Equals(typeof (IntracellularVascularEndoLocation))) return true;
         if (type.Equals(typeof (MembraneLocation))) return true;
         if (type.Equals(typeof(ClassificationType))) return true;
         if (type.Equals(typeof(PopulationAnalysisType))) return true;

         return false;
      }

      private IEnumerable<Type> getAllPossibleTypeDefinedAsAttributeIn(Assembly serializerAssembly)
      {
         var allAttributesTypes = ReflectionHelper.GetConcreteTypesFromAssemblyImplementing<IAttributeMapper>(serializerAssembly, true);
         return from type in allAttributesTypes
                from typeForGenericType in type.GetDeclaredTypesForGeneric(typeof(IAttributeMapper<,>))
            select typeForGenericType.DeclaredType;
      }
   }
}
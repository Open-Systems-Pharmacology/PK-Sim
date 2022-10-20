using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using EntityRules = PKSim.Core.Model.EntityRules;

namespace PKSim.IntegrationTests
{
   public class pksim_domain_model_conventions : StaticContextSpecification
   {
      [Observation]
      public void each_concrete_domain_model_object_should_have_a_defined_serializer_or_attribute_mapper()
      {
         var domainModelAssembly = typeof(PKSimProject).Assembly;

         //in namespace domain model and not a test!
         var allModelType = from type in ReflectionHelper.GetConcreteTypesFromAssembly(domainModelAssembly, true)
            where type.Namespace.StartsWith(typeof(PKSimProject).Namespace)
            where type.IsAnImplementationOf<StaticContextSpecification>() == false
            select type;

         var serializerAssembly = typeof(BaseXmlSerializer<>).Assembly;
         var allSerializerTypes = ReflectionHelper.GetConcreteTypesFromAssemblyImplementing<IXmlSerializer>(serializerAssembly, true);
         var allImplementationType = new Collection<Type>();

         //cache all serializer
         foreach (var type in allSerializerTypes)
         {
            foreach (var typeForGenericType in type.GetDeclaredTypesForGeneric(typeof(IXmlSerializer<,>)))
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
         if (type == typeof(RemoteTemplate)) return true;
         if (type == typeof(LocalTemplate)) return true;
         if (type == typeof(RemoteTemplates)) return true;
         if (type == typeof(PopulationAgeSettings)) return true;
         if (type == typeof(RateKey)) return true;
         if (type == typeof(CalculationMethodCategory)) return true;
         if (type == typeof(Template)) return true;
         if (type == typeof(TransporterContainerTemplate)) return true;
         if (type == typeof(TransporterTemplate)) return true;
         if (type == typeof(ParameterValueVersionCategory)) return true;
         if (type == typeof(LabelGenerationStrategy)) return true;
         if (type == typeof(GlobalPKAnalysis)) return true;
         if (type == typeof(NullIndividualMolecule)) return true;
         if (type == typeof(NoInteractionProcess)) return true;
         if (type == typeof(EntityRules)) return true;
         if (type == typeof(NullPopulationSimulationPKAnalyses)) return true;
         if (type == typeof(NotSelectedSystemicProcess)) return true;
         if (type == typeof(NotAvailableSystemicProcess)) return true;
         if (type == typeof(NullSystemicProcess)) return true;
         if (type == typeof(CompoundProcessParameterMapping)) return true;
         if (type == typeof(MoleculeStartFormula)) return true;
         if (type == typeof(MoleculeParameter)) return true;
         if (type == typeof(SimulationPartialProcess)) return true;
         if (type == typeof(ExpressionResult)) return true;
         if (type == typeof(ExpressionContainerInfo)) return true;
         if (type == typeof(QueryExpressionResults)) return true;
         if (type == typeof(QueryExpressionSettings)) return true;
         if (type == typeof(OntogenyMetaData)) return true;
         if (type == typeof(DistributedParameterValue)) return true;
         if (type == typeof(QuantityValues)) return true;
         if (type == typeof(PopulationSimulationImport)) return true;
         if (type == typeof(GroupingItem)) return true;
         if (type == typeof(NullOntogeny)) return true;
         if (type == typeof(NullNumericField)) return true;
         if (type == typeof(NullOutputField)) return true;
         if (type == typeof(NullSimulation)) return true;
         if (type == typeof(NullParameter)) return true;

         //this type are always generated on the fly in PKSim and do not need to be serialized
         if (type == typeof(PKSimTransport)) return true;
         if (type == typeof(PKSimSpatialStructure)) return true;
         if (type == typeof(PKSimReaction)) return true;
         if (type == typeof(PKSimObserverBuilder)) return true;
         if (type == typeof(PKSimTransport)) return true;


         //help classes that are not stored in our domain objects
         if (type == typeof(NumericFieldContext)) return true;
         if (type == typeof(ParameterValueMetaData)) return true;
         if (type == typeof(ParameterMetaData)) return true;
         if (type == typeof(RateObjectPaths)) return true;
         if (type == typeof(NullObjectPaths)) return true;
         if (type == typeof(FloatMatrix)) return true;
         if (type == typeof(PivotResult)) return true;
         if (type == typeof(DynamicGroup)) return true;
         if (type == typeof(PKAnalysis)) return true;
         if (type == typeof(ParameterRateMetaData)) return true;
         if (type == typeof(RepresentationInfo)) return true;
         if (type == typeof(EmptyRepresentationInfo)) return true;
         if (type == typeof(NullDataRepository)) return true;
         if (type == typeof(ParameterDistributionMetaData)) return true;
         if (type == typeof(BinInterval)) return true;
         if (type == typeof(ContinuousDistributionData)) return true;
         if (type == typeof(DiscreteDistributionData)) return true;
         if (type == typeof(TransportDirection)) return true;
         if (type == typeof(TransportTemplate)) return true;

         //enum are created on the fly
         if (type == typeof(SimulationPartialProcessStatus)) return true;
         if (type == typeof(FormulaType)) return true;
         if (type == typeof(TemplateType)) return true;
         if (type == typeof(StatisticalAggregationType)) return true;
         if (type == typeof(RepresentationObjectType)) return true;
         if (type == typeof(TransportDirectionId)) return true;
         if (type == typeof(OrganType)) return true;
         if (type == typeof(InteractionType)) return true;
         if (type == typeof(ProtocolMode)) return true;
         if (type == typeof(BuildingBlockStatus)) return true;
         if (type == typeof(TemplateDatabaseType)) return true;
         if (type == typeof(MembraneLocation)) return true;
         if (type == typeof(ClassificationType)) return true;
         if (type == typeof(PopulationAnalysisType)) return true;
         if (type == typeof(Localization)) return true;
         if (type == typeof(PKSimContainerType)) return true;
         if (type == typeof(PlasmaProteinBindingPartner)) return true;
         if (type == typeof(PivotArea)) return true;
         if (type == typeof(CompoundType)) return true;
         if (type == typeof(DosingIntervalId)) return true;
         if (type == typeof(LabelGenerationStrategyId)) return true;
         if (type == typeof(SystemicProcessTypeId)) return true;
         if (type == typeof(CategoryType)) return true;
         if (type == typeof(ProcessActionType)) return true;
         if (type == typeof(LoadTemplateWithReference)) return true;

         //TODO not serialized yet
         if (type == typeof(QualificationPlan)) return true;
         if (type == typeof(RunParameterIdentificationQualificationStep)) return true;
         if (type == typeof(RunSimulationQualificationStep)) return true;

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
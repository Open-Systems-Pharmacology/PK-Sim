using System;
using System.Collections.Concurrent;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Chart.ParameterIdentifications;
using OSPSuite.Core.Chart.SensitivityAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Assets;

namespace PKSim.Core.Services
{
   public class ObjectTypeResolver : IObjectTypeResolver
   {
      //using a concurrent dictionary here as the service might be used by other services running in parallel
      private readonly ConcurrentDictionary<Type, string> _typeCache = new ConcurrentDictionary<Type, string>();

      public ObjectTypeResolver()
      {
         initializeCache();
      }

      private void initializeCache()
      {
         addToCache<Individual>(PKSimConstants.ObjectTypes.Individual);
         addToCache<IDistributedParameter>(PKSimConstants.ObjectTypes.DistributedParameter);
         addToCache<IParameter>(PKSimConstants.ObjectTypes.Parameter);
         addToCache<Compound>(PKSimConstants.ObjectTypes.Compound);
         addToCache<IProject>(PKSimConstants.ObjectTypes.Project);
         addToCache<Simulation>(PKSimConstants.ObjectTypes.Simulation);
         addToCache<EnzymaticProcess>(PKSimConstants.ObjectTypes.MetabolizingEnzyme);
         addToCache<Schema>(PKSimConstants.ObjectTypes.Schema);
         addToCache<ISchemaItem>(PKSimConstants.ObjectTypes.SchemaItem);
         addToCache<SimpleProtocol>(PKSimConstants.ObjectTypes.SimpleProtocol);
         addToCache<AdvancedProtocol>(PKSimConstants.ObjectTypes.AdvancedProtocol);
         addToCache<ParameterAlternative>(PKSimConstants.ObjectTypes.ParameterGroupAlternative);
         addToCache<Formulation>(PKSimConstants.ObjectTypes.Formulation);
         addToCache<IndividualEnzyme>(PKSimConstants.ObjectTypes.Enzyme);
         addToCache<IndividualProtein>(PKSimConstants.ObjectTypes.Protein);
         addToCache<RandomPopulation>(PKSimConstants.ObjectTypes.Population);
         addToCache<AdvancedParameter>(PKSimConstants.ObjectTypes.AdvancedParameter);
         addToCache<ParameterAlternativeGroup>(PKSimConstants.ObjectTypes.ParameterGroup);
         addToCache<SystemicProcess>(PKSimConstants.ObjectTypes.SystemicProcess);
         addToCache<Compartment>(PKSimConstants.ObjectTypes.Compartment);
         addToCache<Organ>(PKSimConstants.ObjectTypes.Organ);
         addToCache<Organism>(PKSimConstants.ObjectTypes.Organism);
         addToCache<IMoleculeAmount>(PKSimConstants.ObjectTypes.Molecule);
         addToCache<IndividualTransporter>(PKSimConstants.ObjectTypes.Transporter);
         addToCache<IndividualMolecule>(PKSimConstants.ObjectTypes.Molecule);
         addToCache<ISimulationSubject>(PKSimConstants.ObjectTypes.IndividualOrPopulation);
         addToCache<Population>(PKSimConstants.ObjectTypes.Population);
         addToCache<PKSimEvent>(PKSimConstants.ObjectTypes.Event);
         addToCache<Template>(PKSimConstants.ObjectTypes.Template);
         addToCache<TransportPartialProcess>(PKSimConstants.UI.TransportProtein);
         addToCache<SpecificBindingPartialProcess>(PKSimConstants.UI.SpecificBindingProcesses);
         addToCache<InhibitionProcess>(PKSimConstants.ObjectTypes.InhibitionProcess);
         addToCache<InductionProcess>(PKSimConstants.ObjectTypes.InductionProcess);
         addToCache<IEvent>(PKSimConstants.ObjectTypes.Event);
         addToCache<IEventGroup>(PKSimConstants.ObjectTypes.Event);
         addToCache<PopulationAnalysisGroupingField>(PKSimConstants.ObjectTypes.DerivedField);
         addToCache<PopulationAnalysisExpressionField>(PKSimConstants.ObjectTypes.ExpressionField);
         addToCache<PopulationAnalysisDerivedField>(PKSimConstants.ObjectTypes.DerivedField);
         addToCache<PopulationAnalysisPKParameterField>(PKSimConstants.ObjectTypes.PKParameterField);
         addToCache<PopulationAnalysisParameterField>(PKSimConstants.ObjectTypes.ParameterField);
         addToCache<PopulationAnalysisCovariateField>(PKSimConstants.ObjectTypes.ParameterField);
         addToCache<PopulationAnalysisOutputField>(PKSimConstants.ObjectTypes.OutputField);
         addToCache<PopulationAnalysis>(PKSimConstants.ObjectTypes.PopulationAnalysis);
         addToCache<ISimulationComparison>(PKSimConstants.UI.SimulationComparison);
         addToCache<Ontogeny>(PKSimConstants.ObjectTypes.Ontogeny);
         addToCache<PopulationAnalysisChart>(PKSimConstants.ObjectTypes.PopulationAnalysis);
         addToCache<CurveChart>(PKSimConstants.UI.TimeProfileAnalysis);
         addToCache<DataRepository>(PKSimConstants.ObjectTypes.ObservedData);
         addToCache<TimeProfileAnalysisChart>(PKSimConstants.UI.TimeProfileAnalysis);
         addToCache<ScatterAnalysisChart>(PKSimConstants.UI.ScatterAnalysis);
         addToCache<RangeAnalysisChart>(PKSimConstants.UI.RangeAnalysis);
         addToCache<BoxWhiskerAnalysisChart>(PKSimConstants.UI.BoxWhiskerAnalysis);
         addToCache<ISimulationSettings>(PKSimConstants.UI.SimulationSettings);
         addToCache<SimulationAnalysisWorkflow>(PKSimConstants.ObjectTypes.SimulationAnalysisWorkflow);
         addToCache<GenderRatio>(PKSimConstants.UI.GenderRatio);
         addToCache<ParameterRange>(PKSimConstants.ObjectTypes.Parameter);
         addToCache<RandomPopulationSettings>(PKSimConstants.UI.PopulationSettings);
         addToCache<ImportPopulationSettings>(PKSimConstants.UI.PopulationSettings);
         addToCache<OriginData>(PKSimConstants.UI.OriginData);
         addToCache<ParameterIdentificationCovarianceMatrix>(Captions.ParameterIdentification.CovarianceMatrix);
         addToCache<ParameterIdentificationPredictedVsObservedChart>(Captions.ParameterIdentification.PredictedVsObservedAnalysis);
         addToCache<ParameterIdentificationCorrelationMatrix>(Captions.ParameterIdentification.CorrelationMatrix);
         addToCache<ParameterIdentificationResidualHistogram>(Captions.ParameterIdentification.ResidualHistogramAnalysis);
         addToCache<ParameterIdentificationResidualVsTimeChart>(Captions.ParameterIdentification.ResidualsVsTimeAnalysis);
         addToCache<ParameterIdentificationTimeProfileConfidenceIntervalChart>(Captions.ParameterIdentification.TimeProfileConfidenceIntervalAnalysis);
         addToCache<ParameterIdentificationTimeProfilePredictionIntervalChart>(Captions.ParameterIdentification.TimeProfilePredictionIntervalAnalysis);
         addToCache<ParameterIdentificationTimeProfileVPCIntervalChart>(Captions.ParameterIdentification.TimeProfileVPCIntervalAnalysis);
         addToCache<ParameterIdentificationTimeProfileChart>(Captions.ParameterIdentification.TimeProfileAnalysis);
         addToCache<SensitivityAnalysisPKParameterAnalysis>(Captions.SensitivityAnalysis.SensitivityAnalysisPKParameterAnalysis);
         addToCache<ParameterIdentification>(ObjectTypes.ParameterIdentification);
         addToCache<ObserverSet>(PKSimConstants.ObjectTypes.ObserverSet);
         addToCache<IObserverBuilder>(ObjectTypes.ObserverBuilder);
      }

      private void addToCache<T>(string display)
      {
         _typeCache.TryAdd(typeof (T), display);
      }

      public string TypeFor<T>(T objectRequiringType) where T : class
      {
         if (objectRequiringType == null)
            return PKSimConstants.ObjectTypes.Unknown;

         return typeFor(objectRequiringType.GetType());
      }

      public string TypeFor<T>()
      {
         return typeFor(typeof (T));
      }

      private string typeFor(Type type)
      {
         //Can it be found in the list? return
         if (_typeCache.TryGetValue(type, out var display))
            return display;

         //This takes care of interface and implementation issues
         var firstPossibleType = _typeCache.Keys.ToList().FirstOrDefault(type.IsAnImplementationOf);
         if (firstPossibleType != null) 
            _typeCache.TryAdd(type, _typeCache[firstPossibleType]);

         //Can we now find it in the list? return
         if (_typeCache.TryGetValue(type, out display))
            return display;

         //nope.. let's return the name
         return type.Name;
      }
   }
}
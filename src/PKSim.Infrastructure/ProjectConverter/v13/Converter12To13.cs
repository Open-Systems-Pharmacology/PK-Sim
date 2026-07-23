using System;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using CoreConverter121To130 = OSPSuite.Core.Converters.v13.Converter121To130;

namespace PKSim.Infrastructure.ProjectConverter.v13
{
   /// <summary>
   ///    Brings projects saved before v13 in line with the new oral absorption and particle dissolution model. The model
   ///    added parameters and containers across most building blocks and changed the definition of the lumen parameters.
   ///    Everything is cloned from the templates the PK-Sim database provides; nothing is recreated by hand.
   /// </summary>
   public class Converter12To13 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<Compound>,
      IVisitor<Formulation>,
      IVisitor<PKSimEvent>,
      IVisitor<Simulation>
   {
      private readonly CoreConverter121To130 _coreConverter;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly IFormulationRepository _formulationRepository;
      private readonly IEventGroupRepository _eventGroupRepository;
      private readonly ITemplateStructureUpdater _templateStructureUpdater;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;
      private readonly IPopulationParameterValuesUpdater _populationParameterValuesUpdater;
      private readonly Lazy<Compound> _templateCompound;
      private bool _converted;

      public Converter12To13(
         CoreConverter121To130 coreConverter,
         ICompoundFactory compoundFactory,
         IDefaultIndividualRetriever defaultIndividualRetriever,
         IFormulationRepository formulationRepository,
         IEventGroupRepository eventGroupRepository,
         ITemplateStructureUpdater templateStructureUpdater,
         IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater,
         IPopulationParameterValuesUpdater populationParameterValuesUpdater)
      {
         _coreConverter = coreConverter;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _formulationRepository = formulationRepository;
         _eventGroupRepository = eventGroupRepository;
         _templateStructureUpdater = templateStructureUpdater;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
         _populationParameterValuesUpdater = populationParameterValuesUpdater;
         //Building a compound from the database is expensive and the template never varies, so it is built at most once
         _templateCompound = new Lazy<Compound>(compoundFactory.Create);
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V12;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         (_, _converted) = _coreConverter.Convert(objectToConvert);
         this.Visit(objectToConvert);
         return (ProjectVersions.V13, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         (_, bool converted) = _coreConverter.ConvertXml(element);
         return (ProjectVersions.V13, converted);
      }

      public void Visit(Individual individual) => convertIndividual(individual);

      public void Visit(Population population) => convertPopulation(population);

      public void Visit(Compound compound) => convertCompound(compound);

      public void Visit(Formulation formulation) => convertFormulation(formulation);

      public void Visit(PKSimEvent pkSimEvent) => convertEvent(pkSimEvent);

      public void Visit(Simulation simulation) => convertSimulation(simulation);

      private void convertSimulation(Simulation simulation)
      {
         if (simulation == null)
            return;

         //A simulation keeps its own copy of the building blocks it was created from, so they are converted as well
         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(simulation);

         convertIndividual(simulation.BuildingBlock<Individual>());
         convertPopulation(simulation.BuildingBlock<Population>());
         simulation.Compounds.Each(convertCompound);
         simulation.AllBuildingBlocks<Formulation>().Each(convertFormulation);
         simulation.AllBuildingBlocks<PKSimEvent>().Each(convertEvent);
      }

      private void convertPopulation(Population population)
      {
         if (population == null)
            return;

         //The individual has to catch up first: its new parameters are the ones the population needs values for
         convertIndividual(population.FirstIndividual);

         if (_populationParameterValuesUpdater.AddMissingParameterValuesTo(population))
            _converted = true;
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;

         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);

         var templateIndividual = templateFor(individual);
         if (templateIndividual == null)
            return;

         if (_templateStructureUpdater.AddMissingStructureTo(individual.Organism, templateIndividual.Organism))
            _converted = true;

         if (refreshLumenDefinitions(individual, templateIndividual))
            _converted = true;
      }

      /// <summary>
      ///    The lumen is where the new absorption model redefined existing parameters, turning constants into formulas and
      ///    into distributions. Everywhere else the old definitions still hold, so only the lumen is refreshed.
      /// </summary>
      private bool refreshLumenDefinitions(Individual individual, Individual templateIndividual) =>
         _templateStructureUpdater.RefreshParameterDefinitionsIn(lumenOf(individual), lumenOf(templateIndividual));

      private static IContainer lumenOf(Individual individual) =>
         individual.Organism.GetSingleChildByName<IContainer>(CoreConstants.Organ.LUMEN);

      private Individual templateFor(Individual individual)
      {
         //The new parameters are species dependent, so the template has to match the population of the individual
         var speciesPopulation = individual.OriginData?.Population;
         return speciesPopulation == null ? null : _defaultIndividualRetriever.DefaultIndividualFor(speciesPopulation);
      }

      private void convertCompound(Compound compound)
      {
         if (compound == null)
            return;

         var templateCompound = _templateCompound.Value;

         //Only the parameters defined directly in the compound are synchronized here. Its sub containers hold the
         //parameter alternatives, which are named by the user and must not be aligned with the template by name
         if (_templateStructureUpdater.AddMissingParametersTo(compound, templateCompound))
            _converted = true;

         convertCompoundAlternatives(compound, templateCompound);
      }

      /// <summary>
      ///    Some of the new compound parameters belong to a group that the user can define alternatives for, solubility
      ///    among them. Every alternative carries its own copy of the parameters of its group, so each of them has to be
      ///    brought up to date against the template alternative.
      /// </summary>
      private void convertCompoundAlternatives(Compound compound, Compound templateCompound)
      {
         foreach (var templateGroup in templateCompound.AllParameterAlternativeGroups())
         {
            var templateAlternative = templateGroup.AllAlternatives.FirstOrDefault();
            var compoundGroup = compound.ParameterAlternativeGroup(templateGroup.Name);
            if (templateAlternative == null || compoundGroup == null)
               continue;

            foreach (var alternative in compoundGroup.AllAlternatives)
            {
               if (_templateStructureUpdater.AddMissingParametersTo(alternative, templateAlternative))
                  _converted = true;
            }
         }
      }

      private void convertFormulation(Formulation formulation)
      {
         if (formulation == null)
            return;

         var templateFormulation = _formulationRepository.FormulationBy(formulation.FormulationType);
         if (_templateStructureUpdater.AddMissingParametersTo(formulation, templateFormulation))
            _converted = true;
      }

      private void convertEvent(PKSimEvent pkSimEvent)
      {
         if (pkSimEvent == null)
            return;

         if (removeObsoleteContainersFrom(pkSimEvent))
            _converted = true;

         var templateEventGroup = _eventGroupRepository.FindByName(pkSimEvent.TemplateName);
         //The new reset events are whole containers, so the event is synchronized recursively
         if (_templateStructureUpdater.AddMissingStructureTo(pkSimEvent, templateEventGroup))
            _converted = true;
      }

      private static bool removeObsoleteContainersFrom(PKSimEvent pkSimEvent)
      {
         var obsoleteContainers = pkSimEvent
            .GetAllChildren<IContainer>(x => x.IsNamed(ConverterConstants.Containers.MEAL_STOP_EVENT))
            .ToList();

         obsoleteContainers.Each(x => x.ParentContainer?.RemoveChild(x));

         return obsoleteContainers.Any();
      }
   }
}

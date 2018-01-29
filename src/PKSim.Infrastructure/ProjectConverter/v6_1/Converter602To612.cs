using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v6_1
{
   public class Converter602To612 : IObjectConverter,
      IVisitor<RandomPopulation>,
      IVisitor<PopulationSimulation>,
      IVisitor<Simulation>,
      IVisitor<Formulation>
   {
      private readonly IFormulationRepository _formulationRepository;
      private readonly ICloner _cloner;
      private bool _converted;

      public Converter602To612(IFormulationRepository formulationRepository, ICloner cloner)
      {
         _formulationRepository = formulationRepository;
         _cloner = cloner;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V6_0_2;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V6_1_2, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;

         if (element.Name == "RandomPopulationSettings")
         {
            convertRandomPopulationSettings(element, element.Element("FormulaCache"));
            _converted = true;
         }

         else if (element.Name == "RandomPopulation")
         {
            convertRandomPopulationSettings(element.Element("Settings"), element.Element("FormulaCache"));
            _converted = true;
         }

         element.DescendantsAndSelfNamed("ProteinExpressionContainer").ToList().Each(convertExpressionContainer);
         return (ProjectVersions.V6_1_2, _converted);
      }

      private void convertExpressionContainer(XElement proteinExpressionContainer)
      {
         proteinExpressionContainer.Name = "MoleculeExpressionContainer";
         _converted = true;
      }

      private void convertRandomPopulationSettings(XElement randomPopulationSettingsElement, XElement formulaCacheElement)
      {
         if (randomPopulationSettingsElement == null || formulaCacheElement == null)
            return;

         var baseIndividualElement = randomPopulationSettingsElement.Element("BaseIndividual");
         if (baseIndividualElement == null) return;

         formulaCacheElement.Remove();
         baseIndividualElement.Add(formulaCacheElement);
      }

      public void Visit(RandomPopulation randomPopulation)
      {
         convertRangeIn(randomPopulation);
         _converted = true;
      }

      private void convertRangeIn(RandomPopulation randomPopulation)
      {
         randomPopulation?.Settings.ParameterRanges.Each(convertRange);
      }

      private void convertRange(ParameterRange parameterRange)
      {
         parameterRange.MinValueInDisplayUnit = parameterRange.MinValue;
         parameterRange.MaxValueInDisplayUnit = parameterRange.MaxValue;
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         Visit(populationSimulation.DowncastTo<Simulation>());
         convertRangeIn(populationSimulation.Population as RandomPopulation);
         _converted = true;
      }

      public void Visit(Simulation simulation)
      {
         clearFormulaCacheInReactionBuildingBlock(simulation.Reactions);
         updateMoleculeAmountNegativeValuesAllowed(simulation.Model.Root);
         simulation.AllBuildingBlocks<Formulation>().Each(Visit);
         _converted = true;
      }

      private void updateMoleculeAmountNegativeValuesAllowed(IContainer container)
      {
         container.GetAllChildren<IMoleculeAmount>(x => x.NegativeValuesAllowed = true);
      }

      private void clearFormulaCacheInReactionBuildingBlock(IReactionBuildingBlock reactions)
      {
         if (reactions == null)
            return;

         reactions.FormulaCache.Clear();
         foreach (var reaction in reactions.Where(x => x.Formula != null))
         {
            reactions.FormulaCache.Add(reaction.Formula);
         }
      }

      public void Visit(Formulation formulation)
      {
         if (!formulation.FormulationType.IsOneOf(CoreConstants.Formulation.Weibull, CoreConstants.Formulation.Lint80, CoreConstants.Formulation.Particles, CoreConstants.Formulation.Table))
            return;

         var templateFormulation = _formulationRepository.FormulationBy(formulation.FormulationType);
         var useAsSuspension = _cloner.Clone(templateFormulation.Parameter(CoreConstants.Parameters.USE_AS_SUSPENSION));
         useAsSuspension.Value = 0;
         formulation.Add(useAsSuspension);
         _converted = true;
      }
   }
}
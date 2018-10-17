using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.ProjectConverter.v7_4
{
   public class Converter730To740 : IObjectConverter,
      IVisitor<Simulation>,
      IVisitor<Individual>,
      IVisitor<Population>
   {
      private bool _converted;

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_3_0;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V7_4_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V7_4_0, false);
      }

      public void Visit(Simulation simulation)
      {
         convertSimulation(simulation);
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
      }

      public void Visit(Population population)
      {
         convertPopulation(population);
      }

      private void convertPopulation(Population population)
      {
         convertIndividual(population.FirstIndividual);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null) return;
         individual.AllDefinedMolecules().Each(updateOntogenyFactorsInContainer);
         updatePlasmaProteinOntogenyFactorInContainer(individual.Organism);
      }

      private void convertSimulation(Simulation simulation)
      {
         var allTabletTimeDelayFactorParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameter.TabletTimeDelayFactor));
         allTabletTimeDelayFactorParameters.Each(convertTabletTimeDelayFactorParameter);

         var allSimulationMolecules = simulation.Model.Root.GetChildren<IContainer>(c => c.ContainerType == ContainerType.Molecule);
         allSimulationMolecules.Each(updateOntogenyFactorsInContainer);

         updatePlasmaProteinOntogenyFactorInContainer(simulation.Model.Root.Container(Constants.ORGANISM));

         convertIndividual(simulation.BuildingBlock<Individual>());
      }

      private void updatePlasmaProteinOntogenyFactorInContainer(IContainer container)
      {
         updateOntogenyFactor(container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN));
         updateOntogenyFactor(container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP));
      }

      private void updateOntogenyFactorsInContainer(IContainer container)
      {
         updateOntogenyFactor(container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR));
         updateOntogenyFactor(container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_GI));
      }

      private void updateOntogenyFactor(IParameter ontogenyFactorParameter)
      {
         if (ontogenyFactorParameter == null)
            return;

         ontogenyFactorParameter.DefaultValue = ontogenyFactorParameter.Value;
         ontogenyFactorParameter.Editable = true;
         ontogenyFactorParameter.Visible = true;
         _converted = true;
      }

      private void convertTabletTimeDelayFactorParameter(IParameter parameter)
      {
         parameter.Visible = true;
         parameter.Editable = true;
         //just to make sure it's the last parmaeter visible 
         parameter.Sequence = 20;
         _converted = true;
      }
   }
}
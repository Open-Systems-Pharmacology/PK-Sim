using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Services
{
   public interface IExpressionContainersRetriever
   {
      /// <summary>
      ///    Returns the container in the structure where the molecule will be available
      /// </summary>
      IEnumerable<IContainer> AllContainersFor(ISpatialStructure spatialStructure, IndividualMolecule molecule);

      /// <summary>
      ///    Returns all possible containers of the organism in which a given protein will be defined
      /// </summary>
      IEnumerable<IContainer> AllContainersFor(Organism organism, IndividualMolecule molecule);

      /// <summary>
      ///    Returns all possible containers of the organism represented by the given protein expression container
      ///    (e.g one compartment unless the container is blood or plamsa. In that case, all available blood and plasma
      ///    compartment are retrieved)
      /// </summary>
      IEnumerable<IContainer> AllContainersFor(Organism organism, IEnumerable<IContainer> allOrganismContainers, IndividualMolecule molecule, MoleculeExpressionContainer expressionContainer);

      /// <summary>
      ///    Returns the container in organsim where a molecule can possibly be defined
      /// </summary>
      IEnumerable<IContainer> AllOrganismContainers(Organism organism);

      /// <summary>
      ///    Returns the container in the <paramref name="individual" /> where a molecule can possibly be defined
      /// </summary>
      IEnumerable<IContainer> AllOrganismContainers(Individual individual);

      /// <summary>
      ///    Returns the name of the container where the reaction or transport will be defined
      /// </summary>
      string RelativeExpressionContainerNameFrom(IndividualMolecule individualMolecule, IContainer relativeExpressionContainer);
   }

   public class ExpressionContainersRetriever : IExpressionContainersRetriever
   {
      public IEnumerable<IContainer> AllContainersFor(ISpatialStructure spatialStructure, IndividualMolecule molecule)
      {
         return AllContainersFor(spatialStructure.DowncastTo<IPKSimSpatialStructure>().Organism, molecule);
      }

      public IEnumerable<IContainer> AllContainersFor(Organism organism, IndividualMolecule molecule)
      {
         var allContainers = AllOrganismContainers(organism).ToList();
         var allContainersForProtein = new List<IContainer>();
         foreach (var expressionContainer in molecule.AllExpressionsContainers())
         {
            allContainersForProtein.AddRange(allContainersFor(organism, molecule, allContainers, expressionContainer));
         }
         return allContainersForProtein.Distinct();
      }

      public IEnumerable<IContainer> AllOrganismContainers(Organism organism)
      {
         var allPhysicalContainers = organism.GetAllChildren<IContainer>(x => x.Mode == ContainerMode.Physical).ToList();
         removeEndogenousIgGContainers(allPhysicalContainers, organism);
         return allPhysicalContainers;
      }

      private void removeEndogenousIgGContainers(List<IContainer> allPhysicalContainers, Organism organism)
      {
         removePhysicalChildrenFrom(allPhysicalContainers, organism, CoreConstants.Organ.EndogenousIgG);
      }

      private void removePhysicalChildrenFrom(List<IContainer> allPhysicalContainers, Organism organism, string organName)
      {
         var organ = organism.Organ(organName);
         if (organ == null) return;

         organ.GetChildren<IContainer>(x => x.Mode == ContainerMode.Physical)
            .Each(c => allPhysicalContainers.Remove(c));
      }

      public IEnumerable<IContainer> AllOrganismContainers(Individual individual)
      {
         if (individual == null)
            return Enumerable.Empty<IContainer>();

         return AllOrganismContainers(individual.Organism);
      }

      public IEnumerable<IContainer> AllContainersFor(Organism organism, IEnumerable<IContainer> allOrganismContainers, IndividualMolecule molecule, MoleculeExpressionContainer expressionContainer)
      {
         return allContainersFor(organism, molecule, allOrganismContainers, expressionContainer);
      }

      private IEnumerable<IContainer> allContainersFor(Organism organism, IndividualMolecule molecule, IEnumerable<IContainer> allContainers, MoleculeExpressionContainer expressionContainer)
      {
         if (molecule.MoleculeType == QuantityType.Transporter)
            return allContainersForTransporter(organism, molecule.DowncastTo<IndividualTransporter>(), allContainers, expressionContainer.DowncastTo<TransporterExpressionContainer>());

         var protein = molecule.DowncastTo<IndividualProtein>();
         //plasma always generated
         if (expressionContainer.IsPlasma())
            return allPlasmaCompartmentsOf(allContainers);

         switch (protein.TissueLocation)
         {
            case TissueLocation.ExtracellularMembrane:
               if (expressionContainer.IsBloodCell())
                  return allPlasmaCompartmentsOf(allContainers);
               if (expressionContainer.IsVascularEndothelium())
               {
                  if (protein.MembraneLocation == MembraneLocation.Apical)
                     return allPlasmaCompartmentsOf(allContainers);

                  return allInterstitialCompartmentsOf(allContainers);
               }
               return compartmentFor(organism, expressionContainer, CoreConstants.Compartment.Interstitial);

            case TissueLocation.Intracellular:
               if (expressionContainer.IsBloodCell())
                  return allBloodCellsCompartmentsOf(allContainers);

               if (expressionContainer.IsVascularEndothelium())
               {
                  if (protein.IntracellularVascularEndoLocation == IntracellularVascularEndoLocation.Endosomal)
                     return allEndosomeCompartmentsOf(allContainers);

                  return allInterstitialCompartmentsOf(allContainers);
               }
               return compartmentFor(organism, expressionContainer, CoreConstants.Compartment.Intracellular);

            case TissueLocation.Interstitial:
               if (expressionContainer.IsBloodCell())
                  return allBloodCellsCompartmentsOf(allContainers);

               if (expressionContainer.IsVascularEndothelium())
                  return allInterstitialCompartmentsOf(allContainers);

               return compartmentFor(organism, expressionContainer, CoreConstants.Compartment.Interstitial);
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public string RelativeExpressionContainerNameFrom(IndividualMolecule individualMolecule, IContainer relativeExpressionContainer)
      {
         //Name of container is name of the compartment where the reaction resides (not for transporter as transporter can actually take place
         //in local compartment plasma)
         if (relativeExpressionContainer.IsSurrogate() && !individualMolecule.IsAnImplementationOf<IndividualTransporter>())
         {
            if (relativeExpressionContainer.IsEndosome())
               return CoreConstants.Compartment.VascularEndothelium;

            return relativeExpressionContainer.Name;
         }

         //for lumen, we return the name of the organ with the name of the segment
         if (relativeExpressionContainer.ParentContainer.IsLumen())
            return CoreConstants.ContainerName.LumenSegmentNameFor(relativeExpressionContainer.Name);


         //for non blood cell , plasma compartment or segments, we need to go up to the organ
         return relativeExpressionContainer.ParentContainer.Name;
      }

      private IEnumerable<IContainer> compartmentFor(Organism organism, MoleculeExpressionContainer expressionContainer, string defaultCompartment)
      {
         var usedCompartment = expressionContainer.IsLumen ? expressionContainer.ContainerName : defaultCompartment;
         var compartment = expressionContainer.CompartmentPath(usedCompartment).Resolve<IContainer>(organism);
         if (compartment == null)
            return Enumerable.Empty<IContainer>();
         return new[] {compartment};
      }

      private IEnumerable<IContainer> allPlasmaCompartmentsOf(IEnumerable<IContainer> allContainers)
      {
         return allCompartmentsOfNames(allContainers, CoreConstants.Compartment.Plasma);
      }

      private IEnumerable<IContainer> allInterstitialCompartmentsOf(IEnumerable<IContainer> allContainers)
      {
         return allCompartmentsOfNames(allContainers, CoreConstants.Compartment.Interstitial);
      }

      private IEnumerable<IContainer> allBloodCellsCompartmentsOf(IEnumerable<IContainer> allContainers)
      {
         return allCompartmentsOfNames(allContainers, CoreConstants.Compartment.BloodCells);
      }

      private IEnumerable<IContainer> allEndosomeCompartmentsOf(IEnumerable<IContainer> allContainers)
      {
         return allCompartmentsOfNames(allContainers, CoreConstants.Compartment.Endosome);
      }

      private IEnumerable<IContainer> allCompartmentsOfNames(IEnumerable<IContainer> allContainers, string name)
      {
         return allContainers.Where(c => c.Name.Equals(name));
      }

      private IEnumerable<IContainer> allContainersForTransporter(Organism organism, IndividualTransporter transporter, IEnumerable<IContainer> allContainers, TransporterExpressionContainer expressionContainer)
      {
         if (expressionContainer.IsSurrogate())
            return allCompartmentsOfNames(allContainers, expressionContainer.Name);

         return compartmentFor(organism, expressionContainer, expressionContainer.CompartmentName);
      }
   }
}
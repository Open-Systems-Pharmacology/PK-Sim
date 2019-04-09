using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Assets;
using static PKSim.Core.CoreConstants;
using static PKSim.Core.CoreConstants.Observer;
using ContainerType = OSPSuite.Core.Domain.ContainerType;

namespace PKSim.Presentation.DTO.Mappers
{
   public class PKSimPathToPathElementsMapper : PathToPathElementsMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly Regex _fractionOfDoseLiverRegex;
      private const string OBSERVER_NAME = "observerName";

      public PKSimPathToPathElementsMapper(IRepresentationInfoRepository representationInfoRepository, IEntityPathResolver entityPathResolver) : base(entityPathResolver)
      {
         _representationInfoRepository = representationInfoRepository;
         //Format is (Fraction of dose-DRUG_NAME)-Liver-COMPARTMENT
         var fractionOfDoseLiverObserverPattern = $@"(?<{OBSERVER_NAME}>{FRACTION_OF_DOSE}{COMPOSITE_SEPARATOR}\w*){COMPOSITE_SEPARATOR}{CoreConstants.Organ.Liver}{COMPOSITE_SEPARATOR}\w*";
         _fractionOfDoseLiverRegex = new Regex(fractionOfDoseLiverObserverPattern);
      }

      protected override string DisplayNameFor(IContainer container)
      {
         return _representationInfoRepository.DisplayNameFor(container);
      }

      protected override PathElementDTO CreatePathElementDTO(IContainer container)
      {
         var pathElementDTO = base.CreatePathElementDTO(container);
         var representationInfo = _representationInfoRepository.InfoFor(container);
         representationInfo.UpdatePathElement(pathElementDTO);
         return pathElementDTO;
      }

      protected override PathElementDTO CreateContainerPath(IReadOnlyList<IContainer> containerList)
      {
         if (containerList.Count == 0)
            return base.CreateContainerPath(containerList);

         //This is a parameter in an event group hierarchy
         if (containerList.Last().ContainerType == ContainerType.EventGroup)
            return base.CreatePathElementDTO(containerList.Last());

         //This is a parameter in a formulation
         if (containerList.First().ContainerType == ContainerType.EventGroup)
            return CreatePathElementDTO(containerList.First());

         return CreatePathElementDTO(containerList.Last());
      }

      protected override PathElements CreatePathElementsFrom(IReadOnlyList<IContainer> containers, string name)
      {
         var pathElements = base.CreatePathElementsFrom(containers, name);
         var lastContainer = containers.LastOrDefault();

         if (lastContainer == null)
            return pathElements;

         var quantity = lastContainer.EntityAt<IQuantity>(name);
         if (quantity == null)
            return pathElements;

         //special treatment for PKSim objects
         if (quantity.IsInNeighborhood())
            adjustDisplayPathForNeighborhood(pathElements, quantity);

         var parameter = quantity as IParameter;
         if (parameter != null)
            adjustDisplayPathForParameter(pathElements, parameter);
         else
            adjustDisplayPathForQuantity(pathElements, quantity);

         updateMoleculeIcon(pathElements);
         return pathElements;
      }

      private void updateMoleculeIcon(PathElements pathElements)
      {
         if (!pathElements.Contains(PathElement.Molecule))
            return;

         var moleculePathElement = pathElements[PathElement.Molecule];
         if (ApplicationIcons.HasIconNamed(moleculePathElement.IconName))
            return;

         moleculePathElement.IconName = ApplicationIcons.Drug.IconName;
      }

      private void adjustDisplayPathForParameter(PathElements pathElements, IParameter parameter)
      {
         if (parameter.IsInMucosa())
            adjustDisplayPathForParameterInSubSegment(pathElements, parameter, x => x.IsSegment(), PKSimConstants.ObjectTypes.Mucosa);
         else
            adjustDisplayPathForParameterInSubSegment(pathElements, parameter, x => x.IsLiverZone(), PKSimConstants.ObjectTypes.Organs);

         pathElements[PathElement.Name].DisplayName = _representationInfoRepository.DisplayNameFor(parameter);
      }

      private void adjustDisplayPathForParameterInSubSegment(PathElements pathElements, IParameter parameter, Func<IContainer, bool> containerCondition, string groupingName)
      {
         pathElements.Category = groupingName;
         if (containerCondition(parameter.ParentContainer))
            swapCompartmentWithContainer(pathElements);
      }

      private static void swapCompartmentWithContainer(PathElements pathElements)
      {
         if (!pathElements.Contains(PathElement.BottomCompartment))
            return;

         pathElements[PathElement.Container] = pathElements[PathElement.BottomCompartment];
         pathElements.Remove(PathElement.BottomCompartment);
      }

      private void adjustDisplayPathForQuantity(PathElements pathElements, IQuantity quantity)
      {
         if (quantity.HasAncestorNamed(CoreConstants.Organ.Gallbladder))
            adjustDisplayPathForGallBladder(pathElements, quantity);

         else if (quantity.HasAncestorNamed(CoreConstants.Organ.Lumen))
            adjustDisplayPathForLumen(pathElements, quantity);

         else if (quantity.IsNamed(PLASMA_UNBOUND))
            adjustDisplayPathForContainerObserver(pathElements, quantity);

         else if (quantity.Name.StartsWith(TOTAL_FRACTION_OF_DOSE))
            adjustDisplayPathForTotalFractionOfDose(pathElements, quantity);

         //Container observers directly in a container located under organism
         else if (!pathElements.Contains(PathElement.BottomCompartment))
            adjustDisplayPathForContainerObserver(pathElements, quantity);

         else if (quantity.IsAnImplementationOf<IObserver>())
            adjustDisplayNameForMoleculeObserver(pathElements, quantity.DowncastTo<IObserver>());
      }

      private void adjustDisplayPathForTotalFractionOfDose(PathElements pathElements, IQuantity quantity)
      {
         pathElements[PathElement.Container] = pathElements[PathElement.TopContainer];
         pathElements[PathElement.TopContainer] = new PathElementDTO();
      }

      private void adjustDisplayNameForMoleculeObserver(PathElements pathElements, IObserver observer)
      {
         //For all fraction observers, the name should remain as is except for liver zone observers that need to be rename explicitly
         if (observerIsFractionOfDoseLiver(observer))
            updateNameElementForFractionOfDose(pathElements, observer);

         else if (!FractionObservers.Any(observer.Name.StartsWith))
            updateNameElementToQuantityDimensionName(pathElements, observer);
      }

      private void updateNameElementForFractionOfDose(PathElements pathElements, IObserver observer)
      {
         var observerName = _fractionOfDoseLiverRegex.Matches(observer.Name)[0].Groups[OBSERVER_NAME].Value;
         pathElements[PathElement.Name] = CreatePathElementDTO(observerName);
      }

      private bool observerIsFractionOfDoseLiver(IObserver observer) => _fractionOfDoseLiverRegex.IsMatch(observer.Name);

      private void adjustDisplayPathForNeighborhood(PathElements pathElements, IQuantity quantity)
      {
         var neighborhood = quantity.NeighborhoodAncestor();
         pathElements[PathElement.Container] = CreatePathElementDTO(neighborhood.FirstNeighbor.ParentContainer);
      }

      private void adjustDisplayPathForLumen(PathElements pathElements, IQuantity quantity)
      {
         if (quantityIsConcentration(quantity))
            updateNameElementToQuantityDimensionName(pathElements, quantity);
      }

      private void adjustDisplayPathForGallBladder(PathElements pathElements, IQuantity quantity)
      {
         if (quantity.IsNamed(FRACTION_EXCRETED_TO_BILE))
            return;

         adjustDisplayPathForContainerObserver(pathElements, quantity);
      }

      private void adjustDisplayPathForContainerObserver(PathElements pathElements, IQuantity quantity)
      {
         updateCompartmentElementToQuantityDisplayName(pathElements, quantity);
         updateNameElementToQuantityDimensionName(pathElements, quantity);
      }

      private void updateCompartmentElementToQuantityDisplayName(PathElements pathElements, IQuantity quantity)
      {
         pathElements[PathElement.BottomCompartment] = CreatePathElementDTO(_representationInfoRepository.DisplayNameFor(quantity));
      }

      private void updateNameElementToQuantityDimensionName(PathElements pathElements, IQuantity quantity)
      {
         pathElements[PathElement.Name] = CreatePathElementDTO(dimensionDisplayNameFor(quantity));
      }

      private string dimensionDisplayNameFor(IQuantity quantity)
      {
         if (quantity.Dimension == null)
            return quantity.Name;

         if (quantity.Dimension.Name == Dimension.Fraction)
            return Output.FractionDose;

         if (quantity.Dimension.Name.IsOneOf(Dimension.Mass, Constants.Dimension.AMOUNT))
            return Output.Amount;

         if (quantityIsConcentration(quantity))
            return Output.Concentration;

         return quantity.Dimension.DisplayName;
      }

      private bool quantityIsConcentration(IQuantity quantity)
      {
         return quantity.Dimension != null &&
                quantity.Dimension.Name.IsOneOf(Dimension.MASS_CONCENTRATION, Constants.Dimension.MOLAR_CONCENTRATION);
      }
   }
}
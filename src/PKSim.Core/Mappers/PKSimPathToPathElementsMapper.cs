using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using ContainerExtensions = PKSim.Core.Model.ContainerExtensions;
using ContainerType = OSPSuite.Core.Domain.ContainerType;

namespace PKSim.Core.Mappers
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
         var fractionOfDoseLiverObserverPattern = $@"(?<{OBSERVER_NAME}>{CoreConstants.Observer.FRACTION_OF_DOSE}{CoreConstants.COMPOSITE_SEPARATOR}\w*){CoreConstants.COMPOSITE_SEPARATOR}{CoreConstants.Organ.LIVER}{CoreConstants.COMPOSITE_SEPARATOR}\w*";
         _fractionOfDoseLiverRegex = new Regex(fractionOfDoseLiverObserverPattern);
      }

      protected override string DisplayNameFor(IContainer container)
      {
         return _representationInfoRepository.DisplayNameFor(container);
      }

      protected override PathElement CreatePathElement(IContainer container)
      {
         var pathElementDTO = base.CreatePathElement(container);
         var representationInfo = _representationInfoRepository.InfoFor(container);
         representationInfo.UpdatePathElement(pathElementDTO);
         return pathElementDTO;
      }

      protected override PathElement CreateContainerPath(IReadOnlyList<IContainer> containerList)
      {
         if (containerList.Count == 0)
            return base.CreateContainerPath(containerList);

         //This is a parameter in an event group hierarchy
         if (containerList.Last().ContainerType == ContainerType.EventGroup)
            return base.CreatePathElement(containerList.Last());

         //This is a parameter in a formulation
         if (containerList.First().ContainerType == ContainerType.EventGroup)
            return CreatePathElement(containerList.First());

         return CreatePathElement(containerList.Last());
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
         if (!pathElements.Contains(PathElementId.Molecule))
            return;

         var moleculePathElement = pathElements[PathElementId.Molecule];
         if (ApplicationIcons.HasIconNamed(moleculePathElement.IconName))
            return;

         moleculePathElement.IconName = ApplicationIcons.Drug.IconName;
      }

      private void adjustDisplayPathForParameter(PathElements pathElements, IParameter parameter)
      {
         if (parameter.IsInMucosa())
            adjustDisplayPathForParameterInSubSegment(pathElements, parameter, x => ContainerExtensions.IsSegment(x), PKSimConstants.ObjectTypes.Mucosa);
         else
            adjustDisplayPathForParameterInSubSegment(pathElements, parameter, x => ContainerExtensions.IsLiverZone(x), PKSimConstants.ObjectTypes.Organs);

         pathElements[PathElementId.Name].DisplayName = _representationInfoRepository.DisplayNameFor(parameter);
      }

      private void adjustDisplayPathForParameterInSubSegment(PathElements pathElements, IParameter parameter, Func<IContainer, bool> containerCondition, string groupingName)
      {
         pathElements.Category = groupingName;
         if (containerCondition(parameter.ParentContainer))
            swapCompartmentWithContainer(pathElements);
      }

      private static void swapCompartmentWithContainer(PathElements pathElements)
      {
         if (!pathElements.Contains(PathElementId.BottomCompartment))
            return;

         pathElements[PathElementId.Container] = pathElements[PathElementId.BottomCompartment];
         pathElements.Remove(PathElementId.BottomCompartment);
      }

      private void adjustDisplayPathForQuantity(PathElements pathElements, IQuantity quantity)
      {
         if (quantity.HasAncestorNamed(CoreConstants.Organ.GALLBLADDER))
            adjustDisplayPathForGallBladder(pathElements, quantity);

         else if (quantity.HasAncestorNamed(CoreConstants.Organ.LUMEN))
            adjustDisplayPathForLumen(pathElements, quantity);

         else if (quantity.IsNamed(CoreConstants.Observer.PLASMA_UNBOUND))
            adjustDisplayPathForContainerObserver(pathElements, quantity);

         else if (quantity.Name.StartsWith(CoreConstants.Observer.TOTAL_FRACTION_OF_DOSE))
            adjustDisplayPathForTotalFractionOfDose(pathElements, quantity);

         //Container observers directly in a container located under organism
         else if (!pathElements.Contains(PathElementId.BottomCompartment))
            adjustDisplayPathForContainerObserver(pathElements, quantity);

         else if (quantity.IsAnImplementationOf<IObserver>())
            adjustDisplayNameForMoleculeObserver(pathElements, quantity.DowncastTo<IObserver>());
      }

      private void adjustDisplayPathForTotalFractionOfDose(PathElements pathElements, IQuantity quantity)
      {
         pathElements[PathElementId.Container] = pathElements[PathElementId.TopContainer];
         pathElements[PathElementId.TopContainer] = new PathElement();
      }

      private void adjustDisplayNameForMoleculeObserver(PathElements pathElements, IObserver observer)
      {
         //For all fraction observers, the name should remain as is except for liver zone observers that need to be rename explicitly
         if (observerIsFractionOfDoseLiver(observer))
            updateNameElementForFractionOfDose(pathElements, observer);

         // These observers should be renamed to their dimension (concentration).
         else if (observer.Name.StartsWith(CoreConstants.Observer.CONCENTRATION_IN_CONTAINER))
            updateNameElementToQuantityDimensionName(pathElements, observer);
      }

      private void updateNameElementForFractionOfDose(PathElements pathElements, IObserver observer)
      {
         var observerName = _fractionOfDoseLiverRegex.Matches(observer.Name)[0].Groups[OBSERVER_NAME].Value;
         pathElements[PathElementId.Name] = CreatePathElement(observerName);
      }

      private bool observerIsFractionOfDoseLiver(IObserver observer) => _fractionOfDoseLiverRegex.IsMatch(observer.Name);

      private void adjustDisplayPathForNeighborhood(PathElements pathElements, IQuantity quantity)
      {
         var neighborhood = quantity.NeighborhoodAncestor();
         pathElements[PathElementId.Container] = CreatePathElement(neighborhood.FirstNeighbor.ParentContainer);
      }

      private void adjustDisplayPathForLumen(PathElements pathElements, IQuantity quantity)
      {
         if (quantityIsConcentration(quantity))
            updateNameElementToQuantityDimensionName(pathElements, quantity);
      }

      private void adjustDisplayPathForGallBladder(PathElements pathElements, IQuantity quantity)
      {
         if (quantity.IsNamed(CoreConstants.Observer.FRACTION_EXCRETED_TO_BILE))
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
         pathElements[PathElementId.BottomCompartment] = CreatePathElement(_representationInfoRepository.DisplayNameFor(quantity));
      }

      private void updateNameElementToQuantityDimensionName(PathElements pathElements, IQuantity quantity)
      {
         pathElements[PathElementId.Name] = CreatePathElement(dimensionDisplayNameFor(quantity));
      }

      private string dimensionDisplayNameFor(IQuantity quantity)
      {
         if (quantity.Dimension == null)
            return quantity.Name;

         if (quantity.Dimension.Name == CoreConstants.Dimension.Fraction)
            return CoreConstants.Output.FractionDose;

         if (quantity.Dimension.Name.IsOneOf(Constants.Dimension.MASS_AMOUNT, Constants.Dimension.MOLAR_AMOUNT))
            return CoreConstants.Output.Amount;

         if (quantityIsConcentration(quantity))
            return CoreConstants.Output.Concentration;

         return quantity.Dimension.DisplayName;
      }

      private bool quantityIsConcentration(IQuantity quantity)
      {
         return quantity.Dimension != null &&
                quantity.Dimension.Name.IsOneOf(CoreConstants.Dimension.MASS_CONCENTRATION, Constants.Dimension.MOLAR_CONCENTRATION);
      }
   }
}
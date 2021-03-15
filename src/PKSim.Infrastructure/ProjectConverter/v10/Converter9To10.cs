using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Converters.v10;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.ProjectConverter.v10
{
   public class Converter9To10 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<Simulation>,
      IVisitor<ParameterIdentification>

   {
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ICloner _cloner;
      private readonly Converter90To100 _coreConverter90To100;
      private bool _converted;

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V9;

      public Converter9To10(
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IDefaultIndividualRetriever defaultIndividualRetriever,
         ICloner cloner,
         Converter90To100 coreConverter90To100
      )
      {
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
         _coreConverter90To100 = coreConverter90To100;
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         element.DescendantsAndSelf("Individual").Each(convertIndividualProteinsIn);
         element.DescendantsAndSelf("BaseIndividual").Each(convertIndividualProteinsIn);
         element.DescendantsAndSelf("UserSettings").Each(convertUserSettings);

         return (ProjectVersions.V10, _converted);
      }

      private void convertUserSettings(XElement settingsElement)
      {
         settingsElement.SetAttributeValue("disabledColor", PKSimColors.Disabled.ToArgb().ToString());
         _converted = true;
      }

      private void convertIndividualProteinsIn(XElement individualElement)
      {
         individualElement.Descendants("IndividualEnzyme").Each(convertIndividualProtein);
         individualElement.Descendants("IndividualOtherProtein").Each(convertIndividualProtein);
         individualElement.Descendants("IndividualTransporter").Each(convertIndividualTransporter);
      }

      private void convertIndividualProtein(XElement proteinNode)
      {
         var membraneLocation = EnumHelper.ParseValue<MembraneLocation>(proteinNode.GetAttribute("membraneLocation"));
         var tissueLocation = EnumHelper.ParseValue<TissueLocation>(proteinNode.GetAttribute("tissueLocation"));
         var intracellularVascularEndoLocation =
            EnumHelper.ParseValue<IntracellularVascularEndoLocation>(proteinNode.GetAttribute("intracellularVascularEndoLocation"));

         var localization = LocalizationConverter.ConvertToLocalization(tissueLocation, membraneLocation, intracellularVascularEndoLocation);
         proteinNode.AddAttribute("localization", localization.ToString());
         _converted = true;
      }

      private void convertIndividualTransporter(XElement transporterNode)
      {
         transporterNode.Descendants("TransporterExpressionContainer").Each(transporterContainerNode =>
         {
            // membraneLocation is not used anymore. We will save it temporarily as Tag so that we can access it during object conversion
            var membraneLocation = EnumHelper.ParseValue<MembraneLocation>(transporterContainerNode.GetAttribute("membraneLocation"));
            var tag = new XElement("Tag");
            tag.SetAttributeValue("value", membraneLocation);
            transporterContainerNode.Add(new XElement("Tags", tag));
         });
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V10, _converted);
      }

      public void Visit(Individual individual) => convertIndividual(individual);

      public void Visit(Population population) => convertIndividual(population.FirstIndividual);

      public void Visit(Simulation simulation) => convertIndividual(simulation.BuildingBlock<Individual>());

      public void Visit(ParameterIdentification parameterIdentification)
      {
         (_, _converted) = _coreConverter90To100.Convert(parameterIdentification);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;

         addFractionEndosomalParametersTo(individual);

         //Use to list here as the collection will be modified during iteration
         individual.AllMolecules().ToList().Each(x => convertMolecule(x, individual));
         _converted = true;
      }

      private void addFractionEndosomalParametersTo(Individual individual)
      {
         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var allFractionEndosomalParameters =
            defaultHuman.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameters.FRACTION_ENDOSOMAL));

         allFractionEndosomalParameters.Each(x =>
         {
            //Find container in the individual to convert
            var containerPath = new ObjectPath(x.ParentContainer.ConsolidatedPath());

            var container = individual.Root.EntityAt<IContainer>(containerPath);
            //add clone of parameter
            container.Add(_cloner.Clone(x));
         });
      }

      private void convertMolecule(IndividualMolecule moleculeToConvert, Individual individual)
      {
         //Complete reorganization of molecules in the new version. We need to update value of relative expression.
         //We know that localization was already updated during xml conversion
         var factory = _individualMoleculeFactoryResolver.FactoryFor(moleculeToConvert);

         //Remove from individual so that we can add it again. 
         individual.RemoveMolecule(moleculeToConvert);

         //New molecules is added. We need to update all global parameters as well as relative expression parameters
         var newMolecule = factory.AddMoleculeTo(individual, moleculeToConvert.Name);

         if (newMolecule == null)
            return;

         var allExpressionParameters = individual.AllExpressionParametersFor(newMolecule);
         moleculeToConvert.GetAllChildren<MoleculeExpressionContainer>().Each(x =>
         {
            convertParameter(x.RelativeExpressionParameter, allExpressionParameters[x.Name]);
         });

         moleculeToConvert.AllParameters().Each(p => { convertParameter(p, newMolecule.Parameter(p.Name)); });

         if (!(newMolecule is IndividualTransporter transporter))
            return;

         var allTransporterExpressionContainers = individual.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter)
            .Where(x => x.TransportDirection != TransportDirectionId.None).ToList();

         moleculeToConvert.GetAllChildren<TransporterExpressionContainer>().Each(x =>
         {
            convertTransportContainer(x, allTransporterExpressionContainers, transporter);
         });
      }

      private void convertTransportContainer(TransporterExpressionContainer expressionContainerToConvert,
         IReadOnlyList<TransporterExpressionContainer> allTransporterExpressionContainers, IndividualTransporter transporter)
      {
         //This should never happen as a tag was added in the xml conversion with the membrane location
         if (expressionContainerToConvert.Tags.Any())
            return;

         var membraneLocation = EnumHelper.ParseValue<MembraneLocation>(expressionContainerToConvert.Tags.ElementAt(0).Value);

         var matchingContainers = allTransporterExpressionContainers
            .Where(x => x.LogicalContainerName == expressionContainerToConvert.Name || x.CompartmentName == expressionContainerToConvert.Name)
            .ToList();

         matchingContainers.Each(x => x.TransportDirection = TransportDirections.DefaultDirectionFor(transporter.TransportType, x));

         //only one direction to take into consideration. all good, nothing else to do
         if (matchingContainers.Count == 1)
            return;

         MembraneLocationConverter.ConvertMembraneLocationToParameterFraction(matchingContainers, membraneLocation);
      }

      private void convertParameter(IParameter oldParameter, IParameter newParameter)
      {
         if (newParameter == null)
            return;

         newParameter.Value = oldParameter.Value;
         newParameter.DefaultValue = oldParameter.Value;
         newParameter.IsDefault = oldParameter.IsDefault;
         newParameter.ValueOrigin.UpdateAllFrom(oldParameter.ValueOrigin);
      }
   }
}
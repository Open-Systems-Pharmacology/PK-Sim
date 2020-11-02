using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.ProjectConverter.v10
{
   public class Converter9To10 : IObjectConverter
   {
      private bool _conversionHappened;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V9;

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _conversionHappened = false;
         element.DescendantsAndSelf("Individual").Each(convertIndividualProteinsIn);
         element.DescendantsAndSelf("BaseIndividual").Each(convertIndividualProteinsIn);

         return (ProjectVersions.V10, _conversionHappened);
      }

      private void convertIndividualProteinsIn(XElement individualElement)
      {
         individualElement.Descendants("IndividualEnzyme").Each(convertIndividualProtein);
         individualElement.Descendants("IndividualOtherProtein").Each(convertIndividualProtein);
      }

      private void convertIndividualProtein(XElement proteinNode)
      {
         var membraneLocation = EnumHelper.ParseValue<MembraneLocation>(proteinNode.GetAttribute("membraneLocation"));
         var tissueLocation = EnumHelper.ParseValue<TissueLocation>(proteinNode.GetAttribute("tissueLocation"));
         var intracellularVascularEndoLocation =
            EnumHelper.ParseValue<IntracellularVascularEndoLocation>(proteinNode.GetAttribute("intracellularVascularEndoLocation"));

         var localization = LocalizationConverter.ConvertToLocalization(tissueLocation, membraneLocation, intracellularVascularEndoLocation);
         proteinNode.AddAttribute("localization", localization.ToString());
         _conversionHappened = true;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         return (ProjectVersions.V10, false);
      }
   }
}
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Converters.v7_1;
using OSPSuite.Core.Serialization.Xml.Extensions;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Presentation;

namespace PKSim.Infrastructure.ProjectConverter.v7_1
{
   public class Converter641To710 : IObjectConverter
   {
      private readonly Converter63To710 _coreConverter63To710;
      private bool _converted;
      private const string DEFAULT_PRESENTATION_SETTINGS = "DefaultPresentationSettings";
      private const string PRESENTATION_KEY = "presentationKey";

      public Converter641To710(Converter63To710 coreConverter63To710)
      {
         _coreConverter63To710 = coreConverter63To710;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V6_4_1;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         return (ProjectVersions.V7_1_0, false);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         
         var (_,coreConversionHappened) = _coreConverter63To710.ConvertXml(element);
         element.DescendantsAndSelfNamed("WorkspaceLayoutItem").Each(convertWorkspaceLayoutItem);

         return (ProjectVersions.V7_1_0, _converted || coreConversionHappened);
      }

      private void convertWorkspaceLayoutItem(XElement layoutItemElement)
      {
         var presenterKey = layoutItemElement.GetAttribute("presenterKey");
         if (!string.IsNullOrEmpty(presenterKey))
         {
            layoutItemElement.SetAttributeValue(PRESENTATION_KEY, presenterKey);
            _converted = true;
         }

         var presentationSettings = layoutItemElement.Elements().FirstOrDefault();
         if (presentationSettings == null)
            return;

         //we renamed the class so we have to rename the xml node as well
         if (presentationSettings.Name == "DefaultPresenterSettings")
         {
            presentationSettings.Name = DEFAULT_PRESENTATION_SETTINGS;
            _converted = true;
         }

         //update property of WorkspaceLayoutItem
         var dynamicName = presentationSettings.GetAttribute("dynamicName");
         if (!string.IsNullOrEmpty(dynamicName))
         {
            presentationSettings.SetAttributeValue("dynamicName", "PresentationSettings");
            _converted = true;
         }

         //Key was not saved with the right constant
         if (presenterKey == PresenterConstants.PresenterKeys.ParameterGroupPresenter && presentationSettings.Name == DEFAULT_PRESENTATION_SETTINGS)
         {
            layoutItemElement.SetAttributeValue(PRESENTATION_KEY, PresenterConstants.PresenterKeys.IndividualPKParametersPresenter);
            _converted = true;
         }
      }
   }
}
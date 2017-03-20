using System.Linq;
using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Presentation;
using OSPSuite.Core.Converter.v6_4;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.ProjectConverter.v6_4
{
   public class Converter641To642 : IObjectConverter
   {
      private readonly Converter63To64 _coreConverter63To64;
      private const string DEFAULT_PRESENTATION_SETTINGS = "DefaultPresentationSettings";
      private const string PRESENTATION_KEY = "presentationKey";

      public Converter641To642(Converter63To64 coreConverter63To64)
      {
         _coreConverter63To64 = coreConverter63To64;
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V6_4_1;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         return ProjectVersions.V6_4_2;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         _coreConverter63To64.ConvertXml(element);
         element.DescendantsAndSelfNamed("WorkspaceLayoutItem").Each(convertWorkspaceLayoutItem);

         return ProjectVersions.V6_4_2;
      }

      private void convertWorkspaceLayoutItem(XElement layoutItemElement)
      {
         var presenterKey = layoutItemElement.GetAttribute("presenterKey");
         if (!string.IsNullOrEmpty(presenterKey))
         {
            layoutItemElement.SetAttributeValue(PRESENTATION_KEY, presenterKey);
         }

         var presentationSettings = layoutItemElement.Elements().FirstOrDefault();
         if (presentationSettings == null)
            return;

         //we renamed the class so we have to rename the xml node as well
         if (presentationSettings.Name == "DefaultPresenterSettings")
         {
            presentationSettings.Name = DEFAULT_PRESENTATION_SETTINGS;
         }

         //update property of WorkspaceLayoutItem
         var dynamicName = presentationSettings.GetAttribute("dynamicName");
         if (!string.IsNullOrEmpty(dynamicName))
         {
            presentationSettings.SetAttributeValue("dynamicName", "PresentationSettings");
         }

         //Key was not saved with the right constant
         if (presenterKey == PresenterConstants.PresenterKeys.ParameterGroupPresenter && presentationSettings.Name == DEFAULT_PRESENTATION_SETTINGS)
         {
            layoutItemElement.SetAttributeValue(PRESENTATION_KEY, PresenterConstants.PresenterKeys.IndividualPKParametersPresenter);
         }
      }
   }
}
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface ITemplateTask
   {
      /// <summary>
      ///    Starts the save to template  wokflow asking the user to enter a name under which the
      ///    <paramref name="objectToSave" /> shall be saved.
      /// </summary>
      /// <param name="objectToSave">Object to be saved as template</param>
      /// <param name="templateType">Template type under which the <paramref name="objectToSave" /> will be categorized</param>
      /// <param name="defaultName">Default name for the template</param>
      void SaveToTemplate<T>(T objectToSave, TemplateType templateType, string defaultName) where T : class;

      /// <summary>
      ///    Starts the save to template  wokflow asking the user to enter a name under which the
      ///    <paramref name="objectToSave" /> shall be saved. The default name used willl bethe name of the  <paramref name="objectToSave"/>
      /// </summary>
      /// <param name="objectToSave">Object to be saved as template</param>
      /// <param name="templateType">Template type under which the <paramref name="objectToSave" /> will be categorized</param>
      void SaveToTemplate<T>(T objectToSave, TemplateType templateType) where T : class, IWithName;

      /// <summary>
      ///    Starts the load from template workflows allowing the user to select one template for the given
      ///    <paramref name="templateType" />
      /// </summary>
      /// <returns>Returns the selected template or null if the action was cancelled by the user</returns>
      T LoadFromTemplate<T>(TemplateType templateType) where T : class;
   }
}
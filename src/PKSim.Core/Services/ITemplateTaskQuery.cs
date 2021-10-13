using System.Collections.Generic;
using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ITemplateTaskQuery
   {
      /// <summary>
      ///    Returns all available templates for the given <paramref name="templateType" />
      /// </summary>
      /// <param name="templateType">Type of template object for which the available templates should be retrieved</param>
      IReadOnlyList<Template> AllTemplatesFor(TemplateType templateType);

      /// <summary>
      ///    Returns all available templates of type <paramref name="templateDatabaseType"/> for the given <paramref name="templateType" />
      /// </summary>
      /// <param name="templateDatabaseType">Type of database template to search for.</param>
      /// <param name="templateType">Type of template object for which the available templates should be retrieved</param>
      IReadOnlyList<Template> AllTemplatesFor(TemplateDatabaseType templateDatabaseType, TemplateType templateType);

      /// <summary>
      ///    Returns a fully loaded building block from the database with the given <paramref name="template" />
      /// </summary>
      Task<T> LoadTemplateAsync<T>(Template template);

      /// <summary>
      ///    This method check whether a template already exists in the database for given building block type with given name.
      /// </summary>
      /// <param name="templateDatabaseType">Type of template to search for.</param>
      /// <param name="name">Name to search for.</param>
      /// <param name="templateType">Ttype to search for.</param>
      /// <returns>True, is there is already a template defined with that name</returns>
      bool Exists(TemplateDatabaseType templateDatabaseType, string name, TemplateType templateType);

      /// <summary>
      ///    Save the given <paramref name="templateItem" /> in the template database
      /// </summary>
      void SaveToTemplate(Template templateItem);

      /// <summary>
      ///    Save the given <paramref name="templateItems" /> in the template database/>
      /// </summary>
      void SaveToTemplate(IReadOnlyList<Template> templateItems);

 
      /// <summary>
      ///    Deletes the  template identified by <paramref name="templateToDelete" /> .
      /// </summary>
      void DeleteTemplate(Template templateToDelete);

      /// <summary>
      ///    Returns true if the  is a primitive type (i.e can be saved in the database as template) or false otherwise;
      /// </summary>
      bool IsPrimitiveType(TemplateType templateType);

      /// <summary>
      ///    Renames the given building block template with the given <paramref name="newName" />
      /// </summary>
      /// <param name="buildingBlockTemplate">Building block template to rename</param>
      /// <param name="newName">New name</param>
      void RenameTemplate(Template buildingBlockTemplate, string newName);
   }
}
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter
{
   /// <summary>
   ///    Brings a building block saved by an older version back in line with the structure currently defined in the PK-Sim
   ///    database. Templates are obtained from the database (via a factory or a repository) and the missing pieces are
   ///    cloned into the object being converted. Nothing is ever created from scratch.
   /// </summary>
   public interface ITemplateStructureUpdater
   {
      /// <summary>
      ///    Adds every parameter and sub container defined in <paramref name="templateContainer" /> that is missing from
      ///    <paramref name="containerToUpdate" />. Existing entities are left untouched. Returns <c>true</c> if anything
      ///    was added.
      /// </summary>
      bool AddMissingStructureTo(IContainer containerToUpdate, IContainer templateContainer);

      /// <summary>
      ///    Adds the parameters defined directly in <paramref name="templateContainer" /> that are missing from
      ///    <paramref name="containerToUpdate" />, without descending into sub containers. Use this where the sub
      ///    containers are named by the user rather than by the database, as they are for the alternatives of a compound.
      ///    Returns <c>true</c> if anything was added.
      /// </summary>
      bool AddMissingParametersTo(IContainer containerToUpdate, IContainer templateContainer);

      /// <summary>
      ///    Replaces the definition (formula, distribution, dimension, min/max, display settings) of every parameter of
      ///    <paramref name="containerToUpdate" /> that also exists in <paramref name="templateContainer" /> with the
      ///    definition from the template, while keeping any value the user had defined. Parameters missing from the object
      ///    being converted are ignored: <see cref="AddMissingStructureTo" /> is responsible for those. Returns
      ///    <c>true</c> if anything was replaced.
      /// </summary>
      bool RefreshParameterDefinitionsIn(IContainer containerToUpdate, IContainer templateContainer);
   }

   public class TemplateStructureUpdater : ITemplateStructureUpdater
   {
      private readonly ICloner _cloner;

      public TemplateStructureUpdater(ICloner cloner)
      {
         _cloner = cloner;
      }

      public bool AddMissingStructureTo(IContainer containerToUpdate, IContainer templateContainer)
      {
         if (containerToUpdate == null || templateContainer == null)
            return false;

         var structureAdded = false;

         //A distributed parameter is also a container, so parameters have to be matched before containers
         foreach (var templateChild in templateContainer.Children)
         {
            switch (templateChild)
            {
               case IParameter templateParameter:
                  if (containerToUpdate.Parameter(templateParameter.Name) != null)
                     continue;

                  containerToUpdate.Add(cloneOf(templateParameter));
                  structureAdded = true;
                  break;

               case IContainer templateSubContainer:
                  var subContainerToUpdate = containerToUpdate.GetSingleChildByName<IContainer>(templateSubContainer.Name);
                  if (subContainerToUpdate == null)
                  {
                     //The whole sub structure is missing. Cloning it brings its parameters along in one go
                     containerToUpdate.Add(cloneOf(templateSubContainer));
                     structureAdded = true;
                     continue;
                  }

                  structureAdded |= AddMissingStructureTo(subContainerToUpdate, templateSubContainer);
                  break;
            }
         }

         return structureAdded;
      }

      public bool AddMissingParametersTo(IContainer containerToUpdate, IContainer templateContainer)
      {
         if (containerToUpdate == null || templateContainer == null)
            return false;

         var parametersAdded = false;

         foreach (var templateParameter in templateContainer.GetChildren<IParameter>())
         {
            if (containerToUpdate.Parameter(templateParameter.Name) != null)
               continue;

            containerToUpdate.Add(cloneOf(templateParameter));
            parametersAdded = true;
         }

         return parametersAdded;
      }

      public bool RefreshParameterDefinitionsIn(IContainer containerToUpdate, IContainer templateContainer)
      {
         if (containerToUpdate == null || templateContainer == null)
            return false;

         var definitionRefreshed = false;

         foreach (var templateChild in templateContainer.Children)
         {
            switch (templateChild)
            {
               case IParameter templateParameter:
                  var parameterToRefresh = containerToUpdate.Parameter(templateParameter.Name);
                  if (parameterToRefresh == null)
                     continue;

                  refreshDefinition(containerToUpdate, parameterToRefresh, templateParameter);
                  definitionRefreshed = true;
                  break;

               case IContainer templateSubContainer:
                  var subContainerToUpdate = containerToUpdate.GetSingleChildByName<IContainer>(templateSubContainer.Name);
                  definitionRefreshed |= RefreshParameterDefinitionsIn(subContainerToUpdate, templateSubContainer);
                  break;
            }
         }

         return definitionRefreshed;
      }

      private void refreshDefinition(IContainer parentContainer, IParameter parameterToRefresh, IParameter templateParameter)
      {
         //Read the user value before the old parameter leaves the tree, as a formula may stop resolving once detached
         var userDefinedValue = userDefinedValueOf(parameterToRefresh);

         var refreshedParameter = cloneOf(templateParameter);
         refreshedParameter.ValueOrigin.UpdateAllFrom(parameterToRefresh.ValueOrigin);

         parentContainer.RemoveChild(parameterToRefresh);
         parentContainer.Add(refreshedParameter);

         if (userDefinedValue == null)
            return;

         //Assigning the value marks the parameter as fixed, which is what keeps the user value on top of a new formula
         refreshedParameter.Value = userDefinedValue.Value;
         refreshedParameter.IsDefault = false;
      }

      /// <summary>
      ///    Returns the value the user had defined for the given parameter, or <c>null</c> if the parameter was left at the
      ///    value the database defined for it. The parameter carries the old database default in its <c>DefaultValue</c>,
      ///    so a value differing from it is a user edit. A parameter still driven by its formula or distribution reports no
      ///    difference and is left to adopt the new definition. The template is deliberately not used as the reference: its
      ///    definition changed, so every parameter would look edited against it.
      /// </summary>
      private double? userDefinedValueOf(IParameter parameter)
      {
         if (!parameter.ValueDiffersFromDefault())
            return null;

         var value = parameter.Value;

         //An unresolved formula yields NaN. Writing that back would replace a working definition with a broken value
         return double.IsNaN(value) ? (double?) null : value;
      }

      private T cloneOf<T>(T entityToClone) where T : class, IEntity => _cloner.Clone(entityToClone);
   }
}

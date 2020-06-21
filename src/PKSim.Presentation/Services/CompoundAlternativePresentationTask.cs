using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Services
{
   public interface ICompoundAlternativePresentationTask
   {
      /// <summary>
      ///    Edits the solubility table for the given <paramref name="parameter" />
      /// </summary>
      ICommand EditSolubilityTableFor(IParameter parameter);

      /// <summary>
      ///    Creates an alternative, asks the user for its name,  and adds it to the given
      ///    <paramref name="compoundParameterGroup" />
      /// </summary>
      ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup);

      /// <summary>
      ///    Renames the given <paramref name="parameterAlternative" />
      /// </summary>
      ICommand RenameParameterAlternative(ParameterAlternative parameterAlternative);
   }

   public class CompoundAlternativePresentationTask : ICompoundAlternativePresentationTask
   {
      private readonly ICompoundAlternativeTask _compoundAlternativeTask;
      private readonly IApplicationController _applicationController;
      private readonly IEntityTask _entityTask;

      public CompoundAlternativePresentationTask(
         ICompoundAlternativeTask compoundAlternativeTask,
         IApplicationController applicationController,
         IEntityTask entityTask)
      {
         _compoundAlternativeTask = compoundAlternativeTask;
         _applicationController = applicationController;
         _entityTask = entityTask;
      }

      public ICommand EditSolubilityTableFor(IParameter parameter)
      {
         using (var tablePresenter = _applicationController.Start<IEditTableSolubilityParameterPresenter>())
         {
            if (!tablePresenter.Edit(parameter))
               return new PKSimEmptyCommand();

            return _compoundAlternativeTask.SetAlternativeParameterTable(parameter, tablePresenter.EditedFormula);
         }
      }

      public ICommand RenameParameterAlternative(ParameterAlternative parameterAlternative)
      {
         return _entityTask.StructuralRename(parameterAlternative);
      }

      public ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup)
      {
         var parameterAlternative = compoundParameterGroup.IsNamed(CoreConstants.Groups.COMPOUND_SOLUBILITY) ? createSolubilityCompoundAlternativeFor(compoundParameterGroup) : createStandardParameterAlternativeFor(compoundParameterGroup);

         if (parameterAlternative == null)
            return new PKSimEmptyCommand();

         return _compoundAlternativeTask.AddParameterGroupAlternativeTo(compoundParameterGroup, parameterAlternative);
      }

      private ParameterAlternative createStandardParameterAlternativeFor(ParameterAlternativeGroup compoundParameterGroup)
      {
         using (var presenter = _applicationController.Start<IParameterAlternativeNamePresenter>())
         {
            //canceled by user - nothing to do
            if (!presenter.Edit(compoundParameterGroup))
               return null;

            return _compoundAlternativeTask.CreateAlternative(compoundParameterGroup, presenter.Name);
         }
      }

      private ParameterAlternative createSolubilityCompoundAlternativeFor(ParameterAlternativeGroup solubilityAlternativeGroup)
      {
         using (var presenter = _applicationController.Start<ISolubilityAlternativeNamePresenter>())
         {
            //canceled by user - nothing to do
            if (!presenter.Edit(solubilityAlternativeGroup))
               return null;

            if (presenter.CreateAsTable)
               return _compoundAlternativeTask.CreateSolubilityTableAlternativeFor(solubilityAlternativeGroup, presenter.Name);

            return _compoundAlternativeTask.CreateAlternative(solubilityAlternativeGroup, presenter.Name);
         }
      }

   }
}
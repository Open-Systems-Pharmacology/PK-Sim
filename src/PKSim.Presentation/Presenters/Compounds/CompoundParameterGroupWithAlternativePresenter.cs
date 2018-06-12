using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundParameterGroupWithAlternativePresenter : ICompoundParameterGroupPresenter,
      IListener<AddCompoundParameterGroupAlternativeEvent>,
      IListener<RemoveCompoundParameterGroupAlternativeEvent>
   {
      /// <summary>
      ///    Add a new alternative for the group
      /// </summary>
      void AddAlternative();

      /// <summary>
      ///    Remove the alternative specified as parameter
      /// </summary>
      void RemoveAlternative(ParameterAlternativeDTO parameterAlternativeDTO);

      /// <summary>
      ///    Set the default value for the alternative given as parameter
      /// </summary>
      void SetIsDefaultFor(ParameterAlternativeDTO parameterAlternativeDTO, bool isDefault);

      /// <summary>
      ///    Set a new name for the given alernative
      /// </summary>
      void RenameAlternative(ParameterAlternativeDTO parameterAlternativeDTO);

      /// <summary>
      ///    Edit the value origin for the given alternative
      /// </summary>
      void UpdateValueOriginFor(ParameterAlternativeDTO parameterAlternativeDTO, ValueOrigin newValueOrigin);
   }

   public interface ICompoundParameterGroupWithCalculatedDefaultPresenter : ICompoundParameterGroupWithAlternativePresenter
   {
      /// <summary>
      ///    Show the calculated value for the selected parameter group (for insance all permeability values as a function of
      ///    available lipophilicity)
      /// </summary>
      void UpdateCalculatedValue();

      /// <summary>
      ///    returns true if the given alternative is the calculated alternative otherwise false
      /// </summary>
      /// <param name="parameterAlternativeDTO"></param>
      /// <returns></returns>
      bool IsCalculatedAlternative(ParameterAlternativeDTO parameterAlternativeDTO);
   }

   public abstract class CompoundParameterGroupWithAlternativePresenter<TView> : AbstractSubPresenter<TView, ICompoundParameterGroupWithAlternativePresenter>, ICompoundParameterGroupWithAlternativePresenter where TView : ICompoundParameterGroupWithAlternativeView
   {
      protected readonly ICompoundAlternativeTask _compoundAlternativeTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly string _parameterGroupName;
      protected ParameterAlternativeGroup _parameterGroup;
      protected Compound _compound;

      protected CompoundParameterGroupWithAlternativePresenter(TView view, IRepresentationInfoRepository representationRepository, ICompoundAlternativeTask compoundAlternativeTask, IDialogCreator dialogCreator, string groupName)
         : base(view)
      {
         _dialogCreator = dialogCreator;
         _compoundAlternativeTask = compoundAlternativeTask;
         _dialogCreator = dialogCreator;
         _parameterGroupName = groupName;
         View.Caption = representationRepository.DisplayNameFor(RepresentationObjectType.GROUP, _parameterGroupName);
      }

      protected virtual void EditParameterGroup(ParameterAlternativeGroup parameterGroup)
      {
         _parameterGroup = parameterGroup;
         updateAlternatives();
      }

      private void updateAlternatives()
      {
         FillUpParameterGroupAlternatives();
      }

      public virtual void EditCompound(Compound compound)
      {
         _compound = compound;
         EditParameterGroup(compound.ParameterAlternativeGroup(_parameterGroupName));
      }

      protected abstract IEnumerable<ParameterAlternativeDTO> FillUpParameterGroupAlternatives();

      public void AddAlternative()
      {
         AddCommand(_compoundAlternativeTask.AddParameterGroupAlternativeTo(_parameterGroup));
      }

      public void RemoveAlternative(ParameterAlternativeDTO parameterAlternativeDTO)
      {
         var viewResult = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteAlternative(parameterAlternativeDTO.Name));
         if (viewResult == ViewResult.No) return;

         AddCommand(_compoundAlternativeTask.RemoveParameterGroupAlternative(_parameterGroup, ParameterAlternativeFrom(parameterAlternativeDTO)));
      }

      public void SetIsDefaultFor(ParameterAlternativeDTO parameterAlternativeDTO, bool isDefault)
      {
         AddCommand(_compoundAlternativeTask.SetDefaultAlternativeFor(_parameterGroup, ParameterAlternativeFrom(parameterAlternativeDTO)));
      }

      public void RenameAlternative(ParameterAlternativeDTO parameterAlternativeDTO)
      {
         AddCommand(_compoundAlternativeTask.RenameParameterAlternative(ParameterAlternativeFrom(parameterAlternativeDTO)));
      }

      public void UpdateValueOriginFor(ParameterAlternativeDTO parameterAlternativeDTO, ValueOrigin newValueOrigin)
      {
         AddCommand(_compoundAlternativeTask.UpdateValueOrigin(ParameterAlternativeFrom(parameterAlternativeDTO), newValueOrigin));
      }

      public void Handle(AddCompoundParameterGroupAlternativeEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         updateAlternatives();
      }

      public void Handle(RemoveCompoundParameterGroupAlternativeEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         updateAlternatives();
      }

      private bool canHandle(IEntityContainerEvent entityContainerEvent)
      {
         return Equals(entityContainerEvent.ContainerSubject, _parameterGroup);
      }

      protected ParameterAlternative ParameterAlternativeFrom(ParameterAlternativeDTO parameterAlternativeDTO)
      {
         return parameterAlternativeDTO.ParameterAlternative;
      }
   }
}
﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Represents one alternative for a group of compound parameters
   /// </summary>
   public class ParameterAlternative : Container
   {
      private bool _isDefault;
      public virtual string GroupName => ParentParameterGroup.Name;

      public virtual bool IsDefault
      {
         get => _isDefault;
         set => SetProperty(ref _isDefault, value);
      }

      public virtual ParameterAlternativeGroup ParentParameterGroup => ParentContainer as ParameterAlternativeGroup;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceAlternative = sourceObject as ParameterAlternative;
         if (sourceAlternative == null) return;
         IsDefault = sourceAlternative.IsDefault;
      }

      public virtual IEnumerable<IParameter> AlllParametersWithSameValueOrigin
      {
         get
         {
            if (IsCalculated)
               return this.AllParameters();

            var allInputParameters = this.AllParameters(x => !x.IsDefault).ToList();
            if (allInputParameters.Any())
               return allInputParameters;
            
            return this.AllParameters();
         }
      }

      public virtual bool IsCalculated =>
         this.IsNamed(PKSimConstants.UI.CalculatedAlernative) &&
         CoreConstants.Groups.GroupsWithCalculatedAlternative.Contains(GroupName);
   }
}
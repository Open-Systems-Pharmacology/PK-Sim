using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Parameters
{
   public class ParameterDTO : PathRepresentableDTO, IDXDataErrorInfo, IParameterDTO
   {
      private readonly ICache<double, string> _listOfValues;
      private bool _isFavorite;

      public event EventHandler ValueChanged = delegate { };
      public virtual FormulaType FormulaType { get; set; }
      public virtual string DistributionType { get; set; }
      public virtual IParameter Parameter { get;  }
      public virtual IEnumerable<Unit> AllUnits { get; set; }
      public virtual string DisplayName { get; set; }
      public virtual string Description { get; set; }

      public virtual int Sequence { get; set; }

      public ParameterDTO(IParameter parameter)
      {
         _listOfValues = new Cache<double, string>();
         DisplayName = string.Empty;
         Parameter = parameter;
         bindToParameter();
      }

      private void bindToParameter()
      {
         if (Parameter == null)
            return;

         Parameter.PropertyChanged += handlePropertyChanged;

         if (Parameter.Editable)
            Rules.Add(ParameterDTORules.ParameterIsValid());
      }

      public virtual ICache<double, string> ListOfValues => _listOfValues;

      public virtual bool IsDiscrete => _listOfValues.Any();

      public virtual bool Editable => Parameter.Editable;

      public virtual string Name
      {
         get => Parameter.Name;
         set => Parameter.Name = value;
      }

      /// <summary>
      ///    Percentile is saved as a number between 0 and 1 in a parameter
      /// </summary>
      public virtual double Percentile
      {
         get => Parameter.GetPercentile() * 100;
         set
         {
            /*nothing to do here since the percentile should be set in the command*/
         }
      }

      public virtual double Value
      {
         get
         {
            try
            {
               return Parameter.ValueInDisplayUnit;
            }
            catch (Exception)
            {
               //Maybe implement a way to ask if a value can be computed instead of catching exception
               //Permeability cannot be read in compound as references cannot be resolved
               return 0;
            }
         }
         set
         {
            /*nothing to do here since the value should be set in the command*/
         }
      }

      public virtual Unit DisplayUnit
      {
         get => Parameter.DisplayUnit;
         set
         {
            /*nothing to do here since the unit should be set in the command*/
         }
      }

      public virtual IDimension Dimension
      {
         get => Parameter.Dimension;
         set
         {
            /*nothing to do here since the dimension should be set in the command*/
         }
      }


      public virtual ValueOrigin ValueOrigin
      {
         get => Parameter.ValueOrigin;
         set
         {
            /*nothing to do here since the ValueDescription should be set in the command*/
         }
      }


      public void UpdateValueOriginFrom(ValueOrigin sourceValueOrigin)
      {
         Parameter.UpdateValueOriginFrom(sourceValueOrigin);
      }

      public virtual double KernelValue => Parameter.Value;

      public virtual bool HasUnit => AllUnits.Count() > 1;

      public override bool Equals(object obj)
      {
         var parameterDTO = obj as ParameterDTO;
         return parameterDTO != null && Parameter.Equals(parameterDTO.Parameter);
      }

      public override int GetHashCode() => Parameter.GetHashCode();

      public override string ToString() => DisplayName;

      private void handlePropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName.Equals(CoreConstants.VALUE_PROPERTY_NAME))
         {
            ValueChanged(this, EventArgs.Empty);
         }

         RaisePropertyChanged(e.PropertyName);
      }

      public virtual void GetPropertyError(string propertyName, ErrorInfo info)
      {
         if (!string.Equals(propertyName, CoreConstants.VALUE_PROPERTY_NAME))
            return;

         var errors = this.Validate(propertyName);
         if (!errors.IsEmpty)
         {
            info.ErrorType = ErrorType.Critical;
            info.ErrorText = errors.Message;
            return;
         }

         //parameter is not editable. Nothing to display
         if (!Parameter.Editable) return;

         if (FormulaType != FormulaType.Rate) return;

         //value as not set by the user, only set information
         if (Parameter.IsFixedValue)
         {
            info.ErrorType = ErrorType.Warning;
            info.ErrorText = PKSimConstants.Information.ParameterIsAFormulaWithOverridenValue;
         }
      }

      public virtual void GetError(ErrorInfo info)
      {
         //parameter is not editable. Nothing to display
         if (!Parameter.Editable) return;

         //this is only used to display information on formula
         if (FormulaType != FormulaType.Rate) return;

         if (Parameter.IsFixedValue)
         {
            info.ErrorType = ErrorType.Warning;
            info.ErrorText = PKSimConstants.Information.ParameterIsAFormulaWithOverridenValue;
         }
         else
         {
            info.ErrorType = ErrorType.Information;
            info.ErrorText = PKSimConstants.Information.ParameterIsAFormula;
         }
      }

      public virtual bool IsFavorite
      {
         get => _isFavorite;
         set
         {
            _isFavorite = value;
            OnPropertyChanged(() => IsFavorite);
         }
      }

      public virtual void Release()
      {
         if (Parameter == null) return;
         Parameter.PropertyChanged -= handlePropertyChanged;
      }
   }

   public class WritableParameterDTO : ParameterDTO
   {
      public WritableParameterDTO(IParameter parameter)
         : base(parameter)
      {
         //base rules are not added for editeable parameter. We need them in that case however
         if (!parameter.Editable)
            Rules.Add(ParameterDTORules.ParameterIsValid());
      }

      public override Unit DisplayUnit
      {
         set
         {
            //the unit was set. We have to update the value 
            //18 years=> months, the display value should still be 18 to get 18 months
            double currentValue = Value;
            Parameter.DisplayUnit = value;
            Value = currentValue;
         }
      }

      public override double Value
      {
         set => Parameter.ValueInDisplayUnit = value;
      }

      public override double Percentile
      {
         set => Parameter.SetPercentile(value / 100);
      }
   }

   public class NullParameterDTO : ParameterDTO
   {
      public override double Value { get; set; }
      public override double Percentile { get; set; }
      public override Unit DisplayUnit { get; set; }

      public override double KernelValue => default;

      public NullParameterDTO() : base(null)
      {
         PathElements = new PathElements();
         AllUnits = new List<Unit>();
      }

      public override bool Editable => false;

      public override string Name => string.Empty;
   }
}
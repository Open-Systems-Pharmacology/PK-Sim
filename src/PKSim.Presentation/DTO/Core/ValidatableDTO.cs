using System;
using System.Linq.Expressions;
using BTS.Utility.Reflection;
using BTS.Utility.Validation;

namespace PKSim.Presentation.DTO.Core
{
   public interface IValidatableDTO : IValidatable, INotifier
   {
      void AddNotifiableFor<TValidatable, TResult>(Expression<Func<TValidatable, TResult>> notifier) where TValidatable : IValidatableDTO;
   }

   public abstract class ValidatableDTO : Notifier, IValidatableDTO
   {
      protected readonly IBusinessRuleSet _rules = new BusinessRuleSet();

      public virtual IBusinessRuleSet Rules
      {
         get { return _rules; }
      }

      public void AddNotifiableFor<TValidatable, TResult>(Expression<Func<TValidatable, TResult>> notifier) where TValidatable : IValidatableDTO
      {
         OnPropertyChanged(notifier.Name());
      }
   }

   public abstract class ValidatableDTO<T> : ValidatableDTO where T : IValidatable, INotifier
   {
      protected ValidatableDTO(T underlyingObject)
      {
        this.AddRulesFrom(underlyingObject);
        underlyingObject.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
      }
   }
}
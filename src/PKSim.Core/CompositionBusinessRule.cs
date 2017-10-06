using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;

namespace PKSim.Core
{
   public class CompositionBusinessRule<TValidatable, TValue> : IBusinessRule where TValidatable : IValidatable
   {
      private readonly TValidatable _objectToValidate;
      private readonly IReadOnlyList<IBusinessRule> _underlyingRules;

      public string Name { get; }

      public string Description { get; private set; }

      public CompositionBusinessRule(TValidatable objectToValidate, Expression<Func<TValidatable, TValue>> expression, string propertyName)
      {
         _objectToValidate = objectToValidate;
         var propertyNameToValidate = expression.Name();
         _underlyingRules = objectToValidate.Rules.Where(rule => rule.IsRuleFor(propertyNameToValidate)).All().ToList();
         Name = propertyName;
      }

      public bool IsRuleFor(string propertyName)
      {
         return Name == propertyName;
      }

      public bool IsSatisfiedBy(object item)
      {
         return anySubRulesBrokenForCriteria(rule => !rule.IsSatisfiedBy(_objectToValidate));
      }

      public bool IsSatisfiedBy(object item, object value)
      {
         return anySubRulesBrokenForCriteria(rule => !rule.IsSatisfiedBy(_objectToValidate, value));
      }

      private bool anySubRulesBrokenForCriteria(Func<IBusinessRule, bool> specification)
      {
         var businessRuleSet = new BusinessRuleSet(_underlyingRules.Where(specification));
         Description = businessRuleSet.Message;
         return businessRuleSet.IsEmpty;
      }
   }
}
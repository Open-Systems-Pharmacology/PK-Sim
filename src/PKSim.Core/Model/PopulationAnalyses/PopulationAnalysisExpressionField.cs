using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationAnalysisExpressionField : PopulationAnalysisDerivedField
   {
      private string _expressionPattern;
      private List<string> _fields;

      [Obsolete("For serialization")]
      public PopulationAnalysisExpressionField() : this(string.Empty)
      {
      }

      public PopulationAnalysisExpressionField(string expression) : base(typeof (object))
      {
         SetExpression(expression);
      }

      //ToArray required otherwise a runtime error is thrown..
      public override string Expression => String.Format(_expressionPattern, _fields.ToArray());

      public void SetExpression(string expression)
      {
         if (string.IsNullOrEmpty(expression))
            return;

         //get fields and modify expression to a string pattern
         _fields = new List<string>(FindFieldsIn(expression));
         var i = 0;
         var expressionPattern = expression;
         foreach (var usedField in _fields)
         {
            expressionPattern = expressionPattern.Replace($"[{usedField}]", $"[{{{i++}}}]");
         }
         _expressionPattern = expressionPattern;
      }

      //public only to make it testable
      public string[] FindFieldsIn(string expression)
      {
         const string fieldPattern = "[[](.+?)[]]"; //finds all strings encapsulated with brackets []
         var foundFields = new List<string>();

         foreach (var match in Regex.Matches(expression, fieldPattern))
         {
            var fieldfound = match.ToString().Substring(1, match.ToString().Length - 2);
            if (foundFields.Contains(fieldfound)) continue;
            foundFields.Add(fieldfound);
         }
         return foundFields.ToArray();
      }

      public override bool IsDerivedTypeFor(Type fieldType) => false;

      /// <summary>
      ///    This method renames a referenced field within the expression.
      /// </summary>
      public override void RenameReferencedField(string oldFieldName, string newFieldName)
      {
         for (var i = 0; i < _fields.Count; i++)
         {
            var fieldName = _fields[i];
            if (fieldName == oldFieldName)
               _fields[i] = newFieldName;
         }
      }

      public override IReadOnlyCollection<string> ReferencedFieldNames => _fields;

      public override bool CanBeUsedFor(Type dataType) => true;

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var expressionField = source as PopulationAnalysisExpressionField;
         if (expressionField == null) return;
         _expressionPattern = expressionField._expressionPattern;
         _fields = expressionField._fields.ToList();
      }
   }
}
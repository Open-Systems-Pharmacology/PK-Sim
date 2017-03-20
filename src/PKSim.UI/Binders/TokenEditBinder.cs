using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.Core;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraEditors;

namespace PKSim.UI.Binders
{
   public class TokenEditBinder<TObjectType, TValue> : ElementBinder<TObjectType, IEnumerable<TValue>>
   {
      private readonly TokenEdit _tokenEdit;
      private Func<TValue, string> _displayFor;

      private Action<IEnumerable<TValue>> _onSelectedItemChanged;
      private Func<TObjectType, IEnumerable<TValue>> _knownTokensFunc;

      public TokenEditBinder(IPropertyBinderNotifier<TObjectType, IEnumerable<TValue>> propertyBinder, TokenEdit tokenEdit)
         : base(propertyBinder)
      {
         _tokenEdit = tokenEdit;
         _displayFor = x => x.ToString();
         _onSelectedItemChanged = delegate { };
         _tokenEdit.SelectedItemsChanged += selectedItemsChanged;
         _tokenEdit.Properties.EditValueType = TokenEditValueType.List;
      }

      private void selectedItemsChanged(object sender, ListChangedEventArgs e)
      {
         _onSelectedItemChanged(GetValueFromControl());
      }

      public TokenEditBinder<TObjectType, TValue> OnSelectedItemsChanged(Action<IEnumerable<TValue>> action)
      {
         _onSelectedItemChanged = action;
         return this;
      }

      public override void Bind(TObjectType source)
      {
         //This needs to be done FIRST so that the list of available is already populated when binding
         addTokensToKnownTokens(_knownTokensFunc(source));
         base.Bind(source);
      }

      public TokenEditBinder<TObjectType, TValue> WithDisplay(Func<TValue, string> displayFunc)
      {
         _displayFor = displayFunc;
         return this;
      }

      public override IEnumerable<TValue> GetValueFromControl()
      {
         return _tokenEdit.SelectedItems.OfType<TokenEditToken>().Select(x => x.Value).OfType<TValue>();
      }

      public override void SetValueToControl(IEnumerable<TValue> values)
      {
         setListOfSelectedTokens(values.ToList());
      }

      private void setListOfSelectedTokens(IReadOnlyList<TValue> values)
      {
         addTokensToKnownTokens(values);

         _tokenEdit.SelectedItemsChanged -= selectedItemsChanged;
         _tokenEdit.EditValue = values.Select(_displayFor).ToBindingList();
         _tokenEdit.SelectedItemsChanged += selectedItemsChanged;
      }

      private void addTokensToKnownTokens(IEnumerable<TValue> valueList)
      {
         valueList.Each(addTokenToKnownTokens);
      }

      public TokenEditBinder<TObjectType, TValue> WithKnownTokens(Func<TObjectType, IEnumerable<TValue>> valuesFunc)
      {
         _knownTokensFunc = valuesFunc;
         return this;
      }

      private void addTokenToKnownTokens(TValue value)
      {
         if (!_tokenEdit.Properties.Tokens.Any(token => token.Value.Equals(value)))
         {
            _tokenEdit.Properties.Tokens.Add(new TokenEditToken(_displayFor(value), value));
         }
      }

      public override Control Control
      {
         get { return _tokenEdit; }
      }
   }
}
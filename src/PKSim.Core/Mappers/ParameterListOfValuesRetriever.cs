using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IParameterListOfValuesRetriever
   {
      ICache<double, string> ListOfValuesFor(IParameter parameter);
      void UpdateLisOfValues(ICache<double, string> listOfValues, IParameter parameter);
   }

   public class ParameterListOfValuesRetriever : IParameterListOfValuesRetriever
   {
      private readonly HashSet<string> _parameterWithListOfValues;

      public ParameterListOfValuesRetriever() : this(CoreConstants.Parameters.AllWithListOfValues)
      {
      }

      public ParameterListOfValuesRetriever(HashSet<string> parameterWithListOfValues)
      {
         _parameterWithListOfValues = parameterWithListOfValues;
      }

      public ICache<double, string> ListOfValuesFor(IParameter parameter)
      {
         var listOfValues = new Cache<double, string>(x => parameter.ValueInDisplayUnit);
         UpdateLisOfValues(listOfValues, parameter);
         return listOfValues;
      }

      public void UpdateLisOfValues(ICache<double, string> listOfValues, IParameter parameter)
      {
         if (!_parameterWithListOfValues.Contains(parameter.Name))
            return;

         if (parameter.IsNamed(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION))
         {
            listOfValues.Add(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL, PKSimConstants.UI.Normal);
            listOfValues.Add(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL, PKSimConstants.UI.LogNormal);
         }
         else if (parameter.IsNamed(CoreConstants.Parameters.PLASMA_PROTEIN_BINDING_PARTNER))
         {
            listOfValues.Add(CoreConstants.Compound.BINDING_PARTNER_ALBUMIN, PKSimConstants.UI.Albumin);
            listOfValues.Add(CoreConstants.Compound.BINDING_PARTNER_AGP, PKSimConstants.UI.Glycoprotein);
            listOfValues.Add(CoreConstants.Compound.BINDING_PARTNER_UNKNOWN, PKSimConstants.UI.Unknown);
         }
         else if (parameter.IsNamed(CoreConstants.Parameters.NUMBER_OF_BINS))
         {
            addNumericListOfValues(listOfValues, 1, CoreConstants.Parameters.MAX_NUMBER_OF_BINS);
         }
         else if (parameter.NameIsOneOf(CoreConstants.Parameters.Halogens))
         {
            addNumericListOfValues(listOfValues, 0, CoreConstants.Parameters.MAX_NUMBER_OF_HALOGENS);
         }
         else if (parameter.Name.StartsWith(CoreConstants.Parameters.ParameterCompoundTypeBase))
         {
            listOfValues.Add(CoreConstants.Compound.COMPOUND_TYPE_ACID, CompoundType.Acid.ToString());
            listOfValues.Add(CoreConstants.Compound.COMPOUND_TYPE_NEUTRAL, CompoundType.Neutral.ToString());
            listOfValues.Add(CoreConstants.Compound.COMPOUND_TYPE_BASE, CompoundType.Base.ToString());
         }
         else if (parameter.IsNamed(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM))
         {
            listOfValues.Add(CoreConstants.Parameters.MONODISPERSE, PKSimConstants.UI.Monodisperse);
            listOfValues.Add(CoreConstants.Parameters.POLYDISPERSE, PKSimConstants.UI.Polydisperse);
         }
         else if (parameter.IsNamed(CoreConstants.Parameters.PRECIPITATED_DRUG_SOLUBLE))
         {
            listOfValues.Add(CoreConstants.Parameters.SOLUBLE, PKSimConstants.UI.Soluble);
            listOfValues.Add(CoreConstants.Parameters.INSOLUBLE, PKSimConstants.UI.Insoluble);
         }
         else if (parameter.IsNamed(CoreConstants.Parameters.GESTATIONAL_AGE))
         {
            addNumericListOfValues(listOfValues, CoreConstants.PretermRange.Min(), CoreConstants.PretermRange.Max());
         }
         else if (CoreConstants.Parameters.AllBooleanParameters.Contains(parameter.Name))
         {
            listOfValues.Add(1, PKSimConstants.UI.Yes);
            listOfValues.Add(0, PKSimConstants.UI.No);
         }
         else if (parameter.NameIsOneOf(CoreConstants.Parameters.PARA_ABSORPTION_SINK, CoreConstants.Parameters.TRANS_ABSORPTION_SINK))
         {
            listOfValues.Add(CoreConstants.Parameters.SINK_CONDITION, PKSimConstants.UI.SinkCondition);
            listOfValues.Add(CoreConstants.Parameters.NO_SINK_CONDITION, PKSimConstants.UI.NoSinkCondition);
         }
         else
            throw new ArgumentException("Cannot create list of values", parameter.Name);
      }

      private void addNumericListOfValues(ICache<double, string> cache, int min, int max)
      {
         Enumerable.Range(min, max - min + 1).Each(i => cache.Add(i, i.ToString()));
      }
   }
}
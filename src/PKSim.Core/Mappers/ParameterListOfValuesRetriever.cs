using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Assets.PKSimConstants.UI;
using static PKSim.Core.CoreConstants.Compound;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Mappers
{
   public interface IParameterListOfValuesRetriever
   {
      ICache<double, string> ListOfValuesFor(IParameter parameter);
      string ValueFor<T>(T parameter) where T : IWithValue, IWithName;
      void UpdateLisOfValues(ICache<double, string> listOfValues, IWithName parameter);
   }

   public class ParameterListOfValuesRetriever : IParameterListOfValuesRetriever
   {
      private readonly HashSet<string> _parameterWithListOfValues;
      private readonly NumericFormatter<double> _formatter;

      public ParameterListOfValuesRetriever() : this(new HashSet<string>(AllWithListOfValues))
      {
      }

      public ParameterListOfValuesRetriever(HashSet<string> parameterWithListOfValues)
      {
         _parameterWithListOfValues = parameterWithListOfValues;
         //TODO MOVE TO CORE
         _parameterWithListOfValues.Add(HIDiseaseStateImplementation.CHILD_PUGH_SCORE);
         _formatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);

      }

      public ICache<double, string> ListOfValuesFor(IParameter parameter)
      {
         var listOfValues = new Cache<double, string>(x => parameter.ValueInDisplayUnit);
         UpdateLisOfValues(listOfValues, parameter);
         return listOfValues;
      }

      public string ValueFor<T>(T parameter) where T : IWithValue, IWithName
      {
         if (!_parameterWithListOfValues.Contains(parameter.Name))
            return null;

         var listOfValues = new Cache<double, string>(x => _formatter.Format(x));
         UpdateLisOfValues(listOfValues, parameter);

         return listOfValues[parameter.Value];
      }

      public void UpdateLisOfValues(ICache<double, string> listOfValues, IWithName parameter)
      {
         if (!_parameterWithListOfValues.Contains(parameter.Name))
            return;

         if (parameter.IsNamed(PARTICLE_SIZE_DISTRIBUTION))
         {
            listOfValues.Add(PARTICLE_SIZE_DISTRIBUTION_NORMAL, Normal);
            listOfValues.Add(PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL, LogNormal);
         }
         else if (parameter.IsNamed(PLASMA_PROTEIN_BINDING_PARTNER))
         {
            listOfValues.Add(BINDING_PARTNER_ALBUMIN, Albumin);
            listOfValues.Add(BINDING_PARTNER_AGP, Glycoprotein);
            listOfValues.Add(BINDING_PARTNER_UNKNOWN, Unknown);
         }
         else if (parameter.IsNamed(NUMBER_OF_BINS))
         {
            addNumericListOfValues(listOfValues, 1, MAX_NUMBER_OF_BINS);
         }
         else if (parameter.NameIsOneOf(CoreConstants.Parameters.Halogens))
         {
            addNumericListOfValues(listOfValues, 0, MAX_NUMBER_OF_HALOGENS);
         }
         else if (parameter.Name.StartsWith(ParameterCompoundTypeBase))
         {
            listOfValues.Add(COMPOUND_TYPE_ACID, CompoundType.Acid.ToString());
            listOfValues.Add(COMPOUND_TYPE_NEUTRAL, CompoundType.Neutral.ToString());
            listOfValues.Add(COMPOUND_TYPE_BASE, CompoundType.Base.ToString());
         }
         else if (parameter.IsNamed(PARTICLE_DISPERSE_SYSTEM))
         {
            listOfValues.Add(MONODISPERSE, Monodisperse);
            listOfValues.Add(POLYDISPERSE, Polydisperse);
         }
         else if (parameter.IsNamed(PRECIPITATED_DRUG_SOLUBLE))
         {
            listOfValues.Add(SOLUBLE, Soluble);
            listOfValues.Add(INSOLUBLE, Insoluble);
         }
         else if (parameter.IsNamed(GESTATIONAL_AGE))
         {
            addNumericListOfValues(listOfValues, CoreConstants.PretermRange.Min(), CoreConstants.PretermRange.Max());
         }
         else if (AllBooleanParameters.Contains(parameter.Name))
         {
            listOfValues.Add(1, Yes);
            listOfValues.Add(0, No);
         }
         else if (parameter.NameIsOneOf(PARA_ABSORPTION_SINK, TRANS_ABSORPTION_SINK))
         {
            listOfValues.Add(SINK_CONDITION, SinkCondition);
            listOfValues.Add(NO_SINK_CONDITION, NoSinkCondition);
         }
         else if (parameter.IsNamed(HIDiseaseStateImplementation.CHILD_PUGH_SCORE))
         {
            listOfValues.Add(HIDiseaseStateImplementation.ChildPughScore.A, ChildPughScoreFor("A"));
            listOfValues.Add(HIDiseaseStateImplementation.ChildPughScore.B, ChildPughScoreFor("B"));
            listOfValues.Add(HIDiseaseStateImplementation.ChildPughScore.C, ChildPughScoreFor("C"));
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
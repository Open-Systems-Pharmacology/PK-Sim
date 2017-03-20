using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Services
{
   public interface IFormulationValuesRetriever
   {
      TableFormula TableValueFor(Formulation formulation);
   }

   public interface IFormulationValuesSpecification : IFormulationValuesRetriever, ISpecification<Formulation>
   {
   }

   public class FormulationValuesRetriever : IFormulationValuesRetriever
   {
      private readonly IEnumerable<IFormulationValuesSpecification> _allFormulationValues;

      public FormulationValuesRetriever(IRepository<IFormulationValuesSpecification> repository)
      {
         _allFormulationValues = repository.All();
      }

      public TableFormula TableValueFor(Formulation formulation)
      {
         return _allFormulationValues.Where(fv => fv.IsSatisfiedBy(formulation))
            .Select(fv => fv.TableValueFor(formulation))
            .FirstOrDefault();
      }
   }

   public abstract class FormulationValuesSpecification : IFormulationValuesSpecification
   {
      private readonly IFormulaFactory _formulaFactory;

      protected FormulationValuesSpecification(IFormulaFactory formulaFactory)
      {
         _formulaFactory = formulaFactory;
      }

      public TableFormula TableValueFor(Formulation formulation)
      {
         var tableFormula = _formulaFactory.CreateTableFormula().WithName(CoreConstants.Parameter.FRACTION_DOSE);
         CacheParameterValueFor(formulation);
         foreach (var time in createTimeArrayInMinutes())
         {
            tableFormula.AddPoint(time, ValueFor(formulation, time));
         }
         return tableFormula;
      }

      protected abstract void CacheParameterValueFor(Formulation formulation);

      protected abstract double ValueFor(Formulation formulation, double time);

      private IEnumerable<double> createTimeArrayInMinutes()
      {
         var timeInHours = new List<double>();
         double interval = 1.0 / CoreConstants.RESOLUTION_FOR_FORMULATION_PLOT;
         int maxTime = (int) Math.Floor(CoreConstants.DEFAULT_PROTOCOL_END_TIME_IN_MIN / 60);
         for (int i = 0; i < maxTime; i++)
         {
            timeInHours.Add(i);
            for (int j = 1; j <= CoreConstants.RESOLUTION_FOR_FORMULATION_PLOT - 1; j++)
            {
               timeInHours.Add(i + j * interval);
            }
         }
         timeInHours.Add(maxTime);
         //return time in minutes
         return timeInHours.Select(x => x * 60);
      }

      public abstract bool IsSatisfiedBy(Formulation item);
   }
}
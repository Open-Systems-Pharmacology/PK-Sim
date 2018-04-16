using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class TableFormulaWithXArgument : Formula
   {
      public string TableObjectAlias { get; set; }
      public string XArgumentAlias { get; set; }

      /// <summary>
      ///    Add path to the object with table formula
      /// </summary>
      /// <param name="tableObjectPath"></param>
      public void AddTableObjectPath(IFormulaUsablePath tableObjectPath)
      {
         TableObjectAlias = tableObjectPath.Alias;
         AddObjectPath(tableObjectPath);
      }

      /// <summary>
      ///    Add path to the object with offset formula
      /// </summary>
      /// <param name="xArgumentObjectPath"></param>
      public void AddXArgumentObjectPath(IFormulaUsablePath xArgumentObjectPath)
      {
         XArgumentAlias = xArgumentObjectPath.Alias;
         AddObjectPath(xArgumentObjectPath);
      }

      /// <summary>
      ///    Returns table formula object
      /// </summary>
      /// <param name="refObject">Entity using formula</param>
      public IUsingFormula GetTableObject(IUsingFormula refObject)
      {
         return GetReferencedEntityByAlias(TableObjectAlias, refObject) as IUsingFormula;
      }

      /// <summary>
      ///    Returns reference object used for table formula
      /// </summary>
      /// <param name="refObject">Entity using formula</param>
      public IFormulaUsable GetXArgumentObject(IUsingFormula refObject)
      {
         return GetReferencedEntityByAlias(XArgumentAlias, refObject);
      }

      /// <summary>
      ///    Return the value of the table object for 0-offset value
      /// </summary>
      protected override double CalculateFor(IEnumerable<IObjectReference> usedObjects, IUsingFormula dependentObject)
      {
         var tableObject = GetTableObject(dependentObject);
         var  tableFormula = tableObject?.Formula as TableFormula;

         return tableFormula?.ValueAt(GetXArgumentObject(dependentObject).Value) ?? double.NaN;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);

         var tableFormulaWithReference = source as TableFormulaWithXArgument;
         if (tableFormulaWithReference == null) return;

         TableObjectAlias = tableFormulaWithReference.TableObjectAlias;
         XArgumentAlias = tableFormulaWithReference.XArgumentAlias;
      }
   }
}
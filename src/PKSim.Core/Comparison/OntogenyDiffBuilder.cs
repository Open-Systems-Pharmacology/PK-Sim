using OSPSuite.Core.Comparison;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Comparison;

public class NullOntogenyDiffBuilder : DiffBuilder<NullOntogeny>
{
   public override void Compare(IComparison<NullOntogeny> comparison)
   {
      //if we are here,we have two molecules without ontogeny, nothing to compare
   }
}

public class DatabaseOntogenyDiffBuilder : DiffBuilder<DatabaseOntogeny>
{
   public override void Compare(IComparison<DatabaseOntogeny> comparison)
   {
      //Database ontogeny are the same if they have the same name 
      CompareValues(x => x.Name, PKSimConstants.UI.Ontogeny, comparison);
   }
}

public class UserDefinedOntogenyDiffBuilder : DiffBuilder<UserDefinedOntogeny>
{
   private readonly IObjectComparer _objectComparer;

   public UserDefinedOntogenyDiffBuilder(IObjectComparer objectComparer)
   {
      _objectComparer = objectComparer;
   }

   public override void Compare(IComparison<UserDefinedOntogeny> comparison)
   {
      //User defined ontogeny are the same if they have the same name  and same formula
      CompareValues(x => x.Name, PKSimConstants.UI.Ontogeny, comparison);
      _objectComparer.Compare(comparison.ChildComparison(x => x.Table));
   }
}
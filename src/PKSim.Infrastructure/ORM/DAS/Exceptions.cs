using System.Data;

namespace PKSim.Infrastructure.ORM.DAS
{
   public class NoOpenTransactionException : System.Exception
   {
      public NoOpenTransactionException(): base("No open transaction!")
      {
      }
   }

   public class AlreadyOpenTransactionException : System.Exception
   {
      public AlreadyOpenTransactionException(): base("There is already an open transaction!")
      {
      }
   }

   public class NotConnectedException : System.Exception
   {
      public NotConnectedException(): base("You are not connected to a database!")
      {
      }
   }

   public class AlreadyConnectedException : System.Exception
   {
      public AlreadyConnectedException(): base("You are already connected to a database!")
      {
      }
   }

   public class UnknownDataProvider : System.Exception
   {
      public UnknownDataProvider(): base("The specified data provider is unknown!")
      {
      }
   }

   public class RowNotFoundException : System.Exception
   {
      public RowNotFoundException(string RowID): base(string.Format("Row({0}) could not be found in the database!", RowID))
      {
      }
   }

   public class TooManyRowsFoundException : System.Exception
   {
      public TooManyRowsFoundException(): base("Too many rows have been found in the database!")
      {
      }
   }

   public class NotInsertableException : System.Exception
   {
      public NotInsertableException(): base("Object is not insertable!")
      {
      }
   }

   public class NotEditableException : System.Exception
   {
      public NotEditableException(): base("Object is not editable!")
      {
      }
   }

   public class UnsupportedDataTypeException : System.Exception
   {
      public UnsupportedDataTypeException(string Datatype): base(string.Format("Datatype :{0} is not supported!", Datatype))
      {
      }
   }

   public class UnsupportedDataTypeForAutoValueCreationException : System.Exception
   {
      public UnsupportedDataTypeForAutoValueCreationException(string Datatype): base(string.Format("Datatype :{0} is not supported for auto value creation!", Datatype))
      {
      }
   }

   public class UnknownTableException : System.Exception
   {
      public UnknownTableException(string TableName): base(string.Format("Table :{0} is unknown!", TableName))
      {
      }
   }

   public class InvalidRowStateException : System.Exception
   {
      public InvalidRowStateException(DataRowState RowState) : base(string.Format("RowState :{0} is invalid!", RowState))
      {
      }
   }
}
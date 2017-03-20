namespace PKSim.Core
{
   public class CompositeKey
   {
      public string KeyElement1 { get; private set; }
      public string KeyElement2 { get; private set; }

      public CompositeKey(string keyElement1, string keyElement2)
      {
         KeyElement1 = keyElement1;
         KeyElement2 = keyElement2;
      }

      public override bool Equals(object obj)
      {
         var rateKey = obj as CompositeKey;

         return (rateKey != null)
                && Equals(rateKey.KeyElement1, KeyElement1)
                && Equals(rateKey.KeyElement2, KeyElement2);
      }

      public override int GetHashCode()
      {
         return KeyElement1.GetHashCode() * 37 + KeyElement2.GetHashCode();
      }

      public override string ToString()
      {
         return string.Format("{0}.{1}", KeyElement1, KeyElement2);
      }

      public static implicit operator string(CompositeKey compositeKey)
      {
         return compositeKey.ToString();
      }
   }
}
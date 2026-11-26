using System;
using System.Security.Cryptography;
using System.Text;

namespace PKSim.Core.Model
{
   //Deliberately holds no credential: PK-Sim must never store secrets. They are resolved at download time (see ITemplateCredentialProvider)
   public class TemplateSource
   {
      public string Name { get; set; }
      public string Url { get; set; }

      public TemplateSource()
      {
      }

      public TemplateSource(string name, string url)
      {
         Name = name;
         Url = url;
      }

      public Uri Uri => UriFor(Url);

      public bool HasValidUrl => IsValidUrl(Url);

      public string CacheKey => CacheKeyFor(Url);

      public static string CacheKeyFor(string url)
      {
         //AbsoluteUri lower cases scheme and host only: the case of path and query is significant
         var identity = UriFor(url)?.AbsoluteUri ?? (url ?? string.Empty).Trim();
         var hash = SHA256.HashData(Encoding.UTF8.GetBytes(identity));
         return Convert.ToHexString(hash, 0, 8).ToLowerInvariant();
      }

      public static Uri UriFor(string url)
      {
         if (string.IsNullOrWhiteSpace(url))
            return null;

         return Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ? uri : null;
      }

      public static bool IsValidUrl(string url)
      {
         var uri = UriFor(url);
         if (uri == null)
            return false;

         if (uri.IsFile)
            return true;

         //user information in the url (https://user:password@host) would end up in the user settings file
         if (!string.IsNullOrEmpty(uri.UserInfo))
            return false;

         return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
      }

      public TemplateSource Clone() => new TemplateSource().WithUpdatedPropertiesFrom(this);

      public TemplateSource WithUpdatedPropertiesFrom(TemplateSource source)
      {
         Name = source.Name;
         Url = source.Url;
         return this;
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(this, obj))
            return true;

         return obj is TemplateSource other &&
                string.Equals(Name, other.Name) &&
                string.Equals(Url, other.Url);
      }

      public override int GetHashCode() => HashCode.Combine(Name, Url);

      public override string ToString() => $"{Name} ({Url})";
   }
}

﻿namespace DiscogsConnect
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    
    // reference:
    // http://www.codeproject.com/Articles/108996/Splitting-Pascal-Camel-Case-with-RegEx-Enhancement

    internal class DiscogsContractResolver : DefaultContractResolver
    {
        static readonly string REGEX_PATTERN     = "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])";
        static readonly string REGEX_REPLACEMENT = "_$1";

        protected override string ResolvePropertyName(string propertyName)
        {                        
            var output = Regex.Replace(propertyName, REGEX_PATTERN, REGEX_REPLACEMENT, RegexOptions.Compiled);
            return output.ToLower();
        }

        static readonly Dictionary<Type, string> mapping = new Dictionary<Type, string>
        {
            { typeof(ArtistRelease), "Releases"},
            { typeof(LabelRelease), "Releases"},
            { typeof(MasterVersion), "Versions"},
            { typeof(SearchResult), "Results"},
        };

        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            if (typeof(PaginationResponse).IsAssignableFrom(member.DeclaringType) &&
                member.Name == "Items")
            {
                var resourceType = member.DeclaringType.GetGenericArguments()[0];
                jsonProperty.PropertyName = mapping[resourceType];
            }

            return jsonProperty;
        }
    }
}

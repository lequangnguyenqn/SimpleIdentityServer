namespace SimpleIdentityServer.Services
{
    /// <summary>
    /// Commonly used claim types
    /// </summary>
    public static class JwtClaimTypes
    {
        /// <summary>Unique Identifier for the End-User at the Issuer.</summary>
        public const string Subject = "sub";

        /// <summary>End-User's full name in displayable form including all name parts, possibly including titles and suffixes, ordered according to the End-User's locale and preferences.</summary>
        public const string Name = "name";

        /// <summary>Given name(s) or first name(s) of the End-User. Note that in some cultures, people can have multiple given names; all can be present, with the names being separated by space characters.</summary>
        public const string GivenName = "given_name";

        /// <summary>Surname(s) or last name(s) of the End-User. Note that in some cultures, people can have multiple family names or no family name; all can be present, with the names being separated by space characters.</summary>
        public const string FamilyName = "family_name";

        /// <summary>Shorthand name by which the End-User wishes to be referred to at the RP, such as janedoe or j.doe. This value MAY be any valid JSON string including special characters such as @, /, or whitespace. The relying party MUST NOT rely upon this value being unique</summary>
        /// <remarks>The RP MUST NOT rely upon this value being unique, as discussed in http://openid.net/specs/openid-connect-basic-1_0-32.html#ClaimStability </remarks>
        public const string PreferredUserName = "preferred_username";

        /// <summary>End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322] addr-spec syntax. The relying party MUST NOT rely upon this value being unique</summary>
        public const string Email = "email";
    }
}

import { request } from "http";
import NextAuth from "next-auth"
import "next-auth/jwt"

export const { handlers, auth, signIn, signOut } = NextAuth({
  debug: true,
  theme: { logo: "https://authjs.dev/img/logo-sm.png" },
  providers: [
    {
      id: 'simple-identity-server', // Unique identifier for the provider
      name: 'Simple Identity Server', // Name of the provider
      type: 'oauth', // Provider type
      issuer: "http://localhost:5000",
      authorization: "http://localhost:5000/connect/authorize",
      token: "http://localhost:5000/connect/token",
      userinfo: "http://localhost:5000/connect/userinfo",
      clientId: "client1", // Client ID for authentication from environment variable
      clientSecret: "IEvgIBADANLgkqhkiG9w0BAQEFAASCB", // Client Secret for authentication from environment variable
      checks: ["pkce", "state"], // Security checks to perform
      profile(profile) {
        // Log essential information when a user logs in
        console.log('User logged in', { userId: profile.sub });
        return {
          id: profile.sub, // User ID from the profile
          username: profile.sub?.toLowerCase(), // Username (converted to lowercase)
          name: `${profile.given_name} ${profile.family_name}`, // Full name from given and family names
          email: profile.email, // User email
        };
      }
    },
  ],
})


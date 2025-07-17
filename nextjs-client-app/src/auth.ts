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
      issuer: "http://localhost:5295",
      authorization: "http://localhost:5295/oauth/authorize",
      token: "http://localhost:5295/oauth/token",
      userinfo: "http://localhost:5295/oauth/userinfo",
      clientId: "nextjs_client", // Client ID for authentication from environment variable
      clientSecret: "supersecret123", // Client Secret for authentication from environment variable
      checks: ["pkce", "state"], // Security checks to perform
      profile(profile) {
        // Log essential information when a user logs in
        console.log('User logged in', { userId: profile.sub });
        return {
          id: profile.sub, // User ID from the profile
          username: profile.sub?.toLowerCase(), // Username (converted to lowercase)
          name: `${profile.name}`, // Full name from given and family names
          email: profile.email, // User email
        };
      },
      client: {
        token_endpoint_auth_method: "client_secret_post", // LOOK HERE!!!
      }
    },
  ],
})


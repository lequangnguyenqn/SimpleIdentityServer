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
      issuer: "http://localhost:5295", // Issuer URL from environment variable
      clientId: "test", // Client ID for authentication from environment variable
      clientSecret: "test_secret", // Client Secret for authentication from environment variable
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
  session: { strategy: "jwt" },
  callbacks: {
    jwt({ token, trigger, session, account }) {
      if (trigger === "update") token.name = session.user.name
      return token
    },
    async session({ session, token }) {
      if (token?.accessToken) session.accessToken = token.accessToken

      return session
    },
  },
})

declare module "next-auth" {
  interface Session {
    accessToken?: string
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    accessToken?: string
  }
}
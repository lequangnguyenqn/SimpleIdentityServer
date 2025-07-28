"use server"
import { signIn, signOut } from "auth"
import { Button } from "./ui/button"

export async function SignIn({
  provider,
  ...props
}: { provider?: string } & React.ComponentPropsWithRef<typeof Button>) {
  return (
    <form
      action={async () => {
        "use server"
        await signIn("simple-identity-server")
      }}
    >
      <Button {...props}>Sign In</Button>
    </form>
  )
}

export async function SignOut(props: React.ComponentPropsWithRef<typeof Button>) {
  return (
    <form
      className="w-full"
    >
      <Button onClick={async () => {
        "use server"
        await signOut()
      }} className="w-full p-0" {...props}>
        Sign Out
      </Button>
    </form>
  )
}

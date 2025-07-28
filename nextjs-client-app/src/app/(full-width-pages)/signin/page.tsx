import { signIn } from "auth"
import Button from "@/components/ui/button/Button";


export default async function SignIn() {
  return (
    <form
      action={async () => {
        "use server"
        await signIn("simple-identity-server", {
            redirectTo:'/'
        })
      }}
    >
      <Button className="w-full" size="sm">Sign In</Button>
    </form>
  );
}
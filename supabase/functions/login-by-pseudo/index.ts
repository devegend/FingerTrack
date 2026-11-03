import "@supabase/functions-js/edge-runtime.d.ts";
import { withSupabase } from "npm:@supabase/server@^1";

export default {
    fetch: withSupabase({ auth: "publishable" }, async (request, ctx) => {
        if (request.method !== "POST") {
            return Response.json(
                { error: "Méthode non autorisée." },
                { status: 405 },
            );
        }

        try {
            const body = await request.json();
            const pseudo = String(body.pseudo ?? "").trim();
            const password = String(body.password ?? "");

            if (!pseudo || !password) {
                return Response.json(
                    { error: "Pseudo et mot de passe requis." },
                    { status: 400 },
                );
            }

           
            const { data: profiles, error: profileError } = await ctx.supabaseAdmin
                .from("connexion")
                .select("auth_user_id")
                .eq("pseudo", pseudo)
                .limit(2);

            if (profileError) {
                return Response.json(
                    { error: "Erreur de lecture du compte." },
                    { status: 500 },
                );
            }

            if (!profiles || profiles.length !== 1 || !profiles[0].auth_user_id) {
                return Response.json(
                    { error: "Pseudo ou mot de passe incorrect." },
                    { status: 401 },
                );
            }

            // كنجيب الإيميل المرتبط بحساب Auth
            const { data: userResult, error: userError } =
                await ctx.supabaseAdmin.auth.admin.getUserById(
                    String(profiles[0].auth_user_id),
                );

            const email = userResult?.user?.email;

            if (userError || !email) {
                return Response.json(
                    { error: "Pseudo ou mot de passe incorrect." },
                    { status: 401 },
                );
            }

            // Auth هو اللي كيتحقق من كلمة السر
            const { data: authResult, error: authError } =
                await ctx.supabase.auth.signInWithPassword({ email, password });

            if (authError || !authResult.session) {
                return Response.json(
                    { error: "Pseudo ou mot de passe incorrect." },
                    { status: 401 },
                );
            }

            return Response.json({
                access_token: authResult.session.access_token,
                refresh_token: authResult.session.refresh_token,
            });
        } catch {
            return Response.json(
                { error: "Erreur pendant la connexion." },
                { status: 500 },
            );
        }
    }),
};
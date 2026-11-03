using Supabase.Postgrest.Models;
using PgTableAttribute = Supabase.Postgrest.Attributes.TableAttribute;
using PgPrimaryKeyAttribute = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;
using PgColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;

namespace fingertrack
{
    [PgTableAttribute("connexion")]
    public class ConnexionProfile : BaseModel
    {
        [PgPrimaryKeyAttribute("id")]
        public long Id { get; set; }

        [PgColumnAttribute("auth_user_id")]
        public string AuthUserId { get; set; } = string.Empty;

        [PgColumnAttribute("nom")]
        public string Nom { get; set; } = string.Empty;

        [PgColumnAttribute("poste")]
        public string Poste { get; set; } = string.Empty;

        [PgColumnAttribute("pseudo")]
        public string Pseudo { get; set; } = string.Empty;
    }
}
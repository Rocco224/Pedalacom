using System.ComponentModel.DataAnnotations;

namespace Pedalacom.Models.UserModel
{
    public class User
    {
        [Required(ErrorMessage = "Il campo Email è obbligatorio.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il campo Password è obbligatorio.")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$",
            ErrorMessage = "La password deve contenere almeno una cifra, una lettera minuscola, una lettera maiuscola, un carattere speciale e deve essere lunga almeno 8 caratteri.")]
        public string Password { get; set; }
    }
}

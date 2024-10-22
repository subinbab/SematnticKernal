namespace SemanticAIApp.Models
{
    public class SignUpDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // Role to be assigned
    }
}

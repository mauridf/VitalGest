namespace VitalGest.Core.Enums;

/// <summary>
/// Define os níveis de acesso dos usuários no sistema.
/// </summary>
public enum UserRole
{
    /// <summary>Usuário padrão (médico, enfermeiro, atendente, etc.)</summary>
    User = 1,

    /// <summary>Administrador da clínica</summary>
    Admin = 2,

    /// <summary>Super administrador do sistema (acesso global)</summary>
    SuperAdmin = 3
}
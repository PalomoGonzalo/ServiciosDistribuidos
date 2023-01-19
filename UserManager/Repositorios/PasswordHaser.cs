using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using UserManager.Options;

namespace UserManager.Repositorios
{
    public interface IPasswordHasherRepositorio
    {
         string Hash(string password);
         bool CheckHash(string hash, string password);
    }

    public class PasswordHaserRepositorio : IPasswordHasherRepositorio
    {
        private readonly PassOptions _config;
        public PasswordHaserRepositorio(IOptions<PassOptions> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Checkea el hash, con los parametros definidos de passoptions
        /// que esta en el appsettings
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckHash(string hash, string password)
        {
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException(nameof(password));

            if (String.IsNullOrEmpty(hash))
                throw new ArgumentException(nameof(hash));

            var parts = hash.Split('.', 3);
            if (parts.Length != 3)
            {
                throw new FormatException("Inesperado formato de has");
            }

            var iteraciones = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);


            using (var algorithm = new Rfc2898DeriveBytes(
             password,
             salt,
             iteraciones
            ))
            {
                var keyToCheck = algorithm.GetBytes(_config.KeySize);
                return keyToCheck.SequenceEqual(key);
            }

        }
        /// <summary>
        /// Se crea el hash con los parametros definidos en el appsettings
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Hash(string password)
        {
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException(nameof(password));

            //PBKDF2 IMPLEMENTACION
            using (var algorithm = new Rfc2898DeriveBytes(
             password,
             _config.SaltSize,
             _config.Iteraciones
            ))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(_config.KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);
                return $"{_config.Iteraciones}.{salt}.{key}";
            }
        }
    }
}
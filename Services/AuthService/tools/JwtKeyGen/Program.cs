using System.Security.Cryptography;

var outDir = Directory.GetCurrentDirectory();

using var rsa = RSA.Create(2048);

var privatePem = rsa.ExportPkcs8PrivateKeyPem();
var publicPem = rsa.ExportSubjectPublicKeyInfoPem();

File.WriteAllText(Path.Combine(outDir, "jwt_private.pem"), privatePem);
File.WriteAllText(Path.Combine(outDir, "jwt_public.pem"), publicPem);

Console.WriteLine("Generated jwt_private.pem and jwt_public.pem in: " + outDir);
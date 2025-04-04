﻿// See https://aka.ms/new-console-template for more information

using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

// Console.WriteLine("Hello, World!");
//
// var pfx = new X509Certificate2("./PrivateKeyCert.pfx", "Mister123");
// var privateKeyPfx = pfx.GetRSAPrivateKey();
// var publicKeyPfx = pfx.GetRSAPublicKey();
// var sha256 = new HMACSHA256();
//
// var data = File.ReadAllBytes("./ToEncrypt.txt");
// var sign = GetSign(data, privateKeyPfx, sha256);
// File.WriteAllBytes("./sign.sig",sign);
// var signedData = File.ReadAllBytes("./sign.sig");
// Console.WriteLine($"Signature is valid: {ValidateSign(signedData, data, publicKeyPfx, sha256)}");
// var encryptedData = Encrypt(data, publicKeyPfx);
// File.WriteAllText("./Encrypted.txt",Convert.ToBase64String(encryptedData));
// var dataForDecrypt = Convert.FromBase64String(File.ReadAllText("./Encrypted.txt"));
// var decryptedData = Decrypt(dataForDecrypt, privateKeyPfx);
// Console.WriteLine(Encoding.UTF8.GetString(decryptedData));
// Console.ReadKey();

var senderId = Guid.Parse("89c6aa05-1e2e-405b-a25f-8d4b5a5a5e24");
var recipientId = Guid.Parse("4a298ed9-3448-480a-9bdd-0f623766784d");
var uriBuilder = new UriBuilder("ws://localhost:5091")
{
    Path = "send",
    Query = $"senderId={senderId}&recipientId={recipientId}",
};
var sendPackageClient = new ClientSockets(uriBuilder.Uri.ToString());
await sendPackageClient.ConnectAsync();
await sendPackageClient.SendFilesAsync("./outbound");
await sendPackageClient.CloseConnectionAsync();

static byte[] GetSign(byte[] dataToSign, RSA key, HMACSHA256 sha256)
{
    var hash = sha256.ComputeHash(dataToSign);
    var sign = key.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    return sign;
}


static bool ValidateSign(byte[] sign, byte[]dataToSign, RSA key, HMACSHA256 sha256)
{
    var hash = sha256.ComputeHash(dataToSign);
    return key.VerifyHash(hash, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
}

static byte[] Encrypt(byte[] data, RSA key)
{
    return key.Encrypt(data, RSAEncryptionPadding.Pkcs1);
}

static byte[] Decrypt(byte[] encryptedData, RSA key)
{
    return key.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
}
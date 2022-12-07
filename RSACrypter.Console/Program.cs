using DevAttic.ConfigCrypter.CertificateLoaders;
using DevAttic.ConfigCrypter.Crypters;

const string wrongPassword = "The specified network password is not correct";

string? certificateFile;

do
{
    Console.Write("\nEnter the path to the certificate file or leave blank to terminate: ");
    certificateFile = Console.ReadLine();

    if (!File.Exists(certificateFile))
        Console.WriteLine("\t* File not found!");

} while (!string.IsNullOrEmpty(certificateFile) && !File.Exists(certificateFile));

if (File.Exists(certificateFile))
{
    FilesystemCertificateLoader? loader = null;
    try
    {
        loader = new FilesystemCertificateLoader(certificateFile);
        loader.LoadCertificate();
    }
    catch (Exception e)
    {
        if (e.Message.Contains(wrongPassword))
        {
            string? certificatePassword;

            Console.WriteLine("\nImportant! The certificate file is password protected!");

            bool validPassword = false;

            do
            {
                Console.Write("\nEnter the password for the certificate file or leave blank to terminate: ");
                certificatePassword = Console.ReadLine();

                if (!string.IsNullOrEmpty(certificatePassword))
                {
                    try
                    {
                        loader = new FilesystemCertificateLoader(certificateFile, certificatePassword);
                        loader.LoadCertificate();
                        validPassword = true;
                    }
                    catch (Exception f)
                    {
                        if (e.Message.Contains(wrongPassword))
                        {
                            Console.WriteLine("\t* Incorrect password!");
                            validPassword = false;
                        }
                        else
                            Environment.Exit(-1);
                    }
                }

            } while (!string.IsNullOrEmpty(certificatePassword) && !validPassword);
        }
        else
            Environment.Exit(-1);
    }

    if (loader != null)
    {
        using var crypter = new RSACrypter(loader);

        string? valueToEncrypt;

        do
        {
            Console.Write("\nType text to encrypt or leave blank to terminate: ");
            valueToEncrypt = Console.ReadLine();

            if (!string.IsNullOrEmpty(valueToEncrypt))
                Console.WriteLine(crypter.EncryptString(valueToEncrypt));

        } while (!string.IsNullOrEmpty(valueToEncrypt));
    }
}